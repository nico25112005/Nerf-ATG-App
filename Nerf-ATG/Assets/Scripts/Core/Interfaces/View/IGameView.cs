using Game.Enums;

public interface IGameView
{
    void UpdateHealthBar(byte health, byte maxHealth);
    void UpdateAmmoBar(WeaponType weaponType, byte ammo);
    void UpdateMaxAmmoBar(WeaponType weaponType, ushort maxAmmo);
    void UpdateAbilityIcon(Abilitys ability, double cooldown);
}