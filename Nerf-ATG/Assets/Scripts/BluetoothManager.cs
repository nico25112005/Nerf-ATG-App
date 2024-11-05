using Game;
using Game.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Android;


public class BluetoothManager : MonoBehaviour
{
    private static BluetoothManager instance;
    private static readonly object _lock = new();

    protected Player player = Player.GetInstance();


    void EnsureMainThreadDispatcher()
    {
        if (FindObjectOfType<MainThreadDispatcher>() == null)
        {
            GameObject dispatcher = new GameObject("MainThreadDispatcher");
            dispatcher.AddComponent<MainThreadDispatcher>();
        }
    }
    public static BluetoothManager GetInstance()
    {

        lock (_lock)
        {
            if (instance == null)
            {
                // Create a new GameObject to attach the BluetoothManager if one does not exist
                GameObject singletonObject = new();
                instance = singletonObject.AddComponent<BluetoothManager>();
                singletonObject.name = typeof(BluetoothManager).ToString();

                // Ensure the instance is not destroyed when loading a new scene
                DontDestroyOnLoad(singletonObject);
            }
            return instance;
        }

    }

    public event EventHandler<string> NewDevice;
    public event EventHandler<bool> ConnectionChanged;

    private bool connected;
    public bool Connected
    {
        get { return connected; }
        set
        {
            connected = value;
            ConnectionChanged?.Invoke(this, value);
        }
    }

    public static Dictionary<string, string> devices = new();
    private static AndroidJavaClass unity3dbluetoothplugin;
    private static AndroidJavaObject BluetoothConnector;

    void Awake()
    {
        lock (_lock)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitBluetooth();
        EnsureMainThreadDispatcher();
    }

    // creating an instance of the bluetooth class from the plugin 
    public void InitBluetooth()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        // Check BT and location permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
            || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADMIN")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
        {

            Permission.RequestUserPermissions(new string[] {
                        Permission.CoarseLocation,
                            Permission.FineLocation,
                            "android.permission.BLUETOOTH_ADMIN",
                            "android.permission.BLUETOOTH",
                            "android.permission.BLUETOOTH_SCAN",
                            "android.permission.BLUETOOTH_ADVERTISE",
                             "android.permission.BLUETOOTH_CONNECT"
                    });

        }

