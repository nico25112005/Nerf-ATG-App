public interface IShopItem<T>
{
    byte ShopItemPerRow { get; }
    byte ShopItemPerColumn { get; }

    void Construct(T prefab);
}