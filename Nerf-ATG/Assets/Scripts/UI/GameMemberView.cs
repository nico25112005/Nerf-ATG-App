
using Game.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class GameMemberView : MonoBehaviour, IGameMemberView
{
    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private GameMemberPresenter presenter;


    [Inject]
    private IPlayerModel playerModel;

    [Inject]
    private IGameModel gameModel;

    [Inject]
    private ITcpClientService tcpClientService;


    private bool _isHost = false;

    private void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        presenter = new GameMemberPresenter(this, playerModel, gameModel, tcpClientService);

        //StartCoroutine(SpawnPlayers(20));
    }


    public void UpdateMemberList(IPlayerInfo player) //Todo: refactor to do it without handing over the whole list
    {
        switch (player.Action)
        {
            case PacketAction.Add:
                AddGameMember(player);
                ToastNotification.Show("Player Joined: " + player.Name, "info");
                break;

            case PacketAction.Remove:
                RemoveGameMember(player.PlayerId);
                ToastNotification.Show("Player Left: " + player.Name, "info");
                break;
            
            case PacketAction.Update:
                RemoveGameMember(player.PlayerId);
                AddGameMember(player);
                break;
        }
    }

    private void AddGameMember(IPlayerInfo player)
    {
        GameObject prefabInstance = Instantiate(registry.GetElement("GameMemberPrefab"), registry.GetElement("GameMemberList").transform);
        prefabInstance.transform.Find("MemberName").GetComponent<Text>().text = player.Name;
        prefabInstance.name = player.PlayerId;

        Color32 teamcolor;

        switch ((Team)player.Index)
        {
            case Team.Red:
                teamcolor = new Color32(146, 0, 197, 255);
                break;
            case Team.Blue:
                teamcolor = new Color32(0, 118, 197, 255);
                break;
            case Team.Violet:
                teamcolor = new Color32(197, 0, 32, 255);
                break;
            default:
                teamcolor = new Color32(255, 255, 255, 255);
                break;
        }

        prefabInstance.transform.Find("MemberTeam").GetComponent<Image>().color = teamcolor;

        if (_isHost)
        {
            prefabInstance.transform.Find("MemberTeam").GetComponent<Button>().onClick.AddListener(() => ButtonClick(player));

            void ButtonClick(IPlayerInfo playerInfo)
            {
                presenter.SwitchTeam(playerInfo.PlayerId.ToString());
            }
        }
        else
        {
            prefabInstance.transform.Find("MemberTeam").GetComponent<Button>().interactable = false;
        }
    }

    private void RemoveGameMember(string playerId)
    {
        Destroy(registry.GetElement("GameMemberList").transform.Find(playerId).gameObject);
    }



    public void ActivateHostPanel()
    {
        _isHost = true;
        registry.GetElement("StartGameButton").SetActive(_isHost);
    }

    public void MoveToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StartGame()
    {
        presenter.StartGame();
    }

    public void Quit()
    {
        presenter.Quit();
    }

    //Todo: remove testCode
    /*
    private IEnumerator SpawnPlayers(int amaount)
    {
        int count = 0;
        while (count < amaount)
        {

            presenter.Spawn();
            yield return new WaitForSeconds(1.5f);
            count++;
        }
    }
    */
}