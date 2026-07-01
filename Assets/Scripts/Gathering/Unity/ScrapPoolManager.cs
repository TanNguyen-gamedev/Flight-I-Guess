using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using SysNum = System.Numerics;
using FlightIGuess.Core;
using FlightIGuess.Gathering.Unity;

public class ScrapPoolManager : MonoBehaviour, IClearablePool
{
    [SerializeField] private ScrapPresenter _scrapPrefab;
    
    [Header("Pool Settings")]
    [SerializeField] private int _defaultCapacity = 50;
    [SerializeField] private int _maxSize = 200;
    private RunStatePresenter _runStatePresenter;
    private IObjectPool<ScrapPresenter> _pool;
    
    private void Awake()
    {
        // 1. Register with Master Pool Manager
        var poolManager = Bootstrapper.Instance.GetManager<PoolManager>();
        if(poolManager != null)
        {
            poolManager.RegisterPool(this);
        }
    
        // . Setup the Unity Object Pool
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
        EventBus.Subscribe<FlightIGuess.Enemy.Core.EnemyDeathEvent>(OnEnemyDeathEvent);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<FlightIGuess.Enemy.Core.EnemyDeathEvent>(OnEnemyDeathEvent);
    }

    /// <summary>
    /// Spawns a resource drop at the given position.
    /// Supports dynamic amounts and types so different enemies can drop different loot.
    /// </summary>
    public void SpawnScrap(Vector3 spawnPosition, int amount, ResourceType type)
    {
        if (_runStatePresenter == null)
        {
            _runStatePresenter = Bootstrapper.Instance.GetManager<RunStatePresenter>();
            if (_runStatePresenter == null)
            {
                Debug.LogError("ScrapPoolManager: Cannot spawn scrap, RunStatePresenter is missing!");
                return;
            }
        }

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
        presenter.Init(scrapModel, _runStatePresenter.RunStateModel, _runStatePresenter.Config, _runStatePresenter.PlayerTransform, _pool);

        // Tell the Core State Manager to start checking distance for this scrap
        _runStatePresenter.RunStateModel.AddScrapToTracking(scrapModel);
    }

    private ScrapPresenter CreateScrap()
    {
        return Instantiate(_scrapPrefab, transform);
    }

    private void OnEnemyDeathEvent(FlightIGuess.Enemy.Core.EnemyDeathEvent evt)
    {
        SpawnScrap(new Vector3(evt.Position.X, evt.Position.Y, evt.Position.Z), evt.Amount, evt.ResourceType);
    }

    public void ClearActiveObjects()
    {
        // Iterate backwards since we are releasing elements back to the pool
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).GetComponent<ScrapPresenter>();
            if (child != null && child.gameObject.activeInHierarchy)
            {
                child.ForceReturnToPool();
            }
        }
    }
}
