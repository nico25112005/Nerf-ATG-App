using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Installers/ProjectInstaller")]
public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
{
    public override void InstallBindings()
    {


        Debug.LogWarning("ProjectInstaller");
        Container.Bind<IPlayerModel>().To<PlayerModel>().AsSingle();
        Container.Bind<IGameModel>().To<GameModel>().AsSingle();
        Container.Bind<IServerModel>().To<ServerModel>().AsSingle();

        Container.Bind<ITcpClientService>().To<FakeTcpClientService>().AsSingle();
    }
}