using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixB2CProductFieldsService: campos adicionais de produto do b2c. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza B2CConsultaProdutosCamposAdicionaisDetalhes
        /// </summary>
        [HttpPost("B2CConsultaProdutosCamposAdicionaisDetalhes")]
        [Tags("LinxMicrovixB2CProductFields")]
        public async Task<ActionResult> B2CConsultaProdutosCamposAdicionaisDetalhes([FromServices] ILinxMicrovixB2CProductFieldsService linxMicrovixB2CProductFieldsService)
        {
            var result = await linxMicrovixB2CProductFieldsService.B2CConsultaProdutosCamposAdicionaisDetalhes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosCamposAdicionaisNomes
        /// </summary>
        [HttpPost("B2CConsultaProdutosCamposAdicionaisNomes")]
        [Tags("LinxMicrovixB2CProductFields")]
        public async Task<ActionResult> B2CConsultaProdutosCamposAdicionaisNomes([FromServices] ILinxMicrovixB2CProductFieldsService linxMicrovixB2CProductFieldsService)
        {
            var result = await linxMicrovixB2CProductFieldsService.B2CConsultaProdutosCamposAdicionaisNomes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosCamposAdicionaisValores
        /// </summary>
        [HttpPost("B2CConsultaProdutosCamposAdicionaisValores")]
        [Tags("LinxMicrovixB2CProductFields")]
        public async Task<ActionResult> B2CConsultaProdutosCamposAdicionaisValores([FromServices] ILinxMicrovixB2CProductFieldsService linxMicrovixB2CProductFieldsService)
        {
            var result = await linxMicrovixB2CProductFieldsService.B2CConsultaProdutosCamposAdicionaisValores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaB2CProdutosCamposAdicionais
        /// </summary>
        [HttpPost("B2CConsultaB2CProdutosCamposAdicionais")]
        [Tags("LinxMicrovixB2CProductFields")]
        public async Task<ActionResult> B2CConsultaB2CProdutosCamposAdicionais([FromServices] ILinxMicrovixB2CProductFieldsService linxMicrovixB2CProductFieldsService)
        {
            var result = await linxMicrovixB2CProductFieldsService.B2CConsultaB2CProdutosCamposAdicionais();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaB2CProdutosCamposAdicionaisDados
        /// </summary>
        [HttpPost("B2CConsultaB2CProdutosCamposAdicionaisDados")]
        [Tags("LinxMicrovixB2CProductFields")]
        public async Task<ActionResult> B2CConsultaB2CProdutosCamposAdicionaisDados([FromServices] ILinxMicrovixB2CProductFieldsService linxMicrovixB2CProductFieldsService)
        {
            var result = await linxMicrovixB2CProductFieldsService.B2CConsultaB2CProdutosCamposAdicionaisDados();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaB2CProdutosCamposAdicionaisMedia
        /// </summary>
        [HttpPost("B2CConsultaB2CProdutosCamposAdicionaisMedia")]
        [Tags("LinxMicrovixB2CProductFields")]
        public async Task<ActionResult> B2CConsultaB2CProdutosCamposAdicionaisMedia([FromServices] ILinxMicrovixB2CProductFieldsService linxMicrovixB2CProductFieldsService)
        {
            var result = await linxMicrovixB2CProductFieldsService.B2CConsultaB2CProdutosCamposAdicionaisMedia();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaB2CProdutosCamposAdicionaisMediaDados
        /// </summary>
        [HttpPost("B2CConsultaB2CProdutosCamposAdicionaisMediaDados")]
        [Tags("LinxMicrovixB2CProductFields")]
        public async Task<ActionResult> B2CConsultaB2CProdutosCamposAdicionaisMediaDados([FromServices] ILinxMicrovixB2CProductFieldsService linxMicrovixB2CProductFieldsService)
        {
            var result = await linxMicrovixB2CProductFieldsService.B2CConsultaB2CProdutosCamposAdicionaisMediaDados();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaB2CProdutosCamposAdicionaisOpcoes
        /// </summary>
        [HttpPost("B2CConsultaB2CProdutosCamposAdicionaisOpcoes")]
        [Tags("LinxMicrovixB2CProductFields")]
        public async Task<ActionResult> B2CConsultaB2CProdutosCamposAdicionaisOpcoes([FromServices] ILinxMicrovixB2CProductFieldsService linxMicrovixB2CProductFieldsService)
        {
            var result = await linxMicrovixB2CProductFieldsService.B2CConsultaB2CProdutosCamposAdicionaisOpcoes();

            return Ok($"Records integrated successfully.");
        }
    }
}
