using Game.Enums;

namespace Assets.Scripts.Presenter
{
    public class TcpDataPresenter
    {
        private readonly IPlayerModel playerModel;
        private readonly IGameModel gameModel;
        private readonly IServerModel serverModel;
        private readonly ITcpClientService tcpClientService;

        public TcpDataPresenter(IPlayerModel playerModel, IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService)
        {
            this.playerModel = playerModel;
            this.serverModel = serverModel;
            this.gameModel = gameModel;
            this.tcpClientService = tcpClientService;

            this.tcpClientService.dataReceived += RecivedData;
        }

        private void RecivedData(object sender, byte[] bytes)
        {
            if((ITcpClientService.Connections)sender == ITcpClientService.Connections.Server)
            {
                switch ((PacketType)bytes[0])
                {
                    case PacketType.GameInfo: HandleGameInfo(new GameInfo(bytes)); break;
                    case PacketType.GameStarted: HandleGameStarted(new GameStarted(bytes)); break;
                    case PacketType.PlayerInfo: HandlePlayerInfo(new PlayerInfo(bytes)); break;
                    case PacketType.PlayerStatus: HandlePlayerStatus(new PlayerStatus(bytes)); break;
                    case PacketType.BaseLocation: HandleBaseLocation(new BaseLocation(bytes)); break;
                    case PacketType.ReadyPlayerCount: HandleReadyPlayerCount(new ReadyPlayerCount(bytes)); break;
                    case PacketType.MapPoint: HandleMapPoint(new MapPoint(bytes)); break;
                }
            }
        }

        // ---------- Server Handlers ----------

        private void HandleGameInfo(GameInfo gameInfo)
        {
            switch (gameInfo.Action)
            {
                case PacketAction.Add: serverModel.AddActiveGame(gameInfo); break;
                case PacketAction.Update: serverModel.UpdateActiveGame(gameInfo); break;
                case PacketAction.Remove: serverModel.RemoveActiveGame(gameInfo); break;
            }
        }

        private void HandleGameStarted(GameStarted gameStarted)
        {
            gameModel.GameStart();
            gameModel.teamLeader[(Team)gameStarted.TeamIndex] = (gameStarted.LeaderId, gameStarted.LeaderName);
        }

        private void HandlePlayerInfo(PlayerInfo playerInfo)
        {
            switch (playerInfo.Action)
            {
                case PacketAction.Add or PacketAction.Update: gameModel.UpdatePlayerInfo(playerInfo); break;
                case PacketAction.Remove: gameModel.RemovePlayerInfo(playerInfo.PlayerId); break;
            }
        }

        private void HandlePlayerStatus(PlayerStatus playerStatus)
        {
            switch (playerStatus.Action)
            {
                case PacketAction.Add or PacketAction.Update:
                    gameModel.UpdatePlayerInfo(playerStatus);

                    if((Team) playerStatus.Index == playerModel.Team || playerModel.AbilityActive)
                    {
                        playerStatus.Action = PacketAction.Add;
                        gameModel.UpdateMapPoints(playerStatus);
                    }

                    break;

                case PacketAction.Remove:
                    gameModel.RemovePlayerInfo(playerStatus.PlayerId);
                    gameModel.RemoveMapPoint(playerStatus.Name);
                    break;
            }
        }

        private void HandleBaseLocation(BaseLocation baseLocation)
        {
            gameModel.AddBaseLocation((Team)baseLocation.TeamIndex, new GPS(baseLocation.Longitude, baseLocation.Latitude));
        }

        private void HandleReadyPlayerCount(ReadyPlayerCount readyPlayerCount)
        {
            gameModel.readyPlayerCount = readyPlayerCount.ReadyPlayers;
        }

        private void HandleMapPoint(MapPoint mapPoint)
        {
            switch(mapPoint.Action)
            {
                case PacketAction.Add : gameModel.UpdateMapPoints(mapPoint); break;
                case PacketAction.Remove: gameModel.RemoveMapPoint(mapPoint.Name); break;
            }
        }

        // ---------- Server Handlers End ----------



        // ---------- ESP32 Handlers ----------

        // ---------- ESP32 HandlersEnd ----------

    }
}