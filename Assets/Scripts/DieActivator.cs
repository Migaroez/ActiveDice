using System;
using System.Collections;
using System.Collections.Generic;
using Core.Ioc;
using UnityEngine;

public interface IDieActivator
{
    event EventHandler DieActivationFinished;
}

public class DieActivator : MonoBehaviour, IDieActivator
{
    private IGameManager _gameManager;

    public event EventHandler DieActivationFinished;

    [SerializeField] private GameObject _container;

    void Awake()
    {
        DiContainer.Current.Register<IDieActivator>(this);
    }

    void Start()
    {
        _gameManager = DiContainer.Current.Resolve<IGameManager>();
        _gameManager.GameStateChanged += GameManagerOnGameStateChanged;
    }

    public void HandleSkipButton()
    {
        DieActivationFinished?.Invoke(this,null);
    }

    private void GameManagerOnGameStateChanged(object sender, GameState e)
    {
        _container.SetActive(e == GameState.DieActivation);
    }
}
