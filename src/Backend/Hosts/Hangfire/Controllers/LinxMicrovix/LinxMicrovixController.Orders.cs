using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixOrdersService: pedidos de venda. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxPedidosVenda: informações de pedidos de venda de acordo com a loja pesquisada
        /// </summary>
        [HttpPost("LinxPedidosVenda")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxPedidosVenda([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxPedidosVenda();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPlanosPedidoVenda: os Planos de Pagamentos realizados nos pedidos de vendas
        /// </summary>
        [HttpPost("LinxPlanosPedidoVenda")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxPlanosPedidoVenda([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxPlanosPedidoVenda();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPedidosVendaChecklistEntregaLocal: os tipos de locais cadastrados no checklist de entrega do pedido de venda.
        /// </summary>
        [HttpPost("LinxPedidosVendaChecklistEntregaLocal")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxPedidosVendaChecklistEntregaLocal([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxPedidosVendaChecklistEntregaLocal();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPedidosVendaChecklistEntregaArmazenamento: os tipos de armazenamentos cadastrados no checklist de entrega do pedido de venda.
        /// </summary>
        [HttpPost("LinxPedidosVendaChecklistEntregaArmazenamento")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxPedidosVendaChecklistEntregaArmazenamento([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxPedidosVendaChecklistEntregaArmazenamento();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPedidosVendaChecklistEntregaDificuldade: os tipos de dificuldades cadastradas no checklist de entrega do pedido de venda.
        /// </summary>
        [HttpPost("LinxPedidosVendaChecklistEntregaDificuldade")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxPedidosVendaChecklistEntregaDificuldade([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxPedidosVendaChecklistEntregaDificuldade();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxOrcamentoPosicao: os dados das posições cadastradas para os orçamentos.
        /// </summary>
        [HttpPost("LinxOrcamentoPosicao")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxOrcamentoPosicao([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxOrcamentoPosicao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxB2CPedidos: os pedidos de B2C
        /// </summary>
        [HttpPost("LinxB2CPedidos")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxB2CPedidos([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxB2CPedidos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxB2CPedidosItens: os itens dos pedidos de B2C
        /// </summary>
        [HttpPost("LinxB2CPedidosItens")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxB2CPedidosItens([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxB2CPedidosItens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxB2CPedidosStatus: o status dos pedidos de B2C
        /// </summary>
        [HttpPost("LinxB2CPedidosStatus")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxB2CPedidosStatus([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxB2CPedidosStatus();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxB2CStatus: o cadastro dos status de B2C
        /// </summary>
        [HttpPost("LinxB2CStatus")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxB2CStatus([FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService)
        {
            var result = await linxMicrovixOrdersService.LinxB2CStatus();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxPedidosVenda por codPedido e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxPedidosVenda/{docCompany}/{cnpjEmp}/{codPedido}")]
        [Tags("LinxMicrovixOrders")]
        public async Task<ActionResult> LinxPedidosVendaByCodPedido(
            [FromServices] ILinxMicrovixOrdersService linxMicrovixOrdersService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string codPedido
        )
        {
            var result = await linxMicrovixOrdersService.LinxPedidosVendaByCodPedido(userName, docCompany, cnpjEmp, codPedido);

            return result switch
            {
                true => Ok($"LinxPedidosVenda {codPedido} integrated successfully."),
                false => NotFound($"LinxPedidosVenda {codPedido} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
