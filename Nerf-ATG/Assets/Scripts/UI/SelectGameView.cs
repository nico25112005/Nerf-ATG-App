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

    public void UpdateGameList(List<GameInfo> games)
    {
        foreach (Transform child in registry.GetElement("GameList").transform)
        {
            Destroy(child.gameObject);
        }

        foreach(GameInfo gameInfo in games)
        {
            
            GameObject prefabInstance = Instantiate(registry.GetElement("GamePrefab"), registry.GetElement("GameList").transform);
            prefabInstance.transform.Find("DeviceName").GetComponent<Text>().text = gameInfo.gameName;
            prefabInstance.transform.Find("Playercount").GetComponent<Text>().text = $"{gameInfo.playerCount}/{gameInfo.maxPlayer}";
            prefabInstance.transform.Find("Gamemode").GetComponent<Text>().text = gameInfo.gameType.ToAbbreviation();
            prefabInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClick(prefabInstance.GetComponentInChildren<Text>()));

            void ButtonClick(Text game)
            {
                registry.GetElement("GameConnection").GetComponent<Text>().text = game.text;
            }
        }
    }

    public void Join()
    {

        presenter.Join(registry.GetElement("GameConnection").GetComponent<Text>().text);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void CreateGame()
    {
        string gameName = registry.GetElement("GameName").GetComponent<InputField>().text;

        Dropdown gameTypeDropdown = registry.GetElement("GameType").GetComponent<Dropdown>();

        GameType gameType = gameTypeDropdown.options[gameTypeDropdown.value].text.Replace(" ", "").ToEnum<GameType>();


        presenter.CreateGame(gameName, gameType);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

