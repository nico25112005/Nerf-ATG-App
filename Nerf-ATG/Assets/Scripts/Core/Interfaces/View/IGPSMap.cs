using Game.Enums;
using System.Numerics;

public interface IGPSMap
{
    void UpdateTile(sbyte x, sbyte y, byte[] TileData);
    void UpdateMapLocation(Vector2 MapOffset);
    void PlaceMarker(MarkerType type, PlayerStatus status, Vector2 markerOffset);
    void RemoveMarker(PlayerStatus status);
}