        unity3dbluetoothplugin = new AndroidJavaClass("com.example.unity3dbluetoothplugin.BluetoothConnector");
        BluetoothConnector = unity3dbluetoothplugin.CallStatic<AndroidJavaObject>("getInstance");
    }

    // Start device scan
    public void StartScanDevices()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        devices.Clear();

        BluetoothConnector.CallStatic("StartScanDevices");
    }

    // Stop device scan
    public void StopScanDevices()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        BluetoothConnector.CallStatic("StopScanDevices");
    }

    // This function will be called by Java class to update the scan status,
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void ScanStatus(string status)
    {
        Toast("Scan Status: " + status);
    }

    // This function will be called by Java class whenever a new device is found,
    // and delivers the new devices as a string data="MAC+NAME"
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void NewDeviceFound(string data)
    {

        if (data.Split("+")[1] != "null")
        {
            devices.Add(data.Split("+")[1], data.Split("+")[0]);
            if (NewDevice == null)
            {
                Debug.Log("NewDevice not subscribed to");
                return;
            }
            NewDevice.Invoke(this, data.Split("+")[1]);
        }
    }

    // Get paired devices from BT settings
    public void GetPairedDevices()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        // This function when called returns an array of PairedDevices as "MAC+Name" for each device found
        string[] data = BluetoothConnector.CallStatic<string[]>("GetPairedDevices");

        foreach (var item in data)
        {
            devices.Add(item.Split("+")[1], item.Split("+")[0]);
        }
    }

    // Start BT connect using device MAC address "deviceAdd"
    public void StartConnection(string macAdress)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        BluetoothConnector.CallStatic("StartConnection", macAdress.ToUpper());
    }

    // Stop BT connetion
    public void StopConnection()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        if (Connected)
            BluetoothConnector.CallStatic("StopConnection");
    }

    // This function will be called by Java class to update BT connection status,
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void ConnectionStatus(string status)
    {
        Toast("Connection Status: " + status);
        Connected = status == "connected";
        Debug.Log("Connection Status:" + Connected);
    }

    // This function will be called by Java class whenever BT data is received,
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void ReadData(string data)
    {
        Debug.LogWarning("Read Data: " + data);
        ThreadPool.QueueUserWorkItem(ProcessData, data);
    }

    // Write data to the connected BT device
    public void WriteData(string data)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        Debug.Log(Connected);
        if (Connected)
            BluetoothConnector.CallStatic("WriteData", data);
    }

    // This function will be called by Java class to send Log messages,
    // DO NOT CHANGE ITS NAME OR IT WILL NOT BE FOUND BY THE JAVA CLASS
    public void ReadLog(string data)
    {
        Toast(data);
        Debug.Log(data);
    }

    // Function to display an Android Toast message
    public void Toast(string data)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        BluetoothConnector.CallStatic("Toast", data);
    }


    // Own Functions //

    void ProcessData(object state)
    {
        BluetoothManager threadBluetooth = BluetoothManager.GetInstance();
        string data = (string)state;

        bool gpsRequest = false;

        string teammatesLocation = "";
        string enemysLocation = "";

        try
        {
            if (data.Length > 4) //Recived Data: TeamMate1 / TeamMate2 # Enemy1 / Enemy2 ...
            {
                if (data.Split("#").Length != 2)
                {
                    if (data.StartsWith("#"))
                        enemysLocation = data.Substring(1);  // Entfernt das führende "#"
                    else
                        teammatesLocation = data;
                }
                else
                {
                    // Teilt die Daten auf, wenn zwei Teile existieren
                    teammatesLocation = data.Split("#")[0];
                    enemysLocation = data.Split("#")[1];
                }

                try
                {
                    // Nur wenn teammatesLocation Daten enthält, wird sie verarbeitet
                    if (!string.IsNullOrEmpty(teammatesLocation))
                    {
                        foreach (string teammateGPSData in teammatesLocation.Split("/"))
                        {
                            player.SetTeamMateLocation(teammateGPSData);
                        }
                    }

                    // Nur wenn enemysLocation Daten enthält, wird sie verarbeitet
                    if (!string.IsNullOrEmpty(enemysLocation))
                    {
                        foreach (string enemyGPSData in enemysLocation.Split("/"))
                        {
                            player.SetEnemyLocation(enemyGPSData);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

            }
            else // Recived Data: | 0000 0000 | 0000 00 | 0 | 0 | Damage, Ammo, Reload, GPS - request
            {



                int hexData = Convert.ToInt32(data, 16);

                Debug.Log($"Data: {hexData:X}");

                if ((hexData & 0xFF00) != 0) // Damage
                {
                    if (player.Health - (byte)(hexData >> 8) >= 0)
                    {
                        MainThreadDispatcher.Execute(() => player.Health -= (byte)(hexData >> 8));
                    }
                    else
                    {
                        MainThreadDispatcher.Execute(() => player.Health = 0);
                    }
                }

                if ((hexData & 0x00FC) != 0) // Ammo
                {
                    if (player.Ammo - (byte)((hexData & 0x00FC) >> 2) >= 0)
                    {
                        MainThreadDispatcher.Execute(() => player.Ammo -= (byte)((hexData & 0x00FC) >> 2));
                    }
                    else throw new Exception("To less Ammo");
                }

                if ((hexData & 0x0002) != 0) // Reload
                {

                    if (player.MaxAmmo - (Settings.weaponInfo[player.WeaponType].AmmoPerMag - player.Ammo) >= 0)
                    {
                        MainThreadDispatcher.Execute(() =>
                        {
                            player.MaxAmmo -= (Settings.weaponInfo[player.WeaponType].AmmoPerMag - player.Ammo);
                            player.Ammo = Settings.weaponInfo[player.WeaponType].AmmoPerMag;
                        });
                    }
                    else
                    {
                        if (player.MaxAmmo != 0)
                        {
                            MainThreadDispatcher.Execute(() =>
                            {
                                player.Ammo += (byte)player.MaxAmmo;
                                player.MaxAmmo = 0;
                            });
                        }
                        else throw new Exception("You need to refill your Ammo");
                    }
                }

                gpsRequest = (hexData & 0x0001) != 0; // GPS-request
                // Use MainThreadDispatcher to call WriteData on the main thread
                MainThreadDispatcher.Execute(() => threadBluetooth.WriteData(createResponseData(gpsRequest) + "\n"));
            }

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    string createResponseData(bool gpsRequest)
    {
        if (player.Health == 0)
            return "FFF";

        ulong responseData = (ulong)(player.AbilityActivated ? 1 : 0);
        responseData += (ulong)(player.Ammo << 1);

        if (gpsRequest)
            responseData += player.GPSData.SerialData << 8;

        return responseData.ToString("X");
    }

    public void SendBaseInformations()
    {
        int data = player.Health;
        data += (player.Upgrades[UpgradeType.Damping] << 8);
        data += (((byte)player.WeaponType) << 10);
        data += (((byte)player.TeamInfo) << 12);

        WriteData(data.ToString("X") + "\n");
    }
}
