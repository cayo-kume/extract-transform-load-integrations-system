//using Application.TotalExpress.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.Carries
{
    [ApiController]
    [Authorize]
    [Route("CarriersJobs/TotalExpress")]
    public class TotalExpressController : Controller
    {
        //Refatorar Aqui (adicionar retorno json usando o middleware de erros)
        //private readonly ITotalExpressService _totalExpressService;

        //public TotalExpressController(ITotalExpressService totalExpressService) =>
        //    (_totalExpressService) = (totalExpressService);

//        [HttpPost("GetSendOrder")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotalExpressGetSendOrder([Required][FromQuery] string? orderNumber)
//        {
//            var result = await _totalExpressService.GetSendOrder(orderNumber);
//            return Ok(result);
//        }

//        [HttpPost("SendOrders")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotalExpressSendOrders()
//        {
//            await _totalExpressService.SendOrders();
//            return Ok();
//        }

//        [HttpPost("SendOrder")]
//        public async Task<ActionResult<string?>> TotalExpressSendOrder([Required][FromQuery] string? orderNumber)
//        {
//            var result = await _totalExpressService.SendOrder(orderNumber.Trim().ToUpper());

//            return Ok($"Pedido: {orderNumber} enviado com sucesso.");
//        }

//        [HttpPost("SendOrderAsETUR")]
//        public async Task<ActionResult<string?>> TotalExpressSendOrderAsETUR([Required][FromQuery] string? orderNumber)
//        {
//            var result = await _totalExpressService.SendOrderAsEtur(orderNumber.Trim().ToUpper());

//            return Ok($"Pedido: {orderNumber} enviado com sucesso.");
//        }

//        [HttpPost("UpdateLogOrders")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotalExpressUpdateLogOrders()
//        {
//            await _totalExpressService.InsertLogOrdersByAWBs();

//            return Ok();
//        }
    }
}
