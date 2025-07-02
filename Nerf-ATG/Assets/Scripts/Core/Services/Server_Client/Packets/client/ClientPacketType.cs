
public enum ClientPacketType
{
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
    PlayerDeath,
    /*
    UUID playerID
     */
}
