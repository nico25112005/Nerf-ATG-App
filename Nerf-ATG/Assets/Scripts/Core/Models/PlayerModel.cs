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

    public event EventHandler<byte> OnHealthChanged;
    public event EventHandler<byte> OnAmmoChanged;
    public event EventHandler<ushort> OnMaxAmmoChanged;
    public event EventHandler<GPS> OnLocationChanged;

    public event EventHandler<Abilitys> AbilityActivated; 

    private PlayerModel()
    {
        Coins = Settings.Coins;
        WeaponType = WeaponType.None;
        Team = Team.Red;

        _upgrades = new Dictionary<UpgradeType, byte>
        { 
            { UpgradeType.Health, 0 },
            { UpgradeType.Healing, 0 },
            { UpgradeType.GpsShift, 0 },
            { UpgradeType.Damping, 0 }
        };
    }

    public string Name { get; set; } = "";
    public string Id { get;} = "12345678";

    // Health
    private byte _health;
    public byte Health
    {
        get => _health;
        set
        {
            if(value <= (byte)(Settings.Health + Upgrades[UpgradeType.Health] * 15))
            {
                _health = value;
            }
            else
            {
                _health = (byte)(Settings.Health + Upgrades[UpgradeType.Health] * 15);
            }

            OnHealthChanged?.Invoke(this, _health);
        }
    }

    // Healing
    public byte Healing { get; set; }


    // Ammo
    private byte _ammo;
    public byte Ammo
    { get => _ammo;
        set
        {
            if(value <= Settings.weaponInfo[WeaponType].AmmoPerMag)
            {
                _ammo = value;
            }
            else
            {
                _ammo = (byte)(Settings.weaponInfo[WeaponType].AmmoPerMag);
            }

            OnAmmoChanged?.Invoke(this, _ammo);
        }
    }

    // Max Ammo
    private ushort _maxAmmo;
    public ushort MaxAmmo 
    {
        get => _maxAmmo;
        set
        {
            if(value <= Settings.weaponInfo[WeaponType].MaxAmmo)
            {
                _maxAmmo = value;
            }
            else
            {
                _maxAmmo = Settings.weaponInfo[WeaponType].MaxAmmo;
            }

            OnMaxAmmoChanged?.Invoke(this, _maxAmmo);
        }
    }
    


    // Location
    private GPS _location;
    public GPS Location
    {
        get => _location;
        set
        {
            _location = value;
            OnLocationChanged?.Invoke(this, _location);
        }
    }


    // Team
    public Team Team { get; set; }


    // Weapon
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

    // Coins

    private byte _coins;
    public byte Coins {
        get => _coins;
        set
        {
            _coins = value;
            OnCoinsChanged?.Invoke(this, _coins);
        }
    
    }

    // Upgrades

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


    // Abilities
    public void ActivateAbility(Abilitys abilitys)
    {
        AbilityActivated?.Invoke(this, abilitys);
    }
    public bool AbilityActive { get; set; }
}
