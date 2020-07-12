using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PauseUI : MonoBehaviour
{
    [SerializeField] [InputAxis] private string m_PauseButton;
    [SerializeField] private Button m_ResumeButton;
    [SerializeField] private GameObject m_PauseMenu;

    private void Start()
    {
        m_PauseMenu.SetActive( false );
    }

    private void LateUpdate()
    {
        if (Input.GetButtonUp( m_PauseButton ))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        bool currentActive = m_PauseMenu.activeSelf;
        bool newActive = !currentActive;

        m_PauseMenu.SetActive( newActive );
        Time.timeScale = currentActive ? 1f : 0f;
    }
}
