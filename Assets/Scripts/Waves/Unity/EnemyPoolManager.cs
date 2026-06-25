using Core.Waves;
using UnityEngine;
using UnityEngine.Pool;
using SysNum = System.Numerics;

public class EnemyPoolManager : MonoBehaviour, IEnemySpawner
{
    [SerializeField] private EnemyController _enemyPrefab;
    
    [Header("Pool Settings")]
    [SerializeField] private int _defaultCapacity = 100;
    [SerializeField] private int _maxSize = 500;

    private IObjectPool<EnemyController> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<EnemyController>(
            createFunc: CreateEnemy,
            actionOnGet: e => e.gameObject.SetActive(true),
            actionOnRelease: e => e.gameObject.SetActive(false),
            actionOnDestroy: e => Destroy(e.gameObject),
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    private EnemyController CreateEnemy()
    {
        var enemy = Instantiate(_enemyPrefab, transform);
        // We initialize it right away so it has a reference to its pool to release itself on death
        enemy.Init(_pool);
        return enemy;
    }

    public void SpawnEnemy(SysNum.Vector2 position)
    {
        var enemy = _pool.Get();
        enemy.transform.position = new Vector3(position.X, position.Y, 0f);
        // Reset state here if needed (e.g., HP, Scale)
        enemy.Init(_pool);
    }
}