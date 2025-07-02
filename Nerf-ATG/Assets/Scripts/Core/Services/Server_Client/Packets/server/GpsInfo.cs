using Game.Enums;
using System;
using System.Text;

public class GpsInfo : Packet<ServerPacketType>
{
    public string displayName { get; private set; }
    public byte markerType { get; private set; }
    public double longitude { get; private set; }
    public double latitude { get; private set; }


    public GpsInfo(byte[] bytes) : base(bytes, ServerPacketType.GpsInfo) { }

    public GpsInfo(string displayName, MarkerType markerType, double longitude, double latitude)
        : base(ServerPacketType.GpsInfo)
    {
        this.displayName = displayName;
        this.markerType = (byte)markerType;
        this.longitude = longitude;
        this.latitude = latitude;
    }

    public override void FromBytes(byte[] bytes)
    {
        displayName = Encoding.UTF8.GetString(bytes, 4, 8).TrimEnd('\0');
        markerType = bytes[12];
        longitude = BitConverter.ToDouble(bytes, 13);
        latitude = BitConverter.ToDouble(bytes, 21);
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();
        Array.Copy(Encoding.UTF8.GetBytes(displayName.PadRight(8, '\0')), 0, bytes, 4, 8);
        bytes[12] = markerType;
        Array.Copy(BitConverter.GetBytes(longitude), 0, bytes, 13, 8);
        Array.Copy(BitConverter.GetBytes(latitude), 0, bytes, 21, 8);
    }

    public override string ToString()
    {
        return $"GpsInfo{{displayName='{displayName}', markerType={markerType}, longitude={longitude}, latitude={latitude}}}";
    }
}
