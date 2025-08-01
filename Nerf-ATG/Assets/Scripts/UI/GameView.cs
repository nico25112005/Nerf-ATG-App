using Game;
using Game.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameView : MonoBehaviour, IGameView, IGPSMap, IGameViewUnityExtension
{
    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private GamePresenter gamePresenter;
    private GpsPresenter gpsPresenter;

    [Inject]
    private IPlayerModel playerModel;

    [Inject]
    private IGameModel gameModel;

    [Inject]
    private ITcpClientService tcpClientService;

    [Inject]
    private IMainThreadExecutor mainThreadExecutor;

    private IGpsTileService gpsTileService;
    private IGpsDataService gpsDataService;

    private GameObject[,] maptiles = new GameObject[3, 3];

    private void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        // Beispiel: automatisches Erzeugen, falls nicht vorhanden
        if (GpsDataService.Instance == null)
        {
            new GameObject("GpsDataService").AddComponent<GpsDataService>();
        }

        gpsDataService = GpsDataService.Instance;

        if (GpsTileService.Instance == null)
        {
            new GameObject("GpsTileService").AddComponent<GpsTileService>();
        }

        gpsTileService = GpsTileService.Instance;

        gpsTileService.SetMapSize(new System.Numerics.Vector2(registry.GetElement("Map").GetComponent<RectTransform>().rect.width, registry.GetElement("Map").GetComponent<RectTransform>().rect.height));

        gamePresenter = new GamePresenter(this, playerModel, gameModel, tcpClientService, mainThreadExecutor);

        gpsPresenter = new GpsPresenter(this, playerModel, gameModel, gpsTileService, gpsDataService, tcpClientService, mainThreadExecutor);

        InitMapPrefabs();
    }

    //View
    public void UpdateAmmoBar(WeaponType weaponType, byte ammo)
    {
        GameObject ammoBar = registry.GetElement("AmmoBar");
        RectTransform progressBarRect = ammoBar.transform.Find("Progressbar").GetComponent<RectTransform>();

        ammoBar.transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Ammo: " + ammo.ToString();
        progressBarRect.sizeDelta =
            new Vector2((ammoBar.GetComponent<RectTransform>().rect.width - 20) / Settings.weaponInfo[weaponType].AmmoPerMag * ammo, progressBarRect.rect.height);
    }

    public void UpdateHealthBar(byte health, byte maxHealth)
    {
        GameObject healthBar = registry.GetElement("HealthBar");
        RectTransform progressBarRect = healthBar.transform.Find("Progressbar").GetComponent<RectTransform>();

        healthBar.transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Health: " + health.ToString();
        progressBarRect.sizeDelta =
            new Vector2((healthBar.GetComponent<RectTransform>().rect.width - 20) / maxHealth * health, progressBarRect.rect.height);
    }

    public void UpdateMaxAmmoBar(WeaponType weaponType, ushort maxAmmo)
    {
        GameObject maxAmmoBar = registry.GetElement("MaxAmmoBar");
        RectTransform progressBarRect = maxAmmoBar.transform.Find("Progressbar").GetComponent<RectTransform>();

        maxAmmoBar.transform.Find("TextBackground").Find("Text").GetComponent<Text>().text = "Left Ammo: " + maxAmmo.ToString();
        progressBarRect.sizeDelta =
           new Vector2((maxAmmoBar.GetComponent<RectTransform>().rect.width - 20) / Settings.weaponInfo[weaponType].MaxAmmo * maxAmmo, progressBarRect.rect.height);
    }

    public void UpdateAbilityIcon(Abilitys ability, double cooldown)
    {
        var abilityImage = registry.GetElement("Ability").GetComponent<Image>();
        var abilityButton = registry.GetElement("AbilityButton").GetComponent<Button>();
        var reloadImage = registry.GetElement("AbilityReload").GetComponent<Image>();
        var reloadTime = registry.GetElement("AbilityReloadTime").GetComponent<Text>();

        // Update sprite only if necessary
        Sprite newSprite = GameAssets.Instance.abilitys[ability];
        if (abilityImage.sprite != newSprite)
        {
            abilityImage.sprite = newSprite;
            reloadImage.sprite = newSprite;
        }

        // Update button interactability
        bool shouldBeInteractable = cooldown == 1f;
        if (abilityButton.interactable != shouldBeInteractable)
        {
            abilityButton.interactable = shouldBeInteractable;
            reloadTime.gameObject.SetActive(!shouldBeInteractable);
        }

        reloadTime.text = $"{Settings.abilityInfo[ability].Cooldown * (1 - cooldown):F0}s";
        // Update cooldown fill
        reloadImage.fillAmount = (float)cooldown;
    }
    public void ShowToastMessage(string message, string icon)
    {
        ToastNotification.Show(message, icon);
    }

    //GpsMap

    public void InitMapPrefabs()
    {
        if (registry.GetElement("Map").transform.childCount == 1) //Marker container exists already
        {
            float tileSize = registry.GetElement("Map").GetComponent<RectTransform>().rect.width;
            for (short x = -1; x <= 1; x++)
            {
                for (short y = -1; y <= 1; y++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        maptiles[x + 1, y + 1] = Instantiate(registry.GetElement("TilePrefab"), registry.GetElement("Map").transform);
                        maptiles[x + 1, y + 1].name = "Tile_" + x + "_" + y;
                        maptiles[x + 1, y + 1].transform.localPosition = new Vector3(x * tileSize, -y * tileSize, 0);
                        maptiles[x + 1, y + 1].transform.SetAsFirstSibling();
                    }
                }
            }
        }
    }

    public void UpdateTile(sbyte x, sbyte y, byte[] TileData)
    {
        Texture2D texture = new Texture2D(256, 256);
        texture.LoadImage(TileData);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        if (x == 0 && y == 0)
        {
            registry.GetElement("Map").GetComponent<Image>().sprite = sprite;
        }
        else
        {
            maptiles[x + 1, y + 1].GetComponent<Image>().sprite = sprite;
        }
    }

    public void UpdateMapLocation(System.Numerics.Vector2 MapOffset)
    {
        registry.GetElement("Map").GetComponent<RectTransform>().localPosition = -(new Vector2(MapOffset.X, MapOffset.Y));
    }

    public void UpdateMapPoints(MapPointType mapPointType, IMapPoint mapPoint, System.Numerics.Vector2 markerOffset)
    {
        GameObject markerPrefab = mapPointType switch
        {
            MapPointType.Enemy => registry.GetElement("EnemyMarkerPrefab"),
            MapPointType.Allie => registry.GetElement("AllieMarkerPrefab"),
            MapPointType.Base => registry.GetElement("SpawnMarkerPrefab"),
            _ => throw new ArgumentOutOfRangeException(nameof(mapPointType), mapPointType, null)
        };

        switch (mapPoint.Action)
        {
            case PacketAction.Add:
                AddMapPoint(mapPoint, markerPrefab, markerOffset);
                break;

            case PacketAction.Remove:
                RemoveMapPoint(mapPoint.Name);
                break;

            case PacketAction.Update:
                RemoveMapPoint(mapPoint.Name);
                AddMapPoint(mapPoint, markerPrefab, markerOffset);
                break;
        }

    }

    private void AddMapPoint(IMapPoint mapPoint, GameObject markerPrefab, System.Numerics.Vector2 markerOffset)
    {
        GameObject marker = Instantiate(markerPrefab, registry.GetElement("Map").transform.Find("Markers"));
        marker.name = mapPoint.Name;
        marker.GetComponent<RectTransform>().localPosition = new Vector2(markerOffset.X, markerOffset.Y);

        if (mapPoint is PlayerStatus player)
        {
            marker.transform.Find("Health").GetComponent<Image>().fillAmount = player.Health / 100f;
            marker.transform.Find("Name").GetComponent<Text>().text = player.Name;
        }
    }

    private void RemoveMapPoint(string name)
    {
        Destroy(registry.GetElement("Map").transform.Find("Markers").Find(name).gameObject);
    }


    //Extension
    public void UpdateTeam(Team team)
    {
        registry.GetElement("Team").GetComponent<Text>().text = team.ToString();
    }

    public void UpdateWeaponIcon(WeaponType weaponType)
    {
        registry.GetElement("Weapon").GetComponent<Image>().sprite = GameAssets.Instance.weapons[weaponType];
    }

    public void UpdateUpgradeInfo(IReadOnlyDictionary<UpgradeType, byte> upgrades)
    {
        string result = string.Format("Health: {0}\n", Settings.Health + (byte)(upgrades[UpgradeType.Health] * 15));
        result += string.Format("{0}: {1} hp/s\n", UpgradeType.Healing, Settings.Healing + (byte)(upgrades[UpgradeType.Healing] * 2));
        result += string.Format("{0}: {1} m\n", UpgradeType.GpsShift, upgrades[UpgradeType.GpsShift] * 5);
        result += string.Format("Sound: {0} %", 100 - upgrades[UpgradeType.Damping] * 50);
        registry.GetElement("Stats").GetComponent<Text>().text = result;
    }

    public void DeactivateInformationPanel()
    {
        registry.GetElement("InformationPanel").gameObject.SetActive(false);
    }

    public void SetBaseLocationButtonVisable()
    {
        registry.GetElement("SetBaseLocationButton").gameObject.SetActive(true);
    }

    public void SetBaseInformationText(string text)
    {
        registry.GetElement("SetBaseInformation").GetComponent<Text>().text = text;
    }

    //Buttons

    public void ActivateAbility()
    {
        gamePresenter.ActivateAbility();
    }

    public void SetBaseLocation()
    {
        gpsPresenter.SetBaseLocation();
        registry.GetElement("SetBaseLocationButton").gameObject.SetActive(false);
    }

    public void Quit()
    {
        gamePresenter.Quit();
    }

    private void OnDestroy()
    {
        gamePresenter.Dispose();
        gpsPresenter.Dispose();
    }

}