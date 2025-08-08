using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class GpsDataService : MonoBehaviour, IGpsDataService
{
    public static GpsDataService Instance { get; private set; }

    public event EventHandler<GPS> NewGpsData;

    private Coroutine gpsCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void StartGps()
    {
        if (gpsCoroutine != null)
            return;
        gpsCoroutine = StartCoroutine(GpsRoutine());
    }

    public void StopGps()
    {
        if (gpsCoroutine != null)
        {
            StopCoroutine(gpsCoroutine);
            gpsCoroutine = null;
        }

        if (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
        {
            Input.location.Stop();
        }
    }

    private IEnumerator GpsRoutine()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSecondsRealtime(1f); // Warte auf Dialog
        }
#endif
        byte maxAttempts = 24;
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location not enabled by user.");
            ToastNotification.Show("Please enable GPS", "error");
            if(maxAttempts > 0)
            {
                maxAttempts--;
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                GameManager.GetInstance().ResetGame();
                yield break;
            }
                
        }

        Input.location.Start(0.5f, 0.5f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait-- > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
        }

        if (Input.location.status == LocationServiceStatus.Failed || maxWait <= 0)
        {
            Debug.LogWarning("Unable to start location services.");
            yield break;
        }

        // Loop
        while (true)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                var data = Input.location.lastData;
                NewGpsData?.Invoke(this, new GPS(data.longitude, data.latitude));
            }
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }
}
