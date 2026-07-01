using System;
using System.Numerics;
using FlightIGuess.Core;
using FlightIGuess.Waves.Core;

namespace FlightIGuess.Waves.Core
{
    public struct WaveEndedEvent
    {
        public int WaveNumber;
    }

    public class WaveManager
    {
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveEnded;
        public event Action<float> OnWaveTimerChanged;

        private readonly IEnemySpawner _enemySpawner;
        private readonly float _spawnRadius;
        private Random _random;

        private IWaveMission _currentMission;
        private int _currentWave = 0;
        public int CurrentWave => _currentWave;

        public bool IsWaveActive { get; private set; }
        
        private float _spawnTimer = 0f;
        private float _spawnInterval = 2f;
        public event Action<IWaveMission> OnMissionStarted;
        public event Action<string, float> OnMissionProgressChanged;

        public WaveManager(IEnemySpawner enemySpawner, float spawnRadius)
        {
            _enemySpawner = enemySpawner;
            _spawnRadius = spawnRadius;
            _random = new Random();
        }

        public void StartWave(IWaveMission mission)
        {
            _currentMission = mission;
            _currentMission.OnProgressUpdate += HandleProgressUpdate;
            _currentMission.Start();

            IsWaveActive = true;
            OnMissionStarted?.Invoke(_currentMission);
            OnWaveStarted?.Invoke(CurrentWave);
        }

        public void Tick(float deltaTime, Vector2 playerPosition)
        {
            if (!IsWaveActive) return;
            
            _currentMission.Tick(deltaTime);
            
            if(_currentMission.IsComplete)
            {
                EndWave();
                return;
            }

            // Handle Spawning
            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0)
            {
                RequestSpawn(playerPosition);
                _spawnTimer = _spawnInterval;
            }
        }
         
        private void EndWave()
        {
            _currentMission.OnProgressUpdate -= HandleProgressUpdate;
            IsWaveActive = false;
            OnWaveEnded?.Invoke(CurrentWave);
            
            // Raise an event to notify systems that the wave has ended
            EventBus.Raise(new WaveEndedEvent { WaveNumber = CurrentWave });
            
            _currentWave++;
        }

        public void OnEnemyDefeated()
        {
            _currentMission?.OnEnemyDefeated();
        }

        public void RequestSpawn(Vector2 playerPosition)
        {
            Vector2 spawnPosition = CalculateSpawnPosition(playerPosition);
            _enemySpawner.SpawnEnemy(spawnPosition);
        }

        private Vector2 CalculateSpawnPosition(Vector2 playerPosition)
        {
            // Generate a random angle between 0 and 2*PI radians
            double angle = _random.NextDouble() * Math.PI * 2;
            
            // Calculate X and Y offsets using trigonometry
            float offsetX = (float)Math.Cos(angle) * _spawnRadius;
            float offsetY = (float)Math.Sin(angle) * _spawnRadius;

            // Add offset to player's current position
            return new Vector2(playerPosition.X + offsetX, playerPosition.Y + offsetY);
        }

        private void HandleProgressUpdate(float progress)
        {
            OnMissionProgressChanged?.Invoke(_currentMission.Title, progress);
        }
    }
}
