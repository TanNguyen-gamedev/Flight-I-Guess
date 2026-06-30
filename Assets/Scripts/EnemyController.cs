using UnityEngine;
using UnityEngine.Pool;
using FlightIGuess.Combat.Core;
using FlightIGuess.Core;

public class EnemyController : MonoBehaviour, IDamageable, IKinematicBody
{

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _minSize = 0.5f;
    [SerializeField] private float _maxSize = 3f;
    [SerializeField] private float _spinRate = 30f;
    [SerializeField] private Transform _enemySprite;
    [SerializeField] private float _attackTimer = 1f;
    [SerializeField] private float _basePoint = 10f;
    [SerializeField] private FloatEventChannel _onEnemyDeathFloat;
    [SerializeField] private EnemyDeathEventChannel _onEnemyDeath;
    private float _enemyPoint;

    private bool _canAttack = true;
    private Rigidbody2D _rb;
    private IObjectPool<EnemyController> _pool;
    
    // Core Health System
    private HealthModel _healthModel;
    public HealthModel Health => _healthModel;
    public float Mass => _rb != null ? _rb.mass : 1f;
    public float VelocityMagnitude => _rb != null ? _rb.linearVelocity.magnitude : 0f;
    public System.Numerics.Vector2 ForwardDirection => new System.Numerics.Vector2(transform.up.x, transform.up.y);

    public void Init(IObjectPool<EnemyController> pool)
    {
        _pool = pool;
        _rb = GetComponent<Rigidbody2D>();
        float randomScale = UnityEngine.Random.Range(_minSize, _maxSize);
        _enemyPoint = _basePoint * randomScale;
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
        _onEnemyDeathFloat.RaiseEvent(_enemyPoint);
        _onEnemyDeath.RaiseEvent(transform.position, Mathf.FloorToInt(_enemyPoint), ResourceType.Scrap);            
        _pool.Release(this);
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
        Vector2 randomDirection = Random.insideUnitCircle;
        _rb.AddForce(randomDirection * _speed * Time.fixedDeltaTime, ForceMode2D.Force);
        _enemySprite.transform.Rotate(0f, 0f, _spinRate * Time.fixedDeltaTime);
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
