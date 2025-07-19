
public enum PacketType
{
    Unknown,

    CreateGame,
    /*
    UUID playerID
    GameType gameType (int)
    String gameName size 16
    */
    JoinGame,
    /*
    UUID playerID
    String name size 16
    String gameID size 5
    */
    SwitchTeam,
    /*
    UUID playerID
    */
    // SwitchCaptain,
    /*
    UUID playerID
    UUID captainID
    */
    StartGame,
    /*
     * UUID playerID
    */
    PlayerReady,
    /*
    UUID playerID
    int health
    WeaponType type (int)
    float damping 0 / 50 / 100 %
    */
    ActiveAbility,
    /*
    UUID playerID
    */
    MapPoint,
    /*
    byte markerIndex
    double longitude
    double latitude
    */
    GameInfo,
    /*
    GameType type (int)
    String id size(5)
    String name size(16)
    int playerCount
    int maxPlayer
     */
    PlayerInfo,
    /*
    UUID targetID
    String playerName
    int teamIndex
     */
    PlayerStatus,
    /*
    UUID targetID
    String name size 16
    int teamIndex
    double longitude
    double latitude
    int health
     */
    GameStarted,
    /*
    UUID leaderID
    string leaderName
    byte playerCount
    */
    ReadyPlayerCount,
    /*
    byte readyplayerCount
    */
    BaseLocation,
    /*
    byte teamIndex 
    double longitude
    double latitude
    */
}
