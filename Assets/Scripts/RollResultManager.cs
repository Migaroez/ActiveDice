using System.Linq;
using Core.Ioc;
using Core.Models;
using TMPro;
using UnityEngine;

public class RollResultManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _container;

    [SerializeField]
    private TMP_Text _ones, _twos, _threes, _fours, _fives, _sixes;

    private IDiceManager _diceManager;
    private IGameManager _gameManager;
    private IPlayerManager _playerManager;

    // Start is called before the first frame update
    void Start()
    {
        _diceManager = DiContainer.Current.Resolve<IDiceManager>();
        _diceManager.DiceStoppedRolling += HandleDiceManagerOnDiceStoppedRolling;

        _gameManager = DiContainer.Current.Resolve<IGameManager>();

        _playerManager = DiContainer.Current.Resolve<IPlayerManager>();
    }

    private void HandleDiceManagerOnDiceStoppedRolling(object sender, DieSet[] dieRollResults)
    {
        _ones.text = dieRollResults.FirstOrDefault(dr => dr.Value == 1).Value.ToString();
        _twos.text = dieRollResults.FirstOrDefault(dr => dr.Value == 2).Value.ToString();
        _threes.text = dieRollResults.FirstOrDefault(dr => dr.Value == 3).Value.ToString();
        _fours.text = dieRollResults.FirstOrDefault(dr => dr.Value == 4).Value.ToString();
        _fives.text = dieRollResults.FirstOrDefault(dr => dr.Value == 5).Value.ToString();
        _sixes.text = dieRollResults.FirstOrDefault(dr => dr.Value == 6).Value.ToString();
        _container.SetActive(true);
    }

    public void HandleDieSelectionClicked(int dieValue)
    {
        if (_gameManager.GameState != GameState.Scoring)
            throw new InvalidGameStateException();

        Debug.Log("Die selection clicked: " + dieValue);
        _container.SetActive(false);
    }
}
