using Game;
using Game.Enums;

public class WeaponPresenter
{
    IWeaponView view;
    IPlayerModel playerModel;
    
    public WeaponPresenter(IWeaponView view, IPlayerModel playerModel)
    {
        this.view = view;

        this.playerModel = playerModel;

        this.playerModel.OnCoinsChanged += UpdateCoins;
        this.playerModel.OnWeaponTypeChanged += UpdateWeaponIcon;

        if(view is IWeaponViewUnityExtension unityView)
        {
            unityView.UpdateTeam(playerModel.Team);
        }
    }

    public void UpdateWeaponIcon(object sender, WeaponType weaponType)
    {
        view.UpdateWeaponIcon(weaponType);

        if(view is IWeaponViewUnityExtension unityView)
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
