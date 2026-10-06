//using Application.Jadlog.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.Shippment
{
    [ApiController]
    [Authorize]
    [Route("CarriersJobs/JadLog")]
    public class JadLogController : Controller
    {
        //private readonly IJadlogService _jadlogService;

        //public JadLogController(IJadlogService jadlogService) =>
        //    _jadlogService = jadlogService;

//        [HttpPost("SearchTrackingHistory")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> SearchTrackingHistory()
//        {
//            await _jadlogService.SearchTrackingHistory();

//            return Ok();
//        }

//        [HttpPost("SendOrders")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> SendOrders()
//        {
//            await _jadlogService.InsertOrder();

//            return Ok();
//        }
    }
}
