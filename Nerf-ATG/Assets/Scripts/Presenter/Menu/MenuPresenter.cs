using Assets.Scripts.UI.Model;
using Assets.Scripts.UI.View.Interfaces;
using System;
using System.Collections.Generic;

public class MenuPresenter
{
    private readonly IMenuView view;
    private readonly IModel model;
    private readonly ITcpClientService tcpClientService;

    public MenuPresenter(IMenuView view, IModel model, ITcpClientService tcpClientService)
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

        tcpClientService.Connect("Blaster", "Todo", 1234);
    }

    public void Scan()
    {

        view.UpdateBlasterList(new List<string> { });
    }

    public void ConnectToServer(string name)
    {
        model.Name = name;
        tcpClientService.Connect("Server", "Todo", 1234);

    }
}
