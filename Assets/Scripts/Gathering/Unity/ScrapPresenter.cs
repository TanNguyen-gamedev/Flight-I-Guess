using PrimeTween;
using UnityEngine;
using UnityEngine.Pool;

///<summary>
/// Presenter for the Scrap Model, handles the presentation of the scrap in the game.
/// This class is responsible for updating the position of the scrap in the scene, and for handling the magnetization of the scrap.
/// </summary>
public class ScrapPresenter: MonoBehaviour
{
    private ScrapModel _model;
    private RunStateModel _runState;
    private IObjectPool<ScrapPresenter> _scrapPool;

    
    [SerializeField] private ScrapConfigSO _config;
    private Transform _playerTransform;
    private System.Numerics.Vector2 _currentPosition;

    public Transform PlayerTransform => _playerTransform;
    public RunStateModel RunStateModel => _runState;

    public void Init(ScrapModel model, RunStateModel runState, ScrapConfigSO config, Transform player, IObjectPool<ScrapPresenter> pool)
    {
        _model = model;
        _runState = runState;
        _config = config;
        _playerTransform = player;
        _scrapPool = pool;

        _model.OnMagnetized += HandleMagnetized;
    }

    private void Update()
    {
        if(!_model.IsMagnetized)
        {
            _currentPosition.X = transform.position.x;
            _currentPosition.Y = transform.position.y;
            _model.Position = _currentPosition;
        }
        else if (_playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                _playerTransform.position, 
                _config.MagnetRange * 2f * Time.deltaTime // Move fast enough to catch the player
            );
            
            // Check if we reached the player
            if (Vector3.Distance(transform.position, _playerTransform.position) < 1f)
            {
                OnReachedPlayer();
            }
        }
    }

    private void HandleMagnetized()
    {
        
    }

    private void OnReachedPlayer()
    {
        // Tell the core logic we arrived
        _runState.CollectScrap(_model);
        
        // Clean up event
        _model.OnMagnetized -= HandleMagnetized;
        
        // Return to pool
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
        {
            _scrapPool?.Release(this);
        }
    }

    public void ForceReturnToPool()
    {
        if (_model != null)
        {
            _model.OnMagnetized -= HandleMagnetized;
        }
        ReturnToPool();
    }
}