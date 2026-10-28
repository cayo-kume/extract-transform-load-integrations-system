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
        /// inclui uma duplicata a pagar com as parcelas (POST accounts-payable/v2/duplicates). A loja vai pelo CNPJ; a duplicata chega ao banco
        /// pelo próximo job de duplicatas. O mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("Duplicates/{docCompany}")]
        [Tags("TotvsAccountsPayable")]
        public async Task<ActionResult> PostDuplicates(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.AccountsPayable.Duplicate body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.AccountsPayable.Duplicate> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsAccountsPayableService.PostDuplicates(userName, docCompany, body));
        }

        /// <summary>
        /// agrupa duas ou mais parcelas numa duplicata nova (POST accounts-payable/v2/group-duplicates); a resposta traz a chave da duplicata
        /// gerada. Só parcelas reais: a Totvs recusa as de previsão (testado em homologação). O mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("GroupDuplicates/{docCompany}")]
        [Tags("TotvsAccountsPayable")]
        public async Task<ActionResult> PostGroupDuplicates(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.AccountsPayable.GroupDuplicate body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.AccountsPayable.GroupDuplicate> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsAccountsPayableService.PostGroupDuplicates(userName, docCompany, body));
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
        /// gera crédito de bônus desconto para o cliente (POST person/v2/bonus-due-date). Efeito financeiro e sem estorno pela API: só com
        /// Totvs:Writes:Bonus:Enabled; o mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("BonusDueDate/{docCompany}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> BonusDueDate(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.People.Bonus.BonusDueDate body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.People.Bonus.BonusDueDate> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsBonusService.PostBonusDueDate(userName, docCompany, body));
        }
        /// <summary>
        /// consome saldo de bônus do cliente numa loja (POST person/v2/bonus-consume). Com o personCpfCnpj, o saldo da loja é conferido antes (422
        /// se menor). Efeito financeiro e sem estorno: só com Totvs:Writes:Bonus:Enabled; o mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("BonusConsume/{docCompany}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> BonusConsume(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.People.Bonus.BonusConsumption body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.People.Bonus.BonusConsumption> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsBonusService.PostBonusConsume(userName, docCompany, body));
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
        /// cria ou altera um cliente pessoa física pelo cpf (POST person/v2/individual-customers); a resposta traz o customerCode. Campo ausente
        /// não muda; "" e 0 zeram; os enums vão pelo nome. Altera cliente em uso nas lojas, inclusive bloqueio e limite de crédito. Com o
        /// parâmetro IN_OBRIGATORIO_ENDERECO do ERP, a inclusão exige pelo menos um endereço (a Totvs recusa com 400)
        /// </summary>
        [HttpPost("IndividualCustomers/{docCompany}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> IndividualCustomers(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.People.Customers.IndividualCustomer body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.People.Customers.IndividualCustomer> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPeopleService.PostIndividualCustomers(userName, docCompany, body));
        }

        /// <summary>
        /// cria ou altera um cliente pessoa jurídica pelo cnpj (POST person/v2/legal-customers); a resposta traz o customerCode. Mesmas regras do
        /// cliente pessoa física
        /// </summary>
        [HttpPost("LegalCustomers/{docCompany}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> LegalCustomers(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.People.Customers.LegalCustomer body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.People.Customers.LegalCustomer> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPeopleService.PostLegalCustomers(userName, docCompany, body));
        }

        /// <summary>
        /// relaciona clientes a um representante (POST person/v2/customer-representatives): muda a carteira, que define comissão e roteamento de
        /// pedidos, e a API não tem rota para desfazer
        /// </summary>
        [HttpPost("CustomerRepresentatives/{docCompany}")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> CustomerRepresentatives(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.People.Representatives.CustomerRepresentative body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.People.Representatives.CustomerRepresentative> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsRepresentativesService.PostCustomerRepresentatives(userName, docCompany, body));
        }

        /// <summary>
        /// tira classificações de uma pessoa (POST person/v2/classification-relationship/remove), o par da lista classifications das escritas de
        /// cliente, que só inclui
        /// </summary>
        [HttpPost("ClassificationRelationship/{docCompany}/Remove")]
        [Tags("TotvsPeople")]
        public async Task<ActionResult> RemoveClassificationRelationship(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.People.Classifications.RemoveClassificationRelationship body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.People.Classifications.RemoveClassificationRelationship> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPeopleService.PostRemoveClassificationRelationship(userName, docCompany, body));
        }

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
        /// inclui ou altera um tipo de classificação de produto e as classificações dele (POST product/v2/classifications). Renomear muda a
        /// categorização de todos os produtos do tipo; o tipo 6000 (marca) é recusado (PR10)
        /// </summary>
        [HttpPost("Products/{docCompany}/Classifications")]
        [Tags("TotvsProductCatalogs")]
        public async Task<ActionResult> ProductClassifications(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.Classifications.UpsertTypeClassification body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.Classifications.UpsertTypeClassification> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductCatalogsService.PostClassifications(userName, docCompany, body));
        }
        /// <summary>
        /// inclui códigos de barras nos produtos (POST product/v2/barcodes). O código padrão (isMainCode) é o SKU das integrações de e-commerce
        /// </summary>
        [HttpPost("Products/{docCompany}/BarCodes")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductBarCodes(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.BarCodes.InsertProductBarCodes body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.BarCodes.InsertProductBarCodes> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductsService.PostBarcodes(userName, docCompany, body));
        }
        /// <summary>
        /// altera a quantidade e o padrão de códigos de barras existentes (POST product/v2/barcodes/update); o código não muda
        /// </summary>
        [HttpPost("Products/{docCompany}/BarCodes/Update")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductBarCodesUpdate(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.BarCodes.UpdateProductBarCodes body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.BarCodes.UpdateProductBarCodes> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductsService.PostUpdateBarcodes(userName, docCompany, body));
        }
        /// <summary>
        /// inclui classificações em produtos (productCode) ou grupos (referenceId) (POST product/v2/classification-relationship/create); o tipo 6000
        /// (marca) é recusado (PR10)
        /// </summary>
        [HttpPost("Products/{docCompany}/ClassificationRelationships/Create")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductClassificationRelationshipsCreate(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.Classifications.ProductClassifications body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.Classifications.ProductClassifications> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductsService.PostCreateClassificationRelationship(userName, docCompany, body));
        }
        /// <summary>
        /// altera classificações de produtos ou grupos (POST product/v2/classification-relationship/update); o tipo 6000 (marca) é recusado (PR10)
        /// </summary>
        [HttpPost("Products/{docCompany}/ClassificationRelationships/Update")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductClassificationRelationshipsUpdate(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.Classifications.ProductClassifications body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.Classifications.ProductClassifications> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductsService.PostUpdateClassificationRelationship(userName, docCompany, body));
        }
        /// <summary>
        /// inclui uma referência com os SKUs (POST product/v2/references); a resposta traz os productCode gerados. Cada SKU leva a classificação 6000
        /// (marca); preços e custos são recusados (PR11). Sem rota de exclusão: o mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("Products/{docCompany}/References")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductReferences(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.References.InsertReference body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.References.InsertReference> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductsService.PostReferences(userName, docCompany, body));
        }
        /// <summary>
        /// inclui lotes e itens de lote (POST product/v2/batch/create); sem batchCode a Totvs gera o número. O mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("Products/{docCompany}/Batches/Create")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductBatchesCreate(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.Batches.InsertBatches body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.Batches.InsertBatches> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductsService.PostCreateBatch(userName, docCompany, body));
        }
        /// <summary>
        /// altera dados de itens de lote (PUT product/v2/batch-info), cada item por loja + lote + sequência ou pelo lote do cliente
        /// </summary>
        [HttpPut("Products/{docCompany}/BatchInfo")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductBatchInfo(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.Batches.UpdateBatchInfo body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.Batches.UpdateBatchInfo> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsProductsService.PutBatchInfo(userName, docCompany, body));
        }
        /// <summary>
        /// troca os pedidos de venda preferenciais de um item de lote (POST product/v2/batch-preferentialsalesorder). A Totvs apaga antes todos
        /// os do item, então uma lista vazia só apaga: ela exige ?clear=true
        /// </summary>
        [HttpPost("Products/{docCompany}/BatchPreferentialSalesOrder")]
        [Tags("TotvsProducts")]
        public async Task<ActionResult> ProductBatchPreferentialSalesOrder(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.Products.Batches.BatchPreferentialSalesOrder body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.Products.Batches.BatchPreferentialSalesOrder> validator,
            [FromQuery] bool clear = false
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            if (!clear && body.preferentialSalesOrder is not { Count: > 0 })
                return BadRequest(new[] { new { field = "preferentialSalesOrder", message = "Lista vazia só apaga os pedidos preferenciais do item: para isso, chame com ?clear=true." } });

            return WriteResponse(await _totvsProductsService.PostBatchPreferentialSalesOrder(userName, docCompany, body));
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
        /// inclui um pedido de compra (POST purchase-order/v2/orders); a resposta traz loja e número. O supplierOrderCode é obrigatório: um pedido
        /// do mesmo fornecedor com ele já gravado volta 422, e o mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("PurchaseOrders/{docCompany}")]
        [Tags("TotvsPurchaseOrders")]
        public async Task<ActionResult> PostPurchaseOrders(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.PurchaseOrders.InsertPurchaseOrder body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.PurchaseOrders.InsertPurchaseOrder> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPurchaseOrdersService.PostPurchaseOrders(userName, docCompany, body));
        }
        /// <summary>
        /// troca a quantidade pedida dos itens de um pedido de compra (POST purchase-order/v2/quantity-items). Com o pedido no banco, atendido ou
        /// cancelado, produto fora do pedido e quantidade abaixo da recebida voltam 422 sem chamar a Totvs
        /// </summary>
        [HttpPost("PurchaseOrders/{docCompany}/QuantityItems")]
        [Tags("TotvsPurchaseOrders")]
        public async Task<ActionResult> PurchaseOrdersQuantityItems(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.PurchaseOrders.RequestedQuantity body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.PurchaseOrders.RequestedQuantity> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPurchaseOrdersService.PostPurchaseOrdersQuantityItems(userName, docCompany, body));
        }
        /// <summary>
        /// cria ou troca o vínculo de um item de lote com um pedido de compra (POST purchase-order/v2/purchase-order/batch-link). Lote, pedido ou nota
        /// não encontrados voltam 404; conflito de vínculo, 409
        /// </summary>
        [HttpPost("PurchaseOrders/{docCompany}/BatchLink")]
        [Tags("TotvsPurchaseOrders")]
        public async Task<ActionResult> PurchaseOrderBatchLink(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.PurchaseOrders.BatchLink body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.PurchaseOrders.BatchLink> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsPurchaseOrdersService.PostPurchaseOrderBatchLink(userName, docCompany, body));
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
        /// muda a situação do pedido de venda (POST sales-order/v2/orders/change-status) pelo nome: InProgress, BillingReleased (libera para
        /// faturar), Blocked, InComposition ou InAnalysis
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Status")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderStatus(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderStatusChange body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderStatusChange> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostOrdersChangeStatus(userName, docCompany, body));
        }
        /// <summary>
        /// altera a capa do pedido de venda (POST sales-order/v2/header): prazo médio, operação, tabela de preço e prioridade. Com o pedido no banco,
        /// operação e tabela de preço só mudam em andamento ou bloqueado
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Header")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderHeader(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderHeader body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderHeader> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostHeader(userName, docCompany, body));
        }
        /// <summary>
        /// inclui itens no pedido de venda (POST sales-order/v2/items): valor e reserva. O mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Items")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderItems(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsAdd body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsAdd> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostItems(userName, docCompany, body));
        }
        /// <summary>
        /// aumenta a quantidade dos itens do pedido de venda (POST sales-order/v2/quantity-items); com o item no banco, a nova precisa ser maior que
        /// a atual (reduzir é pelo cancelamento de itens)
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Items/Quantity")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderItemsQuantity(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsQuantity body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemsQuantity> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostQuantityItems(userName, docCompany, body));
        }
        /// <summary>
        /// liga contagens ao pedido de venda (POST sales-order/v2/relationship-counts). O mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/Counts")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderCounts(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderCounts body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderCounts> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostRelationshipCounts(userName, docCompany, body));
        }
        /// <summary>
        /// liga itens de lote aos itens do pedido de venda (POST sales-order/v2/relationship-batch-items); com isUpdateQuantity, a quantidade do
        /// pedido muda. O mesmo corpo em 10 minutos volta 409
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/BatchItems")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderBatchItems(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItems body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItems> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostRelationshipBatchItems(userName, docCompany, body));
        }
        /// <summary>
        /// o ERP escolhe e grava os itens de lote do pedido pelo nuance de cor (POST sales-order/v2/suggest-batch-items-nuance). O mesmo pedido em
        /// 10 minutos volta 409
        /// </summary>
        [HttpPost("SalesOrders/{docCompany}/BatchItems/SuggestNuance")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> SalesOrderBatchItemsNuance(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromBody] Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItemsNuance body,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItemsNuance> validator
        )
        {
            var validation = await validator.ValidateAsync(body);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.PostSuggestBatchItemsNuance(userName, docCompany, body));
        }
        /// <summary>
        /// remove um item do pedido de venda (DELETE sales-order/v2/items); pedido e produto na query (branchCode + orderCode ou orderId; productCode
        /// ou productSku)
        /// </summary>
        [HttpDelete("SalesOrders/{docCompany}/Items")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> DeleteSalesOrderItem(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromQuery] Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemDelete parameters,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderItemDelete> validator
        )
        {
            var validation = await validator.ValidateAsync(parameters);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.DeleteItems(userName, docCompany, parameters));
        }
        /// <summary>
        /// remove o vínculo de um item de lote com o item do pedido (DELETE sales-order/v2/batch-items); pedido, produto e batchSequence na query
        /// </summary>
        [HttpDelete("SalesOrders/{docCompany}/BatchItems")]
        [Tags("TotvsSalesOrders")]
        public async Task<ActionResult> DeleteSalesOrderBatchItem(
            [FromHeader(Name = "X-User-Name"), Required, StringLength(100)] string userName,
            [RegularExpression(@"^\d{14}$")] string docCompany,
            [FromQuery] Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItemDelete parameters,
            [FromServices] IValidator<Domain.Totvs.Dtos.Writes.SalesOrders.OrderBatchItemDelete> validator
        )
        {
            var validation = await validator.ValidateAsync(parameters);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage }));

            return WriteResponse(await _totvsSalesOrdersService.DeleteBatchItems(userName, docCompany, parameters));
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

            // a Totvs disse que o registro não existe (lote, pedido ou nota do batch-link, DP5)
            if (result.apiCalled && result.statusCode == StatusCodes.Status404NotFound)
                return NotFound(result);

            if (result.apiCalled && (result.statusCode == 0 || result.statusCode >= 500))
                return StatusCode(StatusCodes.Status502BadGateway, result);

            return UnprocessableEntity(result);
        }
    }
}
