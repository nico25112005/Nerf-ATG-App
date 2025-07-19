using Game.Enums;
using System;
using System.Text;
using Zenject.SpaceFighter;

public class PlayerStatus : Packet<PacketType>, IPlayerInfo, IMapPoint 
{
    public string PlayerId { get; private set; }
    public string Name { get; private set; }
    public byte Index { get; private set; }
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }
    public byte Health { get; private set; }

    public PlayerStatus(byte[] bytes) : base(bytes) { }

    public PlayerStatus(string playerId, string playerName, Team team, GPS gps, byte health, PacketAction action)
        : base(PacketType.PlayerStatus, action)
    {
        this.PlayerId = playerId;
        this.Name = playerName;
        this.Index = (byte)team;
        this.Longitude = gps.Longitude;
        this.Latitude = gps.Latitude;
        this.Health = health;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
        Name = Encoding.UTF8.GetString(bytes, offset + 8, 12).TrimEnd('\0');
        Index = bytes[offset + 20];
        Longitude = BitConverter.ToDouble(bytes, offset + 21);
        Latitude = BitConverter.ToDouble(bytes, offset + 29);
        Health = bytes[offset + 37];
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
        Array.Copy(Encoding.UTF8.GetBytes(Name.PadRight(12, '\0')), 0, bytes, offset + 8, 12);
        bytes[offset + 20] = Index;
        Array.Copy(BitConverter.GetBytes(Longitude), 0, bytes, offset + 21, 8);
        Array.Copy(BitConverter.GetBytes(Latitude), 0, bytes, offset + 29, 8);
        bytes[offset + 37] = Health;

    }

    public override string ToString()
    {
        return $"PlayerStatus{{playerId='{PlayerId}', playerName='{Name}', teamIndex={Index}, longitude={Longitude}, latitude={Latitude}, health={Health}}}";
    }
}
