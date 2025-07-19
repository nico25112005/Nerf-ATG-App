using Game.Enums;
using System;
using System.Text;

public class GameStarted : Packet<PacketType>
{
    public string LeaderId { get; private set; }
    public string LeaderName { get; private set; }
    public byte TeamIndex { get; private set; }
    public byte PlayerCount { get; private set; }

    public GameStarted(byte[] bytes) : base(bytes) { }

    public GameStarted(string leaderId, string leaderName, byte teamIndex, byte playerCount, PacketAction action)
        : base(PacketType.GameStarted, action)
    {
        this.LeaderId = leaderId;
        this.LeaderName = leaderName;
        this.TeamIndex = teamIndex;
        this.PlayerCount = playerCount;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        LeaderId = Encoding.UTF8.GetString(bytes, offset, 8);
        LeaderName = Encoding.UTF8.GetString(bytes, offset + 8, 12).TrimEnd('\0');
        TeamIndex = bytes[offset + 20];
        PlayerCount = bytes[offset + 21];
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {

        Array.Copy(Encoding.UTF8.GetBytes(LeaderId), 0, bytes, offset, 8);
        Array.Copy(Encoding.UTF8.GetBytes(LeaderName.PadRight(12, '\0')), 0, bytes, offset + 8, 12);
        bytes[offset + 20] = TeamIndex;
        bytes[offset + 21] = PlayerCount;
    }

    public override string ToString()
    {
        return $"GameStarted{{leaderId='{LeaderId}', leaderName='{LeaderName}', playerCount={PlayerCount}}}";
    }
}
