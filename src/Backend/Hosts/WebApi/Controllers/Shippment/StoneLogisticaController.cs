//using Application.Stone.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.Shippment
{
    [ApiController]
    //[Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("CarriersJobs/StoneLogistica")]
    public class StoneLogisticaController : Controller
    {
        //private readonly IStoneService _stoneService;

        //public StoneLogisticaController(IStoneService stoneService) =>
        //    (_stoneService) = (stoneService);

        //[HttpPost("StoneLogisticaCheckDeliveryOrdersToIntegrityLock")]
        //public async Task<ActionResult<string?>> StoneLogisticaCheckDeliveryOrdersToIntegrityLock()
        //{
        //    var result = await _stoneService.CheckDeliveryOrdersIntegrityLock();
        //    return Ok(result);
        //}

        //[HttpPost("StoneLogisticaCheckDeliveryOrders")]
        //public async Task<ActionResult<string?>> StoneLogisticaCheckDeliveryOrders()
        //{
        //    var result = await _stoneService.CheckDeliveryOrders();
        //    return Ok(result);
        //}

        //[HttpPost("StoneLogisticaCheckDeliveryOrder")]
        //public async Task<ActionResult<string?>> StoneLogisticaCheckDeliveryOrder([Required][FromQuery] string orderNumber)
        //{
        //    var result = await _stoneService.CheckDeliveryOrder(orderNumber);
        //    return Ok(result);
        //}

        //[HttpPost("StoneLogisticaSendOrders")]
        //public async Task<ActionResult<string?>> StoneLogisticaSendOrders()
        //{
        //    var result = await _stoneService.CreateDeliveryOrder();
        //    return Ok(result);
        //}

        //[HttpPost("StoneLogisticaGetZplLabels")]
        //public async Task<ActionResult<string?>> StoneLogisticaGetZplLabels()
        //{
        //    var result = await _stoneService.GetZplLabels();
        //    return Ok(result);
        //}

        //[HttpPost("StoneLogisticaCancelOrders")]
        //public async Task<ActionResult<string?>> StoneLogisticaCancelOrders([Required][FromQuery] string? orderNumber)
        //{
        //    var result = await _stoneService.CancelOrder(orderNumber);
        //    return Ok(result);
        //}
    }
}
