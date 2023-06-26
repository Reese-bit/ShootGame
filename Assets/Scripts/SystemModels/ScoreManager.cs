using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : PersistenSingleton<ScoreManager>
{
    #region Score Display
    
    public int Score => score;
    
    private int score;
    private int currentScore;

    [SerializeField] private Vector3 targetScale;

    public void ResetScore()
    {
        score = 0;
        currentScore = 0;
        ScoreDisplay.UpdateText(score);
    }

    public void AddScore(int scorePoint)
    {
        score += scorePoint;
        StartCoroutine(nameof(AddScoreCoroutine));
    }

    IEnumerator AddScoreCoroutine()
    {
        ScoreDisplay.ScaleText(targetScale);
        
        while (score < currentScore)
        {
            score += 1;
            ScoreDisplay.UpdateText(score);

            yield return null;
        }

        ScoreDisplay.ScaleText(Vector3.zero);
    }    

    #endregion

    #region HIGH SCORE SYSTEM

    private readonly string SaveFileName = "playerscore.json";
    private string playerName = "playerName";

    public bool HasNewHighScore => score > LoadPlayerScoreData().list[9].score;

    public void SetPLayerName(string newName)
    {
        playerName = newName;
    }

    public void SavePlayerScoreData()
    {
        var playerScoreData = LoadPlayerScoreData();
        
        playerScoreData.list.Add(new PlayerScore(score,playerName));
        playerScoreData.list.Sort((x,y) => y.score.CompareTo(x.score));
        
        SaveSystem.SaveByJson(SaveFileName,playerScoreData);
    }

    public PlayerScoreData LoadPlayerScoreData()
    {
        var playerScoreData = new PlayerScoreData();

        if (SaveSystem.SaveFileExists(SaveFileName))
        {
            playerScoreData = SaveSystem.LoadFromJson<PlayerScoreData>(SaveFileName);
        }
        else
        {
            while (playerScoreData.list.Count < 10)
            {
                playerScoreData.list.Add(new PlayerScore(0, playerName));
            }
            
            SaveSystem.SaveByJson(SaveFileName,playerScoreData);
        }

        return playerScoreData;
    }

    #endregion
}
