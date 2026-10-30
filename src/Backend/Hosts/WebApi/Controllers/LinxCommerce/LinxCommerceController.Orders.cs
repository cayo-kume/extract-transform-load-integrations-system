using Application.LinxCommerce.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.LinxCommerce
{
    /// <summary>
    /// rotas do LinxCommerceOrdersService: pedidos e status de pedido. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxCommerceController
    {
        /// <summary>
        /// sincroniza os pedidos alterados desde a última leitura (Sales SearchOrders + GetOrder)
        /// </summary>
        [HttpPost("SearchOrders")]
        [Tags("LinxCommerceOrders")]
        public async Task<ActionResult> SearchOrders([FromServices] ILinxCommerceOrdersService linxCommerceOrdersService)
        {
            var result = await linxCommerceOrdersService.SearchOrders();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os status de pedido (Sales SearchOrderStatus)
        /// </summary>
        [HttpPost("SearchOrderStatus")]
        [Tags("LinxCommerceOrders")]
        public async Task<ActionResult> SearchOrderStatus([FromServices] ILinxCommerceOrdersService linxCommerceOrdersService)
        {
            var result = await linxCommerceOrdersService.SearchOrderStatus();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// busca um pedido pelo OrderID (Sales GetOrder) e grava; a chamada fica em linxcommerce.LinxCommerceRequests
        /// </summary>
        [HttpGet("Orders/{docCompany}/{orderId:guid}")]
        [Tags("LinxCommerceOrders")]
        public async Task<ActionResult> Order(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            Guid orderId,
            [FromServices] ILinxCommerceOrdersService linxCommerceOrdersService
        ) =>
            OnDemandResponse(await linxCommerceOrdersService.GetOrder(userName, docCompany, orderId), $"Order {orderId}");

        /// <summary>
        /// busca um pedido pelo número (Sales GetOrderByNumber) e grava; a chamada fica em linxcommerce.LinxCommerceRequests
        /// </summary>
        [HttpGet("Orders/{docCompany}/Number/{orderNumber}")]
        [Tags("LinxCommerceOrders")]
        public async Task<ActionResult> OrderByNumber(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^[A-Za-z0-9._-]{1,50}$")] string orderNumber,
            [FromServices] ILinxCommerceOrdersService linxCommerceOrdersService
        ) =>
            OnDemandResponse(await linxCommerceOrdersService.GetOrderByNumber(userName, docCompany, orderNumber), $"Order {orderNumber}");
    }
}
