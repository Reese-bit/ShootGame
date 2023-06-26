using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIController : MonoBehaviour
{
    [Header("====PLAYER INPUT====")]
    [SerializeField] private PlayerInput playerInput;

    [Header("====AUDIO DATA====")]
    [SerializeField] private AudioData pauseSFX;
    [SerializeField] private AudioData unpauseSFX;
    
    [Header("====CANVAS====")]
    [SerializeField] private Canvas hudCanvas;
    [SerializeField] private Canvas menuCanvas;

    [Header("====PLAYER INPUT====")] 
    [SerializeField]private Button resumeBotton;
    [SerializeField] private Button optionsBotton;
    [SerializeField] private Button mainmenuBotton;

    private int bottonPressedParameterID = Animator.StringToHash("Pressed");
    
    private void OnEnable()
    {
        playerInput.onPause += Pause;
        playerInput.onUnPause += UnPause;
        
        // resumeBotton.onClick.AddListener(OnResumeBottonClick);
        // optionsBotton.onClick.AddListener(OnOptionsBottonClick);
        // mainmenuBotton.onClick.AddListener(OnMainMenuBottonClick);
        
        ButtonPressedBehaviour.buttonFunctionTable.Add(resumeBotton.gameObject.name,OnResumeBottonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(optionsBotton.gameObject.name,OnOptionsBottonClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(mainmenuBotton.gameObject.name,OnMainMenuBottonClick);
    }

    private void OnDisable()
    {
        playerInput.onPause -= Pause;
        playerInput.onUnPause -= UnPause;
        
        //resumeBotton.onClick.RemoveListener(OnResumeBottonClick);
        //resumeBotton.onClick.RemoveAllListeners();
        //optionsBotton.onClick.RemoveListener(OnOptionsBottonClick);
        //optionsBotton.onClick.RemoveAllListeners();
        //mainmenuBotton.onClick.RemoveListener(OnMainMenuBottonClick);
        //mainmenuBotton.onClick.RemoveAllListeners();
        
        ButtonPressedBehaviour.buttonFunctionTable.Clear();
    }

    void Pause()
    {
        TimeController.instance.Pause();
        hudCanvas.enabled = false;
        menuCanvas.enabled = true;

        GameManager.GameState = GameState.Paused;
        
        playerInput.EnablePauseInput();
        playerInput.SwitchDynamicUpdateMode();
        UIInput.instance.SelectUI(resumeBotton);
        AudioManager.Instance.PlaySFX(pauseSFX);
    }

    void UnPause()
    {
        // when choose other buttons, 
        // pressed 'Tab' to choose resumeButton and Pressed
        resumeBotton.Select();
        resumeBotton.animator.SetTrigger(bottonPressedParameterID);
        //OnResumeBottonClick();
        AudioManager.Instance.PlaySFX(unpauseSFX);
    }

    void OnResumeBottonClick()
    {
        TimeController.instance.UnPause();
        hudCanvas.enabled = true;
        menuCanvas.enabled = false;

        GameManager.GameState = GameState.Playing;
        
        playerInput.EnableGamePlayInput();
        playerInput.SwitchFixedUpdateMode();
    }

    void OnOptionsBottonClick()
    {
        //TODO
        UIInput.instance.SelectUI(optionsBotton);
        playerInput.EnablePauseInput();
    }

    void OnMainMenuBottonClick()
    {
        menuCanvas.enabled = false;
        
        //Load Mainmenu Scene
        SceneLoader.Instance.LoadMainMenu();
    }
}
