using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    /// <summary>
    /// Dependency Inversion interface. The core uses this to ask for a visual effect to be spawned.
    /// </summary>
    public interface IEffectSpawner
    {
        void Spawn(string effectId, Vector2 position, Vector2 direction);
    }
}