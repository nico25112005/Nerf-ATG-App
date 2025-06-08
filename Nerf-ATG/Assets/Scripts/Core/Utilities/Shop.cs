using System.ComponentModel;
using UnityEngine;

class UnityItemShop : IShop<GameObject>
{

    // Fields for Shop
    readonly GameObject prefab;
    readonly Transform container;
    readonly Rect size;

    byte positionIndex = 0;

    GameObject instance;

    Rect prefabSize;

    // Constructor for ShopItem
    public UnityItemShop(Transform container, GameObject prefab)
    {
        this.size = container.GetComponent<RectTransform>().rect;
        this.container = container;
        this.prefab = prefab;

    }

    // Method to create a new shop item
    public void CreateShopItem(IShopItem<GameObject> item)
    {
        positionIndex = (byte)container.childCount;

        instance = Object.Instantiate(prefab, container);

        prefabSize = instance.GetComponent<RectTransform>().rect;

        RectTransform upgradeRectTransform = instance.GetComponent<RectTransform>();
        float widthSpacing = ((size.width - prefabSize.width * item.ShopItemPerRow) / (item.ShopItemPerRow + 1));
        float heightSpacing = ((size.height - prefabSize.height * item.ShopItemPerColumn) / (item.ShopItemPerColumn + 1));

        // Placement of the shop item based on position in container
        if (positionIndex < item.ShopItemPerRow)
        {
            upgradeRectTransform.anchoredPosition = new Vector2(widthSpacing + (widthSpacing + prefabSize.width) * positionIndex, -(heightSpacing));
        }
        else
        {
            upgradeRectTransform.anchoredPosition = new Vector2(widthSpacing + (widthSpacing + prefabSize.width) * (positionIndex - item.ShopItemPerColumn), -(heightSpacing * 2 + prefabSize.height));
        }

        item.Construct(instance);
    }
}