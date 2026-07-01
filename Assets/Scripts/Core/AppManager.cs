using UnityEngine;
using UnityEngine.SceneManagement;
using FlightIGuess.Weapons.Unity;

namespace FlightIGuess.Core
{
    public class AppManager : MonoBehaviour
    {
        public WeaponConfigSO SelectedStartingWeapon;
        public int MetaCores { get; set; } 

        private void OnDestroy()
        {
            if (Bootstrapper.Instance != null)
            {
                Bootstrapper.Instance.Unregister(this);
            }
        }

        public void LoadGameplayScene()
        {
            SceneManager.LoadScene("Gameplay");
        }

        public void LoadMainMenuScene()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
