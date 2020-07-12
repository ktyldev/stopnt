using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeInImage : MonoBehaviour
{
    [SerializeField] private Image m_Image;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private float m_FadeDuration;

    private float m_FadeTimer;

    private void OnEnable()
    {
        m_FadeTimer = m_FadeDuration;
        SetAlpha( 0f );
    }

    void Update()
    {
        m_FadeTimer -= Time.deltaTime;

        float alpha = 1f - (m_FadeTimer / m_FadeDuration);

        SetAlpha( Mathf.Clamp01( alpha ) );
    }

    private void SetAlpha(float alpha)
    {
        if (m_Text != null)
        {
            Color color = m_Text.color;
            color.a = alpha;
            m_Text.color = color;
        }

        if (m_Image != null)
        {
            Color color = m_Image.color;
            color.a = alpha;
            m_Image.color = color;
        }
    }
}
