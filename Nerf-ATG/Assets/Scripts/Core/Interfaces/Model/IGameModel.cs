using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGameModel
{
    event EventHandler<List<PlayerInfo>> onPlayersChanged;
    event EventHandler<EventArgs> onGameStart;
    event EventHandler<PlayerStatus> onPlayerStatusChanged;
    event EventHandler<PlayerStatus> onPlayerStatusRemoved;

    IEnumerable<PlayerInfo> players { get; }
    void AddPlayer(PlayerInfo playerInfo);

    void RemovePlayer(PlayerInfo playerInfo);

    GameInfo gameInfo { get; set; }

    void GameStart();

    IReadOnlyDictionary<string, PlayerStatus> playerStatus { get; }

    void UpdatePlayerStatus(PlayerStatus playerStatus);
    void RemovePlayerStatus(string playerID);

}

