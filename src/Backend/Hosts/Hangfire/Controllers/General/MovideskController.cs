using Application.Movidesk.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.General
{
    [ApiController]
    //[Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("MovideskJobs/Movidesk")]
    public class MovideskController : Controller
    {
        private readonly IMovideskTicketsService _movideskTicketsService;
        private readonly IMovideskPeopleService _movideskPeopleService;
        private readonly IMovideskServicesService _movideskServicesService;

        public MovideskController(IMovideskTicketsService movideskTicketsService, IMovideskPeopleService movideskPeopleService, IMovideskServicesService movideskServicesService) =>
            (_movideskTicketsService, _movideskPeopleService, _movideskServicesService) = (movideskTicketsService, movideskPeopleService, movideskServicesService);

        [HttpGet("Tickets")]
        [Tags("MovideskTickets")]
        public async Task<ActionResult> Tickets()
        {
            var result = await _movideskTicketsService.GetTickets();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// get one ticket by number (GET tickets?id={id}) and save it; the call is logged in general.MovideskRequests
        /// </summary>
        [HttpGet("Tickets/{docCompany}/{id:long:min(1)}")]
        [Tags("MovideskTickets")]
        public async Task<ActionResult> Ticket(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            long id
        )
        {
            var result = await _movideskTicketsService.GetTicketById(userName, docCompany, id);

            return result switch
            {
                true => Ok($"Ticket {id} integrated successfully."),
                false => NotFound($"Ticket {id} not found in Movidesk."),
                null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
            };
        }

        [HttpGet("People")]
        [Tags("MovideskPeople")]
        public async Task<ActionResult> People()
        {
            var result = await _movideskPeopleService.GetPeople();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// get one person by Cód. Ref. (GET persons?id={id}) and save it; the call is logged in general.MovideskRequests
        /// </summary>
        [HttpGet("People/{docCompany}/{id}")]
        [Tags("MovideskPeople")]
        public async Task<ActionResult> Person(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(64, MinimumLength = 1), RegularExpression(@"^[A-Za-z0-9]+$")] string id
        )
        {
            var result = await _movideskPeopleService.GetPersonById(userName, docCompany, id);

            return result switch
            {
                true => Ok($"Person {id} integrated successfully."),
                false => NotFound($"Person {id} not found in Movidesk."),
                null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
            };
        }

        [HttpGet("Services")]
        [Tags("MovideskServices")]
        public async Task<ActionResult> Services()
        {
            var result = await _movideskServicesService.GetServices();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// get one service by id (GET services?id={id}) and save it; the call is logged in general.MovideskRequests
        /// </summary>
        [HttpGet("Services/{docCompany}/{id:int:min(1)}")]
        [Tags("MovideskServices")]
        public async Task<ActionResult> Service(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int id
        )
        {
            var result = await _movideskServicesService.GetServiceById(userName, docCompany, id);

            return result switch
            {
                true => Ok($"Service {id} integrated successfully."),
                false => NotFound($"Service {id} not found in Movidesk."),
                null => UnprocessableEntity("Company not registered or invalid response from Movidesk. See the execution log.")
            };
        }
    }
}
