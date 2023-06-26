using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerScore
{
    public int score;
    public string playerName;

    public PlayerScore(int _score, string _playerName)
    {
        this.score = _score;
        this.playerName = _playerName;
    }
}
