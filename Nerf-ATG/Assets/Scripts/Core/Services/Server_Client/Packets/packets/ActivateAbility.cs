
using System;
using System.Text;


public class ActivateAbility : Packet<PacketType>
{
    public string PlayerId { get; private set; }

    public ActivateAbility(byte[] bytes) : base(bytes) { }

    public ActivateAbility(string playerId, PacketAction action) : base(PacketType.ActiveAbility, action)
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
        return $"ActiveAbility: {{playerId='{PlayerId}'}}";
    }
}
