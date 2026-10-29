using Application.LinxCommerce.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxCommerce
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
    }
}
