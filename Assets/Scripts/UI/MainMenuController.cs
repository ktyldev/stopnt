using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Image m_Fader;
    [SerializeField] float m_FadeDuration;
    [SerializeField] string m_GameScene;

    float m_StateProgress;

    public void Update()
    {
        m_StateProgress += Time.deltaTime / m_FadeDuration;
        m_StateProgress = Mathf.Clamp01( m_StateProgress );

        m_Fader.color = new Color( 0f, 0f, 0f, 1f - m_StateProgress );
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene(m_GameScene);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
