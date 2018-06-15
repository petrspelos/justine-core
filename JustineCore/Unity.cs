using JustineCore.Configuration;
using JustineCore.Discord.Features.RPG;
using JustineCore.Discord.Providers.TutorialBots;
using JustineCore.Language;
using JustineCore.Storage;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace JustineCore
{
    internal static class Unity
    {
        private static UnityContainer _container;

        public static UnityContainer Container
        {
            get
            {
                if (_container == null)
                    RegisterTypes();
                return _container;
            }
        }

        public static void RegisterTypes()
        {
            _container = new UnityContainer();
            _container.RegisterType<ILocalization, JsonLocalization>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IDataStorage, JsonLocalStorage>(new ContainerControlledLifetimeManager());
            _container.RegisterType<AppConfig>(new ContainerControlledLifetimeManager());
            _container.RegisterType<RpgRepository>(new ContainerControlledLifetimeManager());
            _container.RegisterType<Discord.Connection>(new ContainerControlledLifetimeManager());
            _container.RegisterType<VerificationProvider>(new ContainerControlledLifetimeManager());
        }

        public static T Resolve<T>()
        {
            return (T)Container.Resolve(typeof(T), string.Empty, new CompositeResolverOverride());
        }
    }
}
