using UnityEngine;
using FlightIGuess.Ship.Unity;
using FlightIGuess.Ship.Core;
using FlightIGuess.Shop.Unity;
using FlightIGuess.Shop.Core;
using FlightIGuess.Weapons.Unity;

public class GameManager : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] private HullConfigSO _startingHull;

    [Header("Scene References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private ShipWeaponsPresenter _weaponsPresenter;
    [SerializeField] private ScrapPoolManager _scrapManager;
    [SerializeField] private ShopPresenter _shopPresenter;
    [SerializeField] private WavePresenter _wavePresenter;

    private ShipModel _shipModel;
    private ShopModel _shopModel;

    private void Start()
    {
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
            }
        }
        else
        {
            Debug.LogError("GameManager: Missing Starting Hull Config!");
        }

        // 3. Create ShopModel and inject into ShopPresenter
        if (_scrapManager != null && _shopPresenter != null && _shipModel != null)
        {
            _shopModel = new ShopModel(_scrapManager.RunStateModel, _shipModel);
            _shopPresenter.Init(_shopModel, _wavePresenter);
        }

        // 4. Wiring the purchase between shop presenter and weapon presenter
        _shopPresenter.OnWeaponPurchased += _weaponsPresenter.OnWeaponPurchased;
    }
}