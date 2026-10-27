using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixB2COrdersService: pedidos e notas do b2c. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza B2CConsultaPedidos
        /// </summary>
        [HttpPost("B2CConsultaPedidos")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidos([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaPedidosItens
        /// </summary>
        [HttpPost("B2CConsultaPedidosItens")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidosItens([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidosItens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaPedidosPlanos
        /// </summary>
        [HttpPost("B2CConsultaPedidosPlanos")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidosPlanos([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidosPlanos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaPedidosStatus
        /// </summary>
        [HttpPost("B2CConsultaPedidosStatus")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidosStatus([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidosStatus();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaPedidosIdentificador: os registro da tabela B2C_vendas
        /// </summary>
        [HttpPost("B2CConsultaPedidosIdentificador")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidosIdentificador([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidosIdentificador();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaPedidosTipos
        /// </summary>
        [HttpPost("B2CConsultaPedidosTipos")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidosTipos([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidosTipos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaStatus
        /// </summary>
        [HttpPost("B2CConsultaStatus")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaStatus([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaStatus();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaCodigoRastreio
        /// </summary>
        [HttpPost("B2CConsultaCodigoRastreio")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaCodigoRastreio([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaCodigoRastreio();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaNFe
        /// </summary>
        [HttpPost("B2CConsultaNFe")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaNFe([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaNFe();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaNFeSituacao
        /// </summary>
        [HttpPost("B2CConsultaNFeSituacao")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaNFeSituacao([FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService)
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaNFeSituacao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca B2CConsultaPedidos por idPedido e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("B2CConsultaPedidos/{docCompany}/{idPedido}")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidosByIdPedido(
            [FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string idPedido
        )
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidosByIdPedido(userName, docCompany, idPedido);

            return result switch
            {
                true => Ok($"B2CConsultaPedidos {idPedido} integrated successfully."),
                false => NotFound($"B2CConsultaPedidos {idPedido} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca B2CConsultaPedidosItens por idPedido e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("B2CConsultaPedidosItens/{docCompany}/{idPedido}")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidosItensByIdPedido(
            [FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string idPedido
        )
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidosItensByIdPedido(userName, docCompany, idPedido);

            return result switch
            {
                true => Ok($"B2CConsultaPedidosItens {idPedido} integrated successfully."),
                false => NotFound($"B2CConsultaPedidosItens {idPedido} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca B2CConsultaPedidosStatus por idPedido e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("B2CConsultaPedidosStatus/{docCompany}/{idPedido}")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaPedidosStatusByIdPedido(
            [FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string idPedido
        )
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaPedidosStatusByIdPedido(userName, docCompany, idPedido);

            return result switch
            {
                true => Ok($"B2CConsultaPedidosStatus {idPedido} integrated successfully."),
                false => NotFound($"B2CConsultaPedidosStatus {idPedido} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca B2CConsultaNFe por idPedido e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("B2CConsultaNFe/{docCompany}/{cnpjEmp}/{idPedido}")]
        [Tags("LinxMicrovixB2COrders")]
        public async Task<ActionResult> B2CConsultaNFeByIdPedido(
            [FromServices] ILinxMicrovixB2COrdersService linxMicrovixB2COrdersService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string idPedido
        )
        {
            var result = await linxMicrovixB2COrdersService.B2CConsultaNFeByIdPedido(userName, docCompany, cnpjEmp, idPedido);

            return result switch
            {
                true => Ok($"B2CConsultaNFe {idPedido} integrated successfully."),
                false => NotFound($"B2CConsultaNFe {idPedido} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
