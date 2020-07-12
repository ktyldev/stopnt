using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GameStateEnd : GameStateBehaviour
{
    [SerializeField] [InputAxis] string m_ContinueButton;
    [SerializeField] GameObject m_GameEndUI;

    public override void StartState()
    {
        m_GameEndUI.SetActive( true );
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
