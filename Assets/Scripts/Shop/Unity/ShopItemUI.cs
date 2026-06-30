using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FlightIGuess.Weapons.Unity;
using JetBrains.Annotations;
using FlightIGuess.Shop.Core;

namespace FlightIGuess.Shop.Unity
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _statsText; // Brotato style stat description
        [SerializeField] private Image _lockIcon;            // Brotato style lock icon
        
        private WeaponConfigSO _weaponConfig;
        private ShopPresenter _shopPresenter;
        private bool _isLocked = false;

        public void Init(WeaponConfigSO config, ShopPresenter presenter)
        {
            _weaponConfig = config;
            _shopPresenter = presenter;

            if(_nameText != null) 
            {
                _nameText.text = config.WeaponId;
            }
            if(_costText != null)
            {
                _costText.text = $"{config.Cost}";
            }
            if(_statsText != null)
            {
                _statsText.text = $"Size: {config.WeaponSize}\nFire Rate: {config.FireRatePerSecond}/s";
            }
            
            if(_lockIcon != null)
            {
                _lockIcon.gameObject.SetActive(false);
            }

            if(_itemIcon != null)
            {
                _itemIcon.sprite = config.WeaponIcon;
            }

            _buyButton.onClick.AddListener(OnBuyClicked);
        }

        // Placeholder for future locking logic
        public void ToggleLock()
        {
            _isLocked = !_isLocked;
            if(_lockIcon != null) 
            {
                _lockIcon.gameObject.SetActive(_isLocked);
            }

        }

        private void OnBuyClicked()
        {
            _shopPresenter.TryBuyWeapon(_weaponConfig, this);
        }

        private void OnDestroy()
        {
            _buyButton.onClick.RemoveListener(OnBuyClicked);
        }

        public void OnItemPurchaseSuccess(ShopItem item)
        {
            Destroy(gameObject);
        }
    }
}