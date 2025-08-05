using Game.Enums;
using System;
using System.Text;


public class JoinGame : Packet<PacketType>
{
    public string PlayerId {get; set;}
    public string GameId {get; set;}

    public JoinGame(byte[] bytes) : base(bytes) { }

    public JoinGame(string playerId, string gameId, PacketAction action) : base(PacketType.JoinGame, action)
    {
        this.PlayerId = playerId;
        this.GameId = gameId;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
        GameId = Encoding.UTF8.GetString(bytes, offset + 8, 8);
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {

        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
        Array.Copy(Encoding.UTF8.GetBytes(GameId), 0, bytes, offset + 8, 8);
    }

    public override string ToString() => $"JoinGame{{ playerId='{PlayerId}', gameId='{GameId}' }}";
}

