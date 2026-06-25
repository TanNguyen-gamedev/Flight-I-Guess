using System;
using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Pure C# model representing a physical slot on the ship where a weapon can be mounted.
    /// </summary>
    public class HardpointModel
    {
        public event Action OnWeaponFired;

        public string HardpointId { get; }
        public HardpointSize SlotSize;
        public WeaponModel EquippedWeapon { get; private set; }
        
        // Turret rotation properties
        public float TurnRateDegreesPerSecond { get; set; } = 360f; // Default turn rate
        public float CurrentAngleDegrees { get; private set; }

        // Firing Arc constraints
        public float ArcCenterDegrees { get; set; }
        public float ArcRangeDegrees { get; set; } = 360f; // Default is full 360 rotation

        public HardpointModel(string hardpointId, float initialAngleDegrees)
        {
            HardpointId = hardpointId;
            CurrentAngleDegrees = initialAngleDegrees;
            ArcCenterDegrees = initialAngleDegrees;
        }

        public void Equip(WeaponModel weapon)
        {
            if (EquippedWeapon != null)
            {
                EquippedWeapon.OnFired -= HandleWeaponFired;
            }

            EquippedWeapon = weapon;
            
            if (EquippedWeapon != null)
            {
                EquippedWeapon.OnFired += HandleWeaponFired;
            }
        }

        private void HandleWeaponFired()
        {
            OnWeaponFired?.Invoke();
        }

        public void AimTowards(Vector2 targetDirection, float deltaTime, float shipRotationDegrees)
        {
            if (targetDirection == Vector2.Zero) return;

            // Calculate target angle (Atan2 returns radians)
            float targetAngleRadians = (float)Math.Atan2(targetDirection.Y, targetDirection.X);
            float targetAngleDegrees = targetAngleRadians * (180f / (float)Math.PI);
            
            // Subtract 90 because 2D top-down sprites face UP (+Y) by default
            targetAngleDegrees -= 90f;
            targetAngleDegrees = NormalizeAngle(targetAngleDegrees);

            // Calculate the dynamic arc center based on the ship's current rotation
            float currentArcCenter = NormalizeAngle(ArcCenterDegrees + shipRotationDegrees);

            // Apply Arc Constraints
            if (ArcRangeDegrees < 360f)
            {
                float halfArc = ArcRangeDegrees / 2f;
                float angleDiffFromCenter = DeltaAngle(currentArcCenter, targetAngleDegrees);
                
                // Clamp target angle within the arc
                if (angleDiffFromCenter > halfArc)
                {
                    targetAngleDegrees = currentArcCenter + halfArc;
                }
                else if (angleDiffFromCenter < -halfArc)
                {
                    targetAngleDegrees = currentArcCenter - halfArc;
                }
            }

            // Move current angle towards target angle
            float delta = DeltaAngle(CurrentAngleDegrees, targetAngleDegrees);
            float maxDelta = TurnRateDegreesPerSecond * deltaTime;

            if (Math.Abs(delta) <= maxDelta)
            {
                CurrentAngleDegrees = targetAngleDegrees;
            }
            else
            {
                CurrentAngleDegrees += Math.Sign(delta) * maxDelta;
            }
            
            CurrentAngleDegrees = NormalizeAngle(CurrentAngleDegrees);
        }

        private float DeltaAngle(float current, float target)
        {
            float delta = (target - current) % 360.0f;
            if (delta > 180.0f) delta -= 360.0f;
            if (delta < -180.0f) delta += 360.0f;
            return delta;
        }

        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }

        public void Tick(float deltaTime, bool isInputHeld, IProjectileSpawner projectileSpawner, IEffectSpawner effectSpawner, Vector2 currentPos)
        {
            // Calculate current direction vector based on our current angle
            // Add 90 back to convert from our sprite-adjusted angle to standard math angle
            float mathAngleRadians = (CurrentAngleDegrees + 90f) * ((float)Math.PI / 180f);
            var currentDir = new Vector2(
                (float)Math.Cos(mathAngleRadians),
                (float)Math.Sin(mathAngleRadians)
            );

            EquippedWeapon?.Tick(deltaTime, isInputHeld, projectileSpawner, effectSpawner, currentPos, currentDir);
        }
    }
}