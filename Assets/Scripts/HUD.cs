using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FlightIGuess.Core;
using FlightIGuess.Gathering.Unity;

public class HUD : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Canvas _hudCanvas;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _scrapText;
    [SerializeField] private TextMeshProUGUI _coresText;
    [SerializeField] private TextMeshProUGUI _missionTitleText;
    [SerializeField] private Slider _progressBar;

    [Header("Events")]
    [SerializeField] private FloatEventChannel _onScoreChange;
    [SerializeField] private VoidEventChannel _onGameOver;
    [SerializeField] private FloatEventChannel _onHighestScore;

    private void Start()
    {
        // Initialize text
        if (_scrapText != null) _scrapText.text = "Scrap: 0";
        if (_coresText != null) _coresText.text = "Cores: 0";
        if (_scoreText != null) _scoreText.text = "Score: 0";
        
    }

    private void OnEnable()
    {
        if (_onScoreChange != null) _onScoreChange.OnEventRaise += OnScoreChange;
    
        var runStatePresenter = Bootstrapper.Instance.GetManager<RunStatePresenter>();
        if (runStatePresenter != null && runStatePresenter.RunStateModel != null)
        {
            runStatePresenter.RunStateModel.OnTotalScrapChanged += OnScrapChange;
            runStatePresenter.RunStateModel.OnTotalCoresChanged += OnCoresChange;
        }
    }

    private void OnDisable()
    {
        if (_onScoreChange != null) _onScoreChange.OnEventRaise -= OnScoreChange;
        
        var runStatePresenter = FindFirstObjectByType<FlightIGuess.Gathering.Unity.RunStatePresenter>();
        if (runStatePresenter != null && runStatePresenter.RunStateModel != null)
        {
            runStatePresenter.RunStateModel.OnTotalScrapChanged -= OnScrapChange;
            runStatePresenter.RunStateModel.OnTotalCoresChanged -= OnCoresChange;
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

    public void OnMissionProgressUpdate(string title, float progress)
    {
        if(_missionTitleText != null)
        {
            _missionTitleText.text = title;
        }
        if(_progressBar != null)
        {
            _progressBar.value = progress;
        }
    }

    public void OnWaveAction(bool isWaveEnded)
    {
        _hudCanvas.enabled = !isWaveEnded;
    }

}
