using System;
using System.Linq;
using Core.Ioc;
using Core.Models;
using UnityEngine;

public interface IDiceManager
{
    void SpawnDice(int numberOfDice);
    void RollDice();
    event EventHandler DiceStartedRolling;
    event EventHandler<DieSet[]> DiceStoppedRolling;
}

public class DiceManager : MonoBehaviour, IDiceManager
{
    [SerializeField] private GameObject _diePrefab;
    [SerializeField] private float _offsetSize = 0.5f;

    private Die[] _dice;
    private int _numberOfStillDice;
    private IGameManager _gameManager;

    public event EventHandler DiceStartedRolling;
    public event EventHandler<DieSet[]> DiceStoppedRolling;

    public void Awake()
    {
        DiContainer.Current.Register<IDiceManager, DiceManager>(this);
    }

    public void Start()
    {
        _gameManager = DiContainer.Current.Resolve<IGameManager>();
    }

    public void SpawnDice(int numberOfDice)
    {
        if (_gameManager.GameState != GameState.Initializing && _gameManager.GameState != GameState.PassingTurn)
            throw new InvalidGameStateException();

        Cleanup();

        var state = 0;
        var xOffset = 0;
        var zOffset = 0;
        var numStepsBeforeTurn = 1;
        var turnCounter = 1;
        _dice = new Die[numberOfDice];
        for (var step = 1; step <= numberOfDice; step++)
        {
            _dice[step - 1] = SpawnAndManage(xOffset, zOffset);

            // Move according to state
            switch (state)
            {
                case 0:
                    xOffset += 1;
                    break;
                case 1:
                    zOffset -= 1;
                    break;
                case 2:
                    xOffset -= 1;
                    break;
                case 3:
                    zOffset += 1;
                    break;
            }

            // Change state
            if (step % numStepsBeforeTurn == 0)
            {
                state = (state + 1) % 4;
                turnCounter++;
                if (turnCounter % 2 == 0)
                {
                    numStepsBeforeTurn++;
                }
            }
        }
    }

    public void RollDice()
    {
        if (_gameManager.GameState != GameState.ReadyToRoll)
            throw new InvalidGameStateException();

        DiceStartedRolling?.Invoke(this, null);

        foreach (var die in _dice)
        {
            _numberOfStillDice = 0;
            die.Roll();
        }
    }

    private Die SpawnAndManage(float xOffset, float zOffset)
    {
        var die = Instantiate(_diePrefab, transform.position + new Vector3(xOffset * _offsetSize, 0, zOffset * _offsetSize), Quaternion.identity).GetComponent<Die>();
        die.StoppedRolling += HandleDieStoppedRolling;
        return die;
    }

    private void HandleDieStoppedRolling(object sender, EventArgs e)
    {
        _numberOfStillDice++;
        if (_numberOfStillDice == _dice.Length)
        {
            TallyResults();
            DiceStoppedRolling?.Invoke(this, TallyResults());
        }
    }

    private DieSet[] TallyResults()
    {
        return _dice.GroupBy(die => die.GetCurrentValue())
            .Select(dice => new DieSet(dice.First().CachedValue, dice.Count())).OrderBy(result => result.Value).ToArray();
    }

    private void Cleanup()
    {
        if (_dice?.Length < 0)
        {
            foreach (var die in _dice)
            {
                Destroy(die);
            }
        }
        _dice = null;
    }
}