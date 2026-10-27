using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixLoyaltyService: fidelidade. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxFidelidade: os dados da tabela fidelidade_parceiro_log cadastrados no portal.
        /// </summary>
        [HttpPost("LinxFidelidade")]
        [Tags("LinxMicrovixLoyalty")]
        public async Task<ActionResult> LinxFidelidade([FromServices] ILinxMicrovixLoyaltyService linxMicrovixLoyaltyService)
        {
            var result = await linxMicrovixLoyaltyService.LinxFidelidade();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoReshop: movimento_campanha_reshop.
        /// </summary>
        [HttpPost("LinxMovimentoReshop")]
        [Tags("LinxMicrovixLoyalty")]
        public async Task<ActionResult> LinxMovimentoReshop([FromServices] ILinxMicrovixLoyaltyService linxMicrovixLoyaltyService)
        {
            var result = await linxMicrovixLoyaltyService.LinxMovimentoReshop();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoReshopItens: movimento_campanha_reshop_item.
        /// </summary>
        [HttpPost("LinxMovimentoReshopItens")]
        [Tags("LinxMicrovixLoyalty")]
        public async Task<ActionResult> LinxMovimentoReshopItens([FromServices] ILinxMicrovixLoyaltyService linxMicrovixLoyaltyService)
        {
            var result = await linxMicrovixLoyaltyService.LinxMovimentoReshopItens();

            return Ok($"Records integrated successfully.");
        }
    }
}
