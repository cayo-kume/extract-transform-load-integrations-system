using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixSellersService: vendedores e funcionários. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxVendedores: os Vendedores cadastrados na loja
        /// </summary>
        [HttpPost("LinxVendedores")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxVendedores([FromServices] ILinxMicrovixSellersService linxMicrovixSellersService)
        {
            var result = await linxMicrovixSellersService.LinxVendedores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxVendedoresFerias: as férias cadatradas para os vendedores.
        /// </summary>
        [HttpPost("LinxVendedoresFerias")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxVendedoresFerias([FromServices] ILinxMicrovixSellersService linxMicrovixSellersService)
        {
            var result = await linxMicrovixSellersService.LinxVendedoresFerias();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMetasVendedores: as Metas de Vendedores das lojas
        /// </summary>
        [HttpPost("LinxMetasVendedores")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxMetasVendedores([FromServices] ILinxMicrovixSellersService linxMicrovixSellersService)
        {
            var result = await linxMicrovixSellersService.LinxMetasVendedores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMetasVendedoresDia: as Metas de Vendedores das lojas por dia
        /// </summary>
        [HttpPost("LinxMetasVendedoresDia")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxMetasVendedoresDia([FromServices] ILinxMicrovixSellersService linxMicrovixSellersService)
        {
            var result = await linxMicrovixSellersService.LinxMetasVendedoresDia();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxComissoesVendedores: as comissões do vendedores
        /// </summary>
        [HttpPost("LinxComissoesVendedores")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxComissoesVendedores([FromServices] ILinxMicrovixSellersService linxMicrovixSellersService)
        {
            var result = await linxMicrovixSellersService.LinxComissoesVendedores();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxListaDaVez: os registros gerados na rotina de Lista da Vez
        /// </summary>
        [HttpPost("LinxListaDaVez")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxListaDaVez([FromServices] ILinxMicrovixSellersService linxMicrovixSellersService)
        {
            var result = await linxMicrovixSellersService.LinxListaDaVez();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxFuncionariosFuncoes: o cadastro da funções de funcionários.
        /// </summary>
        [HttpPost("LinxFuncionariosFuncoes")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxFuncionariosFuncoes([FromServices] ILinxMicrovixSellersService linxMicrovixSellersService)
        {
            var result = await linxMicrovixSellersService.LinxFuncionariosFuncoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxRegistroProfissional: os registros da tabela tipo_registro_profissional.
        /// </summary>
        [HttpPost("LinxRegistroProfissional")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxRegistroProfissional([FromServices] ILinxMicrovixSellersService linxMicrovixSellersService)
        {
            var result = await linxMicrovixSellersService.LinxRegistroProfissional();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxVendedores por codVendedor e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxVendedores/{docCompany}/{cnpjEmp}/{codVendedor}")]
        [Tags("LinxMicrovixSellers")]
        public async Task<ActionResult> LinxVendedoresByCodVendedor(
            [FromServices] ILinxMicrovixSellersService linxMicrovixSellersService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string codVendedor
        )
        {
            var result = await linxMicrovixSellersService.LinxVendedoresByCodVendedor(userName, docCompany, cnpjEmp, codVendedor);

            return result switch
            {
                true => Ok($"LinxVendedores {codVendedor} integrated successfully."),
                false => NotFound($"LinxVendedores {codVendedor} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
