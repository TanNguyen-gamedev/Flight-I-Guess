using System;
using FlightIGuess.Waves.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "KillMissionConfigSO", menuName = "FlightIGuess/WaveMission/KillMissionConfigSO")]
public class KillMissionConfigSO : MissionConfigSO
{
    [SerializeField] private int _kill;
    public override IWaveMission CreateMission()
    {
        KillMission mission = new KillMission(Title, _kill);
        return mission;
    }
}
