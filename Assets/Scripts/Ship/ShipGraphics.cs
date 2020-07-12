using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGraphics : MonoBehaviour
{
    [SerializeField] private Transform _ship;

    [SerializeField] private Vector3 _interpolation;
    [SerializeField] private float _rotationInterpolation;

    void Start()
    {
        transform.position = _ship.position;
    }

    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        Vector3 localPos = _ship.InverseTransformPoint(transform.position);
        localPos.x -= (localPos.x * _interpolation.x * Time.fixedDeltaTime);
        localPos.y -= (localPos.y * _interpolation.y * Time.fixedDeltaTime);
        localPos.z -= (localPos.z * _interpolation.z * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, _ship.rotation, _rotationInterpolation * Time.fixedDeltaTime);
        transform.position = _ship.TransformPoint(localPos);
    }
}
