using Game.Enums;
using System;

public class PlayerModel : IPlayerModel
{
    // Events
    public event EventHandler<WeaponType> OnWeaponTypeChanged;
    public event EventHandler<byte> OnCoinsChanged;

    // Properties
    public string Name { get; set; } = "";
    public Guid Id { get;} = Guid.NewGuid();

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

}
