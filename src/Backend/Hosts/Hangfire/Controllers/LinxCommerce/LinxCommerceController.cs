//using Application.LinxCommerce.Abstractions.Services;
//using Domain.LinxCommerce.Entities.Parameters;
//using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxCommerce
{
    [ApiController]
    [Authorize]
    [Route("LinxCommerceJobs/LinxCommerce")]
    public class LinxCommerceController : Controller
    {
//        private readonly LinxCommerceJobParameter _linxCommerceJobParameter;
//        private readonly List<LinxMethods>? _methods;

//        private readonly IConfiguration _configuration;
//        private readonly ICustomerService _customerService;
//        private readonly ISalesRepresentativeService _salesRepresentativeService;
//        private readonly IOrderService _orderService;
//        private readonly ISKUService _skuService;

//        public LinxCommerceController(
//            IConfiguration configuration,
//            ICustomerService customerService,
//            IOrderService orderService,
//            ISalesRepresentativeService salesRepresentativeService,
//            ISKUService skuService
//        )
//        {
//            _configuration = configuration;
//            _customerService = customerService;
//            _skuService = skuService;
//            _orderService = orderService;
//            _salesRepresentativeService = salesRepresentativeService;

//            _linxCommerceJobParameter = new LinxCommerceJobParameter(
//                docMainCompany: _configuration
//                                .GetSection("LinxCommerce")
//                                .GetSection("DocMainCompany")
//                                .Value,

//                schema: "linx_commerce",

//                databaseName: _configuration
//                                .GetSection("ConfigureServer")
//                                .GetSection("Databases")
//                                .GetSection("LinxCommerce")
//                                .Value,

//                untreatedDatabaseName: _configuration
//                                .GetSection("ConfigureServer")
//                                .GetSection("Databases")
//                                .GetSection("Untreated")
//                                .Value,

//                projectName: _configuration
//                                .GetSection("LinxCommerce")
//                                .GetSection("ProjectName")
//                                .Value,

//                parametersInterval: _configuration
//                                .GetSection("LinxMicrovix")
//                                .GetSection("ParametersDateInterval")
//                                .Value,

//                parametersTableName: _configuration
//                                .GetSection("LinxCommerce")
//                                .GetSection("ParametersTableName")
//                                .Value,

//                keyAuthorization: _configuration
//                                .GetSection("LinxCommerce")
//                                .GetSection("Key")
//                                .Value,

//                userAuthentication: _configuration
//                                .GetSection("LinxCommerce")
//                                .GetSection("Authentication")
//                                .Value
//            );

//            _methods = _configuration
//                            .GetSection("LinxCommerce")
//                            .GetSection("Methods")
//                            .Get<List<LinxMethods>>();
//        }

//        #region Date Interval
//        [HttpPost("SearchSkusByDateInterval")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchSkusByDateInterval()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchSKU")
//                .FirstOrDefault();

//            var result = await _skuService.SearchSKUByDateInterval(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchSkusByQueue")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchSkusByQueue()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchSKU")
//                .FirstOrDefault();

//            var result = await _skuService.SearchSKUByQueue(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchProductsByDateInterval")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchProductsByDateInterval()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchProducts")
//                .FirstOrDefault();

//            var result = await _skuService.SearchProductByDateInterval(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchProductsByQueue")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchProductsByQueue()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchProducts")
//                .FirstOrDefault();

//            var result = await _skuService.SearchProductByQueue(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchOrdersByDateInterval")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchOrdersByDateInterval()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchOrders")
//                .FirstOrDefault();

//            var result = await _orderService.SearchOrdersByDateInterval(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchOrdersByQueue")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchOrdersByQueue()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchOrders")
//                .FirstOrDefault();

//            var result = await _orderService.SearchOrdersByQueue(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchOrderStatus")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchOrderStatus()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchOrderStatus")
//                .FirstOrDefault();

//            var result = await _orderService.SearchOrderStatus(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: "OrderStatus"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchSalesRepresentative")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchSalesRepresentative()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchSalesRepresentative")
//                .FirstOrDefault();

//            var result = await _salesRepresentativeService.SearchSalesRepresentative(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: "SaleRepresentative"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchCustomerByDateInterval")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchCustomerByDateInterval()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchCustomer")
//                .FirstOrDefault();

//            var result = await _customerService.SearchCustomerByDateInterval(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: "Customer"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SearchCustomerByQueue")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SearchCustomerByQueue()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchCustomer")
//                .FirstOrDefault();

//            var result = await _customerService.SearchCustomerByQueue(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: "Customer"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("UpdateTrackingNumberOrder")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> UpdateTrackingNumberOrder()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "UpdateTrackingNumberOrder")
//                .FirstOrDefault();

//            var result = await _orderService.UpdateTrackingNumberOrder(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: "UpdateTrackingNumberOrder"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("SaveSalesRepresentative")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> SaveSalesRepresentative()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SaveSalesRepresentative")
//                .FirstOrDefault();

//            var result = await _salesRepresentativeService.SaveSalesRepresentative(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: "SaveSalesRepresentative"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }
//        #endregion

//        #region Individual
//        [HttpPost("GetOrderByNumber")]
//        public async Task<ActionResult> GetOrderByNumber([Required][FromQuery] string? orderNumber)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchOrders")
//                .FirstOrDefault();

//            var result = await _orderService.GetOrderByNumber(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: "Order",
//                    parametersInterval: "parameters_individual"
//                ),
//                orderNumber: orderNumber
//            );

//            return Ok($"Record: {orderNumber} integrated successfully.");
//        }

//        [HttpPost("GetCustomer")]
//        public async Task<ActionResult> GetCustomer([Required][FromQuery] string? customerId)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SearchCustomer")
//                .FirstOrDefault();

//            var result = await _customerService.GetCustomer(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: "Customer",
//                    parametersInterval: "parameters_individual"
//                ),
//                Identifier: customerId
//            );

//            return Ok($"Record: {customerId} integrated successfully.");
//        }

//        [HttpPost("GetSKU")]
//        public async Task<ActionResult> GetSKU([Required][FromQuery] int productId)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "SKU")
//                .FirstOrDefault();

//            var result = await _skuService.GetSKU(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                productID: productId
//            );

//            return Ok($"Record: {productId} integrated successfully.");
//        }
//        #endregion

//        #region Integrity Lock
//        [HttpPost("IntegrityLockOrderItem")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockOrdersRegisters()
//        {
//            var result = await _orderService.IntegrityLockOrdersRegisters(
//                _linxCommerceJobParameter.SetParameters(
//                    jobName: "IntegrityLockOrdersRegisters",
//                    tableName: "Orders",
//                    parametersInterval: "parameters_individual"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }
//        #endregion
    }
}
