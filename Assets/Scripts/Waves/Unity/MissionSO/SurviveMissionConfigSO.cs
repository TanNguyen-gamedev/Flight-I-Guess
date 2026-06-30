using System;
using FlightIGuess.Waves.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "SurviveMissionConfigSO", menuName = "FlightIGuess/WaveMission/SurviveMissionConfigSO")]
public class SurviveMissionConfigSO : MissionConfigSO
{
    [SerializeField] private float _duration;
    public override IWaveMission CreateMission()
    {
        SurviveMission mission = new SurviveMission(Title, _duration);
        return mission;
    }
}
