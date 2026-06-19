using UnityEngine;

public class SaveSystem : MonoBehaviour
{ 
   [SerializeField] private FloatEventChannel _onHighestScore;
   [SerializeField] private FloatEventChannel _onLoadGame;


    private void OnEnable()
    {
        _onHighestScore.OnEventRaise += SaveHighScore;
    }

    private void OnDisable()
    {
        _onHighestScore.OnEventRaise -= SaveHighScore;
    }

    private void SaveHighScore(float highestScore)
    {
        PlayerPrefs.SetFloat("HighScore", highestScore);
    }

    private void Start()
    {
        _onLoadGame.RaiseEvent(PlayerPrefs.GetFloat("HighScore"));
    }

} 
