using Game;
using Game.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> uiObjects;
        private Player player;
        private BluetoothManager bluetooth;
        private string connectingDevice;

        // ---- Events ---- //
        void OnHealthChange(object sender, EventArgs e)
        {
            uiObjects[4].transform.Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
               new Vector2((uiObjects[4].GetComponent<RectTransform>().rect.width - 20) / Settings.Health * player.Health,
               uiObjects[4].transform.Find("Progressbar").GetComponent<RectTransform>().rect.height);
        }

        void OnAmmoChange(object sender, EventArgs e)
        {
            uiObjects[5].transform.Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
               new Vector2((uiObjects[5].GetComponent<RectTransform>().rect.width - 20) / Settings.weaponInfo[player.WeaponType].AmmoPerMag * player.Ammo,
               uiObjects[5].transform.Find("Progressbar").GetComponent<RectTransform>().rect.height);
        }

        void OnMaxAmmoChange(object sender, EventArgs e)
        {
            uiObjects[6].transform.Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
               new Vector2((uiObjects[6].GetComponent<RectTransform>().rect.width - 20) / Settings.weaponInfo[player.WeaponType].MaxAmmo * player.MaxAmmo,
               uiObjects[6].transform.Find("Progressbar").GetComponent<RectTransform>().rect.height);
        }

        void OnWeaponSelect(object sender, EventArgs e)
        {
            uiObjects[1].GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
        }

        void OnCoinsChange(object sender, EventArgs e)
        {
            uiObjects[2].GetComponent<Text>().text = player.Coins.ToString() + " C";
        }

        void OnConnectionChange(object sender, bool isConnected)
        {
            if (isConnected)
            {
                uiObjects[0].GetComponent<Button>().interactable = true;
                uiObjects[1].GetComponent<Text>().color = Color.white;
            }
            else
            {
                uiObjects[0].GetComponent<Button>().interactable = false;
                uiObjects[1].GetComponent<Text>().color = Color.gray;
            }
        }

        void OnUpgradeChange(object sender, EventArgs e)
        {
            string result = string.Format("Health: {0}\n", player.Health);
            result += string.Format("{0}: {1} hp/s\n", UpgradeType.Healing, player.Upgrades[UpgradeType.Healing] * 2 + Settings.Healing);
            result += string.Format("{0}: {1} m\n", UpgradeType.GpsShift, player.Upgrades[UpgradeType.GpsShift] * 5);
            result += string.Format("Sound: {0} %", 100 - player.Upgrades[UpgradeType.Damping] * 50);
            uiObjects[3].GetComponent<Text>().text = result;
        }

        public void OnNewDevice(object sender, string deviceName)
        {
            Debug.Log("EventTest: " + deviceName);
            GameObject prefabInstance = Instantiate(uiObjects[2], uiObjects[3].transform);
            prefabInstance.transform.Find("DeviceName").GetComponent<Text>().text = deviceName;
            prefabInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClick(prefabInstance.GetComponentInChildren<Text>()));
            deviceName = null;

            void ButtonClick(Text text)
            {
                connectingDevice = text.text;
                uiObjects[4].GetComponent<Text>().text = connectingDevice;
            }
        }

        // ----Initialization ---- //
        void Start()
        {
            player = Player.GetInstance();
            bluetooth = BluetoothManager.GetInstance();

            player.WeaponTypeChanged += OnWeaponSelect;
            player.CoinsChanged += OnCoinsChange;
            player.UpgradesChanged += OnUpgradeChange;
            player.HealthChanged += OnHealthChange;
            player.AmmoChanged += OnAmmoChange;
            player.MaxAmmoChanged += OnMaxAmmoChange;

            bluetooth.NewDevice += OnNewDevice;
            bluetooth.ConnectionChanged += OnConnectionChange;
            bluetooth.NewData += OnDataReceived;

            SceneDepandantManagerStart();
        }
        private void OnDestroy()
        {
            player.WeaponTypeChanged -= OnWeaponSelect;
            player.CoinsChanged -= OnCoinsChange;
            player.UpgradesChanged -= OnUpgradeChange;
            player.HealthChanged -= OnHealthChange;
            player.AmmoChanged -= OnAmmoChange;
            player.MaxAmmoChanged -= OnMaxAmmoChange;

            bluetooth.NewDevice -= OnNewDevice;
            bluetooth.ConnectionChanged -= OnConnectionChange;
            bluetooth.NewData -= OnDataReceived;
        }
        void SceneDepandantManagerStart()
        {
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 0:
                    //bluetooth.StartScanDevices();
                    break;

                case 1:
                    uiObjects[1].GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
                    break;
                case 2:
                    uiObjects[2].GetComponent<Text>().text = player.Coins.ToString() + " C";
                    uiObjects[1].GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
                    uiObjects[0].GetComponent<Text>().text = player.TeamInfo.ToString();
                    OnUpgradeChange(new object(), EventArgs.Empty);
                    break;
                case 3:
                    Debug.Log(player.WeaponType);

                    player.MaxAmmo = Settings.weaponInfo[player.WeaponType].MaxAmmo;
                    player.Ammo = Settings.weaponInfo[player.WeaponType].AmmoPerMag;

                    uiObjects[1].GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
                    uiObjects[0].GetComponent<Text>().text = player.TeamInfo.ToString();

                    OnUpgradeChange(new object(), EventArgs.Empty);
                    OnHealthChange(new object(), EventArgs.Empty);

                    break;
            }
        }

        // ---------- Scene 1 ---------- //
        public void Connect()
        {
            Debug.Log(connectingDevice);
            if (uiObjects[4].GetComponent<Text>().text != "")
            {
                if (BluetoothManager.devices.ContainsKey(connectingDevice))
                    bluetooth.StartConnection(BluetoothManager.devices[connectingDevice]);
            }
        }
        public void Scan()
        {
            foreach (Transform child in uiObjects[3].transform)
            {
                Destroy(child.gameObject);
            }
            uiObjects[4].GetComponent<Text>().text = "";
            connectingDevice = null;
            bluetooth.StartScanDevices();
        }

        // ----- Bluetooth Messages Processing ----- //

        void OnDataReceived(object sender, string data)
        {
            // Processing data in a separate thread
            ThreadPool.QueueUserWorkItem(ProcessData, data);
        }

        void ProcessData(object state)
        {
            BluetoothManager threadBluetooth = BluetoothManager.GetInstance();
            string data = (string)state;
            short hexData = Convert.ToInt16(data, 16);
            Debug.Log($"HexData: {hexData:X}");
            Debug.Log($"HexData: {(hexData & 0XFF00):X}");
            Debug.Log($"HexData: {(hexData >> 8):X}");

            Debug.LogWarning(player.Health);
            if ((hexData & 0xFF00) != 0)
            {
                try
                {
                    Debug.Log(player.Health);
                    MainThreadDispatcher.Execute(() => player.Health -= (byte)(hexData >> 8));
                }
                catch (Exception e)
                {
                    MainThreadDispatcher.Execute(() => threadBluetooth.WriteData("Received Data: " + e.StackTrace));
                    Debug.LogError(e.StackTrace); 
                }
            }
            Debug.LogWarning(player.Health);

            // Use MainThreadDispatcher to call WriteData on the main thread
            MainThreadDispatcher.Execute(() => threadBluetooth.WriteData("Received Data: " + data));        }
    }

}
