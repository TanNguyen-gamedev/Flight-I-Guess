
using UnityEngine;
using UnityEngine.InputSystem;
using PrimeTween;
using FlightIGuess.Ship.Core;
using Unity.VisualScripting;
using UnityEngine.Events;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviour
{
    [Header("Ship Visuals (Zero Allocation Swap)")]
    [SerializeField] private GameObject[] _hullVisuals; // 0 = Fighter, 1 = Corvette, 2 = Cruiser
    
    [Header("Boost Settings")]
    [SerializeField] private float _boostMultiplier = 2.5f;
    [SerializeField] private float _maxHeat = 100f;
    [SerializeField] private float _heatBuildRate = 35f; // Heat per second while boosting
    [SerializeField] private float _heatCoolRate = 15f;  // Heat per second while not boosting
    [SerializeField] private float _overheatPenaltyMultiplier = 2f; // Cooldown is slower if overheated
    
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
    private bool _isBoosting = false;
    
    private float _currentHeat = 0f;
    private bool _isOverheated = false;
    private bool _boostInput = false;

    private ShipModel _shipModel;

    public void Init(ShipModel shipModel)
    {
        _shipModel = shipModel;
        _shipModel.OnHullUpgrade += HandleHullUpgrade;
        
        // Ensure correct visual is enabled on start
        HandleHullUpgrade(_shipModel.Stats);
    }

    private void HandleHullUpgrade(ShipModelStats stats)
    {
        // Zero-allocation visual swap
        for (int i = 0; i < _hullVisuals.Length; i++)
        {
            if (_hullVisuals[i] != null)
            {
                _hullVisuals[i].SetActive(i == (int)stats.HullTier);
            }
        }
    }

    private void Awake()
    {
        _inputSystem = new();
        _rb = GetComponent<Rigidbody2D>();
        // Test purpose only create a ship model
        ShipModelStats stats = new ShipModelStats();
        stats.MaxHP = 100;
        stats.HullTier = HullTier.Fighter;
        stats.ThrustForce = 3000f;
        stats.TurnRate = 180;
        ShipModel test = new ShipModel(stats);
        Init(test);
    }

    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.Move.performed += OnMovePerform;
        _inputSystem.Player.Move.canceled += OnMoveCancel;
        _inputSystem.Player.Sprint.performed += OnBoost;
        _inputSystem.Player.Sprint.canceled += OnBoost;
        _inputSystem.Player.ScrollWheel.performed += OnMouseScroll;
        
    }

    private void OnDisable()
    {
        _inputSystem.Player.Move.performed -= OnMovePerform;
        _inputSystem.Player.Move.canceled -= OnMoveCancel;
        _inputSystem.Player.Sprint.performed -= OnBoost;
        _inputSystem.Player.Sprint.canceled -= OnBoost;
        _inputSystem.Player.ScrollWheel.canceled -= OnMouseScroll;
        _inputSystem.Disable();

        if (_shipModel != null)
        {
            _shipModel.OnHullUpgrade -= HandleHullUpgrade;
        }
    }

    private void OnBoost(InputAction.CallbackContext callback)
    {
        _boostInput = callback.performed;
    }


    private void OnMovePerform(InputAction.CallbackContext callback)
    {
        _moveInput = callback.ReadValue<Vector2>();
        if (_moveInput.y > 0)
        {
            _boosterFlame.SetActive(true);
        }
    }

    private void OnMoveCancel(InputAction.CallbackContext callback)
    {
        _moveInput = callback.ReadValue<Vector2>();
        _boosterFlame.SetActive(false); 
    }

    private void FixedUpdate()
    {
        HandleHeatSystem();
        MovePlayer();
        RotatePlayer();
    }

    private void HandleHeatSystem()
    {
        if (_boostInput && !_isOverheated && _moveInput.y > 0)
        {
            _isBoosting = true;
            _currentHeat += _heatBuildRate * Time.fixedDeltaTime;
            
            if (_currentHeat >= _maxHeat)
            {
                _currentHeat = _maxHeat;
                _isOverheated = true;
                _isBoosting = false;
                // Optional: Fire an event here to trigger an overheat sound/UI
            }
        }
        else
        {
            _isBoosting = false;
            
            if (_currentHeat > 0)
            {
                float coolingRate = _isOverheated ? (_heatCoolRate / _overheatPenaltyMultiplier) : _heatCoolRate;
                _currentHeat -= coolingRate * Time.fixedDeltaTime;
                
                if (_currentHeat <= 0)
                {
                    _currentHeat = 0;
                    _isOverheated = false; // Reset overheat state once fully cooled
                }
            }
        }
    }

    private void MovePlayer()
    {
        // Use stats from ShipModel if initialized, otherwise default to 0 to prevent errors
        float baseThrust = _shipModel != null ? _shipModel.Stats.ThrustForce : 0f;
        float currentThrust = _isBoosting ? baseThrust * _boostMultiplier : baseThrust;
        Vector2 thrustDirection = transform.up * _moveInput.y;
        
        // Apply force in the direction the ship is currently pointing
        _rb.AddForce(thrustDirection * currentThrust * Time.fixedDeltaTime, ForceMode2D.Force);

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
        float baseTurnRate = _shipModel != null ? _shipModel.Stats.TurnRate : 0f;
        float rotationAmount = _moveInput.x * baseTurnRate * Time.fixedDeltaTime;
        
        _rb.MoveRotation(_rb.rotation - rotationAmount);
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     _onGameOver.RaiseEvent();
    //     Instantiate(_exlosionEffect, transform.position, transform.rotation);
    //     Destroy(gameObject);
    // }

    private void OnMouseScroll(InputAction.CallbackContext callback)
    {
        _mouseScrollY = callback.ReadValue<Vector2>().y;
        _onMouseScroll.RaiseEvent(_mouseScrollY);
    }

}
