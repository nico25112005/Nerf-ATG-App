using Game;
using Game.Enums;

public class WeaponPresenter
{
    private readonly IWeaponView view;
    private readonly IPlayerModel playerModel;
    private readonly IServerModel serverModel;



    public WeaponPresenter(IWeaponView view, IPlayerModel playerModel, IServerModel serverModel)
    {
        this.view = view;

        this.playerModel = playerModel;
        this.serverModel = serverModel;


        this.playerModel.OnCoinsChanged += UpdateCoins;
        this.playerModel.OnWeaponTypeChanged += UpdateWeaponIcon;

        if (view is IWeaponViewUnityExtension unityView)
        {
            unityView.UpdateTeam(playerModel.Team);
        }

        serverModel.onPingChanged += UpdatePing;
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

    public void UpdatePing(object sender, long ms)
    {
        if (view is IConnectionInfo connectionInfo)
        {
            connectionInfo.UpdatePing(ms);
        }
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
    }

    public void Quit()
    {
        GameManager.GetInstance().ResetGame();
    }

    public void Dispose()
    {
        playerModel.OnCoinsChanged -= UpdateCoins;
        playerModel.OnWeaponTypeChanged -= UpdateWeaponIcon;

        serverModel.onPingChanged -= UpdatePing;

    }
}
