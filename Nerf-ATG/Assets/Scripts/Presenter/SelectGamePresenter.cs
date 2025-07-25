﻿
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

        //serverModel.AddActiveGame(CreateRandomData.CreateGameInfo());
        //serverModel.AddActiveGame(CreateRandomData.CreateGameInfo());
    }

    void UpdateGameList(object sender, GameInfo gameInfo)
    {
        view.UpdateGameList(gameInfo);
    }

    public void Join(string gameName)
    {
        if (!string.IsNullOrEmpty(gameName))
        {
            tcpClientService.Send(ITcpClientService.Connections.Server, new JoinGame(playerModel.Id.ToString(), gameName, PacketAction.Generic));
            gameModel.gameInfo = serverModel.ActiveGames.First(g => g.GameName == gameName);
        }
    }

    public void CreateGame(string gameName, GameType gameType)
    {       
        tcpClientService.Send(ITcpClientService.Connections.Server, new CreateGame(playerModel.Id.ToString(), gameType, gameName, 16, PacketAction.Generic));
        serverModel.AddActiveGame(new GameInfo(gameType, playerModel.Id.ToString(), gameName, 1, 16, PacketAction.Generic));
        Join(gameName);
    }

    public void Quit()
    {
        tcpClientService.CloseAll();
    }

    public void Dispose()
    {
        serverModel.onActiveGamesChanged -= UpdateGameList;
    }
}
