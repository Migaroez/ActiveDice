using System;
using System.Linq;
using Core.Ioc;
using Core.Models;
using UnityEngine;

public interface IDiceManager
{
    void RollDice();
    event EventHandler<DieSet[]> DiceStoppedRolling;
}

public class DiceManager : MonoBehaviour, IDiceManager
{
    [SerializeField] private GameObject _diePrefab;
    [SerializeField] private int _maxNumberOfDice = 22;
    [SerializeField] private float _offsetSize = 0.5f;

    private Die[] _dice;
    private int _numberOfStillDice;

    public event EventHandler<DieSet[]> DiceStoppedRolling;

    public void Awake()
    {
        DiContainer.Current.Register<IDiceManager,DiceManager>(this);
    }

    // Update is called once per frame
    public void Update()
    {
        // todo remove
        //if (Input.GetKeyUp(KeyCode.R)) SpawnPattern();

        //if (Input.GetKeyUp(KeyCode.E)) RollDice();
    }

    private void SpawnPattern()
    {
        var state = 0;
        var xOffset = 0;
        var zOffset = 0;
        var numStepsBeforeTurn = 1;
        var turnCounter = 1;
        _dice = new Die[_maxNumberOfDice];
        for (var step = 1; step <= _maxNumberOfDice; step++)
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

    public void RollDice()
    {
        foreach (var die in _dice)
        {
            _numberOfStillDice = 0;
            die.Roll();
        }
    }
}