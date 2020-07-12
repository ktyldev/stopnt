using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateBehaviour : MonoBehaviour
{
    public abstract void StartState();
    public abstract bool UpdateState(float timeDelta);
    public abstract void EndState();
}

public class GameplayController : MonoBehaviour
{
    enum GameState
    {
        Init,
            
        PreCountdown, // don't do anything to begin with, to let the player get their bearings.
        Countdown, // count down to the start
        Gameplay, // main gameplay state, systems happen here
        EndState, // show some end ui, with survival time, allow players to restart.

        COUNT,
    }

    private GameStateBehaviour m_CurrentStateBehaviour;
    private GameState m_CurrentState;

    [SerializeField] private GameStateBehaviour m_PrecountdownState;
    [SerializeField] private GameStateBehaviour m_CountdownState;
    [SerializeField] private GameStateBehaviour m_GameplayState;
    [SerializeField] private GameStateBehaviour m_EndState;

    private void Start()
    {
        AdvanceState();
    }

    private GameStateBehaviour GetBehaviourForState(GameState state)
    {
        switch (state)
        {
            case GameState.PreCountdown:
                return m_PrecountdownState;

            case GameState.Countdown:
                return m_CountdownState;

            case GameState.Gameplay:
                return m_GameplayState;

            case GameState.EndState:
                return m_EndState;

            default:
                return null;
        }
    }

    private void AdvanceState()
    {
        if (m_CurrentState != GameState.Init)
        {
            m_CurrentStateBehaviour.EndState();
        }

        m_CurrentState = (GameState) ((int)(m_CurrentState + 1) % (int)GameState.COUNT);
        m_CurrentStateBehaviour = GetBehaviourForState( m_CurrentState );
        m_CurrentStateBehaviour.StartState();
    }

    private void Update()
    {
        bool stateFinished = m_CurrentStateBehaviour.UpdateState( Time.deltaTime );

        if (stateFinished == true)
        {
            AdvanceState();
        }
    }
}
