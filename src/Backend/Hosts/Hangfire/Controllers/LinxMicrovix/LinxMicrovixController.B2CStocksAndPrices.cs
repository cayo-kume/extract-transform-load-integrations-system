using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixB2CStocksAndPricesService: estoque, custos e preços do b2c. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza B2CConsultaProdutosCustos
        /// </summary>
        [HttpPost("B2CConsultaProdutosCustos")]
        [Tags("LinxMicrovixB2CStocksAndPrices")]
        public async Task<ActionResult> B2CConsultaProdutosCustos([FromServices] ILinxMicrovixB2CStocksAndPricesService linxMicrovixB2CStocksAndPricesService)
        {
            var result = await linxMicrovixB2CStocksAndPricesService.B2CConsultaProdutosCustos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosDepositos
        /// </summary>
        [HttpPost("B2CConsultaProdutosDepositos")]
        [Tags("LinxMicrovixB2CStocksAndPrices")]
        public async Task<ActionResult> B2CConsultaProdutosDepositos([FromServices] ILinxMicrovixB2CStocksAndPricesService linxMicrovixB2CStocksAndPricesService)
        {
            var result = await linxMicrovixB2CStocksAndPricesService.B2CConsultaProdutosDepositos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosDetalhes
        /// </summary>
        [HttpPost("B2CConsultaProdutosDetalhes")]
        [Tags("LinxMicrovixB2CStocksAndPrices")]
        public async Task<ActionResult> B2CConsultaProdutosDetalhes([FromServices] ILinxMicrovixB2CStocksAndPricesService linxMicrovixB2CStocksAndPricesService)
        {
            var result = await linxMicrovixB2CStocksAndPricesService.B2CConsultaProdutosDetalhes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosDetalhesDepositos
        /// </summary>
        [HttpPost("B2CConsultaProdutosDetalhesDepositos")]
        [Tags("LinxMicrovixB2CStocksAndPrices")]
        public async Task<ActionResult> B2CConsultaProdutosDetalhesDepositos([FromServices] ILinxMicrovixB2CStocksAndPricesService linxMicrovixB2CStocksAndPricesService)
        {
            var result = await linxMicrovixB2CStocksAndPricesService.B2CConsultaProdutosDetalhesDepositos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosPromocao
        /// </summary>
        [HttpPost("B2CConsultaProdutosPromocao")]
        [Tags("LinxMicrovixB2CStocksAndPrices")]
        public async Task<ActionResult> B2CConsultaProdutosPromocao([FromServices] ILinxMicrovixB2CStocksAndPricesService linxMicrovixB2CStocksAndPricesService)
        {
            var result = await linxMicrovixB2CStocksAndPricesService.B2CConsultaProdutosPromocao();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosTabelas
        /// </summary>
        [HttpPost("B2CConsultaProdutosTabelas")]
        [Tags("LinxMicrovixB2CStocksAndPrices")]
        public async Task<ActionResult> B2CConsultaProdutosTabelas([FromServices] ILinxMicrovixB2CStocksAndPricesService linxMicrovixB2CStocksAndPricesService)
        {
            var result = await linxMicrovixB2CStocksAndPricesService.B2CConsultaProdutosTabelas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza B2CConsultaProdutosTabelasPrecos
        /// </summary>
        [HttpPost("B2CConsultaProdutosTabelasPrecos")]
        [Tags("LinxMicrovixB2CStocksAndPrices")]
        public async Task<ActionResult> B2CConsultaProdutosTabelasPrecos([FromServices] ILinxMicrovixB2CStocksAndPricesService linxMicrovixB2CStocksAndPricesService)
        {
            var result = await linxMicrovixB2CStocksAndPricesService.B2CConsultaProdutosTabelasPrecos();

            return Ok($"Records integrated successfully.");
        }
    }
}
