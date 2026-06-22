
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _minSize = 0.5f;
    [SerializeField] private float _maxSize = 3f;
    [SerializeField] private float _spinRate = 30f;
    [SerializeField] private Transform _enemySprite;
    [SerializeField] private float _attackTimer = 1f;
    [SerializeField] private float _basePoint = 10f;
    [SerializeField] private FloatEventChannel _onEnemyDeath;
    private float _enemyPoint;

    private bool _canAttack = true;
    private Rigidbody2D _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        float randomScale = Random.Range(_minSize, _maxSize);
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
            _onEnemyDeath.RaiseEvent(_enemyPoint);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Projectile"))
        {
            _onEnemyDeath.RaiseEvent(_enemyPoint);
            Destroy(gameObject);
        }
    }

}
