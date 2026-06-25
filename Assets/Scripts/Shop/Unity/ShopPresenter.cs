using UnityEngine;
using FlightIGuess.Shop.Core;
using System.Collections.Generic;
using TMPro;
using FlightIGuess.Weapons.Unity;
using System;

namespace FlightIGuess.Shop.Unity
{
    /// <summary>
    /// Dumb view that reacts to ShopModel events and forwards UI input.
    /// </summary>
    public class ShopPresenter : MonoBehaviour
    {
        [Header("Shop Settings")]
        [SerializeField] private int _itemsPerRoll = 4;
        [SerializeField] private int _rerollCost = 10;

        [Header("UI References")]
        [SerializeField] private Canvas _shopCanvas;
        [SerializeField] private Transform _shopItemContainer;
        [SerializeField] private ShopItemUI _shopItemPrefab;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private TextMeshProUGUI _scrapCount;
        
        [Header("Brotato Specific UI")]
        [SerializeField] private UnityEngine.UI.Button _rerollButton;
        [SerializeField] private TextMeshProUGUI _rerollCostText;
        [SerializeField] private TextMeshProUGUI _currentScrapText;
        [SerializeField] private UnityEngine.UI.Button _nextWaveButton;
        
        [Header("Data")]
        [SerializeField] private List<WeaponConfigSO> _allAvailableWeapons;
        
        private ShopModel _shopModel;
        private WavePresenter _wavePresenter;
        private List<ShopItemUI> _instantiatedItems = new List<ShopItemUI>();
        private WeaponConfigSO _selectedWeapon;

        // Event for purchasing
        public event Action<WeaponConfigSO> OnWeaponPurchased;

        public void Init(ShopModel shopModel, WavePresenter wavePresenter = null)
        {
            _shopModel = shopModel;
            _wavePresenter = wavePresenter;
            
            _shopModel.OnPurchaseSuccess += OnPurchaseSuccess;
            _shopModel.OnPurchaseFailed += OnPurchaseFailed;

            _shopModel.RunStateModel.OnTotalScrapChanged += OnScrapChange;
            
            if (_rerollButton != null) _rerollButton.onClick.AddListener(OnRerollClicked);
            if (_nextWaveButton != null) _nextWaveButton.onClick.AddListener(OnNextWaveClicked);
            if (_rerollCostText != null) _rerollCostText.text = $"Reroll ({_rerollCost})";
            
            RollShopItems();
        }

        private void OnNextWaveClicked()
        {
            if (_wavePresenter != null)
            {
                _wavePresenter.StartNextWave();
            }
        }
        private void Start()
        {
            // Test if the Shop UI works
            RollShopItems();

            // Hide Canvas on Start
            if(_shopCanvas != null)
            {
                _shopCanvas.gameObject.SetActive(false);
            }
        }

        private void OnRerollClicked()
        {
            // Placeholder: Check if can afford reroll, deduct scrap, then RollShopItems()
            RollShopItems();
        }

        private void RollShopItems()
        {
            // Clear existing items if any (Later we will skip clearing locked items)
            foreach (var item in _instantiatedItems)
            {
                if (item != null) Destroy(item.gameObject);
            }
            _instantiatedItems.Clear();

            // Pick 4 random weapons
            for (int i = 0; i < _itemsPerRoll; i++)
            {
                if (_allAvailableWeapons.Count == 0) break;
                
                var randomWeapon = _allAvailableWeapons[UnityEngine.Random.Range(0, _allAvailableWeapons.Count)];
                
                var uiItem = Instantiate(_shopItemPrefab, _shopItemContainer);
                uiItem.Init(randomWeapon, this);
                _instantiatedItems.Add(uiItem);
            }
        }

        public void TryBuyWeapon(WeaponConfigSO config)
        {
            if (_errorText != null) _errorText.text = ""; // Clear previous errors
            
            ShopItem item = config.CreateShopItem();
            _selectedWeapon = config;
            _shopModel.TryBuyItem(item);
        }
        
        private void OnDestroy()
        {
            if (_shopModel != null)
            {
                _shopModel.OnPurchaseSuccess -= OnPurchaseSuccess;
                _shopModel.OnPurchaseFailed -= OnPurchaseFailed;
            }
        }
        
        private void OnPurchaseSuccess(ShopItem item)
        {
            if (_errorText != null) _errorText.text = $"Bought {item.ItemName}!";
            OnWeaponPurchased?.Invoke(_selectedWeapon);
            // Optional: Play a "cha-ching" sound
        }

        private void OnPurchaseFailed(string message)
        {
            if (_errorText != null) _errorText.text = message;
        }

        private void OnScrapChange(int scrap)
        {
            _scrapCount.text = "Scrap: " + scrap;
        }
    }
}
