using Game.Enums;
using System;
using System.Text;


public class BaseLocation : Packet<PacketType>
{
    public byte TeamIndex { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public BaseLocation(byte[] bytes) : base(bytes) { }

    public BaseLocation(Team team, GPS gps, PacketAction action) : base(PacketType.BaseLocation, action)
    {
        this.TeamIndex = (byte)team;
        this.Longitude = gps.Longitude;
        this.Latitude = gps.Latitude;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        TeamIndex = bytes[offset];
        Longitude = BitConverter.ToDouble(bytes, offset + 1);
        Latitude = BitConverter.ToDouble(bytes, offset + 9);
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        bytes[offset + 0] = TeamIndex;
        Array.Copy(BitConverter.GetBytes(Longitude), 0, bytes, offset + 1, 8);
        Array.Copy(BitConverter.GetBytes(Latitude), 0, bytes, offset + 9, 8);
    }

    public override string ToString()
    {
        return $"PlayerStatus{{teamIndex={TeamIndex}, longitude={Longitude}, latitude={Latitude}}}";
    }
}

