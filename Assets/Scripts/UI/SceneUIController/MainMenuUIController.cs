using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header("----CANVAS----")] 
    [SerializeField] private Canvas mainMenuCanvs;
    
    [Header("----BUTTON----")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    
    private void OnEnable()
    {
        ButtonPressedBehaviour.buttonFunctionTable.Add(startGameButton.gameObject.name,OnStartGameButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(optionsButton.gameObject.name,ONOptionsButtonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(exitButton.gameObject.name,OnExitButtonClick);
    }

    private void OnDisable()
    {
        ButtonPressedBehaviour.buttonFunctionTable.Clear();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        GameManager.GameState = GameState.Playing;
        UIInput.instance.SelectUI(startGameButton);
    }

    void OnStartGameButtonClick()
    {
        mainMenuCanvs.enabled = false;
        SceneLoader.Instance.LoadGamePlay();
    }

    void ONOptionsButtonClick()
    {
        UIInput.instance.SelectUI(optionsButton);
    }

    void OnExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
