using FlightIGuess.Waves.Core;
using UnityEngine;

public abstract class MissionConfigSO : ScriptableObject
{
    [SerializeField] protected string Title;
    public abstract IWaveMission CreateMission();
}
