using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServerPacketType
{
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
    UUID playerID
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
}
