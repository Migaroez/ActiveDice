using System;
using System.Collections.Generic;
using Assets.Core.Models;
using Core.Ioc;
using UnityEngine;

public interface IGameManager
{
    int CurrentPlayerIndex { get; }
    GameState GameState { get; }
    int NumberOfDicePerPlayer { get; }
    event EventHandler<GameState> GameStateChanged;
    event EventHandler PlayerAdded;
}

public class GameManager : MonoBehaviour, IGameManager
{
    // dependencies
    private IDiceManager _diceManager;

    private bool _isPaused = false;
    [SerializeField] public int NumberOfDicePerPlayer { get; private set; } = 22; //todo move into gameSettings or something


    public GameState GameState { get; private set; }
    public int CurrentPlayerIndex { get; private set; }

    public event EventHandler<GameState> GameStateChanged;
    public event EventHandler PlayerAdded;

    void Awake()
    {
        DiContainer.Current.Register<IGameManager,GameManager>(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _diceManager = DiContainer.Current.Resolve<IDiceManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ChangeGameState(GameState gameState)
    {
        GameState = gameState;
        GameStateChanged?.Invoke(this,gameState);
    }
}

public enum GameState
{
    Initializing,
    ReadyToRoll,
    Rolling,
    WaitingOnAction,
    ProcessingAction,
    Scoring,
    Passing,
    Finished
}