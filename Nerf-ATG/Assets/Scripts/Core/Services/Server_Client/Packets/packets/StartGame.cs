
using System;
using System.Text;


public class StartGame : Packet<PacketType>
{
    public string PlayerId { get; private set; }

    public StartGame(byte[] bytes) : base(bytes) { }

    public StartGame(string playerId, PacketAction action) : base(PacketType.StartGame, action)
    {
        this.PlayerId = playerId;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
    }

    public override string ToString()
    {
        return $"StartGame{{ playerId='{PlayerId}' }}";
    }
}
