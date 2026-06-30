using System;
using System.Collections.Generic;
using System.Threading;
using FlightIGuess.Core;
using FlightIGuess.Waves;
using FlightIGuess.Waves.Core;
using UnityEngine;
using Random = UnityEngine.Random;
using SysNum = System.Numerics;

public class WavePresenter : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float _spawnRadius = 15f;
    
    [Header("Dependencies")]
    [SerializeField] private EnemyPoolManager _enemyPoolManager;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _shopCanvas; // Reference to the UI we just built
    [SerializeField] private MissionConfigSO[] _missionList;
    private WaveManager _waveManager;
    public WaveManager WaveManager => _waveManager;
    public event Action<bool> OnWaveAction;

    public void Init()
    {
        _enemyPoolManager = Bootstrapper.Instance.GetManager<EnemyPoolManager>();
        if (_enemyPoolManager == null)
        {
            Debug.LogError("EnemyPoolManager not found in the scene!");
            return;
        }
        
        _waveManager = new WaveManager(_enemyPoolManager, _spawnRadius);
        _waveManager.OnWaveEnded += HandleWaveEnded;
        _enemyPoolManager.OnEnemyDefeated += HandleEnemyDefeated;
    }

    public void StartWave()
    {
        _waveManager.StartWave(GetRandomMission());
    }

    private void OnDestroy()
    {
        if (_waveManager != null)
        {
            _waveManager.OnWaveEnded -= HandleWaveEnded;
        }
        if (_enemyPoolManager != null)
        {
            _enemyPoolManager.OnEnemyDefeated -= HandleEnemyDefeated;
        }
    }

    private void HandleEnemyDefeated()
    {
        _waveManager?.OnEnemyDefeated();
    }

    private void Update()
    {
        if (_waveManager == null || !_waveManager.IsWaveActive || _playerTransform == null) return;

        var playerPos = new SysNum.Vector2(_playerTransform.position.x, _playerTransform.position.y);
        _waveManager.Tick(Time.deltaTime, playerPos);
    }

    private void HandleWaveEnded(int completedWaveNumber)
    {
        // Pause the game and show the shop
        Time.timeScale = 0f;
        OnWaveAction?.Invoke(true);
        
    }

    // Called by a "Next Wave" button in the Shop UI
    public void StartNextWave()
    {
        
        Time.timeScale = 1f;
        OnWaveAction?.Invoke(false);
        _waveManager.StartWave(GetRandomMission());
    }

    private IWaveMission GetRandomMission()
    {
        return _missionList[Random.Range(0, _missionList.Length)].CreateMission();
    }
}
