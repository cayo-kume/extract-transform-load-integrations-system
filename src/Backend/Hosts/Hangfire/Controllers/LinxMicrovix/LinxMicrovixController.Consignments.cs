using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixConsignmentsService: remessas. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxMovimentoRemessas: os dados da tabela remessas no portal.
        /// </summary>
        [HttpPost("LinxMovimentoRemessas")]
        [Tags("LinxMicrovixConsignments")]
        public async Task<ActionResult> LinxMovimentoRemessas([FromServices] ILinxMicrovixConsignmentsService linxMicrovixConsignmentsService)
        {
            var result = await linxMicrovixConsignmentsService.LinxMovimentoRemessas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoRemessasItens: os dados da tabela remessas_itens no portal.
        /// </summary>
        [HttpPost("LinxMovimentoRemessasItens")]
        [Tags("LinxMicrovixConsignments")]
        public async Task<ActionResult> LinxMovimentoRemessasItens([FromServices] ILinxMicrovixConsignmentsService linxMicrovixConsignmentsService)
        {
            var result = await linxMicrovixConsignmentsService.LinxMovimentoRemessasItens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoRemessasAcertos: os dados da tabela remessas_acertos no portal.
        /// </summary>
        [HttpPost("LinxMovimentoRemessasAcertos")]
        [Tags("LinxMicrovixConsignments")]
        public async Task<ActionResult> LinxMovimentoRemessasAcertos([FromServices] ILinxMicrovixConsignmentsService linxMicrovixConsignmentsService)
        {
            var result = await linxMicrovixConsignmentsService.LinxMovimentoRemessasAcertos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoRemessasAcertosItens: os dados da tabela remessas_acertos_itens no portal.
        /// </summary>
        [HttpPost("LinxMovimentoRemessasAcertosItens")]
        [Tags("LinxMicrovixConsignments")]
        public async Task<ActionResult> LinxMovimentoRemessasAcertosItens([FromServices] ILinxMicrovixConsignmentsService linxMicrovixConsignmentsService)
        {
            var result = await linxMicrovixConsignmentsService.LinxMovimentoRemessasAcertosItens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxRemessasIdentificadores: recebe o identificador da venda e retorna a relação com os identificadores das notas dos itens da remessa.
        /// </summary>
        [HttpPost("LinxRemessasIdentificadores")]
        [Tags("LinxMicrovixConsignments")]
        public async Task<ActionResult> LinxRemessasIdentificadores([FromServices] ILinxMicrovixConsignmentsService linxMicrovixConsignmentsService)
        {
            var result = await linxMicrovixConsignmentsService.LinxRemessasIdentificadores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxRemessasOperacoes: os dados da tabela remessa_operacoes no portal.
        /// </summary>
        [HttpPost("LinxRemessasOperacoes")]
        [Tags("LinxMicrovixConsignments")]
        public async Task<ActionResult> LinxRemessasOperacoes([FromServices] ILinxMicrovixConsignmentsService linxMicrovixConsignmentsService)
        {
            var result = await linxMicrovixConsignmentsService.LinxRemessasOperacoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxRemessasRetornoTipos: os dados da tabela remessa_retorno_tipos no portal.
        /// </summary>
        [HttpPost("LinxRemessasRetornoTipos")]
        [Tags("LinxMicrovixConsignments")]
        public async Task<ActionResult> LinxRemessasRetornoTipos([FromServices] ILinxMicrovixConsignmentsService linxMicrovixConsignmentsService)
        {
            var result = await linxMicrovixConsignmentsService.LinxRemessasRetornoTipos();

            return Ok($"Records integrated successfully.");
        }
    }
}
