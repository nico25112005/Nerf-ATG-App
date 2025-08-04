using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Enums;
using UnityEngine.SceneManagement;
using Zenject;



public class SelectGameView : MonoBehaviour, ISelectGameView
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
    }

    private void AddGame(GameInfo gameInfo)
    {
        GameObject prefabInstance = Instantiate(registry.GetElement("GamePrefab"), registry.GetElement("GameList").transform);
        prefabInstance.name = gameInfo.GameId;
        prefabInstance.transform.Find("DeviceName").GetComponent<Text>().text = gameInfo.GameName;
        prefabInstance.transform.Find("Playercount").GetComponent<Text>().text = $"{gameInfo.PlayerCount}/{gameInfo.MaxPlayer}";
        prefabInstance.transform.Find("Gamemode").GetComponent<Text>().text = ((GameType)gameInfo.GameType).ToAbbreviation();
        prefabInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClick(prefabInstance.GetComponentInChildren<Text>()));

        void ButtonClick(Text game)
        {
            registry.GetElement("GameConnection").GetComponent<Text>().text = game.text;
        }
    }

    private void RemoveGame(GameInfo gameInfo)
    {
        Destroy(registry.GetElement("GameList").transform.Find(gameInfo.GameId).gameObject);
    }

    public void Join()
    {

        presenter.Join(registry.GetElement("GameConnection").GetComponent<Text>().text);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

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

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
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

}

