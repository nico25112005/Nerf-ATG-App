using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IServerModel
{
    event EventHandler<List<GameInfo>> onActiveGamesChanged;
    IEnumerable<GameInfo> ActiveGames { get; }

    void addActiveGame(GameInfo gameInfo);
    void removeActiveGame(GameInfo gameInfo);

}

