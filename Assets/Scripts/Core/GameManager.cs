using UnityEngine;
using FlightIGuess.Ship.Unity;
using FlightIGuess.Ship.Core;
using FlightIGuess.Shop.Unity;
using FlightIGuess.Shop.Core;
using FlightIGuess.Weapons.Unity;
using FlightIGuess.Combat.Core;
using FlightIGuess.Combat.Unity;
using FlightIGuess.Gathering.Unity;
using System;
using FlightIGuess.Waves.Core;

namespace FlightIGuess.Core
{
    public enum GameState
    {
        Playing,
        Paused,
        Win,
        Loss,
        Shop
    }


    public class GameManager : MonoBehaviour
    {
        [Header("Configurations")]
        [SerializeField] private HullConfigSO _startingHull;

        [Header("Prefabs (Spawned at Runtime)")]
        [SerializeField] private PlayerController _playerPrefab;
        [SerializeField] private ShopPresenter _shopPrefab;
        
        [Header("Spawn Points")]
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _uiCanvasRoot;

        [Header("Scene References")]
        [SerializeField] private RunStatePresenter _runStatePresenter;
        [SerializeField] private WavePresenter _wavePresenter;
        [SerializeField] private HUD _hudPresenter;
        [SerializeField] private CombatPresenter _combatPresenter;

        private PlayerController _playerController;
        private ShipWeaponsPresenter _weaponsPresenter;
        private ShopPresenter _shopPresenter;

        private ShipModel _shipModel;
        private ShopModel _shopModel;
        private CombatManager _combatManager;
        private GameState _gameState = GameState.Playing;
        // The event is no longer needed since we use EventBus
        // public event Action<GameState> OnGameStateChange;

        private void Start()
        {
            Bootstrapper.Instance.Register(this);
            
            // Clean up any leftover pooled objects from a previous run
            if (Bootstrapper.Instance.TryGetManager<PoolManager>(out var poolManager))
            {
                poolManager.ClearAllPools();
            }

            // 0. Initialize Combat System
            _combatManager = new CombatManager();
            if (_combatPresenter != null)
            {
                _combatPresenter.Init(_combatManager);
            }

            // 1. Initialize Wave System
            if (_wavePresenter != null)
            {
                _wavePresenter.Init();
            }

            // 1.5 Spawn Player and Shop
            SpawnEntities();

            // 2. Create ShipModel from Config
            if (_startingHull != null)
            {
                _shipModel = _startingHull.CreateShipModel();
                
                // Link the hardpoints that the ShipWeaponsPresenter initialized
                if (_weaponsPresenter != null)
                {
                    _shipModel.TrackHardPoint(_weaponsPresenter.HardpointModels);
                }

                // Inject into PlayerController
                if (_playerController != null)
                {
                    _playerController.Init(_shipModel);
                    _shipModel.Health.OnDeath += HandlePlayerDeath;
                    
                    // Register the player's health model with the combat manager
                    if (_combatManager != null && _shipModel.Health != null)
                    {
                        _combatManager.Register(_shipModel.Health);
                    }
                }

                // Inject the starting weapon from AppManager
                if (_weaponsPresenter != null)
                {
                    _shipModel.TrackHardPoint(_weaponsPresenter.HardpointModels);

                    AppManager appManager = Bootstrapper.Instance.GetManager<AppManager>();
                    
                    WeaponConfigSO startingWeapon = 
                    (appManager != null && appManager.SelectedStartingWeapon != null) 
                    ? appManager.SelectedStartingWeapon 
                    : null; 

                    if (startingWeapon != null)
                    {
                        _weaponsPresenter.OnWeaponPurchased(startingWeapon);
                    }
                }
            }
            else
            {
                Debug.LogError("GameManager: Missing Starting Hull Config!");
            }

            // 3. Create ShopModel and inject into ShopPresenter
            if (_runStatePresenter != null && _shopPresenter != null && _shipModel != null)
            {
                _shopModel = new ShopModel(_runStatePresenter.RunStateModel, _shipModel);
                _shopPresenter.Init(_shopModel, _wavePresenter);
            }

            // 4. Wiring the purchase between shop presenter and weapon presenter
            if(_shopPresenter != null && _weaponsPresenter != null)
            {
                _shopPresenter.OnWeaponPurchased += _weaponsPresenter.OnWeaponPurchased;
            }

            // 5. Wiring the event wave ened between wave presenter and shop presenter
            if(_wavePresenter != null && _shopPresenter != null)
            {
                _wavePresenter.OnWaveAction += _shopPresenter.OnWaveAction;
            }
            
            // 6. Wiring the mission from wave presenter to hud presenter
            if(_wavePresenter != null && _hudPresenter != null)
            {
                _wavePresenter.WaveManager.OnMissionProgressChanged += _hudPresenter.OnMissionProgressUpdate;
                _wavePresenter.OnWaveAction += _hudPresenter.OnWaveAction;
            }

            // n. Start the wave after all set up stuff works
            if(_wavePresenter != null)
            {
                _wavePresenter.StartWave();
                EventBus.Subscribe<WaveEndedEvent>(OnWaveEnded);
            }

        }

