using System.Net.Http;
using System.Threading.Tasks;

namespace AuthenticationHandler.App
{
    public class SecuredServiceClient
    {
        private readonly HttpClient _client;

        public SecuredServiceClient(HttpClient client)
        {
            _client = client;
        }

        public Task<HttpResponseMessage> SendAsync()
        {
            return _client.GetAsync("/");
        }
    }
}