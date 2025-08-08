using Game;
using Game.Enums;
using System;
using System.Linq;
using System.Timers;

public class GamePresenter
{
    private Timer timer;
    private float elapsedTime = 0;
    private float cooldown = 0;

    private readonly IGameView view;

    private readonly IPlayerModel playerModel;
    private readonly IGameModel gameModel;
    private readonly IServerModel serverModel;

    private readonly ITcpClientService tcpClientService;
    private readonly IMainThreadExecutor mainThreadExecutor;


    public GamePresenter(IGameView view, IPlayerModel playerModel, IGameModel gameModel, IServerModel serverModel, ITcpClientService tcpClientService, IMainThreadExecutor mainThreadExecutor)
    {
        this.view = view;

        this.playerModel = playerModel;
        this.gameModel = gameModel;
        this.serverModel = serverModel;

        this.tcpClientService = tcpClientService;
        this.mainThreadExecutor = mainThreadExecutor;

        playerModel.OnHealthChanged += UpdateHealthBar;
        playerModel.OnAmmoChanged += UpdateAmmoBar;
        playerModel.OnMaxAmmoChanged += UpdateMaxAmmoBar;
        playerModel.AbilityActivated += HealthPackageAbility;
        playerModel.AbilityActivated += RapidFireAbility;
        playerModel.OnLocationChanged += SetBaseLocationButtonActive;

        gameModel.onReadyPlayerCountChanged += UpdateInformationPanel;
        gameModel.onNewBaseLocation += NewBaseSet;

        serverModel.onPingChanged += UpdatePing;

        Init();
    }


    private void Init()
    {
        if (view is IGameViewUnityExtension unityView)
        {
            unityView.UpdateTeam(playerModel.Team);
            unityView.UpdateWeaponIcon(playerModel.WeaponType);
            unityView.UpdateUpgradeInfo(playerModel.Upgrades);
        }

        view.UpdateAbilityIcon(Settings.weaponInfo[playerModel.WeaponType].Ability, 1);


        UpdateHealthBar(this, playerModel.Health);
        UpdateAmmoBar(this, playerModel.Ammo);
        UpdateMaxAmmoBar(this, playerModel.MaxAmmo);
        UpdateInformationPanel(this, 0);
    }

    public void UpdateHealthBar(object sender, byte health)
    {
        UnityEngine.Debug.Log("Health: " + health);
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

    public void SetBaseLocationButtonActive(object sender, GPS gps)
    {
        if (gameModel.readyPlayerCount == gameModel.playerInfo.Count() && string.Equals(gameModel.teamLeader[playerModel.Team].Item1, playerModel.Id.ToString()[..8]))
        {
            view.SetBaseLocationButtonVisable();
            playerModel.OnLocationChanged -= SetBaseLocationButtonActive;
        }
    }

    public void NewBaseSet(object sender, Team team)
    {
        UnityEngine.Debug.LogWarning($"NewBaseSetEvent: {sender}");

        if (team == playerModel.Team)
        {
            view.SetBaseInformationText($"Your Teamleader is: {gameModel.teamLeader[playerModel.Team].Item2}\n" +
                       $"Waiting for the teamleaders to set their base: {gameModel.baseLocation.Count()}/{gameModel.teamLeader.Count()}\n" +
                       $"Within a radius of {Game.Settings.BaseRadius} around your base, you can heal and replenish your ammunition.");

        }

        if (gameModel.baseLocation.Count() == gameModel.teamLeader.Count())
        {
            view.DeactivateInformationPanel();
            view.StartSendingPlayerStatus();
            gameModel.onNewBaseLocation -= NewBaseSet;
        }
    }

    public void UpdateInformationPanel(object sender, byte readyPlayerCount)
    {

        if (readyPlayerCount < gameModel.playerInfo.Count())
        {
            UnityEngine.Debug.LogWarning($"Waiting: {sender}, Ready: {gameModel.readyPlayerCount}/{gameModel.playerInfo.Count()}");


            view.SetBaseInformationText($"Waiting for all players to get ready\n\n Ready: {gameModel.readyPlayerCount}/{gameModel.playerInfo.Count()}");
        }
        else
        {
            if (string.Equals(gameModel.teamLeader[playerModel.Team].Item1, playerModel.Id.ToString()[..8]))
            {
                UnityEngine.Debug.LogWarning($"teamleaderid: {gameModel.teamLeader[playerModel.Team].Item1}, playerid: {playerModel.Id}");
                UnityEngine.Debug.LogWarning($"Teamleader: {sender}");

                view.SetBaseInformationText($"Please go to a place where you would like to set the base for your team." +
                    $"You can then heal yourself and replenish your ammunition within a radius of {Game.Settings.BaseRadius} around your base.");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"No Teamleader: {sender}");

                view.SetBaseInformationText($"Your Teamleader is: {gameModel.teamLeader[playerModel.Team].Item2}\n" +
                    $"Waiting for the teamleaders to set their base: 0/{gameModel.teamLeader.Count()}\n" +
                    $"Within a radius of {Game.Settings.BaseRadius} around your base, you can heal and replenish your ammunition.");
            }
        }
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
            mainThreadExecutor.Execute(() => view.ShowToastMessage("Ability ready", "success"));
            timer.Stop();
            timer.Dispose();
        }
    }

    public void HealthPackageAbility(object sender, Abilitys ability)
    {
        if (ability == Abilitys.Healpackage)
        {
            playerModel.Health += 50;
        }
    }

    public void RapidFireAbility(object sender, Abilitys ability)
    {
        if (ability == Abilitys.RapidFire)
        {
            //Todo: TcpClient to esp32
        }
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
        GameManager.GetInstance().ResetGame();
    }

    public void UpdatePlayerStatus()
    {
        tcpClientService.Send(ITcpClientService.Connections.Server, new PlayerStatus(playerModel.Id.ToString(), playerModel.Name, playerModel.Team, playerModel.Location, playerModel.Health ,PacketAction.Update));
    }

    public void Dispose()
    {
        playerModel.OnHealthChanged -= UpdateHealthBar;
        playerModel.OnAmmoChanged -= UpdateAmmoBar;
        playerModel.OnMaxAmmoChanged -= UpdateMaxAmmoBar;
        playerModel.AbilityActivated -= HealthPackageAbility;
        playerModel.AbilityActivated -= RapidFireAbility;

        view.StopSendingPlayerStatus();

        serverModel.onPingChanged -= UpdatePing;

        if (timer != null)
        {
            timer.Stop();
            timer.Dispose();
        }
    }
}