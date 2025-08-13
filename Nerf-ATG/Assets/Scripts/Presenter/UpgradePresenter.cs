using Game;
using Game.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class UpgradePresenter
{

    private readonly IUpgradeView view;

    private readonly IPlayerModel playerModel;
    private readonly IServerModel serverModel;

    private readonly ITcpClientService tcpClientService;


    public UpgradePresenter(IUpgradeView view, IPlayerModel playerModel, IServerModel serverModel, ITcpClientService tcpClientService)
    {
        this.view = view;
        this.playerModel = playerModel;
        this.serverModel = serverModel;

        this.tcpClientService = tcpClientService;


        if (view is IUpgradeViewUnityExtension unityView)
        {
            unityView.UpdateTeam(playerModel.Team);
            unityView.UpdateWeaponIcon(playerModel.WeaponType);
        }

        playerModel.OnUpgradesChanged += UpdateUpgradePrefab;
        playerModel.OnCoinsChanged += UpdateCoins;

        view.UpdateCoins(playerModel.Coins);

        serverModel.onPingChanged += UpdatePing;
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
            view.ShowToastMessage("Not enough coins", "error");
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
        playerModel.Healing = (byte)(Settings.Healing + playerModel.Upgrades[UpgradeType.Healing] * 2);

        tcpClientService.Send(ITcpClientService.Connections.Server, new PlayerReady(playerModel.Id.ToString(), playerModel.Health, playerModel.WeaponType, playerModel.Upgrades[UpgradeType.Damping], PacketAction.Generic));
    }

    public void UpdatePing(object sender, long ms)
    {
        if (view is IConnectionInfo connectionInfo)
        {
            connectionInfo.UpdatePing(ms);
        }
    }

    public void Quit()
    {
        tcpClientService.Send(ITcpClientService.Connections.Server, new QuitGame(playerModel.Id.ToString(), PacketAction.Generic));
    }

    public void Dispose()
    {
        playerModel.OnUpgradesChanged -= UpdateUpgradePrefab;
        playerModel.OnCoinsChanged -= UpdateCoins;

        serverModel.onPingChanged -= UpdatePing;
    }
}