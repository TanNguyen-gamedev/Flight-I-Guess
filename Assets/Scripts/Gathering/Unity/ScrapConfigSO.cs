using UnityEngine;
using PrimeTween;

[CreateAssetMenu(fileName = "ScrapConfigSO", menuName = "FlightIGuess/Gathering/ScrapConfigSO")]
public class ScrapConfigSO : ScriptableObject
{
    [Header("Gather setting")]
    public float MagnetRange = 10f;
    public float Duration = 1f;
    public TweenSettings FlyDuration;
}
