using Core.Ioc;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private IGameManager _gameManager;
    private IPlayerManager _playerManager;

    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private GameObject _currentPlayerDisplay;
    [SerializeField] private GameObject _rollButton;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = DiContainer.Current.Resolve<IGameManager>();
        _gameManager.GameStateChanged += _gameManager_GameStateChanged;
        _gameManager.ActivePlayerChanged += GameManagerOnActivePlayerChanged;

        _playerManager = DiContainer.Current.Resolve<IPlayerManager>();
    }


    private void _gameManager_GameStateChanged(object sender, GameState gameState)
    {
        _currentPlayerDisplay.SetActive(gameState != GameState.Initializing && gameState != GameState.PassingTurn && gameState != GameState.Finished);
        _rollButton.SetActive(gameState == GameState.ReadyToRoll);
    }

    private void GameManagerOnActivePlayerChanged(object sender, int currentPlayerIndex)
    {
        _playerName.text = _playerManager.Players[currentPlayerIndex].Name;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
