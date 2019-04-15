#if NETSTANDARD2_0

using System;
using System.Net.Http;
using AuthenticationHandler;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ***************************************************************************************
// DO NOT CHANGE THE NAMESPACE! It is .Net Core convention for configuration extensions. *
// ***************************************************************************************
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddAuthentication(this IHttpClientBuilder builder,
            Func<IServiceProvider, ClientCredentials> credentialsProvider,
            Func<IServiceProvider, string> identityAuthorityProvider)
        {
            builder.Services.TryAddSingleton<AccessTokensCacheManager>();
            builder.AddHttpMessageHandler(provider =>
            {
                var credentials = credentialsProvider.Invoke(provider);
                var identityAuthority = identityAuthorityProvider.Invoke(provider);

                return CreateDelegatingHandler(provider, credentials, identityAuthority);
            });

            return builder;
        }

        public static IHttpClientBuilder AddAuthentication(this IHttpClientBuilder builder, ClientCredentials credentials, string identityAuthority)
        {
            builder.Services.TryAddSingleton<AccessTokensCacheManager>();
            builder.AddHttpMessageHandler(provider => CreateDelegatingHandler(provider, credentials, identityAuthority));
            
            return builder;
        }

        private static AuthenticationDelegatingHandler CreateDelegatingHandler(IServiceProvider provider, 
            ClientCredentials credentials, string identityAuthority)
        {
            var httpClient = CreateHttpClient(provider, identityAuthority);
            var accessTokensCacheManager = provider.GetRequiredService<AccessTokensCacheManager>();

            return new AuthenticationDelegatingHandler(accessTokensCacheManager, credentials, httpClient);
        }

        private static HttpClient CreateHttpClient(IServiceProvider provider, string identityAuthority)
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(identityAuthority);

            return httpClient;
        }
    }
}
#endif