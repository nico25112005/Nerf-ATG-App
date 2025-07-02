using Game.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

public class GpsPresenter
{
    private Vector2 currentTile = Vector2.Zero;
    private Dictionary<string, PlayerStatus> oldPlayerStatuses = new();

    IGPSMap gpsMap;

    IPlayerModel playerModel;
    IGameModel gameModel;

    ITcpClientService tcpClientService;

    IGpsTileService gpsTileService;
    IGpsDataService gpsDataService;


    public GpsPresenter(IGPSMap gpsMap, IPlayerModel playerModel, IGameModel gameModel, IGpsTileService gpsTileService, IGpsDataService gpsDataService, ITcpClientService tcpClientService)
    {
        this.gpsMap = gpsMap;

        this.playerModel = playerModel;
        this.gameModel = gameModel;

        this.tcpClientService = tcpClientService;

        this.gpsTileService = gpsTileService;
        this.gpsDataService = gpsDataService;

        gpsDataService.NewGpsData += NewGpsData;

        playerModel.OnLocationChanged += RequestGpsMap;
        playerModel.AbilityActivated += RadarAbility;

        gameModel.onPlayerStatusChanged += UpdateMarkers;
        gameModel.onPlayerStatusRemoved += RemoveMarker;

        gpsTileService.OnTileDataReceived += UpdateGpsMap;

        gpsDataService.StartGps();

    }

    //GpsData

    public void NewGpsData(object sender, GPS gpsData)
    {
        if (gpsData != null && gpsData != playerModel.Location)
        {
            playerModel.Location = gpsData;

        }
    }

    //GpsMap
    public void RequestGpsMap(object sender, GPS gpsData)
    {

        Vector2 newTileCoordinates = gpsTileService.GpsToTileCoordinates(gpsData);
        if (newTileCoordinates != currentTile)
        {
            currentTile = newTileCoordinates;


            for (sbyte x = -1; x <= 1; x++)
            {
                for (sbyte y = -1; y <= 1; y++)
                {
                    gpsTileService.RequestTile(gpsData, x, y);
                }
            }

            foreach(PlayerStatus playerStatus in gameModel.playerStatus.Values)
            {
                UpdateMarkers(null, playerStatus);
            }
        }

        gpsMap.UpdateMapLocation(gpsTileService.GpsToTilePosition(currentTile, gpsData));
    }

    public void UpdateGpsMap(byte[] tileData, sbyte x, sbyte y)
    {
        gpsMap.UpdateTile(x,y, tileData);
    }

    public void UpdateMarkers(object sender, PlayerStatus playerStatus)
    {
        MarkerType type;
        UnityEngine.Debug.Log("Presenter: " + playerStatus);

        if (playerStatus.teamIndex == (int)playerModel.Team)
        {
            type = MarkerType.Allie;
        }
        else if (playerStatus.teamIndex >= 10)
        {
            type = (MarkerType)(playerStatus.teamIndex / 10 + 1);
        }
        else
        {
            type = MarkerType.Enemy;
        }

        UnityEngine.Debug.Log(playerStatus.playerName + ": MarkerType: " + type);

        gpsMap.PlaceMarker(type, playerStatus, gpsTileService.GpsToTilePosition(currentTile, new GPS(playerStatus.longitude, playerStatus.latitude)));

    }

    public void RemoveMarker(object sender, PlayerStatus playerStatus)
    {
        gpsMap.RemoveMarker(playerStatus);

    }


    public void RadarAbility(object sender, Abilitys ability)
    {
        if(ability == Abilitys.GPSLocate)
        {
            foreach (PlayerStatus playerStatus in gameModel.playerStatus.Values)
            {
                UpdateMarkers("Radar", playerStatus);
            }
        }
    }


    public void Dispose()
    {
        gpsDataService.StopGps();
        gpsDataService.NewGpsData -= NewGpsData;
        playerModel.OnLocationChanged -= RequestGpsMap;
        playerModel.AbilityActivated -= RadarAbility;

        gameModel.onPlayerStatusChanged -= UpdateMarkers;
    }
}