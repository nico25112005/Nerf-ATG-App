

using System;
using Unity.VisualScripting;

public abstract class Packet<T> where T : Enum
{
    public T Type { get; private set; }
    public PacketAction Action { get; set; }
    public Packet(byte[] bytes)
    {
        this.Type = (T)Enum.ToObject(typeof(T), bytes[0]);
        this.Action = (PacketAction)bytes[1];
        this.FromBytes(bytes);
    }

    public Packet(T type, PacketAction action)
    {
        this.Type = type;
        this.Action = action;
    }

    public void ToBytes(byte[] bytes)
    {
        bytes[0] = Convert.ToByte(Type);
        bytes[1] = (byte)Action;
        WritePayload(bytes, 4); // Payload begins after 4 bytes
    }

    public void FromBytes(byte[] bytes)
    {
        ReadPayload(bytes, 4);
    }

    protected abstract void ReadPayload(byte[] bytes, byte offset);
    protected abstract void WritePayload(byte[] bytes, byte offset);
}

public enum PacketAction
{
    Generic,
    Add,
    Update,
    Remove,
    Replace
}

