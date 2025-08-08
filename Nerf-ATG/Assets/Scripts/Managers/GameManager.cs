
using System.Collections.Generic;

public class GameManager
{
    private static GameManager Instance;

    private IPlayerModel PlayerModel;
    private IGameModel GameModel;
    private IServerModel ServerModel;
    private ITcpClientService TcpClientService;


    private GameManager(IPlayerModel playerModel, IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService)
    {
        PlayerModel = playerModel;
        GameModel = gameModel;
        ServerModel = serverModel;
        TcpClientService = tcpClientService;
    }

    public static GameManager GetInstance()
    {
        if(Instance == null)
        {
            throw new System.Exception("No Objects Installed for GameManager");
        }
        else
        {
            return Instance;
        }
    }

    public static void InstallObjects(IPlayerModel playerModel, IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService)
    {
        Instance = new GameManager(playerModel, gameModel, serverModel, tcpClientService);
    }

    public void ResetGame()
    {

        List<object> objects = new List<object>(){PlayerModel, GameModel, ServerModel, TcpClientService};

        foreach (var obj in objects)
        {
            if(obj is IResetable resetable)
            {
                resetable.Reset();
                UnityEngine.Debug.Log("Reset" + obj);
            }
        }

        TcpClientService.Send(ITcpClientService.Connections.Server, new QuitGame(PlayerModel.Id.ToString(), PacketAction.Generic));
        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectGame");
    }
}
