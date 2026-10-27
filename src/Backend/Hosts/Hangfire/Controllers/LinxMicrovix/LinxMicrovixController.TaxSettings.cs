using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixTaxSettingsService: configurações tributárias. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxConfiguracoesTributarias: o cadastro de configurações tributárias da empresa
        /// </summary>
        [HttpPost("LinxConfiguracoesTributarias")]
        [Tags("LinxMicrovixTaxSettings")]
        public async Task<ActionResult> LinxConfiguracoesTributarias([FromServices] ILinxMicrovixTaxSettingsService linxMicrovixTaxSettingsService)
        {
            var result = await linxMicrovixTaxSettingsService.LinxConfiguracoesTributarias();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxConfiguracoesTributariasDetalhes: o detalhamento das configurações tributárias da empresa
        /// </summary>
        [HttpPost("LinxConfiguracoesTributariasDetalhes")]
        [Tags("LinxMicrovixTaxSettings")]
        public async Task<ActionResult> LinxConfiguracoesTributariasDetalhes([FromServices] ILinxMicrovixTaxSettingsService linxMicrovixTaxSettingsService)
        {
            var result = await linxMicrovixTaxSettingsService.LinxConfiguracoesTributariasDetalhes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxConfiguracoesTributariasDetalhesSimplificado: o detalhamento das configurações tributarias de forma simplificada.
        /// </summary>
        [HttpPost("LinxConfiguracoesTributariasDetalhesSimplificado")]
        [Tags("LinxMicrovixTaxSettings")]
        public async Task<ActionResult> LinxConfiguracoesTributariasDetalhesSimplificado([FromServices] ILinxMicrovixTaxSettingsService linxMicrovixTaxSettingsService)
        {
            var result = await linxMicrovixTaxSettingsService.LinxConfiguracoesTributariasDetalhesSimplificado();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxConfiguracoesTributariasEmpresas: o conteúdo da tabela config_tributaria_empresa.
        /// </summary>
        [HttpPost("LinxConfiguracoesTributariasEmpresas")]
        [Tags("LinxMicrovixTaxSettings")]
        public async Task<ActionResult> LinxConfiguracoesTributariasEmpresas([FromServices] ILinxMicrovixTaxSettingsService linxMicrovixTaxSettingsService)
        {
            var result = await linxMicrovixTaxSettingsService.LinxConfiguracoesTributariasEmpresas();

            return Ok($"Records integrated successfully.");
        }
    }
}
