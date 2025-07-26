using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IServerModel
{
    event EventHandler<GameInfo> onActiveGamesChanged;
    IEnumerable<GameInfo> ActiveGames { get; }

    void UpdateActiveGame(GameInfo gameInfo);
    void AddActiveGame(GameInfo gameInfo);
    void RemoveActiveGame(GameInfo gameInfo);

}

