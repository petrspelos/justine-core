using JustineCore.Language;
using JustineCore.Storage;
using JustineCore.Tests.Helpers;
using Unity;
using Unity.Resolution;

namespace JustineCore.Tests
{
    internal static class TestUnity
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
            _container.RegisterType<IDataStorage, InMemoryDataStorage>();
            _container.RegisterSingleton<ILocalization, JsonLocalization>();
        }

        public static T Resolve<T>()
        {
            return (T)Container.Resolve(typeof(T), string.Empty, new CompositeResolverOverride());
        }
    }
}
