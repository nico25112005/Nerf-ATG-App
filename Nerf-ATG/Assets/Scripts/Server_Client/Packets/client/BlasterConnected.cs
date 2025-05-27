
using System;
using UnityEngine;
using System.Text;


public class BlasterConnected : Packet<ClientPacketType>
{
    private string playerId;

    public BlasterConnected(byte[] bytes) : base(bytes, ClientPacketType.AppStarted) { }

    public BlasterConnected(string playerId) : base(ClientPacketType.AppStarted)
    {
        this.playerId = playerId;
    }

    public override void FromBytes(byte[] bytes)
    {
        playerId = Encoding.UTF8.GetString(bytes, 0, 12).TrimEnd('\0');

    }

    public override void ToBytes(byte[] bytes)
    {
        Array.Copy(Encoding.UTF8.GetBytes(playerId.PadRight(12, '\0')), 0, bytes, 4, 12);
    }

    public string GetPlayerId()
    {
        return playerId;
    }

    public override string ToString()
    {
        return $"AppStarted{{playerId='{playerId}'}}";
    }
}

