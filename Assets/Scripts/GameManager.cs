using System;
using System.Linq;
using Core.Ioc;
using Core.Models;
using UnityEngine;

public interface IGameManager
{
    int CurrentPlayerIndex { get; }
    GameState GameState { get; }
    int NumberOfDicePerPlayer { get; }
    event EventHandler<GameState> GameStateChanged;
    event EventHandler<int> ActivePlayerChanged;
}

public class GameManager : MonoBehaviour, IGameManager
{
    // dependencies
    private IDiceManager _diceManager;
    private IPlayerManager _playerManager;
    private IRollResultManager _rollResultManager;
    private IDieActivator _dieActivator;

    private bool _isPaused = false;
    private GameObject[] _playerObjects;

    [SerializeField] public int NumberOfDicePerPlayer { get; private set; } = 22; //todo move into gameSettings or something
    [SerializeField] private GameObject _playerPrefab;

    public GameState GameState { get; private set; }
    public int CurrentPlayerIndex { get; private set; }

    public event EventHandler<GameState> GameStateChanged;
    public event EventHandler<int> ActivePlayerChanged;

    void Awake()
    {
        DiContainer.Current.Register<IGameManager, GameManager>(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _diceManager = DiContainer.Current.Resolve<IDiceManager>();
        _diceManager.DiceStartedRolling += _diceManager_DiceStartedRolling;
        _diceManager.DiceStoppedRolling += DiceManagerOnDiceStoppedRolling;

        _playerManager = DiContainer.Current.Resolve<IPlayerManager>();
        _playerManager.PlayerListClosed += PlayerManagerOnPlayerListClosed;

        _rollResultManager = DiContainer.Current.Resolve<IRollResultManager>();
        _rollResultManager.DiceScored += RollResultManagerOnDiceScored;

        _dieActivator = DiContainer.Current.Resolve<IDieActivator>();
        _dieActivator.DieActivationFinished += DieActivatorOnDieActivationFinished;
    }

    private void DieActivatorOnDieActivationFinished(object sender, EventArgs e)
    {
        ChangeGameState(GameState.Scoring);
    }

    private void RollResultManagerOnDiceScored(object sender, EventArgs e)
    {
        if (_playerManager.Players.Any(p => p.RemainingDice == 0))
        {
            ChangeGameState(GameState.Finished);
            // todo show winner and overview
            Debug.Log("Game finished");
        }

        ChangeGameState(GameState.PassingTurn);
        ActivateNextPlayer();
        ChangeGameState(GameState.ReadyToRoll);
    }

    private void DiceManagerOnDiceStoppedRolling(object sender, DieSet[] e)
    {
        ChangeGameState(_playerManager.Players[CurrentPlayerIndex].AmountOfScoredDice > 0
            ? GameState.DieActivation
            : GameState.Scoring);
    }

    private void _diceManager_DiceStartedRolling(object sender, EventArgs e)
    {
        ChangeGameState(GameState.Rolling);
    }

    private void PlayerManagerOnPlayerListClosed(object sender, EventArgs e)
    {
        if (GameState == GameState.Initializing)
        {
            SetupPlayerObjects();
        }
    }

    private void ChangeGameState(GameState gameState)
    {
        GameState = gameState;
        GameStateChanged?.Invoke(this, gameState);
    }

    private void SetupPlayerObjects()
    {
        if (GameState != GameState.Initializing)
            throw new InvalidGameStateException();

        // destroy all current
        if (_playerObjects != null)
        {
            foreach (var playerObject in _playerObjects)
            {
                Destroy(playerObject);
            }
        }

        _playerObjects = new GameObject[_playerManager.Players.Count];
        for (var i = 0; i < _playerManager.Players.Count; i++)
        {
            _playerObjects[i] = Instantiate(_playerPrefab);
            _playerObjects[i].SetActive(false);
        }

        CurrentPlayerIndex = -1;
        ActivateNextPlayer();
    }

    private void ActivateNextPlayer()
    {
        if(GameState != GameState.Initializing && GameState != GameState.PassingTurn)
            throw new InvalidGameStateException();

        if(CurrentPlayerIndex >= 0)
            _playerObjects[CurrentPlayerIndex].SetActive(false);

        CurrentPlayerIndex = CurrentPlayerIndex + 1 == _playerManager.Players.Count
            ? 0
            : CurrentPlayerIndex + 1;
        _playerObjects[CurrentPlayerIndex].SetActive(true);
        _diceManager.SpawnDice(_playerManager.Players[CurrentPlayerIndex].RemainingDice);
        ActivePlayerChanged?.Invoke(this, CurrentPlayerIndex);
        ChangeGameState(GameState.ReadyToRoll);
    }
}

public enum GameState
{
    Initializing,
    ReadyToRoll,
    Rolling, 
    DieActivation,
    Scoring,
    PassingTurn,
    Finished
}
public class InvalidGameStateException: SystemException{}