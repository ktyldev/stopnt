using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PulseColours : MonoBehaviour
{
    [SerializeField] private Material[] _materials;
    [SerializeField] private AnimationCurve _pulse;
    [SerializeField] private int _bpm;
    [SerializeField] private Color _colour = Color.black;
    [SerializeField] private float _strength;

    private List<Color> _colours = new List<Color>();

    private float _time;
    private float _beatFrequency => 60f / _bpm;
    private float _beatLengthInSeconds;

    private const string PROP_COLOR = "NearColor";

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            _colours.Add(_materials[i].GetColor(PROP_COLOR));
        }

        _beatLengthInSeconds = _pulse.keys.Last().time;
        _time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _time + _beatFrequency)
        {
            _time = Time.time;
        }

        for (int i = 0; i < _colours.Count; i++)
        {
            var colour = _colours[i];
            float timeDiff = Time.time - _time;

            if (timeDiff <= _beatLengthInSeconds)
            {
                colour = Color.Lerp(
                    colour,
                    _colour,
                    _pulse.Evaluate(timeDiff));
            }

            _materials[i].SetColor(PROP_COLOR, colour);
        }
    }
}
