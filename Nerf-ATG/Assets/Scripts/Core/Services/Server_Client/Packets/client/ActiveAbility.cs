
using System;
using System.Text;


public class ActiveAbility : Packet<ClientPacketType>
{
    public string playerId { get; private set; }

    public ActiveAbility(byte[] bytes) : base(bytes, ClientPacketType.ActiveAbility) { }

    public ActiveAbility(string playerId) : base(ClientPacketType.ActiveAbility)
    {
        this.playerId = playerId;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 4, 8);
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();
        Array.Copy(Encoding.UTF8.GetBytes(playerId), 0, bytes, 4, 8);
    }

    public override string ToString()
    {
        return $"ActiveAbility: {{playerId='{playerId}'}}\nType: {this.GetType()}";
    }
}
