
using System.Numerics;

namespace FlightIGuess.Enemy.Core
{
    public struct EnemyDeathEvent
    {
        public Vector3 Position;
        public int Amount;
        public ResourceType ResourceType;
    }
}