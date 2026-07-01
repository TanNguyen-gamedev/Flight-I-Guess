using UnityEngine;
using UnityEngine.Pool;
using FlightIGuess.Combat.Core;
using FlightIGuess.Core;
using FlightIGuess.Enemy.Core;
namespace FlightIGuess.Enemy.Unity
{
    public class EnemyController : MonoBehaviour, IDamageable, IKinematicBody
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _minSize = 0.5f;
        [SerializeField] private float _maxSize = 3f;
        [SerializeField] private float _spinRate = 30f;
        [SerializeField] private Transform _enemySprite;
        [SerializeField] private float _attackTimer = 1f;
        [SerializeField] private float _basePoint = 10f;
        private IEnemyBrain _brain;
        private Transform _playerTransform;

        private bool _canAttack = true;
        private Rigidbody2D _rb;
        private IObjectPool<EnemyController> _enemyPool;

        private int _randomScale;
        
        // Core Health System
        private HealthModel _healthModel;
        public HealthModel Health => _healthModel;
        public float Mass => _rb != null ? _rb.mass : 1f;
        public float VelocityMagnitude => _rb != null ? _rb.linearVelocity.magnitude : 0f;
        public System.Numerics.Vector2 ForwardDirection => new System.Numerics.Vector2(transform.up.x, transform.up.y);

        public void Init(IObjectPool<EnemyController> pool, Transform playerTransform)
        {
            _enemyPool = pool;
            _rb = GetComponent<Rigidbody2D>();
            _brain = new SwarmerBrain();
            _playerTransform = playerTransform;

            float randomScale = Random.Range(_minSize, _maxSize);
            _randomScale = (int)randomScale;
            transform.localScale = new Vector3(randomScale, randomScale, 1f);
            
            // Setup basic health model for now (10 HP, no shields)
            // In the future, this should come from an EnemyConfigSO
            if (_healthModel == null)
            {
                _healthModel = new HealthModel(10f * randomScale, 0f, 0f, 0f);
                _healthModel.OnDeath += HandleDeath;
            }
            else
            {
                // If pulling from pool, fully restore the hull
                _healthModel.FullyRestoreHull();
            }
        }

        private void HandleDeath()
        {
            EventBus.Raise(new EnemyDeathEvent
            {
                Position = new System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z),
                Amount = _randomScale, // Can be randomized or taken from EnemyConfig later
                ResourceType = ResourceType.Scrap
            });

            _enemyPool.Release(this);
        }

        public void ForceReturnToPool()
        {
            if (gameObject.activeInHierarchy)
            {
                _enemyPool.Release(this);
            }
        }

        private void OnEnable()
        {
            if(_healthModel != null)
            {
                _healthModel.FullyRestoreHull();
                // Need to re-subscribe because OnDisable removes it!
                _healthModel.OnDeath -= HandleDeath; // Safety un-sub to prevent doubles
                _healthModel.OnDeath += HandleDeath;
            }
        }
        
        private void OnDisable()
        {
            if (_healthModel != null)
            {
                _healthModel.OnDeath -= HandleDeath;
            }
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            _brain.Tick(Time.fixedDeltaTime);
            if(_playerTransform == null)
            {
                return;
            }
            System.Numerics.Vector2 playerPosition = new System.Numerics.Vector2(_playerTransform.position.x, _playerTransform.position.y);
            System.Numerics.Vector2 enemyPosition = new System.Numerics.Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            EnemyIntent intent = _brain.Think(playerPosition, enemyPosition);
            Vector2 movement = new Vector2(intent.DesireMovement.X, intent.DesireMovement.Y);
            _rb.AddForce(movement * _speed , ForceMode2D.Force);

            // Rotate the sprite to face the direction of movement
            float angle = Mathf.Atan2(intent.DesireLookDirection.Y, intent.DesireLookDirection.X) * Mathf.Rad2Deg - 90f;
            float smoothedAngle = Mathf.MoveTowardsAngle(_rb.rotation, angle, _spinRate * Time.fixedDeltaTime);
            _rb.MoveRotation(smoothedAngle);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var otherDamageable = collision.collider.GetComponent<IDamageable>();
            var otherKinematic = collision.collider.GetComponent<IKinematicBody>();

            if (otherDamageable != null || otherKinematic != null)
            {
                Debug.Log("Collision detected!");
                var collisionDamage = new CollisionDamage
                {
                    EntityA = this,
                    BodyA = this,
                    EntityB = otherDamageable,
                    BodyB = otherKinematic,
                    RelativeVelocityMagnitude = collision.relativeVelocity.magnitude
                };
                    
                EventBus.Raise(collisionDamage);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            // For projectile hits, the projectile should ideally call CombatManager.ProcessDamage directly
            // But if we want to handle trigger hits here:
            var otherDamageable = collider.GetComponent<IDamageable>();
            if (otherDamageable != null)
            {
                // Handled by projectile logic
            }
        }

    }
}