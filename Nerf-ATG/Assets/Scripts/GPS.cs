using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Android;

public class GPS : MonoBehaviour
{
    public static double CalculateDistance(GPSData gps1, GPSData gps2)
    {
        double dlon = gps2.Longitude - gps1.Longitude;
        double dlat = gps2.Latitude - gps1.Latitude;

        double a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(gps1.Latitude) * Math.Cos(gps2.Latitude) * Math.Pow(Math.Sin(dlon / 2), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = 63710 * c; // Radius der Erde in Metern

        return distance;
    }

    public static bool IsWithinRadius(GPSData location1, GPSData location2, float radius)
    {
        if (location1 == null || location2 == null)
            return false;

        double distance = GPS.CalculateDistance(location1, location2);
        Debug.Log("Distance: " + distance);
        return distance <= radius; // Überprüfung, ob die Distanz kleiner oder gleich 8 Metern ist
    }
}
