public interface IMapPoint
{
    string Name { get; }
    byte Index { get; }
    double Longitude { get; }
    double Latitude { get; }
    PacketAction Action { get; set; }

}