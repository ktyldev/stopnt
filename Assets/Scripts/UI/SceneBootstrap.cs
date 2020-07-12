using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneBootstrap : MonoBehaviour
{
    enum LoadState
    {
        Idle,
        FadeOut,
        LoadScene,
        FadeIn,

        COUNT,
    }

    public static SceneBootstrap Instance { get; private set; }
    public bool CanLoad => m_State == LoadState.Idle;

    [Header("References")]
    [SerializeField] private Image m_Fader;
    [SerializeField] private Canvas m_FadeCanvas;

    [Header( "Scenes" )]
    [SerializeField] private string m_MainMenuScene;
    [SerializeField] private string m_GameplayScene;
    [SerializeField] private string m_ControlsScene;

    private bool m_HasShownControls = false;

    [Header( "Fading" )]
    [SerializeField] private float m_FadeDuration;

    [ShowNonSerializedField]
    private LoadState m_State = LoadState.FadeIn;
    private float m_FadeTimer;
    private string m_SceneToLoad;
    private AsyncOperation m_SceneLoadOperation;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad( this );

            m_State = LoadState.FadeIn;
            m_FadeCanvas.enabled = true;
            OnStateStart( m_State );
        }
        else
        {
            Destroy( this.gameObject );
        }
    }

    private void AdvanceState()
    {
        m_State = (LoadState) ((int)(m_State + 1) % (int)LoadState.COUNT);
        m_FadeTimer = 0f;
        OnStateStart( m_State );
    }

    private void OnStateStart(LoadState state)
    {
        switch (state)
        {
            case LoadState.LoadScene:
                if (m_SceneToLoad == null)
                {
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif

                }
                else
                {
                    m_SceneLoadOperation = SceneManager.LoadSceneAsync(m_SceneToLoad, LoadSceneMode.Single);
                }
                return;

            case LoadState.FadeOut:
                m_FadeCanvas.enabled = true;
                return;

            case LoadState.FadeIn:
                m_SceneLoadOperation = null;
                m_SceneToLoad = null;
                return;

            case LoadState.Idle:
                m_FadeCanvas.enabled = false;
                return;
        }
    }

    private void Update()
    {
        switch (m_State)
        {
            case LoadState.Idle:
                return;

            case LoadState.LoadScene:
                if (m_SceneLoadOperation.isDone)
                    AdvanceState();
                return;

            case LoadState.FadeIn:
            case LoadState.FadeOut:
                m_FadeTimer += Time.deltaTime / m_FadeDuration;
                m_FadeTimer = Mathf.Clamp01( m_FadeTimer );

                if (m_FadeTimer >= 1f)
                {
                    AdvanceState();
                }

                float alpha =
                    m_State == LoadState.FadeOut
                        ? m_FadeTimer
                        : 1f - m_FadeTimer;

                SetFaderAlpha( alpha );

                return;
        }
    }

    public void LoadScene(string sceneName)
    {
        if (CanLoad == false)
            return;

        m_SceneToLoad = sceneName;
        AdvanceState();
    }

    public void LoadGameplay()
    {
        if (m_HasShownControls == false)
        {
            LoadControls();
        }
        else
        {
            LoadScene( m_GameplayScene );
        }
    }

    public void LoadControls()
    {
        LoadScene( m_ControlsScene );
        m_HasShownControls = true;
    }

    public void LoadMainMenu()
        => LoadScene( m_MainMenuScene );

    public void ExitGame()
        => LoadScene( null );

    private void SetFaderAlpha(float alpha)
    {
        m_Fader.color = new Color( 0, 0, 0, alpha );
    }
}
