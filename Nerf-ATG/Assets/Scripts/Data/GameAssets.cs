using Game.Enums;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public static GameAssets Instance
    {
        get
        {
            if (_instance == null) _instance = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _instance;
        }
    }

    public Sprite gps;
    public Sprite health;
    public Sprite healing;
    public Sprite damping;

    public Sprite sniper;
    public Sprite mp;
    public Sprite rifle;
    public Sprite empty;

    public Sprite medkit;
    public Sprite rapidfire;
    public Sprite radar;

    public Dictionary<WeaponType, Sprite> weapons;
    public Dictionary<UpgradeType, Sprite> upgrades;
    public Dictionary<Abilitys, Sprite> abilitys;

    private void Awake()
    {
        weapons = new()
        {
            {WeaponType.Sniper, sniper},
            {WeaponType.Mp, mp},
            {WeaponType.Rifle, rifle},
            {WeaponType.None , empty}
        };

        upgrades = new()
        {
            {UpgradeType.GpsShift, gps},
            {UpgradeType.Health, health},
            {UpgradeType.Healing, healing},
            {UpgradeType.Damping, damping}
        };

        abilitys = new()
        {
            {Abilitys.RapidFire, rapidfire},
            {Abilitys.GPSLocate, radar},
            {Abilitys.Healpackage, medkit}
        };
    }
}