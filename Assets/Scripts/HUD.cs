using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private TextMeshProUGUI _scrapText;
    [SerializeField] private TextMeshProUGUI _coresText;

    [Header("Events")]
    [SerializeField] private FloatEventChannel _onScoreChange;
    [SerializeField] private VoidEventChannel _onGameOver;
    [SerializeField] private FloatEventChannel _onHighestScore;

    private void Start()
    {
        if (_restartButton != null) _restartButton.gameObject.SetActive(false);
        if (_highScoreText != null) _highScoreText.gameObject.SetActive(false);
        
        // Initialize text
        if (_scrapText != null) _scrapText.text = "Scrap: 0";
        if (_coresText != null) _coresText.text = "Cores: 0";
        if (_scoreText != null) _scoreText.text = "Score: 0";
    }

    private void OnEnable()
    {
        if (_onScoreChange != null) _onScoreChange.OnEventRaise += OnScoreChange;
        if (_onGameOver != null) _onGameOver.OnEventRaise += OnGameOver;
        if (_onHighestScore != null) _onHighestScore.OnEventRaise += OnHighScore;
        
        if (_restartButton != null) _restartButton.onClick.AddListener(ReloadScene);
        
        // Find ScrapPoolManager and subscribe to resource events
        var scrapManager = FindFirstObjectByType<ScrapPoolManager>();
        if (scrapManager != null && scrapManager.RunStateModel != null)
        {
            scrapManager.RunStateModel.OnTotalScrapChanged += OnScrapChange;
            scrapManager.RunStateModel.OnTotalCoresChanged += OnCoresChange;
        }
    }

    private void OnDisable()
    {
        if (_onScoreChange != null) _onScoreChange.OnEventRaise -= OnScoreChange;
        if (_onGameOver != null) _onGameOver.OnEventRaise -= OnGameOver;
        if (_onHighestScore != null) _onHighestScore.OnEventRaise -= OnHighScore;
        
        if (_restartButton != null) _restartButton.onClick.RemoveListener(ReloadScene);
        
        var scrapManager = FindFirstObjectByType<ScrapPoolManager>();
        if (scrapManager != null && scrapManager.RunStateModel != null)
        {
            scrapManager.RunStateModel.OnTotalScrapChanged -= OnScrapChange;
            scrapManager.RunStateModel.OnTotalCoresChanged -= OnCoresChange;
        }
    }

    private void OnScrapChange(int totalScrap)
    {
        if (_scrapText != null) _scrapText.text = "Scrap: " + totalScrap;
    }
    
    private void OnCoresChange(int totalCores)
    {
        if (_coresText != null) _coresText.text = "Cores: " + totalCores;
    }

    private void OnScoreChange(float score)
    {
        int roundedScore = Mathf.RoundToInt(score);
        if (_scoreText != null) _scoreText.text = "Score: " + roundedScore;
    }

    private void OnGameOver()
    {
        if (_restartButton != null) _restartButton.gameObject.SetActive(true);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnHighScore(float highScore)
    {
        if (_highScoreText != null)
        {
            _highScoreText.gameObject.SetActive(true);
            _highScoreText.text = "HIGH SCORE: " + Mathf.FloorToInt(highScore);
        }
    }
}
