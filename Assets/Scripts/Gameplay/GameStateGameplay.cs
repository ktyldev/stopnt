using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GameStateGameplay : GameStateBehaviour
{
    [SerializeField] private ShipController m_Controller;
    [SerializeField] private DisplacementMask m_HealthMask;
    [SerializeField] private TimerUI m_TimerUI;
    [SerializeField] private SpeedometerUI m_Speedo;
    [SerializeField] private float m_ShipHealthMax;
    [SerializeField] private float m_InitialImmunity;
    [CurveRange( 0f, 0f, 360f, 1f )] [SerializeField] private AnimationCurve m_DifficultyCurve;
    [ShowNonSerializedField] private float m_ShipHealth;

    public override void StartState()
    {
        m_Controller.enabled = true;
        m_Controller.GetComponent<Rigidbody>().isKinematic = false;
        m_TimerUI.TimerStart();
        m_ShipHealth = m_ShipHealthMax;
    }

    public override bool UpdateState( float timeDelta )
    {
        float difficulty = m_DifficultyCurve.Evaluate( m_TimerUI.Value );
        m_Controller.ForceModifier = difficulty;
        m_Speedo.SetDifficulty( difficulty );

        float currentVelocity = m_Controller.Velocity;

        m_InitialImmunity -= timeDelta;

        if (m_Speedo.InTheRed && m_InitialImmunity < 0f)
        {
            m_ShipHealth -= timeDelta;
        }

        m_HealthMask.SetValue( m_ShipHealth / m_ShipHealthMax );

        return m_ShipHealth <= 0f;
    }

    public override void EndState()
    {
        m_Controller.enabled = false;
        m_Controller.GetComponent<Rigidbody>().isKinematic = true;
        m_TimerUI.TimerStop();
    }
}
