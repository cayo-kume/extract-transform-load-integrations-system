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
        private readonly IValidator<Domain.Totvs.Dtos.Writes.AccountsPayable.ChangeInstallmentStage> _changeInstallmentStageValidator;

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
            IValidator<Domain.Totvs.Dtos.Writes.AccountsPayable.ChangeInstallmentStage> changeInstallmentStageValidator
        ) =>
            (_totvsPeopleService, _totvsSellersService, _totvsProductsService, _totvsPurchaseOrdersService, _totvsSalesOrdersService, _totvsAccountsPayableService, _totvsInvoicesService, _totvsPersonCatalogsService, _totvsRepresentativesService, _changeInstallmentStageValidator) =
            (totvsPeopleService, totvsSellersService, totvsProductsService, totvsPurchaseOrdersService, totvsSalesOrdersService, totvsAccountsPayableService, totvsInvoicesService, totvsPersonCatalogsService, totvsRepresentativesService, changeInstallmentStageValidator);

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
        /// resposta das escritas (docs/PLANO-TOTVS-METODOS-FALTANTES.md §1.2): 200 aceita; 429 com Retry-After; 502 quando a
        /// Totvs foi chamada e não respondeu ou respondeu 5xx (a escrita pode ter sido feita); 422 nos demais (empresa ou loja
        /// não cadastrada, escrita desligada, recusa da Totvs)
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

            if (result.apiCalled && (result.statusCode == 0 || result.statusCode >= 500))
                return StatusCode(StatusCodes.Status502BadGateway, result);

            return UnprocessableEntity(result);
        }
    }
}
