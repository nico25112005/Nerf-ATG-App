using System;
using System.Text;
using Game.Enums;


public class CreateGame : Packet<ClientPacketType>
{
    private string playerId;
    private GameType gameType;
    private string gameName;

    public CreateGame(byte[] bytes) : base(bytes, ClientPacketType.CreateGame) { }

    public CreateGame(string playerId, GameType gameType, string gameName) : base(ClientPacketType.CreateGame)
    {
        this.playerId = playerId;
        this.gameType = gameType;
        this.gameName = gameName;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');
        gameType = (GameType)TCPClient.ConvertToInt(bytes, 12, BitConverter.ToInt32);
        gameName = Encoding.UTF8.GetString(bytes, 14, 5).TrimEnd('\0');
    }

    public override void ToBytes(byte[] bytes)
    {
        Array.Copy(Encoding.UTF8.GetBytes(playerId.PadRight(12, '\0')), 0, bytes, 0, 12);
        Array.Copy(Encoding.UTF8.GetBytes(gameType.ToString().PadRight(16, '\0')), 0, bytes, 12, 16);
        Array.Copy(Encoding.UTF8.GetBytes(gameName.PadRight(5, '\0')), 0, bytes, 28, 5);
    }

    public string GetPlayerId()
    {
        return playerId;
    }

    public GameType GetGameType()
    {
        return gameType;
    }

    public string GetGameName()
    {
        return gameName;
    }

    public override string ToString()
    {
        return $"CreateGame{{playerId='{playerId}', gameType={gameType}, gameName='{gameName}'}}";
    }
}


