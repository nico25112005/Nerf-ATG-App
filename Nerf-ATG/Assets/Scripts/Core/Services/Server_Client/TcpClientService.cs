using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

internal class TcpClientService : ITcpClientService
{
    private class ConnectionData
    {
        public TcpClient Client;
        public NetworkStream Stream;
        public CancellationTokenSource Cancellation;
    }

    private readonly Dictionary<ITcpClientService.Connections, ConnectionData> connections = new();

    public event EventHandler<bool> Connected;
    public event EventHandler<byte[]> dataReceived;

    public void Connect(ITcpClientService.Connections connectionId, string ip, int port)
    {
        if (connections.ContainsKey(connectionId))
        {
            throw new Exception("Already Connected");
        }

        try
        {
            var client = new TcpClient();
            client.Connect(ip, port);
            var stream = client.GetStream();
            var cts = new CancellationTokenSource();

            connections[connectionId] = new ConnectionData
            {
                Client = client,
                Stream = stream,
                Cancellation = cts
            };
            Task.Run(() => ReceiveMessages(connectionId, cts.Token));
        }
        catch (Exception)
        {
            // Fehlerbehandlung nach Bedarf
        }
    }

    public void Send(ITcpClientService.Connections connectionId, Packet<PacketType> packet)
    {
        if (!connections.TryGetValue(connectionId, out var conn))
        {
            return;
        }

        Task.Run(() =>
        {
            try
            {
                if (conn.Client.Connected)
                {
                    byte[] data = new byte[ITcpClientService.PACKET_SIZE];
                    packet.ToBytes(data);
                    conn.Stream.Write(data, 0, ITcpClientService.PACKET_SIZE);
                }
            }
            catch (Exception)
            {
                // Fehlerbehandlung nach Bedarf
            }
        });
    }

    private async Task ReceiveMessages(ITcpClientService.Connections connectionId, CancellationToken token)
    {
        if (!connections.TryGetValue(connectionId, out var conn))
            return;

        const int PacketSize = ITcpClientService.PACKET_SIZE;
        var readBuffer = new byte[4096];
        var packetBuffer = new List<byte>(PacketSize * 4);

        try
        {
            while (!token.IsCancellationRequested)
            {
                int bytesRead = await conn.Stream.ReadAsync(readBuffer, 0, readBuffer.Length, token);
                if (bytesRead == 0)
                    break; // lost connection

               
                packetBuffer.AddRange(readBuffer.AsSpan(0, bytesRead).ToArray());

                
                while (packetBuffer.Count >= PacketSize)
                {
                    var packet = packetBuffer.GetRange(0, PacketSize).ToArray();
                    packetBuffer.RemoveRange(0, PacketSize);

                    dataReceived?.Invoke(connectionId, packet);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // normaler Abbruch
        }
        catch (Exception ex)
        {
            // Logging z.B.
        }
    }


    public void Close(ITcpClientService.Connections connectionId)
    {
        if (connections.TryGetValue(connectionId, out var conn))
        {
            try
            {
                conn.Cancellation.Cancel();
                conn.Stream.Close();
                conn.Client.Close();
                connections.Remove(connectionId);
            }
            catch (Exception)
            {
                // Fehlerbehandlung nach Bedarf
            }
        }
    }

    public void CloseAll()
    {
        foreach (var connectionId in connections.Keys.ToList())
        {
            Close(connectionId);
        }
    }

    public void imitateReceive(ITcpClientService.Connections connectionId, byte[] data)
    {
        dataReceived.Invoke(connectionId, data);
    }
}
