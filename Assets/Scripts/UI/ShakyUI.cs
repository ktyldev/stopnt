using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakyUI : MonoBehaviour
{
    [SerializeField] float m_ShakeMagnitude = 1f;
    RectTransform m_Transform;
    Vector2 m_BasePosition;
    float m_ShakeAmount;

    void Start()
    {
        m_Transform = GetComponent<RectTransform>();
        m_BasePosition = m_Transform.anchoredPosition;
    }

    void LateUpdate()
    {
        float angle = Random.Range( 0f, Mathf.PI * 2f );
        Vector2 shakeVector
            = new Vector2(
                Mathf.Sin( angle ),
                Mathf.Cos( angle )
            ) * m_ShakeAmount * m_ShakeMagnitude;

        m_Transform.anchoredPosition = m_BasePosition + shakeVector;
    }

    public void SetShakeAmount(float magnitude)
    {
        m_ShakeAmount = magnitude;
    }
}
