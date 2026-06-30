using System.Numerics;

namespace FlightIGuess.Enemy.Core
{
    public class SwarmerBrain : IEnemyBrain
    {
        public EnemyIntent Think(Vector2 playerPosition, Vector2 currentPosition)
        {
            Vector2 direction = playerPosition - currentPosition;
            return new EnemyIntent(Vector2.Normalize(direction), direction, false);
        }

        public void Tick(float dt)
        {
        }
    }
}