﻿using System.Collections;
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
    [Range(0, 0.01f)]
    [SerializeField] private float _turn;

    [Header("Air brakes")]
    [SerializeField] private float _airbrakingForce;
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
    [Range(0.001f, 0.01f)]
    [SerializeField] private float _drag;

    private Rigidbody _rb;

    private float _accelerationInput;
    private float _rudderInput;
    private Airbrake _airbrakeInput;
    private struct Airbrake
    {
        public bool Left => Input.GetButton(AXIS_AIRBRAKE_L);
        public bool Right => Input.GetButton(AXIS_AIRBRAKE_R);
        public bool Either => Left || Right;
        public bool Both => Left && Right;
    }

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
        _accelerationInput = Mathf.Clamp01(_accelerationInput);
        _rudderInput = Input.GetAxis(AXIS_RUDDER);
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
        Tilt();

        DampHorizontalMovement();
        Drag();

        // apply control
        _rb.AddForce(transform.forward * _accelerationInput * _acceleration, ForceMode.Acceleration);

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
        if (_airbrakeInput.Both)
        {
            force = _airbrakingForce * 4;
        }
        else if (_airbrakeInput.Either)
        {
            force = _airbrakingForce;
        }

        Debug.Log(Input.GetAxis(AXIS_BRAKE));
        var regularBrake = Input.GetAxis(AXIS_BRAKE) * _brake;
        force = Mathf.Max(regularBrake, force);

        _rb.velocity *= 1 - force;
    }

    private void Turn()
    {
        float turn =  _turn;
        if (Mathf.Abs(_rudderInput) > 0.1f)
        {
            bool abLeft = _rudderInput < 0 && _airbrakeInput.Left;
            bool abRight = _rudderInput > 0 && _airbrakeInput.Right;
            if (abLeft || abRight)
            {
                turn = _airbrakeTurn;
            }
        }

        Debug.Log($"l: {_airbrakeInput.Left} r: {_airbrakeInput.Right}");

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

    }

    private void Tilt()
    {
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
