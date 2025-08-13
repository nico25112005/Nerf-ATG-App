

using Game.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class GameMemberPresenter
{
    private readonly IGameMemberView view;
    private readonly IGameModel gameModel;
    private readonly IPlayerModel playerModel;
    private readonly IServerModel serverModel;
    private readonly ITcpClientService tcpClientService;




    public GameMemberPresenter( IGameMemberView view, IPlayerModel playerModel, IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService)
    {
        this.view = view;

        this.playerModel = playerModel;
        this.gameModel = gameModel;
        this.serverModel = serverModel;

        this.tcpClientService = tcpClientService;



        gameModel.onPlayerInfoChanged += UpdateMemberList;
        gameModel.onGameStart += NextScene;

        EvalueateGameHost();

        serverModel.onPingChanged += UpdatePing;

    }

    public void UpdateMemberList(object sender, IPlayerInfo player)
    {
        view.UpdateMemberList(player);
    }

    private void NextScene(object sender, EventArgs e)
    {
        view.MoveToNextScene();
    }


    public void StartGame()
    {
        tcpClientService.Send(ITcpClientService.Connections.Server, new StartGame(playerModel.Id.ToString(), PacketAction.Generic));
    }
    
    private void EvalueateGameHost()
    {
        if(playerModel.Id.ToString() == gameModel.gameInfo.GameId)
        {
            view.ActivateHostPanel();
        }
    }

    public void SwitchTeam(string playerId)
    {
        if((GameType)gameModel.gameInfo.GameType == GameType.TeamDeathMatch)
        {
            tcpClientService.Send(ITcpClientService.Connections.Server, new SwitchTeam(playerId, PacketAction.Generic));
        }
    }

    public void UpdatePing(object sender, long ms)
    {
        if (view is IConnectionInfo connectionInfo)
        {
            connectionInfo.UpdatePing(ms);
        }
    }

    public void Quit()
    {
        tcpClientService.Send(ITcpClientService.Connections.Server, new QuitGame(playerModel.Id.ToString(), PacketAction.Generic));
    }

    public void Dispose()
    {
        gameModel.onPlayerInfoChanged -= UpdateMemberList;
        gameModel.onGameStart -= NextScene;

        serverModel.onPingChanged -= UpdatePing;
    }
}
