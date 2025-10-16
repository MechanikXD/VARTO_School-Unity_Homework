using Core.Audio;
using Core.DataBase;
using Zenject;

namespace Core
{
    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<FirebaseProxy>().FromComponentInHierarchy().AsSingle();
            Container.Bind<AudioController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<string>().FromInstance("This text will be injected in bullets").AsSingle();
        }
    }
}