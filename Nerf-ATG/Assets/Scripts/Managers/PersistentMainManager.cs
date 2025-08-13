using Assets.Scripts.Presenter;
using UnityEngine;
using Zenject;

public class PersistentMainManager : MonoBehaviour
{
    [Inject]
    private IPlayerModel playerModel;

    [Inject]
    private IGameModel gameModel;

    [Inject]
    private IServerModel serverModel;

    [Inject]
    private ITcpClientService tcpClientService;

    [Inject]
    private IMainThreadExecutor mainThreadExecutor;



    void Awake()
    {
        InitializeServices();
        DontDestroyOnLoad(gameObject);
    }

    private void InitializeServices()
    {
        Debug.Log("Services wurden initialisiert.");

        new TcpDataPresenter(playerModel, gameModel, serverModel, tcpClientService, mainThreadExecutor);
        
    }

    private void OnApplicationQuit()
    {
        if(gameModel.gameInfo != null)
            tcpClientService.Send(ITcpClientService.Connections.Server, new QuitGame(playerModel.Id.ToString(), PacketAction.Generic));
        tcpClientService.CloseAll();
    }
}
