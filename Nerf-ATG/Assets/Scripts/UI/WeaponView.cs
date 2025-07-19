using Game;
using Game.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;



public class WeaponView : MonoBehaviour, IWeaponView, IWeaponViewUnityExtension
{
    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private WeaponPresenter presenter;


    [Inject]
    private IPlayerModel playerModel;

    [Inject]
    private ITcpClientService tcpClientService;


    private IShop<GameObject> shop;


    void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        presenter = new WeaponPresenter(this, playerModel, tcpClientService);

        this.shop = new UnityItemShop(registry.GetElement("Container").transform, registry.GetElement("WeaponPrefab"));

        foreach (WeaponInfo weaponinfo in Settings.weaponInfo.Values)
        {
             shop.CreateShopItem(new WeaponShopItem(weaponinfo, presenter.BuyWeapon));
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

    //Buttons

    public void Next()
    {
        presenter.NextScene();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        presenter.Quit();
    }

    public void SetNextSceneActive(bool value)
    {
        registry.GetElement("NextButton").GetComponent<Button>().interactable = value;
    }

    private void OnDestroy()
    {
        presenter.Dispose();
    }
}
