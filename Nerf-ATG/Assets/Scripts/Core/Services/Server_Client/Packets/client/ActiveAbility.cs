
using System;
using System.Text;


public class ActiveAbility : Packet<ClientPacketType>
{
    private string playerId;

    public ActiveAbility(byte[] bytes) : base(bytes, ClientPacketType.ActiveAbility) { }

    public ActiveAbility(string playerId) : base(ClientPacketType.ActiveAbility)
    {
        this.playerId = playerId;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');
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
        return $"ActiveAbility{{playerId='{playerId}'}}";
    }
}
