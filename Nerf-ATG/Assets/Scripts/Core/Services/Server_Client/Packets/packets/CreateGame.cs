using System;
using System.Text;
using Game.Enums;

public class CreateGame : Packet<PacketType>
{
    public string PlayerId { get; private set; }
    public byte GameType { get; private set; }
    public string GameName { get; private set; }
    public byte MaxPlayer { get; private set; }

    public CreateGame(byte[] bytes) : base(bytes)
    {
    }

    public CreateGame(string playerId, GameType gameType, string gameName, byte maxPlayer, PacketAction action) : base(PacketType.CreateGame, action)
    {
        this.PlayerId = playerId;
        this.GameType = (byte)gameType;
        this.GameName = gameName;
        this.MaxPlayer = maxPlayer;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
        GameType = bytes[offset + 8];
        GameName = Encoding.UTF8.GetString(bytes, offset + 9, 12).TrimEnd('\0');
        MaxPlayer = bytes[offset + 21];
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
        bytes[offset + 8] = GameType;
        Array.Copy(Encoding.UTF8.GetBytes(GameName.PadRight(12, '\0')), 0, bytes, offset + 9, 12);
        bytes[offset + 21] = MaxPlayer;
    }

    public override string ToString()
    {
        return $"CreateGame{{ playerId='{PlayerId}', gameType='{GameType}', gameName='{GameName}', maxPlayer='{MaxPlayer}' }}";
    }
}