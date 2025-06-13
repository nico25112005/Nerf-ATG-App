using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GpsTileService : MonoBehaviour, IGpsTileService
{
    public static GpsTileService Instance { get; private set; }

    [SerializeField]
    private int zoom = 18;

    private System.Numerics.Vector2 mapSize = System.Numerics.Vector2.Zero;

    // Event für Tile-Bytes (null bei Fehler)
    public event Action<byte[], sbyte, sbyte> OnTileDataReceived;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }


    // Helper-Methode für externen async-Download-Aufruf
    public void RequestTile(GPS gps, sbyte x = 0, sbyte y = 0)
    {
        StartCoroutine(DownloadTileCoroutine(gps, x, y));
    }

    private IEnumerator DownloadTileCoroutine(GPS gps, sbyte offsetX, sbyte offsetY)
    {
        var centerTile = GpsToTileCoordinates(gps);
        int targetTileX = (int)centerTile.X + offsetX;
        int targetTileY = (int)centerTile.Y + offsetY;

        string url = $"https://tile.openstreetmap.org/{zoom}/{targetTileX}/{targetTileY}.png";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            #if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
            #else
            if (www.isNetworkError || www.isHttpError)
            #endif
            {
                Debug.LogError($"[GpsTileService] Fehler beim Laden der Tile: {www.error}");
                OnTileDataReceived?.Invoke(null, 0 , 0);
            }
            else
            {
                byte[] data = www.downloadHandler.data;
                OnTileDataReceived?.Invoke(data, offsetX, offsetY);
            }
        }
    }

    public System.Numerics.Vector2 GpsToTileCoordinates(GPS gps)
    {
        double latRad = gps.Latitude * Math.PI / 180.0;
        int n = 1 << zoom;

        int x = (int)((gps.Longitude + 180.0) / 360.0 * n);
        int y = (int)((1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * n);

        Debug.Log($"[GpsTileService] GpsToTileCoordinates: GPS({gps.Latitude}, {gps.Longitude}) -> Tile({x}, {y})");
        return new System.Numerics.Vector2(x, y);
    }
    public System.Numerics.Vector2 GpsToTilePosition(System.Numerics.Vector2 currentTile, GPS gps)
    {
        float latRad = (float)(gps.Latitude * Mathf.Deg2Rad);
        int n = 1 << zoom;
        float x = (float)(((gps.Longitude + 180.0) / 360.0 * n - (currentTile.X + 0.5)) * mapSize.X);
        float y = (float)(((1.0 - Mathf.Log((float)(Mathf.Tan(latRad) + 1.0 / Mathf.Cos(latRad))) / Mathf.PI) / 2.0 * n - (currentTile.Y + 0.5)) * mapSize.Y * -1);

        return new System.Numerics.Vector2(x, y);
    }

    public void SetMapSize(System.Numerics.Vector2 newMapSize)
    {
        mapSize = newMapSize;
        Debug.Log($"[GpsTileService] MapSize gesetzt auf ({mapSize.X}, {mapSize.Y})");
    }
}
