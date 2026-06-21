using System.Collections.Generic;
using UnityEngine;
using FlightIGuess.Weapons.Core;
using SysNum = System.Numerics;
using UnityEngine.InputSystem;

namespace FlightIGuess.Weapons.Unity
{
    /// <summary>
    /// Humble Object that connects the pure C# WeaponModels to Unity's Update loop and Input.
    /// </summary>
    public class ShipWeaponsPresenter : MonoBehaviour
    {
        [SerializeField] private HardpointAuthoring[] _hardpointAuthorings;
        [SerializeField] private ProjectilePoolManager _poolManager;
        
        // In a real game, this would be injected or read from an InputManager.
        // For now, we read directly from the new Input System asset.
        private InputSystem_Actions _inputSystem;
        
        private List<HardpointModel> _hardpointModels;

        private void Awake()
        {
            _inputSystem = new InputSystem_Actions();
            _hardpointModels = new List<HardpointModel>();

            // Initialize models
            foreach (var authoring in _hardpointAuthorings)
            {
                var hardpointModel = new HardpointModel(authoring.HardpointId);
                
                // --- MOCK WEAPON SETUP ---
                // Normally, a LoadoutManager would equip these from SaveData or ScriptableObjects.
                // We inject a basic weapon directly here to prove the architecture.
                var trigger = new AutoFireTrigger(fireRatePerSecond: 5f); // 5 shots per second
                var emitter = new SingleShotEmitter();
                var weapon = new WeaponModel(trigger, emitter, "BasicLaser");
                
                hardpointModel.Equip(weapon);
                // -------------------------

                _hardpointModels.Add(hardpointModel);
            }
        }

        private void OnEnable()
        {
            _inputSystem.Enable();
        }

        private void OnDisable()
        {
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

                var pos = new SysNum.Vector2(authoring.Position.x, authoring.Position.y);
                var dir = new SysNum.Vector2(authoring.Direction.x, authoring.Direction.y);

                model.Tick(dt, isFiring, _poolManager, pos, dir);
            }
        }
    }
}
