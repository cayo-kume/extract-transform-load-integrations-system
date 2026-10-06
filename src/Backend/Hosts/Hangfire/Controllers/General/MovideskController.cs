//using Application.Movidesk.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.General
{
    [ApiController]
    [Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("MovideskJobs/Movidesk")]
    public class MovideskController : Controller
    {
        //private readonly IMovideskService _movideskService;

        //public MovideskController(
        //    IMovideskService movideskService
        //)
        //{
        //    _movideskService = movideskService;
        //}

        //[HttpPost("Persons")]
        //public async Task<ActionResult> Persons()
        //{
        //    var result = await _movideskService.GetPersons();

        //    return Ok($"Records integrated successfully.");
        //}

        //[HttpPost("Services")]
        //public async Task<ActionResult> Services()
        //{
        //    var result = await _movideskService.GetServices();

        //    return Ok($"Records integrated successfully.");
        //}

        //[HttpPost("Tickets")]
        //public async Task<ActionResult> Tickets()
        //{
        //    var result = await _movideskService.GetTickets();

        //    return Ok($"Records integrated successfully.");
        //}

        //[HttpPost("Ticket")]
        //public async Task<ActionResult> Ticket(object response)
        //{
        //    try
        //    {
        //        var result = await _movideskService.InsertTicket(response.ToString());

        //        return Ok($"Records integrated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;
        //        return Content($"Erro: {ex.Message}");
        //    }
        //}

        //[HttpPost("UpdateTicket")]
        //public async Task<ActionResult> UpdateTicket()
        //{
        //    try
        //    {
        //        var result = await _movideskService.UpdateTicket();

        //        return Ok($"Records integrated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;
        //        return Content($"Erro: {ex.Message}");
        //    }
        //}

        //[HttpPost("BuildInvoicesFromTickets")]
        //public async Task<ActionResult> BuildInvoicesFromTickets()
        //{
        //    try
        //    {
        //        await _movideskService.BuildInvoicesFromTickets();

        //        return Ok($"Records integrated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;
        //        return Content($"Erro: {ex.Message}");
        //    }
        //}
    }
}
