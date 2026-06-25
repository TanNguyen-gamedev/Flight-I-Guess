using System;
using System.Collections.Generic;
using System.Diagnostics;
using FlightIGuess.Weapons.Core;

namespace FlightIGuess.Ship.Core
{
    public struct ShipModelStats
    {
        public float MaxHP;
        public float ThrustForce;
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
        // TODO: Implement stats (MaxHP, Thrust, TurnRate)
        public ShipModelStats Stats;
        public event Action<ShipModelStats> OnHullUpgrade;

        // TODO: Implement hardpoint tracking based on HullTier
        private List<HardpointModel> _activeHardPoint;
        public ShipModel(ShipModelStats stats)
        {
            _activeHardPoint = new List<HardpointModel>();
            Stats = stats;
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
                }
            }
            Stats.ActiveHardPoint = _activeHardPoint;
        }
        // TODO: Implement UpgradeHull() logic
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
