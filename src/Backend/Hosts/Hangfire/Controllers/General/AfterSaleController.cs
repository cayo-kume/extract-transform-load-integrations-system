using Application.AfterSale.Abstractions.Services;
using Application.AfterSale.Services;
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
    [Route("AfterSaleJobs/AfterSale")]
    public class AfterSaleController : Controller
    {
        private readonly IAfterSaleMeService _afterSaleMeService;
        private readonly IAfterSaleReversesService _afterSaleReversesService;
        private readonly IAfterSaleRefundsService _afterSaleRefundsService;

        public AfterSaleController(
            IAfterSaleMeService afterSaleService,
            IAfterSaleReversesService afterSaleReversesService,
            IAfterSaleRefundsService afterSaleRefundsService
        ) =>
        (
            _afterSaleMeService,
            _afterSaleReversesService,
            _afterSaleRefundsService
        ) 
        = 
        (
            afterSaleService,
            afterSaleReversesService,
            afterSaleRefundsService
        );

        [HttpPost("Me")]
        [Tags("AfterSaleEcommerces")]
        public async Task<ActionResult> Me()
        {
            var result = await _afterSaleMeService.GetMe();

            return Ok($"Records integrated successfully.");
        }

        [HttpPost("Reverses")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> Reverses()
        {
            var result = await _afterSaleReversesService.GetReverses();

            return Ok($"Records integrated successfully.");
        }

        [HttpPost("Reverse")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> ReversesById()
        {
            var result = await _afterSaleReversesService.GetReverse();

            return Ok($"Records integrated successfully.");
        }

        [HttpPost("Status")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> ReversesStatus()
        {
            var result = await _afterSaleReversesService.GetStatus();

            return Ok($"Records integrated successfully.");
        }

        [HttpPost("Transportations")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> Transportations()
        {
            var result = await _afterSaleReversesService.GetTransportations();

            return Ok($"Records integrated successfully.");
        }

        [HttpPost("CourierAttributes")]
        [Tags("AfterSaleReverses")]
        public async Task<ActionResult> CourierAttributes()
        {
            var result = await _afterSaleReversesService.GetCourierAttributes();

            return Ok($"Records integrated successfully.");
        }
    }
}
