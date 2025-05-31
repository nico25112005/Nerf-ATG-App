using System;
using System.Text;

public class PlayerInfo : Packet<ServerPacketType>
{
    public string playerId { get; private set; }
    public string playerName { get; private set; }
    public int teamIndex { get; private set; }

    public PlayerInfo(byte[] bytes) : base(bytes, ServerPacketType.PlayerInfo) { }

    public PlayerInfo(string playerId, string playerName, int teamIndex)
        : base(ServerPacketType.PlayerInfo)
    {
        this.playerId = playerId;
        this.playerName = playerName;
        this.teamIndex = teamIndex;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');
        playerName = Encoding.UTF8.GetString(bytes, 12, 16).TrimEnd('\0');
        teamIndex = BitConverter.ToInt32(bytes, 28);
    }

    public override void ToBytes(byte[] bytes)
    {
        byte[] idBytes = Encoding.UTF8.GetBytes(playerId.PadRight(12, '\0'));
        Array.Copy(idBytes, 0, bytes, 0, 12);

        byte[] nameBytes = Encoding.UTF8.GetBytes(playerName.PadRight(16, '\0'));
        Array.Copy(nameBytes, 0, bytes, 12, 16);

        byte[] teamBytes = BitConverter.GetBytes(teamIndex);
        Array.Copy(teamBytes, 0, bytes, 28, 4);
    }

    public override string ToString()
    {
        return $"PlayerInfo{{playerId='{playerId}', playerName='{playerName}', teamIndex={teamIndex}}}";
    }
}
