using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IServerModel
{
    event EventHandler<GameInfo> onActiveGamesChanged;
    IReadOnlyDictionary<string, GameInfo> ActiveGames { get; }

    void AddOrUpdateActiveGame(GameInfo gameInfo);
    void RemoveActiveGame(GameInfo gameInfo);

}

