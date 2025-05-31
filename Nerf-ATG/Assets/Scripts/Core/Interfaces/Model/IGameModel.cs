using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGameModel
{
    event EventHandler<List<PlayerInfo>> onPlayersChanged;

    IEnumerable<PlayerInfo> players { get; }
    void AddPlayer(PlayerInfo playerInfo);

    void RemovePlayer(PlayerInfo playerInfo);

    GameInfo gameInfo { get; set; }

    event EventHandler<EventArgs> onGameStart;
    void GameStart();

}

