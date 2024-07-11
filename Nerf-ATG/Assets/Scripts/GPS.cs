using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Android;

public class GPS : MonoBehaviour
{
    public static double CalculateDistance(GPSData gps1, GPSData gps2)
    {
        Debug.LogWarning("Gps1:" + gps1.Longitude + " " + gps1.Latitude);
        Debug.LogWarning("Gps2:" + gps2.Longitude + " " + gps2.Latitude);

        double dlon = gps2.Longitude - gps1.Longitude;
        double dlat = gps2.Latitude - gps1.Latitude;

        double a = Math.Pow(Math.Sin(dlat / 2), 2) + Math.Cos(gps1.Latitude) * Math.Cos(gps2.Latitude) * Math.Pow(Math.Sin(dlon / 2), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = 63710 * c; // Radius der Erde in Metern

        return distance;
    }

}
