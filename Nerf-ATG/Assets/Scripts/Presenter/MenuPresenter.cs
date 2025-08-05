using Game;
using System.Collections.Generic;

public class MenuPresenter
{
    private readonly IMenuView view;
    private readonly IPlayerModel playerModel;
    private readonly ITcpClientService tcpClientService;
    //private readonly WiFiBridge wifiBridge = WiFiBridge.Instance;

    public MenuPresenter(IMenuView view, IPlayerModel playerModel, ITcpClientService tcpClientService)
    {
        this.view = view;
        this.playerModel = playerModel;
        this.tcpClientService = tcpClientService;

        this.tcpClientService.ConnectionStatusChanged += TcpConnectionChanged;
    }


    public void UpdateBlasterList(List<string> blasters)
    {
        view.UpdateBlasterList(blasters);
    }

    public void Connect()
    {
        //tcpClientService.Connect(ITcpClientService.Connections.ESP32, "Todo", 1234);
    }

    public void Scan()
    {
        view.UpdateBlasterList(new List<string> { "Blaster1" });
    }

    public void ConnectToServer(string name)
    {
        playerModel.Name = name;
        tcpClientService.Connect(ITcpClientService.Connections.Server, Settings.ServerIP, Settings.ServerPort);

    }

    public void TcpConnectionChanged(object sender, bool connected)
    {
        tcpClientService.Send(ITcpClientService.Connections.Server, new ConnectToServer(playerModel.Id, playerModel.Name, PacketAction.Generic));
        view.LoadNextScene();
    }

    public void Dispose()
    {
        tcpClientService.ConnectionStatusChanged -= TcpConnectionChanged;
    }
}
