using UnityEngine;
using UnityEngine.Android;

public class WiFiBridge
{
    private static WiFiBridge instance;
    private AndroidJavaClass wifiBridgeClass;
    private AndroidJavaObject activityContext;

    // Singleton Zugriff mit automatischer Initialisierung
    public static WiFiBridge Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WiFiBridge();
            }
            return instance;
        }
    }

    private WiFiBridge() { Init(); }

    // Init automatisch (Activity aus Unity holen)
    private void Init()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        // Check BT and location permissions
        if (
            !Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
            || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            || !Permission.HasUserAuthorizedPermission("android.permission.ACCESS_COARSE_LOCATION")
            || !Permission.HasUserAuthorizedPermission("android.permission.ACCESS_FINE_LOCATION")
            || !Permission.HasUserAuthorizedPermission("android.permission.ACCESS_WIFI_STATE")
            || !Permission.HasUserAuthorizedPermission("android.permission.CHANGE_WIFI_STATE")
        )
        {

            Permission.RequestUserPermissions(new string[] {
                        Permission.CoarseLocation,
                            Permission.FineLocation,
                            "android.permission.ACCESS_COARSE_LOCATION",
                            "android.permission.ACCESS_FINE_LOCATION",
                            "android.permission.ACCESS_WIFI_STATE",
                            "android.permission.CHANGE_WIFI_STATE"
                    });
        }

        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        wifiBridgeClass = new AndroidJavaClass("com.nerfatg.wifi.WiFiBridge");
        wifiBridgeClass.CallStatic("init", activityContext);
        
    }

    public bool EnableWifi()
    {
        return wifiBridgeClass.CallStatic<bool>("enableWifi");
    }

    public bool DisableWifi()
    {
        return wifiBridgeClass.CallStatic<bool>("disableWifi");
    }

    public bool StartScan()
    {
        return wifiBridgeClass.CallStatic<bool>("startScan");
    }

    public string GetScanResultsJson()
    {
        return wifiBridgeClass.CallStatic<string>("getScanResultsJson");
    }

    public string ConnectToWifi(string ssid, string password)
    {
        return wifiBridgeClass.CallStatic<string>("connectToWifi", ssid, password);
    }

    public string DisconnectFromWifi()
    {
        return wifiBridgeClass.CallStatic<string>("disconnectFromWifi");
    }

    public bool IsWifiEnabled()
    {
        return wifiBridgeClass.CallStatic<bool>("isWifiEnabled");
    }
}
