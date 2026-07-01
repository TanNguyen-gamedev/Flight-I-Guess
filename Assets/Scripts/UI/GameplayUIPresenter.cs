using UnityEngine;
using FlightIGuess.Core;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace FlightIGuess.UI
{
    public class GameplayUIPresenter : MonoBehaviour
    {
        [Header("Canvas References")]
        [SerializeField] private GameObject _pauseMenuCanvas;
        [SerializeField] private GameObject _gameOverCanvas;
        [SerializeField] private GameObject _victoryCanvas;
        [SerializeField] private GameObject _hudCanvas; // We need to toggle the HUD off when in menus

        [Header("Button References")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button[] _mainMenuButtons; // Array to assign all "Return to Main Menu" buttons
        [SerializeField] private Button[] _restartButtons; // Array to assign all "Restart" buttons

        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = Bootstrapper.Instance.GetManager<GameManager>();
            if (_gameManager == null)
            {
                Debug.LogError("GameplayUIPresenter could not find GameManager in the scene!");
            }

            // Wire up UI Buttons via code instead of inspector
            if (_resumeButton != null) _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            
            foreach (var btn in _mainMenuButtons)
            {
                if (btn != null) btn.onClick.AddListener(OnReturnToMainMenuClicked);
            }

            foreach (var btn in _restartButtons)
            {
                if (btn != null) btn.onClick.AddListener(OnRestartButtonClicked);
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(HandleStateChange);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(HandleStateChange);
        }

        private void OnDestroy()
        {
            // Clean up listeners
            if (_resumeButton != null) _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            
            foreach (var btn in _mainMenuButtons)
            {
                if (btn != null) btn.onClick.RemoveListener(OnReturnToMainMenuClicked);
            }

            foreach (var btn in _restartButtons)
            {
                if (btn != null) btn.onClick.RemoveListener(OnRestartButtonClicked);
            }
        }

        private void HandleStateChange(GameStateChangedEvent evt)
        {
            // Turn everything off first
            if (_pauseMenuCanvas != null) _pauseMenuCanvas.SetActive(false);
            if (_gameOverCanvas != null) _gameOverCanvas.SetActive(false);
            if (_victoryCanvas != null) _victoryCanvas.SetActive(false);
            if (_hudCanvas != null) _hudCanvas.SetActive(false);

            // Turn on the specific canvas based on the new state
            switch (evt.NewState)
            {
                case GameState.Playing:
                    if (_hudCanvas != null) _hudCanvas.SetActive(true);
                    break;
                case GameState.Paused:
                    if (_pauseMenuCanvas != null) _pauseMenuCanvas.SetActive(true);
                    if (_hudCanvas != null) _hudCanvas.SetActive(true); // Keep HUD visible behind pause menu
                    break;
                case GameState.Loss:
                    if (_gameOverCanvas != null) _gameOverCanvas.SetActive(true);
                    break;
                case GameState.Win:
                    if (_victoryCanvas != null) _victoryCanvas.SetActive(true);
                    break;
                case GameState.Shop:
                    break;
            }
        }

        // --- Button Click Handlers ---

        private void OnResumeButtonClicked()
        {
            if (_gameManager != null)
            {
                _gameManager.ChangeGameState(GameState.Playing);
            }
        }

        private void OnRestartButtonClicked()
        {
            // To restart, we simply reload the Gameplay scene
            var transitionManager = Bootstrapper.Instance.GetManager<SceneTransitionManager>();
            if (transitionManager != null)
            {
                transitionManager.LoadSceneAsync("Gameplay").Forget();
            }
            else
            {
                Debug.LogError("GameplayUIPresenter: Could not find SceneTransitionManager!");
            }
        }

        private void OnReturnToMainMenuClicked()
        {
            var transitionManager = Bootstrapper.Instance.GetManager<SceneTransitionManager>();
            if (transitionManager != null)
            {
                transitionManager.LoadSceneAsync("MainMenu").Forget();
            }
            else
            {
                Debug.LogError("GameplayUIPresenter: Could not find SceneTransitionManager!");
            }
        }
    }
}
