using System;

public interface ITcpClientService
{
    public enum Connections
    {
        Server,
        ESP32
    }

    void Connect(Connections connectionId, string ip, int port);

    void Send(Connections connectionId, Packet<ClientPacketType> packet);

    //void Receive();

    void Close(Connections connectionId);

    void CloseAll();

    event EventHandler<bool> Connected;
    event EventHandler<byte[]> dataReceived;
}
