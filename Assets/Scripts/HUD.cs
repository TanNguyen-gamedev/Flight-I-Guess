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
    [SerializeField] private Slider _heatBar; // Optional: add a reference for heat UI

    [Header("Events")]
    [SerializeField] private FloatEventChannel _onScoreChange;
    [SerializeField] private VoidEventChannel _onGameOver;
    [SerializeField] private FloatEventChannel _onHighestScore;

    private void Awake()
    {
        Bootstrapper.Instance.Register(this);
    }

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
    
        if (Bootstrapper.Instance.TryGetManager<RunStatePresenter>(out var runStatePresenter))
        {
            if (runStatePresenter.RunStateModel != null)
            {
                runStatePresenter.RunStateModel.OnTotalScrapChanged += OnScrapChange;
                runStatePresenter.RunStateModel.OnTotalCoresChanged += OnCoresChange;
            }
        }
    }

    private void OnDisable()
    {
        if (_onScoreChange != null) _onScoreChange.OnEventRaise -= OnScoreChange;
        
        if (Bootstrapper.Instance.TryGetManager<RunStatePresenter>(out var runStatePresenter))
        {
            if (runStatePresenter.RunStateModel != null)
            {
                runStatePresenter.RunStateModel.OnTotalScrapChanged -= OnScrapChange;
                runStatePresenter.RunStateModel.OnTotalCoresChanged -= OnCoresChange;
            }
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

    public void OnHeatChanged(float currentHeat, float maxHeat)
    {
        if (_heatBar != null)
        {
            _heatBar.value = currentHeat / maxHeat;
        }
    }

    public void OnOverheatStateChanged(bool isOverheated)
    {
        if (_heatBar != null)
        {
            // E.g., Change color to red when overheated, back to normal when cooled
            var fillImage = _heatBar.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = isOverheated ? Color.red : Color.yellow;
            }
        }
    }

    public void OnWaveAction(bool isWaveEnded)
    {
        _hudCanvas.enabled = !isWaveEnded;
    }

    private void OnDestroy()
    {
        if (Bootstrapper.Instance != null)
        {
            Bootstrapper.Instance.Unregister(this);
        }
    }
}
