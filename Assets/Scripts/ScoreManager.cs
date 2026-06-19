using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private FloatEventChannel _onEnemyDeath;
    [SerializeField] private FloatEventChannel _onScoreChange;
    [SerializeField] private FloatEventChannel _onHighestScore;
    [SerializeField] private FloatEventChannel _onLoadScore;
    [SerializeField] private VoidEventChannel _onGameOver;
    private float _totalScore = 0f;
    private float _highestScore = 0f;
    private void OnEnable()
    {
        _onEnemyDeath.OnEventRaise += OnEnemyDeath;
        _onLoadScore.OnEventRaise += LoadHighScore;
        _onGameOver.OnEventRaise += OnGameOver;
    }

    private void OnDisable()
    {
        _onEnemyDeath.OnEventRaise -= OnEnemyDeath;
        _onLoadScore.OnEventRaise -= LoadHighScore;
        _onGameOver.OnEventRaise -= OnGameOver;
    }

    private void OnEnemyDeath(float score)
    {
        _totalScore += score;
        _onScoreChange.RaiseEvent(_totalScore);
    }

    private void LoadHighScore(float highestScore)
    {
        _highestScore = highestScore;
    }

    private void OnGameOver()
    {
        if(_totalScore > _highestScore)
        {
            _onHighestScore.RaiseEvent(_totalScore);
        }
        else
        {
            _onHighestScore.RaiseEvent(_highestScore);
        }
    }

}
