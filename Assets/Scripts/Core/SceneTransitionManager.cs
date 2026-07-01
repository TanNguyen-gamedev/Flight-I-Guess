using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlightIGuess.Core
{
    public class SceneTransitionManager : MonoBehaviour
    {
        private bool _isLoading;
        public async UniTask LoadSceneAsync(string sceneName)
        {
            if (_isLoading)
            {
                return;
            }
            
            _isLoading = true;
            Time.timeScale = 0f;
            
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
            
            Time.timeScale = 1f;
            _isLoading = false;
        }
    }
}