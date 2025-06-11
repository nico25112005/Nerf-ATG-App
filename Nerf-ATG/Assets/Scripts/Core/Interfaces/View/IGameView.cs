using Game.Enums;

public interface IGameView
{
    void UpdateHealthBar(byte health);
    void UpdateAmmoBar(WeaponType weaponType, byte ammo);
    void UpdateMaxAmmoBar(WeaponType weaponType, ushort maxAmmo);
}