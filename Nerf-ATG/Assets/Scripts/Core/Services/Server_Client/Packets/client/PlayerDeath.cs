using Game.Enums;
using System;
using System.Text;


public class PlayerDeath : Packet<ClientPacketType>
{
    public string playerId { get; private set; }

    public PlayerDeath(byte[] bytes) : base(bytes, ClientPacketType.PlayerDeath) { }

    public PlayerDeath(string playerId) : base(ClientPacketType.PlayerDeath)
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
        return $"AppStarted{{playerId='{playerId}'}}";
    }
}

