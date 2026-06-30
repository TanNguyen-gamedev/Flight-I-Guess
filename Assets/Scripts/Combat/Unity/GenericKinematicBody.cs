using UnityEngine;
using FlightIGuess.Combat.Core;

namespace FlightIGuess.Combat.Unity
{
    /// <summary>
    /// A simple, reusable component to attach to physics objects that don't have health (like Asteroids)
    /// but still need to deal velocity-based damage to IDamageable entities they collide with.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class GenericKinematicBody : MonoBehaviour, IKinematicBody
    {
        private Rigidbody2D _rb;
        public System.Numerics.Vector2 ForwardDirection => new System.Numerics.Vector2(transform.up.x, transform.up.y);

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public float Mass => _rb != null ? _rb.mass : 1f;
        public float VelocityMagnitude => _rb != null ? _rb.linearVelocity.magnitude : 0f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var otherDamageable = collision.collider.GetComponent<IDamageable>();
            var otherKinematic = collision.collider.GetComponent<IKinematicBody>();

            // If we hit something that can take damage or has mass
            if (otherDamageable != null || otherKinematic != null)
            {
                var collisionDamage = new CollisionDamage
                {
                    EntityA = null, // We are not damageable
                    BodyA = this,
                    EntityB = otherDamageable,
                    BodyB = otherKinematic,
                    RelativeVelocityMagnitude = collision.relativeVelocity.magnitude
                };
                
                FlightIGuess.Core.EventBus.Raise(collisionDamage);
            }
        }
    }
}