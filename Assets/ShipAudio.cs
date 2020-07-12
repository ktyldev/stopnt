using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAudio : MonoBehaviour
{
    private const string LAYER_WALLS = "Walls";

    [SerializeField] private ShipController _ship;
    [SerializeField] private AudioClip[] _engineNoises;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _unitConversionRatio;
    [Range(0,1)]
    [SerializeField] private float _smoothing;

    [SerializeField] private AudioClip[] _collisionNoises;
    [SerializeField] private AudioSource _collisionSource;

    private readonly List<EngineSample> _samples = new List<EngineSample>();

    private float _step;
    private float _smoothedVelocity;
    private int _wallLayer;

    private class EngineSample
    {
        public AudioSource source;
        public float speed;
    }

    private void Awake()
    {
        var maxSpeed = _maxSpeed * _unitConversionRatio;
        _step = maxSpeed / _engineNoises.Length;

        _wallLayer = LayerMask.NameToLayer(LAYER_WALLS);
    }

    void Start()
    {
        var empty = new GameObject();
        empty.AddComponent<AudioSource>();

        // engine noises
        for (int i = 0; i < _engineNoises.Length; i++)
        {
            var clip = _engineNoises[i];

            var go = Instantiate(empty, transform);
            var source = go.GetComponent<AudioSource>();
            source.clip = clip;
            source.volume = 0;
            source.playOnAwake = false;
            source.loop = true;
            go.name = clip.name;

            var speed = _step * i;

            _samples.Add(new EngineSample
            {
                source = source,
                speed = speed
            });

            Debug.Log($"speed: {speed} source: {source}");
        }
        foreach (var sample in _samples)
        {
            sample.source.Play();
        }

        Destroy(empty);

    }

    void Update()
    {
        _smoothedVelocity = Mathf.Lerp(_smoothedVelocity, _ship.Velocity, _smoothing);

        for (int i = 0; i < _samples.Count; i++)
        {
            _samples[i].source.volume = GetVolume(_smoothedVelocity, _samples[i].speed);
        }
    }

    private float GetVolume(float sample, float reference)
    {
        var distance = Mathf.Abs(sample - reference);

        if (distance > _step)
        {
            return 0;
        }

        return Mathf.InverseLerp(_step, 0, distance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != _wallLayer)
        {
            return;
        }

        if (_collisionSource.isPlaying)
        {
            _collisionSource.Stop();
        }

        int r = Random.Range(0, _collisionNoises.Length);
        _collisionSource.clip = _collisionNoises[r];
        _collisionSource.Play();
        _collisionSource.time = 0.1f;
    }
}
