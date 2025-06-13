using System;
using System.Numerics;
public interface IGpsTileService
{
    event Action<byte[], sbyte, sbyte> OnTileDataReceived;

    void RequestTile(GPS gps, sbyte x, sbyte y);
    Vector2 GpsToTilePosition(Vector2 currentTile, GPS gps);
    Vector2 GpsToTileCoordinates(GPS gps);

    void SetMapSize(Vector2 mapSize);

}