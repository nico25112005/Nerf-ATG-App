using Game.Enums;
using System;
using System.Text;


public class JoinGame : Packet<ClientPacketType>
{
    public string playerId {get; set;}
    public string gameName {get; set;}

    public JoinGame(byte[] bytes) : base(bytes, ClientPacketType.JoinGame) { }

    public JoinGame(string playerId, string gameName) : base(ClientPacketType.JoinGame)
    {
        this.playerId = playerId;
        this.gameName = gameName;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 4, 8);
        gameName = Encoding.UTF8.GetString(bytes, 12, 12).TrimEnd('\0');
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();

        Array.Copy(Encoding.UTF8.GetBytes(playerId), 0, bytes, 4, 8);
        Array.Copy(Encoding.UTF8.GetBytes(gameName.PadRight(12, '\0')), 0, bytes, 12, 12);
    }

    public override string ToString() => $"JoinGame{{ playerId='{playerId}', gameName='{gameName}' }}";
}

