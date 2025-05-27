using UnityEngine;

// Verbindet View und Presenter (kann auch Zenject verwenden)
public class GameInstaller : MonoBehaviour
{
    [SerializeField] private PlayerView _playerViewPrefab;

    private PlayerPresenter _presenter;

    private void Awake()
    {
        // Abhängigkeiten erstellen
        Player player = Player.GetInstance();
        PlayerView view = Instantiate(_playerViewPrefab);

        // Presenter injizieren
        _presenter = new PlayerPresenter(player, view);
    }

    private void OnDestroy()
    {
        _presenter?.Dispose();
    }
}