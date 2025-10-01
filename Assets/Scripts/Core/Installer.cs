using Other;
using Zenject;

namespace Core
{
    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Put message in container for greeter to say
            Container.Bind<string>().FromInstance("Hello World!").AsCached();
            // But greeter itself, so it will be created on injection
            Container.Bind<Greeter>().FromNewComponentOnNewGameObject().AsSingle();
        }
    }
}