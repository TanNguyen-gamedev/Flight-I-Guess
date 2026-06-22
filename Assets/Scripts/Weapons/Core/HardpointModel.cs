using System;
using System.Diagnostics;
using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Pure C# model representing a physical slot on the ship where a weapon can be mounted.
    /// </summary>
    public class HardpointModel
    {
        public string HardpointId { get; }
        public WeaponModel EquippedWeapon { get; private set; }
        
        // Turret rotation properties
        public float TurnRateDegreesPerSecond { get; set; } = 360f; // Default turn rate
        public float CurrentAngleDegrees { get; private set; }

        public HardpointModel(string hardpointId, float initialAngleDegrees)
        {
            HardpointId = hardpointId;
            CurrentAngleDegrees = initialAngleDegrees;
        }

        public void Equip(WeaponModel weapon)
        {
            EquippedWeapon = weapon;
        }

        public void AimTowards(Vector2 targetDirection, float deltaTime)
        {
            if (targetDirection == Vector2.Zero) return;

            // Calculate target angle (Atan2 returns radians)
            float targetAngleRadians = (float)Math.Atan2(targetDirection.Y, targetDirection.X);
            float targetAngleDegrees = targetAngleRadians * (180f / (float)Math.PI);
            
            // Subtract 90 because 2D top-down sprites face UP (+Y) by default
            targetAngleDegrees -= 90f;

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