using Game.Enums;
using System;
using System.Text;
using Zenject.SpaceFighter;

public class PlayerStatus : Packet<ServerPacketType>
{
    public string playerId { get; private set; }
    public string playerName { get; private set; }
    public byte teamIndex { get; private set; }
    public double longitude { get; private set; }
    public double latitude { get; private set; }
    public byte health { get; private set; }

    public PlayerStatus(byte[] bytes) : base(bytes, ServerPacketType.PlayerStatus) { }

    public PlayerStatus(string playerId, string playerName, Team team, double longitude, double latitude, byte health)
        : base(ServerPacketType.PlayerStatus)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.teamIndex = (byte)team;
        this.longitude = longitude;
        this.latitude = latitude;
        this.health = health;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 4, 8);
        playerName = Encoding.UTF8.GetString(bytes, 12, 12).TrimEnd('\0');
        teamIndex = bytes[24];
        longitude = BitConverter.ToDouble(bytes, 25);
        latitude = BitConverter.ToDouble(bytes, 38);
        health = bytes[46];
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();

        Array.Copy(Encoding.UTF8.GetBytes(playerId), 0, bytes, 4, 8);
        Array.Copy(Encoding.UTF8.GetBytes(playerName.PadRight(12, '\0')), 0, bytes, 12, 12);
        bytes[24] = teamIndex;
        Array.Copy(BitConverter.GetBytes(longitude), 0, bytes, 25, 8);
        Array.Copy(BitConverter.GetBytes(latitude), 0, bytes, 38, 8);
        bytes[46] = health;

    }

    public override string ToString()
    {
        return $"PlayerStatus{{playerId='{playerId}', playerName='{playerName}', teamIndex={teamIndex}, longitude={longitude}, latitude={latitude}, health={health}}}";
    }
}
