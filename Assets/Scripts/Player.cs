using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private CharacterController _characterController = null;

    [SerializeField]
    private Camera _camera = null;

    [SerializeField]
    private AudioSource _audioDamage = null;
    
    [SerializeField]
    private AutomaticRifle _automaticRifle = null;
    
    [Space]
    
    [SerializeField]
    [Range(0.1f, 2.0f)] 
    private float _mouseSensitivity = 1.0f;
    
    [SerializeField] 
    [Range(1.0f, 5.0f)]
    private float _speed = 0.0f;
    
    [SerializeField]
    [Range(2.0f, 10.0f)] 
    private float _runSpeed = 0.0f;
    
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float _jumpSpeed = 0.0f;
    #endregion

    #region Private Fields
    private Vector3 _move = Vector3.zero;

    private float _horizontalAxis = 0.0f;
    private float _verticalAxis   = 0.0f;
    private float _mouseXAxis     = 0.0f;
    private float _mouseYAxis     = 0.0f;

    private float _verticalSpeed = 0.0f;

    private int _health = 0;
    #endregion

    #region Constants
    private const float CAMERA_ANGLE_LIMIT = 80.0f;
    private const float G                  = -9.81f;

    public const int MAX_HEALTH = 100;
    #endregion

    #region Public Fields
    public static Player instance = null;
    public static Action<int> onHealthChanged = null;
    public static Action onDead = null;
    #endregion

    #region Private Methods
    private void Awake()
    {
        instance = this;

        AddHealth(MAX_HEALTH);
        
        if(Application.platform == RuntimePlatform.WebGLPlayer)
            _mouseSensitivity*=2;
    }

    private void Update()
    {
        InitAxis();
        Move();
        Rotate();
        RotateCamera();
        Aiming();
        Reload();
        Shooting();
    }

    private void InitAxis()
    {
        _horizontalAxis = Input.GetAxis("Horizontal");
        _verticalAxis   = Input.GetAxis("Vertical");
        _mouseXAxis     = Input.GetAxis("Mouse X") * _mouseSensitivity;
        _mouseYAxis     = Input.GetAxis("Mouse Y") * _mouseSensitivity;
    }

    private void Move()
    {
        float speed = getSpeed() * Time.deltaTime;
        
        _horizontalAxis *= speed;
        _verticalAxis   *= speed;
        
        setVerticalSpeed();
        
        _move.Set(_horizontalAxis, _verticalSpeed * Time.deltaTime, _verticalAxis);
        _move = transform.TransformDirection(_move);

        _characterController.Move(_move);
        
        void setVerticalSpeed()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
            {
                _verticalSpeed = _jumpSpeed;
                return;
            }

            if (_characterController.isGrounded)
                _verticalSpeed = -0.001f;
            else
                _verticalSpeed += G  * Time.deltaTime;
        }

        float getSpeed()
        {
            if (Input.GetKey(KeyCode.LeftShift))
                return _runSpeed;
            
            return _speed;
        }
    }

    private void Rotate()
    {
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.y += _mouseXAxis;
        
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    private void RotateCamera()
    {
        Vector3 eulerAngles = _camera.transform.rotation.eulerAngles;
        float angle = eulerAngles.x -= _mouseYAxis;

        clampAngle();
        
        eulerAngles.x = angle;
        
        _camera.transform.rotation = Quaternion.Euler(eulerAngles);

        void clampAngle()
        {
            bool outOfRange = angle > CAMERA_ANGLE_LIMIT && angle < 360.0f - CAMERA_ANGLE_LIMIT;

            if (!outOfRange)
                return;

            angle = angle - CAMERA_ANGLE_LIMIT < 360.0f - CAMERA_ANGLE_LIMIT - angle 
                ? Mathf.Clamp(angle, 0.0f, CAMERA_ANGLE_LIMIT) 
                : Mathf.Clamp(angle, 360.0f - CAMERA_ANGLE_LIMIT, 360.0f);
        }
    }

    private void Aiming()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            _automaticRifle.Aiming();
        
        if (Input.GetKeyUp(KeyCode.Mouse1))
            _automaticRifle.Aiming(true);
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
            _automaticRifle.Reload();
    }

    private void Shooting()
    {
        if (Input.GetKey(KeyCode.Mouse0))
            _automaticRifle.Shooting();
    }
    #endregion

    #region Public Methods
    public void AddHealth(int health)
    {
        if (health < 0)
            _audioDamage.Play();
        
        _health += health;
        _health = Mathf.Clamp(_health, 0, MAX_HEALTH);
        onHealthChanged?.Invoke(_health);

        if (IsDead())
            onDead?.Invoke();
    }

    public void pickUpHealth(int health, out bool relevant)
    {
        relevant = false;
        
        if(IsCompletelyHealthy())
            return;

        AddHealth(health);

        relevant = true;
    }

    public bool IsDead() => _health == 0;

    public bool IsCompletelyHealthy() => _health == 100;

    #endregion
}
