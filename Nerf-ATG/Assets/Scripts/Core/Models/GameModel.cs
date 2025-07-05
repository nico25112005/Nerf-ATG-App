

using System;
using System.Collections.Generic;

public class GameModel : IGameModel
{
    public event EventHandler<List<PlayerInfo>> onPlayersChanged;
    public event EventHandler<EventArgs> onGameStart;
    public event EventHandler<PlayerStatus> onPlayerStatusChanged;
    public event EventHandler<PlayerStatus> onPlayerStatusRemoved;

    public List<PlayerInfo> _players = new();

    public IEnumerable<PlayerInfo> players => _players;


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


    public void AddPlayer(PlayerInfo playerInfo)
    {
        _players.Add(playerInfo);
        onPlayersChanged?.Invoke(this, _players);
    }

    public void RemovePlayer(PlayerInfo playerInfo)
    {
        _players.Remove(playerInfo);
        onPlayersChanged?.Invoke(this, _players);
    }

    public void GameStart()
    {
        onGameStart?.Invoke(this, EventArgs.Empty);
    }


    private Dictionary<string, PlayerStatus> _playerStatus = new();
    public IReadOnlyDictionary<string, PlayerStatus> playerStatus => _playerStatus;
    public void UpdatePlayerStatus(PlayerStatus status)
    {
        _playerStatus[status.playerId] = status;
        onPlayerStatusChanged?.Invoke(this, status);
    }

    public void RemovePlayerStatus(string id)
    {
        onPlayerStatusRemoved?.Invoke(this, _playerStatus[id]);
        _playerStatus.Remove(id);
    }
}
