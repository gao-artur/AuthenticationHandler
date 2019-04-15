using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationHandler
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private const string TokenEndpoint = "connect/token";

        private readonly ClientCredentials _clientCredentials;
        private readonly HttpClient _accessControlHttpClient;
        private readonly AccessTokensCacheManager _accessTokensCacheManager;

        public AuthenticationDelegatingHandler(
            AccessTokensCacheManager accessTokensCacheManager,
            ClientCredentials clientCredentials,
            string identityAuthority)
            : this(accessTokensCacheManager,
                clientCredentials,
                new HttpClient {BaseAddress = new Uri(identityAuthority)})
        {
        }

        public AuthenticationDelegatingHandler(
            AccessTokensCacheManager accessTokensCacheManager,
            ClientCredentials clientCredentials,
            HttpClient accessControlHttpClient)
        {
            _accessTokensCacheManager = accessTokensCacheManager;
            _clientCredentials = clientCredentials;
            _accessControlHttpClient = accessControlHttpClient;

            if (_accessControlHttpClient.BaseAddress == null)
            {
                throw new AuthenticationHandlerException($"{nameof(HttpClient.BaseAddress)} should be set to Identity Server url");
            }

            if (_accessControlHttpClient.BaseAddress?.AbsoluteUri.EndsWith("/") == false)
            {
                _accessControlHttpClient.BaseAddress = new Uri(_accessControlHttpClient.BaseAddress.AbsoluteUri + "/");
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await GetToken();

            request.Headers.Authorization = new AuthenticationHeaderValue(token.Scheme, token.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<TokenResponse> GetToken()
        {
            var token = _accessTokensCacheManager.GetToken(_clientCredentials.ClientId);
            if (token == null)
            {
                token = await GetNewToken(_clientCredentials);
                _accessTokensCacheManager.AddOrUpdateToken(_clientCredentials.ClientId, token);
            }

            return token;
        }

        private async Task<TokenResponse> GetNewToken(ClientCredentials credentials)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint))
            {
                request.Content = new FormUrlEncodedContent(new []
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"), 
                    new KeyValuePair<string, string>("client_id", credentials.ClientId), 
                    new KeyValuePair<string, string>("client_secret", credentials.ClientSecret), 
                    new KeyValuePair<string, string>("scope", credentials.Scopes)
                });

                var response = await _accessControlHttpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var tokenResponse = await response.DeserializeAsync<TokenResponse>();
                    return tokenResponse;
                }

                var errorMessage = await GetErrorMessageAsync(response);
                throw new AuthenticationHandlerException(errorMessage);
            }
        }

        private async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
        {
            var baseErrorMessage =
                $"Error occured while trying to get access token from identity authority {response.RequestMessage.RequestUri}.";

            var errorMessage = baseErrorMessage;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.DeserializeAsync<TokenErrorResponse>();
                errorMessage = $"{errorMessage} Error details: {errorResponse.Error}";
            }
            else
            {
                errorMessage = $"{errorMessage} Status code: {(int)response.StatusCode} - {response.StatusCode}";
            }

            return errorMessage;
        }
    }
}