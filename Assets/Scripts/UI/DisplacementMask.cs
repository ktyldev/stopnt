using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplacementMask : MonoBehaviour
{
    [SerializeField] RectTransform m_MaskTransform;
    [SerializeField] RectTransform m_ChildTransform;
    [SerializeField] [Range( 0f, 1f )] float m_MaskAmount;
    [SerializeField] [Range( 0f, 1f )] float m_MaskMin = 0f;
    [SerializeField] [Range( 0f, 1f )] float m_MaskMax = 1f;

    Vector2 m_MaskBasePosition;
    Vector2 m_ChildBasePosition;

    public void SetValue(float amount)
    {
        m_MaskAmount = amount;
    }

    private void Start()
    {
        m_MaskBasePosition = m_MaskTransform.anchoredPosition;
        m_ChildBasePosition = m_ChildTransform.anchoredPosition;
    }

    private void LateUpdate()
    {
        float amt = 1f - Mathf.Lerp( m_MaskMin, m_MaskMax, m_MaskAmount );
        Vector2 offset = new Vector2( amt * m_MaskTransform.sizeDelta.x, 0f );
        m_MaskTransform.anchoredPosition = m_MaskBasePosition - offset;
        m_ChildTransform.anchoredPosition = m_ChildBasePosition + offset;
    }
}
