using Game.Enums;
using System.Collections.Generic;

public interface IGameViewUnityExtension
{
    void UpdateTeam(Team team);
    void UpdateWeaponIcon(WeaponType weaponType);
    void UpdateUpgradeInfo(IReadOnlyDictionary<UpgradeType, byte> upgrades);
}