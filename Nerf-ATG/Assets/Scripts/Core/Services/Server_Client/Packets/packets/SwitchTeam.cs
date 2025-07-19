using System;
using System.Text;


public class SwitchTeam : Packet<PacketType>
{
    public string PlayerId { get; private set; }

    public SwitchTeam(byte[] bytes) : base(bytes) { }

    public SwitchTeam(string playerId, PacketAction action) : base(PacketType.SwitchTeam, action)
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
        return $"SwitchTeam{{ playerId='{PlayerId}' }}";
    } 
}

