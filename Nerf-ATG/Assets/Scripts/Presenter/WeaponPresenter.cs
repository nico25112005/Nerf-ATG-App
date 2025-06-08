


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

        //this.view.UpdateTeam(playerModel.Team);

    }

    public void UpdateWeaponIcon(object sender, WeaponType weaponType)
    {
        view.UpdateWeapon(weaponType);
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
        
    }

    public void Quit()
    {
        
    }
}
