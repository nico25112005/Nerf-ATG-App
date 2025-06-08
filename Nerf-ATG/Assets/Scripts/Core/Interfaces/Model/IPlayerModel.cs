using Game.Enums;
using System;

public interface IPlayerModel
{
    string Name { get; set; }
    Guid Id { get; }
    WeaponType WeaponType { get; set; }
    byte Coins { get; set; }

    event EventHandler<WeaponType> OnWeaponTypeChanged;
    event EventHandler<byte> OnCoinsChanged;
}
