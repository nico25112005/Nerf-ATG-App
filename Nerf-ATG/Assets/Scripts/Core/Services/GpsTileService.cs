using System;
using System.Numerics;
using System.Net;

public class GpsTileService : IGpsTileService
{
    private readonly int zoom = 18;
    private Vector2 mapSize = Vector2.Zero;

    public void SetMapSize(Vector2 newMapSize)
    {
        this.mapSize = newMapSize;
    }

    public byte[] GetTile(GPS gps, sbyte x, sbyte y)
    {
        var centerTile = GpsToTileCoordinates(gps);
        var targetTileX = (int)centerTile.X + x;
        var targetTileY = (int)centerTile.Y + y;

        string url = $"https://tile.openstreetmap.org/{zoom}/{targetTileX}/{targetTileY}.png";

        using (var webClient = new WebClient())
        {
            try
            {
                return webClient.DownloadData(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading tile: {ex.Message}");
                return null;
            }
        }
    }

    public Vector2 GpsToTileCoordinates(GPS gps)
    {
        double latRad = gps.Latitude * Math.PI / 180.0;
        int n = 1 << zoom;

        int x = (int)((gps.Longitude + 180.0) / 360.0 * n);
        int y = (int)((1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * n);

        return new Vector2(x, y);
    }

    public Vector2 GpsToTilePosition(GPS playerLocation, GPS targetLocation)
    {
        var centerCoords = GpsToTileCoordinates(playerLocation);
        int n = 1 << zoom;

        double latRad = targetLocation.Latitude * Math.PI / 180.0;

        // Berechnung der relativen Position in Kachelkoordinaten (mit Offset 0.5)
        double tileX = ((targetLocation.Longitude + 180.0) / 360.0 * n - (centerCoords.X + 0.5));
        double tileY = ((1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * n - (centerCoords.Y + 0.5)) * -1;

        // Statt feste 256 Pixel, benutzen wir mapSize (Pixelgröße der Mapanzeige)
        // mapSize.X = Breite in Pixel, mapSize.Y = Höhe in Pixel
        double x = tileX * mapSize.X;
        double y = tileY * mapSize.Y;

        return new Vector2((float)x, (float)y);
    }
}
