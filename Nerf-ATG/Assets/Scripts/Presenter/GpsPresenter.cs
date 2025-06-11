using Game.Enums;
using System.Collections.Generic;
using System.Linq;
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
        playerModel.OnLocationChanged += UpdateGpsMap;
        gameModel.onPlayerStatusChanged += UpdateMarkers;
        gameModel.onPlayerStatusRemoved += RemoveMarker;

        gpsDataService.StartGps();

    }

    //GpsData

    public void NewGpsData(object sender, GPS gpsData)
    {
        if(gpsData != null && gpsData != playerModel.Location)
        {
            playerModel.Location = gpsData;
        }
    }

    //GpsMap
    public void UpdateGpsMap(object sender, GPS gpsData)
    {
        if(gpsTileService.GpsToTileCoordinates(gpsData) != currentTile)
        {
            for(sbyte x = -1 ; x <= 1; x++)
            {
                for(sbyte y = -1; y <= 1; y++)
                {
                    gpsMap.UpdateTile(x, y, gpsTileService.GetTile(gpsData, x, y));
                }
            }
        }

        gpsMap.UpdateMapLocation(gpsTileService.GpsToTilePosition(playerModel.Location, gpsData));
    }

    public void UpdateMarkers(object sender, PlayerStatus playerStatus)
    {
        MarkerType type;

        if(playerStatus.teamIndex == (int)playerModel.Team)
        {
            type = MarkerType.Allie;
        }
        else if(playerStatus.teamIndex >= 10)
        {
            type = (MarkerType)((int)playerStatus.teamIndex / 10 + 1);
        }
        else
        {
            type = MarkerType.Enemy;
        }

        gpsMap.PlaceMarker(type, playerStatus);

    }

    public void RemoveMarker(object sender, PlayerStatus playerStatus)
    {
        gpsMap.RemoveMarker(playerStatus);

    }


    public void Dispose()
    {
        gpsDataService.StopGps();
        gpsDataService.NewGpsData -= NewGpsData;
        playerModel.OnLocationChanged -= UpdateGpsMap;
        gameModel.onPlayerStatusChanged -= UpdateMarkers;
    }
}