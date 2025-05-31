using System;
using UnityEngine;

public class FakeTcpClientService : ITcpClientService
{
    private static FakeTcpClientService _instance;
    private static readonly object _lock = new object();

    private FakeTcpClientService() { }

    public static FakeTcpClientService Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new FakeTcpClientService();
                }
                return _instance;
            }
        }
    }

    public event EventHandler<bool> Connected;
    public event EventHandler<byte[]> dataReceived;

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

    public void Send(ITcpClientService.Connections connectionId, Packet<ClientPacketType> packet)
    {
        Debug.Log($"Send: {connectionId}, {packet.GetType()}");
    }
}
