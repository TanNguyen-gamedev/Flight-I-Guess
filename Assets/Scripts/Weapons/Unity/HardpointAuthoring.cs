using UnityEngine;

namespace FlightIGuess.Weapons.Unity
{
    /// <summary>
    /// Represents a physical location on the ship.
    /// Provides spatial data to the pure C# HardpointModel.
    /// </summary>
    public class HardpointAuthoring : MonoBehaviour
    {
        [SerializeField] private string _hardpointId;
        public string HardpointId => _hardpointId;

        // In 2D, transform.up is typically the forward firing direction.
        public Vector2 Position => transform.position;
        public Vector2 Direction => transform.up; 
    }
}
