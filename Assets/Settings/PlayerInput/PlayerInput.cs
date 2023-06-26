using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "PlayerInput")]
public class PlayerInput : ScriptableObject, InputActions.IGamePlayActions,InputActions.IPauseActions,InputActions.IGameOverScreenActions
{
    InputActions inputActions;
    public event UnityAction<Vector2> onMove = delegate {};
    public event UnityAction onStopMove = delegate {};
    public event UnityAction onFire = delegate {};
    public event UnityAction onStopFire = delegate {};
    public event UnityAction onDodge = delegate {};
    public event UnityAction onOverDirven = delegate {}; 
    public event UnityAction onPause = delegate {};
    public event UnityAction onLaunchMissile = delegate {};
    
    public event UnityAction onUnPause = delegate {};
    
    public event UnityAction onGameOverScreen = delegate {};

    private void OnEnable() 
    {
        inputActions = new InputActions();

        inputActions.GamePlay.SetCallbacks(this);    
        inputActions.Pause.SetCallbacks(this);
        inputActions.GameOverScreen.SetCallbacks(this);
    }

    private void OnDisable() 
    {
        DisableAllInput();    
    }

    void SwitchActionMap(InputActionMap inputActionMap,bool isUIInput)
    {
        inputActions.Disable();
        inputActionMap.Enable();

        if (isUIInput)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SwitchDynamicUpdateMode()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    }

    public void SwitchFixedUpdateMode()
    {
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
    }

    //diaable all input when play some animations which is unstoppable
    public void DisableAllInput() => inputActions.Disable();

    public void EnableGamePlayInput() => SwitchActionMap(inputActions.GamePlay,false);

    public void EnablePauseInput() => SwitchActionMap(inputActions.Pause,true);

    public void EnableGameOverScreenInput() => SwitchActionMap(inputActions.GameOverScreen,true);

    public void OnMove(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
        //when gameplay get performed phase
        //if(context.phase == InputActionPhase.Performed)
        if(context.performed)
        {
            //it has initialized by a empty delegate
            //if(onMove != null)
                onMove.Invoke(context.ReadValue<Vector2>());
        }

        //if(context.phase == InputActionPhase.Canceled)
        if(context.canceled)
        {
            onStopMove.Invoke();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();

        //if(context.phase == InputActionPhase.Performed)
        if(context.performed)
        {
            onFire.Invoke();
        }

        //if(context.phase == InputActionPhase.Canceled)
        if(context.canceled)
        {
            onStopFire.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onDodge.Invoke();
        }
    }

    public void OnOverDirven(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onOverDirven.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onPause.Invoke();
        }
    }

    public void OnLaunchMissile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLaunchMissile.Invoke();
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onUnPause.Invoke();
        }
    }

    public void OnGameOver(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onGameOverScreen.Invoke();
        }
    }
}
