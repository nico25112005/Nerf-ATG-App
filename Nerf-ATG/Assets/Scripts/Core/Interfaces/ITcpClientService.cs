using System;

public interface ITcpClientService
{
    void Connect(string connectionId, string ip, int port);

    void SendMessage(string connectionId, Packet<ClientPacketType> packet);

    void Close(string connectionId);

    void CloseAll();
}
