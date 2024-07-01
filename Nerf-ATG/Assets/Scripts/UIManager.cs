using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Game.Enums;
using Game;
using System.Data;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> uiObjects;
        private Player player;
        private BluetoothManager bluetooth;
        private string connectingDevice;
        
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
            if(isConnected)
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

        void Awake()
        {
            player = Player.GetInstance();
            bluetooth = BluetoothManager.GetInstance();

            player.WeaponTypeChanged += OnWeaponSelect;
            player.CoinsChanged += OnCoinsChange;
            player.UpgradesChanged += OnUpgradeChange;
            bluetooth.NewDevice += OnNewDevice;
            bluetooth.ConnectionChanged += OnConnectionChange;

            SceneDepandantManagerStart();
        }

        private void OnDestroy()
        {
            player.WeaponTypeChanged -= OnWeaponSelect;
            player.CoinsChanged -= OnCoinsChange;
            player.UpgradesChanged -= OnUpgradeChange;
            bluetooth.NewDevice -= OnNewDevice;
            bluetooth.ConnectionChanged -= OnConnectionChange;
        }
        void SceneDepandantManagerStart()
        {
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 0:
                    bluetooth.StartScanDevices();
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
            }
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

        // ---------- Scene 1 ----------

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
    }

}
