using Game.Enums;
using System;
using System.Text;

public class PlayerInfo : Packet<PacketType>, IPlayerInfo
{
    public string PlayerId { get; private set; }
    public string Name { get; private set; }
    public byte Index { get; private set; }

    public PlayerInfo(byte[] bytes) : base(bytes) { }

    public PlayerInfo(string playerId, string playerName, Team team, PacketAction action)
        : base(PacketType.PlayerInfo, action)
    {
        this.PlayerId = playerId;
        this.Name = playerName;
        this.Index = (byte)team;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
        Name = Encoding.UTF8.GetString(bytes, offset + 8, 12).TrimEnd('\0');
        Index = bytes[offset + 20];
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
        Array.Copy(Encoding.UTF8.GetBytes(Name.PadRight(12, '\0')), 0, bytes, offset + 8, 12);
        bytes[offset + 20] = Index;

    }

    public override string ToString()
    {
        return $"PlayerInfo{{playerId='{PlayerId}', playerName='{Name}', teamIndex={Index}}}";
    }
}
