using Application.Totvs.Abstractions.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.Totvs
{
    /// <summary>
    /// rotas que disparam as leituras da Totvs. Cada rota entra junto com o seu service, um por vez
    /// (docs/PROPOSTA-PADRONIZACAO-TOTVS.md §6.4 e §9), com o verbo da documentação oficial: POST nas buscas, GET em Branches.
    /// As rotas sob demanda (fase 3) são GET por chave, com o cabeçalho X-User-Name, e ficam registradas em totvs.TotvsRequests.
    /// As escritas (docs/PLANO-TOTVS-METODOS-FALTANTES.md) validam o corpo aqui, antes de chamar a Totvs, e só rodam com
    /// Totvs:Writes:&lt;grupo&gt;:Enabled = true
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
        private readonly ITotvsPersonCatalogsService _totvsPersonCatalogsService;
        private readonly ITotvsRepresentativesService _totvsRepresentativesService;
        private readonly ITotvsProductCatalogsService _totvsProductCatalogsService;
        private readonly ITotvsProductPricesService _totvsProductPricesService;
        private readonly IValidator<Domain.Totvs.Dtos.Writes.AccountsPayable.ChangeInstallmentStage> _changeInstallmentStageValidator;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.Invoices.InvoiceProductSearch> _invoiceProductSearchValidator;
        private readonly ITotvsBonusService _totvsBonusService;
        private readonly IValidator<Domain.Totvs.Dtos.Reads.People.Bonus.ListBalanceBonus> _listBalanceBonusValidator;

        public TotvsController(
            ITotvsPeopleService totvsPeopleService,
            ITotvsSellersService totvsSellersService,
            ITotvsProductsService totvsProductsService,
            ITotvsPurchaseOrdersService totvsPurchaseOrdersService,
            ITotvsSalesOrdersService totvsSalesOrdersService,
            ITotvsAccountsPayableService totvsAccountsPayableService,
            ITotvsInvoicesService totvsInvoicesService,
            ITotvsPersonCatalogsService totvsPersonCatalogsService,
            ITotvsRepresentativesService totvsRepresentativesService,
            ITotvsProductCatalogsService totvsProductCatalogsService,
            ITotvsProductPricesService totvsProductPricesService,
            IValidator<Domain.Totvs.Dtos.Writes.AccountsPayable.ChangeInstallmentStage> changeInstallmentStageValidator,
            IValidator<Domain.Totvs.Dtos.Reads.Invoices.InvoiceProductSearch> invoiceProductSearchValidator,
            ITotvsBonusService totvsBonusService,
            IValidator<Domain.Totvs.Dtos.Reads.People.Bonus.ListBalanceBonus> listBalanceBonusValidator
        ) =>
            (_totvsPeopleService, _totvsSellersService, _totvsProductsService, _totvsPurchaseOrdersService, _totvsSalesOrdersService, _totvsAccountsPayableService, _totvsInvoicesService, _totvsPersonCatalogsService, _totvsRepresentativesService, _totvsProductCatalogsService, _totvsProductPricesService, _changeInstallmentStageValidator, _invoiceProductSearchValidator, _totvsBonusService, _listBalanceBonusValidator) =
            (totvsPeopleService, totvsSellersService, totvsProductsService, totvsPurchaseOrdersService, totvsSalesOrdersService, totvsAccountsPayableService, totvsInvoicesService, totvsPersonCatalogsService, totvsRepresentativesService, totvsProductCatalogsService, totvsProductPricesService, changeInstallmentStageValidator, invoiceProductSearchValidator, totvsBonusService, listBalanceBonusValidator);

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
        /// busca uma pessoa física por código (POST person/v2/individuals/search com personCodeList) e grava; a chamada fica em totvs.TotvsRequests
        /// </summary>
        [HttpGet("Customers/{docCompany}/{code:int:min(1)}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> Customer(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int code
        )
        {
            var result = await _totvsPeopleService.GetCustomerByCode(userName, docCompany, code);

            return result switch
            {
                true => Ok($"Customer {code} integrated successfully."),
                false => NotFound($"Customer {code} not found in Totvs."),
                null => UnprocessableEntity("Company not registered or invalid response from Totvs. See the execution log.")
            };
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
        /// busca uma pessoa jurídica por código (POST person/v2/legal-entities/search com personCodeList) e grava; a chamada fica em totvs.TotvsRequests
        /// </summary>
        [HttpGet("LegalEntities/{docCompany}/{code:int:min(1)}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> LegalEntity(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int code
        )
        {
            var result = await _totvsPeopleService.GetLegalEntityByCode(userName, docCompany, code);

            return result switch
            {
                true => Ok($"Legal entity {code} integrated successfully."),
                false => NotFound($"Legal entity {code} not found in Totvs."),
                null => UnprocessableEntity("Company not registered or invalid response from Totvs. See the execution log.")
            };
        }

        /// <summary>
        /// sincroniza o catálogo de classificações (GET person/v2/classifications)
        /// </summary>
        [HttpGet("People/Classifications")]
        [Tags("TotvsPersonCatalogs")]
        public async Task<ActionResult> Classifications()
        {
            var result = await _totvsPersonCatalogsService.GetClassifications();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de tipos de e-mail (GET person/v2/email-types)
        /// </summary>
        [HttpGet("People/EmailTypes")]
        [Tags("TotvsPersonCatalogs")]
        public async Task<ActionResult> EmailTypes()
        {
            var result = await _totvsPersonCatalogsService.GetEmailTypes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de tipos de telefone (GET person/v2/phone-types)
        /// </summary>
        [HttpGet("People/PhoneTypes")]
        [Tags("TotvsPersonCatalogs")]
        public async Task<ActionResult> PhoneTypes()
        {
            var result = await _totvsPersonCatalogsService.GetPhoneTypes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de tipos de contato (GET person/v2/contact-types)
        /// </summary>
        [HttpGet("People/ContactTypes")]
        [Tags("TotvsPersonCatalogs")]
        public async Task<ActionResult> ContactTypes()
        {
            var result = await _totvsPersonCatalogsService.GetContactTypes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os representantes, com clientes, tabelas de preço, regiões de venda e parâmetros (POST person/v2/representatives/search)
        /// </summary>
        [HttpPost("Representatives")]
        [Tags("TotvsRepresentatives")]
        public async Task<ActionResult> Representatives()
        {
            var result = await _totvsRepresentativesService.PostSearchRepresentatives();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// busca um representante por código (POST person/v2/representatives/search com representativeCodeList) e grava; a chamada fica em totvs.TotvsRequests
        /// </summary>
        [HttpGet("Representatives/{docCompany}/{code:int:min(1)}")]
        [Tags("TotvsRepresentatives")]
        public async Task<ActionResult> Representative(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int code
        )
        {
            var result = await _totvsRepresentativesService.GetRepresentativeByCode(userName, docCompany, code);

            return result switch
            {
                true => Ok($"Representative {code} integrated successfully."),
                false => NotFound($"Representative {code} not found in Totvs."),
                null => UnprocessableEntity("Company not registered or invalid response from Totvs. See the execution log.")
            };
        }

        /// <summary>
        /// sincroniza as recepções de venda (GET person/v2/reception-sale/search), pela data do movimento
        /// </summary>
        [HttpGet("ReceptionSales")]
        [Tags("TotvsRepresentatives")]
        public async Task<ActionResult> ReceptionSales()
        {
            var result = await _totvsRepresentativesService.GetSearchReceptionSale();

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
        /// sincroniza as regiões de atuação, com estados, CEPs e cidades (GET seller/v2/operational-area*)
        /// </summary>
        [HttpGet("OperationalAreas")]
        [Tags("TotvsSellers")]
        public async Task<ActionResult> OperationalAreas()
        {
            var result = await _totvsSellersService.GetOperationalArea();

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
        /// sincroniza os preços de todos os tipos, por loja de cada marca (POST product/v2/prices/search)
        /// </summary>
        [HttpPost("Products/Prices")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductPrices()
        {
            var result = await _totvsProductPricesService.PostSearchPrices();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os custos de todos os tipos, por loja de cada marca (POST product/v2/costs/search)
        /// </summary>
        [HttpPost("Products/Costs")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductCosts()
        {
            var result = await _totvsProductPricesService.PostSearchCosts();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os itens das tabelas de preço vigentes, por loja e escala (POST product/v2/price-tables/search)
        /// </summary>
        [HttpPost("Products/PriceTables")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductPriceTables()
        {
            var result = await _totvsProductPricesService.PostSearchPriceTables();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os itens de promoção de todas as lojas (GET product/v2/promotion)
        /// </summary>
        [HttpGet("Products/Promotions")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductPromotions()
        {
            var result = await _totvsProductPricesService.GetPromotion();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os saldos de todos os tipos, por loja de cada marca, com pendentes e locais (POST product/v2/balances/search)
        /// </summary>
        [HttpPost("Products/Balances")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductBalances()
        {
            var result = await _totvsProductsService.PostSearchBalances();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os produtos similares (GET product/v2/similar)
        /// </summary>
        [HttpGet("Products/Similar")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductSimilar()
        {
            var result = await _totvsProductsService.GetSimilar();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os lotes de todas as lojas (POST product/v2/batch/search)
        /// </summary>
        [HttpPost("Products/Batches")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductBatches()
        {
            var result = await _totvsProductsService.PostSearchBatch();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// confere os produtos de cada marca com as referências gravadas e recupera os que faltam (POST product/v2/product-codes/search)
        /// </summary>
        [HttpPost("Products/ProductCodes")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductCodes()
        {
            var result = await _totvsProductsService.PostSearchProductCodes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de tipos de classificação de produto (GET product/v2/classificationType)
        /// </summary>
        [HttpGet("Products/ClassificationTypes")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductClassificationTypes()
        {
            var result = await _totvsProductCatalogsService.GetClassificationType();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de classificações de produto (GET product/v2/classifications)
        /// </summary>
        [HttpGet("Products/Classifications")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductClassifications()
        {
            var result = await _totvsProductCatalogsService.GetClassifications();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de categorias de produto (GET product/v2/category)
        /// </summary>
        [HttpGet("Products/Categories")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductCategories()
        {
            var result = await _totvsProductCatalogsService.GetCategory();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de grades (GET product/v2/grid)
        /// </summary>
        [HttpGet("Products/Grids")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductGrids()
        {
            var result = await _totvsProductCatalogsService.GetGrid();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de unidades de medida (GET product/v2/measurement-unit)
        /// </summary>
        [HttpGet("Products/MeasurementUnits")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductMeasurementUnits()
        {
            var result = await _totvsProductCatalogsService.GetMeasurementUnit();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de cores (POST product/v2/colors/search)
        /// </summary>
        [HttpPost("Products/Colors")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductColors()
        {
            var result = await _totvsProductCatalogsService.PostSearchColors();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de tipos de campo adicional (GET product/v2/additional-fields-types)
        /// </summary>
        [HttpGet("Products/AdditionalFieldsTypes")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductAdditionalFieldsTypes()
        {
            var result = await _totvsProductCatalogsService.GetAdditionalFieldsTypes();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de configurações de agrupador de produto (GET product/v2/product-grouper-configuration)
        /// </summary>
        [HttpGet("Products/GrouperConfigurations")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductGrouperConfigurations()
        {
            var result = await _totvsProductCatalogsService.GetProductGrouperConfiguration();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de itens de instrução (GET product/v2/instruction-items)
        /// </summary>
        [HttpGet("Products/InstructionItems")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductInstructionItems()
        {
            var result = await _totvsProductCatalogsService.GetInstructionItems();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de composições (POST product/v2/compositions)
        /// </summary>
        [HttpPost("Products/Compositions")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductCompositions()
        {
            var result = await _totvsProductCatalogsService.PostCompositions();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de cabeçalhos de tabela de preço (GET product/v2/price-tables-headers)
        /// </summary>
        [HttpGet("Products/PriceTablesHeaders")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductPriceTablesHeaders()
        {
            var result = await _totvsProductCatalogsService.GetPriceTablesHeaders();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de escalas de tabela de preço (GET product/v2/price-table-scales)
        /// </summary>
        [HttpGet("Products/PriceTableScales")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductPriceTableScales()
        {
            var result = await _totvsProductCatalogsService.GetPriceTableScales();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// busca a referência de um produto por código (POST product/v2/references/search com productCodeList, nas matrizes das marcas) e grava; as chamadas ficam em totvs.TotvsRequests
        /// </summary>
        [HttpGet("Products/{docCompany}/{code:int:min(1)}")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> Product(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int code
        )
        {
            var result = await _totvsProductsService.GetProductByCode(userName, docCompany, code);

            return result switch
            {
                true => Ok($"Product {code} integrated successfully."),
                false => NotFound($"Product {code} not found in Totvs."),
                null => UnprocessableEntity("Company not registered or invalid response from Totvs. See the execution log.")
            };
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
        /// sincroniza o catálogo de tipos de desconto (GET sales-order/v2/discount-type)
        /// </summary>
        [HttpGet("SalesOrderDiscountTypes")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderDiscountTypes()
        {
            var result = await _totvsSalesOrdersService.GetDiscountType();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza o catálogo de classificações de pedido de venda (GET sales-order/v2/classifications)
        /// </summary>
        [HttpGet("SalesOrderClassifications")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderClassifications()
        {
            var result = await _totvsSalesOrdersService.GetClassifications();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os lotes de pedidos de venda (GET sales-order/v2/batch-items)
        /// </summary>
        [HttpGet("SalesOrderBatches")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderBatches()
        {
            var result = await _totvsSalesOrdersService.GetBatchItems();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza as sugestões de faturamento, com pedidos e itens (GET sales-order/v2/billing-suggestions)
        /// </summary>
        [HttpGet("BillingSuggestions")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> BillingSuggestions()
        {
            var result = await _totvsSalesOrdersService.GetBillingSuggestions();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// busca um pedido de venda por loja + número (POST sales-order/v2/orders/search com branchCodeList + orderCodeList) e grava; a chamada fica em totvs.TotvsRequests
        /// </summary>
        [HttpGet("SalesOrders/{docCompany}/{branchCode:int:min(1)}/{orderCode:int:min(1)}")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrder(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            int orderCode
        )
        {
            var result = await _totvsSalesOrdersService.GetSalesOrderByKey(userName, docCompany, branchCode, orderCode);

            return result switch
            {
                true => Ok($"Sales order {branchCode}/{orderCode} integrated successfully."),
                false => NotFound($"Sales order {branchCode}/{orderCode} not found in Totvs."),
                null => UnprocessableEntity("Company not registered or invalid response from Totvs. See the execution log.")
            };
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
        /// troca o estágio de uma parcela a pagar (POST accounts-payable/v2/duplicates/installments/stage). A Totvs valida a
        /// transição (1 → 5 ou 2; 5 → 2 ou 1; 2 → 1 ou 5); a chamada fica em totvs.TotvsRequests
        /// </summary>
        [HttpPost("Duplicates/{docCompany}/Installments/Stage")]
        [Tags("TotvsAccountsPayable")]
        public async Task<ActionResult> DuplicatesInstallmentsStage(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.AccountsPayable.ChangeInstallmentStage body
        )
        {
            var validation = await _changeInstallmentStageValidator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsAccountsPayableService.PostDuplicatesInstallmentsStage(userName, docCompany, body));
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

        /// <summary>
        /// sincroniza as faixas de numeração inutilizadas de todas as lojas (GET fiscal/v2/invoices/disable)
        /// </summary>
        [HttpGet("InvoicesDisable")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> InvoicesDisable()
        {
            var result = await _totvsInvoicesService.GetInvoicesDisable();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// sincroniza os centros de custo, com as classificações (GET fiscal/v2/cost-center)
        /// </summary>
        [HttpGet("CostCenters")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> CostCenters()
        {
            var result = await _totvsInvoicesService.GetCostCenter();

            return Ok($"Records integrated successfully.");
        }

        /// <summary>
        /// busca uma nota fiscal por loja + sequência (POST fiscal/v2/invoices/search com branchCodeList + invoiceSequenceList) e grava; a chamada fica em totvs.TotvsRequests
        /// </summary>
        [HttpGet("Invoices/{docCompany}/{branchCode:int:min(1)}/{invoiceSequence:int:min(1)}")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> Invoice(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            int invoiceSequence
        )
        {
            var result = await _totvsInvoicesService.GetInvoiceByKey(userName, docCompany, branchCode, invoiceSequence);

            return result switch
            {
                true => Ok($"Invoice {branchCode}/{invoiceSequence} integrated successfully."),
                false => NotFound($"Invoice {branchCode}/{invoiceSequence} not found in Totvs."),
                null => UnprocessableEntity("Company not registered or invalid response from Totvs. See the execution log.")
            };
        }

        /// <summary>
        /// devolve o XML de uma NF-e/NFC-e pela chave de acesso (GET fiscal/v2/xml-contents/{accessKey}), sem gravar
        /// </summary>
        [HttpGet("ElectronicInvoices/{docCompany}/{accessKey}/Xml")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> InvoiceXml(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{44}$")] string accessKey
        ) =>
            ReadResponse(await _totvsInvoicesService.GetXmlContents(userName, docCompany, accessKey));

        /// <summary>
        /// devolve o PDF do DANFE de uma NF-e (POST fiscal/v2/danfe-search com o XML do xml-contents); nfeDocumentType 1 retrato
        /// (padrão da Totvs) ou 2 simplificado. A NFC-e (65) dá erro na Totvs (testado): 422 com a mensagem
        /// </summary>
        [HttpGet("ElectronicInvoices/{docCompany}/{accessKey}/Danfe")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> InvoiceDanfe(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{44}$")] string accessKey,
            [FromQuery, Range(1, 2)] int? nfeDocumentType
        ) =>
            ReadResponse(await _totvsInvoicesService.PostSearchDanfe(userName, docCompany, accessKey, nfeDocumentType));

        /// <summary>
        /// devolve o PDF da impressão da transação de uma nota (POST fiscal/v2/transaction/print); template é o modelo do TRAFP004 e
        /// type 1 normal ou 2 richtext
        /// </summary>
        [HttpGet("Invoices/{docCompany}/{branchCode:int:min(1)}/{invoiceSequence:int:min(1)}/Print")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> InvoicePrint(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            int invoiceSequence,
            [FromQuery, Required] DateTime invoiceDate,
            [FromQuery, Required, Range(1, int.MaxValue)] int template,
            [FromQuery, Range(1, 2)] int type = 1
        ) =>
            ReadResponse(await _totvsInvoicesService.PostTransactionPrint(userName, docCompany, branchCode, invoiceSequence, invoiceDate.Date, template, type));

        /// <summary>
        /// devolve os itens de uma nota com transações e notas referenciadas por produto (GET fiscal/v2/invoices/item-detail-search), pela
        /// loja + sequência + data, sem gravar
        /// </summary>
        [HttpGet("Invoices/{docCompany}/{branchCode:int:min(1)}/{invoiceSequence:int:min(1)}/Items")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> InvoiceItems(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            int invoiceSequence,
            [FromQuery, Required] DateTime invoiceDate
        ) =>
            ReadResponse(await _totvsInvoicesService.GetSearchInvoicesItemDetail(userName, docCompany, branchCode, invoiceSequence, invoiceDate.Date, null));

        /// <summary>
        /// devolve os itens de uma NF-e/NFC-e pela chave de acesso (GET fiscal/v2/invoices/item-detail-search), sem gravar
        /// </summary>
        [HttpGet("ElectronicInvoices/{docCompany}/{accessKey}/Items")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> ElectronicInvoiceItems(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^\d{44}$")] string accessKey
        ) =>
            ReadResponse(await _totvsInvoicesService.GetSearchInvoicesItemDetail(userName, docCompany, null, null, null, accessKey));

        /// <summary>
        /// devolve o saldo de produtos em condicional de uma pessoa numa loja (GET fiscal/v2/invoices/pending-conditional-products): personCode
        /// ou personCpfCnpj, um dos dois; 404 sem saldo
        /// </summary>
        [HttpGet("ConditionalProducts/{docCompany}/{branchCode:int:min(1)}")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> ConditionalProducts(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            [FromQuery, Range(1, int.MaxValue)] int? personCode,
            [FromQuery, RegularExpression(@"^(\d{11}|\d{14})$")] string? personCpfCnpj,
            [FromQuery] bool onlyPendingItem = true
        )
        {
            if ((personCode is null) == String.IsNullOrEmpty(personCpfCnpj))
                return BadRequest(new[] { new { field = "personCode", message = "Informe personCode ou personCpfCnpj, um dos dois." } });

            return ReadResponse(await _totvsInvoicesService.GetInvoicesPendingConditionalProducts(userName, docCompany, branchCode, personCode, String.IsNullOrEmpty(personCpfCnpj) ? null : personCpfCnpj, onlyPendingItem));
        }

        /// <summary>
        /// devolve uma página de produtos das notas (POST fiscal/v2/invoice-products/search), com o filtro no corpo: lojas da empresa e pelo
        /// menos produto, nota ou chave de acesso; quem chama pagina
        /// </summary>
        [HttpPost("InvoiceProducts/{docCompany}/Search")]
        [Tags("TotvsInvoices")]
        public async Task<ActionResult> InvoiceProducts(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Reads.Invoices.InvoiceProductSearch body
        )
        {
            var validation = await _invoiceProductSearchValidator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return ReadResponse(await _totvsInvoicesService.PostSearchInvoiceProducts(userName, docCompany, body));
        }

        /// <summary>
        /// devolve os dados de uma loja pelo código (1 a 4 dígitos) ou CNPJ (GET person/v2/branches/{branchId}), sem gravar
        /// </summary>
        [HttpGet("Branches/{docCompany}/{branchId}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> Branch(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [RegularExpression(@"^(\d{1,4}|\d{14})$")] string branchId
        ) =>
            ReadResponse(await _totvsPeopleService.GetBranches(userName, docCompany, branchId));

        /// <summary>
        /// devolve as estatísticas de compra e crédito de um cliente (GET person/v2/person-statistics): customerCode ou customerCpfCnpj, um dos
        /// dois; branchCode repetido (sem ele, todas as lojas da empresa)
        /// </summary>
        [HttpGet("PersonStatistics/{docCompany}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> PersonStatistics(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromQuery, Range(1, int.MaxValue)] int? customerCode,
            [FromQuery, RegularExpression(@"^(\d{11}|\d{14})$")] string? customerCpfCnpj,
            [FromQuery] List<int>? branchCode
        )
        {
            if ((customerCode is null) == String.IsNullOrEmpty(customerCpfCnpj))
                return BadRequest(new[] { new { field = "customerCode", message = "Informe customerCode ou customerCpfCnpj, um dos dois." } });

            return ReadResponse(await _totvsPeopleService.GetPersonStatistics(userName, docCompany, customerCode, String.IsNullOrEmpty(customerCpfCnpj) ? null : customerCpfCnpj, branchCode));
        }

        /// <summary>
        /// devolve o saldo de bônus de uma pessoa nas lojas (POST person/v2/list-balance-bonus), com o corpo validado; sem saldo, 404
        /// </summary>
        [HttpPost("ListBalanceBonus/{docCompany}")]
        [Tags("TotvsBonus")]
        public async Task<ActionResult> ListBalanceBonus(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Reads.People.Bonus.ListBalanceBonus body
        )
        {
            var validation = await _listBalanceBonusValidator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return ReadResponse(await _totvsBonusService.PostListBalanceBonus(userName, docCompany, body));
        }

        /// <summary>
        /// resolve um código (produto, pack, barras, RFID, item de lote) para o produto (GET product/v2/products/{code}), sem gravar
        /// </summary>
        [HttpGet("Products/{docCompany}/Codes/{code}")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductByAnyCode(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [StringLength(40)] string code,
            [FromQuery, Range(1, 9999)] int? branchCode
        ) =>
            ReadResponse(await _totvsProductsService.GetProducts(userName, docCompany, code, branchCode));

        /// <summary>
        /// devolve o kardex de um produto numa loja (GET product/v2/kardex-movement): todos os parâmetros obrigatórios, até 180 dias
        /// </summary>
        [HttpGet("Products/{docCompany}/Kardex")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductKardex(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromQuery, Required, Range(1, int.MaxValue)] int productCode,
            [FromQuery, Required, Range(1, 9999)] int branchCode,
            [FromQuery, Required] DateTime startDate,
            [FromQuery, Required] DateTime endDate,
            [FromQuery, Required, Range(1, 999)] int balanceType
        )
        {
            if (startDate.Date > endDate.Date || (endDate.Date - startDate.Date).TotalDays > 180)
                return BadRequest(new[] { new { field = "startDate", message = "O período vai do startDate ao endDate, com no máximo 180 dias." } });

            return ReadResponse(await _totvsProductsService.GetKardexMovement(userName, docCompany, productCode, branchCode, startDate.Date, endDate.Date, balanceType));
        }

        /// <summary>
        /// devolve a grade de frequência de um produto (GET product/v2/grid-frequency), uma página por chamada
        /// </summary>
        [HttpGet("Products/{docCompany}/{productCode:int:min(1)}/GridFrequency")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductGridFrequency(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int productCode,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 100)] int pageSize = 100
        ) =>
            ReadResponse(await _totvsProductsService.GetGridFrequency(userName, docCompany, productCode, page, pageSize));

        /// <summary>
        /// devolve as conversões de unidade de um produto ou grupo (GET product/v2/measurement-conversions): productCode ou groupSequence
        /// </summary>
        [HttpGet("Products/{docCompany}/MeasurementConversions")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductMeasurementConversions(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromQuery, Range(1, int.MaxValue)] int? productCode,
            [FromQuery, Range(1, int.MaxValue)] int? groupSequence,
            [FromQuery] bool? onlyDefault
        )
        {
            if ((productCode is null) == (groupSequence is null))
                return BadRequest(new[] { new { field = "productCode", message = "Informe productCode ou groupSequence, um dos dois." } });

            return ReadResponse(await _totvsProductsService.GetMeasurementConversions(userName, docCompany, productCode, groupSequence, onlyDefault));
        }

        /// <summary>
        /// devolve a composição por produto (GET product/v2/composition-product): pelo menos uma das listas
        /// </summary>
        [HttpGet("Products/{docCompany}/CompositionProduct")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductCompositionProduct(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromQuery] List<int>? productCodeList,
            [FromQuery] List<int>? groupCodeList,
            [FromQuery] List<string>? referenceCodeList,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 100)] int pageSize = 100
        )
        {
            if ((productCodeList?.Count ?? 0) + (groupCodeList?.Count ?? 0) + (referenceCodeList?.Count ?? 0) == 0)
                return BadRequest(new[] { new { field = "productCodeList", message = "Informe productCodeList, groupCodeList ou referenceCodeList." } });

            return ReadResponse(await _totvsProductsService.GetCompositionProduct(userName, docCompany, productCodeList, groupCodeList, referenceCodeList, page, pageSize));
        }

        /// <summary>
        /// devolve a composição por grupo (GET product/v2/composition-group-product): pelo menos uma das listas
        /// </summary>
        [HttpGet("Products/{docCompany}/CompositionGroupProduct")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductCompositionGroupProduct(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromQuery] List<int>? productCodeList,
            [FromQuery] List<int>? groupCodeList,
            [FromQuery] List<string>? referenceCodeList,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 100)] int pageSize = 100
        )
        {
            if ((productCodeList?.Count ?? 0) + (groupCodeList?.Count ?? 0) + (referenceCodeList?.Count ?? 0) == 0)
                return BadRequest(new[] { new { field = "productCodeList", message = "Informe productCodeList, groupCodeList ou referenceCodeList." } });

            return ReadResponse(await _totvsProductsService.GetCompositionGroupProduct(userName, docCompany, productCodeList, groupCodeList, referenceCodeList, page, pageSize));
        }

        /// <summary>
        /// busca um pedido de compra por loja + número (POST purchase-order/v2/search com branchCodeList + orderCodeList) e grava; a chamada fica em totvs.TotvsRequests
        /// </summary>
        [HttpGet("PurchaseOrders/{docCompany}/{branchCode:int:min(1)}/{orderCode:int:min(1)}")]
        [Tags("TotvsPurchaseOrders")]
        public async Task<ActionResult> PurchaseOrder(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            int orderCode
        )
        {
            var result = await _totvsPurchaseOrdersService.GetPurchaseOrderByKey(userName, docCompany, branchCode, orderCode);

            return result switch
            {
                true => Ok($"Purchase order {branchCode}/{orderCode} integrated successfully."),
                false => NotFound($"Purchase order {branchCode}/{orderCode} not found in Totvs."),
                null => UnprocessableEntity("Company not registered or invalid response from Totvs. See the execution log.")
            };
        }

        /// <summary>
        /// devolve as notas de um pedido de venda (GET sales-order/v2/invoices), sem gravar; invoiceStatus repetido filtra a situação
        /// </summary>
        [HttpGet("SalesOrders/{docCompany}/{branchCode:int:min(1)}/{orderCode:int:min(1)}/Invoices")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderInvoices(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            int orderCode,
            [FromQuery] List<string>? invoiceStatus
        ) =>
            ReadResponse(await _totvsSalesOrdersService.GetInvoices(userName, docCompany, branchCode, orderCode, invoiceStatus is { Count: > 0 } ? invoiceStatus : null));

        /// <summary>
        /// devolve os itens pendentes de um pedido de venda, com os preços líquidos (GET sales-order/v2/pending-items), sem gravar
        /// </summary>
        [HttpGet("SalesOrders/{docCompany}/{branchCode:int:min(1)}/{orderCode:int:min(1)}/PendingItems")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderPendingItems(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            int orderCode
        ) =>
            ReadResponse(await _totvsSalesOrdersService.GetPendingItems(userName, docCompany, branchCode, orderCode));

        /// <summary>
        /// devolve os itens de lote ligados a um pedido de venda (GET sales-order/v2/changed-batch-items), sem gravar; sem período, desde 2000
        /// </summary>
        [HttpGet("SalesOrders/{docCompany}/{branchCode:int:min(1)}/{orderCode:int:min(1)}/ChangedBatchItems")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderChangedBatchItems(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int branchCode,
            int orderCode,
            [FromQuery, Range(1, int.MaxValue)] int? batchCode,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate
        ) =>
            ReadResponse(await _totvsSalesOrdersService.GetChangedBatchItems(userName, docCompany, branchCode, orderCode, batchCode, startDate, endDate));

        /// <summary>
        /// inclui mensagens para uma pessoa (POST person/v2/messages), exibidas ao atender o cliente. A resposta traz a sequence de cada
        /// mensagem, a única fonte para alterar ou excluir depois (PE8); o mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("Messages/{docCompany}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> Messages(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.People.Messages.InsertMessagePerson body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.People.Messages.InsertMessagePerson> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPeopleService.PostMessages(userName, docCompany, body));
        }

        /// <summary>
        /// altera uma mensagem da pessoa (PUT person/v2/messages/change) pela pessoa e sequence: a descrição vai sempre (a Totvs recusa sem
        /// ela), com a situação (3 = cancelada, o caminho normal para tirar uma mensagem) ou a prioridade
        /// </summary>
        [HttpPut("Messages/{docCompany}/Change")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> ChangeMessages(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.People.Messages.UpdateMessagePerson body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.People.Messages.UpdateMessagePerson> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPeopleService.PutChangeMessages(userName, docCompany, body));
        }

        /// <summary>
        /// exclui uma mensagem da pessoa em definitivo (DELETE person/v2/messages). A Totvs só exclui mensagem não lida (StatusNotPermited,
        /// testado em homologação). Sem leitura de mensagens na API para conferir antes: prefira o PUT com status 3 (cancelada)
        /// </summary>
        [HttpDelete("Messages/{docCompany}/{personCode:int:range(1,999999999)}/{sequence:int:min(1)}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> DeleteMessages(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            int personCode,
            int sequence
        ) =>
            WriteResponse(await _totvsPeopleService.DeleteMessages(userName, docCompany, personCode, sequence));

        /// <summary>
        /// inclui ou altera uma cor (POST product/v2/colors). Um code que já existe é alterado, e o nome novo vale em todas as referências
        /// que usam a cor; as lojas de colorInfoAdditionals precisam ser da empresa
        /// </summary>
        [HttpPost("Products/{docCompany}/Colors")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> Colors(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.Colors.UpsertColor body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.Colors.UpsertColor> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductCatalogsService.PostColors(userName, docCompany, body));
        }

        /// <summary>
        /// inclui uma grade de tamanhos (POST product/v2/grids); a resposta traz o gridCode. Não há rota para alterar ou excluir grade, e o
        /// mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("Products/{docCompany}/Grids")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> Grids(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.Grids.InsertGrid body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.Grids.InsertGrid> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductCatalogsService.PostGrids(userName, docCompany, body));
        }

        /// <summary>
        /// põe um pedido de compra em andamento (1) ou bloqueado (5) (POST purchase-order/v2/orders/change-status). Com o pedido no banco, a
        /// troca só vale entre essas duas situações (422 sem chamar a Totvs nos demais casos)
        /// </summary>
        [HttpPost("PurchaseOrders/{docCompany}/ChangeStatus")]
        [Tags("TotvsPurchaseOrders")]
        public async Task<ActionResult> PurchaseOrdersChangeStatus(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.PurchaseOrders.UpdateOrderStatus body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.PurchaseOrders.UpdateOrderStatus> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPurchaseOrdersService.PostPurchaseOrdersChangeStatus(userName, docCompany, body));
        }

        /// <summary>
        /// altera os dados adicionais de um pedido de venda (POST sales-order/v2/additional-order): rastreio, estágio do e-commerce, entrega
        /// antecipada (entryType N ou S), tipo de experiência. O pedido vai por orderId ou por branchCode + orderCode
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Additional")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderAdditional(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderAdditional body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderAdditional> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostAdditionalOrder(userName, docCompany, body));
        }

        /// <summary>
        /// altera transportadora, redespacho, frete e endereço de entrega de um pedido de venda (POST sales-order/v2/shipping-order); o frete
        /// muda o total do pedido. A Totvs recusa em pedido bloqueado e em pedido do e-commerce (InvalidStatusUpdate, CannotChangedOrderEcommerce)
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Shipping")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderShipping(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderShipping body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderShipping> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostShippingOrder(userName, docCompany, body));
        }

        /// <summary>
        /// inclui linhas de observação num pedido de venda e na nota (POST sales-order/v2/observations-order). Repetir duplica as linhas: o
        /// mesmo corpo em 10 minutos volta 409. A Totvs recusa em pedido do e-commerce (CannotChangedOrderEcommerce)
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Observations")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderObservations(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderObservations body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderObservations> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostObservationsOrder(userName, docCompany, body));
        }

        /// <summary>
        /// altera a medida de rolo e a previsão de faturamento dos itens de um pedido de venda (POST sales-order/v2/additional-items), com o
        /// produto por código ou SKU
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Items/Additional")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderItemsAdditional(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsAdditional body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsAdditional> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostAdditionalItems(userName, docCompany, body));
        }

        /// <summary>
        /// resposta das leituras sem persistência (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11): 200 com o JSON ou o arquivo; 404 quando
        /// a Totvs não acha; 429 com Retry-After; 502 para 5xx, falta de resposta ou JSON inválido; 422 nos demais (empresa não cadastrada,
        /// 400 da Totvs)
        /// </summary>
        private ActionResult ReadResponse(Domain.Totvs.Dtos.Writes.Result result)
        {
            if (result.success)
                return result.file is not null ? File(result.file, result.contentType ?? "application/octet-stream", result.fileName) : Ok(result);

            if (result.statusCode == StatusCodes.Status404NotFound)
                return NotFound(result);

            return WriteResponse(result);
        }

        /// <summary>
        /// resposta das escritas (docs/PLANO-TOTVS-METODOS-FALTANTES.md §1.2): 200 aceita; 429 com Retry-After; 409 na repetição recusada
        /// (T5); 502 quando a Totvs foi chamada e não respondeu ou respondeu 5xx (a escrita pode ter sido feita); 422 nos demais (empresa
        /// ou loja não cadastrada, escrita desligada, recusa da Totvs)
        /// </summary>
        private ActionResult WriteResponse(Domain.Totvs.Dtos.Writes.Result result)
        {
            if (result.success)
                return Ok(result);

            if (result.statusCode == StatusCodes.Status429TooManyRequests)
            {
                if (result.retryAfterSeconds is int seconds)
                    Response.Headers.RetryAfter = seconds.ToString();

                return StatusCode(StatusCodes.Status429TooManyRequests, result);
            }

            if (result.statusCode == StatusCodes.Status409Conflict)
                return Conflict(result);

            if (result.apiCalled && (result.statusCode == 0 || result.statusCode >= 500))
                return StatusCode(StatusCodes.Status502BadGateway, result);

            return UnprocessableEntity(result);
        }
    }
}
