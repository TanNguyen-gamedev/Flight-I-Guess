using UnityEngine;
using UnityEngine.Pool;
using FlightIGuess.Core;
using FlightIGuess.Weapons.Core;
using FlightIGuess.Combat.Core;

namespace FlightIGuess.Weapons.Unity
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PooledProjectile : MonoBehaviour, IKinematicBody
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 2f;
        [SerializeField] private string _hitEffectId = "Explosion";
        [SerializeField] private TrailRenderer _trail;
        
        private Rigidbody2D _rb;
        private IObjectPool<PooledProjectile> _projectilePool;
        private float _currentLifeTimer;

        public float Mass => _rb.mass;

        public System.Numerics.Vector2 ForwardDirection => new System.Numerics.Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y);

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _trail = GetComponent<TrailRenderer>();
        }

        public void SetPool(IObjectPool<PooledProjectile> pool)
        {
            _projectilePool = pool;
        }

        public void Fire(Vector2 direction)
        {
            _currentLifeTimer = _lifeTime;
            
            // Using velocity instead of AddForce to ensure consistent speed and easier resetting
            _rb.linearVelocity = direction * _speed;
            
            // Align rotation with direction
            // -90 for sprite allign
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if(_trail != null)
            {
                _trail.enabled = true;
            }
        }

        private void FixedUpdate()
        {
            // Manual lifetime tracking to avoid Coroutines or Destroy()
            if (_currentLifeTimer > 0)
            {
                _currentLifeTimer -= Time.fixedDeltaTime;
                if (_currentLifeTimer <= 0)
                {
                    ReleaseToPool();
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Projectile"))
            {
                return;
            }
            if (!string.IsNullOrEmpty(_hitEffectId))
            {
                Vector2 hitPoint = transform.position;
                Vector2 hitNormal = -_rb.linearVelocity.normalized;

                if (collision.contactCount > 0)
                {
                    hitPoint = collision.GetContact(0).point;
                    hitNormal = collision.GetContact(0).normal;
                }

                EventBus.Raise(new SpawnEffectEvent(
                    _hitEffectId, 
                    new System.Numerics.Vector2(hitPoint.x, hitPoint.y), 
                    new System.Numerics.Vector2(hitNormal.x, hitNormal.y)
                ));
            }

            // Projectile damage logic
            var damageable = collision.collider.GetComponentInParent<IDamageable>();
            if (damageable != null && damageable.Health != null)
            {
                // TODO: Read this damage value from a WeaponConfig instead of hardcoding
                damageable.Health.ApplyDamage(10f); 
            }

            ReleaseToPool();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
             if(collider.gameObject.CompareTag("Projectile"))
            {
                return;
            }
            if (!string.IsNullOrEmpty(_hitEffectId))
            {
                Vector2 hitPoint = transform.position;
                Vector2 hitNormal = -_rb.linearVelocity.normalized;

                EventBus.Raise(new SpawnEffectEvent(
                    _hitEffectId, 
                    new System.Numerics.Vector2(hitPoint.x, hitPoint.y), 
                    new System.Numerics.Vector2(hitNormal.x, hitNormal.y)
                ));
            }

            // Projectile damage logic
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null && damageable.Health != null)
            {
                // TODO: Read this damage value from a WeaponConfig instead of hardcoding
                damageable.Health.ApplyDamage(10f);
                Debug.Log($"Current Hull: {damageable.Health.CurrentHull}");
                Debug.Log($"Projectile hit {collider.name} for 10 damage");
            }

            ReleaseToPool();
        }

        private void ReleaseToPool()
        {
            // Zero out physics before returning to pool
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;

            if(_trail != null)
            {
                _trail.Clear();
                _trail.enabled = false;
            }
            
            if (gameObject.activeInHierarchy)
            {
                _projectilePool?.Release(this);
            }
        }

        public void ForceReturnToPool()
        {
            ReleaseToPool();
        }
    }
}
