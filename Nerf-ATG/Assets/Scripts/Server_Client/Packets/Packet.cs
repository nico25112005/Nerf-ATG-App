

using System;
using Unity.VisualScripting;

public abstract class Packet<T> where T : Enum
{
    private readonly T type;
    public Packet(byte[] bytes, T type)
    {
        this.type = type;
        BitConverter.GetBytes(Convert.ToInt32(type)).CopyTo(bytes, 0);
        this.FromBytes(bytes);
    }

    public Packet(T type)
    {
        this.type = type;
    }

    public T GetType()
    {
        return type;
    }

    public abstract void FromBytes(byte[] bytes);
    public abstract void ToBytes(byte[] bytes);
}

