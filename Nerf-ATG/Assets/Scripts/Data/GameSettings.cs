using Assets.Scripts;
using Game.Enums;
using System;
using System.Buffers.Text;
using System.Collections.Generic;

namespace Game
{

    namespace Enums
    {
        public static class EnumExtensions
        {
            public static string ToAbbreviation(this GameType gameType)
            {
                return gameType switch
                {
                    GameType.FreeForAll => "FFA",
                    GameType.TeamDeathMatch => "TDM",
                    _ => gameType.ToString()
                };
            }

            public static string ToAbbreviation(this WeaponType weaponType)
            {
                return weaponType switch
                {
                    WeaponType.Sniper => "SNP",
                    WeaponType.Mp => "MP",
                    WeaponType.Rifle => "RFL",
                    WeaponType.None => "NON",
                    _ => weaponType.ToString()
                };
            }

            public static string ToAbbreviation(this Team team)
            {
                return team switch
                {
                    Team.Red => "RED",
                    Team.Blue => "BLU",
                    Team.Violet => "VIO",
                    _ => team.ToString()
                };
            }

            public static string ToAbbreviation(this UpgradeType upgrade)
            {
                return upgrade switch
                {
                    UpgradeType.Health => "HP",
                    UpgradeType.Healing => "HL",
                    UpgradeType.GpsShift => "GPS",
                    UpgradeType.Damping => "DMP",
                    _ => upgrade.ToString()
                };
            }

            public static string ToAbbreviation(this Abilitys ability)
            {
                return ability switch
                {
                    Abilitys.RapidFire => "RF",
                    Abilitys.GPSLocate => "GPS",
                    Abilitys.Healpackage => "HLPKG",
                    _ => ability.ToString()
                };
            }

            public static T ToEnum<T>(this string value) where T : struct, Enum
            {
                if (Enum.TryParse<T>(value, out T enumValue))
                {
                    return enumValue;
                }
                return default;
            }

        }

        public enum UpgradeType
        {
            Health,
            Healing,
            GpsShift,
            Damping
        }

        public enum WeaponType
        {
            Sniper,
            Mp,
            Rifle,
            None
        }

        public enum Team
        {
            Red,
            Blue,
            Violet
        }

        public enum Abilitys
        {
            RapidFire,
            GPSLocate,
            Healpackage
        }

        public enum GameType
        {
            FreeForAll,
            TeamDeathMatch
        }
    }

    public static class Settings
    {
        public static byte BaseRadius = 8;

        public static byte Coins = 25;
        public static byte Healing = 4;
        public static byte Health = 100;

        public static Dictionary<UpgradeType, UpgradeInfo> upgradeInfo = new()
        {
            {UpgradeType.Health, new UpgradeInfo(UpgradeType.Health, new byte[] { 2, 2, 2 }, GameAssets.Instance.health)},
            {UpgradeType.Healing, new UpgradeInfo(UpgradeType.Healing, new byte[] { 1, 3, 4 }, GameAssets.Instance.healing)},
            {UpgradeType.GpsShift, new UpgradeInfo(UpgradeType.GpsShift, new byte[] { 4, 8 }, GameAssets.Instance.gps)},
            {UpgradeType.Damping, new UpgradeInfo(UpgradeType.Damping, new byte[] { 4, 8 }, GameAssets.Instance.damping)}
        };

        public static Dictionary<WeaponType, WeponInfo> weaponInfo = new()
        {
            {WeaponType.Sniper, new WeponInfo(WeaponType.Sniper, 13, GameAssets.Instance.sniper, 1, 0f, 10, 2, Abilitys.GPSLocate, 8, 2.5f, 35)},
            {WeaponType.Mp, new WeponInfo(WeaponType.Mp, 9, GameAssets.Instance.mp, 1, 0f, 300, 75, Abilitys.RapidFire, 3, 0.125f, 1)},
            {WeaponType.Rifle,new WeponInfo(WeaponType.Rifle, 12, GameAssets.Instance.rifle, 3, 0.33f, 180, 45, Abilitys.Healpackage, 4, 1.5f, 4)}
        };
    }
}
