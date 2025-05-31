using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

internal class TcpClientService : ITcpClientService
{
    private const byte PACKET_SIZE = 64;

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
            return;
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

    public void Send(ITcpClientService.Connections connectionId, Packet<ClientPacketType> packet)
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
                    byte[] data = new byte[PACKET_SIZE];
                    packet.ToBytes(data);
                    conn.Stream.Write(data, 0, PACKET_SIZE);
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

        try
        {
            while (!token.IsCancellationRequested)
            {
                if (conn.Client.Available >= PACKET_SIZE)
                {
                    byte[] bytes = new byte[PACKET_SIZE];
                    await conn.Stream.ReadAsync(bytes, 0, PACKET_SIZE, token);

                    dataReceived.Invoke(this, bytes);
                }

                await Task.Delay(100, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Erwartetes Abbruchsignal
        }
        catch (Exception)
        {
            // Fehlerbehandlung nach Bedarf
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

}
