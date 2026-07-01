using System.Collections.Generic;
using FlightIGuess.Core;
using FlightIGuess.Waves.Core;
using UnityEngine;

namespace FlightIGuess.Combat.Core
{
    public struct CollisionDamage
    {
        public IDamageable EntityA;
        public IKinematicBody BodyA;
        public IDamageable EntityB;
        public IKinematicBody BodyB;
        public float RelativeVelocityMagnitude;
        public Vector2 IncomingDirection;
    }
    public class CombatManager
    {
        private List<HealthModel> _activeEntities = new List<HealthModel>();
        
        // Track recently processed collisions to prevent double-damage from symmetric OnCollisionEnter2D events.
        // We use a HashSet of string hashes to quickly check if a pair of bodies has collided this frame.
        private HashSet<int> _processedCollisionsThisFrame = new HashSet<int>();
        
        private float _minimumImpactVelocity;
        private float _damageMultiplier;

        public void SetDamageConfig(float minVelocity, float multiplier)
        {
            _minimumImpactVelocity = minVelocity;
            _damageMultiplier = multiplier;
        }

        public void Initialize()
        {
            EventBus.Subscribe<CollisionDamage>(ProcessCollision);
            EventBus.Subscribe<WaveEndedEvent>(HandleWaveEnded);
        }

        public void Dispose()
        {
            EventBus.Unsubscribe<CollisionDamage>(ProcessCollision);
            EventBus.Unsubscribe<WaveEndedEvent>(HandleWaveEnded);
        }

        private void HandleWaveEnded(WaveEndedEvent waveEndedEvent)
        {
            foreach (var entity in _activeEntities)
            {
                entity.FullyRestoreHull();
            }
        }

        public void Register(HealthModel model)
        {
            if (!_activeEntities.Contains(model))
            {
                _activeEntities.Add(model);
            }
        }

        public void Unregister(HealthModel model)
        {
            _activeEntities.Remove(model);
        }

        public void Tick(float dt)
        {
            // Clear the collision tracker at the start of every frame
            _processedCollisionsThisFrame.Clear();

            foreach(var entity in _activeEntities)
            {
                entity.Tick(dt);
            }
        }

        public void ProcessCollision(CollisionDamage collision)
        {
            float velocity = collision.RelativeVelocityMagnitude;
            if(velocity < _minimumImpactVelocity)
            {
                return;
            }

            if(CheckDuplicateDamage(collision))
            {
                return;
            }

            ApplyCollisionDamage(collision,  velocity);

        }

        private void ApplyCollisionDamage(CollisionDamage collision, float velocity)
        {

            float damageToA = velocity * collision.BodyB.Mass * _damageMultiplier;
            float damageToB = velocity * collision.BodyA.Mass * _damageMultiplier;
            if(collision.EntityA != null)
            {
                HealthModel healthA = collision.EntityA.Health;
                if(healthA != null)
                {
                    healthA.ApplyDamage(damageToA);
                }
            }
            if(collision.EntityB != null)
            {
                HealthModel healthB = collision.EntityB.Health;
                if(healthB != null)
                {
                    healthB.ApplyDamage(damageToB);
                }
            }
        }

        private bool CheckDuplicateDamage(CollisionDamage collision)
        {
            // Create a unique hash for this pair of colliding bodies.
            // Using GetHashCode on the objects. We order them to ensure A-hits-B produces the same hash as B-hits-A.
            int hashA = collision.BodyA != null ? collision.BodyA.GetHashCode() : 0;
            int hashB = collision.BodyB != null ? collision.BodyB.GetHashCode() : 0;
            
            // A simple commutative hash function (addition or XOR) ensures order doesn't matter
            int collisionHash = hashA + hashB;

            // If we've already processed this exact collision this frame, ignore it
            if (_processedCollisionsThisFrame.Contains(collisionHash))
            {
                return true;
            }
            
            // Mark it as processed
            _processedCollisionsThisFrame.Add(collisionHash);

            return false;
        }

        public void ProcessDamage(IDamageable target, float amount)
        {
            if(target != null && target.Health != null)
            {
                target.Health.ApplyDamage(amount);
            }
        }
    }
}