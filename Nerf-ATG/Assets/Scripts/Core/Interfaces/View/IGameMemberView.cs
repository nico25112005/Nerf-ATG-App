
using System.Collections.Generic;

public interface IGameMemberView
{
    void UpdateMemberList(List<IPlayerInfo> gameMembers);

    void ActivateHostPanel();

    void MoveToNextScene();
}
