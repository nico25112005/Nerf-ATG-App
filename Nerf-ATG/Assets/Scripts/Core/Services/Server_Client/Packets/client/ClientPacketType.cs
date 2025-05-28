
public enum ClientPacketType
{
    AppStarted,
    /*
    UUID playerID
     */
    CreateGame,
    /*
    UUID playerID
    GameType gameType (int)
    String gameName size 16
    */
    RefreshGames,
    /*
    UUID playerID
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
    PlayerReady,
    /*
    UUID playerID
    int health
    WeaponType type (int)
    float damping 0 / 50 / 100 %
    */
    SetBaseLocation,
    /*
    UUID playerID
    double longitude
    double latitude
    */
    ActiveAbility,
    /*
    UUID playerID
    */
    PlayerDeath,
    /*
    UUID playerID
     */
    PlayerRespawned
    /*
    UUID playerID
     */
}
