using UnityEngine;
using UnityEngine.Pool;

namespace FlightIGuess.Weapons.Unity
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PooledProjectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 2f;
        
        private Rigidbody2D _rb;
        private IObjectPool<PooledProjectile> _pool;
        private float _currentLifeTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void SetPool(IObjectPool<PooledProjectile> pool)
        {
            _pool = pool;
        }

        public void Fire(Vector2 direction)
        {
            _currentLifeTimer = _lifeTime;
            
            // Using velocity instead of AddForce to ensure consistent speed and easier resetting
            _rb.linearVelocity = direction * _speed;
            
            // Align rotation with direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
            // Handle damage logic here later...
            
            ReleaseToPool();
        }

        private void ReleaseToPool()
        {
            // Zero out physics before returning to pool
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            
            if (gameObject.activeInHierarchy)
            {
                _pool?.Release(this);
            }
        }
    }
}
