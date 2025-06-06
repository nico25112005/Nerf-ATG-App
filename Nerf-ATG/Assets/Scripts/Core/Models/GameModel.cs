

using System;
using System.Collections.Generic;

public class GameModel : IGameModel
{
    public event EventHandler<List<PlayerInfo>> onPlayersChanged;
    public event EventHandler<EventArgs> onGameStart;

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
}
