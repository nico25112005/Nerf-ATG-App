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
        DontDestroyOnLoad(this);
    }

    public void StartGps()
    {
        if (gpsCoroutine != null)
            return;
        Debug.Log("GpsRoutine started");
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

        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location not enabled by user.");
            yield break;
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
            Debug.Log("Getting GPS Data");
            if (Input.location.status == LocationServiceStatus.Running)
            {
                var data = Input.location.lastData;
                NewGpsData?.Invoke(this, new GPS(data.longitude, data.latitude));
            }
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }
}
