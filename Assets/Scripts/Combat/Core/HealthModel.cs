using System;
using System.Numerics;

namespace FlightIGuess.Combat.Core
{
    public class HealthModel
    {
        public float MaxHull { get; private set; }
        public float CurrentHull { get; private set; }
        
        public float MaxShield { get; private set; }
        public float CurrentShield { get; private set; }

        public float ShieldRegenDelay { get; private set; }
        public float ShieldRegenRate { get; private set; }

        private float _timeSinceLastDamage;

        public event Action<float, float> OnHullChanged;
        public event Action<float, float> OnShieldChanged;
        public event Action OnDeath;

        public HealthModel(float maxHull, float maxShield, float shieldRegenDelay, float shieldRegenRate)
        {
            MaxHull = maxHull;
            CurrentHull = maxHull;
            
            MaxShield = maxShield;
            CurrentShield = maxShield;
            
            ShieldRegenDelay = shieldRegenDelay;
            ShieldRegenRate = shieldRegenRate;
            
            _timeSinceLastDamage = 0f;
        }

        public void ApplyDamage(float amount)
        {
            if (CurrentHull <= 0) 
            {
                return; // Already dead
            }
            _timeSinceLastDamage = 0f; // Reset regen timer

            if (CurrentShield > 0)
            {
                if (amount <= CurrentShield)
                {
                    CurrentShield -= amount;
                    amount = 0;
                }
                else
                {
                    amount -= CurrentShield;
                    CurrentShield = 0;
                }
                OnShieldChanged?.Invoke(CurrentShield, MaxShield);
            }

            if (amount > 0)
            {
                CurrentHull -= amount;
                if (CurrentHull < 0) CurrentHull = 0;
                OnHullChanged?.Invoke(CurrentHull, MaxHull);

                if (CurrentHull <= 0)
                {
                    OnDeath?.Invoke();
                }
            }
        }

        public void Tick(float dt)
        {
            if (CurrentHull <= 0) return; // Dead entities don't regen
            
            if (CurrentShield >= MaxShield) return; // Shield is full

            _timeSinceLastDamage += dt;

            if (_timeSinceLastDamage >= ShieldRegenDelay)
            {
                CurrentShield += ShieldRegenRate * dt;
                if (CurrentShield > MaxShield)
                {
                    CurrentShield = MaxShield;
                }
                OnShieldChanged?.Invoke(CurrentShield, MaxShield);
            }
        }

        public void FullyRestoreHull()
        {
            CurrentHull = MaxHull;
            OnHullChanged?.Invoke(CurrentHull, MaxHull);
        }
    }
}