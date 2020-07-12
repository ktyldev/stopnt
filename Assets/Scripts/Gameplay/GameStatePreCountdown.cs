using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatePreCountdown : GameStateBehaviour
{
    [SerializeField] private float m_Duration;
    [SerializeField] private ShipController m_Controller;
    [SerializeField] private TimerUI m_TimerUI;

    private float m_Timer;

    public override void StartState()
    {
        m_Timer = 0f;
        m_Controller.enabled = false;
        m_Controller.GetComponent<Rigidbody>().isKinematic = true;
        m_TimerUI.TimerReset();
    }

    public override bool UpdateState( float timeDelta )
    {
        m_Timer += timeDelta;
        return m_Timer > m_Duration;
    }

    public override void EndState()
    {
    }
}
