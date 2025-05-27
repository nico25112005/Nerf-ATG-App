using System;
using System.Text;
using Game.Enums;

public class PlayerReady : Packet<ClientPacketType>
{
    private string playerId;
    private int healt;
    private WeaponType weapon;
    private int damping;

    public PlayerReady(byte[] bytes) : base(bytes, ClientPacketType.AppStarted)
    {
    }

    public PlayerReady(string playerId, int healt, WeaponType weapon, int damping) : base(ClientPacketType.AppStarted)
    {
        this.playerId = playerId;
        this.healt = healt;
        this.weapon = weapon;
        this.damping = damping;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');
        healt = TCPClient.ConvertToInt(bytes, 12, BitConverter.ToInt32);
        weapon = (WeaponType)TCPClient.ConvertToInt(bytes, 14, BitConverter.ToInt32);
        damping = TCPClient.ConvertToInt(bytes, 16, BitConverter.ToInt32);

    }

    public override void ToBytes(byte[] bytes)
    {
        Array.Copy(Encoding.UTF8.GetBytes(playerId.PadRight(12, '\0')), 0, bytes, 0, 12);

    }

    public string GetPlayerId()
    {
        return playerId;
    }

    public override string ToString()
    {
        return $"AppStarted{{playerId='{playerId}'}}";
    }
}
