using Game;
using Game.Enums; // Contains the enum definitions for UpgradeType and WeaponType
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Suppress editor hints for compiler-generated code
namespace Assets.Scripts
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        // Definition of the ShopItem class
        class ShopItem
        {

            // Fields for ShopItem
            readonly byte positionIndex = 0;
            readonly GameObject transform;
            readonly IShopItems itemInfo;
            Rect size;
            Rect prefabSize;

            // Constructor for ShopItem
            public ShopItem(Transform container, GameObject prefab, IShopItems itemInfo)
            {
                positionIndex = (byte)container.childCount;
                this.transform = Instantiate(prefab, container);
                this.itemInfo = itemInfo;
                this.size = container.GetComponent<RectTransform>().rect;
                this.prefabSize = transform.GetComponent<RectTransform>().rect;
                CreateNewShopItem();
            }

            // Method to create a new shop item
            void CreateNewShopItem()
            {
                RectTransform upgradeRectTransform = transform.GetComponent<RectTransform>();
                float widthSpacing = ((size.width - prefabSize.width * itemInfo.ShopItemPerRow) / (itemInfo.ShopItemPerRow + 1));
                float heightSpacing = ((size.height - prefabSize.height * itemInfo.ShopItemPerColumn) / (itemInfo.ShopItemPerColumn + 1));

                // Placement of the shop item based on position in container
                if (positionIndex < itemInfo.ShopItemPerRow)
                {
                    upgradeRectTransform.anchoredPosition = new Vector2(widthSpacing + (widthSpacing + prefabSize.width) * positionIndex, -(heightSpacing));
                }
                else
                {
                    upgradeRectTransform.anchoredPosition = new Vector2(widthSpacing + (widthSpacing + prefabSize.width) * (positionIndex - itemInfo.ShopItemPerColumn), -(heightSpacing * 2 + prefabSize.height));
                }

                itemInfo.Construct(transform);
            }
        }
        readonly Player player = Player.GetInstance();
        private void Start()
        {
            try
            {
                switch (SceneManager.GetActiveScene().name)
                {
                    case "Weapons": InitWeaponScene(); break;
                    case "Upgrades": InitUpgradeScene(); break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {

                Debug.Log(e.StackTrace);
            }
        }

        public void ResetUpgrades()
        {
            player.Coins = (byte)(25 - Settings.weaponInfo[player.WeaponType].Price);
            
            player.SetUpgrades(UpgradeType.Healing, 0);
            player.SetUpgrades(UpgradeType.Health, 0);
            player.SetUpgrades(UpgradeType.GpsShift, 0);
            player.SetUpgrades(UpgradeType.Damping, 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void InitWeaponScene()
        {
            new ShopItem(transform, prefab, Settings.weaponInfo[WeaponType.Sniper]);
            new ShopItem(transform, prefab, Settings.weaponInfo[WeaponType.Mp]);
            new ShopItem(transform, prefab, Settings.weaponInfo[WeaponType.Rifle]);
        }

        public void InitUpgradeScene()
        {
            new ShopItem(transform, prefab, Settings.upgradeInfo[UpgradeType.Health]);
            new ShopItem(transform, prefab, Settings.upgradeInfo[UpgradeType.Healing]);
            new ShopItem(transform, prefab, Settings.upgradeInfo[UpgradeType.GpsShift]);
            new ShopItem(transform, prefab, Settings.upgradeInfo[UpgradeType.Damping]);
        }

    }
}
