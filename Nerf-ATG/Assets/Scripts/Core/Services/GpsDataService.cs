using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Android;

public class GpsDataService : IGpsDataService
{
    public event EventHandler<GPS> NewGpsData;

    private CancellationTokenSource _gpsTokenSource;

    public async void StartGps()
    {
        #if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            await AwaitSecondsRealtime(1f); // Gib Zeit für Dialog
        }
        #endif

        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location not enabled by user.");
            return;
        }

        Input.location.Start(0.5f, 0.5f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            await AwaitSecondsRealtime(1f);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed || maxWait <= 0)
        {
            Debug.LogWarning("Unable to start location services.");
            return;
        }

        _gpsTokenSource = new CancellationTokenSource();
        _ = RunGpsUpdateLoop(_gpsTokenSource.Token);
    }

    public void StopGps()
    {
        _gpsTokenSource?.Cancel();
        if (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
        {
            Input.location.Stop();
        }
    }

    private async System.Threading.Tasks.Task RunGpsUpdateLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            UpdateGps();
            await System.Threading.Tasks.Task.Delay(50, token); // alle 50ms
        }
    }

    private void UpdateGps()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            try
            {
                var data = Input.location.lastData;
                NewGpsData?.Invoke(this, new GPS(data.latitude, data.longitude));
                Debug.Log($"GPS update: {data.latitude}, {data.longitude}, accuracy: {data.horizontalAccuracy}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        else
        {
            Debug.LogWarning("GPS not running.");
        }
    }

    private static System.Threading.Tasks.Task AwaitSecondsRealtime(float seconds)
    {
        return System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(seconds));
    }
}
