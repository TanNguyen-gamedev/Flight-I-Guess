using UnityEngine;
using FlightIGuess.Combat.Core;
using FlightIGuess.Ship.Core;
using FlightIGuess.Core;

namespace FlightIGuess.Combat.Unity
{
    /// <summary>
    /// Unity presenter that bridges the pure C# CombatManager with Unity's lifecycle.
    /// </summary>
    public class CombatPresenter : MonoBehaviour
    {
        [Header("Damage Settings")]
        [Tooltip("Minimum relative velocity required to deal collision damage.")]
        [SerializeField] private float _minimumImpactVelocity = 5f;
        
        [Tooltip("Multiplier applied to mass * velocity to calculate final damage.")]
        [SerializeField] private float _damageMultiplier = 0.5f;

        private CombatManager _combatManager;

        private void Awake()
        {
            if (Bootstrapper.Instance != null)
            {
                Bootstrapper.Instance.Register(this);
            }
        }

        public void Init(CombatManager combatManager)
        {
            _combatManager = combatManager;
            _combatManager.SetDamageConfig(_minimumImpactVelocity, _damageMultiplier);
            _combatManager.Initialize();
        }

        private void Update()
        {
            if (_combatManager != null)
            {
                _combatManager.Tick(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            if (Bootstrapper.Instance != null)
            {
                Bootstrapper.Instance.Unregister(this);
            }

            if (_combatManager != null)
            {
                _combatManager.Dispose();
            }
        }
        
        // Helper to register entities from other presenters if needed
        public void RegisterEntity(HealthModel healthModel)
        {
            _combatManager?.Register(healthModel);
        }

        public void UnregisterEntity(HealthModel healthModel)
        {
            _combatManager?.Unregister(healthModel);
        }
    }
}