using System;
using UnityEngine.EventSystems;

public interface IGpsDataService
{
    event EventHandler<GPS> NewGpsData;
    void StartGps();
    void StopGps();
}