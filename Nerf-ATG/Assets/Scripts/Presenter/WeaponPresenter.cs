using Game;
using Game.Enums;

public class WeaponPresenter
{
    private readonly IWeaponView view;
    private readonly IPlayerModel playerModel;
    private readonly ITcpClientService tcpClientService;

    public WeaponPresenter(IWeaponView view, IPlayerModel playerModel, ITcpClientService tcpClientService)
    {
        this.view = view;

        this.playerModel = playerModel;

        this.tcpClientService = tcpClientService;

        this.playerModel.OnCoinsChanged += UpdateCoins;
        this.playerModel.OnWeaponTypeChanged += UpdateWeaponIcon;

        if (view is IWeaponViewUnityExtension unityView)
        {
            unityView.UpdateTeam(playerModel.Team);
        }
    }

    public void UpdateWeaponIcon(object sender, WeaponType weaponType)
    {
        view.UpdateWeaponIcon(weaponType);

        if (view is IWeaponViewUnityExtension unityView)
        {
            unityView.SetNextSceneActive(weaponType != WeaponType.None);
        }
    }

    public void UpdateCoins(object sender, byte coins)
    {
        view.UpdateCoins(coins);
    }



    //Buttons

    public void BuyWeapon(WeaponType weaponType)
    {
        playerModel.WeaponType = weaponType;
    }

    public void NextScene()
    {
        playerModel.Coins -= Settings.weaponInfo[playerModel.WeaponType].Price;
        playerModel.Ammo = Settings.weaponInfo[playerModel.WeaponType].AmmoPerMag;
        playerModel.MaxAmmo = Settings.weaponInfo[playerModel.WeaponType].MaxAmmo;


        tcpClientService.Send(ITcpClientService.Connections.Server, new PlayerReady(playerModel.Id.ToString(), playerModel.Health, playerModel.WeaponType, playerModel.Upgrades[UpgradeType.Damping], PacketAction.Generic));
    }

    public void Quit()
    {

    }

    public void Dispose()
    {
        playerModel.OnCoinsChanged -= UpdateCoins;
        playerModel.OnWeaponTypeChanged -= UpdateWeaponIcon;
    }
}
