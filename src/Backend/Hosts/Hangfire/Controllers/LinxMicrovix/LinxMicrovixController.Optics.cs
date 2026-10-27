using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixOpticsService: ótica. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxProdutosOpticosTipo: os tipos de produtos opticos.
        /// </summary>
        [HttpPost("LinxProdutosOpticosTipo")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxProdutosOpticosTipo([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxProdutosOpticosTipo();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosOpticosTipoAro: os tipos de aros
        /// </summary>
        [HttpPost("LinxProdutosOpticosTipoAro")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxProdutosOpticosTipoAro([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxProdutosOpticosTipoAro();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosOpticosFormatoAro: os formatos de aros
        /// </summary>
        [HttpPost("LinxProdutosOpticosFormatoAro")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxProdutosOpticosFormatoAro([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxProdutosOpticosFormatoAro();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxDadosOpticosDav: os dados ópticos dos produtos da DAV
        /// </summary>
        [HttpPost("LinxDadosOpticosDav")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxDadosOpticosDav([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxDadosOpticosDav();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxOticoPrismaDescricao: descrição de prismas óticos.
        /// </summary>
        [HttpPost("LinxOticoPrismaDescricao")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxOticoPrismaDescricao([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxOticoPrismaDescricao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxOticoReceitas: receitas óticas.
        /// </summary>
        [HttpPost("LinxOticoReceitas")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxOticoReceitas([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxOticoReceitas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxPosicaoOsRamoOptico: o cadastro da posição da OS do Ramo Optico
        /// </summary>
        [HttpPost("LinxPosicaoOsRamoOptico")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxPosicaoOsRamoOptico([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxPosicaoOsRamoOptico();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxOrdensServicoPosicaoOsRamoOpticoHistorico: o histórico das posições das ordens de serviço do ramo optico
        /// </summary>
        [HttpPost("LinxOrdensServicoPosicaoOsRamoOpticoHistorico")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxOrdensServicoPosicaoOsRamoOpticoHistorico([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxOrdensServicoPosicaoOsRamoOpticoHistorico();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxOrcamentoComponenteFormula: os componentes de fórmulas relacionados ao orçamento.
        /// </summary>
        [HttpPost("LinxOrcamentoComponenteFormula")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxOrcamentoComponenteFormula([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxOrcamentoComponenteFormula();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxComponenteFormulaDav: os componentes de fórmulas relacionados ao Dav.
        /// </summary>
        [HttpPost("LinxComponenteFormulaDav")]
        [Tags("LinxMicrovixOptics")]
        public async Task<ActionResult> LinxComponenteFormulaDav([FromServices] ILinxMicrovixOpticsService linxMicrovixOpticsService)
        {
            var result = await linxMicrovixOpticsService.LinxComponenteFormulaDav();

            return Ok($"Records integrated successfully.");
        }
    }
}
