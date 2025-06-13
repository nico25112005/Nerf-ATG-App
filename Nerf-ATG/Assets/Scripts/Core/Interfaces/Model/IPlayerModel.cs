using Game.Enums;
using System;
using System.Collections.Generic;

public interface IPlayerModel
{
    event EventHandler<WeaponType> OnWeaponTypeChanged;
    event EventHandler<byte> OnCoinsChanged;
    event EventHandler<Dictionary<UpgradeType, byte>> OnUpgradesChanged;

    event EventHandler<byte> OnHealthChanged;
    event EventHandler<byte> OnAmmoChanged;
    event EventHandler<ushort> OnMaxAmmoChanged;

    event EventHandler<GPS> OnLocationChanged;
    event EventHandler<Abilitys> AbilityActivated;

    string Name { get; set; }
    Guid Id { get; }
    WeaponType WeaponType { get; set; }
    byte Coins { get; set; }
    Team Team { get; set; }

    IReadOnlyDictionary<UpgradeType, byte> Upgrades { get; }
    void SetUpgrades(UpgradeType upgradeType, byte amount);
    void AddUpgrade(UpgradeType upgradeType);

    byte Health { get; set; }
    byte Healing { get; set; }
    byte Ammo { get; set; }
    ushort MaxAmmo { get; set; }

    GPS Location { get; set; }


    void ActivateAbility(Abilitys ability);
}
