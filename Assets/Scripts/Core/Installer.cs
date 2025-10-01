using Core.Audio;
using Core.DataBase;
using Zenject;

namespace Core
{
    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<AudioController>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<FirebaseProxy>().FromComponentInHierarchy().AsSingle().NonLazy();
        }
    }
}