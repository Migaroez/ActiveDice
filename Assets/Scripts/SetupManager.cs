using System;
using Core.Ioc;
using TMPro;
using UnityEngine;

public interface ISetupManager
{
    public event EventHandler NewPlayerRequested;
}

public class SetupManager : MonoBehaviour, ISetupManager
{
    public event EventHandler NewPlayerRequested;

    public void Awake()
    {
        DiContainer.Current.Register<ISetupManager,SetupManager>(this);
    }
}
