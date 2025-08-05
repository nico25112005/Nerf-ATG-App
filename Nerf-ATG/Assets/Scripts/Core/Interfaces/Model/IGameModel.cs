using Game.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public interface IGameModel
{
    event EventHandler<EventArgs> onGameStart;
    event EventHandler<IPlayerInfo> onPlayerInfoChanged;
    event EventHandler<byte> onReadyPlayerCountChanged;
    event EventHandler<EventArgs> onNewBaseLocation;
    event EventHandler<IMapPoint> onMapPointChanged;
    byte readyPlayerCount { get; set; }

    GameInfo gameInfo { get; set; }

    void GameStart();

    IReadOnlyDictionary<string, IPlayerInfo> playerInfo { get; }

    void AddOrUpdatePlayerInfo(IPlayerInfo playerInfo);
    void RemovePlayerInfo(string playerID);

    Dictionary<Team, (string, string)> teamLeader { get; set; }
    IReadOnlyDictionary<Team, GPS> baseLocation { get; }

    void AddBaseLocation(Team team, GPS gps);

    IReadOnlyDictionary<string, IMapPoint> mapPoints { get; }

    void AddOrUpdateMapPoints(IMapPoint mapPoint);
    void RemoveMapPoint(string Name);

}

