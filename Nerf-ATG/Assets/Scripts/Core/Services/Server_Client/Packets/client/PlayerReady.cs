using System;
using System.Text;
using Game.Enums;

public class PlayerReady : Packet<ClientPacketType>
{
    public string playerId {get; private set;}
    public byte healt {get; private set;}
    public byte weapon {get; private set;}
    public byte damping {get; private set;}

    public PlayerReady(byte[] bytes) : base(bytes, ClientPacketType.PlayerReady)
    {
    }

    public PlayerReady(string playerId, byte healt, WeaponType weapon, byte damping) : base(ClientPacketType.PlayerReady)
    {
        this.playerId = playerId;
        this.healt = healt;
        this.weapon = (byte)weapon;
        this.damping = damping;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 4, 8);
        healt = bytes[12];
        weapon = bytes[13];
        damping = bytes[14];

    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();

        Array.Copy(Encoding.UTF8.GetBytes(playerId), 0, bytes, 4, 8);
        bytes[12] = healt;
        bytes[13] = weapon;
        bytes[14] = damping;


    }

    public override string ToString()
    {
        return $"AppStarted{{playerId='{playerId}'}}";
    }
}
