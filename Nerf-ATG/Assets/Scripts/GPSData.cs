using System;
using System.Threading.Tasks;
using UnityEngine;

public class GPSData
{
    private double latitude;
    private double longitude;
    private ulong serialData;
    private readonly object _lock = new object(); // Lock-Objekt für die Synchronisierung

    public double Latitude
    {
        get { return latitude; }
        set
        {
            try
            {
                latitude = value;
                _ = EncodeSerialDataAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }
    }

    public double Longitude
    {
        get { return longitude; }
        set
        {
            try
            {
                longitude = value;
                _ = EncodeSerialDataAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
        }
    }

    public ulong SerialData
    {
        get
        {
            lock (_lock)
            {
                return serialData;
            }
        }
        set
        {
            lock (_lock)
            {
                serialData = value;
                _ = DecodeSerialDataAsync(serialData); // Asynchrone Dekodierung aufrufen
            }
        }
    }

    private async Task EncodeSerialDataAsync()
    {
        serialData = await Task.Run(() => EncodeSerialData());
    }

    private ulong EncodeSerialData()
    {
        double nominalLatitude = latitude + 90.0;

        short latDegrees = (short)Math.Floor(nominalLatitude);
        nominalLatitude = (nominalLatitude - latDegrees) * 60;
        byte latMinutes = (byte)Math.Floor(nominalLatitude);
        nominalLatitude = (nominalLatitude - latMinutes) * 60;
        byte latSeconds = (byte)Math.Floor(nominalLatitude);
        nominalLatitude = (nominalLatitude - latSeconds) * 32;           // 2^5 = 32 5 bits zur verfügung
        byte latMilliseconds = (byte)Math.Floor(nominalLatitude);

        ulong result = (ulong)((latDegrees << 17) | (latMinutes << 11) | (latSeconds << 5) | (latMilliseconds));

        double nominalLongitude = longitude + 180.0;
        short lonDegrees = (short)Math.Floor(nominalLongitude);
        nominalLongitude = (nominalLongitude - lonDegrees) * 60;
        byte lonMinutes = (byte)Math.Floor(nominalLongitude);
        nominalLongitude = (nominalLongitude - lonMinutes) * 60;
        byte lonSeconds = (byte)Math.Floor(nominalLongitude);
        nominalLongitude = (nominalLongitude - lonSeconds) * 32;           // 2^5 = 32 5 bits zur verfügung
        byte lonMilliseconds = (byte)Math.Floor(nominalLongitude);

        result |= (ulong)((lonDegrees << 17) | (lonMinutes << 11) | (lonSeconds << 5) | (lonMilliseconds)) << 26;

        return result;
    }

    private async Task DecodeSerialDataAsync(ulong hexCode)
    {
        (double newLatitude, double newLongitude) = await Task.Run(() => DecodeSerialData(hexCode));
        lock (_lock)
        {
            latitude = newLatitude;
            longitude = newLongitude;
        }
    }

    public (double latitude, double longitude) DecodeSerialData(ulong hexCode)
    {
        // Extrahiere Latitude-Teile
        ulong latCode = hexCode & 0x3FFFFFF;
        short latDegrees = (short)((latCode >> 17) & 0x7F);
        byte latMinutes = (byte)((latCode >> 11) & 0x3F);
        byte latSeconds = (byte)((latCode >> 5) & 0x3F);
        byte latMilliseconds = (byte)(latCode & 0x1F);

        // Rekonstruiere Latitude
        double latitude = latDegrees - 90.0;
        latitude += (double)latMinutes / 60.0;
        latitude += (double)latSeconds / 3600.0;
        latitude += (double)latMilliseconds / (3600.0 * 32.0);

        // Extrahiere Longitude-Teile
        ulong lonCode = (hexCode >> 26) & 0x3FFFFFF;
        short lonDegrees = (short)((lonCode >> 17) & 0xFF);
        byte lonMinutes = (byte)((lonCode >> 11) & 0x3F);
        byte lonSeconds = (byte)((lonCode >> 5) & 0x3F);
        byte lonMilliseconds = (byte)(lonCode & 0x1F);

        // Rekonstruiere Longitude
        double longitude = lonDegrees - 180.0;
        longitude += (double)lonMinutes / 60.0;
        longitude += (double)lonSeconds / 3600.0;
        longitude += (double)lonMilliseconds / (3600.0 * 32.0);

        return (latitude, longitude);
    }
}
