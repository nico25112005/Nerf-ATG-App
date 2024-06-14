using Game;
using Game.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Player
    {
        private static Player instance;
        private static readonly object _lock = new object();

        public Dictionary<UpgradeType, byte> upgradeLevel;

        private Team teamInfo;
        private byte health;
        private byte coins;
        private WeaponType weaponType;

        public event EventHandler<EventArgs> TeamInfoChanged;
        public event EventHandler<EventArgs> HealthChanged;
        public event EventHandler<EventArgs> CoinsChanged;
        public event EventHandler<EventArgs> WeaponTypeChanged;

        private Player()
        {
            ResetInstance();
        }

        public void ResetInstance()
        {
            upgradeLevel = new Dictionary<UpgradeType, byte>
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
                if (instance == null)
                {
                    instance = new Player();
                }
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

        public bool BuyUpgrade(UpgradeType upgradeType, byte price, byte maxUpgrades)
        {
            if (Coins - price >= 0 && maxUpgrades > upgradeLevel[upgradeType])
            {
                Coins -= price;
                upgradeLevel[upgradeType] += 1;
                return true;
            }
            else return false;
        }

        public void ResetUpgrades()
        {
            Coins = (byte)(25 - Settings.weaponInfo[this.WeaponType].Price);
            upgradeLevel.Keys.ToList().ForEach(key => upgradeLevel[key] = 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public override string ToString()
        {
            string result = "Player Info:\n";
            result += string.Format("Team: {0}\n", TeamInfo);
            result += string.Format("Health: {0}\n", Health);
            result += string.Format("Coins: {0}\n", Coins);

            result += "Upgrade Levels:\n";
            foreach (var kvp in upgradeLevel)
            {
                result += string.Format("{0}: {1}\n", kvp.Key, kvp.Value);
            }

            return result;
        }
    }
}
