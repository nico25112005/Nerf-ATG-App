using Game.Enums;
using UnityEngine;
using System.Timers;
using System.Linq;
using System.Diagnostics;
using System;

namespace Assets.Scripts.Presenter
{
    public class TcpDataPresenter
    {
        private readonly IPlayerModel playerModel;
        private readonly IGameModel gameModel;
        private readonly IServerModel serverModel;
        private readonly ITcpClientService tcpClientService;
        private readonly IMainThreadExecutor mainThreadExecutor;

        Timer timer;
        readonly Stopwatch stopwatch = new();

        public TcpDataPresenter(IPlayerModel playerModel, IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService, IMainThreadExecutor mainThreadExecutor)
        {
            this.playerModel = playerModel;
            this.serverModel = serverModel;
            this.gameModel = gameModel;
            this.tcpClientService = tcpClientService;
            this.mainThreadExecutor = mainThreadExecutor;

            this.tcpClientService.DataReceived += RecivedData;
            this.tcpClientService.ConnectionStatusChanged += ConnectionChanged;
        }

        private void RecivedData(object sender, byte[] bytes)
        {
            if ((ITcpClientService.Connections)sender == ITcpClientService.Connections.Server)
            {
                UnityEngine.Debug.Log("Recived new Packet: " + (PacketType)bytes[0]);
                //Debug.Log(string.Join("|", bytes.Select(b => b.ToString("D3"))));
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
                    case PacketType.ServerMessage: mainThreadExecutor.Execute(() => HandleServerMessage(new ServerMessage(bytes))); break;
                    case PacketType.QuitGame: mainThreadExecutor.Execute(() => GameManager.GetInstance().ResetGame()); break;
                    case PacketType.Ping: PingRecived(); break;
                }
            }
        }

        private void ConnectionChanged(object sender, bool connected)
        {
            if((ITcpClientService.Connections)sender == ITcpClientService.Connections.Server)
            {
                if (connected == true)
                {
                    timer = new Timer(1000);
                    timer.AutoReset = true;
                    timer.Elapsed += (s, e) =>
                    {
                        tcpClientService.Send(ITcpClientService.Connections.Server, new Ping(playerModel.Id.ToString(), PacketAction.Generic));
                        stopwatch.Restart();
                    };

                    timer.Start();   
                }
                else
                {
                    timer.Stop();
                    timer.Dispose();

                    stopwatch.Stop();
                }
            }
            
        }

        private void PingRecived()
        {
            stopwatch.Stop();
            long ping = stopwatch.ElapsedMilliseconds;

            mainThreadExecutor.Execute(() => serverModel.Ping(ping));

            stopwatch.Reset();
        }

        // ---------- Server Handlers ----------

        private void HandleGameInfo(GameInfo gameInfo)
        {
            switch (gameInfo.Action)
            {
                case PacketAction.Add or PacketAction.Update: serverModel.AddOrUpdateActiveGame(gameInfo); break;
                case PacketAction.Remove: serverModel.RemoveActiveGame(gameInfo); break;
            }
        }

        private void HandleGameStarted(GameStarted gameStarted)
        {
            gameModel.teamLeader.TryAdd((Team)gameStarted.TeamIndex, (gameStarted.LeaderId, gameStarted.LeaderName));
            if (playerModel.Team == (Team)gameStarted.TeamIndex)
            {
                gameModel.GameStart();
            }
        }

        private void HandlePlayerInfo(PlayerInfo playerInfo)
        {
            switch (playerInfo.Action)
            {
                case PacketAction.Add or PacketAction.Update:
                    gameModel.AddOrUpdatePlayerInfo(playerInfo);
                    if (playerInfo.PlayerId == playerModel.Id.ToString()[..8])
                    {
                        playerModel.Team = (Team)playerInfo.Index;
                    }
                    break;
                case PacketAction.Remove: gameModel.RemovePlayerInfo(playerInfo.PlayerId); break;
            }
        }

        private void HandlePlayerStatus(PlayerStatus playerStatus)
        {
            switch (playerStatus.Action)
            {
                case PacketAction.Add or PacketAction.Update:

                    gameModel.AddOrUpdatePlayerInfo(playerStatus);

                    if ((Team)playerStatus.Index == playerModel.Team || playerModel.AbilityActive)
                    {
                        if (playerStatus.PlayerId != playerModel.Id.ToString()[..8])
                        {
                            gameModel.AddOrUpdateMapPoints(playerStatus);
                        }
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

            if (playerModel.Team == (Team)baseLocation.TeamIndex)
            {
                gameModel.AddOrUpdateMapPoints(new MapPoint("Base", MapPointType.Base, new GPS(baseLocation.Longitude, baseLocation.Latitude), PacketAction.Add));
            }
        }

        private void HandleReadyPlayerCount(ReadyPlayerCount readyPlayerCount)
        {
            gameModel.readyPlayerCount = readyPlayerCount.ReadyPlayers;
        }

        private void HandleMapPoint(MapPoint mapPoint)
        {
            switch (mapPoint.Action)
            {
                case PacketAction.Add: gameModel.AddOrUpdateMapPoints(mapPoint); break;
                case PacketAction.Remove: gameModel.RemoveMapPoint(mapPoint.Name); break;
            }
        }

        private void HandleSwitchTeam(SwitchTeam switchTeam)
        {
            playerModel.Team = (playerModel.Team == Team.Red) ? Team.Blue : Team.Red;
        }

        private void HandleServerMessage(ServerMessage serverMessage)
        {
            ToastNotification.Show(serverMessage.Message, "alert");
        }



        // ---------- Server Handlers End ----------



        // ---------- ESP32 Handlers ----------

        // ---------- ESP32 HandlersEnd ----------

    }
}