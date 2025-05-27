using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;



internal class TCPClient
{

    //Singelton
    private static TCPClient instance;
    private static readonly object _lock = new();

    public static TCPClient GetInstance()
    {
        lock (_lock)
        {
            if (instance == null)
            {
                instance = new TCPClient();
                instance.Connect();
            }
            return instance;
        }
    }

    private TCPClient()
    {
        ip = "192.168.56.1";
        port = 25565;
        client = new TcpClient();
        cts = new CancellationTokenSource();
    }

    private const byte PACKET_SIZE = 64;


    //Variables

    private string ip;
    private int port;

    private TcpClient client;
    private NetworkStream networkStream;

    private CancellationTokenSource cts;

    //private Dictionary<ClientPacketType, >;


    //Methods


    public void Connect()
    {
        try
        {

            // Verbindung zum Server herstellen
            client.Connect(ip, port);
            Debug.Log("Verbunden mit dem Server.");

            // Netzwerkstream und Reader/Writer einrichten
            networkStream = client.GetStream();


            // Starten der Empfangs- und Sendetasks auf eigenen Threads
            Task.Run(() => ReceiveMessages(cts.Token));
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    // Nachricht an den Server senden (wird auf einem eigenen Task ausgeführt)
    public void SendMessage(Packet<ClientPacketType> packet)
    {
        Task.Run(() =>
        {
            try
            {
                if (client.Connected)
                {
                    byte[] data = new byte[PACKET_SIZE];
                    packet.ToBytes(data);

                    networkStream.Write(data, 0, PACKET_SIZE);

                    //writer.WriteLine(message);  // Nachricht senden
                    Debug.Log("Nachricht gesendet: ");
                }
                else
                {
                    Debug.Log("Keine Verbindung oder Nachricht leer.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        });
    }

    // Nachricht vom Server empfangen (läuft in einem eigenen Thread)
    private async Task ReceiveMessages(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (client.Available >= PACKET_SIZE)  // Wenn Daten verfügbar sind
                {
                    byte[] bytes = new byte[PACKET_SIZE];

                    networkStream.Read(bytes, 0, PACKET_SIZE);

                    string bytesString = "";
                    foreach(byte b in bytes)
                    {
                        bytesString += "|" + b.ToString();
                    }
                    Debug.Log(bytesString);

                    int type = ConvertToInt(bytes, 0, BitConverter.ToInt32);

                    Debug.LogWarning("Type: " + (ServerPacketType)type);


                }
                await Task.Delay(100); // Verzögerung, um den Thread nicht unnötig zu blockieren
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public static T ConvertToInt<T>(byte[] bytes, int startIndex, Func<byte[], int, T> bitConverter) where T : struct
    {
        int byteSize = Marshal.SizeOf<T>();

        byte[] tempBytes = new byte[byteSize];

        Array.Copy(bytes, startIndex, tempBytes, 0, byteSize);
        tempBytes = tempBytes.Reverse().ToArray();
        return bitConverter(tempBytes, 0);
        
    }
    // Verbindung schließen
    public void Close()
    {
        try
        {
            cts.Cancel();  // Stoppe die Threads
            networkStream.Close();
            client.Close();
            Debug.Log("Verbindung geschlossen.");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

}

