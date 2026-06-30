using System.Numerics;

namespace FlightIGuess.Weapons.Core
{
    public struct SpawnEffectEvent
    {
        public string EffectId { get; }
        public Vector2 Position { get; }
        public Vector2 Direction { get; }

        public SpawnEffectEvent(string effectId, Vector2 position, Vector2 direction)
        {
            EffectId = effectId;
            Position = position;
            Direction = direction;
        }
    }
}
