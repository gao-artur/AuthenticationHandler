using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationHandler.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        private readonly SecuredServiceClient _client;

        public ExampleController(SecuredServiceClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _client.SendAsync());
        }

    }
}
