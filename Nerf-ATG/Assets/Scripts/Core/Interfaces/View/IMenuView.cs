using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IMenuView
{
    void UpdateBlasterList(List<string> blasters);
    void LoadNextScene();
}

