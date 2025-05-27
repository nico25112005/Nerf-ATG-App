using Game;
using Game.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Timers;
using System.Net.NetworkInformation;
using System.Net.Mail;

public class Player
{
    public event EventHandler<EventArgs> TeamInfoChanged;
    public event EventHandler<EventArgs> HealthChanged;
    public event EventHandler<EventArgs> CoinsChanged;
    public event EventHandler<EventArgs> WeaponTypeChanged;
    public event EventHandler<EventArgs> UpgradesChanged;
    public event EventHandler<EventArgs> AmmoChanged;
    public event EventHandler<EventArgs> MaxAmmoChanged;
    public event EventHandler<EventArgs> GpsDataChanged;
    public event EventHandler<GPSData> EnemyLocationChanged;
    public event EventHandler<GPSData> TeamMateLocationChanged;


    private static Player instance;
    private static readonly object _lock = new();

    private Team teamInfo;
    private byte health;
    private byte coins;
    private WeaponType weaponType;
    private byte ammo;
    private int maxAmmo;



    private Player()
    {
        ResetInstance();
    }

    private void ResetInstance()
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

    public void DestroyInstance()
    {
        instance = null;
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

    public GPSData BaseLocation { get; set; }

    public Dictionary<UpgradeType, byte> Upgrades { get; private set; }

    public string PlayerName { get; set; }

    public string BlasterMacAdress { get; set; }

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

    public bool AbilityActivated { get; set; } = false;

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

    public GPSData GPSData { get; set; }

    public List<GPSData> EnemyLocations { get; set; } = new List<GPSData>();

    public List<GPSData> TeamMateLocations { get; set; } = new List<GPSData>();

    public void SetEnemyLocation(string locationHexCode)
    {
        EnemyLocations.Add(new GPSData
        {
            SerialData = Convert.ToUInt64(locationHexCode, 16)
        });
        EnemyLocationChanged?.Invoke(this, EnemyLocations.Last());
    }

    public void SetTeamMateLocation(string locationHexCode)
    {
        TeamMateLocations.Add(new GPSData
        {
            SerialData = Convert.ToUInt64(locationHexCode, 16)
        });
        TeamMateLocationChanged?.Invoke(this, TeamMateLocations.Last());
    }

    Timer baseRefillTimer;
    public void SetGPSData(double longitude, double latitude)
    {
        if(baseRefillTimer == null)
        {
            baseRefillTimer = new Timer(1000);
            baseRefillTimer.AutoReset = true;
            baseRefillTimer.Elapsed += BaseRefill;
            baseRefillTimer.Start();
        }

        try
        {
            GPSData = new GPSData
            {
                Longitude = longitude,
                Latitude = latitude
            };

            GpsDataChanged?.Invoke(this, EventArgs.Empty);

            if (GPS.IsWithinRadius(GPSData, BaseLocation, Settings.BaseRadius))
            {
                baseRefillTimer.Start();
            }
            else
            {
                baseRefillTimer.Stop();
            }

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void BaseRefill(object state, ElapsedEventArgs e)
    {
        Debug.LogWarning("Base refilling");
        if (Health + Settings.Healing <= Settings.Health)
        {
            MainThreadDispatcher.Execute(() =>
            {
                Health += Settings.Healing;
                //HealthChanged?.Invoke(this, EventArgs.Empty);
            });
        }
        else
        {
            if (Health < Settings.Health)
            {
                MainThreadDispatcher.Execute(() =>
                {
                    Health = Settings.Health;
                    //HealthChanged?.Invoke(this, EventArgs.Empty);
                });
            }
        }

        if (MaxAmmo + (Settings.weaponInfo[weaponType].MaxAmmo / 10) <= Settings.weaponInfo[weaponType].MaxAmmo)
        {
            MainThreadDispatcher.Execute(() =>
            {
                MaxAmmo += (Settings.weaponInfo[weaponType].MaxAmmo / 10);
                //MaxAmmoChanged?.Invoke(this, EventArgs.Empty);
            });
        }
        else
        {
            if (MaxAmmo < Settings.weaponInfo[weaponType].MaxAmmo)
            {
                MainThreadDispatcher.Execute(() =>
                {
                    MaxAmmo = Settings.weaponInfo[weaponType].MaxAmmo;
                    //MaxAmmoChanged?.Invoke(this, EventArgs.Empty);
                });
            }
        }
    }

    public void SetUpgrades(UpgradeType type, byte value)
    {
        Upgrades[type] = value;

        UpgradesChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ApplyUpgrades()
    {
        Settings.Health += (byte)(Upgrades[UpgradeType.Health] * 15);
        health = Settings.Health;
        Settings.Healing += (byte)(Upgrades[UpgradeType.Healing] * 2);
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
