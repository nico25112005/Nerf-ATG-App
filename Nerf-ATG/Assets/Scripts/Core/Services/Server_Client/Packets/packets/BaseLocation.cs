using Game.Enums;
using System;
using System.Text;
using Zenject.SpaceFighter;


public class BaseLocation : Packet<PacketType>
{
    public string PlayerId;
    public byte TeamIndex { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public BaseLocation(byte[] bytes) : base(bytes) { }

    public BaseLocation(string playerId, Team team, GPS gps, PacketAction action) : base(PacketType.BaseLocation, action)
    {
        this.PlayerId = playerId;
        this.TeamIndex = (byte)team;
        this.Longitude = gps.Longitude;
        this.Latitude = gps.Latitude;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
        TeamIndex = bytes[offset + 8];
        Longitude = BitConverter.ToDouble(bytes, offset + 9);
        Latitude = BitConverter.ToDouble(bytes, offset + 17);
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
        bytes[offset + 8] = TeamIndex;
        Array.Copy(BitConverter.GetBytes(Longitude), 0, bytes, offset + 9, 8);
        Array.Copy(BitConverter.GetBytes(Latitude), 0, bytes, offset + 17, 8);
    }

    public override string ToString()
    {
        return $"BaseLocation{{teamIndex={TeamIndex}, longitude={Longitude}, latitude={Latitude}}}";
    }
}

