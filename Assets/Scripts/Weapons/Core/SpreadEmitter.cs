using System;
using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Emits multiple projectiles in a cone/spread pattern.
    /// </summary>
    public class SpreadEmitter : IWeaponEmitter
    {
        private readonly int _projectileCount;
        private readonly float _spreadAngleDegrees;

        public SpreadEmitter(int projectileCount, float spreadAngleDegrees)
        {
            _projectileCount = Math.Max(1, projectileCount);
            _spreadAngleDegrees = spreadAngleDegrees;
        }

        public void Emit(IProjectileSpawner projectileSpawner, IEffectSpawner effectSpawner, string projectileId, string effectId, Vector2 origin, Vector2 baseDirection)
        {
            if (!string.IsNullOrEmpty(effectId))
            {
                effectSpawner?.Spawn(effectId, origin, baseDirection);
            }

            if (_projectileCount == 1)
            {
                projectileSpawner.Spawn(projectileId, origin, baseDirection);
                return;
            }

            // Calculate base angle in radians
            float baseAngle = (float)Math.Atan2(baseDirection.Y, baseDirection.X);
            float spreadRadians = _spreadAngleDegrees * ((float)Math.PI / 180f);
            
            float startAngle = baseAngle - (spreadRadians / 2f);
            float angleStep = spreadRadians / (_projectileCount - 1);

            for (int i = 0; i < _projectileCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                
                var spreadDirection = new Vector2(
                    (float)Math.Cos(currentAngle),
                    (float)Math.Sin(currentAngle)
                );

                projectileSpawner.Spawn(projectileId, origin, spreadDirection);
            }
        }
    }
}