using System.Threading;
using Core.Waves;
using UnityEngine;
using SysNum = System.Numerics;

public class WavePresenter : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float _spawnRadius = 15f;
    
    [Header("Dependencies")]
    [SerializeField] private EnemyPoolManager _enemyPoolManager;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _shopCanvas; // Reference to the UI we just built

    private WaveManager _waveManager;
    public WaveManager WaveManager => _waveManager;

    public void Init()
    {
        _waveManager = new WaveManager(_enemyPoolManager, _spawnRadius);
        _waveManager.OnWaveEnded += HandleWaveEnded;
        
        // Start the first wave!
        _waveManager.StartWave();
    }

    private void OnDestroy()
    {
        if (_waveManager != null)
        {
            _waveManager.OnWaveEnded -= HandleWaveEnded;
        }
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
        if (_shopCanvas != null)
        {
            _shopCanvas.SetActive(true);
        }
    }

    // Called by a "Next Wave" button in the Shop UI
    public void StartNextWave()
    {
        if (_shopCanvas != null)
        {
            _shopCanvas.SetActive(false);
        }
        Time.timeScale = 1f;
        _waveManager.StartWave();
    }
}
