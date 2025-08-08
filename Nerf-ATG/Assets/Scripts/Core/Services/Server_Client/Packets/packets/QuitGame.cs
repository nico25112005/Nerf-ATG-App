
using System;
using System.Text;


public class QuitGame : Packet<PacketType>
{
    public string PlayerId { get; private set; }

    public QuitGame(byte[] bytes) : base(bytes) { }

    public QuitGame(string playerId, PacketAction action) : base(PacketType.QuitGame, action)
    {
        this.PlayerId = playerId;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        PlayerId = Encoding.UTF8.GetString(bytes, offset, 8);
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(PlayerId), 0, bytes, offset, 8);
    }

    public override string ToString()
    {
        return $"QuitGame: {{playerId='{PlayerId}'}}";
    }
}
