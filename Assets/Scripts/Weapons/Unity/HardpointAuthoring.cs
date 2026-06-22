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
        [SerializeField] private WeaponConfigSO _initialWeapon;
        
        public string HardpointId => _hardpointId;
        public WeaponConfigSO InitialWeapon => _initialWeapon;

        public Vector2 Position => transform.position;
        public float InitialAngle => transform.rotation.eulerAngles.z;
    }
}
