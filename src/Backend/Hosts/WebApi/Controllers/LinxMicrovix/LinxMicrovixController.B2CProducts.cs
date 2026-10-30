using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixB2CProductsService: produtos do b2c. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza B2CConsultaProdutos
        /// </summary>
        [HttpPost("B2CConsultaProdutos")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutos([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosAssociados
        /// </summary>
        [HttpPost("B2CConsultaProdutosAssociados")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosAssociados([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosAssociados();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosCampanhas
        /// </summary>
        [HttpPost("B2CConsultaProdutosCampanhas")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosCampanhas([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosCampanhas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosCodebar
        /// </summary>
        [HttpPost("B2CConsultaProdutosCodebar")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosCodebar([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosCodebar();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosDimensoes
        /// </summary>
        [HttpPost("B2CConsultaProdutosDimensoes")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosDimensoes([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosDimensoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosInformacoes
        /// </summary>
        [HttpPost("B2CConsultaProdutosInformacoes")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosInformacoes([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosInformacoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosStatus
        /// </summary>
        [HttpPost("B2CConsultaProdutosStatus")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosStatus([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosStatus();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosFlags
        /// </summary>
        [HttpPost("B2CConsultaProdutosFlags")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosFlags([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosFlags();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosTags
        /// </summary>
        [HttpPost("B2CConsultaProdutosTags")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosTags([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosTags();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosPalavrasChavePesquisa
        /// </summary>
        [HttpPost("B2CConsultaProdutosPalavrasChavePesquisa")]
        [Tags("LinxMicrovixB2CProducts")]
        public async Task<ActionResult> B2CConsultaProdutosPalavrasChavePesquisa([FromServices] ILinxMicrovixB2CProductsService linxMicrovixB2CProductsService)
        {
            var result = await linxMicrovixB2CProductsService.B2CConsultaProdutosPalavrasChavePesquisa();

            return Ok($"Records integrated successfully.");
        }
    }
}
