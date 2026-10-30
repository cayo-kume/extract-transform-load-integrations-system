//using Application.Oto.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.General
{
    [ApiController]
    [Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("OttoJobs/Otto")]
    public class OttoController : Controller
    {
        //private readonly IOtoService _otoService;

        //public OttoController(IOtoService otoService) =>
        //    _otoService = otoService;

        ////[HttpPost("Customer")]
        ////public async Task<ActionResult> Customer([Required][FromQuery] string doc_client)
        ////{
        ////    try
        ////    {
        ////        var result = await _otoService.SendCustomer(doc_client);

        ////        return Ok($"Records integrated successfully.");
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Response.StatusCode = 400;
        ////        return Content($"Erro: {ex.Message}");
        ////    }
        ////}

        //[HttpPost("Customers")]
        //public async Task<ActionResult> Customers()
        //{
        //    try
        //    {
        //        var result = await _otoService.SendCustomers();

        //        return Ok($"Records integrated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;
        //        return Content($"Erro: {ex.Message}");
        //    }
        //}

        //[HttpPost("CustomersCashBack")]
        //public async Task<ActionResult> CustomersCashBack()
        //{
        //    try
        //    {
        //        var result = await _otoService.SendCustomersCashBack();

        //        return Ok($"Records integrated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;
        //        return Content($"Erro: {ex.Message}");
        //    }
        //}

        ////[HttpPost("Product")]
        ////public async Task<ActionResult> Product([Required][FromQuery] int cod_product)
        ////{
        ////    try
        ////    {
        ////        var result = await _otoService.SendProduct(cod_product);

        ////        return Ok($"Records integrated successfully.");
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Response.StatusCode = 400;
        ////        return Content($"Erro: {ex.Message}");
        ////    }
        ////}

        //[HttpPost("Products")]
        //public async Task<ActionResult> Products()
        //{
        //    try
        //    {
        //        var result = await _otoService.SendProducts();

        //        return Ok($"Records integrated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;
        //        return Content($"Erro: {ex.Message}");
        //    }
        //}

        ///// <summary>
        ///// se pedido for venda facil, informar a chave nfe no lugar do código do pedido
        ///// </summary>
        ///// <param name="order_number"></param>
        ///// <returns></returns>
        ////[HttpPost("Order")]
        ////public async Task<ActionResult> Order([Required][FromQuery] string order_number)
        ////{
        ////    try
        ////    {
        ////        var result = await _otoService.SendOrder(order_number);

        ////        return Ok($"Records integrated successfully.");
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Response.StatusCode = 400;
        ////        return Content($"Erro: {ex.Message}");
        ////    }
        ////}

        //[HttpPost("Orders")]
        //public async Task<ActionResult> Orders()
        //{
        //    try
        //    {
        //        var result = await _otoService.SendOrders();

        //        return Ok($"Records integrated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;
        //        return Content($"Erro: {ex.Message}");
        //    }
        //}

        ////[HttpPost("Seller")]
        ////public async Task<ActionResult> Seller([Required][FromQuery] int cod_seller)
        ////{
        ////    try
        ////    {
        ////        var result = await _otoService.SendSeller(cod_seller);

        ////        return Ok($"Records integrated successfully.");
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Response.StatusCode = 400;
        ////        return Content($"Erro: {ex.Message}");
        ////    }
        ////}

        //[HttpPost("Sellers")]
        //public async Task<ActionResult> Sellers()
        //{
        //    try
        //    {
        //        var result = await _otoService.SendSellers();

        //        return Ok($"Records integrated successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 400;
        //        return Content($"Erro: {ex.Message}");
        //    }
        //}

        ////[HttpPost("Company")]
        ////public async Task<ActionResult> Company([Required][FromQuery] string cnpj_emp)
        ////{
        ////    try
        ////    {
        ////        var result = await _otoService.SendStore(cnpj_emp);

        ////        return Ok($"Records integrated successfully.");
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Response.StatusCode = 400;
        ////        return Content($"Erro: {ex.Message}");
        ////    }
        ////}

        //[HttpPost("Companys")]
        //public async Task<ActionResult> Companys()
        //{
        //    try
        //    {
        //        var result = await _otoService.SendStores();

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
