using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]   
public class PerspectiveSkewedMenuItem : MonoBehaviour
{
    [SerializeField] float m_SkewAmount;
    RectTransform m_Transform;

    Vector2 m_BasePosition;
    Vector2 m_CurrentOffset;
    Vector2 m_TargetOffset;

    Vector2 m_CurrentWalk;

    const float k_PositionLerp = 0.03f;

    private void Start()
    {
        m_Transform = GetComponent<RectTransform>();
        m_BasePosition = m_Transform.anchoredPosition;
    }

    private void LateUpdate()
    {
        Vector2 cursorWorldSpace = Input.mousePosition;
        Vector2 cursorViewportPosition = Camera.main.ScreenToViewportPoint( cursorWorldSpace );

        m_TargetOffset = new Vector2(
            (0.5f - Mathf.Clamp01( cursorViewportPosition.x )) * 2f,
            (0.5f - Mathf.Clamp01( cursorViewportPosition.y )) * 2f
        );

        m_CurrentOffset = Vector2.Lerp( m_CurrentOffset, m_TargetOffset, k_PositionLerp );

        m_Transform.anchoredPosition = m_BasePosition + ((m_CurrentOffset + ScreenWalkingManager.WalkPosition) * m_SkewAmount);
    }
}
