using Game.Enums;
using Game;
using System.Numerics;
using System.Timers;


public class GpsPresenter
{
    private Vector2 currentTile = Vector2.Zero;

    private Timer baseRefillTimer;
    private Timer radarAbilityTimer;


    private readonly IGPSMap gpsMap;

    private readonly IPlayerModel playerModel;
    private readonly IGameModel gameModel;

    private readonly ITcpClientService tcpClientService;
    private readonly IGpsTileService gpsTileService;
    private readonly IGpsDataService gpsDataService;

    private readonly IMainThreadExecutor mainThreadExecutor;

    public GpsPresenter(IGPSMap gpsMap, IPlayerModel playerModel, IGameModel gameModel, IGpsTileService gpsTileService, IGpsDataService gpsDataService, ITcpClientService tcpClientService, IMainThreadExecutor mainThreadExecutor)
    {
        this.gpsMap = gpsMap;

        this.playerModel = playerModel;
        this.gameModel = gameModel;

        this.tcpClientService = tcpClientService;
        this.gpsTileService = gpsTileService;
        this.gpsDataService = gpsDataService;

        this.mainThreadExecutor = mainThreadExecutor;

        gpsDataService.NewGpsData += NewGpsData;

        playerModel.OnLocationChanged += RequestGpsMap;
        playerModel.AbilityActivated += RadarAbility;
        playerModel.OnLocationChanged += BaseRefill;

        gameModel.onMapPointChanged += UpdateMapPoints;

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

            foreach (PlayerStatus playerStatus in gameModel.playerInfo.Values)
            {
                UpdateMapPoints(null, playerStatus);
            }
        }

        gpsMap.UpdateMapLocation(gpsTileService.GpsToTilePosition(currentTile, gpsData));
    }

    public void UpdateGpsMap(byte[] tileData, sbyte x, sbyte y)
    {
        gpsMap.UpdateTile(x, y, tileData);
    }

    public void UpdateMapPoints(object sender, IMapPoint playerStatus)
    {
        MapPointType type;

        if(playerStatus.Index >= 10)
            type = (MapPointType)playerStatus.Index;
        else
        {
            if (playerStatus.Index == (byte)playerModel.Team)
                type = MapPointType.Allie;
            else
                type = MapPointType.Enemy;
        }

        UnityEngine.Debug.Log(playerStatus.Name + ": MarkerType: " + type);

        gpsMap.UpdateMapPoints(type, playerStatus, gpsTileService.GpsToTilePosition(currentTile, new GPS(playerStatus.Longitude, playerStatus.Latitude)));

    }

    public void RadarAbility(object sender, Abilitys ability) // Todo: Implement
    {
        if (ability == Abilitys.GPSLocate)
        {
            if(radarAbilityTimer == null)
            {
                radarAbilityTimer = new Timer(5000);
                radarAbilityTimer.AutoReset = true;
                radarAbilityTimer.Elapsed += (s, e) => RadarAbilityElapsed();
                radarAbilityTimer.Start();
            }
            playerModel.AbilityActive = true;
        }
    }

    private void RadarAbilityElapsed()
    {
        playerModel.AbilityActive = false;
        foreach(PlayerStatus playerStatus in gameModel.mapPoints.Values)
        {
            if (playerStatus.Index != (byte)playerModel.Team)
            {
                gameModel.RemoveMapPoint(playerStatus.Name);
            }
        }
    }


    public void BaseRefill(object sender, GPS gps)
    {
        if (baseRefillTimer == null)
        {
            baseRefillTimer = new Timer(1000);
            baseRefillTimer.AutoReset = true;
            baseRefillTimer.Elapsed += (s, e) => Refill();
        }

        if (GPS.IsWithinRadius(gps, gameModel.baseLocation[playerModel.Team], Game.Settings.BaseRadius))
        {
            baseRefillTimer.Start();
        }
        else
        {
            baseRefillTimer.Stop();
        }

    }

    private void Refill()
    {
        if (playerModel.Health + Settings.Healing <= Settings.Health)
        {
            MainThreadDispatcher.Execute(() => playerModel.Health += Settings.Healing);
        }
        else
        {
            if (playerModel.Health < Settings.Health)
            {
                MainThreadDispatcher.Execute(() => playerModel.Health = Settings.Health);
            }
        }

        if (playerModel.MaxAmmo + (Settings.weaponInfo[playerModel.WeaponType].MaxAmmo / 10) <= Settings.weaponInfo[playerModel.WeaponType].MaxAmmo)
        {
            MainThreadDispatcher.Execute(() => playerModel.MaxAmmo += (ushort)(Settings.weaponInfo[playerModel.WeaponType].MaxAmmo / 10));
        }
        else
        {
            if (playerModel.MaxAmmo < Settings.weaponInfo[playerModel.WeaponType].MaxAmmo)
            {
                MainThreadDispatcher.Execute(() => playerModel.MaxAmmo = Settings.weaponInfo[playerModel.WeaponType].MaxAmmo);
            }
        }

    }

    // Buttons
    public void SetBaseLocation()
    {
        tcpClientService.Send(ITcpClientService.Connections.Server, new BaseLocation(playerModel.Team, playerModel.Location, PacketAction.Add));
    }


    public void Dispose()
    {
        gpsDataService.StopGps();
        gpsDataService.NewGpsData -= NewGpsData;
        playerModel.OnLocationChanged -= RequestGpsMap;
        playerModel.AbilityActivated -= RadarAbility;
        playerModel.OnLocationChanged -= BaseRefill;

        gameModel.onMapPointChanged -= UpdateMapPoints;
    }
}