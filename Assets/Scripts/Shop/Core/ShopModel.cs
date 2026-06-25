using System;
using FlightIGuess.Ship.Core;
using FlightIGuess.Weapons.Core;
using NUnit.Framework;

namespace FlightIGuess.Shop.Core
{
    public struct ShopItem
    {
        public string ItemName;
        public int Cost;
        public bool IsWeapon;
        public HardpointSize WeaponSize;
        public HullTier HullUpgradeTier;

        public ShopItem(string itemName, int cost, bool isWeapon, HardpointSize weaponSize, HullTier hullTier)
        {
            ItemName = itemName;
            Cost = cost;
            IsWeapon = isWeapon;
            WeaponSize = weaponSize;
            HullUpgradeTier = hullTier; 
        }
    }


    /// <summary>
    /// Pure C# model that validates transactions (Scrap cost, Hardpoint Size constraints) 
    /// before invoking purchase success events for the UI.
    /// </summary>
    public class ShopModel
    {
        public event Action<ShopItem> OnPurchaseSuccess;
        public event Action<string> OnPurchaseFailed;
        private RunStateModel _runStateModel;
        public RunStateModel RunStateModel => _runStateModel;
        private ShipModel _shipModel;
        
        public ShopModel(RunStateModel runStateModel, ShipModel shipModel)
        {
            _runStateModel = runStateModel;
            _shipModel = shipModel;
        }

        public bool TryBuyItem(ShopItem item)
        {
            if (_runStateModel.CurrentScrap < item.Cost)
            {
                OnPurchaseFailed?.Invoke("Not enough scrap!");
                return false;
            }

            if (item.IsWeapon && !CheckValidWeapon(item))
            {
               return false;
            }

            // If we get here, the purchase is valid
            _runStateModel.SpendScrap(item.Cost);
            OnPurchaseSuccess?.Invoke(item);
            return true;
        }

        private bool CheckValidWeapon(ShopItem item)
        {
            // Check if the ship is large enough to mount this weapon
            if ((int)item.WeaponSize > (int)_shipModel.Stats.HullTier)
            {
                OnPurchaseFailed?.Invoke($"Ship Hull too small for {item.WeaponSize} weapon.");
                return false;
            }

            // Check if there is an empty hardpoint of the correct size
            bool hasEmptySlot = false;
            foreach (var hardpoint in _shipModel.Stats.ActiveHardPoint)
            {
                if (hardpoint.EquippedWeapon == null && (int)hardpoint.SlotSize >= (int)item.WeaponSize)
                {
                    hasEmptySlot = true;
                    break;
                }
            }

            if (!hasEmptySlot)
            {
                OnPurchaseFailed?.Invoke("No empty hardpoints of required size.");
                return false;
            }
            return true;
        }
    }
}
