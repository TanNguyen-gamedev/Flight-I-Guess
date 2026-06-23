using PrimeTween;
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
        
        [Header("Recoil Visuals")]
        [Tooltip("The transform to apply visual recoil to. If null, falls back to this transform.")]
        [SerializeField] private Transform _visuals;
        
        private Sequence _recoilSequence;
        private WeaponConfigSO _currentWeaponConfig;
        
        public string HardpointId => _hardpointId;
        public WeaponConfigSO InitialWeapon => _initialWeapon;

        public Transform Visuals => _visuals != null ? _visuals : transform;

        public Vector2 Position => transform.position;
        public float InitialAngle => transform.rotation.eulerAngles.z;

        public void SetWeaponConfig(WeaponConfigSO config)
        {
            _currentWeaponConfig = config;
        }

        public void PlayRecoilTween()
        {
            if (Visuals == null || _currentWeaponConfig == null) return;

            // Stop the existing sequence if it's running
            if (_recoilSequence.isAlive)
            {
                _recoilSequence.Stop();
            }
            
            // Reset to origin (assuming the visual child is centered at 0,0,0)
            Visuals.localPosition = Vector3.zero;

            // Recoil backwards along the local Y axis (since 2D sprites face up)
            Vector3 recoilPos = new Vector3(0f, -_currentWeaponConfig.RecoilDistance, 0f);
            
            _recoilSequence = Sequence.Create()
                .Chain(Tween.LocalPosition(Visuals, new TweenSettings<Vector3>(recoilPos, _currentWeaponConfig.RecoilOutSettings)))
                .Chain(Tween.LocalPosition(Visuals, new TweenSettings<Vector3>(Vector3.zero, _currentWeaponConfig.RecoilReturnSettings)));
        }
    }
}
