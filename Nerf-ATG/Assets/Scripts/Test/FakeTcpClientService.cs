using System;
using System.Net.Sockets;
using System.Text;


public class FakeTcpClient : ITcpClientService
{
    public void Close(string connectionId)
    {
        throw new NotImplementedException();
    }

    public void CloseAll()
    {
    }

    public void Connect(string connectionId, string ip, int port)
    {
        throw new NotImplementedException();
    }

    public void SendMessage(string connectionId, Packet<ClientPacketType> packet)
    {
        throw new NotImplementedException();
    }
}
