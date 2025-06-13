using System;
using System.Numerics;
using System.Timers;

public class VirtualPlayer
{
    private readonly IGameModel gameModel;
    private readonly IMainThreadExecutor mainThreadExecutor;
    private readonly float radius;
    private readonly string name;
    private readonly byte teamindex;
    private readonly GPS originalGps;
    private Timer timer;
    private readonly Guid id;
    private Random random = new Random();

    private GPS lastGps;
    private Vector2 lastOffset;
    private double health = 100;

    private const double EarthRadius = 6378137; // in Meter (WGS84)


    public VirtualPlayer(GPS gps, IGameModel gameModel, IMainThreadExecutor mainThreadExecutor, float radius, string name)
    {
        this.gameModel = gameModel;
        this.mainThreadExecutor = mainThreadExecutor;
        this.radius = radius;
        this.name = name;

        id = Guid.NewGuid();

        originalGps = gps;
        lastGps = gps;
        lastOffset = new Vector2(1, 0);
        teamindex = (byte)random.Next(0, 2);


        initTimer();
    }

    private void initTimer()
    {
        timer = new Timer(500);
        timer.AutoReset = true;
        timer.Elapsed += (sender, args) => UpdateLocation();
        timer.Start();


    }

    private void UpdateLocation()
    {
        Vector2 randomUnit = Vector2.Normalize(new Vector2(
        (float)(random.NextDouble() * 2 - 1),
        (float)(random.NextDouble() * 2 - 1)
    ));

        // Neue Richtung = Mischung aus letzter Richtung (70%) + neuer zufälliger Richtung (30%)
        Vector2 direction = Vector2.Normalize(lastOffset * 0.7f + randomUnit * 0.3f);

        // Fester Schritt in Meter pro Tick, z. B. 0.5 m
        float stepSize = 0.5f;

        // Neue GPS-Position berechnen
        lastGps = OffsetGps(lastGps, direction * stepSize);

        // Speichere die neue Richtung für das nächste Mal
        lastOffset = direction;


        if (random.NextDouble() > 0.5)
        {
            health -= 5;
        }
        else
        {
            health += 5;
        }

        if (health > 100) health = 100;
        if (health < 0) health = 0;

        PlayerStatus playerStatus = new PlayerStatus(id.ToString(), name, teamindex, lastGps.Longitude, lastGps.Latitude, (byte)health);

        mainThreadExecutor.Execute(() => gameModel.UpdatePlayerStatus(playerStatus));
    }


    private GPS OffsetGps(GPS gps, Vector2 offset)
    {

        double deltaLat = offset.Y / EarthRadius;
        double deltaLon = offset.X / (EarthRadius * Math.Cos(gps.Latitude * Math.PI / 180));

        double newLat = gps.Latitude + deltaLat * 180 / Math.PI;
        double newLon = gps.Longitude + deltaLon * 180 / Math.PI;

        GPS newGps = new GPS((float)newLon, (float)newLat);

        if (GPS.CalculateDistance(newGps, originalGps) >= radius)
        {
            return gps;
        }
        else
        {
            return newGps;
        }

    }

    public void StopVirutalPlayer()
    {
        timer.Stop();
        timer.Dispose();
    }


}