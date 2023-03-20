using System;
using Assets.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Core.Ioc;
using TMPro;
using UnityEngine;

public interface IPlayerManager
{
    IReadOnlyList<IPlayerViewData> Players { get; }
    event EventHandler PlayerListClosed;
    PlayerData GetEditablePlayer(Guid id);
}

public class PlayerManager : MonoBehaviour, IPlayerManager
{

    [SerializeField] private TMP_InputField _addPlayerName;
    [SerializeField] private GameObject _addPlayerUi;
    [SerializeField] private GameObject _playerListContainer;
    [SerializeField] private GameObject _playerList;
    [SerializeField] private GameObject _playerDisplayPrefab;
    [SerializeField] private float _playerDisplaySpacing = 50;

    [SerializeField] private GameObject _closeButton;
    [SerializeField] private GameObject _addPlayerButton;

    private GameObject[] _playerDisplays;
    private List<PlayerData> _players = new List<PlayerData>();
    private IGameManager _gameManager;
    public IReadOnlyList<IPlayerViewData> Players => _players;

    public event EventHandler PlayerListClosed;

    public void Awake()
    {
        DiContainer.Current.Register<IPlayerManager, PlayerManager>(this);
    }

    public void Start()
    {
        _gameManager = DiContainer.Current.Resolve<IGameManager>();
        UpdateButtons();
    }

    public void OnEnable()
    {
        // possible call before start
        if (_gameManager == null)
            return;

        UpdateButtons();
    }

    public void AddPlayer()
    {
        _players.Add(new PlayerData(_addPlayerName.text, _gameManager.NumberOfDicePerPlayer));
        _addPlayerUi.SetActive(false);
        ShowPlayers();
    }

    public void CancelAddPlayer()
    {
        _addPlayerUi.SetActive(false);
        ShowPlayers();
    }

    public void ShowPlayers()
    {
        _playerDisplays = new GameObject[_players.Count];
        for (var index = 0; index < _players.Count; index++)
        {
            var player = _players[index];
            var uiElement = Instantiate(_playerDisplayPrefab, _playerList.transform);
            var rect = uiElement.GetComponent<RectTransform>();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, index * _playerDisplaySpacing,rect.rect.height);
            uiElement.transform.Find("Name").GetComponent<TMP_Text>().text = player.Name;
            _playerDisplays[index] = uiElement;
        }
        _playerListContainer.SetActive(true);
    }

    public void ClosePlayerManager()
    {
        HidePlayers();
        PlayerListClosed?.Invoke(this, null);
    }

    public void ShowAddPlayer()
    {
        HidePlayers();
        _addPlayerName.text = string.Empty;
        _addPlayerUi.SetActive(true);
        _addPlayerName.Select();
        _addPlayerName.ActivateInputField();
    }

    public PlayerData GetEditablePlayer(Guid id)
    {
        return _players.FirstOrDefault(p => p.Id == id);
    }

    private void HidePlayers()
    {
        if (_playerDisplays?.Length > 0)
        {
            foreach (var display in _playerDisplays)
            {
                Destroy(display);
            }
            _playerDisplays = null;
        }

        _playerListContainer.SetActive(false);
    }

    private void UpdateButtons()
    {
        _closeButton.GetComponentInChildren<TMP_Text>().text =
            _gameManager.GameState == GameState.Initializing ? "Start" : "Close";
        _addPlayerButton.SetActive(_gameManager.GameState == GameState.Initializing);
    }
}
