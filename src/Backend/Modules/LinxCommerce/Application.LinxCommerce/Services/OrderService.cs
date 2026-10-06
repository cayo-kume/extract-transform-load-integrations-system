using Application.Core.Abstractions.Services;
using Application.LinxCommerce.Abstractions.Services;
using Domain.Core.Abstractions.Api;
using Domain.Core.Enums;
using Domain.LinxCommerce.Entities.Order;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.Responses;
using Domain.LinxCommerce.Interfaces.Repositorys;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using static Domain.LinxCommerce.Entities.Responses.SearchOrder;

namespace Application.LinxCommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IOrderRepository _orderRepository;

        private readonly IValidator<Order.Root> _validator;
        private readonly IValidator<SearchOrderStatus.Result> _statusValidator;

        private List<string?> _errorMessagesList = new List<string?>();
        private readonly IMemoryCache _cache;
        private const string OrdersCacheKey = "LinxCommerce:Orders";
        private const string OrderStatusHashKey = "LinxCommerce:OrderStatusHash";

        public OrderService([FromKeyedServices("LinxCommerceAPI")] IApiClient apiClient, ILoggerService logger, IOrderRepository orderRepository, IValidator<Order.Root> validator, IValidator<SearchOrderStatus.Result> statusValidator, IMemoryCache cache) =>
            (_apiClient, _logger, _orderRepository, _validator, _statusValidator, _cache) = (apiClient, logger, orderRepository, validator, statusValidator, cache);

        private static Dictionary<string, string> BuildAuthHeaders(LinxCommerceJobParameter jobParameter)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{jobParameter.UserAuthentication}:{jobParameter.KeyAuthorization}");
            return new Dictionary<string, string>
            {
                { "Authorization", "Basic " + Convert.ToBase64String(plainTextBytes) }
            };
        }

        public Task<bool?> GetOrder(LinxCommerceJobParameter jobParameter, string? orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> GetOrderByNumber(LinxCommerceJobParameter jobParameter, string? orderNumber)
        {
            var ordersAPIList = new List<Order.Root>();

            var getOrderResponse = await _apiClient.PostAsync(
                route: "/v1/Sales/API.svc/web/GetOrderByNumber",
                body: orderNumber,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order.Root>(getOrderResponse);

            ordersAPIList.Add(new Order.Root(order, getOrderResponse));

            if (ordersAPIList.Count() > 0)
            {
                await _orderRepository.BulkInsertIntoTableRaw(jobParameter, ordersAPIList, _logger.GetExecutionGuid());
            }

            return true;
        }

        public Task<string?> GetOrderGroup()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetOrderGroupByNumber()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetOrderPackage()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetOrderPayment()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetOrderPayments()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetOrderStatus()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetPaymentTerm()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetQueueOrders()
        {
            throw new NotImplementedException();
        }

        public Task<string?> MakeOrderTransition()
        {
            throw new NotImplementedException();
        }

        public Task<string?> RunOrderWorkflow()
        {
            throw new NotImplementedException();
        }

        public Task<string?> SaveOrderItemSerialKey()
        {
            throw new NotImplementedException();
        }

        public Task<string?> SaveOrderPackage()
        {
            throw new NotImplementedException();
        }

        public Task<string?> SearchFinancialOrderInfo()
        {
            throw new NotImplementedException();
        }

        public Task<string?> SearchOrderPackage()
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> SearchOrdersByDateInterval(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceOrders);

            int pageIndex = 0, pageCount = 0;
            var ordersResults = new List<Result>();

            while (pageIndex <= pageCount)
            {
                var objectRequest = new
                {
                    Page = new { PageIndex = pageIndex, PageSize = 500 },
                    Where = $"(ModifiedDate>=\"{DateTime.Now.AddDays(-2).Date:yyyy-MM-dd}T00:00:00\" && ModifiedDate<=\"{DateTime.Now.Date:yyyy-MM-dd}T23:59:59\")",
                    //Where = $"(CreatedDate>=\"2025-08-12T00:00:00\" && CreatedDate<=\"2025-08-12T23:59:59\")",
                    WhereMetadata = "",
                    OrderBy = "OrderNumber",
                };

                var response = await _apiClient.PostAsync(
                    route: "/v1/Sales/API.svc/web/SearchOrders",
                    body: objectRequest,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchOrder.Root>(response);

                if (pageCount == 0)
                    pageCount = result.Page.PageCount;

                pageIndex++;

                ordersResults.AddRange(result.Result);
            }

            var cached = await _cache.GetOrCreateAsync(OrdersCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var list = await _orderRepository.GetRegistersExists(
                    ordersResults
                        .Select(x => x.OrderID)
                        .ToList()
                );

                return list
                    .Where(x => x != null)
                    .GroupBy(x => x.OrderID)
                    .ToDictionary(g => g.Key, g => g.First());
            });

            ordersResults.RemoveAll(x =>
               cached.TryGetValue(x.OrderID, out var y) && x.ModifiedDate < y.ModifiedDate
            );

            if (ordersResults.Count() > 0)
            {
                var ordersAPIList = new List<Order.Root>();

                foreach (var orderID in ordersResults)
                {
                    var getOrderResponse = await _apiClient.PostAsync(
                        route: "/v1/Sales/API.svc/web/GetOrder",
                        body: orderID.OrderID.ToString(),
                        headers: BuildAuthHeaders(jobParameter)
                    ) ?? string.Empty;

                    var order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order.Root>(getOrderResponse);
                    var validations = _validator.Validate(order);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = $"Error when convert record - order: {order.OrderNumber} | order_id: {order.OrderID}\n";

                        for (int j = 0; j < validations.Errors.Count(); j++)
                        {
                            var msg = validations.Errors[j].ErrorMessage;
                            var property = validations.Errors[j].PropertyName;
                            var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                            message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                        }

                        _errorMessagesList.Add(message);
                    }

                    ordersAPIList.Add(new Order.Root(order, getOrderResponse));
                }

                if (ordersAPIList.Count() > 0)
                {
                    await _orderRepository.BulkInsertIntoTableRaw(jobParameter, ordersAPIList, _logger.GetExecutionGuid());

                    for (int i = 0; i < ordersAPIList.Count; i++)
                    {
                        _logger.AddRecord(ordersAPIList[i].CustomerID.ToString(), ordersAPIList[i].Responses.Where(pair => pair.Key == ordersAPIList[i].OrderID).Select(pair => pair.Value).FirstOrDefault());
                        _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - order: {ordersAPIList[i].OrderNumber} | order_id: {ordersAPIList[i].OrderID}")).FirstOrDefault());
                    }

                    foreach (var s in ordersAPIList.Where(x => x != null))
                        cached[s.OrderID] = s;
                }
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool?> SearchOrdersByQueue(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceOrders);

            var objectRequest = new
            {
                QueueID = 21,
                Page = new { PageIndex = 0, PageSize = 1000 },
                Where = "EntityKeyName==\"OrderID\"",
                WhereMetadata = "",
                OrderBy = "QueueItemID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Sales/API.svc/web/GetQueueOrders",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var ordersAPIList = new List<Order.Root>();
            var enqueuedItens = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchQueuedOrder.Root>(response);
            var enqueuedOrders = enqueuedItens
                                .Result
                                .Where(x => x.QueueItem.EntityKeyName == "OrderID")
                                .GroupBy(y => y.QueueItem.EntityKeyValue)
                                .Select(g => g.First())
                                .ToList();

            foreach (var enqueuedOrder in enqueuedOrders)
            {
                var getOrderResponse = await _apiClient.PostAsync(
                    route: "/v1/Sales/API.svc/web/GetOrder",
                    body: enqueuedOrder.QueueItem.EntityKeyValue,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order.Root>(getOrderResponse);
                var validations = _validator.Validate(order);

                if (validations.Errors.Count() > 0)
                {
                    var message = $"Error when convert record - order: {order.OrderNumber} | order_id: {order.OrderID}\n";

                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        var msg = validations.Errors[j].ErrorMessage;
                        var property = validations.Errors[j].PropertyName;
                        var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                        message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                    }

                    _errorMessagesList.Add(message);
                }

                ordersAPIList.Add(new Order.Root(order, getOrderResponse));
            }

            if (ordersAPIList.Count() > 0)
            {
                await _orderRepository.BulkInsertIntoTableRaw(jobParameter, ordersAPIList, _logger.GetExecutionGuid());

                for (int i = 0; i < ordersAPIList.Count; i++)
                {
                    _logger.AddRecord(ordersAPIList[i].CustomerID.ToString(), ordersAPIList[i].Responses.Where(pair => pair.Key == ordersAPIList[i].OrderID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - order: {ordersAPIList[i].OrderNumber} | order_id: {ordersAPIList[i].OrderID}")).FirstOrDefault());
                }

                //remover da fila de integração apenas se o registro for inserido no banco de dados
                var listQueueItemID = new List<string>();

                foreach (var order in enqueuedOrders)
                {
                    listQueueItemID.Add(order.QueueItem.QueueItemID);
                }

                var dequeueObjectRequest = new { QueueItems = listQueueItemID };

                var dequeueResponse = await _apiClient.PostAsync(
                    route: "/v1/Queue/API.svc/web/DequeueQueueItems",
                    body: dequeueObjectRequest,
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public Task<string?> SearchOrdersBySeller()
        {
            throw new NotImplementedException();
        }

        public Task<string?> SearchOrdersCandidates()
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> SearchOrderStatus(LinxCommerceJobParameter jobParameter)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceOrdersStatus);

            var _listSomenteNovos = new List<SearchOrderStatus.Result>();

            var objectRequest = new
            {
                Page = new { PageIndex = 0, PageSize = 0 },
                Where = $"",
                WhereMetadata = "",
                OrderBy = "OrderStatusID",
            };

            var response = await _apiClient.PostAsync(
                route: "/v1/Sales/API.svc/web/SearchOrderStatus",
                body: objectRequest,
                headers: BuildAuthHeaders(jobParameter)
            ) ?? string.Empty;

            var hash = _logger.ComputeSha256Hash(response);

            var cachedHash = _cache.Get<string>(OrderStatusHashKey) ?? String.Empty;
            if (!cachedHash.Equals(hash))
            {
                var orderStatusResult = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchOrderStatus.Root>(response);

                foreach (var status in orderStatusResult.Result)
                {
                    var validations = _statusValidator.Validate(status);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = $"Error when convert record - OrderStatusID: {status.OrderStatusID} | Status: {status.Status}\n";

                        for (int j = 0; j < validations.Errors.Count(); j++)
                        {
                            var msg = validations.Errors[j].ErrorMessage;
                            var property = validations.Errors[j].PropertyName;
                            var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                            message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                        }

                        _errorMessagesList.Add(message);
                    }

                    var getOrderStatusResponse = await _apiClient.PostAsync(
                        route: "/v1/Sales/API.svc/web/GetOrderStatus",
                        body: status.OrderStatusID,
                        headers: BuildAuthHeaders(jobParameter)
                    ) ?? string.Empty;

                    var orderStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchOrderStatus.Result>(getOrderStatusResponse);

                    _listSomenteNovos.Add(new SearchOrderStatus.Result(status, getOrderStatusResponse));
                }
            }

            if (_listSomenteNovos.Count() > 0)
            {
                await _orderRepository.BulkInsertIntoOrdersStatusTable(jobParameter: jobParameter, registros: _listSomenteNovos, parentExecutionGUID: _logger.GetExecutionGuid());

                for (int i = 0; i < _listSomenteNovos.Count; i++)
                {
                    _logger.AddRecord(_listSomenteNovos[i].OrderStatusID.ToString(), _listSomenteNovos[i].Responses.Where(pair => pair.Key == _listSomenteNovos[i].OrderStatusID).Select(pair => pair.Value).FirstOrDefault());
                    _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"Error when convert record - OrderStatusID: {_listSomenteNovos[i].OrderStatusID} | Status: {_listSomenteNovos[i].Status}")).FirstOrDefault());
                }

                _cache.Set(OrderStatusHashKey, hash, TimeSpan.FromHours(2));
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public Task<bool?> UpdateOrder()
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> UpdateTrackingNumberOrder(LinxCommerceJobParameter jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxCommerceUpdateTrackingNumberOrder);

            var successfulTrackingNumber = new List<OrderTrackingNumber>();
            var trackingNumberOrders = await _orderRepository.GetTrackingNumbersToUpdate();

            if (trackingNumberOrders.Count() > 0)
            {
                for (int i = 0; i < trackingNumberOrders.Count(); i += 80)
                {
                    var lote = trackingNumberOrders.Skip(i).Take(80).ToList();

                    foreach (var item in lote)
                    {
                        var objectRequest = new
                        {
                            OrderID = item.OrderID,
                            WorkflowType = "SHIPPED",
                            Invoice = new
                            {
                                OrderInvoiceID = item.OrderInvoiceID,
                                Code = item.OrderInvoiceCode,
                                IsIssued = true
                            },
                            Shipment = new
                            {
                                TrackingNumber = item.TrackingNumber,
                                TrackingUrl = ""
                            },
                        };

                        var result = await _apiClient.PostAsync(
                            route: "/v1/Sales/API.svc/web/UpdateOrder",
                            body: objectRequest,
                            headers: BuildAuthHeaders(jobParameter)
                        );

                        if (result != null)
                        {
                            await _orderRepository.InsertIntoUpdateTrackingNumberOrderTable(jobParameter, item, _logger.GetExecutionGuid());

                            _logger.AddRecord(
                                item.OrderID.ToString(),
                                Newtonsoft.Json.JsonConvert.SerializeObject(objectRequest)
                            );
                        }
                    }

                    Thread.Sleep(5 * 1000);
                }
            }

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public Task<string?> UpdateOrderInvoice()
        {
            throw new NotImplementedException();
        }

        public Task<string?> UpdateOrderPackageTrackingNumber()
        {
            throw new NotImplementedException();
        }

        public async Task<bool?> IntegrityLockOrdersRegisters(LinxCommerceJobParameter jobParameter)
        {
            var lostOrderItemList = await _orderRepository.GetLostOrdersRegisters();
            var ordersAPIList = new List<Order.Root>();

            foreach (var order in lostOrderItemList)
            {
                var response = await _apiClient.PostAsync(
                    route: "/v1/Sales/API.svc/web/GetOrder",
                    body: order.OrderID.ToString(),
                    headers: BuildAuthHeaders(jobParameter)
                ) ?? string.Empty;

                var _order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order.Root>(response);

                ordersAPIList.Add(new Order.Root(_order, response));
            }

            if (ordersAPIList.Count() > 0)
                await _orderRepository.BulkInsertIntoTableRaw(jobParameter, ordersAPIList);

            return true;
        }
    }
}