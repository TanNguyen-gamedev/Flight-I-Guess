using System;

namespace FlightIGuess.Waves.Core
{
    /// <summary>
    /// Strategy interface for optional in-wave missions.
    /// </summary>
    public interface IWaveMission
    {
        string Title {get;}
        float ProgressPercentage {get;}
        bool IsComplete {get;}
        public event Action<float> OnProgressUpdate;


        void Start();

        void Tick(float deltaTime);
        void OnEnemyDefeated();        
    }
}
