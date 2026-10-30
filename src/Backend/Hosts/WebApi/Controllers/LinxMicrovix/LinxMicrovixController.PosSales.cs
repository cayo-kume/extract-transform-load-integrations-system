using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixPosSalesService: vendas pos temporárias. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxVendasPosTmp: os registros da tabela vendas_pos_tmp
        /// </summary>
        [HttpPost("LinxVendasPosTmp")]
        [Tags("LinxMicrovixPosSales")]
        public async Task<ActionResult> LinxVendasPosTmp([FromServices] ILinxMicrovixPosSalesService linxMicrovixPosSalesService)
        {
            var result = await linxMicrovixPosSalesService.LinxVendasPosTmp();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxVendasPosProdutosTmp: os registros da tabela vendas_pos_produtos_tmp.
        /// </summary>
        [HttpPost("LinxVendasPosProdutosTmp")]
        [Tags("LinxMicrovixPosSales")]
        public async Task<ActionResult> LinxVendasPosProdutosTmp([FromServices] ILinxMicrovixPosSalesService linxMicrovixPosSalesService)
        {
            var result = await linxMicrovixPosSalesService.LinxVendasPosProdutosTmp();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxVendasPosAgrupamentoTmp: os registros da tabela vendas_pos_agrupamento_tmp
        /// </summary>
        [HttpPost("LinxVendasPosAgrupamentoTmp")]
        [Tags("LinxMicrovixPosSales")]
        public async Task<ActionResult> LinxVendasPosAgrupamentoTmp([FromServices] ILinxMicrovixPosSalesService linxMicrovixPosSalesService)
        {
            var result = await linxMicrovixPosSalesService.LinxVendasPosAgrupamentoTmp();

            return Ok($"Records integrated successfully.");
        }
    }
}
