﻿using Assets.Scripts.Presenter;
using UnityEngine;
using Zenject;

public class PersistentMainManager : MonoBehaviour
{
    [Inject]
    private IGameModel gameModel;

    [Inject]
    private IServerModel serverModel;

    [Inject]
    private ITcpClientService tcpClientService;



    void Awake()
    {

        InitializeServices();
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeServices()
    {
        Debug.Log("Services wurden initialisiert.");

        new TcpDataPresenter(gameModel, serverModel, tcpClientService);
        
    }

}
