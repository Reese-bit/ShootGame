using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistenSingleton<GameManager>
{
    public static Action onGameOver;
    public static GameState GameState { get => Instance.gameState; set => Instance.gameState = value; }
    [SerializeField] private GameState gameState = GameState.Playing;
}

public enum GameState
{
    Playing,
    Paused,
    GameOver,
    Scoring
}
