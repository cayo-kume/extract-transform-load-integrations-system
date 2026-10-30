using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixServiceOrdersService: ordens de serviço. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxOrdensServico: as ordens de serviço
        /// </summary>
        [HttpPost("LinxOrdensServico")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxOrdensServico([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxOrdensServico();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxOrdensServicoProdutos: os produtos das ordens de serviço
        /// </summary>
        [HttpPost("LinxOrdensServicoProdutos")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxOrdensServicoProdutos([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxOrdensServicoProdutos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxServicos: o cadastro de serviços de acordo com a loja pesquisada ou dados da estrutura mercadológica
        /// </summary>
        [HttpPost("LinxServicos")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxServicos([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxServicos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxServicosDetalhes: informações dos serviços de acordo com a loja pesquisada
        /// </summary>
        [HttpPost("LinxServicosDetalhes")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxServicosDetalhes([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxServicosDetalhes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxServicoClientes: os dados dos clientes do serviço.
        /// </summary>
        [HttpPost("LinxServicoClientes")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxServicoClientes([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxServicoClientes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxServicoAgendaComandas: a agenda das comandas dos serviços.
        /// </summary>
        [HttpPost("LinxServicoAgendaComandas")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxServicoAgendaComandas([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxServicoAgendaComandas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxServicoAgendaEventos: a agenda dos eventos dos serviços.
        /// </summary>
        [HttpPost("LinxServicoAgendaEventos")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxServicoAgendaEventos([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxServicoAgendaEventos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxOrdemServicoExternaDav: as ordens de serviço externas das DAVs
        /// </summary>
        [HttpPost("LinxOrdemServicoExternaDav")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxOrdemServicoExternaDav([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxOrdemServicoExternaDav();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoOrdemServicoExterna: a ordem de serviço externa da movimentação.
        /// </summary>
        [HttpPost("LinxMovimentoOrdemServicoExterna")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxMovimentoOrdemServicoExterna([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxMovimentoOrdemServicoExterna();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxValeOrdemServicoExterna: os dados de vale ordem de serviço externa.
        /// </summary>
        [HttpPost("LinxValeOrdemServicoExterna")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxValeOrdemServicoExterna([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxValeOrdemServicoExterna();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPagamentoParcialDav: os pagamentos parciais da DAVs
        /// </summary>
        [HttpPost("LinxPagamentoParcialDav")]
        [Tags("LinxMicrovixServiceOrders")]
        public async Task<ActionResult> LinxPagamentoParcialDav([FromServices] ILinxMicrovixServiceOrdersService linxMicrovixServiceOrdersService)
        {
            var result = await linxMicrovixServiceOrdersService.LinxPagamentoParcialDav();

            return Ok($"Records integrated successfully.");
        }
    }
}
