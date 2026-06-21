using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Dependency Inversion interface. The core uses this to ask for a projectile to be spawned,
    /// without knowing anything about Unity's GameObjects or Object Pools.
    /// </summary>
    public interface IProjectileSpawner
    {
        void Spawn(string projectileId, Vector2 position, Vector2 direction);
    }
}
