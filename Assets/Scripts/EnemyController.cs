using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour
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

    public void Init(IObjectPool<EnemyController> pool)
    {
        _pool = pool;
        _rb = GetComponent<Rigidbody2D>();
        float randomScale = UnityEngine.Random.Range(_minSize, _maxSize);
        _enemyPoint = _basePoint * randomScale;
        transform.localScale = new Vector3(randomScale, randomScale, 1f);
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
        if(collision.gameObject.CompareTag("Projectile"))
        {
            _onEnemyDeathFloat.RaiseEvent(_enemyPoint);
            _onEnemyDeath.RaiseEvent(transform.position, Mathf.FloorToInt(_enemyPoint), ResourceType.Scrap);            
            _pool.Release(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Projectile"))
        {
            _onEnemyDeathFloat.RaiseEvent(_enemyPoint);
            _onEnemyDeath.RaiseEvent(transform.position, Mathf.FloorToInt(_enemyPoint), ResourceType.Scrap);            
            _pool.Release(this);
        }
    }

}
