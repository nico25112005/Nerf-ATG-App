using Game.Enums;
using System;
using System.Text;

public class GameInfo : Packet<ServerPacketType>
{
    public byte gameType { get; private set; }
    public string gameId { get; private set; }
    public string gameName { get; private set; }
    public byte playerCount { get; private set; }
    public byte maxPlayer { get; private set; }

    public GameInfo(byte[] bytes) : base(bytes, ServerPacketType.GameInfo) { }

    public GameInfo(GameType gameType, string gameId, string gameName, byte playerCount, byte maxPlayer)
        : base(ServerPacketType.GameInfo)
    {
        this.gameType = (byte)gameType;
        this.gameId = gameId;
        this.gameName = gameName;
        this.playerCount = playerCount;
        this.maxPlayer = maxPlayer;
    }

    public override void FromBytes(byte[] bytes)
    {
        gameType = bytes[4];
        gameId = Encoding.UTF8.GetString(bytes, 5, 8);
        gameName = Encoding.UTF8.GetString(bytes, 13, 12).TrimEnd('\0');
        playerCount = bytes[25];
        maxPlayer = bytes[26];
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();
        bytes[4] = gameType;
        Array.Copy(Encoding.UTF8.GetBytes(gameId), 0, bytes, 5, 8);
        Array.Copy(Encoding.UTF8.GetBytes(gameName.PadRight(12, '\0')), 0, bytes, 13, 12);
        bytes[25] = playerCount;
        bytes[26] = maxPlayer;
    }

    public override string ToString()
    {
        return $"GameInfo{{gameType={gameType}, gameId='{gameId}', gameName='{gameName}', playerCount={playerCount}, maxPlayer={maxPlayer}}}";
    }
}
