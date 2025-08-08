
using System;
using System.Text;


public class ServerMessage : Packet<PacketType>
{
    public string Message { get; private set; }

    public ServerMessage(byte[] bytes) : base(bytes) { }

    public ServerMessage(string message, PacketAction action) : base(PacketType.ServerMessage, action)
    {
        this.Message = message;
    }

    protected override void ReadPayload(byte[] bytes, byte offset)
    {
        Message = Encoding.UTF8.GetString(bytes, offset, 32);
    }

    protected override void WritePayload(byte[] bytes, byte offset)
    {
        Array.Copy(Encoding.UTF8.GetBytes(Message), 0, bytes, offset, 32);
    }

    public override string ToString()
    {
        return $"ServerMessage: {{Message='{Message}'}}";
    }
}
