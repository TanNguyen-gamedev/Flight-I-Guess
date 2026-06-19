using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    [SerializeField] private UIDocument _uIDocument;
    [SerializeField] private FloatEventChannel _onScoreChange;
    [SerializeField] private VoidEventChannel _onGameOver;
    [SerializeField] private FloatEventChannel _onHighestScore;
    
    private Button _restartButton;
    private Label _scoreText;
    private Label _highScoreText;

    private void Awake()
    {
        _scoreText = _uIDocument.rootVisualElement.Q<Label>("ScoreLabel");
        _restartButton = _uIDocument.rootVisualElement.Q<Button>("RestartButton");
        _highScoreText = _uIDocument.rootVisualElement.Q<Label>("HighScore");
    }

    private void Start()
    {
        _restartButton.style.display = DisplayStyle.None;
        _highScoreText.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        _onScoreChange.OnEventRaise += OnScoreChange;
        _onGameOver.OnEventRaise += OnGameOver;
        _restartButton.clicked += ReloadScene;
        _onHighestScore.OnEventRaise += OnHighScore;
    }

    private void OnDisable()
    {
        _onScoreChange.OnEventRaise -= OnScoreChange;
        _onGameOver.OnEventRaise -= OnGameOver;
        _restartButton.clicked -= ReloadScene;
        _onHighestScore.OnEventRaise -= OnHighScore;
    }

    private void OnScoreChange(float score)
    {
        int roundedScore = Mathf.RoundToInt(score);
        _scoreText.text = "Score: " + roundedScore;
    }

    private void OnGameOver()
    {
        _restartButton.style.display = DisplayStyle.Flex;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnHighScore(float highScore)
    {
        _highScoreText.style.display = DisplayStyle.Flex;
        _highScoreText.text = "HIGH SCORE: " + Mathf.FloorToInt(highScore);
    }

}
