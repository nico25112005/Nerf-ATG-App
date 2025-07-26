using Game.Enums;
using System.Collections.Generic;
using System.Xml.Serialization;

public interface IUpgradeView
{
    void UpdateCoins(byte coins);
    void UpdateUpgrades(Dictionary<UpgradeType, byte> upgrades);
    void ShowToastMessage(string message, string icon);

}