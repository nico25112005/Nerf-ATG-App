using Game.Enums;
using System;
using System.Text;

public class PlayerInfo : Packet<ServerPacketType>
{
    public string playerId { get; private set; }
    public string playerName { get; private set; }
    public byte teamIndex { get; private set; }

    public PlayerInfo(byte[] bytes) : base(bytes, ServerPacketType.PlayerInfo) { }

    public PlayerInfo(string playerId, string playerName, Team team)
        : base(ServerPacketType.PlayerInfo)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.teamIndex = (byte)team;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 4, 8);
        playerName = Encoding.UTF8.GetString(bytes, 12, 12).TrimEnd('\0');
        teamIndex = bytes[24];
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();
        Array.Copy(Encoding.UTF8.GetBytes(playerId), 0, bytes, 4, 8);
        Array.Copy(Encoding.UTF8.GetBytes(playerName.PadRight(12, '\0')), 0, bytes, 12, 12);
        bytes[24] = teamIndex;

    }

    public override string ToString()
    {
        return $"PlayerInfo{{playerId='{playerId}', playerName='{playerName}', teamIndex={teamIndex}}}";
    }
}
