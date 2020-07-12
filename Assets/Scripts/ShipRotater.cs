using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRotater : MonoBehaviour
{
    [SerializeField] public float m_RotationSpeed;
    [SerializeField] public Transform m_Ship;

    void Update()
    {
        m_Ship.Rotate( 0f, 0f, m_RotationSpeed * Time.deltaTime );
    }
}
