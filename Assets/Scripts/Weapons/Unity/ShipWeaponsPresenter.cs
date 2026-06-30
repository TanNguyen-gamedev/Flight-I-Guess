using System.Collections.Generic;
using UnityEngine;
using FlightIGuess.Weapons.Core;
using SysNum = System.Numerics;
using UnityEngine.InputSystem;
using PrimeTween;
using FlightIGuess.Core;

namespace FlightIGuess.Weapons.Unity
{
    /// <summary>
    /// Humble Object that connects the pure C# WeaponModels to Unity's Update loop and Input.
    /// </summary>
    public class ShipWeaponsPresenter : MonoBehaviour
    {
        [SerializeField] private HardpointAuthoring[] _hardpointAuthorings;
        [SerializeField] private ProjectilePoolManager _projectilePoolManager;
        [SerializeField] private EffectPoolManager _effectPoolManager;
        
        // In a real game, this would be injected or read from an InputManager.
        // For now, we read directly from the new Input System asset.
        private InputSystem_Actions _inputSystem;
        
        private List<HardpointModel> _hardpointModels;
        public List<HardpointModel> HardpointModels => _hardpointModels;
        private Vector2 _mousePosition;
        private bool _isFiring = false;
        private Dictionary<string, HardpointAuthoring> _visualBinding;

        private void Awake()
        {
            _inputSystem = new InputSystem_Actions();
            _hardpointModels = new List<HardpointModel>();
            _visualBinding = new Dictionary<string, HardpointAuthoring>();
            _projectilePoolManager = Bootstrapper.Instance.GetManager<ProjectilePoolManager>();
            _effectPoolManager = Bootstrapper.Instance.GetManager<EffectPoolManager>();

            // Initialize models
            foreach (var authoring in _hardpointAuthorings)
            {
                var hardpointModel = new HardpointModel(authoring.HardpointId, authoring.InitialAngle);
                hardpointModel.SlotSize = authoring.SlotSize;
                
                // If the authoring component has an initial weapon configured, equip it.
                if (authoring.InitialWeapon != null)
                {
                    var weapon = authoring.InitialWeapon.CreateWeaponModel();
                    hardpointModel.Equip(weapon);
                    hardpointModel.TurnRateDegreesPerSecond = authoring.InitialWeapon.TurnRateDegreesPerSecond;
                    hardpointModel.ArcRangeDegrees = authoring.ArcRangeDegrees;
                    
                    // Pass the config to the authoring component so it knows its visual recoil profile
                    authoring.SetWeaponConfig(authoring.InitialWeapon);
                }
                
                _hardpointModels.Add(hardpointModel);
                _visualBinding.Add(authoring.HardpointId, authoring);

                // Subscribe to the model's OnWeaponFired event to play visual recoil
                hardpointModel.OnWeaponFired += authoring.PlayRecoilTween;
            }
        }

        private void OnEnable()
        {
            _inputSystem.Enable();
            _inputSystem.Player.Attack.performed += OnAttack;
            _inputSystem.Player.Attack.canceled += OnAttack;
            _inputSystem.Player.MousePosition.performed += OnMouseMove;
        }

        private void OnDisable()
        {
            _inputSystem.Player.MousePosition.canceled -= OnMouseMove;
            _inputSystem.Player.Attack.performed -= OnAttack;
            _inputSystem.Player.Attack.canceled -= OnAttack;
            _inputSystem.Disable();
        }

        private void FixedUpdate()
        {
            // The prompt requests physics/movement in FixedUpdate. Projectile spawning is physics-adjacent.
            // We read the input state manually (is the button held?)
            float dt = Time.fixedDeltaTime;

            for (int i = 0; i < _hardpointModels.Count; i++)
            {
                var model = _hardpointModels[i];
                var authoring = _hardpointAuthorings[i];

                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(_mousePosition);
                mouseWorldPos.z = 0f;
                Vector2 aimDirection = mouseWorldPos - authoring.transform.position;
                
                // Get the ship's current rotation (Z axis in 2D)
                float shipRotation = transform.rotation.eulerAngles.z;
                
                // 1. Pass the target direction and ship rotation to the Pure C# model
                var targetDir = new SysNum.Vector2(aimDirection.x, aimDirection.y);
                model.AimTowards(targetDir, dt, shipRotation);

                // 2. Read the calculated angle back and apply it to the Unity Transform
                authoring.transform.rotation = Quaternion.Euler(0, 0, model.CurrentAngleDegrees);

                var pos = new SysNum.Vector2(authoring.Position.x, authoring.Position.y);

                model.Tick(dt, _isFiring, _projectilePoolManager, _effectPoolManager, pos);
            }
        }

        private void OnMouseMove(InputAction.CallbackContext callback)
        {
            _mousePosition = callback.ReadValue<Vector2>();
        }

        private void OnAttack(InputAction.CallbackContext callback)
        {
            _isFiring = callback.performed;
        }

        public void OnWeaponPurchased(WeaponConfigSO purchasedWeapon)
        {
            foreach(var hardpointModel in _hardpointModels)
            {
                // Must check if the slot is empty AND large enough for the weapon!
                if(hardpointModel.EquippedWeapon == null && (int)hardpointModel.SlotSize >= (int)purchasedWeapon.WeaponSize)
                {
                    WeaponModel weapon = purchasedWeapon.CreateWeaponModel();
                    hardpointModel.Equip(weapon);
                    if(_visualBinding.TryGetValue(hardpointModel.HardpointId, out var authoring))
                    {
                        authoring.SetWeaponConfig(purchasedWeapon);
                    }
                    hardpointModel.TurnRateDegreesPerSecond = purchasedWeapon.TurnRateDegreesPerSecond;
                    hardpointModel.ArcRangeDegrees = authoring.ArcRangeDegrees;

                    return; // Stop after equipping in the first valid slot
                }
            }
        }

    }
}