using System;
using System.Text;


public class ReadyPlayerCount : Packet<PacketType>
{
    public byte ReadyPlayers { get; private set; }

    public ReadyPlayerCount(byte[] bytes) : base(bytes) { }

    public ReadyPlayerCount(byte readyPlayerCount, PacketAction action) : base(PacketType.ReadyPlayerCount, action)
    {
        this.ReadyPlayers = readyPlayerCount;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        ReadyPlayers = bytes[offset];
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        bytes[offset] = ReadyPlayers;
    }

    public override string ToString()
    {
        return $"ReadyPlayerCount{{ readyPlayerCount={ReadyPlayers} }}";
    } 
}

