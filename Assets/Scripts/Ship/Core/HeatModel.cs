using System;

namespace FlightIGuess.Ship.Core
{
    public class HeatModel
    {
        public float MaxHeat { get; private set; }
        public float CurrentHeat { get; private set; }
        public bool IsOverheated { get; private set; }

        public float HeatBuildRate { get; private set; }
        public float HeatCoolRate { get; private set; }
        public float OverheatPenaltyMultiplier { get; private set; }

        public event Action<float, float> OnHeatChanged;
        public event Action<bool> OnOverheatStateChanged;

        public HeatModel(float maxHeat, float heatBuildRate, float heatCoolRate, float overheatPenaltyMultiplier)
        {
            MaxHeat = maxHeat;
            CurrentHeat = 0f;
            IsOverheated = false;

            HeatBuildRate = heatBuildRate;
            HeatCoolRate = heatCoolRate;
            OverheatPenaltyMultiplier = overheatPenaltyMultiplier;
        }

        public void Tick(float dt, bool isAddingHeat)
        {
            if (isAddingHeat && !IsOverheated)
            {
                CurrentHeat += HeatBuildRate * dt;
                
                if (CurrentHeat >= MaxHeat)
                {
                    CurrentHeat = MaxHeat;
                    IsOverheated = true;
                    OnOverheatStateChanged?.Invoke(IsOverheated);
                }
                
                OnHeatChanged?.Invoke(CurrentHeat, MaxHeat);
            }
            else
            {
                if (CurrentHeat > 0)
                {
                    float coolingRate = IsOverheated ? (HeatCoolRate / OverheatPenaltyMultiplier) : HeatCoolRate;
                    CurrentHeat -= coolingRate * dt;
                    
                    if (CurrentHeat <= 0)
                    {
                        CurrentHeat = 0;
                        if (IsOverheated)
                        {
                            IsOverheated = false;
                            OnOverheatStateChanged?.Invoke(IsOverheated);
                        }
                    }
                    
                    OnHeatChanged?.Invoke(CurrentHeat, MaxHeat);
                }
            }
        }
    }
}