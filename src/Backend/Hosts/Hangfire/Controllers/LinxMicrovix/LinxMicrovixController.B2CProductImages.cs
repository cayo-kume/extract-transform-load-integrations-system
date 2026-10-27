using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixB2CProductImagesService: imagens de produto do b2c. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza B2CConsultaProdutosImagens
        /// </summary>
        [HttpPost("B2CConsultaProdutosImagens")]
        [Tags("LinxMicrovixB2CProductImages")]
        public async Task<ActionResult> B2CConsultaProdutosImagens([FromServices] ILinxMicrovixB2CProductImagesService linxMicrovixB2CProductImagesService)
        {
            var result = await linxMicrovixB2CProductImagesService.B2CConsultaProdutosImagens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosImagensURL
        /// </summary>
        [HttpPost("B2CConsultaProdutosImagensURL")]
        [Tags("LinxMicrovixB2CProductImages")]
        public async Task<ActionResult> B2CConsultaProdutosImagensURL([FromServices] ILinxMicrovixB2CProductImagesService linxMicrovixB2CProductImagesService)
        {
            var result = await linxMicrovixB2CProductImagesService.B2CConsultaProdutosImagensURL();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaImagens
        /// </summary>
        [HttpPost("B2CConsultaImagens")]
        [Tags("LinxMicrovixB2CProductImages")]
        public async Task<ActionResult> B2CConsultaImagens([FromServices] ILinxMicrovixB2CProductImagesService linxMicrovixB2CProductImagesService)
        {
            var result = await linxMicrovixB2CProductImagesService.B2CConsultaImagens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaImagensHD
        /// </summary>
        [HttpPost("B2CConsultaImagensHD")]
        [Tags("LinxMicrovixB2CProductImages")]
        public async Task<ActionResult> B2CConsultaImagensHD([FromServices] ILinxMicrovixB2CProductImagesService linxMicrovixB2CProductImagesService)
        {
            var result = await linxMicrovixB2CProductImagesService.B2CConsultaImagensHD();

            return Ok($"Records integrated successfully.");
        }
    }
}
