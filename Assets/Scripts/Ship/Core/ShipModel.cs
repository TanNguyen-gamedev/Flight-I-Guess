using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using FlightIGuess.Weapons.Core;

using FlightIGuess.Combat.Core;

namespace FlightIGuess.Ship.Core
{
    public struct ShipModelStats
    {
        public float MaxHP;
        public float Acceleration;
        public float MaxSpeed;
        public int TurnRate;
        public HullTier HullTier;
        public List<HardpointModel> ActiveHardPoint;

    }


    /// <summary>
    /// Pure C# model tracking hull stats, current tier, and hardpoints.
    /// Manages the pure C# logic of expanding hardpoints during a hull upgrade.
    /// </summary>
    public class ShipModel
    {
        public ShipModelStats Stats;
        public HealthModel Health { get; private set; }
        public HeatModel Heat { get; private set; }
        
        public event Action<ShipModelStats> OnHullUpgrade;
        public event Action<float, Vector2> OnShipRecoil;

        private List<HardpointModel> _activeHardPoint;
        public ShipModel(ShipModelStats stats, HealthModel healthModel, HeatModel heatModel)
        {
            _activeHardPoint = new List<HardpointModel>();
            Stats = stats;
            Health = healthModel;
            Heat = heatModel;
            Stats.ActiveHardPoint = _activeHardPoint;
        }

        public void TrackHardPoint(List<HardpointModel> hardpointModels)
        {
            foreach(var hardpoint in hardpointModels)
            {
                if((int)hardpoint.SlotSize > (int)Stats.HullTier)
                {
                    throw new InvalidOperationException($"Cannot mount {hardpoint.SlotSize} weapon on {Stats.HullTier} hull.");
                }
                else
                {
                    _activeHardPoint.Add(hardpoint);
                    // Use a wrapper/lambda so we don't try to assign event to event
                    hardpoint.OnWeaponFired -= HandleWeaponFired; // Prevent double subscription
                    hardpoint.OnWeaponFired += HandleWeaponFired;
                }
            }
            Stats.ActiveHardPoint = _activeHardPoint;
        }

        private void HandleWeaponFired(float recoilForce, Vector2 direction)
        {
            OnShipRecoil?.Invoke(recoilForce, direction);
        }
        public void UpgradeHull(ShipModelStats stats)
        {  
            _activeHardPoint.Clear();
            TrackHardPoint(stats.ActiveHardPoint);

            Stats = stats;
            Stats.ActiveHardPoint = _activeHardPoint;
            OnHullUpgrade?.Invoke(Stats);
        }


    }
}
