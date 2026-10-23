using Application.Totvs.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.Totvs
{
    /// <summary>
    /// rotas que disparam as leituras da Totvs. Cada rota entra junto com o seu service, um por vez
    /// (docs/PROPOSTA-PADRONIZACAO-TOTVS.md §6.4 e §9), com o verbo da documentação oficial: POST nas buscas, GET em Branches
    /// </summary>
    [ApiController]
    //[Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("TotvsJobs/Totvs")]
    public class TotvsController : Controller
    {
        private readonly ITotvsPeopleService _totvsPeopleService;
        private readonly ITotvsSellersService _totvsSellersService;
        private readonly ITotvsProductsService _totvsProductsService;
        private readonly ITotvsPurchaseOrdersService _totvsPurchaseOrdersService;
        private readonly ITotvsSalesOrdersService _totvsSalesOrdersService;
        private readonly ITotvsAccountsPayableService _totvsAccountsPayableService;
        private readonly ITotvsInvoicesService _totvsInvoicesService;

        public TotvsController(
            ITotvsPeopleService totvsPeopleService,
            ITotvsSellersService totvsSellersService,
            ITotvsProductsService totvsProductsService,
            ITotvsPurchaseOrdersService totvsPurchaseOrdersService,
            ITotvsSalesOrdersService totvsSalesOrdersService,
            ITotvsAccountsPayableService totvsAccountsPayableService,
            ITotvsInvoicesService totvsInvoicesService
        ) =>
            (_totvsPeopleService, _totvsSellersService, _totvsProductsService, _totvsPurchaseOrdersService, _totvsSalesOrdersService, _totvsAccountsPayableService, _totvsInvoicesService) =
            (totvsPeopleService, totvsSellersService, totvsProductsService, totvsPurchaseOrdersService, totvsSalesOrdersService, totvsAccountsPayableService, totvsInvoicesService);

        /// <summary>
        /// sincroniza as empresas (lojas) da holding (GET person/v2/branchesList)
        /// </summary>
        [HttpGet("Branches")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> Branches()
        {
            var result = await _totvsPeopleService.GetBranchesList();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza as pessoas físicas (POST person/v2/individuals/search)
        /// </summary>
        [HttpPost("Customers")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> Customers()
        {
            var result = await _totvsPeopleService.PostSearchCustomers();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza as pessoas jurídicas (POST person/v2/legal-entities/search)
        /// </summary>
        [HttpPost("LegalEntities")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> LegalEntities()
        {
            var result = await _totvsPeopleService.PostSearchLegalEntity();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os vendedores de todas as lojas da holding (POST seller/v2/search)
        /// </summary>
        [HttpPost("Sellers")]
        [Tags("TotvsSellers")]
        public async Task<ActionResult> Sellers()
        {
            var result = await _totvsSellersService.PostSearchSellers();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza as referências e produtos das marcas (POST product/v2/references/search), uma chamada por matriz
        /// </summary>
        [HttpPost("Products")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> Products()
        {
            var result = await _totvsProductsService.PostSearchReferences();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os pedidos de compra de todas as lojas da holding (POST purchase-order/v2/search)
        /// </summary>
        [HttpPost("PurchaseOrders")]
        [Tags("TotvsPurchaseOrders")]
        public async Task<ActionResult> PurchaseOrders()
        {
            var result = await _totvsPurchaseOrdersService.PostSearchPurchaseOrders();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os pedidos de venda de todas as lojas da holding (POST sales-order/v2/orders/search)
        /// </summary>
        [HttpPost("SalesOrders")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrders()
        {
            var result = await _totvsSalesOrdersService.PostSearchSalesOrders();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza as duplicatas (parcelas) a pagar de todas as lojas da holding (POST accounts-payable/v2/duplicates/search)
        /// </summary>
        [HttpPost("Duplicates")]
        [Tags("TotvsAccountsPayable")]
        public async Task<ActionResult> Duplicates()
        {
            var result = await _totvsAccountsPayableService.PostSearchDuplicates();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os fechamentos de comissão de todas as lojas da holding (POST accounts-payable/v2/comissions-paid/search)
        /// </summary>
        [HttpPost("ComissionsPaid")]
        [Tags("TotvsAccountsPayable")]
        public async Task<ActionResult> ComissionsPaid()
        {
            var result = await _totvsAccountsPayableService.PostSearchComissionsPaid();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza as notas fiscais de todas as lojas da holding (POST fiscal/v2/invoices/search)
        /// </summary>
        [HttpPost("Invoices")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> Invoices()
        {
            var result = await _totvsInvoicesService.PostSearchInvoices();

            return Ok($"Records integrated successfully.");
        }
    }
}
