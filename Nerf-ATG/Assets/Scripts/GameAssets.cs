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
    public Sprite epmty;

    public Dictionary<WeaponType, Sprite> weapons;

    private void Awake()
    {
        weapons = new()
        {
            {WeaponType.Sniper, sniper},
            {WeaponType.Mp, mp},
            {WeaponType.Rifle, rifle},
            {WeaponType.None , epmty}
        };
    }
}
