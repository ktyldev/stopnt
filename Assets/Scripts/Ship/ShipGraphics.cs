using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGraphics : MonoBehaviour
{
    [SerializeField] private Transform _ship;
    [SerializeField] private ShipController _shipController;

    [SerializeField] private Vector3 _interpolation;
    [SerializeField] private float _rotationInterpolation;
    [SerializeField] private float _topSpeed;

    [Range(0, 1)]
    [SerializeField] private float _glow;

    [SerializeField] private Material _trim;
    [SerializeField] private Color _trimEmissionBase;
    [SerializeField] private float _trimEmissionMaxScale;

    [SerializeField] private Material _engineSpike;
    [SerializeField] private Material _trail;
    [SerializeField] private Color _engineSpikeBaseColour;
    [SerializeField] private Gradient _heatGradient;
    [SerializeField] private AnimationCurve _heatEmission;
    [SerializeField] private float _heatScale;

    void Start()
    {
        transform.position = _ship.position;
    }

    private const string PROP_EMISSION = "_EmissionColor";
    private const string PROP_BASE = "_BaseColor";

    private void LateUpdate()
    {
        _glow = Mathf.InverseLerp(0, _topSpeed, _shipController.Velocity);
        _glow = Mathf.Clamp01(_glow);

        // trim emission
        Color trim = _trimEmissionBase * _glow * _trimEmissionMaxScale;
        _trim.SetColor(PROP_EMISSION, trim);

        // engine glow
        Color engineGlow = _heatGradient.Evaluate(_glow);
        Color engineColour = Color.Lerp(_engineSpikeBaseColour, engineGlow, _glow);
        _engineSpike.SetColor(PROP_BASE, engineColour);
        _trail.SetColor(PROP_BASE, engineGlow);

        Color engineEmission = engineGlow * _heatEmission.Evaluate(_glow) * _heatScale;
        _engineSpike.SetColor(PROP_EMISSION, engineEmission);
        _trail.SetColor(PROP_EMISSION, engineGlow);
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
