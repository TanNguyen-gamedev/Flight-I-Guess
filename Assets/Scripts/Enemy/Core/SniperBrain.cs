
using System;
using System.Numerics;

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
        private float _offset = 1f;
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
            Vector2 toPlayer = playerPosition - currentPosition;

            switch(_currentState)
            {
                case States.Reposition:
                {
                    Vector2 directionToPlayer = Vector2.Normalize(toPlayer);
                    float distance = toPlayer.Length();
                    Vector2 desireMovement = (distance > _optimalRange) ? directionToPlayer : -directionToPlayer;
                    return new EnemyIntent(desireMovement, directionToPlayer, false, false);
                }
                case States.Aim:
                {
                    Vector2 dumbPlayerPos = new Vector2(playerPosition.X + _offset,
                    playerPosition.Y);
                    _lockedAimDirections = Vector2.Normalize(dumbPlayerPos - currentPosition);
                    return new EnemyIntent(Vector2.Zero, _lockedAimDirections, true, false);      
                }
                case States.Locked:
                {
                    return new EnemyIntent(Vector2.Zero, _lockedAimDirections, true, false);
                }
                case States.Fire:
                {
                    return new EnemyIntent(Vector2.Zero, _lockedAimDirections, false, true);
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
                        _currentState = States.Fire;
                        // Reuse lock duration for fire duration
                        _stateTimer = _lockDuration;
                        break;
                    }
                    case States.Fire:
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