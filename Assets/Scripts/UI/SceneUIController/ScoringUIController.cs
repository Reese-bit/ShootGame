using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ScoringUIController : MonoBehaviour
{
    [Header("----BACKGROUND----")]
    [SerializeField] private Image backGround;
    [SerializeField] private Sprite[] backGroundImages;

    [Header("----SCORING SCREEN----")] 
    [SerializeField] private Canvas scoringCanvas;
    [SerializeField] private Text playerScoreText;
    [SerializeField] private Button buttonMainMenu;
    [SerializeField] private Transform highScoreLeaderBoardContainer;

    [Header("----HIGH SCORE SCREEN----")]
    [SerializeField] private Canvas newHighScoreScreenCanvas;
    [SerializeField] private Button buttonCancel;
    [SerializeField] private Button buttonSubmit;
    [SerializeField] private InputField playerNameInputField;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        ShowRandomBackgroundImages();

        if (ScoreManager.Instance.HasNewHighScore)
        {
            ShowNewHighScoreScreen();
        }
        else
        {
            ShowScoreScreen();
        }
        
        ButtonPressedBehaviour.buttonFunctionTable.Add(buttonMainMenu.gameObject.name,OnButtonMainMenuClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(buttonSubmit.gameObject.name,OnButtonSubmitClick);
        ButtonPressedBehaviour.buttonFunctionTable.Add(buttonCancel.gameObject.name,HideNewHighScreen);
        
        GameManager.GameState = GameState.Scoring;
    }

    private void OnDisable()
    {
        ButtonPressedBehaviour.buttonFunctionTable.Clear();
    }

    void ShowRandomBackgroundImages()
    {
        backGround.sprite = backGroundImages[Random.Range(0, backGroundImages.Length)];
    }

    void ShowNewHighScoreScreen()
    {
        newHighScoreScreenCanvas.enabled = true;
        UIInput.instance.SelectUI(buttonCancel);
    }

    void HideNewHighScreen()
    {
        newHighScoreScreenCanvas.enabled = false;
        ScoreManager.Instance.SavePlayerScoreData();
        ShowRandomBackgroundImages();
        ShowScoreScreen();
    }

    void ShowScoreScreen()
    {
        scoringCanvas.enabled = true;
        playerScoreText.text = ScoreManager.Instance.Score.ToString();
        UIInput.instance.SelectUI(buttonMainMenu);

        // Update high score leaderboard UI
        UpdateHighScoreLeaderBoard();
        
    }

    void UpdateHighScoreLeaderBoard()
    {
        var playerScoreList = ScoreManager.Instance.LoadPlayerScoreData().list;

        for (int i = 0; i < highScoreLeaderBoardContainer.childCount; i++)
        {
            var child = highScoreLeaderBoardContainer.GetChild(i);

            child.Find("Rank").GetComponent<Text>().text = (i + 1).ToString();
            child.Find("Score").GetComponent<Text>().text = (playerScoreList[i].score).ToString();
            child.Find("Name").GetComponent<Text>().text = (playerScoreList[i].playerName);
        }
    }

    void OnButtonMainMenuClick()
    {
        scoringCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenu();
    }

    void OnButtonSubmitClick()
    {
        if (!string.IsNullOrEmpty(playerNameInputField.text))
        {
            ScoreManager.Instance.SetPLayerName(playerNameInputField.text);
        }
        
        HideNewHighScreen();
    }
    
}
