using System;
using System.Text;

public class PlayerStatus : Packet<ServerPacketType>
{
    public string Id { get; set; }
    public string name { get; set; }
    public int teamIndex { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
    public int health { get; set; }

    public PlayerStatus(byte[] bytes) : base(bytes, ServerPacketType.PlayerStatus) { }

    public PlayerStatus(string Id, string Name, int teamIndex, double longitude, double latitude, int health)
        : base(ServerPacketType.PlayerStatus)
    {
        this.Id = Id;
        this.name = Name;
        this.teamIndex = teamIndex;
        this.longitude = longitude;
        this.latitude = latitude;
        this.health = health;
    }

    public override void FromBytes(byte[] bytes)
    {
        Id = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');
        name = Encoding.UTF8.GetString(bytes, 12, 16).TrimEnd('\0');
        teamIndex = BitConverter.ToInt32(bytes, 28);
        longitude = BitConverter.ToDouble(bytes, 32);
        latitude = BitConverter.ToDouble(bytes, 40);
        health = BitConverter.ToInt32(bytes, 48);
    }

    public override void ToBytes(byte[] bytes)
    {
        byte[] idBytes = Encoding.UTF8.GetBytes(Id.PadRight(12, '\0'));
        Array.Copy(idBytes, 0, bytes, 0, 12);

        byte[] nameBytes = Encoding.UTF8.GetBytes(name.PadRight(16, '\0'));
        Array.Copy(nameBytes, 0, bytes, 12, 16);

        Array.Copy(BitConverter.GetBytes(teamIndex), 0, bytes, 28, 4);
        Array.Copy(BitConverter.GetBytes(longitude), 0, bytes, 32, 8);
        Array.Copy(BitConverter.GetBytes(latitude), 0, bytes, 40, 8);
        Array.Copy(BitConverter.GetBytes(health), 0, bytes, 48, 4);
    }

    public override string ToString()
    {
        return $"PlayerStatus{{playerId='{Id}', playerName='{name}', teamIndex={teamIndex}, longitude={longitude}, latitude={latitude}, health={health}}}";
    }
}
