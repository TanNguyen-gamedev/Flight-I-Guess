using System;
using System.Numerics;

namespace FlightIGuess.Waves.Core
{
    public interface IEnemySpawner
    {
        void SpawnEnemy(Vector2 position);
    }
}
