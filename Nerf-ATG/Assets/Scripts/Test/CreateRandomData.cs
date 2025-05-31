

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
        int teamIndex = _random.Next(0, 3); // 0 oder 1

        return new PlayerInfo(playerId, playerName, teamIndex);
    }
}
