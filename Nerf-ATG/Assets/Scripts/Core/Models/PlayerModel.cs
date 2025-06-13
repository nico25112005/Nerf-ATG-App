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

    // Properties
    public string Name { get; set; } = "";
    public Guid Id { get;} = Guid.NewGuid();

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

    public byte Healing { get; set; }

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

    public void ActivateAbility(Abilitys abilitys)
    {
        AbilityActivated?.Invoke(this, abilitys);
    }
}
