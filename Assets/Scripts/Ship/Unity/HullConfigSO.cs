using UnityEngine;
using FlightIGuess.Ship.Core;
using FlightIGuess.Combat.Core;

namespace FlightIGuess.Ship.Unity
{
    [CreateAssetMenu(fileName = "NewHullConfig", menuName = "FlightIGuess/Ship/Hull Config")]
    public class HullConfigSO : ScriptableObject
    {
        [Header("Hull Stats")]
        public HullTier Tier;
        public float MaxHP = 100f;
        public float MaxShield = 100f;
        public float ShieldRegenRate = 10f;
        public float ShieldRegenDelay = 5f;
        public float MaxSpeed = 15f;
        public float Acceleration = 20f;
        public int TurnRate = 180;

        [Header("Heat & Boost Settings")]
        public float MaxHeat = 100f;
        public float HeatBuildRate = 35f; // Heat per second while boosting
        public float HeatCoolRate = 15f;  // Heat per second while not boosting
        public float OverheatPenaltyMultiplier = 2f; // Cooldown is slower if overheated

        public ShipModel CreateShipModel()
        {
            var healthModel = new HealthModel(MaxHP, MaxShield, ShieldRegenDelay, ShieldRegenRate);
            var heatModel = new HeatModel(MaxHeat, HeatBuildRate, HeatCoolRate, OverheatPenaltyMultiplier);
            var stats = new ShipModelStats
            {
                HullTier = Tier,
                MaxHP = MaxHP,
                Acceleration = Acceleration,
                TurnRate = TurnRate,
                MaxSpeed = MaxSpeed,
                ActiveHardPoint = new System.Collections.Generic.List<Weapons.Core.HardpointModel>()
            };
            return new ShipModel(stats, healthModel, heatModel);
        }
    }
}