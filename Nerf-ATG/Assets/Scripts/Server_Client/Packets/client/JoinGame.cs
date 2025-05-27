using System;
using System.Text;


public class JoinGame : Packet<ClientPacketType>
{
    private string playerId;
    private string playerName;
    private string gameName;

    public JoinGame(byte[] bytes) : base(bytes, ClientPacketType.JoinGame) { }

    public JoinGame(string playerId, string playerName, string gameName) : base(ClientPacketType.JoinGame)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.gameName = gameName;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');
        playerName = Encoding.UTF8.GetString(bytes, 12, 16).TrimEnd('\0');
        gameName = Encoding.UTF8.GetString(bytes, 28, 5).TrimEnd('\0');
    }

    public override void ToBytes(byte[] bytes)
    {
        Array.Copy(Encoding.UTF8.GetBytes(playerId.PadRight(12, '\0')), 0, bytes, 0, 12);
        Array.Copy(Encoding.UTF8.GetBytes(playerName.PadRight(16, '\0')), 0, bytes, 12, 16);
        Array.Copy(Encoding.UTF8.GetBytes(gameName.PadRight(5, '\0')), 0, bytes, 28, 5);
    }

    public string GetPlayerId() => playerId;
    public string GetPlayerName() => playerName;
    public string GetGameName() => gameName;

    public override string ToString() => $"JoinGame{{ playerId='{playerId}', playerName='{playerName}', gameName='{gameName}' }}";
}

