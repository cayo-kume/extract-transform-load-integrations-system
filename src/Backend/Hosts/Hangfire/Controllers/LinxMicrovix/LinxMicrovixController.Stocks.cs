using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixStocksService: estoque e balanços. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxProdutosDetalhes: informações dos produtos de acordo com a loja pesquisada
        /// </summary>
        [HttpPost("LinxProdutosDetalhes")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxProdutosDetalhes([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxProdutosDetalhes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosDetalhesSimplificado: os dados adicionais de produtos de forma simplificada
        /// </summary>
        [HttpPost("LinxProdutosDetalhesSimplificado")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxProdutosDetalhesSimplificado([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxProdutosDetalhesSimplificado();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosDetalhesDepositos: os saldos dos produtos por depósito.
        /// </summary>
        [HttpPost("LinxProdutosDetalhesDepositos")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxProdutosDetalhesDepositos([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxProdutosDetalhesDepositos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosDepositos: os depósitos cadastrados no ERP
        /// </summary>
        [HttpPost("LinxProdutosDepositos")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxProdutosDepositos([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxProdutosDepositos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosInventario: informações do saldo do produto na data pesquisada
        /// </summary>
        [HttpPost("LinxProdutosInventario")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxProdutosInventario([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxProdutosInventario();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxListagemBalanco: a listagem de balanços.
        /// </summary>
        [HttpPost("LinxListagemBalanco")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxListagemBalanco([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxListagemBalanco();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxBalancos: os balanços.
        /// </summary>
        [HttpPost("LinxBalancos")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxBalancos([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxBalancos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxBalancosArquivos: os dados dos arquivos de balanços.
        /// </summary>
        [HttpPost("LinxBalancosArquivos")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxBalancosArquivos([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxBalancosArquivos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxBalancosSecoes: as seções dos balanços.
        /// </summary>
        [HttpPost("LinxBalancosSecoes")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxBalancosSecoes([FromServices] ILinxMicrovixStocksService linxMicrovixStocksService)
        {
            var result = await linxMicrovixStocksService.LinxBalancosSecoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxProdutosDetalhes por codProduto e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxProdutosDetalhes/{docCompany}/{cnpjEmp}/{codProduto}")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxProdutosDetalhesByCodProduto(
            [FromServices] ILinxMicrovixStocksService linxMicrovixStocksService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string codProduto
        )
        {
            var result = await linxMicrovixStocksService.LinxProdutosDetalhesByCodProduto(userName, docCompany, cnpjEmp, codProduto);

            return result switch
            {
                true => Ok($"LinxProdutosDetalhes {codProduto} integrated successfully."),
                false => NotFound($"LinxProdutosDetalhes {codProduto} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca LinxProdutosDepositos por codDeposito e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxProdutosDepositos/{docCompany}/{codDeposito}")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxProdutosDepositosByCodDeposito(
            [FromServices] ILinxMicrovixStocksService linxMicrovixStocksService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(100)] string codDeposito
        )
        {
            var result = await linxMicrovixStocksService.LinxProdutosDepositosByCodDeposito(userName, docCompany, codDeposito);

            return result switch
            {
                true => Ok($"LinxProdutosDepositos {codDeposito} integrated successfully."),
                false => NotFound($"LinxProdutosDepositos {codDeposito} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca LinxProdutosInventario por codProduto e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxProdutosInventario/{docCompany}/{cnpjEmp}/{codProduto}")]
        [Tags("LinxMicrovixStocks")]
        public async Task<ActionResult> LinxProdutosInventarioByCodProduto(
            [FromServices] ILinxMicrovixStocksService linxMicrovixStocksService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string codProduto
        )
        {
            var result = await linxMicrovixStocksService.LinxProdutosInventarioByCodProduto(userName, docCompany, cnpjEmp, codProduto);

            return result switch
            {
                true => Ok($"LinxProdutosInventario {codProduto} integrated successfully."),
                false => NotFound($"LinxProdutosInventario {codProduto} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
