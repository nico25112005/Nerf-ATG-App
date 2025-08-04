using Game.Enums;
using UnityEngine;
using System.Threading;

namespace Assets.Scripts.Presenter
{
    public class TcpDataPresenter
    {
        private readonly IPlayerModel playerModel;
        private readonly IGameModel gameModel;
        private readonly IServerModel serverModel;
        private readonly ITcpClientService tcpClientService;
        private readonly IMainThreadExecutor mainThreadExecutor;

        public TcpDataPresenter(IPlayerModel playerModel, IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService, IMainThreadExecutor mainThreadExecutor)
        {
            this.playerModel = playerModel;
            this.serverModel = serverModel;
            this.gameModel = gameModel;
            this.tcpClientService = tcpClientService;
            this.mainThreadExecutor = mainThreadExecutor;

            this.tcpClientService.dataReceived += RecivedData;
        }

        private void RecivedData(object sender, byte[] bytes)
        {
            if ((ITcpClientService.Connections)sender == ITcpClientService.Connections.Server)
            {
                Debug.Log("Recived new Packet: " + (PacketType)bytes[0]);
                Debug.Log(string.Join("", bytes));
                switch ((PacketType)bytes[0])
                {
                    case PacketType.GameInfo: mainThreadExecutor.Execute(() => HandleGameInfo(new GameInfo(bytes))); break;
                    case PacketType.GameStarted: mainThreadExecutor.Execute(() => HandleGameStarted(new GameStarted(bytes))); break;
                    case PacketType.PlayerInfo: mainThreadExecutor.Execute(() => HandlePlayerInfo(new PlayerInfo(bytes))); break;
                    case PacketType.PlayerStatus: mainThreadExecutor.Execute(() => HandlePlayerStatus(new PlayerStatus(bytes))); break;
                    case PacketType.BaseLocation: mainThreadExecutor.Execute(() => HandleBaseLocation(new BaseLocation(bytes))); break;
                    case PacketType.ReadyPlayerCount: mainThreadExecutor.Execute(() => HandleReadyPlayerCount(new ReadyPlayerCount(bytes))); break;
                    case PacketType.MapPoint: mainThreadExecutor.Execute(() => HandleMapPoint(new MapPoint(bytes))); break;
                    case PacketType.SwitchTeam: mainThreadExecutor.Execute(() => HandleSwitchTeam(new SwitchTeam(bytes))); break;
                }
            }
        }

        // ---------- Server Handlers ----------

        private void HandleGameInfo(GameInfo gameInfo)
        {
            Debug.Log(gameInfo);
            switch (gameInfo.Action)
            {
                case PacketAction.Add: serverModel.AddActiveGame(gameInfo); break;
                case PacketAction.Update: serverModel.UpdateActiveGame(gameInfo); break;
                case PacketAction.Remove: serverModel.RemoveActiveGame(gameInfo); break;
            }
        }

        private void HandleGameStarted(GameStarted gameStarted)
        {
            Debug.Log(gameStarted);
            gameModel.teamLeader.TryAdd((Team)gameStarted.TeamIndex, (gameStarted.LeaderId, gameStarted.LeaderName));
            Debug.Log("Team is the same: " + (playerModel.Team == (Team)gameStarted.TeamIndex));
            Debug.Log("Team: " + playerModel.Team);
            Debug.Log("TeamIndex: " + (Team)gameStarted.TeamIndex);
            if (playerModel.Team == (Team)gameStarted.TeamIndex)
            {
                Debug.Log("Started");
                gameModel.GameStart();
            }
        }

        private void HandlePlayerInfo(PlayerInfo playerInfo)
        {
            Debug.Log(playerInfo);
            switch (playerInfo.Action)
            {
                case PacketAction.Add or PacketAction.Update:
                    gameModel.UpdatePlayerInfo(playerInfo);
                    if(playerInfo.PlayerId == playerModel.Id.ToString()[..8])
                    {
                        playerModel.Team = (Team)playerInfo.Index;
                    }
                    break;
                case PacketAction.Remove: gameModel.RemovePlayerInfo(playerInfo.PlayerId); break;
            }
        }

        private void HandlePlayerStatus(PlayerStatus playerStatus)
        {
            Debug.Log(playerStatus);
            switch (playerStatus.Action)
            {
                case PacketAction.Add or PacketAction.Update:
                    gameModel.UpdatePlayerInfo(playerStatus);

                    if ((Team)playerStatus.Index == playerModel.Team || playerModel.AbilityActive)
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
            Debug.Log(baseLocation);
            gameModel.AddBaseLocation((Team)baseLocation.TeamIndex, new GPS(baseLocation.Longitude, baseLocation.Latitude));

            if(playerModel.Team == (Team)baseLocation.TeamIndex)
            {
                gameModel.UpdateMapPoints(new MapPoint("Base", MapPointType.Base, new GPS(baseLocation.Longitude, baseLocation.Latitude), PacketAction.Add));
            }
        }
        
        private void HandleReadyPlayerCount(ReadyPlayerCount readyPlayerCount)
        {
            Debug.Log(readyPlayerCount);
            gameModel.readyPlayerCount = readyPlayerCount.ReadyPlayers;
        }

        private void HandleMapPoint(MapPoint mapPoint)
        {
            Debug.Log(mapPoint);
            switch (mapPoint.Action)
            {
                case PacketAction.Add: gameModel.UpdateMapPoints(mapPoint); break;
                case PacketAction.Remove: gameModel.RemoveMapPoint(mapPoint.Name); break;
            }
        }

        private void HandleSwitchTeam(SwitchTeam switchTeam)
        {
            playerModel.Team = (playerModel.Team == Team.Red) ? Team.Blue : Team.Red;
        }

        // ---------- Server Handlers End ----------



        // ---------- ESP32 Handlers ----------

        // ---------- ESP32 HandlersEnd ----------

    }
}