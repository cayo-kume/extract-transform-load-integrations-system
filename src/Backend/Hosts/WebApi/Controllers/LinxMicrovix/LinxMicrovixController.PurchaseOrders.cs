using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixPurchaseOrdersService: pedidos de compra. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxPedidosCompra: informações de pedidos de compra de acordo com a loja pesquisada
        /// </summary>
        [HttpPost("LinxPedidosCompra")]
        [Tags("LinxMicrovixPurchaseOrders")]
        public async Task<ActionResult> LinxPedidosCompra([FromServices] ILinxMicrovixPurchaseOrdersService linxMicrovixPurchaseOrdersService)
        {
            var result = await linxMicrovixPurchaseOrdersService.LinxPedidosCompra();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPedidosCompraStatus: os status de pedidos de compra.
        /// </summary>
        [HttpPost("LinxPedidosCompraStatus")]
        [Tags("LinxMicrovixPurchaseOrders")]
        public async Task<ActionResult> LinxPedidosCompraStatus([FromServices] ILinxMicrovixPurchaseOrdersService linxMicrovixPurchaseOrdersService)
        {
            var result = await linxMicrovixPurchaseOrdersService.LinxPedidosCompraStatus();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPedidosCompraNumeroFornecedor: o numero do fornecedor do pedido de compra.
        /// </summary>
        [HttpPost("LinxPedidosCompraNumeroFornecedor")]
        [Tags("LinxMicrovixPurchaseOrders")]
        public async Task<ActionResult> LinxPedidosCompraNumeroFornecedor([FromServices] ILinxMicrovixPurchaseOrdersService linxMicrovixPurchaseOrdersService)
        {
            var result = await linxMicrovixPurchaseOrdersService.LinxPedidosCompraNumeroFornecedor();

            return Ok($"Records integrated successfully.");
        }
    }
}
