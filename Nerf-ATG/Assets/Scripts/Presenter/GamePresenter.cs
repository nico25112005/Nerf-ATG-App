using Game;
using Game.Enums;
using System;
using System.Timers;
public class GamePresenter
{

    private Timer timer;
    private float elapsedTime = 0;
    float cooldown = 0;


    IGameView view;

    IPlayerModel playerModel;
    IGameModel gameModel;

    ITcpClientService tcpClientService;
    IMainThreadExecutor mainThreadExecutor;



    public GamePresenter(IGameView view, IPlayerModel playerModel, IGameModel gameModel, ITcpClientService tcpClientService, IMainThreadExecutor mainThreadExecutor)
    {
        this.view = view;

        this.playerModel = playerModel;
        this.gameModel = gameModel;

        this.tcpClientService = tcpClientService;
        this.mainThreadExecutor = mainThreadExecutor;

        playerModel.OnHealthChanged += UpdateHealthBar;
        playerModel.OnAmmoChanged += UpdateAmmoBar;
        playerModel.OnMaxAmmoChanged += UpdateMaxAmmoBar;
        playerModel.AbilityActivated += HealthPackageAbility;
        playerModel.AbilityActivated += RapidFireAbility;

        playerModel.WeaponType = WeaponType.Sniper;

        if (view is IGameViewUnityExtension unityView)
        {
            unityView.UpdateTeam(playerModel.Team);
            unityView.UpdateWeaponIcon(playerModel.WeaponType);
            unityView.UpdateUpgradeInfo(playerModel.Upgrades);
        }

        view.UpdateAbilityIcon(Settings.weaponInfo[playerModel.WeaponType].Ability, 1);
        playerModel.Health += 0;
        playerModel.Ammo += 0;
        playerModel.MaxAmmo += 0;

        playerModel.Health -= 110;

    }

    public void UpdateHealthBar(object sender, byte health)
    {
        view.UpdateHealthBar(health, (byte)(Settings.Health + playerModel.Upgrades[UpgradeType.Health] * 15));
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

    public void ActivateAbility()
    {
        var ability = Settings.weaponInfo[playerModel.WeaponType].Ability;

        view.UpdateAbilityIcon(ability, 0);
        
        playerModel.ActivateAbility(ability);


        cooldown = Settings.abilityInfo[ability].Cooldown;
        elapsedTime = 0f;

        timer = new Timer(cooldown * 1); // 1 updates per cooldown duration
        timer.Elapsed += (s, e) => OnTimerTick(ability);
        timer.AutoReset = true;
        timer.Start();
    }

    private void OnTimerTick(Abilitys ability)
    {
        elapsedTime += (float)timer.Interval / 1000f;

        float fillAmount = (float)Math.Min(1.0, Math.Max(0.0, elapsedTime / cooldown));
        mainThreadExecutor.Execute(() => view.UpdateAbilityIcon(ability, fillAmount));

        if (elapsedTime >= cooldown)
        {
            timer.Stop();
            timer.Dispose();
        }
    }

    public void HealthPackageAbility(object sender, Abilitys ability)
    {
        if(ability == Abilitys.Healpackage)
        {
            playerModel.Health += 50;
        }
    }

    public void RapidFireAbility(object sender, Abilitys ability)
    {
        if(ability == Abilitys.RapidFire)
        {
            //Todo: TcpClient to esp32
        }
    }


    int count = 0;
    public void Quit()
    {
        new VirtualPlayer(playerModel.Location, gameModel, mainThreadExecutor, 15, $"Player_{++count}");
    }

    public void Dispose()
    {

        playerModel.OnHealthChanged -= UpdateHealthBar;
        playerModel.OnAmmoChanged -= UpdateAmmoBar;
        playerModel.OnMaxAmmoChanged -= UpdateMaxAmmoBar;
        playerModel.AbilityActivated -= HealthPackageAbility;
        playerModel.AbilityActivated -= RapidFireAbility;


        if (timer != null)
        {
            timer.Stop();
            timer.Dispose();
        }
    }
}