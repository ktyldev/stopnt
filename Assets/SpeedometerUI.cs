using UnityEngine;
using TMPro;
using NaughtyAttributes;
public class SpeedometerUI : MonoBehaviour
{
    const string k_SpeedFormat = "{0}<size=80%>kph</size>";

    [Header("References")]
    [SerializeField] private DisplacementMask m_Throttle;
    [SerializeField] private TextMeshProUGUI m_SpeedText;
    [SerializeField] private ShakyUI[] m_Shakers;
    [SerializeField] private ShipController m_Ship;

    [Header("Speed Settings")]
    [SerializeField] [Range( 0, 1000 )] private float m_TopSpeed;
    [SerializeField] private float m_UnitConversionRatio;
    [ShowNonSerializedField] private float m_CurrentSpeed;

    [Header("Juice")]
    [CurveRange(0,0,1,1)]
    [SerializeField] AnimationCurve m_ShakeCurve;
    [SerializeField] [Range( 0, 1 )] private float m_SpeedInterpolant;
    [ShowNonSerializedField] private float m_DisplayedSpeed;

    private void LateUpdate()
    {
        m_CurrentSpeed = m_Ship.Velocity * m_UnitConversionRatio;
        m_DisplayedSpeed = Mathf.Lerp( m_DisplayedSpeed, m_CurrentSpeed, m_SpeedInterpolant );

        float normalisedSpeed = m_DisplayedSpeed / m_TopSpeed;
        m_SpeedText.SetText( string.Format( k_SpeedFormat, Mathf.RoundToInt( m_DisplayedSpeed ) ) );
        m_Throttle.SetValue( normalisedSpeed );

        float shakeAmount = m_ShakeCurve.Evaluate( normalisedSpeed );

        for ( int i = 0; i < m_Shakers.Length; ++i )
        {
            m_Shakers[ i ].SetShakeAmount( shakeAmount );
        }
    }
}
