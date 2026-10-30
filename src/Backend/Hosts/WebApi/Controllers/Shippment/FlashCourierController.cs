using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.Shippment
{
    [ApiController]
    [Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("CarriersJobs/FlashCourier")]
    public class FlashCourierController : Controller
    {
        //private readonly IFlashCourierService _flashCourierService;

        //public FlashCourierController(IFlashCourierService flashCourierService) =>
        //    (_flashCourierService) = (flashCourierService);

        //[HttpPost("SendOrders")]
        //public async Task<ActionResult<string?>> FlashCourierSendOrders()
        //{
        //    await _flashCourierService.SendOrders();
        //    return Ok();
        //}

        //[HttpPost("SendOrder")]
        //public async Task<ActionResult<string?>> FlashCourierSendOrder([Required][FromQuery] string? orderNumber)
        //{
        //    var result = await _flashCourierService.SendOrder(orderNumber.Trim().ToUpper());

        //    //return BadRequest($"A API FlashCourier não conseguiu enviar o pedido: {orderNumber}.");
        //    return Ok($"Pedido: {orderNumber} enviado com sucesso.");
        //}

        //[HttpPost("UpdateLogOrders")]
        //public async Task<ActionResult<string?>> FlashCourierUpdateLogOrders()
        //{
        //    await _flashCourierService.UpdateLogOrders();

        //    return Ok();
        //}
    }
}
