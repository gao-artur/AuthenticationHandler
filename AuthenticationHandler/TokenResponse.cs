using Newtonsoft.Json;

namespace AuthenticationHandler
{
    public class TokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string Scheme { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public long ExpirationInSeconds { get; set; }
    }
}