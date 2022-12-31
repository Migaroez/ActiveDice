using System;
using Core.Ioc;
using Core.Models;
using UnityEngine;

public interface IGameManager
{
    GameState GameState { get; }
    event EventHandler<GameState> GameStateChanged;
}

public class GameManager : MonoBehaviour, IGameManager
{
    private bool _isPaused = false;
    private IDiceManager _diceManager;

    // Roll => Show Result => Action => Score
    public GameState GameState { get; private set; }

    public event EventHandler<GameState> GameStateChanged;

    void Awake()
    {
        DiContainer.Current.Register<IGameManager>(this);
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

    private void PrintOutResults(DieRollResult[] results)
    {
        foreach (var result in results)
        {
            Debug.Log(result.Value + " : " + result.Count);
        }
    }

    private void ChangeGameState(GameState gameState)
    {
        // todo add validation
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