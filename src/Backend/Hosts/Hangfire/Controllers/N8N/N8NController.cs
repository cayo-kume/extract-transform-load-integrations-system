using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.N8N
{
    [ApiController]
    [Authorize]
    //#if DEBUG
    //    [ApiExplorerSettings(IgnoreApi = true)]
    //#endif
    [Route("Newbloomers/N8N")]
    public class N8NController : Controller
    {
    }
}
