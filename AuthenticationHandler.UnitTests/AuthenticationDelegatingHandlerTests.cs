using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace AuthenticationHandler.UnitTests
{
    public class AuthenticationDelegatingHandlerTests
    {
        private TokenResponse GoodTokenResponse =>
            new TokenResponse{AccessToken = "blah", ExpirationInSeconds = 360, Scheme = "Bearer-blah"};

        private TokenErrorResponse BadTokenResponse =>
            new TokenErrorResponse {Error = "Boom!"};

        private HttpClient GoodTokenClient =>
            CreateClient(HttpStatusCode.OK, GoodTokenResponse, TestUrl);

        private HttpClient BadTokenClient =>
            CreateClient(HttpStatusCode.BadRequest, BadTokenResponse, TestUrl);

        private ClientCredentials DefaultClientCredentials([CallerMemberName] string clientId = "") =>
            new ClientCredentials{ClientId = clientId};

        private const string TestUrl = "http://example.com";

        [Fact]
        public async Task GoodToken()
        {
            var delegatingHandler = new AuthenticationDelegatingHandler(
                new AccessTokensCacheManager(),
                DefaultClientCredentials(),
                GoodTokenClient);

            var requestHttpClient = CreateTestClient(delegatingHandler);
            
            var response = await requestHttpClient.SendAsync(new HttpRequestMessage());

            Assert.NotNull(response);
        }

        [Fact]
        public async Task BadToken()
        {
            var delegatingHandler = new AuthenticationDelegatingHandler(
                new AccessTokensCacheManager(),
                DefaultClientCredentials(),
                BadTokenClient);

            var requestHttpClient = CreateTestClient(delegatingHandler);
            
            var ex = await Assert.ThrowsAsync<AuthenticationHandlerException>(
                () => requestHttpClient.SendAsync(new HttpRequestMessage()));

            Assert.Contains("Error details: Boom!", ex.Message);
        }

        private HttpClient CreateTestClient(DelegatingHandler authenticationHandler)
        {
            authenticationHandler.InnerHandler = new FakeHttpClientHandler<object>(HttpStatusCode.OK)
            {
                AssertAuthorizationHeader = true
            };
            var client = new HttpClient(authenticationHandler) {BaseAddress = new Uri(TestUrl)};
            return client;
        }

        private HttpClient CreateClient<T>(HttpStatusCode statusCode, T content, string baseUri = null)  where T : class 
        {
            var handler = new FakeHttpClientHandler<T>(statusCode, content);
            var client = new HttpClient(handler);

            if (baseUri != null)
                client.BaseAddress = new Uri(baseUri);

            return client;
        }

        private class FakeHttpClientHandler<T> : HttpClientHandler where T : class 
        {
            public bool AssertAuthorizationHeader { get; set; }

            private readonly HttpStatusCode _statusCode;
            private readonly string _stringContent;

            public FakeHttpClientHandler(HttpStatusCode statusCode, T content = default(T)) 
            {
                _statusCode = statusCode;
                if(content != default(T))
                    _stringContent = JsonConvert.SerializeObject(content);
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (AssertAuthorizationHeader)
                {
                    Assert.Equal("Bearer-blah", request.Headers.Authorization.Scheme);
                    Assert.Equal("blah", request.Headers.Authorization.Parameter);
                }

                var httpResponseMessage = new HttpResponseMessage(_statusCode);
                if(_stringContent != null)
                    httpResponseMessage.Content = new StringContent(_stringContent);

                var requestMessage = new HttpRequestMessage {RequestUri = new Uri(TestUrl)};
                httpResponseMessage.RequestMessage = requestMessage;

                return Task.FromResult(httpResponseMessage);
            }
        }
    }
}