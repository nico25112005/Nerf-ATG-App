using Game.Enums;
using System;
using System.Text;


public class JoinGame : Packet<PacketType>
{
    public string PlayerId {get; set;}
    public string GameName {get; set;}

    public JoinGame(byte[] bytes) : base(bytes) { }

    public JoinGame(string playerId, string gameName, PacketAction action) : base(PacketType.JoinGame, action)
    {
        this.PlayerId = playerId;
        this.GameName = gameName;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
        GameName = Encoding.UTF8.GetString(bytes, offset + 8, 12).TrimEnd('\0');
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {

        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
        Array.Copy(Encoding.UTF8.GetBytes(GameName.PadRight(12, '\0')), 0, bytes, offset + 8, 12);
    }

    public override string ToString() => $"JoinGame{{ playerId='{PlayerId}', gameName='{GameName}' }}";
}

