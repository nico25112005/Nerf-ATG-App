using Game;
using Game.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;



public class WeaponView : MonoBehaviour, IWeaponView
{
    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private WeaponPresenter presenter;


    [Inject]
    private IPlayerModel playerModel;

    private IShop<GameObject> shop;


    void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        presenter = new WeaponPresenter(this, playerModel);

        this.shop = new UnityItemShop(registry.GetElement("Container").transform, registry.GetElement("WeaponPrefab"));

        foreach (WeaponInfo weaponinfo in Settings.weaponInfo.Values)
        {
            Debug.Log(weaponinfo);
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

    public void UpdateWeapon(WeaponType weaponType)
    {
        registry.GetElement("Weapon").GetComponent<Image>().sprite = GameAssets.Instance.weapons[weaponType];
    }

    //Buttons

    public void Next()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        //presenter.Quit();
    }

}
