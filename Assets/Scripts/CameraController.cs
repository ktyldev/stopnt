using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private ShipController _ship;
    [Range(0,1)]
    [SerializeField] private float _smoothing;
    [SerializeField] private float _lookAhead = 10;

    private Transform _target;
    private Vector3 _offset;

    private void Awake()
    {
        _target = _ship.transform;
        _offset = transform.position - _target.position;
    }

    // fixed update to match position updates of vehicle
    private void FixedUpdate()
    {
        var offset = Quaternion.Euler(Vector3.up * _target.rotation.eulerAngles.y) * _offset; 
        var target = _target.position + offset;
        transform.position = Vector3.Lerp(transform.position, target, _smoothing);
        transform.LookAt(_target.position + _target.forward * _lookAhead);
    }
}
