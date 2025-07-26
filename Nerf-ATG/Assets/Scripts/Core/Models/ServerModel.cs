using System;
using System.Collections.Generic;
using UnityEngine;
public class ServerModel : IServerModel
{
    public event EventHandler<GameInfo> onActiveGamesChanged;

    List<GameInfo> _activeGames = new();
    public IEnumerable<GameInfo> ActiveGames => _activeGames;

    public void UpdateActiveGame(GameInfo gameInfo)
    {
        _activeGames[_activeGames.FindIndex(x => x.GameId == gameInfo.GameId)] = gameInfo;
        onActiveGamesChanged?.Invoke(this, gameInfo);
    }

    public void RemoveActiveGame(GameInfo gameInfo)
    {
        _activeGames.Remove(gameInfo);
        onActiveGamesChanged?.Invoke(this, gameInfo);
    }

    public void AddActiveGame(GameInfo gameInfo)
    {
        _activeGames.Add(gameInfo);
        Debug.Log("before");
        onActiveGamesChanged?.Invoke(this, gameInfo);
        Debug.Log("after");
    }
}
