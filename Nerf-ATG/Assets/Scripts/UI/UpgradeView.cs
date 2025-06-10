using Game;
using Game.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using Zenject.SpaceFighter;



public class UpgradeView : MonoBehaviour, IUpgradeView, IUpgradeViewUnityExtension
{
    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private UpgradePresenter presenter;

    [Inject]
    private IPlayerModel playerModel;

    private IShop<GameObject> shop;


    private EventHandler<Dictionary<UpgradeType, byte>> upgradesChanged;

    void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        presenter = new UpgradePresenter(this, playerModel);

        shop = new UnityItemShop(registry.GetElement("Container").transform, registry.GetElement("UpgradePrefab"));

        foreach (UpgradeInfo upgradeinfo in Settings.upgradeInfo.Values)
        {
             shop.CreateShopItem(new UpgradeShopItem(upgradeinfo, presenter.BuyUpgrade));
        }
    }

    public void UpdateTeam(Team team)
    {
        registry.GetElement("Team").GetComponent<Text>().text = team.ToString();
    }

    public void UpdateCoins(byte coins)
    {
        registry.GetElement("Coins").GetComponent<Text>().text = coins.ToString() + "C";
    }

    public void UpdateWeaponIcon(WeaponType weaponType)
    {
        registry.GetElement("Weapon").GetComponent<Image>().sprite = GameAssets.Instance.weapons[weaponType];
    }

    public void UpdateUpgrades(Dictionary<UpgradeType, byte> upgrades)
    {
        string result = string.Format("Health: {0}\n", Settings.Health + (byte)(upgrades[UpgradeType.Health] * 15));
        result += string.Format("{0}: {1} hp/s\n", UpgradeType.Healing, Settings.Healing + (byte)(upgrades[UpgradeType.Healing] * 2));
        result += string.Format("{0}: {1} m\n", UpgradeType.GpsShift, upgrades[UpgradeType.GpsShift] * 5);
        result += string.Format("Sound: {0} %", 100 - upgrades[UpgradeType.Damping] * 50);
        registry.GetElement("Stats").GetComponent<Text>().text = result;

        UpdatePrefab(upgrades);
    }

    private void UpdatePrefab(Dictionary<UpgradeType, byte> upgrades)
    {
        GameObject instance;
        byte[] Price;
        foreach(var upgrade in upgrades)
        {
            instance = registry.GetElement("Container").transform.Find(upgrade.Key.ToString()).gameObject;
            Price = Settings.upgradeInfo[upgrade.Key].Price;

            instance.transform.Find("ProgressbarBackground").Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
            new Vector2((instance.GetComponent<RectTransform>().rect.width - 20) / Price.Length * upgrade.Value,
                instance.transform.Find("ProgressbarBackground").Find("Progressbar").GetComponent<RectTransform>().rect.height);


            instance.transform.Find("PriceBackground").Find("Price").GetComponent<Text>().text =
                            (Price.Length > upgrade.Value) ? Price.GetValue(upgrade.Value) + " C" : "MAX";

            instance.transform.Find("UpgradeImageBackground").GetComponent<Button>().interactable = (Price.Length != upgrade.Value);
        }
    }

    //Buttons

    public void resetUpgrades()
    {
        presenter.ResetUpgrades();
    }
    public void Next()
    {
        presenter.NextScene();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        presenter.Quit();
    }

    private void OnDestroy()
    {
        presenter.Dispose();
    }
}
