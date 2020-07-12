using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    private const string AXIS_ACCELERATE = "Vertical";
    private const string AXIS_RUDDER = "Horizontal";
    private const string AXIS_AIRBRAKE_L = "Airbrake L";
    private const string AXIS_AIRBRAKE_R = "Airbrake R";
    private const string AXIS_BRAKE = "Brake";
    private const string LAYER_MAGNETIC = "Magnetic";

    [SerializeField] private Transform _graphics;
    [SerializeField] private float _tilt;

    [Header("Control")]
    [Range(100, 1000)]
    [SerializeField] private float _acceleration;
    [Range(0, 0.01f)]
    [SerializeField] private float _brake;
    [SerializeField] private float _turn;
    [SerializeField] private float _strafe;
    [SerializeField] private bool _throttleLocked;

    [Header("Air brakes")]
    [SerializeField] private float _airbrakingForce;
    [SerializeField] private float _airbrakeTurn;

    [Header("Hover")]
    [SerializeField] private Transform _repulsor;
    [SerializeField] private float _hoverHeight;
    [SerializeField] private float _minHeight;
    [Range(0, 1)]
    [SerializeField] private float _magnetSmoothing;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float _alignmentSmoothing;
    [Range(0, 1)]
    [SerializeField] private float _tiltSmoothing;

    [Header("Damping")]
    [Range(0, 1)]
    [SerializeField] private float _horizontalDamping;
    [Range(0.001f, 0.01f)]
    [SerializeField] private float _drag;

    private Rigidbody _rb;

    private float _accelerationInput;
    private float _currentRotation;
    private float _currentTilt;
    private int _magnetLayer;

    public float Rudder { get; private set; }
    public AirbrakeInput Airbrake { get; private set; }
    public float Velocity => _rb.velocity.magnitude;
    public struct AirbrakeInput
    {
        public bool Left => Input.GetButton(AXIS_AIRBRAKE_L);
        public bool Right => Input.GetButton(AXIS_AIRBRAKE_R);
        public bool Either => Left || Right;
        public bool Both => Left && Right;
    }

    private Vector3 TrackNormal => TrackHit.normal;
    private Vector3 TrackPosition => TrackHit.point;
    private RaycastHit TrackHit => Physics.Raycast(transform.position, -transform.up, out var hit, Mathf.Infinity, _magnetLayer) ? hit : default;

    private float Inertia => _rb.mass * _rb.velocity.magnitude;

    private void Awake()
    {
        _magnetLayer = LayerMask.GetMask(LAYER_MAGNETIC);
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _accelerationInput = Input.GetAxis(AXIS_ACCELERATE);
        _accelerationInput = Mathf.Clamp01(_accelerationInput);
        Rudder = Input.GetAxis(AXIS_RUDDER);
    }

    private void FixedUpdate()
    {
        var actualHeight = TrackHit.distance;
        var groundPosition = TrackPosition;

        var h = _hoverHeight;
        //var h = Mathf.Lerp(actualHeight, _hoverHeight, _magnetSmoothing);
        h = Mathf.Max(_minHeight, h);
        var targetPosition = groundPosition + TrackNormal * h;
        var targetHeight = targetPosition.y;

        // set hover height
        var pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetHeight, _magnetSmoothing);
        transform.position = pos;

        AlignToTrack();
        Tilt();

        DampHorizontalMovement();
        Drag();

        // apply control
        float input = _throttleLocked ? 1 : _accelerationInput;
        _rb.AddForce(transform.forward * input * _acceleration, ForceMode.Acceleration);

        Turn();
        Brake();
    }

    private void Drag()
    {
        if (_accelerationInput < 0.1f)
        {
            _rb.velocity *= (1 - _drag);
        }
    }

    private void Brake()
    {
        float force = 0;
        if (Airbrake.Both)
        {
            force = _airbrakingForce * 4;
        }
        else if (Airbrake.Either)
        {
            force = _airbrakingForce;
        }

        var regularBrake = Input.GetAxis(AXIS_BRAKE) * _brake;
        force = Mathf.Max(regularBrake, force);

        _rb.velocity *= 1 - force;
    }

    private void Turn()
    {
        float turn =  _turn;
        if (Mathf.Abs(Rudder) > 0.1f)
        {
            bool abLeft = Rudder < 0 && Airbrake.Left;
            bool abRight = Rudder > 0 && Airbrake.Right;
            if (abLeft || abRight)
            {
                turn = _airbrakeTurn;
            }
        }
        else if (!Airbrake.Both)
        {
            // strafe
            if (Airbrake.Left)
            {
                _rb.AddForce(-transform.right * _strafe, ForceMode.VelocityChange);
            }

            if (Airbrake.Right)
            {
                _rb.AddForce(transform.right * _strafe, ForceMode.VelocityChange);
            }
        }

        _currentRotation += Rudder * turn * Time.fixedDeltaTime;
    }

    private void DampHorizontalMovement()
    {
        var localVelocity = transform.InverseTransformDirection(_rb.velocity);

        localVelocity.x *= 1 - _horizontalDamping;

        _rb.velocity = transform.TransformDirection(localVelocity);
    }

    private Quaternion _oldRotation;
    private Quaternion _targetRotation;
    private void AlignToTrack()
    {
        // align to track
        _oldRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
        transform.up = TrackNormal;
        // rotate around local up
        transform.Rotate(_currentRotation * Vector3.up, Space.Self);
        _targetRotation = transform.rotation;
        transform.rotation = Quaternion.Slerp(_oldRotation, _targetRotation, _alignmentSmoothing);

    }

    private void Tilt()
    {
        _graphics.localRotation = Quaternion.identity;
        float targetTilt = Rudder * -_tilt;
        _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, _tiltSmoothing);
        _graphics.Rotate(Vector3.forward * _currentTilt);
    }
}
