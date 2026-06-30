using System;
using System.Diagnostics;
using FlightIGuess.Waves.Core;

public class KillMission : IWaveMission
{
    private string _title;
    private float _progresspercentage;
    private bool _isComplete = false;
    private int _currentKillCount = 0;
    private int _totalKillObjective;
    public string Title => _title;

    public float ProgressPercentage => _progresspercentage;

    public bool IsComplete => _isComplete;
    public event Action<int> OnKillSuccess;
    public event Action<float> OnProgressUpdate;
    public KillMission(string title, int kills)
    {
        _title = title;
        _totalKillObjective = kills;
    }
    public void OnEnemyDefeated()
    {
        if (_isComplete) return;

        _currentKillCount++;
        _progresspercentage = Math.Clamp((float)_currentKillCount / _totalKillObjective, 0f, 1f);

        if(_currentKillCount >= _totalKillObjective && !_isComplete)
        {
            _isComplete = true;
            _progresspercentage = 1f;
            OnKillSuccess?.Invoke(_currentKillCount);
        }
        OnProgressUpdate?.Invoke(_progresspercentage);
    }

    public void Start()
    {
        _progresspercentage = 0f;
        OnProgressUpdate?.Invoke(_progresspercentage);
    }

    public void Tick(float deltaTime)
    {
        
    }
}