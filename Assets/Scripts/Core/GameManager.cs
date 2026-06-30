using UnityEngine;
using FlightIGuess.Ship.Unity;
using FlightIGuess.Ship.Core;
using FlightIGuess.Shop.Unity;
using FlightIGuess.Shop.Core;
using FlightIGuess.Weapons.Unity;
using FlightIGuess.Combat.Core;
using FlightIGuess.Combat.Unity;
using FlightIGuess.Gathering.Unity;

namespace FlightIGuess.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Configurations")]
        [SerializeField] private HullConfigSO _startingHull;

        [Header("Scene References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private ShipWeaponsPresenter _weaponsPresenter;
        [SerializeField] private RunStatePresenter _runStatePresenter;
        [SerializeField] private ShopPresenter _shopPresenter;
        [SerializeField] private WavePresenter _wavePresenter;
        [SerializeField] private HUD _hudPresenter;
        [SerializeField] private CombatPresenter _combatPresenter;

        private ShipModel _shipModel;
        private ShopModel _shopModel;
        private CombatManager _combatManager;

        private void Start()
        {
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
                    
                    // Register the player's health model with the combat manager
                    if (_combatManager != null && _shipModel.Health != null)
                    {
                        _combatManager.Register(_shipModel.Health);
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
            }

        }

        private void OnDestroy()
        {
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