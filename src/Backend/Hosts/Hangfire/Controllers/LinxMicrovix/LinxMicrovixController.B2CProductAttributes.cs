using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixB2CProductAttributesService: atributos de produto do b2c. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza B2CConsultaClassificacao
        /// </summary>
        [HttpPost("B2CConsultaClassificacao")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaClassificacao([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaClassificacao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaColecoes
        /// </summary>
        [HttpPost("B2CConsultaColecoes")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaColecoes([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaColecoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaEspessuras
        /// </summary>
        [HttpPost("B2CConsultaEspessuras")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaEspessuras([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaEspessuras();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaGrade1
        /// </summary>
        [HttpPost("B2CConsultaGrade1")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaGrade1([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaGrade1();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaGrade2
        /// </summary>
        [HttpPost("B2CConsultaGrade2")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaGrade2([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaGrade2();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaLinhas
        /// </summary>
        [HttpPost("B2CConsultaLinhas")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaLinhas([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaLinhas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaMarcas
        /// </summary>
        [HttpPost("B2CConsultaMarcas")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaMarcas([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaMarcas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaSetores
        /// </summary>
        [HttpPost("B2CConsultaSetores")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaSetores([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaSetores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaUnidade
        /// </summary>
        [HttpPost("B2CConsultaUnidade")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaUnidade([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaUnidade();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaFornecedores
        /// </summary>
        [HttpPost("B2CConsultaFornecedores")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaFornecedores([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaFornecedores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaFlags
        /// </summary>
        [HttpPost("B2CConsultaFlags")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaFlags([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaFlags();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaTags
        /// </summary>
        [HttpPost("B2CConsultaTags")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaTags([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaTags();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaPalavrasChavePesquisa
        /// </summary>
        [HttpPost("B2CConsultaPalavrasChavePesquisa")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaPalavrasChavePesquisa([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaPalavrasChavePesquisa();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaLegendasCadastrosAuxiliares
        /// </summary>
        [HttpPost("B2CConsultaLegendasCadastrosAuxiliares")]
        [Tags("LinxMicrovixB2CProductAttributes")]
        public async Task<ActionResult> B2CConsultaLegendasCadastrosAuxiliares([FromServices] ILinxMicrovixB2CProductAttributesService linxMicrovixB2CProductAttributesService)
        {
            var result = await linxMicrovixB2CProductAttributesService.B2CConsultaLegendasCadastrosAuxiliares();

            return Ok($"Records integrated successfully.");
        }
    }
}
