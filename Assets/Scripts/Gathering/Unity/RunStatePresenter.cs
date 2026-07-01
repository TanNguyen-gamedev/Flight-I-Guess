using UnityEngine;
using FlightIGuess.Core;
using SysNum = System.Numerics;

namespace FlightIGuess.Gathering.Unity
{
    public class RunStatePresenter : MonoBehaviour
    {
        [SerializeField] private ScrapConfigSO _config;
        [SerializeField] private Transform _playerTransform;

        private RunStateModel _runStateModel;
        public ScrapConfigSO Config => _config;
        public Transform PlayerTransform => _playerTransform;
        
        public RunStateModel RunStateModel => _runStateModel;

        private void Awake()
        {
            _runStateModel = new RunStateModel();
            
            if (_config != null)
            {
                _runStateModel.SetMagnetRadius(_config.MagnetRange);
            }
            else
            {
                Debug.LogError("RunStatePresenter: Missing ScrapConfigSO!");
            }

              // Register this global state manager to the Bootstrapper so others can find it
            var bootstrapper = Bootstrapper.Instance;
            if (bootstrapper != null)
            {

                bootstrapper.Register(this);
            }
            else
            {
                Debug.LogError("RunStatePresenter: Bootstrapper not found!");
            }
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            // Drive the C# tick from Unity's Update loop
            var playerPos = new SysNum.Vector2(
                _playerTransform.position.x, 
                _playerTransform.position.y
            );
            
            _runStateModel.Tick(playerPos);
        }

        public void SetPlayerTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        private void OnDestroy()
        {
            if (Bootstrapper.Instance != null)
            {
                Bootstrapper.Instance.Unregister(this);
            }
        }
    }
}
