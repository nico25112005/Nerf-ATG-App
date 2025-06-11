using System.Numerics;
public interface IGpsTileService
{
    byte[] GetTile(GPS gps, sbyte x, sbyte y);
    Vector2 GpsToTilePosition(GPS playerLocation, GPS targetLocation);
    Vector2 GpsToTileCoordinates(GPS gps);

    void SetMapSize(Vector2 mapSize);

}