using Game.Enums;
using System.Numerics;

public interface IGPSMap
{
    void UpdateTile(sbyte x, sbyte y, byte[] TileData);
    void UpdateMapLocation(Vector2 MapOffset);
    void UpdateMapPoints(MapPointType type, IMapPoint marker, Vector2 markerOffset);
}