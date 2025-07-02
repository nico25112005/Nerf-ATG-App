using System;
using System.Text;
using Game.Enums;


public class CreateGame : Packet<ClientPacketType>
{
    public string playerId { get; private set; }
    public byte gameType { get; private set; }
    public string gameName { get; private set; }
    public byte maxPlayer { get; private set; }

    public CreateGame(byte[] bytes) : base(bytes, ClientPacketType.CreateGame) { }

    public CreateGame(string playerId, GameType gameType, string gameName, byte maxPlayer) : base(ClientPacketType.CreateGame)
    {
        this.playerId = playerId;
        this.gameType = (byte)gameType;
        this.gameName = gameName;
        this.maxPlayer = maxPlayer;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 4, 8);
        gameType = bytes[12];
        gameName = Encoding.UTF8.GetString(bytes, 13, 12).TrimEnd('\0');
        maxPlayer = bytes[21];
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();

        Array.Copy(Encoding.UTF8.GetBytes(playerId), 0, bytes, 4, 8);
        bytes[12] = gameType;
        Array.Copy(Encoding.UTF8.GetBytes(gameName.PadRight(12, '\0')), 0, bytes, 13, 12);
        bytes[21] = maxPlayer;
    }

    public override string ToString()
    {
        return $"CreateGame{{ playerId='{playerId}', gameType='{gameType}', gameName='{gameName}', maxPlayer='{maxPlayer}' }}";
    }
}


