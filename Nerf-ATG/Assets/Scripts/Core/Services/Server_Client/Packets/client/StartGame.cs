
using System;
using System.Text;


public class StartGame : Packet<ClientPacketType>
{
    public string playerId { get; private set; }

    public StartGame(byte[] bytes) : base(bytes, ClientPacketType.StartGame) { }

    public StartGame(string playerId) : base(ClientPacketType.StartGame)
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

    public override string ToString()
    {
        return $"ActiveAbility{{playerId='{playerId}'}}";
    }
}
