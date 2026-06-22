using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Strategy for determining HOW and WHERE a weapon fires (e.g., straight line, spread, cone).
    /// </summary>
    public interface IWeaponEmitter
    {
        /// <summary>
        /// Emits the projectiles using the provided spawner.
        /// </summary>
        void Emit(IProjectileSpawner projectileSpawner, IEffectSpawner effectSpawner, string projectileId, string effectId, Vector2 origin, Vector2 baseDirection);
    }
}