
using System.Collections.Generic;

public interface IGameMemberView
{
    void UpdateMemberList(List<PlayerInfo> gameMembers);

    void ActivateHostPanel();

    void MoveToNextScene();
}
