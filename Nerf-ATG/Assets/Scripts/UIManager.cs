using System;
using System.Collections.Generic;
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

        void Start()
        {
            Debug.Log("UI-Manager Start");
            player = Player.GetInstance();
            bluetooth = BluetoothManager.GetInstance();

            player.WeaponTypeChanged += OnWeaponSelect;
            player.CoinsChanged += OnCoinsChange;
            bluetooth.NewDevice += OnNewDevice;
            bluetooth.ConnectionChanged += OnConnectionChange;
            Debug.Log("UI-Manager Start Finished");

            SceneDepandantManagerStart();
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
                    uiObjects[1].GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
                    uiObjects[0].GetComponent<Text>().text = player.TeamInfo.ToString();
                    break;

                default:
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
