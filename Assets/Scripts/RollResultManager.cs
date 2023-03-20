using System;
using System.Linq;
using Core.Ioc;
using Core.Models;
using TMPro;
using UnityEngine;

public interface IRollResultManager
{
    event EventHandler DiceScored;
}

public class RollResultManager : MonoBehaviour, IRollResultManager
{
    [SerializeField]
    private GameObject _container;

    [SerializeField]
    private TMP_Text _ones, _twos, _threes, _fours, _fives, _sixes;

    private IDiceManager _diceManager;
    private IGameManager _gameManager;
    private IPlayerManager _playerManager;

    public event EventHandler DiceScored;

    void Awake()
    {
        DiContainer.Current.Register<IRollResultManager>(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        _diceManager = DiContainer.Current.Resolve<IDiceManager>();
        _diceManager.DiceStoppedRolling += HandleDiceManagerOnDiceStoppedRolling;

        _gameManager = DiContainer.Current.Resolve<IGameManager>();
        _gameManager.GameStateChanged += GameManagerOnGameStateChanged;

        _playerManager = DiContainer.Current.Resolve<IPlayerManager>();
    }

    private void GameManagerOnGameStateChanged(object sender, GameState e)
    {
        if (e != GameState.Scoring)
            return;
        _container.SetActive(true);
    }

    private void HandleDiceManagerOnDiceStoppedRolling(object sender, DieSet[] dieRollResults)
    {
        _ones.text = dieRollResults.FirstOrDefault(dr => dr.Value == 1).Count.ToString();
        _twos.text = dieRollResults.FirstOrDefault(dr => dr.Value == 2).Count.ToString();
        _threes.text = dieRollResults.FirstOrDefault(dr => dr.Value == 3).Count.ToString();
        _fours.text = dieRollResults.FirstOrDefault(dr => dr.Value == 4).Count.ToString();
        _fives.text = dieRollResults.FirstOrDefault(dr => dr.Value == 5).Count.ToString();
        _sixes.text = dieRollResults.FirstOrDefault(dr => dr.Value == 6).Count.ToString();
    }

    // Unity UI event
    public void HandleDieSelectionClicked(int dieValue)
    {
        if (_gameManager.GameState != GameState.Scoring)
            throw new InvalidGameStateException();

        Debug.Log("Die selection clicked: " + dieValue);
        _playerManager.GetEditablePlayer(_playerManager.Players[_gameManager.CurrentPlayerIndex].Id)
            .ScoreDice(dieValue,_diceManager.LastResult.FirstOrDefault(r => r.Value == dieValue).Count);
        _container.SetActive(false);
        DiceScored?.Invoke(this, null);
    }
}
