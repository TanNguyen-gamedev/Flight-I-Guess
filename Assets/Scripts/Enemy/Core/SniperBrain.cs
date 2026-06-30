
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using Unity.VisualScripting;

namespace FlightIGuess.Enemy.Core
{
    enum States
    {
        Reposition,
        Aim,
        Locked,
        Fire,
    }
    public class SniperBrain : IEnemyBrain
    {
        private float _optimalRange = 15f;
        private float _aimDuration = 2f;
        private float _lockDuration = 0.5f;
        private float _cooldownDuration =3f;

        private States _currentState;
        private float _stateTimer;
        private Vector2 _lockedAimDirections;
        private SniperBrain(float optimalRange, float aimDuration, float lockDuration, float cooldownDuration)
        {
            _optimalRange = optimalRange;
            _aimDuration = aimDuration;
            _lockDuration = lockDuration;
            _cooldownDuration = cooldownDuration;
        }
       
        public EnemyIntent Think(Vector2 playerPosition, Vector2 currentPosition)
        {
              switch(_currentState)
                {
                    case States.Reposition:
                    {
                        Vector2 ToPlayer = playerPosition - currentPosition;
                        Vector2 direction = Vector2.Normalize(ToPlayer);
                        return new EnemyIntent(playerPosition - (direction * _optimalRange), ToPlayer, false);
                    }
                    case States.Aim:
                    {
                        _currentState = States.Locked;
                        _stateTimer = _lockDuration;
                        break;
                    }
                    case States.Locked:
                    {
                        _currentState = States.Reposition;
                        _stateTimer = _cooldownDuration;
                        break;
                    }
                }
                return new EnemyIntent();
        }

        public void Tick(float dt)
        {
            _stateTimer -= dt;
            if(_stateTimer <= 0f)
            {
                switch(_currentState)
                {
                    case States.Reposition:
                    {
                        _currentState = States.Aim;
                        _stateTimer = _aimDuration;
                        break;
                    }
                    case States.Aim:
                    {
                        _currentState = States.Locked;
                        _stateTimer = _lockDuration;
                        break;
                    }
                    case States.Locked:
                    {
                        _currentState = States.Reposition;
                        _stateTimer = _cooldownDuration;
                        break;
                    }
                }
            }
        }
    }
}