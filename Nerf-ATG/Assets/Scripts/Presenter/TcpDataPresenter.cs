﻿using System;
using System.Linq;
namespace Assets.Scripts.Presenter
{
    public class TcpDataPresenter
    {
        readonly IServerModel serverModel;
        readonly IGameModel gameModel;
        readonly ITcpClientService tcpClientService;

        public TcpDataPresenter(IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService)
        {
            this.serverModel = serverModel;
            this.gameModel = gameModel;
            this.tcpClientService = tcpClientService;

            tcpClientService.dataReceived += RecivedData;
        }

        private void RecivedData(object sender, byte[] bytes)
        {
            switch ((ServerPacketType)BitConverter.ToInt32(bytes, 0))
            {
                case ServerPacketType.PlayerStatus:
                    break;

                case ServerPacketType.PlayerInfo:
                    PlayerInfo playerinfo = new PlayerInfo(bytes);

                    if (!gameModel.players.Contains(playerinfo))
                    {
                        gameModel.AddPlayer(playerinfo);
                    }
                    else
                    {
                        gameModel.RemovePlayer(playerinfo);
                    }

                    break;

                case ServerPacketType.GameInfo:

                    GameInfo gameinfo = new GameInfo(bytes);

                    if (!serverModel.ActiveGames.Contains(gameinfo))
                    {
                        serverModel.addActiveGame(gameinfo);
                    }
                    else
                    {
                        serverModel.removeActiveGame(gameinfo);
                    }

                    break;

                case ServerPacketType.GameStarted:
                    gameModel.GameStart();
                    break;

            }
        }
    }
}
