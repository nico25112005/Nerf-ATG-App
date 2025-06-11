using Game;
using Game.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline.Actions;

public class GamePresenter
{

    IGameView view;

    IPlayerModel playerModel;

    ITcpClientService tcpClientService;


    public GamePresenter(IGameView view, IPlayerModel playerModel, ITcpClientService tcpClientService)
    {
        this.view = view;

        this.playerModel = playerModel;
        
        this.tcpClientService = tcpClientService;

        playerModel.OnHealthChanged += UpdateHealthBar;
        playerModel.OnAmmoChanged += UpdateAmmoBar;
        playerModel.OnMaxAmmoChanged += UpdateMaxAmmoBar;

        if(view is IGameViewUnityExtension unityView)
        {
            unityView.UpdateTeam(playerModel.Team);
            unityView.UpdateWeaponIcon(playerModel.WeaponType);
            unityView.UpdateUpgradeInfo(playerModel.Upgrades);
        }

        

    }

    public void UpdateHealthBar(object sender, byte health)
    {
        view.UpdateHealthBar(health);
    }

    public void UpdateAmmoBar(object sender, byte ammo)
    {
        view.UpdateAmmoBar(playerModel.WeaponType, ammo);
    }

    public void UpdateMaxAmmoBar(object sender, ushort maxAmmo)
    {
        view.UpdateMaxAmmoBar(playerModel.WeaponType, maxAmmo);
    }


    // buttons

    public void Quit()
    {

    }

    public void Dispose()
    {
        playerModel.OnHealthChanged -= UpdateHealthBar;
        playerModel.OnAmmoChanged -= UpdateAmmoBar;
        playerModel.OnMaxAmmoChanged -= UpdateMaxAmmoBar;
    }
}