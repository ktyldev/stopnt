using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class ScreenWalkingManager : MonoBehaviour
{
    [SerializeField] private float m_MinWalkingPeriod;
    [SerializeField] private float m_MaxWalkingPeriod;

    [SerializeField] private float m_WalkMagnitude;

    public static ScreenWalkingManager Instance;

    private const float k_WalkInterpolant = 0.0008f;

    private Vector2 m_CurrentPosition;
    private Vector2 m_TargetPosition;
    private float m_ResetTimer;

    public static Vector2 WalkPosition => Instance.m_CurrentPosition;

    private void Awake()
    {
        Instance = this;
        ResetWalkCycle();
    }

    private void Update()
    {
        m_CurrentPosition = Vector2.Lerp( m_CurrentPosition, m_TargetPosition, k_WalkInterpolant );
        m_ResetTimer -= Time.deltaTime;

        if (m_ResetTimer < 0.0f)
        {
            ResetWalkCycle();
        }
    }

    private void ResetWalkCycle()
    {
        m_ResetTimer = Random.Range( m_MinWalkingPeriod, m_MaxWalkingPeriod );

        m_TargetPosition = new Vector2(
            Random.Range( -1.0f, 1.0f ),
            Random.Range( -1.0f, 1.0f )
        ) * m_WalkMagnitude;
    }
}
