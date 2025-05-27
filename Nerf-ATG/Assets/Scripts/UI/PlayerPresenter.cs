using Game;
using System;

// Der Presenter kennt die Logik, aber NICHTS von Unity!
public class PlayerPresenter
{
    private readonly Player _player;
    private readonly PlayerView _view;

    public PlayerPresenter(Player player, PlayerView view)
    {
        _player = player;
        _view = view;

        // Events abonnieren
        _player.HealthChanged += OnHealthChanged;
        _view.OnAbilityButtonClicked += OnAbilityActivated;
    }

    private void OnHealthChanged(object sender, EventArgs e)
    {
        // Model-Daten in UI umwandeln
        _view.UpdateHealthUI(_player.Health, Settings.Health);
    }

    private void OnAbilityActivated()
    {
        // Logik bei Button-Klick
        if (_player.Coins >= 10)
        {
            _player.AbilityActivated = true;
        }
    }

    public void Dispose()
    {
        // Events abmelden (verhindert Memory Leaks)
        _player.HealthChanged -= OnHealthChanged;
        _view.OnAbilityButtonClicked -= OnAbilityActivated;
    }
}