#if NET471

using System;
using System.Net.Http;

namespace AuthenticationHandler.Extensions.Framework
{
    internal class AuthorizedHttpClientFactory : IAuthorizedHttpClientFactory
    {
        private readonly AccessTokensCacheManager _cacheManager;

        public AuthorizedHttpClientFactory(AccessTokensCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public HttpClient CreateClient(string authorityUrl, string clientId, string secret, string scopes)
        {
            var credentials = new ClientCredentials
            {
                ClientId = clientId,
                ClientSecret = secret,
                Scopes = scopes
            };

            var accessControlClient = new HttpClient { BaseAddress = new Uri(authorityUrl) }; 

            var authHandler = new AuthenticationDelegatingHandler(_cacheManager, credentials, accessControlClient)
            {
                InnerHandler = new HttpClientHandler()
            };

            return new HttpClient(authHandler, disposeHandler: true);
        }
    }
}

#endif