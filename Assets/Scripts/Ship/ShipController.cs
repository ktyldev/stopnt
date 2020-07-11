using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    private const string AXIS_ACCELERATE = "Vertical";
    private const string AXIS_RUDDER = "Horizontal";
    private const string AXIS_AIRBRAKE = "Rotation";
    private const string LAYER_MAGNETIC = "Magnetic";

    [SerializeField] private Transform _graphics;
    [SerializeField] private float _tilt;

    [Header("Control")]
    [Range(10_000, 100_000)]
    [SerializeField] private float _acceleration;
    [SerializeField] private float _turn;
    [SerializeField] private float _airbrakeTurn;

    [Header("Hover")]
    [SerializeField] private Transform _repulsor;
    [SerializeField] private float _hoverHeight;
    [SerializeField] private float _repulsion;
    [SerializeField] private float _attraction;
    [Range(0.001f, 0.1f)]
    [SerializeField] private float _alignmentSmoothing;

    [Header("Damping")]
    [Range(0, 1)]
    [SerializeField] private float _horizontalDamping;

    private Rigidbody _rb;

    private float _accelerationInput;
    private float _rudderInput;
    private float _airbrakeInput;

    private float _currentRotation;
    private int _magnetLayer;

    private Vector3 TrackNormal => TrackHit.normal;
    private Vector3 TrackPosition => TrackHit.point;
    private RaycastHit TrackHit => Physics.Raycast(_repulsor.position, -_repulsor.up, out var hit, Mathf.Infinity, _magnetLayer) ? hit : default;

    private float Inertia => _rb.mass * _rb.velocity.magnitude;

    private void Awake()
    {
        _magnetLayer = LayerMask.GetMask(LAYER_MAGNETIC);
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _accelerationInput = Input.GetAxis(AXIS_ACCELERATE);
        _rudderInput = Input.GetAxis(AXIS_RUDDER);
        _airbrakeInput = Input.GetAxis(AXIS_AIRBRAKE);
    }

    private void FixedUpdate()
    {

        // apply magnetic forces
        var magneticForceDir = -TrackNormal;
        var magnetDirNormalised = magneticForceDir.normalized;
        var distance = magneticForceDir.magnitude;

        var magneticForce = -magnetDirNormalised * MagneticForce(distance);

        var groundPosition = TrackPosition;
        var targetPosition = groundPosition - magnetDirNormalised * _hoverHeight;
        var targetHeight = targetPosition.y;

        // set hover height
        var pos = transform.position;
        pos.y = targetHeight;
        transform.position = pos;

        AlignToTrack();
        DampHorizontalMovement();

        // apply control
        _rb.AddForce(transform.forward * _accelerationInput * _acceleration, ForceMode.Force);

        // TODO: torque to rotate ship, based on airbrakes. make them more effective at speed!
        //_currentRotation += _airbrakeInput * _rotation * Time.fixedDeltaTime;

        bool sameDir = Mathf.Sign(_rudderInput) == Mathf.Sign(_airbrakeInput);
        bool bothEngaged = Mathf.Abs(_rudderInput) > 0.1f && Mathf.Abs(_airbrakeInput) > 0.1f;
        float turn = (sameDir && bothEngaged) ? _airbrakeTurn : _turn;

        _currentRotation += _rudderInput * turn * Time.fixedDeltaTime;
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

        //transform.Rotate(_rotationInput * Vector3.up * _rotation * Time.fixedDeltaTime, Space.Self);

        _graphics.localRotation = Quaternion.identity;
        float tilt = _rudderInput * -_tilt;
        _graphics.Rotate(Vector3.forward * tilt);
    }

    private float MagneticForce(float distance)
    {
        float x;

        if (distance > _hoverHeight)
        {
            x = distance;
            x = x * x * x;
            x *= -_attraction;
        }
        else
        {
            x = _hoverHeight - distance;
            x = x * x * x;
            x *= _repulsion;
        }

        return x;
    }
}
