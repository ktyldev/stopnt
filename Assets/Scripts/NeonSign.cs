using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonSign : MonoBehaviour
{
    private const string BASE_COLOUR = "_BaseColor";

    [SerializeField] private Renderer _renderer;

    [SerializeField] private Color[] _baseColours;
    [SerializeField] private Color[] _neonColours;

    private void Start()
    {
        // base colour
        int b = UnityEngine.Random.Range(0, _baseColours.Length);
        var baseColour = _baseColours[b];
        var baseProps = new MaterialPropertyBlock();
        baseProps.SetColor(BASE_COLOUR, baseColour);
        _renderer.SetPropertyBlock(baseProps, 2);

        // neon colour
        int n = UnityEngine.Random.Range(0, _neonColours.Length);
        var neonColour = _neonColours[n];
        var neonProps = new MaterialPropertyBlock();
        neonProps.SetColor(BASE_COLOUR, neonColour);
        neonProps.SetColor("_EmissionColor", neonColour * 2);
        _renderer.SetPropertyBlock(neonProps, 0);
        _renderer.SetPropertyBlock(neonProps, 1);

    }
}
