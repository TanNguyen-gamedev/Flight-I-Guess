using UnityEngine;
using FlightIGuess.Weapons.Core;

namespace FlightIGuess.Weapons.Unity
{
    public enum TriggerType
    {
        AutoFire,
        Charge
    }

    public enum EmitterType 
    { 
        SingleShot, 
        Spread 
    }

    /// <summary>
    /// Unity data container for Weapon statistics. 
    /// Acts as a factory to generate the pure C# WeaponModel for the Core domain.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "FlightIGuess/Weapons/Weapon Config")]
    public class WeaponConfigSO : ScriptableObject
    {
        [Header("Identity")]
        public string WeaponId = "DefaultWeapon";

        [Header("Visuals & Projectile")]
        [Tooltip("The ID of the projectile to spawn from the ProjectilePoolManager")]
        public string ProjectileId;
        
        [Tooltip("The ID of the effect to spawn from the EffectPoolManager")]
        public string EffectId;

        [Header("Trigger Settings")]
        public TriggerType TriggerType = TriggerType.AutoFire;

        [Tooltip("Used if TriggerType is AutoFire")]
        public float FireRatePerSecond = 5f;

        [Tooltip("Used if TriggerType is Charge")]
        public float ChargeTimeSeconds = 1f;

        [Header("Emitter Settings")]
        public EmitterType EmitterType = EmitterType.SingleShot;
        
        [Tooltip("Only used if EmitterType is Spread")]
        public int ProjectileCount = 3;
        
        [Tooltip("Only used if EmitterType is Spread")]
        public float SpreadAngleDegrees = 30f;

        [Header("Turret Settings")]
        [Tooltip("How fast the hardpoint can rotate when this weapon is equipped (degrees per second)")]
        public float TurnRateDegreesPerSecond = 360f;

        /// <summary>
        /// Factory method to create the pure C# model from this Unity data.
        /// </summary>
        public WeaponModel CreateWeaponModel()
        {
            // Create the Trigger based on Enum selection
            IWeaponTrigger trigger = TriggerType switch
            {
                TriggerType.AutoFire => new AutoFireTrigger(FireRatePerSecond),
                TriggerType.Charge => new ChargeTrigger(ChargeTimeSeconds),
                _ => new AutoFireTrigger(FireRatePerSecond)
            };
            
            // Create the Emitter based on Enum selection
            IWeaponEmitter emitter = EmitterType switch
            {
                EmitterType.SingleShot => new SingleShotEmitter(),
                EmitterType.Spread => new SpreadEmitter(ProjectileCount, SpreadAngleDegrees),
                _ => new SingleShotEmitter()
            };

            return new WeaponModel(trigger, emitter, ProjectileId, EffectId);
        }
    }
}
