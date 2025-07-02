using Game.Enums;
using System;
using System.Text;

public class GameStarted : Packet<ServerPacketType>
{
    public string leaderId { get; private set; }
    public string leaderName { get; private set; }
    public byte playerCount { get; private set; }



    public GameStarted(byte[] bytes) : base(bytes, ServerPacketType.GameStarted) { }

    public GameStarted(string leaderId, string leaderName, byte playerCount)
        : base(ServerPacketType.GameStarted)
    {
        this.leaderId = leaderId;
        this.leaderName = leaderName;
        this.playerCount = playerCount;
    }

    public override void FromBytes(byte[] bytes)
    {
        leaderId = Encoding.UTF8.GetString(bytes, 4, 8);
        leaderName = Encoding.UTF8.GetString(bytes, 12, 12).TrimEnd('\0');
        playerCount = bytes[24];
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();

        Array.Copy(Encoding.UTF8.GetBytes(leaderId), 0, bytes, 4, 8);
        Array.Copy(Encoding.UTF8.GetBytes(leaderName.PadRight(12, '\0')), 0, bytes, 12, 12);
        bytes[24] = playerCount;
    }

    public override string ToString()
    {
        return $"GameStarted{{leaderId='{leaderId}', leaderName='{leaderName}', playerCount={playerCount}}}";
    }
}
