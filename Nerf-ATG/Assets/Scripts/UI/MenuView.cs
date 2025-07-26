using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;



public class MenuView : MonoBehaviour, IMenuView
{
    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private MenuPresenter presenter;

    [Inject]
    private IPlayerModel playerModel;

    [Inject]
    private ITcpClientService tcpClientService;


    private void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        presenter = new MenuPresenter(this, playerModel, tcpClientService);
    }


    // view

    public void UpdateBlasterList(List<string> blasters)
    {

        foreach (Transform child in registry.GetElement("DeviceList").transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string blaster in blasters)
        {
            GameObject prefabInstance = Instantiate(registry.GetElement("DevicePrefab"), registry.GetElement("DeviceList").transform);
            prefabInstance.transform.Find("DeviceName").GetComponent<Text>().text = blaster;
            prefabInstance.GetComponent<Button>().onClick.AddListener(() => ButtonClick(prefabInstance.GetComponentInChildren<Text>()));

            void ButtonClick(Text blaster)
            {
                registry.GetElement("ConnectionDevice").GetComponent<Text>().text = blaster.text;
            }
        }
    }

    // buttons

    public void Connect()
    {
        presenter.Connect();
    }

    public void Scan()
    {
        presenter.Scan();
    }

    public void ConnectToServer()
    {
        string name = registry.GetElement("Name").GetComponent<InputField>().text;
        if (string.IsNullOrEmpty(name))
        {
            ToastNotification.Show("Please enter a name!", "error");
        }
        else if(name.Length > 12)
        {
            ToastNotification.Show("Name too long!", "error");
        }
        else
        {
            presenter.ConnectToServer(registry.GetElement("Name").GetComponent<InputField>().text);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

}
