using Cysharp.Threading.Tasks;
using FlightIGuess.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FlightIGuess.UI
{
    public class MainMenuPresenter : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _quitButton; // Added Quit Button
        [SerializeField] private string _gameplaySceneName = "Gameplay";

        private void Start()
        {
            if (_playButton != null)
            {
                _playButton.onClick.AddListener(OnPlayClicked);
            }
            else
            {
                Debug.LogWarning("MainMenuPresenter: Play button is not assigned!");
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(OnQuitClicked);
            }
        }

        private void OnPlayClicked()
        {
            // Future Phase 6: This is where we will tell the AppManager which weapon the player selected
            // AppManager appManager = Bootstrapper.Instance.GetManager<AppManager>();
            // if (appManager != null) { appManager.SelectedStartingWeapon = ... }

            // We use the global Bootstrapper to find the SceneTransitionManager
            var transitionManager = Bootstrapper.Instance.GetManager<SceneTransitionManager>();
            
            if (transitionManager != null)
            {
                // .Forget() is used because OnPlayClicked is synchronous (void), 
                // but LoadSceneAsync is asynchronous (UniTask).
                transitionManager.LoadSceneAsync(_gameplaySceneName).Forget();
            }
            else
            {
                Debug.LogError("MainMenuPresenter: Could not find SceneTransitionManager on the Bootstrapper!");
            }
        }
        
        private void OnQuitClicked()
        {
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        private void OnDestroy()
        {
            if (_playButton != null)
            {
                _playButton.onClick.RemoveListener(OnPlayClicked);
            }
            if (_quitButton != null)
            {
                _quitButton.onClick.RemoveListener(OnQuitClicked);
            }
        }
    }
}
