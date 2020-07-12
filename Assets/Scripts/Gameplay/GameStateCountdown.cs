using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateCountdown : GameStateBehaviour
{
    [SerializeField] TextFlasher m_CountdownText;
    [SerializeField] float m_ElementDuration = 0.8f;
    [SerializeField] float m_TextDisplayDuration = 2.0f;

    private string[] m_CountdownElements = new string[]
    {
        "3",
        "2",
        "1",
        "go!",
    };

    private int m_CurrentElement;
    private float m_ElementTimer;

    public override void StartState()
    {
        m_CurrentElement = 0;
        m_ElementTimer = 0f;
        m_CountdownText.FlashText( m_CountdownElements[ 0 ], m_TextDisplayDuration );
    }

    public override bool UpdateState( float timeDelta )
    {
        m_ElementTimer += timeDelta;

        if (m_ElementTimer >= m_ElementDuration)
        {
            m_CurrentElement++;
            m_CountdownText.FlashText( m_CountdownElements[ m_CurrentElement ], m_TextDisplayDuration );
            m_ElementTimer = 0f;
        }

        return m_CurrentElement == m_CountdownElements.Length - 1;
    }

    public override void EndState()
    {
    }
}
