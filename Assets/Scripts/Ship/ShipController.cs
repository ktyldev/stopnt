using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    private const string AXIS_ACCELERATE = "Vertical";
    private const string AXIS_STRAFE = "Horizontal";
    private const string AXIS_ROTATION = "Rotation";
    private const string LAYER_MAGNETIC = "Magnetic";

    [SerializeField] private Transform _graphics;
    [SerializeField] private float _tilt;

    [Header("Control")]
    [Range(10_000, 100_000)]
    [SerializeField] private float _acceleration;
    [Range(10_000, 100_000)]
    [SerializeField] private float _strafe;
    [SerializeField] private float _rotation;

    [Header("Hover")]
    [SerializeField] private Transform _repulsor;
    [SerializeField] private float _hoverHeight;
    [SerializeField] private float _repulsion;
    [SerializeField] private float _attraction;
    [Range(0.9f, 1.0f)]
    [SerializeField] private float _horizontalDamping;

    private Rigidbody _rb;

    private float _accelerationInput;
    private float _strafeInput;
    private float _rotationInput;

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
        _strafeInput = Input.GetAxis(AXIS_STRAFE);
        _rotationInput = Input.GetAxis(AXIS_ROTATION);
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
        // TODO: torque to rotate ship, based on airbrakes. make them more effective at speed!
        _currentRotation += _rotationInput * _rotation * Time.fixedDeltaTime;
        _rb.AddForce(transform.forward * _accelerationInput * _acceleration, ForceMode.Force);
        _rb.AddForce(transform.right * _strafeInput * _strafe, ForceMode.Force);
    }

    private void DampHorizontalMovement()
    {
        var localVelocity = transform.InverseTransformDirection(_rb.velocity);
        localVelocity.x *= _horizontalDamping;
        _rb.velocity = transform.TransformDirection(localVelocity);
    }

    private void AlignToTrack()
    {
        transform.rotation = Quaternion.identity;
        transform.up = TrackNormal;
        transform.Rotate(Vector3.up * _currentRotation, Space.Self);

        _graphics.localRotation = Quaternion.identity;
        float tilt = _strafeInput * -_tilt;
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
