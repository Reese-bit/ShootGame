using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Canvas HUDCanvas;
    [SerializeField] private AudioData gameOverScreenSFX;

    private Canvas canvas;
    private Animator animator;
    private int exitStateID = Animator.StringToHash("GameOverScreenExit");

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        animator = GetComponent<Animator>();

        canvas.enabled = false;
        animator.enabled = false;
    }

    private void OnEnable()
    {
        GameManager.onGameOver += OnGameOver;

        playerInput.onGameOverScreen += OnGameOverScreen;
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= OnGameOver;

        playerInput.onGameOverScreen -= OnGameOverScreen;
    }

    void OnGameOver()
    {
        HUDCanvas.enabled = false;
        canvas.enabled = true;
        animator.enabled = true;
        playerInput.DisableAllInput(); // ?
    }

    void OnGameOverScreen()
    {
        AudioManager.Instance.PlaySFX(gameOverScreenSFX);
        playerInput.DisableAllInput();
        animator.Play(exitStateID);
        SceneLoader.Instance.LoadScoring(); // TODO
    }

    void EnableGameOverScreenInput()
    {
        playerInput.EnableGameOverScreenInput();
    }
}
