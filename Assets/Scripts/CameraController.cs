using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _offset;
    
    [Header("Zoom Settings")]
    [SerializeField] private float _minZoom = 3f;
    [SerializeField] private float _maxZoom = 15f;
    [SerializeField] private float _zoomSensivity = 0.05f;

    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private FloatEventChannel _onMouseScroll;
    

    private float _targetZoom;
    private float _scrollInputY;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _targetZoom = _camera.orthographicSize;
    }

    private void OnEnable()
    {
        _onMouseScroll.OnEventRaise += OnMouseScroll;
    }

    private void OnDisable()
    {
        _onMouseScroll.OnEventRaise -= OnMouseScroll;
    }

    private void Update()
    {
        if(_playerTransform == null)
        {
            return;
        }
        transform.position = _playerTransform.position + _offset;

        if(_scrollInputY != 0)
        {
            _targetZoom -= _scrollInputY * _zoomSensivity;

            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
            // _scrollInputY = 0;
        }

        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, _zoomSpeed * Time.deltaTime);
    }

    private void OnMouseScroll(float mouseScrollY)
    {
        _scrollInputY = mouseScrollY;
    }
}
