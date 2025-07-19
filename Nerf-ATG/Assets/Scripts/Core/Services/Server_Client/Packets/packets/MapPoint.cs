using Game.Enums;
using System;
using System.Text;
using Zenject.SpaceFighter;

public class MapPoint : Packet<PacketType>, IMapPoint
{
    public string Name { get; private set; }
    public byte Index { get; private set; }
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }

    public MapPoint(byte[] bytes) : base(bytes) { }

    public MapPoint(string DisplayName, MapPointType team, double longitude, double latitude, PacketAction action)
        : base(PacketType.MapPoint, action)
    {
        this.Name = DisplayName;
        this.Index = (byte)team;
        this.Longitude = longitude;
        this.Latitude = latitude;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        Name = Encoding.UTF8.GetString(bytes, offset, 12).TrimEnd('\0');
        Index = bytes[offset + 12];
        Longitude = BitConverter.ToDouble(bytes, offset + 13);
        Latitude = BitConverter.ToDouble(bytes, offset + 21);
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {

        Array.Copy(Encoding.UTF8.GetBytes(Name.PadRight(12, '\0')), 0, bytes, offset, 12);
        bytes[offset + 12] = Index;
        Array.Copy(BitConverter.GetBytes(Longitude), 0, bytes, offset + 13, 8);
        Array.Copy(BitConverter.GetBytes(Latitude), 0, bytes, offset + 21, 8);

    }

    public override string ToString()
    {
        return $"MapPoint{{ Name='{Name}', Index={Index}, longitude={Longitude}, latitude={Latitude} }}";
    }
}
