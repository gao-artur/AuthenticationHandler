#if NET471

using SimpleInjector;

namespace AuthenticationHandler.Extensions.Framework
{
    public static class SimpleInjectorExtensions
    {
        public static Container RegisterAuthorizedHttpClientFactory(this Container container)
        {
            container.RegisterSingleton<AccessTokensCacheManager>();
            container.RegisterSingleton<IAuthorizedHttpClientFactory, AuthorizedHttpClientFactory>();
            return container;
        }
    }
}

#endif