//using Application.Totvs.Abstractions.Services;
//using Application.Totvs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Totvs
{
    [ApiController]
    [Authorize]
    [Route("TotvsJobs/Totvs")]
    public class TotvsController : Controller
    {
        //private readonly IPersonService _personService;
        //private readonly IProductService _productService;
        //private readonly ISellerService _sellerService;
        //private readonly IFiscalService _fiscalService;
        //private readonly IPurchaseOrderService _purchaseOrderlService;
        //private readonly ISalesOrderService _salesOrderlService;
        //private readonly IAccountsPayableService _accountsPayableService;
        //private readonly IGeneralService _generalService;

        //public TotvsController(ISellerService sellerService, IProductService productService, IPersonService personService, IFiscalService fiscalService, IPurchaseOrderService purchaseOrderlService, ISalesOrderService salesOrderlService, IAccountsPayableService accountsPayableService, IGeneralService generalService) =>
        //    (_sellerService, _productService, _personService, _fiscalService, _purchaseOrderlService, _salesOrderlService, _accountsPayableService, _generalService) = (sellerService, productService, personService, fiscalService, purchaseOrderlService, salesOrderlService, accountsPayableService, generalService);

//        [HttpPost("TotvsProducts")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsProducts()
//        {
//            var result = await _productService.PostSearchReferences();
//            return Ok(result);
//        }

//        [HttpPost("TotvsOperations")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsOperations()
//        {
//            var result = await _generalService.SearchOperations();
//            return Ok(result);
//        }

//        [HttpPost("TotvsPrices")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsPrices()
//        {
//            var result = await _productService.PostSearchPrices();
//            return Ok(result);
//        }

//        [HttpPost("TotvsCosts")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsCosts()
//        {
//            var result = await _productService.PostSearchCosts();
//            return Ok(result);
//        }

//        [HttpPost("TotvsBalances")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsBalances()
//        {
//            var result = await _productService.PostSearchBalances();
//            return Ok(result);
//        }

//        [HttpPost("TotvsLegalEntities")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsLegalEntities()
//        {
//            var result = await _personService.PostSearchLegalEntity();
//            return Ok(result);
//        }

//        [HttpPost("TotvsCustomers")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsCustomers()
//        {
//            var result = await _personService.PostSearchCustomers();
//            return Ok(result);
//        }

//        [HttpPost("TotvsBranches")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsBranches()
//        {
//            var result = await _personService.GetBranchesList();
//            return Ok(result);
//        }

//            [HttpPost("TotvsInvoices")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsInvoices()
//        {
//            var result = await _fiscalService.PostSearchInvoices();
//            return Ok(result);
//        }

//        [HttpPost("TotvsPurchaseOrders")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsPurchaseOrders()
//        {
//            var result = await _purchaseOrderlService.PostSearchPurchaseOrders();
//            return Ok(result);
//        }

//        [HttpPost("TotvsSaleOrders")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsSaleOrders()
//        {
//            var result = await _salesOrderlService.PostSearchSalesOrders();
//            return Ok(result);
//        }

//        [HttpPost("TotvsSelers")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsSelers()
//        {
//            var result = await _sellerService.PostSearchSellers();
//            return Ok(result);
//        }

//        [HttpPost("TotvsAccountsPayableDuplicates")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsAccountsPayableDuplicates()
//        {
//            var result = await _accountsPayableService.PostSearchDuplicates();
//            return Ok(result);
//        }

//        [HttpPost("TotvsAccountsPayableComissionsPaid")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult<string?>> TotvsAccountsPayableComissionsPaid()
//        {
//            var result = await _accountsPayableService.PostSearchComissionsPaid();
//            return Ok(result);
//        }
    }
}
