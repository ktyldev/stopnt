using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GameStateGameplay : GameStateBehaviour
{
    [SerializeField] private ShipController m_Controller;
    [SerializeField] private DisplacementMask m_HealthMask;
    [SerializeField] private TimerUI m_TimerUI;
    [SerializeField] private float m_ShipHealthMax;
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
        float currentVelocity = m_Controller.Velocity;
        m_ShipHealth -= timeDelta;
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
