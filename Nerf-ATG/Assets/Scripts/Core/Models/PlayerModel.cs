using Game;
using Game.Enums;
using System;
using System.Collections.Generic;

public class PlayerModel : IPlayerModel
{
    // Events
    public event EventHandler<WeaponType> OnWeaponTypeChanged;
    public event EventHandler<byte> OnCoinsChanged;
    public event EventHandler<Dictionary<UpgradeType, byte>> OnUpgradesChanged;

    private PlayerModel()
    {
        Coins = Settings.Coins;
        WeaponType = WeaponType.None;
        Team = Team.Violet;

        _upgrades = new Dictionary<UpgradeType, byte>
        { 
            { UpgradeType.Health, 0 },
            { UpgradeType.Healing, 0 },
            { UpgradeType.GpsShift, 0 },
            { UpgradeType.Damping, 0 }
        };
    }

    // Properties
    public string Name { get; set; } = "";
    public Guid Id { get;} = Guid.NewGuid();

    public byte Health { get; set; }
    public byte Healing { get; set; }
    public byte Ammo { get; set; }
    public ushort MaxAmmo { get; set; }
    public GPS Location { get; set; }

    public Team Team { get; set; }

    private WeaponType _weaponType;
    public WeaponType WeaponType
    {
        get => _weaponType;
        set
        {
            _weaponType = value;
            OnWeaponTypeChanged?.Invoke(this, _weaponType);
        }
    }


    private byte _coins;
    public byte Coins {
        get => _coins;
        set
        {
            _coins = value;
            OnCoinsChanged?.Invoke(this, _coins);
        }
    
    }

    Dictionary<UpgradeType, byte> _upgrades;
    public IReadOnlyDictionary<UpgradeType, byte> Upgrades => _upgrades;

    public void SetUpgrades(UpgradeType upgradeType, byte amount)
    {
        _upgrades[upgradeType] = amount;
        OnUpgradesChanged?.Invoke(this, _upgrades);
    }

    public void AddUpgrade(UpgradeType upgradeType)
    {
        _upgrades[upgradeType]++;
        OnUpgradesChanged?.Invoke(this, _upgrades);
    }
}
