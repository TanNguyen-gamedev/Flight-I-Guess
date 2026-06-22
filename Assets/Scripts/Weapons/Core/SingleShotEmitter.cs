using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Emits a single projectile directly forward.
    /// </summary>
    public class SingleShotEmitter : IWeaponEmitter
    {
        public void Emit(IProjectileSpawner projectileSpawner, IEffectSpawner effectSpawner, string projectileId, string effectId, Vector2 origin, Vector2 baseDirection)
        {
            projectileSpawner.Spawn(projectileId, origin, baseDirection);
            
            if (!string.IsNullOrEmpty(effectId))
            {
                effectSpawner?.Spawn(effectId, origin, baseDirection);
            }
        }
    }
}