        private void HandlePlayerDeath()
        {
            ChangeGameState(GameState.Loss);
        }

        public void TogglePause()
        {
            if (_gameState == GameState.Playing)
            {
                ChangeGameState(GameState.Paused);
            }
            else if (_gameState == GameState.Paused)
            {
                ChangeGameState(GameState.Playing);
            }
        }

        public void ChangeGameState(GameState newState)
        {
            if (_gameState == newState) return;

            GameState previousState = _gameState;
            _gameState = newState;
            
            // Handle time scale base on game state
            switch (_gameState)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                case GameState.Win:
                case GameState.Loss:
                case GameState.Shop:
                    Time.timeScale = 0f;
                    break;
            }
            
            EventBus.Raise(new GameStateChangedEvent { NewState = _gameState, PreviousState = previousState });
        }

        private void OnWaveEnded(WaveEndedEvent @event)
        {
            if(@event.WaveNumber >= 10)
            {
                ChangeGameState(GameState.Win);
            }
            else
            {
                ChangeGameState(GameState.Shop);
            }
        }

        private void SpawnEntities()
        {
            if (_playerPrefab != null)
            {
                Vector3 spawnPos = _playerSpawnPoint != null ? _playerSpawnPoint.position : Vector3.zero;
                _playerController = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
                
                // We assume the ShipWeaponsPresenter is attached to the player prefab
                _weaponsPresenter = _playerController.GetComponentInChildren<ShipWeaponsPresenter>();

                // Inform the RunStatePresenter about the newly spawned player so the magnet logic works
                if (_runStatePresenter != null)
                {
                    _runStatePresenter.SetPlayerTransform(_playerController.transform);
                }
                
                // Inform the WavePresenter about the newly spawned player
                if (_wavePresenter != null)
                {
                    _wavePresenter.SetPlayerTransform(_playerController.transform);
                }
            }
            else
            {
                Debug.LogError("GameManager: Player Prefab is missing!");
            }

            if (_shopPrefab != null)
            {
                _shopPresenter = Instantiate(_shopPrefab);
            }
            else
            {
                Debug.LogError("GameManager: Shop Prefab is missing!");
            }
        }

        private void OnDestroy()
        {
            if (Bootstrapper.Instance != null)
            {
                Bootstrapper.Instance.Unregister(this);
            }

            if (_shopPresenter != null && _weaponsPresenter != null)
            {
                _shopPresenter.OnWeaponPurchased -= _weaponsPresenter.OnWeaponPurchased;
            }

            if (_wavePresenter != null && _shopPresenter != null)
            {
                _wavePresenter.OnWaveAction -= _shopPresenter.OnWaveAction;
            }

            if (_wavePresenter != null && _hudPresenter != null)
            {
                _wavePresenter.WaveManager.OnMissionProgressChanged -= _hudPresenter.OnMissionProgressUpdate;
            }
        }
    }
}