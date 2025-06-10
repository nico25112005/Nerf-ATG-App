using Game.Enums;
using System.Collections.Generic;

public interface IUpgradeView
{
    void UpdateCoins(byte coins);
    void UpdateUpgrades(Dictionary<UpgradeType, byte> upgrades);

}