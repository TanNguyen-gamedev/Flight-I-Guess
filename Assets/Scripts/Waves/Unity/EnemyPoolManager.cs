using System;
using FlightIGuess.Waves.Core;
using UnityEngine;
using UnityEngine.Pool;
using SysNum = System.Numerics;

using FlightIGuess.Combat.Unity;
using FlightIGuess.Core;

public class EnemyPoolManager : MonoBehaviour, IEnemySpawner
{
    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private CombatPresenter _combatPresenter;
    
    [Header("Pool Settings")]
    [SerializeField] private int _defaultCapacity = 100;
    [SerializeField] private int _maxSize = 500;

    private IObjectPool<EnemyController> _enemyPool;

    public event Action OnEnemyDefeated;

    private void Awake()
    {
        var poolManager = Bootstrapper.Instance.GetManager<PoolManager>();
        if(poolManager == null)
        {
            Debug.LogError("PoolManager not found in the scene!");
            return;
        }
        poolManager.RegisterPool(this);

        _enemyPool = new ObjectPool<EnemyController>(
            createFunc: CreateEnemy,
            actionOnGet: e => e.gameObject.SetActive(true),
            actionOnRelease: HandleEnemyReleased,
            actionOnDestroy: e => Destroy(e.gameObject),
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    private void Start()
    {
        _combatPresenter = Bootstrapper.Instance.GetManager<CombatPresenter>();
        if(_combatPresenter == null)
        {
            Debug.LogError("CombatPresenter not found in the scene!");
            return;
        }
    }

    private void HandleEnemyReleased(EnemyController enemy)
    {
        enemy.gameObject.SetActive(false);
        if (_combatPresenter != null && enemy.Health != null)
        {
            _combatPresenter.UnregisterEntity(enemy.Health);
        }
        OnEnemyDefeated?.Invoke();
    }

    private EnemyController CreateEnemy()
    {
        var enemy = Instantiate(_enemyPrefab, transform);
        // We initialize it right away so it has a reference to its pool to release itself on death
        enemy.Init(_enemyPool);
        return enemy;
    }

    public void SpawnEnemy(SysNum.Vector2 position)
    {
        var enemy = _enemyPool.Get();
        enemy.transform.position = new Vector3(position.X, position.Y, 0f);
        // Reset state here if needed (e.g., HP, Scale)
        enemy.Init(_enemyPool);
        
        if (_combatPresenter != null && enemy.Health != null)
        {
            _combatPresenter.RegisterEntity(enemy.Health);
        }
    }
}