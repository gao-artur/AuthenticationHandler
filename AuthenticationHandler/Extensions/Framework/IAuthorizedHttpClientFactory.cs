#if NET471

using System.Net.Http;

namespace AuthenticationHandler.Extensions.Framework
{
    public interface IAuthorizedHttpClientFactory
    {
        /// <summary>
        /// Creates new HttpClient on each call. On executing request it will try to reuse cached token previously acquired
        /// from Identity Serer, if no cached token found or it expired will try to acquire new token and cache it.
        /// The token will be set into Authorize header on each request.
        /// You can provide more than one required scope by delimiting them with spaces.
        /// </summary>
        /// <param name="authorityUrl">Identity Server url</param>
        /// <param name="clientId">client_id</param>
        /// <param name="secret">client_secret</param>
        /// <param name="scopes">scope</param>
        /// <returns></returns>
        HttpClient CreateClient(string authorityUrl, string clientId, string secret, string scopes);
    }
}

#endif