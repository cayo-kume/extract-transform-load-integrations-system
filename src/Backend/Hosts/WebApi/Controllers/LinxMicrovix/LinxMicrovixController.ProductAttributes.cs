using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixProductAttributesService: atributos de produto. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxSetores: os setores cadastrados
        /// </summary>
        [HttpPost("LinxSetores")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxSetores([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxSetores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxLinhas: todos os registros do cadastro auxiliar linha
        /// </summary>
        [HttpPost("LinxLinhas")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxLinhas([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxLinhas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMarcas: as marcas cadastradas.
        /// </summary>
        [HttpPost("LinxMarcas")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxMarcas([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxMarcas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxColecoes: as coleções cadastradas.
        /// </summary>
        [HttpPost("LinxColecoes")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxColecoes([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxColecoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxTamanhos: os tamanhos cadastrados.
        /// </summary>
        [HttpPost("LinxTamanhos")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxTamanhos([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxTamanhos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCores: as cores cadastradas.
        /// </summary>
        [HttpPost("LinxCores")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxCores([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxCores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxEspessuras: as espessuras cadastradas.
        /// </summary>
        [HttpPost("LinxEspessuras")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxEspessuras([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxEspessuras();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxUnidades: as unidades cadastradas.
        /// </summary>
        [HttpPost("LinxUnidades")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxUnidades([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxUnidades();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxClassificacoes: as classificacoes cadastradas.
        /// </summary>
        [HttpPost("LinxClassificacoes")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxClassificacoes([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxClassificacoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxSubClasses: o cadastro das Sub Classes dos clientes e fornecedores.
        /// </summary>
        [HttpPost("LinxSubClasses")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxSubClasses([FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService)
        {
            var result = await linxMicrovixProductAttributesService.LinxSubClasses();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxSetores por idSetor e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxSetores/{docCompany}/{idSetor}")]
        [Tags("LinxMicrovixProductAttributes")]
        public async Task<ActionResult> LinxSetoresByIdSetor(
            [FromServices] ILinxMicrovixProductAttributesService linxMicrovixProductAttributesService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string idSetor
        )
        {
            var result = await linxMicrovixProductAttributesService.LinxSetoresByIdSetor(userName, docCompany, idSetor);

            return result switch
            {
                true => Ok($"LinxSetores {idSetor} integrated successfully."),
                false => NotFound($"LinxSetores {idSetor} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
