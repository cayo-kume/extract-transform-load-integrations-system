using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Text;

namespace WebApi.Controllers
{
    [ApiController]
#if DEBUG
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("Api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration) =>
            _configuration = configuration;

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token()
        {
            var authHeader = Request.Headers.Authorization;
            var authHeaderInfo = authHeader.ToString().Split(' ');
            var base64TokenInfo = Convert.FromBase64String(authHeaderInfo[1]);
            var base64DecodedIdAndSecret = Encoding.UTF8.GetString(base64TokenInfo);

            var app = ConfidentialClientApplicationBuilder
                        .Create(base64DecodedIdAndSecret.Split(":")[0])
                        .WithClientSecret(base64DecodedIdAndSecret.Split(":")[1])
                        .WithAuthority($"{_configuration.GetValue<string>("AzureAd:Instance")}{_configuration.GetValue<string>("AzureAd:TenantId")}")
                        .Build();

            var tokenResult = await app.AcquireTokenForClient(new List<string> { "api://438e5dd3-6937-46fd-8684-9020ed3ffdb0/.default" }).ExecuteAsync();

            return Ok(new { access_token = tokenResult.AccessToken });
        }
    }
}
