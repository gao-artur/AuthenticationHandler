using Xunit;

namespace AuthenticationHandler.UnitTests
{
    public class AccessTokensCacheManagerTest
    {
        private readonly AccessTokensCacheManager _cacheManager;

        public AccessTokensCacheManagerTest()
        {
            _cacheManager = new AccessTokensCacheManager();
        }

        [Fact]
        public void GetToken_EmptyCache_Null()
        {
            var token = _cacheManager.GetToken("first");
            Assert.Null(token);
        }

        [Fact]
        public void AddOrUpdate_ThenGetSameClientId_NotNull()
        {
            const string client = "client1";
            var originalToken = GenerateToken(120);
            _cacheManager.AddOrUpdateToken(client, originalToken);

            var token = _cacheManager.GetToken(client);

            Assert.Equal(originalToken, token);
        }

        [Fact]
        public void AddOrUpdate_SameClientIdTwice_GetReturnsLatest()
        {
            const string client = "client1";

            var originalToken1 = GenerateToken(120);
            _cacheManager.AddOrUpdateToken(client, originalToken1);

            var originalToken2 = GenerateToken(240);
            _cacheManager.AddOrUpdateToken(client, originalToken2);

            var token = _cacheManager.GetToken(client);

            Assert.Equal(originalToken2, token);
        }

        [Fact]
        public void AddOrUpdate_ThenGetDifferentClientId_Null()
        {
            const string client1 = "client1";
            const string client2 = "client2";
            var originalToken = GenerateToken(120);
            _cacheManager.AddOrUpdateToken(client1, originalToken);

            var token = _cacheManager.GetToken(client2);

            Assert.Null(token);
        }

        [Fact]
        public void Get_TokenExpired_Null()
        {
            const string client1 = "client1";
            var originalToken = GenerateToken(-120);
            _cacheManager.AddOrUpdateToken(client1, originalToken);

            var token = _cacheManager.GetToken(client1);

            Assert.Null(token);
        }

        [Fact]
        public void AddOrUpdate_DifferentClientIds_GetDifferentTokens()
        {
            const string client1 = "client1";
            const string client2 = "client2";

            var originalToken1 = GenerateToken(120);
            _cacheManager.AddOrUpdateToken(client1, originalToken1);

            var originalToken2 = GenerateToken(240);
            _cacheManager.AddOrUpdateToken(client2, originalToken2);

            var token1 = _cacheManager.GetToken(client1);
            Assert.Equal(originalToken1, token1);

            var token2 = _cacheManager.GetToken(client2);
            Assert.Equal(originalToken2, token2);
        }
        
        private TokenResponse GenerateToken(long expirationInSeconds)
        {
            var token = new TokenResponse
            {
                Scheme = "Bearer",
                AccessToken = "blah-blah",
                ExpirationInSeconds = expirationInSeconds
            };

            return token;
        }
    }
}