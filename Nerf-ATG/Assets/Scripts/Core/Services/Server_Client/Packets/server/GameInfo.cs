using Game.Enums;
using System;
using System.Text;

public class GameInfo : Packet<ServerPacketType>
{
    public GameType gameType { get; private set; }
    public string gameId { get; private set; }
    public string gameName { get; private set; }
    public int playerCount { get; private set; }
    public int maxPlayer { get; private set; }

    public GameInfo(byte[] bytes) : base(bytes, ServerPacketType.GameInfo) { }

    public GameInfo(GameType gameType, string gameId, string gameName, int playerCount, int maxPlayer)
        : base(ServerPacketType.GameInfo)
    {
        this.gameType = gameType;
        this.gameId = gameId;
        this.gameName = gameName;
        this.playerCount = playerCount;
        this.maxPlayer = maxPlayer;
    }

    public override void FromBytes(byte[] bytes)
    {
        gameType = (GameType)BitConverter.ToInt32(bytes, 0);
        gameId = Encoding.UTF8.GetString(bytes, 4, 5).TrimEnd('\0');
        gameName = Encoding.UTF8.GetString(bytes, 9, 16).TrimEnd('\0');
        playerCount = BitConverter.ToInt32(bytes, 25);
        maxPlayer = BitConverter.ToInt32(bytes, 29);
    }

    public override void ToBytes(byte[] bytes)
    {
        Array.Copy(BitConverter.GetBytes((int)gameType), 0, bytes, 0, 4);

        byte[] idBytes = Encoding.UTF8.GetBytes(gameId.PadRight(5, '\0'));
        Array.Copy(idBytes, 0, bytes, 4, 5);

        byte[] nameBytes = Encoding.UTF8.GetBytes(gameName.PadRight(16, '\0'));
        Array.Copy(nameBytes, 0, bytes, 9, 16);

        Array.Copy(BitConverter.GetBytes(playerCount), 0, bytes, 25, 4);
        Array.Copy(BitConverter.GetBytes(maxPlayer), 0, bytes, 29, 4);
    }

    public override string ToString()
    {
        return $"GameInfo{{gameType={gameType}, gameId='{gameId}', gameName='{gameName}', playerCount={playerCount}, maxPlayer={maxPlayer}}}";
    }
}
