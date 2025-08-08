using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface ISelectGameView
{
    void UpdateGameList(GameInfo gameInfo);
    void ShowToastMessage(string message, string icon);
    void LoadNextScene();
}

