using System.Collections;
using System.Collections.Generic;
using UnityEngine.Android;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;
using Assets.Scripts;

public class BluetoothManager : MonoBehaviour
{
    private static BluetoothManager instance;
    private static readonly object _lock = new object();

    public static BluetoothManager GetInstance()
    {

        lock (_lock)
        {
            if (instance == null)
            {
                // Create a new GameObject to attach the BluetoothManager if one does not exist
                GameObject singletonObject = new GameObject();
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
        Debug.Log(data);
        Debug.Log(data.Split("+")[1]);

        if (data.Split("+")[1] != "null")
        {
            devices.Add(data.Split("+")[1], data.Split("+")[0]);
            if (NewDevice == null)
            {
                Debug.Log("NewDevice not subscribed to");
                return;
            }
            Debug.Log("Should invoke");
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
        Debug.Log("BT Stream: " + data);
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
}
