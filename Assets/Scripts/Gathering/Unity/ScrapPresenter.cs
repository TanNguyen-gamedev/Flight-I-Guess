using PrimeTween;
using UnityEngine;
using UnityEngine.Pool;

public class ScrapPresenter: MonoBehaviour
{
    private ScrapModel _model;
    private RunStateModel _runState;
    private IObjectPool<ScrapPresenter> _pool;
    
    [SerializeField] private ScrapConfigSO _config;
    private Transform _playerTransform;
    private System.Numerics.Vector2 _currentPosition;

    public void Init(ScrapModel model, RunStateModel runState, ScrapConfigSO config, Transform player, IObjectPool<ScrapPresenter> pool)
    {
        _model = model;
        _runState = runState;
        _config = config;
        _playerTransform = player;
        _pool = pool;

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
            // Fallback: If PrimeTween doesn't support dynamic tracking in this version,
            // we handle the interpolation manually in Update.
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
        // Removed PrimeTween here. We now handle the movement in Update() 
        // to guarantee it tracks the moving player without version-specific PrimeTween features.
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
            _pool?.Release(this);
        }
    }
}