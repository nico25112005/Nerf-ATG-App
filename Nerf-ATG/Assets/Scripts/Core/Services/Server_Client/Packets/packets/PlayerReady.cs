using System;
using System.Text;
using Game.Enums;

public class PlayerReady : Packet<PacketType>
{
    public string PlayerId {get; private set;}
    public byte Healt {get; private set;}
    public byte Weapon {get; private set;}
    public byte Damping {get; private set;}

    public PlayerReady(byte[] bytes) : base(bytes)
    {
    }

    public PlayerReady(string playerId, byte healt, WeaponType weapon, byte damping, PacketAction action) : base(PacketType.PlayerReady, action)
    {
        this.PlayerId = playerId;
        this.Healt = healt;
        this.Weapon = (byte)weapon;
        this.Damping = damping;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
        Healt = bytes[offset + 8];
        Weapon = bytes[offset + 9];
        Damping = bytes[offset + 10];

    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
        bytes[offset + 8] = Healt;
        bytes[offset + 9] = Weapon;
        bytes[offset + 10] = Damping;


    }

    public override string ToString()
    {
        return $"AppStarted{{playerId='{PlayerId}'}}";
    }
}
