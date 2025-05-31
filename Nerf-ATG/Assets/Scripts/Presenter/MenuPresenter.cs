using System.Collections.Generic;

public class MenuPresenter
{
    private readonly IMenuView view;
    private readonly IPlayerModel model;
    private readonly ITcpClientService tcpClientService;
    //private readonly WiFiBridge wifiBridge = WiFiBridge.Instance;

    public MenuPresenter(IMenuView view, IPlayerModel model, ITcpClientService tcpClientService)
    {
        this.view = view;
        this.model = model;
        this.tcpClientService = tcpClientService;
    }


    public void UpdateBlasterList(List<string> blasters)
    {
        view.UpdateBlasterList(blasters);
    }

    public void Connect()
    {
        tcpClientService.Connect(ITcpClientService.Connections.ESP32, "Todo", 1234);
    }

    public void Scan()
    {
        view.UpdateBlasterList(new List<string> { "Blaster1" });
    }

    public void ConnectToServer(string name)
    {
        model.Name = name;
        tcpClientService.Connect(ITcpClientService.Connections.Server, "Todo", 1234);

    }
}
