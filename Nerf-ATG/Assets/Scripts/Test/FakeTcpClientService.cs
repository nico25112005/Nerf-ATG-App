using System;
using UnityEngine;

public class FakeTcpClientService : ITcpClientService
{
    public event EventHandler<bool> ConnectionStatusChanged;
    public event EventHandler<byte[]> DataReceived;

    public void Close(ITcpClientService.Connections connectionId)
    {
        Debug.Log("Close");
    }

    public void CloseAll()
    {
        Debug.Log("CloseAll");
    }

    public void Connect(ITcpClientService.Connections connectionId, string ip, int port)
    {
        Debug.Log($"Connect: {connectionId}, {ip}, {port}");
    }

    public void imitateReceive(ITcpClientService.Connections connectionId, byte[] data)
    {
        DataReceived?.Invoke(connectionId, data);
    }

    public void Send(ITcpClientService.Connections connectionId, Packet<PacketType> packet)
    {
        Debug.Log($"Send: {connectionId}, {packet.Type}");
    }
}
