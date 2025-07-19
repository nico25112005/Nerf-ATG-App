

using Game.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameMemberPresenter
{
    IGameMemberView view;
    IGameModel gameModel;
    IPlayerModel playerModel;
    ITcpClientService tcpClientService;



    public GameMemberPresenter( IGameMemberView view, IPlayerModel playerModel, IGameModel gameModel, ITcpClientService tcpClientService)
    {
        this.view = view;
        this.gameModel = gameModel;
        this.playerModel = playerModel;
        this.tcpClientService = tcpClientService;


        gameModel.onPlayerInfoChanged += UpdateMemberList;
        gameModel.onGameStart += NextScene;

        EvalueateGameHost();

    }

    public void UpdateMemberList(object sender, IPlayerInfo members) //IPlayerInfo is not used yet. UpdateMemberList has to be changed first.
    {
        view.UpdateMemberList(gameModel.playerInfo.Values.ToList());
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

    public void Quit()
    {
        //Todo: Global Quit
    }

    public void Spawn()
    {
        gameModel.UpdatePlayerInfo(CreateRandomData.CreatePlayerInfo());
    }
}
