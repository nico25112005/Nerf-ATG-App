
using Game.Enums;
using System.Collections.Generic;
using System.Linq;

public class SelectGamePresenter
{
    private readonly ISelectGameView view;
    private readonly IPlayerModel playerModel;
    private readonly IGameModel gameModel;
    private readonly IServerModel serverModel;

    private readonly ITcpClientService tcpClientService;


    public SelectGamePresenter(ISelectGameView view, IPlayerModel playerModel, IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService)
    {
        this.view = view;

        this.playerModel = playerModel;
        this.gameModel = gameModel;
        this.serverModel = serverModel;

        this.tcpClientService = tcpClientService;


        serverModel.onActiveGamesChanged += UpdateGameList;

        serverModel.onPingChanged += UpdatePing;
    }

    void UpdateGameList(object sender, GameInfo gameInfo)
    {
        view.UpdateGameList(gameInfo);
    }

    public void Join(string gameName)
    {
        if (!string.IsNullOrEmpty(gameName))
        {
            GameInfo gameInfo = serverModel.ActiveGames.First(g => g.Value.GameName == gameName).Value;

            if(gameInfo.PlayerCount == gameInfo.MaxPlayer)
            {
                view.ShowToastMessage("Game is full", "error");
            }
            if (gameInfo.MaxPlayer == 0)
            {
                view.ShowToastMessage("Game is playing", "error");
            }
            else
            {
                gameModel.gameInfo = gameInfo;
                tcpClientService.Send(ITcpClientService.Connections.Server, new JoinGame(playerModel.Id.ToString(), gameModel.gameInfo.GameId, PacketAction.Add));
                view.LoadNextScene();
            }

        }
        else
        {
            view.ShowToastMessage("No game selected", "error");
        }
    }

    public void CreateGame(string gameName, GameType gameType)
    {       
        tcpClientService.Send(ITcpClientService.Connections.Server, new CreateGame(playerModel.Id.ToString(), gameType, gameName, 16, PacketAction.Generic));
        serverModel.AddOrUpdateActiveGame(new GameInfo(gameType, playerModel.Id.ToString(), gameName, 0, 16, PacketAction.Generic));
        Join(gameName);
    }

    public void UpdatePing(object sender, long ms)
    {
        if(view is IConnectionInfo connectionInfo)
        {
            connectionInfo.UpdatePing(ms);
        }
    }

    public void Quit()
    {
        GameManager.GetInstance().ResetGame();
    }

    public void Dispose()
    {
        serverModel.onActiveGamesChanged -= UpdateGameList;
        serverModel.onPingChanged -= UpdatePing;
    }
}
