using System.Numerics;

namespace FlightIGuess.Enemy.Core
{
    public struct EnemyIntent
    {
        public Vector2 DesireMovement;
        public Vector2 DesireLookDirection;
        public bool IsFiring;
        public EnemyIntent(Vector2 desireMovement, Vector2 desireLookDirection, bool isFiring)
        {
            DesireMovement = desireMovement;
            DesireLookDirection = desireLookDirection;
            IsFiring = isFiring;
        }
    }
    public interface IEnemyBrain
    {
        public EnemyIntent Think(Vector2 playerPosition, Vector2 currentPosition);
        public void Tick(float dt);
    }
}