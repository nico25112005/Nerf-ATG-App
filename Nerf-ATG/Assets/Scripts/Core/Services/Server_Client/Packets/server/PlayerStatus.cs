using System;
using System.Text;

public class PlayerStatus : Packet<ServerPacketType>
{
    private string playerId;
    private string playerName;
    private int teamIndex;
    private double longitude;
    private double latitude;
    private int health;

    public PlayerStatus(byte[] bytes) : base(bytes, ServerPacketType.PlayerStatus) { }

    public PlayerStatus(string playerId, string playerName, int teamIndex, double longitude, double latitude, int health)
        : base(ServerPacketType.PlayerStatus)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.teamIndex = teamIndex;
        this.longitude = longitude;
        this.latitude = latitude;
        this.health = health;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');
        playerName = Encoding.UTF8.GetString(bytes, 12, 16).TrimEnd('\0');
        teamIndex = BitConverter.ToInt32(bytes, 28);
        longitude = BitConverter.ToDouble(bytes, 32);
        latitude = BitConverter.ToDouble(bytes, 40);
        health = BitConverter.ToInt32(bytes, 48);
    }

    public override void ToBytes(byte[] bytes)
    {
        byte[] idBytes = Encoding.UTF8.GetBytes(playerId.PadRight(12, '\0'));
        Array.Copy(idBytes, 0, bytes, 0, 12);

        byte[] nameBytes = Encoding.UTF8.GetBytes(playerName.PadRight(16, '\0'));
        Array.Copy(nameBytes, 0, bytes, 12, 16);

        Array.Copy(BitConverter.GetBytes(teamIndex), 0, bytes, 28, 4);
        Array.Copy(BitConverter.GetBytes(longitude), 0, bytes, 32, 8);
        Array.Copy(BitConverter.GetBytes(latitude), 0, bytes, 40, 8);
        Array.Copy(BitConverter.GetBytes(health), 0, bytes, 48, 4);
    }

    public override string ToString()
    {
        return $"PlayerStatus{{playerId='{playerId}', playerName='{playerName}', teamIndex={teamIndex}, longitude={longitude}, latitude={latitude}, health={health}}}";
    }
}
