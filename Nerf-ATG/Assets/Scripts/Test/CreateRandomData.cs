

using Game.Enums;
using System;

public static class CreateRandomData
{
    private static Random _random = new Random();
    public static GameInfo CreateGameInfo()
    {
        return new GameInfo
        (
            (GameType)_random.Next(0, Enum.GetValues(typeof(GameType)).Length),
            Guid.NewGuid().ToString("N").Substring(0, 5),
            $"Spiel {_random.Next(1, 100)}",
            _random.Next(1, 10),
            _random.Next(10, 20)
        );
    }

    public static PlayerInfo CreatePlayerInfo()
    {
        string playerId = Guid.NewGuid().ToString();
        string playerName = $"Player {(_random.Next(1, 100))}";
        int teamIndex = _random.Next(0, 3); // 0 oder 1 oder 2

        return new PlayerInfo(playerId, playerName, teamIndex);
    }

    public static PlayerStatus CreatePlayerStatus(GPS center, string name, double rangeMeters = 20)
    {
        string id = $"Player{_random.Next(1, 100)}";
        int teamIndex = _random.Next(0, 2);

        // Umrechnung Meter in Grad Latitude (konstant)
        double latOffset = ((_random.NextDouble() * 2) - 1) * (rangeMeters / 111000.0);

        // Umrechnung Meter in Grad Longitude (abhängig von Latitude)
        double metersPerDegreeLon = 111000.0 * Math.Cos(center.Latitude * Math.PI / 180.0);
        double lonOffset = ((_random.NextDouble() * 2) - 1) * (rangeMeters / metersPerDegreeLon);

        double latitude = center.Latitude + latOffset;
        double longitude = center.Longitude + lonOffset;

        int health = _random.Next(50, 101);

        return new PlayerStatus(id, name, teamIndex, longitude, latitude, health);
    }
}
