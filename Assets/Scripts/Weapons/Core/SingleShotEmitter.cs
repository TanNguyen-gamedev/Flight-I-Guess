using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Emits a single projectile directly forward.
    /// </summary>
    public class SingleShotEmitter : IWeaponEmitter
    {
        public void Emit(IProjectileSpawner spawner, string projectileId, Vector2 origin, Vector2 baseDirection)
        {
            spawner.Spawn(projectileId, origin, baseDirection);
        }
    }
}
