using System;
using System.Text;


public class SwitchTeam : Packet<ClientPacketType>
{
    public string playerId { get; private set; }

    public SwitchTeam(byte[] bytes) : base(bytes, ClientPacketType.SwitchTeam) { }

    public SwitchTeam(string playerId) : base(ClientPacketType.SwitchTeam)
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
        return $"SwitchTeam{{ playerId='{playerId}' }}";
    } 
}

