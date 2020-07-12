using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFlasher : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    private float m_FlashTimer;

    private void Start()
    {
        m_Text.enabled = false;
    }

    public void FlashText(string text, float duration)
    {
        m_Text.SetText( text );
        m_Text.enabled = true;
        m_FlashTimer = duration;
    }

    private void Update()
    {
        m_FlashTimer -= Time.deltaTime;
        if (m_FlashTimer < 0f && m_Text.enabled)
        {
            m_Text.enabled = false;
        }
    }
}
