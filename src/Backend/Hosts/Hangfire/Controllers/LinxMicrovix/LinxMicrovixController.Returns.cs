using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixReturnsService: trocas e devoluções. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxMovimentoTrocas: os valores e números das trocas realizadas nas vendas
        /// </summary>
        [HttpPost("LinxMovimentoTrocas")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxMovimentoTrocas([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxMovimentoTrocas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoTrocafone: os valores e números dos vouchers Trocafone realizadas nas vendas
        /// </summary>
        [HttpPost("LinxMovimentoTrocafone")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxMovimentoTrocafone([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxMovimentoTrocafone();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoDevolucoesItens: os dados da tabela movimento_devolucoes_itens cadastrados no portal.
        /// </summary>
        [HttpPost("LinxMovimentoDevolucoesItens")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxMovimentoDevolucoesItens([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxMovimentoDevolucoesItens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoOrigemDevolucoes: os documentos de origem relacionados as devoluções
        /// </summary>
        [HttpPost("LinxMovimentoOrigemDevolucoes")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxMovimentoOrigemDevolucoes([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxMovimentoOrigemDevolucoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMotivoDevolucao: os motivos de devoluções cadastrados no ERP.
        /// </summary>
        [HttpPost("LinxMotivoDevolucao")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxMotivoDevolucao([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxMotivoDevolucao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxTrocaUnificadaResumoVendas: as vendas do troca unificada.
        /// </summary>
        [HttpPost("LinxTrocaUnificadaResumoVendas")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxTrocaUnificadaResumoVendas([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxTrocaUnificadaResumoVendas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxTrocaUnificadaResumoVendasItens
        /// </summary>
        [HttpPost("LinxTrocaUnificadaResumoVendasItens")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxTrocaUnificadaResumoVendasItens([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxTrocaUnificadaResumoVendasItens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxTrocaUnificadaResumoBaixa: os dados do resumo baixa do troca unificada
        /// </summary>
        [HttpPost("LinxTrocaUnificadaResumoBaixa")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxTrocaUnificadaResumoBaixa([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxTrocaUnificadaResumoBaixa();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxTradeinParceiro: os registros cadastrados na tabela tradein_parceiro do portal
        /// </summary>
        [HttpPost("LinxTradeinParceiro")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxTradeinParceiro([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxTradeinParceiro();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxDevolucaoRemanejoFabrica: as devoluções e remanejos de fábrica.
        /// </summary>
        [HttpPost("LinxDevolucaoRemanejoFabrica")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxDevolucaoRemanejoFabrica([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxDevolucaoRemanejoFabrica();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxDevolucaoRemanejoFabricaItem: os itens das devoluções e remanejos de fábrica.
        /// </summary>
        [HttpPost("LinxDevolucaoRemanejoFabricaItem")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxDevolucaoRemanejoFabricaItem([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxDevolucaoRemanejoFabricaItem();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxDevolucaoRemanejoFabricaStatus: o cadastro de status da rotina devoluções e remanejos de fábrica.
        /// </summary>
        [HttpPost("LinxDevolucaoRemanejoFabricaStatus")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxDevolucaoRemanejoFabricaStatus([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxDevolucaoRemanejoFabricaStatus();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxDevolucaoRemanejoFabricaTipo: o cadastro de tipos da rotina devoluções e remanejos de fábrica.
        /// </summary>
        [HttpPost("LinxDevolucaoRemanejoFabricaTipo")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxDevolucaoRemanejoFabricaTipo([FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService)
        {
            var result = await linxMicrovixReturnsService.LinxDevolucaoRemanejoFabricaTipo();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxMovimentoTrocas por docOrigem e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxMovimentoTrocas/{docCompany}/{cnpjEmp}/{docOrigem}")]
        [Tags("LinxMicrovixReturns")]
        public async Task<ActionResult> LinxMovimentoTrocasByDocOrigem(
            [FromServices] ILinxMicrovixReturnsService linxMicrovixReturnsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string docOrigem
        )
        {
            var result = await linxMicrovixReturnsService.LinxMovimentoTrocasByDocOrigem(userName, docCompany, cnpjEmp, docOrigem);

            return result switch
            {
                true => Ok($"LinxMovimentoTrocas {docOrigem} integrated successfully."),
                false => NotFound($"LinxMovimentoTrocas {docOrigem} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
