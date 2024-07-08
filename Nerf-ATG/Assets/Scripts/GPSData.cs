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
                _ = CalculateSerialDataAsync();
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
                _ = CalculateSerialDataAsync();
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
        private set
        {
            lock (_lock)
            {
                serialData = value;
            }
        }
    }

    private async Task CalculateSerialDataAsync()
    {
        SerialData = await Task.Run(() => CalculateSerialData());
    }

    private ulong CalculateSerialData()
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
}
