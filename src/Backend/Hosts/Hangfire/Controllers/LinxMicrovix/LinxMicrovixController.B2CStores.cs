using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixB2CStoresService: empresas, vendedores, transportadores e planos do b2c. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza B2CConsultaEmpresas
        /// </summary>
        [HttpPost("B2CConsultaEmpresas")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaEmpresas([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaEmpresas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaCNPJsChave
        /// </summary>
        [HttpPost("B2CConsultaCNPJsChave")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaCNPJsChave([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaCNPJsChave();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaVendedores
        /// </summary>
        [HttpPost("B2CConsultaVendedores")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaVendedores([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaVendedores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaTransportadores
        /// </summary>
        [HttpPost("B2CConsultaTransportadores")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaTransportadores([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaTransportadores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaFormasPagamento
        /// </summary>
        [HttpPost("B2CConsultaFormasPagamento")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaFormasPagamento([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaFormasPagamento();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaPlanos
        /// </summary>
        [HttpPost("B2CConsultaPlanos")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaPlanos([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaPlanos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaPlanosParcelas
        /// </summary>
        [HttpPost("B2CConsultaPlanosParcelas")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaPlanosParcelas([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaPlanosParcelas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaTiposCobrancaFrete
        /// </summary>
        [HttpPost("B2CConsultaTiposCobrancaFrete")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaTiposCobrancaFrete([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaTiposCobrancaFrete();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaTipoEncomenda
        /// </summary>
        [HttpPost("B2CConsultaTipoEncomenda")]
        [Tags("LinxMicrovixB2CStores")]
        public async Task<ActionResult> B2CConsultaTipoEncomenda([FromServices] ILinxMicrovixB2CStoresService linxMicrovixB2CStoresService)
        {
            var result = await linxMicrovixB2CStoresService.B2CConsultaTipoEncomenda();

            return Ok($"Records integrated successfully.");
        }
    }
}
