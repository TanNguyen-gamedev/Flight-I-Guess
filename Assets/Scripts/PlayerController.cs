
using UnityEngine;
using UnityEngine.InputSystem;
using PrimeTween;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _thrustForce = 10f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _turnRate = 30f;
    [SerializeField] private GameObject _boosterFlame;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _exlosionEffect;
    [SerializeField] private VoidEventChannel _onGameOver;
    [SerializeField] private FloatEventChannel _onMouseScroll;
    private InputSystem_Actions _inputSystem;
    private Rigidbody2D _rb;
    private Vector3 _mousePos;
    private Vector2 _aimDirection;
    private Vector2 _moveInput;
    private float _mouseScrollY;
    private bool _isThursting = false;

    private void Awake()
    {
        _inputSystem = new();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.Move.performed += OnMovePerform;
        _inputSystem.Player.Move.canceled += OnMoveCancel;
        _inputSystem.Player.ScrollWheel.performed += OnMouseScroll;
    }

    private void OnDisable()
    {
        _inputSystem.Player.Move.performed -= OnMovePerform;
        _inputSystem.Player.Move.canceled -= OnMoveCancel;
        _inputSystem.Player.ScrollWheel.canceled -= OnMouseScroll;
        _inputSystem.Disable();
    }

    private void OnMovePerform(InputAction.CallbackContext callback)
    {
        _moveInput = callback.ReadValue<Vector2>();
        _boosterFlame.SetActive(true);
    }

    private void OnMoveCancel(InputAction.CallbackContext callback)
    {
        _moveInput = callback.ReadValue<Vector2>();
        _boosterFlame.SetActive(false); // Fix: also turn off the booster flame!
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        Vector2 thrustDirection = transform.up * _moveInput.y;
        // Apply force in the direction the ship is currently pointing
        _rb.AddForce(thrustDirection * _thrustForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
    }

    private void Update()
    {
        UpdateMousePosition();
    }

    private void UpdateMousePosition()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        _aimDirection = (_mousePos - transform.position).normalized;
    }

    private void RotatePlayer()
    { 
        float rotationAmount = _moveInput.x * _turnRate * Time.fixedDeltaTime;
        
        _rb.MoveRotation(_rb.rotation - rotationAmount);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _onGameOver.RaiseEvent();
        Instantiate(_exlosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnMouseScroll(InputAction.CallbackContext callback)
    {
        _mouseScrollY = callback.ReadValue<Vector2>().y;
        _onMouseScroll.RaiseEvent(_mouseScrollY);
    }

}
