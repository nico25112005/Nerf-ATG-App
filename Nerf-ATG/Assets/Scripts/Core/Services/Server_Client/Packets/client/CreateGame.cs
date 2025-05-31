using System;
using System.Text;
using Game.Enums;


public class CreateGame : Packet<ClientPacketType>
{
    public string playerId { get; set; }
    public GameType gameType { get; set; }
    public string gameName { get; set; }
    public int maxPlayer { get; set; }

    public CreateGame(byte[] bytes) : base(bytes, ClientPacketType.CreateGame) { }

    public CreateGame(string playerId, GameType gameType, string gameName, int maxPlayer) : base(ClientPacketType.CreateGame)
    {
        this.playerId = playerId;
        this.gameType = gameType;
        this.gameName = gameName;
        this.maxPlayer = maxPlayer;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');
        gameType = (GameType)Converter.ConvertToInt(bytes, 12, BitConverter.ToInt32);
        gameName = Encoding.UTF8.GetString(bytes, 14, 5).TrimEnd('\0');
        maxPlayer = Converter.ConvertToInt(bytes, 19, BitConverter.ToInt32);
    }

    public override void ToBytes(byte[] bytes)
    {
        Array.Copy(Encoding.UTF8.GetBytes(playerId.PadRight(12, '\0')), 0, bytes, 0, 12);
        Array.Copy(Encoding.UTF8.GetBytes(gameType.ToString().PadRight(16, '\0')), 0, bytes, 12, 16);
        Array.Copy(Encoding.UTF8.GetBytes(gameName.PadRight(5, '\0')), 0, bytes, 28, 5);
        Array.Copy(BitConverter.GetBytes(maxPlayer), 0, bytes, 33, 4);
    }

    public override string ToString()
    {
        return $"CreateGame{{playerId='{playerId}', gameType={gameType}, gameName='{gameName}'}}";
    }
}


