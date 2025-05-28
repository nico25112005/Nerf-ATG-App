using Assets.Scripts.UI.View.Interfaces;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

//using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Menu : MonoBehaviour, IMenuView
{
    [Header("UI References")]
    [SerializeField]
    private List<UIElementEntry> uiElements;

    private UIElementRegistry registry;
    private MenuPresenter presenter;
    private TcpClientService client;


    public string PlayerName { get; private set; }


    private void Awake()
    {
        registry = gameObject.AddComponent<UIElementRegistry>();
        registry.RegisterElements(uiElements);

        client = new TcpClientService();

        var model = new FakeModel();
        presenter = new MenuPresenter(this, model, client);
    }


    // view

    public void UpdateBlasterList(List<string> blasters)
    {
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
       
    }

    public void Scan()
    {

    }

    public void ConnectToServer()
    {
        presenter.ConnectToServer(registry.GetElement("Name").GetComponent<InputField>().text);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
