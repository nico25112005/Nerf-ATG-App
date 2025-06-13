using Game.Enums;
using System;
using UnityEngine;
using UnityEngine.UI;

// Definition of a record for upgrade information
public class UpgradeShopItem : IShopItem<GameObject>
{
    UpgradeInfo upgradeInfo;
    Action<UpgradeType> buyUpgrade;

    public UpgradeShopItem(UpgradeInfo upgradeInfo, Action<UpgradeType> buyUpgrade)
    {
        this.upgradeInfo = upgradeInfo;
        this.buyUpgrade = buyUpgrade;
    }
    public byte ShopItemPerRow => 2;

    public byte ShopItemPerColumn => 2;

    public void Construct(GameObject instance)
    {
        instance.name = upgradeInfo.Type.ToString();
        // Update the price display, image, and add the upgrade button event listener
        instance.transform.Find("PriceBackground").Find("Price").GetComponent<Text>().text = upgradeInfo.Price.GetValue(0).ToString() + " C";
        instance.transform.Find("UpgradeImageBackground").Find("UpgradeImage").GetComponent<Image>().sprite = GameAssets.Instance.upgrades[upgradeInfo.Type];
        instance.transform.Find("UpgradeImageBackground").GetComponent<Button>().onClick.AddListener(() => buyUpgrade(upgradeInfo.Type)); // Set button event

        // Update the progress bar length based on upgrade levels
        instance.transform.Find("ProgressbarBackground").Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
            new Vector2(0, instance.transform.Find("ProgressbarBackground").Find("Progressbar").GetComponent<RectTransform>().rect.height); // Set progress bar to 0;

    }
}


// Definition of a record for weapon information
public class WeaponShopItem : IShopItem<GameObject>
{
    private readonly WeaponInfo weaponInfo;
    private readonly Action<WeaponType> buyWeapon;

    public WeaponShopItem(WeaponInfo weaponInfo, Action<WeaponType> buyWeapon)
    {
        this.weaponInfo = weaponInfo;
        this.buyWeapon = buyWeapon;
    }
    public byte ShopItemPerRow => 3;

    public byte ShopItemPerColumn => 1;

    public void Construct(GameObject instance)
    {
        // Update the price display, image, and add the upgrade button event listener
        instance.transform.Find("PriceBackground").Find("Price").GetComponent<Text>().text = weaponInfo.Price.ToString() + " C";
        instance.transform.Find("UpgradeImageBackground").Find("UpgradeImage").GetComponent<Image>().sprite = GameAssets.Instance.weapons[weaponInfo.Type];
        instance.transform.Find("AbilityBackground").Find("Abilities").GetComponent<Text>().text =
                            $"• Max Ammo: {weaponInfo.MaxAmmo}\n" +
                            $"• Ammo Per Mag: {weaponInfo.AmmoPerMag}\n" +
                            $"• Reload Time: {weaponInfo.ReloadTime}\n" +
                            $"• Damage: {weaponInfo.Damage}\n" +
                            $"• Ability: {weaponInfo.Ability}\n" +
                            $"• Bullets/Shot: {weaponInfo.BulletsPerShot}\n" +
                            $"\n     --- Advanced ---\n" +
                            $"• Time After Bullet: {weaponInfo.TimeAfterBullet}\n" +
                            $"• Shot Time: {weaponInfo.ShotTime}\n";

        instance.transform.Find("UpgradeImageBackground").GetComponent<Button>().onClick.AddListener(() => buyWeapon(weaponInfo.Type)); // Set button event
    }
}

