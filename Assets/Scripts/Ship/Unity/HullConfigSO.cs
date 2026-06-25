using UnityEngine;
using FlightIGuess.Ship.Core;

namespace FlightIGuess.Ship.Unity
{
    [CreateAssetMenu(fileName = "NewHullConfig", menuName = "FlightIGuess/Ship/Hull Config")]
    public class HullConfigSO : ScriptableObject
    {
        public HullTier Tier;
        public float MaxHP = 100f;
        public float ThrustForce = 1000f;
        public int TurnRate = 180;

        public ShipModel CreateShipModel()
        {
            var stats = new ShipModelStats
            {
                HullTier = Tier,
                MaxHP = MaxHP,
                ThrustForce = ThrustForce,
                TurnRate = TurnRate,
                ActiveHardPoint = new System.Collections.Generic.List<Weapons.Core.HardpointModel>()
            };
            return new ShipModel(stats);
        }
    }
}