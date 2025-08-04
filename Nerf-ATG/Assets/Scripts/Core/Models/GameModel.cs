using Game.Enums;
using System;
using System.Collections.Generic;

public class GameModel : IGameModel
{
    public event EventHandler<List<PlayerInfo>> onPlayersChanged;

    public event EventHandler<EventArgs> onGameStart;

    public event EventHandler<IPlayerInfo> onPlayerInfoChanged;

    public event EventHandler<byte> onReadyPlayerCountChanged;

    public event EventHandler<EventArgs> onNewBaseLocation;

    public event EventHandler<IMapPoint> onMapPointChanged;

    private byte _readPlayerCount;

    public byte readyPlayerCount
    {
        get => _readPlayerCount;
        set
        {
            _readPlayerCount = value;
            onReadyPlayerCountChanged?.Invoke(this, value);
        }
    }

    public void GameStart()
    {
        onGameStart?.Invoke(this, EventArgs.Empty);
    }

    private GameInfo _gameInfo;

    public GameInfo gameInfo
    {
        get => _gameInfo;
        set
        {
            if (_gameInfo == null)
                _gameInfo = value;
            else
                throw new Exception("GameInfo already set!");
        }
    }

    //Base & Teamleader
    public Dictionary<Team, (string, string)> teamLeader { get; set; } = new();

    private Dictionary<Team, GPS> _baseLocation = new();
    public IReadOnlyDictionary<Team, GPS> baseLocation => _baseLocation;

    public void AddBaseLocation(Team team, GPS gps)
    {
        _baseLocation.TryAdd(team, gps);
        onNewBaseLocation?.Invoke(this, EventArgs.Empty);
    }

    //PlayerInfo
    private Dictionary<string, IPlayerInfo> _playerInfo = new();

    public IReadOnlyDictionary<string, IPlayerInfo> playerInfo => _playerInfo;

    public void UpdatePlayerInfo(IPlayerInfo status)
    {
        _playerInfo[status.PlayerId] = status;
        onPlayerInfoChanged?.Invoke(this, status);
    }

    public void RemovePlayerInfo(string id)
    {
        if (_playerInfo.ContainsKey(id))
        {
            _playerInfo[id].Action = PacketAction.Remove;
            onPlayerInfoChanged?.Invoke(this, _playerInfo[id]);
            _playerInfo.Remove(id);
        }
    }

    //MapPoint

    private Dictionary<string, IMapPoint> _mapPoints = new();
    public IReadOnlyDictionary<string, IMapPoint> mapPoints => _mapPoints;

    public void UpdateMapPoints(IMapPoint mapPoint)
    {
        _mapPoints[mapPoint.Name] = mapPoint;
        onMapPointChanged?.Invoke(this, mapPoint);
    }

    public void RemoveMapPoint(string name)
    {
        if (_mapPoints.ContainsKey(name))
        {
            _mapPoints[name].Action = PacketAction.Remove;
            onMapPointChanged?.Invoke(this, _mapPoints[name]);
            _mapPoints.Remove(name);
        }
    }
}