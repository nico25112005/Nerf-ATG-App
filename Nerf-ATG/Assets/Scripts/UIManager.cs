using Game;
using Game.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Timers;


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
            uiObjects[4].transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Health: " + player.Health.ToString();
            uiObjects[4].transform.Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
               new Vector2((uiObjects[4].GetComponent<RectTransform>().rect.width - 20) / Settings.Health * player.Health,
               uiObjects[4].transform.Find("Progressbar").GetComponent<RectTransform>().rect.height);
        }

        void OnAmmoChange(object sender, EventArgs e)
        {
            uiObjects[5].transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Ammo: " + player.Ammo.ToString();
            uiObjects[5].transform.Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
               new Vector2((uiObjects[5].GetComponent<RectTransform>().rect.width - 20) / Settings.weaponInfo[player.WeaponType].AmmoPerMag * player.Ammo,
               uiObjects[5].transform.Find("Progressbar").GetComponent<RectTransform>().rect.height);
        }

        void OnMaxAmmoChange(object sender, EventArgs e)
        {
            uiObjects[6].transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Left Ammo: " + player.MaxAmmo.ToString();
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

        public void OnGPSDataChange(object sender, EventArgs e)
        {
            StartCoroutine(LoadTileAndPlaceMarker());
        }

        void OnEnemyLocationChange(object sender, GPSData e)
        {
            MainThreadDispatcher.Execute(() => PlaceMarkerOnTile(uiObjects[12], e.Latitude, e.Longitude));
        }
        void OnTeamMateLocationChange(object sender, GPSData e)
        {
            MainThreadDispatcher.Execute(() => PlaceMarkerOnTile(uiObjects[13], e.Latitude, e.Longitude));
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
            player.GpsDataChanged += OnGPSDataChange;
            player.EnemyLocationChanged += OnEnemyLocationChange;
            player.TeamMateLocationChanged += OnTeamMateLocationChange;

            bluetooth.NewDevice += OnNewDevice;
            bluetooth.ConnectionChanged += OnConnectionChange;

            SceneDepandantManagerStart();
        }
        private void OnDestroy()
        {
            StopCoroutine(InitGPS());

            player.WeaponTypeChanged -= OnWeaponSelect;
            player.CoinsChanged -= OnCoinsChange;
            player.UpgradesChanged -= OnUpgradeChange;
            player.HealthChanged -= OnHealthChange;
            player.AmmoChanged -= OnAmmoChange;
            player.MaxAmmoChanged -= OnMaxAmmoChange;
            player.GpsDataChanged -= OnGPSDataChange;
            player.EnemyLocationChanged -= OnEnemyLocationChange;
            player.TeamMateLocationChanged -= OnTeamMateLocationChange;

            bluetooth.NewDevice -= OnNewDevice;
            bluetooth.ConnectionChanged -= OnConnectionChange;
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

                    player.MaxAmmo = Settings.weaponInfo[player.WeaponType].MaxAmmo;
                    player.Ammo = Settings.weaponInfo[player.WeaponType].AmmoPerMag;

                    uiObjects[1].GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
                    uiObjects[0].GetComponent<Text>().text = player.TeamInfo.ToString();

                    OnUpgradeChange(new object(), EventArgs.Empty);
                    OnHealthChange(new object(), EventArgs.Empty);

                    StartCoroutine(InitGPS());
                    bluetooth.SendBaseInformations();

                    break;
            }
        }

        // Buttons //
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

        public void SetBaseLocation()
        {
            player.BaseLocation = player.GPSData;
            uiObjects[7].SetActive(false);
            PlaceMarkerOnTile(uiObjects[9], player.BaseLocation.Latitude, player.BaseLocation.Longitude);
        }
        
        public void aktivateAbility()
        {
            Timer colldownTimer = new Timer(120000);
            colldownTimer.Elapsed += (sender, e) =>
            {
                MainThreadDispatcher.Execute(() => uiObjects[14].GetComponent<Button>().interactable = true);
                colldownTimer.Stop();
                colldownTimer.Dispose();
            };
            colldownTimer.AutoReset = false;
            colldownTimer.Start();
            uiObjects[14].GetComponent<Button>().interactable = false;
            player.AbilityActivated = true;
        }
        
        //----- GPS Location ----- //
        IEnumerator InitGPS()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitForSeconds(1); // Geben Sie dem Benutzer Zeit, auf das Dialogfeld zu reagieren
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                yield break;
            }

            Input.location.Start(0.5f, 0.5f);

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                bluetooth.Toast("Didn't initialize GPS");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                bluetooth.Toast("Unable to determine device location");
                yield break;
            }
            else
            {
                uiObjects[10].SetActive(true);
                //Acces granted
                bluetooth.Toast("GPS initialized");
                InvokeRepeating("UpdateGPS", 0f, 0.25f);
            }
        }

        private void UpdateGPS()
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                try
                {
                    player.SetGPSData(Input.location.lastData.longitude, Input.location.lastData.latitude);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.LogWarning("GPS not running");
            }
        }

        // ----- GPS Visualication ----- //

        int zoom = 18;
        Vector2Int currentTileCoords = Vector2Int.zero;

        GameObject[,] maptiles = new GameObject[3,3];

        IEnumerator LoadTileAndPlaceMarker()
        {
            
            double latitude = player.GPSData.Latitude;
            double longitude = player.GPSData.Longitude;
            if (uiObjects[8].transform.childCount == 1) //Marker container exists already
            {
                float tileSize = uiObjects[8].GetComponent<RectTransform>().rect.width;
                for (short x = -1; x <= 1; x++)
                {
                    for (short y = -1; y <= 1; y++)
                    {
                        if (y != 0 || x != 0)
                        {
                            maptiles[x + 1, y + 1] = Instantiate(uiObjects[11], uiObjects[8].transform);
                            maptiles[x + 1, y + 1].transform.localPosition = new Vector3(x * tileSize, -y * tileSize, 0);
                            maptiles[x + 1, y + 1].transform.SetAsFirstSibling();
                        }
                    }
                }
            }

            Vector2Int newTileCoords = GPSToTile(latitude, longitude, zoom);

            // Prüfen, ob neue Tile-Koordinaten erforderlich sind
            if (currentTileCoords != newTileCoords)
            {
                currentTileCoords = newTileCoords; // Aktualisiere die aktuellen Tile-Koordinaten

                for (short x = -1; x <= 1; x++)
                {
                    for (short y = -1; y <= 1; y++)
                    {
                        string url = $"https://tile.openstreetmap.org/{zoom}/{(currentTileCoords.x + x)}/{(currentTileCoords.y + y)}.png";

                        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
                        yield return www.SendWebRequest();

                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            Debug.Log(www.error);
                        }
                        else
                        {
                            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                            Debug.Log($"Texturewidth: {texture.width} Textureheight: {texture.height}");

                            // Aktualisiere das UI-Element mit dem neuen Tile
                            if (y == 0 && x == 0)
                            {
                                uiObjects[8].GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            }
                            else
                            {
                                maptiles[x + 1, y + 1].GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            }
                        }

                    }
                }

                foreach(Transform marker in uiObjects[8].transform.Find("Markers").transform)
                {
                    Destroy(marker.gameObject);
                }

                //Spawn base marker
                if(player.BaseLocation != null)
                    PlaceMarkerOnTile(uiObjects[9], player.BaseLocation.Latitude, player.BaseLocation.Longitude);

                /*/Spawn enemy and team mate markers
                if(player.EnemyLocations.Count != 0)
                {
                    foreach(GPSData gps in player.EnemyLocations)
                    {
                        PlaceMarkerOnTile(uiObjects[12], gps.Latitude, gps.Longitude);
                    }
                }

                if (player.TeamMateLocations.Count != 0)
                {
                    foreach (GPSData gps in player.TeamMateLocations)
                    {
                        PlaceMarkerOnTile(uiObjects[13], gps.Latitude, gps.Longitude);
                    }
                }
                */
            }

            uiObjects[8].GetComponent<RectTransform>().localPosition = -GPSToTilePosition(latitude, longitude);
        }


        void PlaceMarkerOnTile(GameObject prefab, double latitude, double longitude)
        {
            Vector2 tilePosition = GPSToTilePosition(latitude, longitude);
            GameObject marker = Instantiate(prefab, new Vector3(tilePosition.x, tilePosition.y, 0), Quaternion.identity);
            marker.transform.SetParent(uiObjects[8].transform.Find("Markers").transform, false);
        }

        Vector2Int GPSToTile(double latitude, double longitude, int zoom)
        {
            float latRad = (float)(latitude * Mathf.Deg2Rad);
            int n = 1 << zoom;
            int x = (int)((longitude + 180.0) / 360.0 * n);
            int y = (int)((1.0 - Mathf.Log((float)(Mathf.Tan(latRad) + 1.0 / Mathf.Cos(latRad))) / Mathf.PI) / 2.0 * n);

            return new Vector2Int(x, y);
        }

        Vector2 GPSToTilePosition(double latitude, double longitude)
        {
            float latRad = (float)(latitude * Mathf.Deg2Rad);
            int n = 1 << zoom;
            float x = (float)(((longitude + 180.0) / 360.0 * n - (currentTileCoords.x + 0.5)) * uiObjects[8].GetComponent<RectTransform>().rect.width);
            float y = (float)(((1.0 - Mathf.Log((float)(Mathf.Tan(latRad) + 1.0 / Mathf.Cos(latRad))) / Mathf.PI) / 2.0 * n - (currentTileCoords.y + 0.5)) * uiObjects[8].GetComponent<RectTransform>().rect.height * -1);

            return new Vector2(x, y);
        }

    }

}
