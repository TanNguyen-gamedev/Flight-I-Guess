using System.Collections.Generic;
using UnityEngine;
using FlightIGuess.Weapons.Core;
using SysNum = System.Numerics;
using UnityEngine.InputSystem;
using PrimeTween;

namespace FlightIGuess.Weapons.Unity
{
    /// <summary>
    /// Humble Object that connects the pure C# WeaponModels to Unity's Update loop and Input.
    /// </summary>
    public class ShipWeaponsPresenter : MonoBehaviour
    {
        [SerializeField] private HardpointAuthoring[] _hardpointAuthorings;
        [SerializeField] private ProjectilePoolManager _poolManager;
        [SerializeField] private EffectPoolManager _effectPoolManager;
        
        // In a real game, this would be injected or read from an InputManager.
        // For now, we read directly from the new Input System asset.
        private InputSystem_Actions _inputSystem;
        
        private List<HardpointModel> _hardpointModels;
        private Vector2 _mousePosition;

        private void Awake()
        {
            _inputSystem = new InputSystem_Actions();
            _hardpointModels = new List<HardpointModel>();

            // Initialize models
            foreach (var authoring in _hardpointAuthorings)
            {
                var hardpointModel = new HardpointModel(authoring.HardpointId, authoring.InitialAngle);
                
                // If the authoring component has an initial weapon configured, equip it.
                if (authoring.InitialWeapon != null)
                {
                    var weapon = authoring.InitialWeapon.CreateWeaponModel();
                    hardpointModel.Equip(weapon);
                    hardpointModel.TurnRateDegreesPerSecond = authoring.InitialWeapon.TurnRateDegreesPerSecond;
                    
                    // Pass the config to the authoring component so it knows its visual recoil profile
                    authoring.SetWeaponConfig(authoring.InitialWeapon);
                }
                
                _hardpointModels.Add(hardpointModel);

                // Subscribe to the model's OnWeaponFired event to play visual recoil
                hardpointModel.OnWeaponFired += authoring.PlayRecoilTween;
            }
        }

        private void OnEnable()
        {
            _inputSystem.Enable();
            _inputSystem.Player.MousePosition.performed += OnMouseMove;
        }

        private void OnDisable()
        {
            _inputSystem.Player.MousePosition.canceled -= OnMouseMove;
            _inputSystem.Disable();
        }

        private void FixedUpdate()
        {
            // The prompt requests physics/movement in FixedUpdate. Projectile spawning is physics-adjacent.
            // We read the input state manually (is the button held?)
            bool isFiring = _inputSystem.Player.Attack.IsPressed();
            float dt = Time.fixedDeltaTime;

            for (int i = 0; i < _hardpointModels.Count; i++)
            {
                var model = _hardpointModels[i];
                var authoring = _hardpointAuthorings[i];

                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(_mousePosition);
                mouseWorldPos.z = 0f;
                Vector2 aimDirection = mouseWorldPos - authoring.transform.position;
                
                // 1. Pass the target direction to the Pure C# model so it can do the rotation math
                var targetDir = new SysNum.Vector2(aimDirection.x, aimDirection.y);
                model.AimTowards(targetDir, dt);

                // 2. Read the calculated angle back and apply it to the Unity Transform
                authoring.transform.rotation = Quaternion.Euler(0, 0, model.CurrentAngleDegrees);

                var pos = new SysNum.Vector2(authoring.Position.x, authoring.Position.y);

                model.Tick(dt, isFiring, _poolManager, _effectPoolManager, pos);
            }
        }

        private void OnMouseMove(InputAction.CallbackContext callback)
        {
            _mousePosition = callback.ReadValue<Vector2>();
        }

    }
}