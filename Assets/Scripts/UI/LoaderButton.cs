using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
    
[System.Serializable]
public enum LoadType
{
    MainMenu,
    Controls,
    Gameplay,
    Exit,

    COUNT
}

public class LoaderButton : MonoBehaviour, ISubmitHandler, IPointerClickHandler
{
    [SerializeField] private LoadType m_SceneType;

    public void Dispatch() 
    {
        switch (m_SceneType)
        {
            case LoadType.Gameplay:
                SceneBootstrap.Instance.LoadGameplay();
                break;

            case LoadType.MainMenu:
                SceneBootstrap.Instance.LoadMainMenu();
                break;

            case LoadType.Controls:
                SceneBootstrap.Instance.LoadControls();
                break;

            case LoadType.Exit:
                SceneBootstrap.Instance.ExitGame();
                break;

        }
    }

    public void OnPointerClick( PointerEventData eventData )
        => Dispatch(); 

    public void OnSubmit( BaseEventData eventData )
        => Dispatch(); 
}
