//using Application.Dootax.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.General
{
    [ApiController]
    [Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("DootaxJobs/Dootax")]
    public class DootaxController : Controller
    {
        //private readonly IDootaxService _dootaxService;

        //public DootaxController(IDootaxService dootaxService) =>
        //    _dootaxService = dootaxService;

        //[HttpPost("ImportFiles")]
        //public async Task<ActionResult> ImportFiles()
        //{
            //var result = await _dootaxService.ImportFilesUpload();

            //return Ok($"Records integrated successfully.");
        //}
    }
}
