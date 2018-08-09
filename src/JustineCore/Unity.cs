using Discord.WebSocket;
using JustineCore.Configuration;
using JustineCore.Discord.Features.RPG;
using JustineCore.Discord.Providers.TutorialBots;
using JustineCore.Language;
using JustineCore.Storage;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;
using Unity.Injection;
using JustineCore.Discord;
using JustineCore.Discord.Features.TutorialServer;

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
            // _container.RegisterType<ILocalization, JsonLocalization>(new ContainerControlledLifetimeManager());
            // _container.RegisterType<IDataStorage, JsonLocalStorage>(new ContainerControlledLifetimeManager());
            // _container.RegisterType<AppConfig>(new ContainerControlledLifetimeManager());
            // _container.RegisterType<RpgRepository>(new ContainerControlledLifetimeManager());
            // _container.RegisterType<Discord.Connection>(new ContainerControlledLifetimeManager());
            // _container.RegisterType<VerificationProvider>(new ContainerControlledLifetimeManager());

            _container.RegisterSingleton<ILocalization, JsonLocalization>();
            _container.RegisterSingleton<IDataStorage, JsonLocalStorage>();
            _container.RegisterSingleton<AppConfig>();
            _container.RegisterSingleton<RpgRepository>();
            _container.RegisterSingleton<Discord.Connection>();
            _container.RegisterSingleton<VerificationProvider>();
            _container.RegisterSingleton<WaitingRoomService>();
            _container.RegisterSingleton<ProblemBoardService>();
            _container.RegisterSingleton<ProblemProvider>();
            _container.RegisterType<DiscordSocketConfig>(new InjectionFactory(c => DiscordSocketConfigFactory.GetDefault()));
            _container.RegisterSingleton<DiscordSocketClient>(new InjectionConstructor(typeof(DiscordSocketConfig)));
        }

        public static T Resolve<T>()
        {
            return (T)Container.Resolve(typeof(T), string.Empty, new CompositeResolverOverride());
        }
    }
}
