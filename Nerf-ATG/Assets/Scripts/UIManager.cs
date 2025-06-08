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

        [System.Serializable]
        public class UIElementEntry
        {
            public string key; // Der Name des Elements
            public GameObject element; // Das zugehörige UI-Objekt
        }

        [SerializeField]
        private List<UIElementEntry> uiElements = new List<UIElementEntry>();

        private Dictionary<string, GameObject> uiDic;

        private void InitializeUIElements()
        {
            uiDic = new Dictionary<string, GameObject>();

            foreach (var entry in uiElements)
            {
                if (!uiDic.ContainsKey(entry.key))
                {
                    uiDic.Add(entry.key, entry.element);
                }
                else
                {
                    Debug.LogWarning($"Duplicate key detected: {entry.key}");
                }
            }
        }

        // UI-Element abrufen
        public GameObject GetUIElement(string key)
        {
            if (uiDic.ContainsKey(key))
            {
                return uiDic[key];
            }
            Debug.LogWarning($"UI Element with key {key} not found.");
            return null;
        }


        private void AssignEvents()
        {
            player.WeaponTypeChanged += OnWeaponSelect;
            player.CoinsChanged += OnCoinsChange;
            player.UpgradesChanged += OnUpgradeChange;
            player.HealthChanged += OnHealthChange;
            player.AmmoChanged += OnAmmoChange;
            player.MaxAmmoChanged += OnMaxAmmoChange;
            player.GpsDataChanged += OnGPSDataChange;
            player.EnemyLocationChanged += OnEnemyLocationChange;
            player.TeamMateLocationChanged += OnTeamMateLocationChange;
        }

        private void UnsignEvents()
        {
            player.WeaponTypeChanged -= OnWeaponSelect;
            player.CoinsChanged -= OnCoinsChange;
            player.UpgradesChanged -= OnUpgradeChange;
            player.HealthChanged -= OnHealthChange;
            player.AmmoChanged -= OnAmmoChange;
            player.MaxAmmoChanged -= OnMaxAmmoChange;
            player.GpsDataChanged -= OnGPSDataChange;
            player.EnemyLocationChanged -= OnEnemyLocationChange;
            player.TeamMateLocationChanged -= OnTeamMateLocationChange;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------//
        //---------------------------------------------------------------------------------------------------------------------------------------------//
        //---------------------------------------------------------------------------------------------------------------------------------------------//

        private Player player;
        private string connectingDevice;
        //private TCPClient tcp;

        // ---- Events ---- //
        void OnHealthChange(object sender, EventArgs e)
        {
            GetUIElement("HealthBar").transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Health: " + player.Health.ToString();
            GetUIElement("HealthBar").transform.Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
               new Vector2((GetUIElement("HealthBar").GetComponent<RectTransform>().rect.width - 20) / Settings.Health * player.Health,
               GetUIElement("HealthBar").transform.Find("Progressbar").GetComponent<RectTransform>().rect.height);
        }

        void OnAmmoChange(object sender, EventArgs e)
        {
            GetUIElement("AmmoBar").transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Ammo: " + player.Ammo.ToString();
            GetUIElement("AmmoBar").transform.Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
               new Vector2((GetUIElement("AmmoBar").GetComponent<RectTransform>().rect.width - 20) / Settings.weaponInfo[player.WeaponType].AmmoPerMag * player.Ammo,
               GetUIElement("AmmoBar").transform.Find("Progressbar").GetComponent<RectTransform>().rect.height);
        }

        void OnMaxAmmoChange(object sender, EventArgs e)
        {
            GetUIElement("MaxAmmoBar").transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Left Ammo: " + player.MaxAmmo.ToString();
            GetUIElement("MaxAmmoBar").transform.Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
               new Vector2((GetUIElement("MaxAmmoBar").GetComponent<RectTransform>().rect.width - 20) / Settings.weaponInfo[player.WeaponType].MaxAmmo * player.MaxAmmo,
               GetUIElement("MaxAmmoBar").transform.Find("Progressbar").GetComponent<RectTransform>().rect.height);
        }

        void OnWeaponSelect(object sender, EventArgs e)
        {
            //GetUIElement("WeaponIcon").GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
        }

        void OnCoinsChange(object sender, EventArgs e)
        {
            GetUIElement("Coins").GetComponent<Text>().text = player.Coins.ToString() + " C";
        }

        void OnConnectionChange(object sender, bool isConnected)
        {
            if (isConnected)
            {
                GetUIElement("ConnectToServer").GetComponent<Button>().interactable = true;
                GetUIElement("StartButtonText").GetComponent<Text>().color = Color.white;
                //tcp.SendMessage(new BlasterConnected(player.BlasterMacAdress));
            }
            else
            {
                GetUIElement("ConnectToServer").GetComponent<Button>().interactable = false;
                GetUIElement("StartButtonText").GetComponent<Text>().color = Color.gray;
            }
        }

        void OnUpgradeChange(object sender, EventArgs e)
        {
            string result = string.Format("Health: {0}\n", Settings.Health + (byte)(player.Upgrades[UpgradeType.Health] * 15));
            result += string.Format("{0}: {1} hp/s\n", UpgradeType.Healing, Settings.Healing + (byte)(player.Upgrades[UpgradeType.Healing] * 2));
            result += string.Format("{0}: {1} m\n", UpgradeType.GpsShift, player.Upgrades[UpgradeType.GpsShift] * 5);
            result += string.Format("Sound: {0} %", 100 - player.Upgrades[UpgradeType.Damping] * 50);
            GetUIElement("Stats").GetComponent<Text>().text = result;
        }

        public void OnNewDevice(object sender, string deviceName)
        {
            GameObject prefabInstance = Instantiate(GetUIElement("DevicePrefab"), GetUIElement("DeviceList").transform);
            prefabInstance.transform.Find("DeviceName").GetComponent<Text>().text = deviceName;
            prefabInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClick(prefabInstance.GetComponentInChildren<Text>()));
            deviceName = null;

            void ButtonClick(Text text)
            {
                connectingDevice = text.text;
                GetUIElement("ConnectionDevice").GetComponent<Text>().text = connectingDevice;
            }
        }

        public void OnGPSDataChange(object sender, EventArgs e)
        {
            StartCoroutine(LoadTileAndPlaceMarker());
        }

        void OnEnemyLocationChange(object sender, GPSData e)
        {
            MainThreadDispatcher.Execute(() => PlaceMarkerOnTile(GetUIElement("EnemyPin"), e.Latitude, e.Longitude));
        }

        void OnTeamMateLocationChange(object sender, GPSData e)
        {
            MainThreadDispatcher.Execute(() => PlaceMarkerOnTile(GetUIElement("TeamMatePin"), e.Latitude, e.Longitude));
        }

        // ----Initialization ---- //
        void Start()
        {
            //tcp = TCPClient.GetInstance();
            InitializeUIElements();

            player = Player.GetInstance();

            AssignEvents();

            SceneDepandantManagerStart();
        }
        private void OnDestroy()
        {
            StopCoroutine(InitGPS());
            UnsignEvents();

        }
        void SceneDepandantManagerStart()
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Menu":
                    //bluetooth.StartScanDevices();
                    break;

                case "Weapons":
                    //GetUIElement("WeaponIcon").GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
                    break;
                case "Upgrades":
                    GetUIElement("Coins").GetComponent<Text>().text = player.Coins.ToString() + " C";
                    //GetUIElement("WeaponIcon").GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
                    GetUIElement("TeamInfo").GetComponent<Text>().text = player.TeamInfo.ToString();
                    OnUpgradeChange(new object(), EventArgs.Empty);
                    break;
                case "Game":

                    player.MaxAmmo = Settings.weaponInfo[player.WeaponType].MaxAmmo;
                    player.Ammo = Settings.weaponInfo[player.WeaponType].AmmoPerMag;

                    //GetUIElement("WeaponIcon").GetComponent<Image>().sprite = GameAssets.Instance.weapons[player.WeaponType];
                    GetUIElement("TeamInfo").GetComponent<Text>().text = player.TeamInfo.ToString();

                    OnUpgradeChange(new object(), EventArgs.Empty);
                    OnHealthChange(new object(), EventArgs.Empty);

                    StartCoroutine(InitGPS());

                    break;
            }
        }

        // Buttons //

        public void Connect()
        {
            
        }

        public void Scan()
        {
            foreach (Transform child in GetUIElement("DeviceList").transform)
            {
                Destroy(child.gameObject);
            }
            GetUIElement("ConnectionDevice").GetComponent<Text>().text = "";
            
        }

        public void SetBaseLocation()
        {
            player.BaseLocation = player.GPSData;
            GetUIElement("HomeLocation").SetActive(false);
            PlaceMarkerOnTile(GetUIElement("HomePin"), player.BaseLocation.Latitude, player.BaseLocation.Longitude);
        }
        
        public void aktivateAbility()
        {
            Timer colldownTimer = new Timer(120000);
            colldownTimer.Elapsed += (sender, e) =>
            {
                MainThreadDispatcher.Execute(() => GetUIElement("AbilityBackground").GetComponent<Button>().interactable = true);
                colldownTimer.Stop();
                colldownTimer.Dispose();
            };
            colldownTimer.AutoReset = false;
            colldownTimer.Start();
            GetUIElement("AbilityBackground").GetComponent<Button>().interactable = false;
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
                // Todo: info needed
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                // Todo: info needed
                yield break;
            }
            else
            {
                GetUIElement("SetBaseButton").SetActive(true);
                //Acces granted
                // Todo: info needed
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
            if (GetUIElement("Map").transform.childCount == 1) //Marker container exists already
            {
                float tileSize = GetUIElement("Map").GetComponent<RectTransform>().rect.width;
                for (short x = -1; x <= 1; x++)
                {
                    for (short y = -1; y <= 1; y++)
                    {
                        if (y != 0 || x != 0)
                        {
                            maptiles[x + 1, y + 1] = Instantiate(GetUIElement("TilePrefab"), GetUIElement("Map").transform);
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
                                GetUIElement("Map").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            }
                            else
                            {
                                maptiles[x + 1, y + 1].GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                            }
                        }

                    }
                }

                foreach(Transform marker in GetUIElement("Map").transform.Find("Markers").transform)
                {
                    Destroy(marker.gameObject);
                }

                //Spawn base marker
                if(player.BaseLocation != null)
                    PlaceMarkerOnTile(GetUIElement("HomePin"), player.BaseLocation.Latitude, player.BaseLocation.Longitude);

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

            GetUIElement("Map").GetComponent<RectTransform>().localPosition = -GPSToTilePosition(latitude, longitude);
        }


        void PlaceMarkerOnTile(GameObject prefab, double latitude, double longitude)
        {
            Vector2 tilePosition = GPSToTilePosition(latitude, longitude);
            GameObject marker = Instantiate(prefab, new Vector3(tilePosition.x, tilePosition.y, 0), Quaternion.identity);
            marker.transform.SetParent(GetUIElement("Map").transform.Find("Markers").transform, false);
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
            float x = (float)(((longitude + 180.0) / 360.0 * n - (currentTileCoords.x + 0.5)) * GetUIElement("Map").GetComponent<RectTransform>().rect.width);
            float y = (float)(((1.0 - Mathf.Log((float)(Mathf.Tan(latRad) + 1.0 / Mathf.Cos(latRad))) / Mathf.PI) / 2.0 * n - (currentTileCoords.y + 0.5)) * GetUIElement("Map").GetComponent<RectTransform>().rect.height * -1);

            return new Vector2(x, y);
        }

    }

}
