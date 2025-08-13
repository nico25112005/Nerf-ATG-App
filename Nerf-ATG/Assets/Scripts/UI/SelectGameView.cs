using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Enums;
using UnityEngine.SceneManagement;
using Zenject;



public class SelectGameView : MonoBehaviour, ISelectGameView, IConnectionInfo
{


    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private SelectGamePresenter presenter;

    [Inject]
    private IPlayerModel playerModel;

    [Inject]
    private IGameModel gameModel;

    [Inject]
    private IServerModel serverModel;

    [Inject]
    private ITcpClientService tcpClientService;


    private void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        presenter = new SelectGamePresenter(this, playerModel, gameModel, serverModel, tcpClientService);

    }

    public void UpdateGameList(GameInfo gameInfo)
    {
        Debug.LogWarning($"View: Added game {gameInfo}");

        switch (gameInfo.Action)
        {
            case PacketAction.Add:
                AddGame(gameInfo);
                ToastNotification.Show("New Game: " + gameInfo.GameName, "info");
                break;

            case PacketAction.Remove:
                RemoveGame(gameInfo);
                ToastNotification.Show("Game " + gameInfo.GameName + " got closed", "info");
                break;

            case PacketAction.Update:
                RemoveGame(gameInfo);
                AddGame(gameInfo);
                break;
        }

        Debug.Log($"GameCount: {registry.GetElement("GameList").transform.childCount}");


    }

    private void AddGame(GameInfo gameInfo)
    {
        GameObject prefabInstance = Instantiate(registry.GetElement("GamePrefab"), registry.GetElement("GameList").transform);
        prefabInstance.name = gameInfo.GameId;
        prefabInstance.transform.Find("DeviceName").GetComponent<Text>().text = gameInfo.GameName;
        Text playercount = prefabInstance.transform.Find("Playercount").GetComponent<Text>();
        if (gameInfo.MaxPlayer == 0)
        {
            playercount.text = "Playing";
            playercount.color = Color.green;
            playercount.fontSize = 40;
            playercount.fontStyle = FontStyle.Normal;

        }
        else if (gameInfo.PlayerCount == gameInfo.MaxPlayer)
        {
            playercount.text = "Full";
            playercount.color = Color.red;
            playercount.fontStyle = FontStyle.Normal;
        }
        else
        {
            playercount.text = $"{gameInfo.PlayerCount}/{gameInfo.MaxPlayer}";
        }

        prefabInstance.transform.Find("Gamemode").GetComponent<Text>().text = ((GameType)gameInfo.GameType).ToAbbreviation();
        prefabInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClick(prefabInstance.GetComponentInChildren<Text>()));

        void ButtonClick(Text game)
        {
            registry.GetElement("GameConnection").GetComponent<Text>().text = game.text;
        }
    }

    private void RemoveGame(GameInfo gameInfo)
    {
        if (registry.GetElement("GameList").transform.Find(gameInfo.GameId) != null)
        {
            Destroy(registry.GetElement("GameList").transform.Find(gameInfo.GameId).gameObject);
        }
        else
        {
            Debug.LogWarning("Game not found; Not deleted: " + gameInfo);
        }
    }

    public void Join()
    {

        presenter.Join(registry.GetElement("GameConnection").GetComponent<Text>().text);
    }

    public void CreateGame()
    {
        string gameName = registry.GetElement("GameName").GetComponent<InputField>().text;

        if (string.IsNullOrEmpty(gameName))
        {
            ToastNotification.Show("Please enter a gamename!", "error");
        }
        else if (name.Length > 12)
        {
            ToastNotification.Show("Gamename too long!", "error");
        }
        else
        {

            Dropdown gameTypeDropdown = registry.GetElement("GameType").GetComponent<Dropdown>();

            GameType gameType = gameTypeDropdown.options[gameTypeDropdown.value].text.Replace(" ", "").ToEnum<GameType>();


            presenter.CreateGame(gameName, gameType);
        }
    }

    public void UpdatePing(long ms)
    {
        registry.GetElement("Ping").GetComponent<Text>().text = ms.ToString() + " ms";
    }

    public void Quit()
    {
        presenter.Quit();

        SceneManager.LoadScene("Menu");
    }

    private void OnDestroy()
    {
        presenter.Dispose();
    }

    public void ShowToastMessage(string message, string icon)
    {
        ToastNotification.Show(message, icon);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}

