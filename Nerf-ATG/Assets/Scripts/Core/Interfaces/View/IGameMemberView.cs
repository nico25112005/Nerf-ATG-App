
using System.Collections.Generic;

public interface IGameMemberView
{
    void UpdateMemberList(IPlayerInfo player);

    void ActivateHostPanel();

    void MoveToNextScene();
}
