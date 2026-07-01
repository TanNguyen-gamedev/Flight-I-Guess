using UnityEngine;
using UnityEngine.Pool;

namespace FlightIGuess.Weapons.Unity
{
    public class PooledEffect : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 2f;

        private IObjectPool<PooledEffect> _pool;
        private float _currentLifeTimer;

        public void SetPool(IObjectPool<PooledEffect> pool)
        {
            _pool = pool;
        }

        public void Fire(Vector2 direction)
        {
            _currentLifeTimer = _lifeTime;
            
            // Align rotation with direction
            // -90 for sprite allign
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
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

        private void ReleaseToPool()
        {
            if (gameObject.activeInHierarchy)
            {
                _pool?.Release(this);
            }
        }

        public void ForceReturnToPool()
        {
            ReleaseToPool();
        }
    }
}