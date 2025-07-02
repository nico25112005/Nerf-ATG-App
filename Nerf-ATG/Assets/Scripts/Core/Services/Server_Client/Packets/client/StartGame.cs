
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
        playerId = Encoding.UTF8.GetString(bytes, 4, 8);
    }

    public override void ToBytes(byte[] bytes)
    {
        bytes[0] = (byte)GetType();

        Array.Copy(Encoding.UTF8.GetBytes(playerId), 0, bytes, 4, 8);
    }

    public override string ToString()
    {
        return $"StartGame{{ playerId='{playerId}' }}";
    }
}
