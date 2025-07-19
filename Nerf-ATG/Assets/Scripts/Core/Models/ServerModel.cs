using System;
using System.Collections.Generic;
using UnityEngine;
public class ServerModel : IServerModel
{
    public event EventHandler<List<GameInfo>> onActiveGamesChanged;

    List<GameInfo> _activeGames = new();
    public IEnumerable<GameInfo> ActiveGames => _activeGames;

    public void UpdateActiveGame(GameInfo gameInfo)
    {
        _activeGames[_activeGames.FindIndex(x => x.GameId == gameInfo.GameId)] = gameInfo;
        onActiveGamesChanged?.Invoke(this, _activeGames);
    }

    public void RemoveActiveGame(GameInfo gameInfo)
    {
        _activeGames.Remove(gameInfo);
        onActiveGamesChanged?.Invoke(this, _activeGames);
    }

    public void AddActiveGame(GameInfo gameInfo)
    {
        _activeGames.Add(gameInfo);
        onActiveGamesChanged?.Invoke(this, _activeGames);
    }
}
