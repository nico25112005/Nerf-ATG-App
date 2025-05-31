using Assets.Scripts.Presenter;
using UnityEngine;

public class PersistentMainManager : MonoBehaviour
{
    public static PersistentMainManager Instance { get; private set; }

    void Awake()
    {
        // Singleton sicherstellen
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeServices();
    }

    private void InitializeServices()
    {
        Debug.Log("Services wurden initialisiert.");



        new TcpDataPresenter(ServerModel.Instance, GameModel.Instance, FakeTcpClientService.Instance);
        
    }

}
