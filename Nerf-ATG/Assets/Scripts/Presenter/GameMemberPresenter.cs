

using Game.Enums;
using System;
using System.Collections.Generic;

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


        gameModel.onPlayersChanged += UpdateMemberList;
        gameModel.onGameStart += NextScene;

        EvalueateGameHost();

    }

    public void UpdateMemberList(object sender, List<PlayerInfo> members)
    {
        view.UpdateMemberList(members);
    }

    private void NextScene(object sender, EventArgs e)
    {
        view.MoveToNextScene();
    }


    public void StartGame()
    {
        tcpClientService.Send(ITcpClientService.Connections.Server, new StartGame(playerModel.Id.ToString()));
    }

    private void EvalueateGameHost()
    {
        if(playerModel.Id.ToString() == gameModel.gameInfo.gameId)
        {
            view.ActivateHostPanel();
        }
    }

    public void SwitchTeam(string playerId)
    {
        if(gameModel.gameInfo.gameType == GameType.TeamDeathMatch)
        {
            tcpClientService.Send(ITcpClientService.Connections.Server, new SwitchTeam(playerId));
        }
    }

    public void Quit()
    {
        //Todo: Global Quit
    }

    public void Spawn()
    {
        gameModel.AddPlayer(CreateRandomData.CreatePlayerInfo());
    }
}
