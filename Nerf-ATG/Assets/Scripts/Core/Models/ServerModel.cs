using System;
using System.Collections.Generic;
using UnityEngine;
public class ServerModel : IServerModel
{
    private static ServerModel _instance;
    private static readonly object _lock = new object();

    private ServerModel() { }

    public static ServerModel Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new ServerModel();
                }
                return _instance;
            }
        }
    }


    public event EventHandler<List<GameInfo>> onActiveGamesChanged;

    List<GameInfo> _activeGames = new();
    public IEnumerable<GameInfo> activeGames => _activeGames;

    public void addActiveGame(GameInfo gameInfo)
    {
        _activeGames.Add(gameInfo);
        onActiveGamesChanged?.Invoke(this, _activeGames);
    }

    public void removeActiveGame(GameInfo gameInfo)
    {
        _activeGames.Remove(gameInfo);
        onActiveGamesChanged?.Invoke(this, _activeGames);
    }
}
