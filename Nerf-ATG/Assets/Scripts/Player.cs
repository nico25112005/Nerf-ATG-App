using Game.Enums;
using Game;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UIElements.Experimental;
using UnityEngine;

public class Player
{
    private static Player instance;
    private static readonly object _lock = new();

    //private Dictionary<UpgradeType, byte> upgrades;
    private Team teamInfo;
    private byte health;
    private byte coins;
    private WeaponType weaponType;
    private byte ammo;
    private int maxAmmo;
    private GPSData gpsData = new();



    public event EventHandler<EventArgs> TeamInfoChanged;
    public event EventHandler<EventArgs> HealthChanged;
    public event EventHandler<EventArgs> CoinsChanged;
    public event EventHandler<EventArgs> WeaponTypeChanged;
    public event EventHandler<EventArgs> UpgradesChanged;
    public event EventHandler<EventArgs> AmmoChanged;
    public event EventHandler<EventArgs> MaxAmmoChanged;
    public event EventHandler<EventArgs> GpsDataChanged;

    private Player()
    {
        ResetInstance();
    }

    public void ResetInstance()
    {
        Upgrades = new Dictionary<UpgradeType, byte>
        {
            { UpgradeType.Health, 0 },
            { UpgradeType.Healing, 0 },
            { UpgradeType.GpsShift, 0 },
            { UpgradeType.Damping, 0 }
        };

        Coins = 25;
        Health = 100;
        TeamInfo = Team.Violet;
        WeaponType = WeaponType.None;
    }


    public static Player GetInstance()
    {
        lock (_lock)
        {
            instance ??= new Player();
            return instance;
        }
    }

    public Team TeamInfo
    {
        get { return teamInfo; }
        set
        {
            if (teamInfo != value)
            {
                teamInfo = value;
                TeamInfoChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public Dictionary<UpgradeType, byte> Upgrades { get; private set; }

    public byte Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = value;
                HealthChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public byte Coins
    {
        get { return coins; }
        set
        {
            if (coins != value)
            {
                coins = value;
                CoinsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public byte Ammo
    {
        get { return ammo; }
        set
        {
            if (ammo != value)
            {
                ammo = value;
                AmmoChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public int MaxAmmo
    {
        get { return maxAmmo; }
        set
        {
            if (maxAmmo != value)
            {
                maxAmmo = value;
                MaxAmmoChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public WeaponType WeaponType
    {
        get { return weaponType; }
        set
        {
            if (weaponType != value)
            {
                weaponType = value;
                WeaponTypeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public GPSData GPSData
    {
        get { return gpsData; }
        set
        {
            gpsData = value;
            //GpsDataChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SetUpgrades(UpgradeType type, byte value)
    {
        Upgrades[type] = value;
        health = (byte)(Settings.Health + Upgrades[UpgradeType.Health] * 15);
        UpgradesChanged?.Invoke(this, EventArgs.Empty);
    }
    public override string ToString()
    {
        string result = "Player Info:\n";
        result += string.Format("Team: {0}\n", TeamInfo);
        result += string.Format("Health: {0}\n", Health);
        result += string.Format("Coins: {0}\n", Coins);

        result += "Upgrade Levels:\n";
        foreach (var kvp in Upgrades)
        {
            result += string.Format("{0}: {1}\n", kvp.Key, kvp.Value);
        }

        return result;
    }
}
