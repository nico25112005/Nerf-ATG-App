using Game.Enums;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using Game;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

namespace Assets.Scripts
{
    public interface IShopItems
    {
        byte UpgradesPerRow { get; }
        byte UpgradesPerColumn { get; }
        void Construct(GameObject transform);
    }

    // Definition of a record for upgrade information
    public record UpgradeInfo(UpgradeType Type, byte[] Price, Sprite Image) : IShopItems
    {
        readonly Player player = Player.GetInstance(); // Get player instance

        public byte UpgradesPerRow => 2;

        public byte UpgradesPerColumn => 2;
        public void Construct(GameObject upgradeTransform)
        {
            // Update the price display, image, and add the upgrade button event listener
            upgradeTransform.transform.Find("PriceBackground").Find("Price").GetComponent<Text>().text =
                                (Price.Length > player.Upgrades[Type]) ? Price.GetValue(player.Upgrades[Type]) + " C" : "MAX";
            upgradeTransform.transform.Find("UpgradeImageBackground").Find("UpgradeImage").GetComponent<Image>().sprite = Image;
            upgradeTransform.transform.Find("UpgradeImageBackground").GetComponent<Button>().onClick.AddListener(() => { Buy(upgradeTransform); }); // Set button event

            // Update the progress bar length based on upgrade levels
            upgradeTransform.transform.Find("ProgressbarBackground").Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
                new Vector2((upgradeTransform.GetComponent<RectTransform>().rect.width - 20) / Price.Length * player.Upgrades[Type],
                upgradeTransform.transform.Find("ProgressbarBackground").Find("Progressbar").GetComponent<RectTransform>().rect.height); // Set progress bar to correct length;
        }

        void Buy(GameObject upgradeTransform)
        {
            if (Price.Length == player.Upgrades[Type])
            {
                Debug.Log("maximum upgrades");
                return;
            }
            if (player.Coins - Price[player.Upgrades[Type]] < 0)
            {
                Debug.Log("zu wenig geld");
                return;
            }
            // Player buying the upgrade
            //player.BuyUpgrade(Type, (byte)Price.GetValue(player.Upgrades[Type]), (byte)Price.Length) --> Old

            if (player.Coins - (byte)Price.GetValue(player.Upgrades[Type]) >= 0 && (byte)Price.Length > player.Upgrades[Type])
            {
                //pay
                player.Coins -= (byte)Price.GetValue(player.Upgrades[Type]);
                player.SetUpgrades(Type, (byte)(player.Upgrades[Type] + 1));


                // Update the progress bar and price
                upgradeTransform.transform.Find("ProgressbarBackground").Find("Progressbar").GetComponent<RectTransform>().sizeDelta =
                    new Vector2((upgradeTransform.GetComponent<RectTransform>().rect.width - 20) / Price.Length * player.Upgrades[Type],
                    upgradeTransform.transform.Find("ProgressbarBackground").Find("Progressbar").GetComponent<RectTransform>().rect.height); // Set progress bar to correct length

                upgradeTransform.transform.Find("PriceBackground").Find("Price").GetComponent<Text>().text =
                    (Price.Length > player.Upgrades[Type]) ? Price.GetValue(player.Upgrades[Type]) + " C" : "MAX";
            }
            Debug.Log(player.ToString());
        }
    }



    // Definition of a record for weapon information
    public record WeponInfo(WeaponType Type, byte Price, Sprite Image, byte BulletsPerShot, float TimeAfterBullet , int MaxAmmo, byte AmmoPerMag, Abilitys Abilitys, float ReloadTime, float ShotTime, byte Damage) : IShopItems
    {
        readonly Player player = Player.GetInstance(); // Get player instance
        public byte UpgradesPerRow => 3;

        public byte UpgradesPerColumn => 1;

        public void Construct(GameObject weaponTransform)
        {
            // Update the price display, image, and add the upgrade button event listener
            weaponTransform.transform.Find("PriceBackground").Find("Price").GetComponent<Text>().text = Price.ToString() + " C";
            weaponTransform.transform.Find("UpgradeImageBackground").Find("UpgradeImage").GetComponent<Image>().sprite = Image;
            weaponTransform.transform.Find("AbilityBackground").Find("Abilities").GetComponent<Text>().text =
                               $"• Max Ammo: {MaxAmmo}\n" +
                               $"• Ammo Per Mag: {AmmoPerMag}\n" +
                               $"• Reload Time: {ReloadTime}\n" +
                               $"• Damage: {Damage}\n" +
                               $"• Ability: {Abilitys}\n" +
                               $"• Bullets/Shot: {BulletsPerShot}\n" +
                               $"\n     --- Advanced ---\n" +
                               $"• Time After Bullet: {TimeAfterBullet}\n" +
                               $"• Shot Time: {ShotTime}\n" ;

            weaponTransform.transform.Find("UpgradeImageBackground").GetComponent<Button>().onClick.AddListener(SelectWeapon); // Set button event
        }

        void SelectWeapon()
        {
            player.WeaponType = Type;
            player.Coins = (byte)(Settings.Coins - Settings.weaponInfo[Type].Price);
        }
    }
}
