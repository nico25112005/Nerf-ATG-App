using Game;
using Game.Enums;
using System.Collections.Generic;
using System.Linq;

public class UpgradePresenter
{

    IUpgradeView view;

    IPlayerModel playerModel;

    public UpgradePresenter(IUpgradeView view, IPlayerModel playerModel)
    {
        this.view = view;
        this.playerModel = playerModel;

        if (view is IUpgradeViewUnityExtension unityView)
        {
            unityView.UpdateTeam(playerModel.Team);
            unityView.UpdateWeaponIcon(playerModel.WeaponType);
        }

        playerModel.OnUpgradesChanged += UpdateUpgradePrefab;
        playerModel.OnCoinsChanged += UpdateCoins;

        view.UpdateCoins(playerModel.Coins);
    }

    public void UpdateUpgradePrefab(object sender, Dictionary<UpgradeType, byte> upgrades)
    {
        view.UpdateUpgrades(upgrades);

    }

    public void UpdateCoins(object sender, byte coins)
    {
        view.UpdateCoins(coins);
    }


    //buttons

    public void BuyUpgrade(UpgradeType upgradeType)
    {
        byte upgradeCost = Settings.upgradeInfo[upgradeType].Price[playerModel.Upgrades[upgradeType]];

        if (playerModel.Coins - upgradeCost >= 0)
        {
            playerModel.AddUpgrade(upgradeType);
            playerModel.Coins -= upgradeCost;
        }
        else
        {
            //Todo: toast info or something
        }
    }

    public void ResetUpgrades()
    {
        foreach (UpgradeType upgrades in playerModel.Upgrades.Keys.ToList())
        {
            playerModel.SetUpgrades(upgrades, 0);
        }

        playerModel.Coins = (byte)(Settings.Coins - Settings.weaponInfo[playerModel.WeaponType].Price);
    }

    public void NextScene()
    {
        playerModel.Health = (byte)(Settings.Health + playerModel.Upgrades[UpgradeType.Health] * 15);
        playerModel.Healing = (byte)(Settings.Health + playerModel.Upgrades[UpgradeType.Health] * 2);
    }

    public void Quit()
    {

    }

    public void Dispose()
    {
        playerModel.OnUpgradesChanged -= UpdateUpgradePrefab;
        playerModel.OnCoinsChanged -= UpdateCoins;
    }
}