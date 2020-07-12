using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class GameStateEnd : GameStateBehaviour
{
    [SerializeField] [InputAxis] string m_ContinueButton;
    [SerializeField] GameObject m_GameEndUI;
    [SerializeField] TextMeshProUGUI m_HUDTime;
    [SerializeField] TextMeshProUGUI m_FinalTime;

    public override void StartState()
    {
        m_GameEndUI.SetActive( true );
        m_FinalTime.SetText( m_HUDTime.text );
    }

    public override bool UpdateState( float timeDelta )
    {
        return Input.GetButtonUp(m_ContinueButton);
    }

    public override void EndState()
    {
        SceneBootstrap.Instance.LoadGameplay();
    }
}
