using UnityEngine;
using TMPro;
using NaughtyAttributes;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Text;

    private float m_Value;
    private bool m_Active;

    private const string timeFormat = "{0:00}:{1:00}.{2:000}";

    void LateUpdate()
    {
        if (m_Active)
        {
            m_Value += Time.deltaTime;
            DoDraw();
        }
    }

    private void DoDraw()
    {
        int minutes = Mathf.FloorToInt( m_Value / 60f );
        int seconds = Mathf.FloorToInt( m_Value % 60f );
        int millis = Mathf.FloorToInt( (m_Value * 1000f) % 1000f );

        m_Text.SetText(string.Format(timeFormat, minutes, seconds, millis));
    }

    [Button]
    public void TimerStop()
    {
        m_Active = false;
        DoDraw();
    }

    [Button]
    public void TimerStart()
    {
        m_Active = true;
        DoDraw();
    }

    [Button]
    public void TimerReset()
    {
        m_Value = 0f;
        m_Active = false;
        DoDraw();
    }
}
