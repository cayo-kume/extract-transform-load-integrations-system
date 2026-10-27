using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixProductsService: produtos. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxProdutos: o cadastro de produtos de acordo com a loja pesquisada ou dados da estrutura mercadológica
        /// </summary>
        [HttpPost("LinxProdutos")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutos([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosAssociados: a associação entre produtos.
        /// </summary>
        [HttpPost("LinxProdutosAssociados")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosAssociados([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutosAssociados();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosCamposAdicionais: os campos adicionais preenchidos dos Produtos cadastrados no portal
        /// </summary>
        [HttpPost("LinxProdutosCamposAdicionais")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosCamposAdicionais([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutosCamposAdicionais();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosCodBar: o cadastro de códigos de barras dos produtos
        /// </summary>
        [HttpPost("LinxProdutosCodBar")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosCodBar([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutosCodBar();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosFornec: os registros de produtos x fornecedores
        /// </summary>
        [HttpPost("LinxProdutosFornec")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosFornec([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutosFornec();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosInformacoes: informações dos produtos (características).
        /// </summary>
        [HttpPost("LinxProdutosInformacoes")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosInformacoes([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutosInformacoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosImagensURL: as URLs das imagens dos produtos.
        /// </summary>
        [HttpPost("LinxProdutosImagensURL")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosImagensURL([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutosImagensURL();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosLotes: o saldo do lotes de produtos.
        /// </summary>
        [HttpPost("LinxProdutosLotes")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosLotes([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutosLotes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxLotesProdutos: os dados dos lotes dos produtos.
        /// </summary>
        [HttpPost("LinxLotesProdutos")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxLotesProdutos([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxLotesProdutos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosSerial: o saldo atual dos seriais cadastrados no portal.
        /// </summary>
        [HttpPost("LinxProdutosSerial")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosSerial([FromServices] ILinxMicrovixProductsService linxMicrovixProductsService)
        {
            var result = await linxMicrovixProductsService.LinxProdutosSerial();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxProdutos por codProduto e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxProdutos/{docCompany}/{codProduto}")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosByCodProduto(
            [FromServices] ILinxMicrovixProductsService linxMicrovixProductsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string codProduto
        )
        {
            var result = await linxMicrovixProductsService.LinxProdutosByCodProduto(userName, docCompany, codProduto);

            return result switch
            {
                true => Ok($"LinxProdutos {codProduto} integrated successfully."),
                false => NotFound($"LinxProdutos {codProduto} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca LinxProdutosCamposAdicionais por codProduto e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxProdutosCamposAdicionais/{docCompany}/{cnpjEmp}/{codProduto}")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosCamposAdicionaisByCodProduto(
            [FromServices] ILinxMicrovixProductsService linxMicrovixProductsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string codProduto
        )
        {
            var result = await linxMicrovixProductsService.LinxProdutosCamposAdicionaisByCodProduto(userName, docCompany, cnpjEmp, codProduto);

            return result switch
            {
                true => Ok($"LinxProdutosCamposAdicionais {codProduto} integrated successfully."),
                false => NotFound($"LinxProdutosCamposAdicionais {codProduto} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca LinxProdutosCodBar por codProduto e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxProdutosCodBar/{docCompany}/{codProduto}")]
        [Tags("LinxMicrovixProducts")]
        public async Task<ActionResult> LinxProdutosCodBarByCodProduto(
            [FromServices] ILinxMicrovixProductsService linxMicrovixProductsService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string codProduto
        )
        {
            var result = await linxMicrovixProductsService.LinxProdutosCodBarByCodProduto(userName, docCompany, codProduto);

            return result switch
            {
                true => Ok($"LinxProdutosCodBar {codProduto} integrated successfully."),
                false => NotFound($"LinxProdutosCodBar {codProduto} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
