using Newtonsoft.Json;

namespace AuthenticationHandler
{
    public class TokenErrorResponse
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}