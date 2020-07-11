using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    private const string AXIS_ACCELERATE = "Vertical";
    private const string AXIS_STRAFE = "Horizontal";
    private const string AXIS_ROTATION = "Rotation";

    [SerializeField] private Transform _com;
    [SerializeField] private Transform _graphics;
    [SerializeField] private float _tilt;

    [Header("Control")]
    [Range(10_000, 100_000)]
    [SerializeField] private float _acceleration;
    [Range(10_000, 100_000)]
    [SerializeField] private float _strafe;
    [SerializeField] private float _rotation;

    [Header("Hover")]
    [SerializeField] private ConfigurableJoint _repulsor;
    [SerializeField] private float _spring;
    [SerializeField] private float _damper;

    private Rigidbody _rb;
    private float _accelerationInput;
    private float _strafeInput;
    private float _rotationInput;

    private void Awake()
    {
        // use the rb that's actually on the floor
        _rb = _repulsor.GetComponent<Rigidbody>();
        _rb.centerOfMass = _com.localPosition;
    }

    private void Start()
    {
        float maxForce = _repulsor.yDrive.maximumForce;
        _repulsor.yDrive = new JointDrive
        {
            positionSpring = _spring,
            positionDamper = _damper,
            maximumForce = maxForce
        };
    }

    private void Update()
    {
        _accelerationInput = Input.GetAxis(AXIS_ACCELERATE);
        _strafeInput = Input.GetAxis(AXIS_STRAFE);
        _rotationInput = Input.GetAxis(AXIS_ROTATION);

        var rot = Quaternion.identity;
        rot *= Quaternion.Euler(Vector3.forward * _strafeInput * -_tilt);
        _graphics.localRotation = rot;
    }

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.up * _rotationInput * _rotation * Time.fixedDeltaTime);

        _rb.AddForce(transform.forward * _accelerationInput * _acceleration, ForceMode.Force);
        _rb.AddForce(transform.right * _strafeInput * _strafe, ForceMode.Force);
    }
}
