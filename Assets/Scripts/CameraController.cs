using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private ShipController _ship;
    [Range(0,1)]
    [SerializeField] private float _positionSmoothing;
    [SerializeField] private float _lookAhead = 10;
    [SerializeField] private float _lookSide;
    [SerializeField] private float _cameraSlide;
    [Range(0,1)]
    [SerializeField] private float _sideSmoothing;

    private Transform _target;
    private Vector3 _offset;
    private Vector3 _lookAheadPosition;

    private void Awake()
    {
        _target = _ship.transform;
        _offset = transform.position - _target.position;
    }

    // fixed update to match position updates of vehicle
    private void FixedUpdate()
    {
        // set camera position
        //var offset = Quaternion.Euler(Vector3.up * _target.rotation.eulerAngles.y) * _offset; 
        var offset = _target.rotation * _offset;
        var cameraSlide = _target.right * _ship.Rudder * _cameraSlide;
        var target = _target.position + offset + cameraSlide;
        transform.position = Vector3.Lerp(transform.position, target, _positionSmoothing);

        // set look target
        var lookAheadTarget = _target.position + _target.forward * _lookAhead;
        var lookSide = _target.right * _ship.Rudder * _lookSide;
        lookAheadTarget += lookSide;
        _lookAheadPosition = Vector3.Lerp(_lookAheadPosition, lookAheadTarget, _sideSmoothing);
        transform.LookAt(_lookAheadPosition);
    }
}
