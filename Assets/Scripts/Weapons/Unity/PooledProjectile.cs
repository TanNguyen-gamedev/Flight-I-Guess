using UnityEngine;
using UnityEngine.Pool;

namespace FlightIGuess.Weapons.Unity
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PooledProjectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 2f;
        [SerializeField] private string _hitEffectId = "Explosion";
        [SerializeField] private TrailRenderer _trail;
        
        private Rigidbody2D _rb;
        private IObjectPool<PooledProjectile> _pool;
        private FlightIGuess.Weapons.Core.IEffectSpawner _effectSpawner;
        private float _currentLifeTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _trail = GetComponent<TrailRenderer>();
        }

        public void SetPool(IObjectPool<PooledProjectile> pool)
        {
            _pool = pool;
        }

        public void SetEffectSpawner(FlightIGuess.Weapons.Core.IEffectSpawner effectSpawner)
        {
            _effectSpawner = effectSpawner;
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
            if (!string.IsNullOrEmpty(_hitEffectId) && _effectSpawner != null)
            {
                Vector2 hitPoint = transform.position;
                Vector2 hitNormal = -_rb.linearVelocity.normalized;

                if (collision.contactCount > 0)
                {
                    hitPoint = collision.GetContact(0).point;
                    hitNormal = collision.GetContact(0).normal;
                }

                _effectSpawner.Spawn(
                    _hitEffectId, 
                    new System.Numerics.Vector2(hitPoint.x, hitPoint.y), 
                    new System.Numerics.Vector2(hitNormal.x, hitNormal.y)
                );
            }

            // Handle damage logic here later...
            ReleaseToPool();
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
             if(collider.gameObject.CompareTag("Projectile"))
            {
                return;
            }
            if (!string.IsNullOrEmpty(_hitEffectId) && _effectSpawner != null)
            {
                Vector2 hitPoint = transform.position;
                Vector2 hitNormal = -_rb.linearVelocity.normalized;

                _effectSpawner.Spawn(
                    _hitEffectId, 
                    new System.Numerics.Vector2(hitPoint.x, hitPoint.y), 
                    new System.Numerics.Vector2(hitNormal.x, hitNormal.y)
                );
            }

            // Handle damage logic here later...
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
                _pool?.Release(this);
            }
        }
    }
}
