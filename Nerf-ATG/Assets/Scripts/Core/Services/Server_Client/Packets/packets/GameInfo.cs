using Game.Enums;
using System;
using System.Text;

public class GameInfo : Packet<PacketType>
{
    public byte GameType { get; private set; }
    public string GameId { get; private set; }
    public string GameName { get; private set; }
    public byte PlayerCount { get; private set; }
    public byte MaxPlayer { get; private set; }

    public GameInfo(byte[] bytes) : base(bytes) { }

    public GameInfo(GameType gameType, string gameId, string gameName, byte playerCount, byte maxPlayer, PacketAction action) : base(PacketType.GameInfo, action)
    {
        this.GameType = (byte)gameType;
        this.GameId = gameId;
        this.GameName = gameName;
        this.PlayerCount = playerCount;
        this.MaxPlayer = maxPlayer;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        GameType = bytes[offset];
        GameId = Encoding.UTF8.GetString(bytes, offset + 1, 8);
        GameName = Encoding.UTF8.GetString(bytes, offset + 9, 12).TrimEnd('\0');
        PlayerCount = bytes[offset + 21];
        MaxPlayer = bytes[offset + 22];
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        bytes[offset] = GameType;
        Array.Copy(Encoding.UTF8.GetBytes(GameId), 0, bytes, offset + 1, 8);
        Array.Copy(Encoding.UTF8.GetBytes(GameName.PadRight(12, '\0')), 0, bytes, offset + 9, 12);
        bytes[offset + 21] = PlayerCount;
        bytes[offset + 22] = MaxPlayer;
    }

    public override string ToString()
    {
        return $"GameInfo{{gameType={GameType}, gameId='{GameId}', gameName='{GameName}', playerCount={PlayerCount}, maxPlayer={MaxPlayer}}}";
    }
}
