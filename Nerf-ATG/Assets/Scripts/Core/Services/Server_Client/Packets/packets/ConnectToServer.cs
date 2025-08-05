
using System;
using System.Text;


public class ConnectToServer : Packet<PacketType>
{
    public string PlayerId { get; private set; }
    public string PlayerName { get; private set; }

    public ConnectToServer(byte[] bytes) : base(bytes) { }

    public ConnectToServer(string playerId, string playerName, PacketAction action) : base(PacketType.ConnectToServer, action)
    {
        this.PlayerId = playerId;
        this.PlayerName = playerName;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
        PlayerName = Encoding.UTF8.GetString(bytes, offset + 8, 12).TrimEnd('\0');
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
        Array.Copy(Encoding.UTF8.GetBytes(PlayerName.PadRight(12, '\0')), 0, bytes, offset + 8, 12);
    }

    public override string ToString()
    {
        return $"PlayerInfo: {{playerId='{PlayerId}', playerName='{PlayerName}'}}";
    }
}
