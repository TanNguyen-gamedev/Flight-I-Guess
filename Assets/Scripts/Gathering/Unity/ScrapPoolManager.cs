using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using SysNum = System.Numerics;

public class ScrapPoolManager : MonoBehaviour
{
    [SerializeField] private ScrapPresenter _scrapPrefab;
    [SerializeField] private ScrapConfigSO _config;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private EnemyDeathEventChannel _onEnemyDeath;
    
    [Header("Pool Settings")]
    [SerializeField] private int _defaultCapacity = 50;
    [SerializeField] private int _maxSize = 200;

    private IObjectPool<ScrapPresenter> _pool;
    private RunStateModel _runStateModel;
    
    // Public accessor so the HUD can subscribe to events
    public RunStateModel RunStateModel => _runStateModel;

    private void Awake()
    {
        // 1. Initialize the Core C# Model
        _runStateModel = new RunStateModel();
        _runStateModel.SetMagnetRadius(_config.MagnetRange);

        // 2. Setup the Unity Object Pool
        _pool = new ObjectPool<ScrapPresenter>(
            createFunc: CreateScrap,
            actionOnGet: s => s.gameObject.SetActive(true),
            actionOnRelease: s => s.gameObject.SetActive(false),
            actionOnDestroy: s => Destroy(s.gameObject),
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    private void OnEnable()
    {
        _onEnemyDeath.OnEventRaise += OnEnemyDeath;
    }

    private void OnDisable()
    {
        _onEnemyDeath.OnEventRaise -= OnEnemyDeath;
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        // Drive the C# tick from Unity's Update loop
        var playerPos = new SysNum.Vector2(
            _playerTransform.position.x, 
            _playerTransform.position.y
        );
        
        _runStateModel.Tick(playerPos);
    }

    /// <summary>
    /// Spawns a resource drop at the given position.
    /// Supports dynamic amounts and types so different enemies can drop different loot.
    /// </summary>
    public void SpawnScrap(Vector3 spawnPosition, int amount, ResourceType type)
    {
        // Get a visual presenter from the pool
        var presenter = _pool.Get();
        presenter.transform.position = spawnPosition;

        // Create the pure C# model for this specific scrap
        // Note: We create a new model here to keep things simple for now.
        // If garbage collection becomes an issue, we can pool ScrapModel as well.
        var scrapModel = new ScrapModel();
        var sysNumPos = new SysNum.Vector2(spawnPosition.x, spawnPosition.y);
        
        // Initialize with dynamic amount and type!
        scrapModel.Init(amount, type, sysNumPos);

        // Link them together
        presenter.Init(scrapModel, _runStateModel, _config, _playerTransform, _pool);

        // Tell the Core State Manager to start checking distance for this scrap
        _runStateModel.AddScrapToTracking(scrapModel);
    }

    private ScrapPresenter CreateScrap()
    {
        return Instantiate(_scrapPrefab);
    }

    private void OnEnemyDeath(Vector3 spawnPosition, int amount, ResourceType type)
    {
        SpawnScrap(spawnPosition, amount, type);
    }
}
