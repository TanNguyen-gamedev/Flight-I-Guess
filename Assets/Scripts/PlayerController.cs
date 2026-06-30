
using UnityEngine;
using UnityEngine.InputSystem;
using PrimeTween;
using FlightIGuess.Ship.Core;
using Unity.VisualScripting;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using FlightIGuess.Ship.Unity;
using System;

using FlightIGuess.Combat.Core;

public class PlayerController : MonoBehaviour, IDamageable, IKinematicBody
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
    private bool _isBoosting = false;
    
    private float _currentHeat = 0f;
    private bool _isOverheated = false;
    private bool _boostInput = false;

    private ShipModel _shipModel;

    public HealthModel Health => _shipModel?.Health;
    public float Mass => _rb != null ? _rb.mass : 1f;
    public float VelocityMagnitude => _rb != null ? _rb.linearVelocity.magnitude : 0f;
    public System.Numerics.Vector2 ForwardDirection => new System.Numerics.Vector2(transform.up.x, transform.up.y);


    public void Init(ShipModel shipModel)
    {
        _shipModel = shipModel;
        _shipModel.OnHullUpgrade += HandleHullUpgrade;
        _shipModel.OnShipRecoil += HandleShipRecoil;
        
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
            //_boosterFlame.SetActive(true);
        }
    }

    private void OnMoveCancel(InputAction.CallbackContext callback)
    {
        _moveInput = callback.ReadValue<Vector2>();
        //_boosterFlame.SetActive(false); 
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
        float baseAccel = _shipModel != null ? _shipModel.Stats.Acceleration : 0f;
        float maxSpeed = _shipModel != null ? _shipModel.Stats.MaxSpeed : 10f;
        float currentAccel= _isBoosting ? baseAccel * _boostMultiplier : baseAccel;
        Vector2 thrustDirection = transform.up * _moveInput.y;
        
        // 1. Add acceleration to Velocity
        _rb.linearVelocity += thrustDirection * currentAccel * Time.fixedDeltaTime;
        // 2. Clamp to Max Speed (so we don't drift infinitely fast)
        // NOTE: This clamping might fight against recoil impulse if recoil pushes you over max speed.
        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
         _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
        }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherDamageable = collision.collider.GetComponentInParent<IDamageable>();
        var otherKinematic = collision.collider.GetComponentInParent<IKinematicBody>();

        if (otherDamageable != null || otherKinematic != null)
        {
            var collisionDamage = new CollisionDamage
            {
                EntityA = this,
                BodyA = this,
                EntityB = otherDamageable,
                BodyB = otherKinematic,
                RelativeVelocityMagnitude = collision.relativeVelocity.magnitude
            };
            
            FlightIGuess.Core.EventBus.Raise(collisionDamage);
        }
        
    }

    private void OnMouseScroll(InputAction.CallbackContext callback)
    {
        _mouseScrollY = callback.ReadValue<Vector2>().y;
        _onMouseScroll.RaiseEvent(_mouseScrollY);
    }

    private void HandleShipRecoil(float recoilForce, System.Numerics.Vector2 fireDirection)
    {
        Vector2 direction = - new Vector2(fireDirection.X, fireDirection.Y);
        _rb.AddForce(direction * recoilForce,ForceMode2D.Impulse);
    }

}
