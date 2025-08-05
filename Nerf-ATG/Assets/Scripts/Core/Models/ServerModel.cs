using System;
using System.Collections.Generic;
using UnityEngine;
public class ServerModel : IServerModel
{
    public event EventHandler<GameInfo> onActiveGamesChanged;

    Dictionary<string, GameInfo> _activeGames = new();
    public IReadOnlyDictionary<string, GameInfo> ActiveGames => _activeGames;

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
}
