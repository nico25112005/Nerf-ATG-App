using System;

public interface ITcpClientService
{
    public enum Connections
    {
        Server,
        ESP32
    }

    const byte PACKET_SIZE = 64;

    void Connect(Connections connectionId, string ip, int port);

    void Send(Connections connectionId, Packet<PacketType> packet);

    //void Receive();

    void Close(Connections connectionId);

    void CloseAll();

    event EventHandler<bool> ConnectionStatusChanged;
    event EventHandler<byte[]> DataReceived;

    void imitateReceive(Connections connectionId, byte[] data);
}
