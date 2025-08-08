using System;
using System.Collections.Generic;
using UnityEngine;
public class ServerModel : IServerModel, IResetable
{
    public event EventHandler<GameInfo> onActiveGamesChanged;
    public event EventHandler<long> onPingChanged;

    Dictionary<string, GameInfo> _activeGames = new();
    public IReadOnlyDictionary<string, GameInfo> ActiveGames => _activeGames;

    public void Ping(long ms)
    {
        onPingChanged?.Invoke(this, ms);
    }

    public void AddOrUpdateActiveGame(GameInfo gameInfo)
    {
        _activeGames[gameInfo.GameId] = gameInfo;
        onActiveGamesChanged?.Invoke(this, gameInfo);
    }

    public void RemoveActiveGame(GameInfo gameInfo)
    {
        _activeGames.Remove(gameInfo.GameId);
        onActiveGamesChanged?.Invoke(this, gameInfo);
    }

    public void Reset()
    {
        onActiveGamesChanged = null;
        onPingChanged = null;

        _activeGames = new();
    }
}
