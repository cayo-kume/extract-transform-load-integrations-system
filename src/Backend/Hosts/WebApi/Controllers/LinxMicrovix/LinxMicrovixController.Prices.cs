using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers.LinxMicrovix
{
    /// <summary>
    /// rotas do LinxMicrovixPricesService: preços, promoções e cupons. O service é injetado na ação ([FromServices]), porque cada
    /// arquivo da classe partial injeta só o seu
    /// </summary>
    public partial class LinxMicrovixController
    {
        /// <summary>
        /// sincroniza LinxProdutosTabelas: o cadastro de Tabelas de Preços
        /// </summary>
        [HttpPost("LinxProdutosTabelas")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxProdutosTabelas([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxProdutosTabelas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosTabelasPrecos: a relação dos produtos por tabela de preço
        /// </summary>
        [HttpPost("LinxProdutosTabelasPrecos")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxProdutosTabelasPrecos([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxProdutosTabelasPrecos();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxProdutosPromocoes: informações das promoções dos produtos de acordo com a loja pesquisada
        /// </summary>
        [HttpPost("LinxProdutosPromocoes")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxProdutosPromocoes([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxProdutosPromocoes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxAcoesPromocionais: as Ações Promocionais cadastradas no Microvix
        /// </summary>
        [HttpPost("LinxAcoesPromocionais")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxAcoesPromocionais([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxAcoesPromocionais();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxAcoesPromocionaisCombinacaoProdutosItens: os itens da combinação de produtos de ações promocionais.
        /// </summary>
        [HttpPost("LinxAcoesPromocionaisCombinacaoProdutosItens")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxAcoesPromocionaisCombinacaoProdutosItens([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxAcoesPromocionaisCombinacaoProdutosItens();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxAcoesPromocionaisProdutosCortesia: os produtos de cortesia de ações promocionais.
        /// </summary>
        [HttpPost("LinxAcoesPromocionaisProdutosCortesia")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxAcoesPromocionaisProdutosCortesia([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxAcoesPromocionaisProdutosCortesia();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMovimentoAcoesPromocionais: as Ações Promocionais realizadas nas movimentações
        /// </summary>
        [HttpPost("LinxMovimentoAcoesPromocionais")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxMovimentoAcoesPromocionais([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxMovimentoAcoesPromocionais();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCupomDesconto: os registros do cadastro de cupons de desconto.
        /// </summary>
        [HttpPost("LinxCupomDesconto")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxCupomDesconto([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxCupomDesconto();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxCupomDescontoVendas: o id do cupom de desconto, o valor do cupom e identificador das vendas que utilizaram cupons de desconto.
        /// </summary>
        [HttpPost("LinxCupomDescontoVendas")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxCupomDescontoVendas([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxCupomDescontoVendas();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza LinxMotivoDesconto: os motivos de desconto cadastrados no portal
        /// </summary>
        [HttpPost("LinxMotivoDesconto")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxMotivoDesconto([FromServices] ILinxMicrovixPricesService linxMicrovixPricesService)
        {
            var result = await linxMicrovixPricesService.LinxMotivoDesconto();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sob demanda: busca LinxProdutosTabelas por idTabela e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxProdutosTabelas/{docCompany}/{cnpjEmp}/{idTabela}")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxProdutosTabelasByIdTabela(
            [FromServices] ILinxMicrovixPricesService linxMicrovixPricesService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string idTabela
        )
        {
            var result = await linxMicrovixPricesService.LinxProdutosTabelasByIdTabela(userName, docCompany, cnpjEmp, idTabela);

            return result switch
            {
                true => Ok($"LinxProdutosTabelas {idTabela} integrated successfully."),
                false => NotFound($"LinxProdutosTabelas {idTabela} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }

        /// <summary>
        /// sob demanda: busca LinxProdutosTabelasPrecos por idTabela e codProduto e grava; a chamada fica em linxmicrovix.LinxMicrovixRequests
        /// </summary>
        [HttpGet("LinxProdutosTabelasPrecos/{docCompany}/{cnpjEmp}/{idTabela}/{codProduto}")]
        [Tags("LinxMicrovixPrices")]
        public async Task<ActionResult> LinxProdutosTabelasPrecosByIdTabelaAndCodProduto(
            [FromServices] ILinxMicrovixPricesService linxMicrovixPricesService,
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{14}$")] string cnpjEmp,
            [StringLength(100)] string idTabela,
            [StringLength(100)] string codProduto
        )
        {
            var result = await linxMicrovixPricesService.LinxProdutosTabelasPrecosByIdTabelaAndCodProduto(userName, docCompany, cnpjEmp, idTabela, codProduto);

            return result switch
            {
                true => Ok($"LinxProdutosTabelasPrecos {idTabela} {codProduto} integrated successfully."),
                false => NotFound($"LinxProdutosTabelasPrecos {idTabela} {codProduto} not found in Linx Microvix."),
                null => UnprocessableEntity("Company not registered, store not found or invalid response from Linx Microvix. See the execution log.")
            };
        }
    }
}
