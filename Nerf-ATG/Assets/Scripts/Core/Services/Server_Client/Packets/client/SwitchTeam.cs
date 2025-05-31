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
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');

    }

    public override void ToBytes(byte[] bytes)
    {
        Array.Copy(Encoding.UTF8.GetBytes(playerId.PadRight(12, '\0')), 0, bytes, 0, 12);

    }

    public override string ToString() => $"JoinGame{{ playerId={playerId}";
}

