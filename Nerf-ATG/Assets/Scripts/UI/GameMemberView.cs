
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMemberView : MonoBehaviour, IGameMemberView
{
    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private GameMemberPresenter presenter;


    private bool _isHost = false;

    private void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        presenter = new GameMemberPresenter(this, PlayerModel.Instance, GameModel.Instance, FakeTcpClientService.Instance);

        StartCoroutine(SpawnPlayers(20));
    }


    public void UpdateMemberList(List<PlayerInfo> gameMembers)
    {
        foreach (Transform child in registry.GetElement("GameMemberList").transform)
        {
            Destroy(child.gameObject);
        }

        foreach (PlayerInfo gameMember in gameMembers)
        {
            GameObject prefabInstance = Instantiate(registry.GetElement("GameMemberPrefab"), registry.GetElement("GameMemberList").transform);
            prefabInstance.transform.Find("MemberName").GetComponent<Text>().text = gameMember.playerName;

            Color32 teamcolor;

            switch (gameMember.teamIndex)
            {
                case 0:
                    teamcolor = new Color32(146, 0, 197, 255);
                    break;
                case 1:
                    teamcolor = new Color32(0, 118, 197, 255);
                    break;
                case 2:
                    teamcolor = new Color32(197, 0, 32, 255);
                    break;
                default:
                    teamcolor = new Color32(255, 255, 255, 255);
                    break;
            }

            prefabInstance.transform.Find("MemberTeam").GetComponent<Image>().color = teamcolor;
            if (!_isHost)
                prefabInstance.transform.Find("MemberTeam").GetComponent<Button>().interactable = false;
            prefabInstance.transform.Find("MemberTeam").GetComponent<Button>().onClick.AddListener(() => ButtonClick(gameMember));
            void ButtonClick(PlayerInfo playerInfo)
            {
                presenter.SwitchTeam(playerInfo.playerId.ToString());
            }
        }

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

}