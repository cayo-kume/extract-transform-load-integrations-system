using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixFinancialService: financeiro. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxFaturas: as faturas a receber ou a pagar de uma loja
        /// </summary>
        [HttpPost("LinxFaturas")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxFaturas([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxFaturas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxFaturasOrigem: as faturas de origem
        /// </summary>
        [HttpPost("LinxFaturasOrigem")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxFaturasOrigem([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxFaturasOrigem();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPortadores: os dados dos portadores.
        /// </summary>
        [HttpPost("LinxPortadores")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxPortadores([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxPortadores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxAntecipacoesFinanceiras: informações das antecipações financeiras do portal
        /// </summary>
        [HttpPost("LinxAntecipacoesFinanceiras")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxAntecipacoesFinanceiras([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxAntecipacoesFinanceiras();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxAntecipacoesFinanceirasPlanos: os dados da tabela antecipacoes_financeiras  e antecipacoes_financeiras_planos no portal.
        /// </summary>
        [HttpPost("LinxAntecipacoesFinanceirasPlanos")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxAntecipacoesFinanceirasPlanos([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxAntecipacoesFinanceirasPlanos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxDesfazimentosAntecipacoes: os valores registrados na tabela desfazimentos_antecipacoes
        /// </summary>
        [HttpPost("LinxDesfazimentosAntecipacoes")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxDesfazimentosAntecipacoes([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxDesfazimentosAntecipacoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxDesfazimentosAntecipacoesDocumento: os valores registrados na tabela desfazimentos_antecipacoes_documento
        /// </summary>
        [HttpPost("LinxDesfazimentosAntecipacoesDocumento")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxDesfazimentosAntecipacoesDocumento([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxDesfazimentosAntecipacoesDocumento();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxDesfazimentosAntecipacoesItens: os valores registrados na tabela desfazimentos_antecipacoes_itens
        /// </summary>
        [HttpPost("LinxDesfazimentosAntecipacoesItens")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxDesfazimentosAntecipacoesItens([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxDesfazimentosAntecipacoesItens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCategoriasFinanceiras: categoria financeira
        /// </summary>
        [HttpPost("LinxCategoriasFinanceiras")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxCategoriasFinanceiras([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxCategoriasFinanceiras();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCentroCusto: as informações dos centros de custos cadastrados no portal
        /// </summary>
        [HttpPost("LinxCentroCusto")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxCentroCusto([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxCentroCusto();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPlanoContas: as informações do plano de contas cadastrado no portal
        /// </summary>
        [HttpPost("LinxPlanoContas")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxPlanoContas([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxPlanoContas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxLancContabil: informações dos Lançamentos Contábeis por conta e empresa
        /// </summary>
        [HttpPost("LinxLancContabil")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxLancContabil([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxLancContabil();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxHistoricosContabeis: os dados dos históricos contábeis.
        /// </summary>
        [HttpPost("LinxHistoricosContabeis")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxHistoricosContabeis([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxHistoricosContabeis();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxExtratoBancarioLancamentos: os registros da tabela extrato_bancario_lancamentos.
        /// </summary>
        [HttpPost("LinxExtratoBancarioLancamentos")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxExtratoBancarioLancamentos([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxExtratoBancarioLancamentos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxExtratoBancarioConciliacoes: os registros da tabela extrato_bancario_conciliacoes.
        /// </summary>
        [HttpPost("LinxExtratoBancarioConciliacoes")]
        [Tags("LinxMicrovixFinancial")]
        public async Task<ActionResult> LinxExtratoBancarioConciliacoes([FromServices] ILinxMicrovixFinancialService linxMicrovixFinancialService)
        {
            var result = await linxMicrovixFinancialService.LinxExtratoBancarioConciliacoes();

            return Ok($"Records integrated successfully.");
        }
    }
}
