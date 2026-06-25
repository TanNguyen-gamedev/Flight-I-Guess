using System;
using System.Numerics;
using FlightIGuess.Waves.Core.Interfaces;

namespace Core.Waves
{
    public class WaveManager
    {
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveEnded;
        public event Action<float> OnWaveTimerChanged;

        private readonly IEnemySpawner _enemySpawner;
        private readonly float _spawnRadius;
        private Random _random;

        public int CurrentWave { get; private set; } = 1;
        public float WaveDuration { get; private set; } = 10f; // 60 seconds per wave, 10 when testing
        public float TimeRemaining { get; private set; }
        public bool IsWaveActive { get; private set; }
        
        private float _spawnTimer = 0f;
        private float _spawnInterval = 2f;

        public WaveManager(IEnemySpawner enemySpawner, float spawnRadius)
        {
            _enemySpawner = enemySpawner;
            _spawnRadius = spawnRadius;
            _random = new Random();
        }

        public void StartWave()
        {
            TimeRemaining = WaveDuration;
            IsWaveActive = true;
            _spawnInterval = Math.Max(0.5f, 2f - (CurrentWave * 0.1f)); // Spawns get faster each wave
            OnWaveStarted?.Invoke(CurrentWave);
        }

        public void Tick(float deltaTime, Vector2 playerPosition)
        {
            if (!IsWaveActive) return;

            TimeRemaining -= deltaTime;
            OnWaveTimerChanged?.Invoke(TimeRemaining);

            if (TimeRemaining <= 0)
            {
                EndWave();
                return;
            }

            // Handle Spawning synchronously based on deltaTime
            _spawnTimer -= deltaTime;
            if (_spawnTimer <= 0)
            {
                RequestSpawn(playerPosition);
                _spawnTimer = _spawnInterval;
            }
        }

        private void EndWave()
        {
            IsWaveActive = false;
            OnWaveEnded?.Invoke(CurrentWave);
            CurrentWave++;
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
    }
}
