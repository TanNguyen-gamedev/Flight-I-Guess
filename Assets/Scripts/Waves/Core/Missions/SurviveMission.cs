using System;
using System.Diagnostics;
using FlightIGuess.Waves.Core;

public class SurviveMission : IWaveMission
{

    private string _title;
    private float _progresspercentage;
    private bool _isComplete;
    private float _initialSurviveTime;
    private float _maxDuration;
    public string Title => _title;

    public float ProgressPercentage => _progresspercentage;

    public bool IsComplete => _isComplete;
    public event Action OnSurviveSuccess;
    public event Action<float> OnProgressUpdate;

    public SurviveMission(string title, float duration)
    {
        _title = title;
        if(duration > 0)
        {
            _initialSurviveTime = duration;
            _maxDuration = duration;
        }
        else
        {
            throw new ArgumentException("Survive duration must be greater than 0");
        }
    }
    public void OnEnemyDefeated()
    {
        
    }

    public void Start()
    {
        _progresspercentage = 0f;
        OnProgressUpdate?.Invoke(_progresspercentage);
    }

    public void Tick(float deltaTime)
    {
        if (_isComplete) return;

        _initialSurviveTime -= deltaTime;
        
        // Calculate progress (0 to 1)
        _progresspercentage = Math.Clamp((_maxDuration - _initialSurviveTime) / _maxDuration, 0f, 1f);

        if(_initialSurviveTime <= 0 && !_isComplete)
        {
            _isComplete = true;
            _progresspercentage = 1f;
            OnSurviveSuccess?.Invoke();
        }
        OnProgressUpdate?.Invoke(_progresspercentage);
    }
}