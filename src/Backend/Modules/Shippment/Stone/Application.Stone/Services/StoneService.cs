using Application.Core.Abstractions.Services;
using Application.Stone.Abstractions.Services;
using Domain.Core.Abstractions.Api;
using Domain.Core.Enums;
using Domain.Stone.Abstractions.Repository;
using Domain.Stone.Entities;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Application.Stone.Services
{
    public class StoneService : IStoneService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IValidator<Domain.Stone.Dtos.Order> _validator;
        private readonly IStoneRepository _stoneRepository;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "Stone:DeliveryOrders";

        public StoneService(IStoneRepository stoneRepository, [FromKeyedServices("StoneAPI")] IApiClient apiClient, ILoggerService logger, IValidator<Domain.Stone.Dtos.Order> validator, IMemoryCache cache) =>
            (_stoneRepository, _apiClient, _logger, _validator, _cache) = (stoneRepository, apiClient, logger, validator, cache);

        public async Task<bool> CancelOrder(string orderNumber)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.StoneLogisticaCancelOrders);

            var parameters = await _stoneRepository.GetParameters();

            var jObject = new JObject
            {
                { "email", parameters.First()?.email },
                { "password", parameters.First()?.password }
            };

            var responseToken = await _apiClient.PostAsync(route: "auth/login", body: jObject);
            var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(responseToken);

            var _jObject = new JObject
            {
                { "referenceKey", orderNumber }
            };

            var response = await _apiClient.DeleteAsync(route: $"deliveries/id_or_ref_key", body: _jObject, headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.accessToken}" } });
            var retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(response);

            _logger.AddMessage(
                    $"Concluída com sucesso: pedido {orderNumber} cancelado!"
                );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool> CheckDeliveryOrder(string orderNumber)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.StoneLogisticaTrackingHistory);

            var listRecords = new List<Order>();
            var parameters = await _stoneRepository.GetParameters();

            var jObject = new JObject
            {
                { "email", parameters.First()?.email },
                { "password", parameters.First()?.password }
            };

            string prefixo = orderNumber.Contains("MI-") ? "3294_30_" : "3294_29_";

            var responseToken = await _apiClient.PostAsync(route: "auth/login", body: jObject);
            var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(responseToken);

            var response = await _apiClient.GetAsync(route: $"deliveries/{prefixo}{orderNumber}", headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.accessToken}" } });
            var retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Stone.Dtos.Response>(response);

            if (response.Contains("message"))
            {
                var error = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Stone.Dtos.Error>(response);
                listRecords.Add(new Order(orderNumber: orderNumber, request: "", message: error.message, error: error.error));
            }
            else
            {
                var pedido = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Stone.Dtos.Order>(response);
                listRecords.Add(new Order(order: pedido));
            }

            if (listRecords.Count() > 0)
            {
                await _stoneRepository.BulkInsertIntoTableRaw(listRecords, _logger.GetExecutionGuid());

                var cached = _cache.GetOrCreate(CacheKey, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                    return new Dictionary<Guid, Domain.Stone.Entities.Order>();
                });

                foreach (var s in listRecords.Where(x => x != null))
                    cached[s.orderId] = s;

                listRecords.ForEach(s =>
                    _logger.AddRecord(
                        s.orderId.ToString(),
                        s.Responses
                            .Where(pair => pair.Key == s.orderNumber)
                            .Select(pair => pair.Value)
                            .FirstOrDefault()
                    )
                );
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {listRecords.Count()} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool> CheckDeliveryOrders()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.StoneLogisticaTrackingHistory);

            var ordersAPIList = new List<Domain.Stone.Dtos.Order>();
            var listRecords = new List<Order>();
            var parameters = await _stoneRepository.GetParameters();

            foreach (var parameter in parameters)
            {
                var jObject = new JObject
                {
                    { "email", parameter.email },
                    { "password", parameter.password }
                };

                int pageIndex = 1;
                int? pageCount = 1;
                var responseToken = await _apiClient.PostAsync(route: "auth/login", body: jObject);
                var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(responseToken);

                while (pageIndex <= pageCount)
                {
                    var response = await _apiClient.GetAsync(route: $"deliveries/?paginated=true&currentPage={pageIndex}&pageLimit=100&orderBy=createdAt", headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.accessToken}" } });
                    //var response = await _apiClient.GetAsync(route: $"deliveries/?paginated=true&currentPage={pageIndex}&pageLimit=100&fromDate={DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")}T00:00:00.000Z&untilDate={DateTime.Today.ToString("yyyy-MM-dd")}T23:59:59.000Z&orderBy=createdAt", headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.accessToken}" } });
                    var retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Stone.Dtos.Response>(response);

                    if (pageCount == 1)
                        pageCount = retorno.pagination.lastPage;

                    pageIndex++;

                    ordersAPIList.AddRange(retorno.data);
                }

                foreach (var orderDto in ordersAPIList)
                {
                    var validations = _validator.Validate(orderDto);

                    if (validations.Errors.Count() > 0)
                    {
                        for (int j = 0; j < validations.Errors.Count(); j++)
                        {
                            _logger.AddMessage(
                                message: $"Error when convert record - order: {orderDto.referenceKey} | order_id: {orderDto.id}\n" +
                                         $"{validations.Errors[j]}"
                            );
                        }
                    }

                    try
                    {
                        listRecords.Add(new Order(order: orderDto));
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }

                var cached = await _cache.GetOrCreateAsync(CacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await _stoneRepository.GetRegistersExists(
                        registros: listRecords
                                    .Select(x => x.orderId)
                                    .ToList()
                    );

                    return list
                        .Where(x => x != null)
                        .GroupBy(x => x.orderId)
                        .ToDictionary(g => g.Key, g => g.First());
                });

                listRecords.RemoveAll(x =>
                   cached.TryGetValue(x.orderId, out var y) &&
                   x.updatedAt < y.updatedAt
                );

                if (listRecords.Count() > 0)
                {
                    await _stoneRepository.BulkInsertIntoTableRaw(listRecords, _logger.GetExecutionGuid());

                    foreach (var s in listRecords.Where(x => x != null))
                        cached[s.orderId] = s;

                    listRecords.ForEach(s =>
                        _logger.AddRecord(
                            s.orderId.ToString(),
                            s.Responses
                                .Where(pair => pair.Key == s.orderNumber)
                                .Select(pair => pair.Value)
                                .FirstOrDefault()
                        )
                    );
                }

                _logger.AddMessage(
                    $"Concluída com sucesso: {listRecords.Count()} registro(s) novo(s) inserido(s)!"
                );
            }

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }

        public async Task<bool> CreateDeliveryOrder()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.StoneLogisticaSendOrders);

            var _listSomenteNovos = new List<Order>();
            var parameters = await _stoneRepository.GetParameters();
            var orders = await _stoneRepository.GetRegistersExists();

            if (orders.Count() > 0)
            {
                foreach (var parameter in parameters)
                {
                    var jObject = new JObject
                    {
                        { "email", parameter.email },
                        { "password", parameter.password }
                    };

                    var responseToken = await _apiClient.PostAsync(route: "auth/login", body: jObject);
                    var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(responseToken);

                    foreach (var order in orders)
                    {
                        try
                        {
                            var jObj = BuildJObject(order, parameters.FirstOrDefault());

                            string? response = await _apiClient.PostAsync(
                                route: "deliveries",
                                body: jObj,
                                headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.accessToken}" } }
                            );

                            if (response.Contains("message"))
                            {
                                var error = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Stone.Dtos.Error>(response);
                                _listSomenteNovos.Add(new Order(orderNumber: order.Number, request: Newtonsoft.Json.JsonConvert.SerializeObject(jObj), message: error.message, error: error.error));
                                continue;
                            }

                            var pedido = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Stone.Dtos.Order>(response);
                            _listSomenteNovos.Add(new Order(order: pedido, orderNumber: order.Number, response: response, request: Newtonsoft.Json.JsonConvert.SerializeObject(jObj)));
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }

                if (_listSomenteNovos.Count() > 0)
                {
                    await _stoneRepository.BulkInsertIntoTableRaw(_listSomenteNovos, _logger.GetExecutionGuid());

                    _listSomenteNovos.ForEach(s =>
                        _logger.AddRecord(
                            s.orderNumber,
                            s.Responses
                                .Where(pair => pair.Key == s.orderNumber)
                                .Select(pair => pair.Value)
                                .FirstOrDefault()
                        )
                    );
                }
            }

            _logger.AddMessage(
                $"Concluída com sucesso: {_listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
            );

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return false;
        }

        public Task<string> GetAccessToken()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDeliveryOptions()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> GetZplLabels()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.StoneLogisticaGetZplLabels);

            var parameters = await _stoneRepository.GetParameters();
            var orders = await _stoneRepository.GetExistingReferenceKeys();

            if (orders.Count() > 0)
            {
                foreach (var parameter in parameters)
                {
                    var jObject = new JObject
                    {
                        { "email", parameter.email },
                        { "password", parameter.password }
                    };

                    var responseToken = await _apiClient.PostAsync(route: "auth/login", body: jObject);
                    var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(responseToken);

                    foreach (var order in orders)
                    {
                        try
                        {
                            string response = String.Empty;
                            string prefixo = String.Empty;

                            if (order.referencekey == null && order.documento.Contains("MI-"))
                                prefixo = "3294_29_";
                            else if (order.referencekey == null && order.documento.Contains("OA-"))
                                prefixo = "3294_30_";

                            if (prefixo != String.Empty)
                                response = await _apiClient.GetAsync(route: $"deliveries/{prefixo}{order.documento}/label?format=zpl", headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.accessToken}" } });
                            else
                                response = await _apiClient.GetAsync(route: $"deliveries/{order.referencekey}/label?format=zpl", headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.accessToken}" } });

                            if (response.Contains("erro"))
                            {
                                var error = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Stone.Dtos.Error>(response);
                                order.SetError(error.message, error.error);
                                continue;
                            }

                            var zpl = await _apiClient.GetAsync(route: response);
                            order.SetZpl(zpl);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }

                await _stoneRepository.InsertZpls(orders.ToList());

                _logger.AddMessage(
                    $"Concluída com sucesso: {orders.Count()} registro(s) novo(s) inserido(s)!"
                );
            }

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return false;
        }

        private JObject BuildJObject(OrdersToBeSent order, Parameters? parameters)
        {
            string carrierName = String.Empty;
            string carrierService = String.Empty;
            var itensArray = new JArray();

            if (order.logistic_contract_name.Contains("OA_Any_Standard_CD_VOLO_PAC"))
            {
                carrierName = "correios";
                carrierService = "pac";
            }
            else if (!order.logistic_contract_name.Contains("Sameday") && !order.logistic_contract_name.Contains("SameDay"))
            {
                carrierName = "correios";
                carrierService = "sedex";
            }

            foreach (var item in order.itens)
            {
                itensArray.Add(new JObject
                {
                    { "description", $"{item.Description}" },
                    { "invoiceNumber", $"{order.invoice.Number}" },
                    { "invoiceKey", $"{order.invoice.KeyNfe}" },
                    { "quantity", item.Quantity },
                    { "unitPrice", item.UnitaryValue },
                    { "weight", 1 },
                    { "height", 1 },
                    { "depth", 1 },
                    { "width", 1 },
                    { "isDangerous", 0 }
                });
            }

            return new JObject
            {
                { "referenceKey", $"{order.Number}" },
                { "processType", "sync" },
                { "acceptedAgreements", true },
                { "quoteId", String.Empty },
                { "contractPrice", order.amount_value },
                { "customer", new JObject {
                        { "name", $"{order.client.Reason}" },
                        { "document", $"{order.client.Doc}" },
                        { "phoneNumber", $"999999999" },
                        { "email", $"{order.client.Email}" }
                    }
                },
                { "sender", new JObject {
                        { "logisticAccountId", "" },
                        { "name", $"{order.company.Reason}" },
                        { "phoneNumber", $"{order.company.Phone}" },
                        { "document", $"{order.company.Doc}" }
                    }
                },
                { "carrier", new JObject {
                        { "name", carrierName },
                        { "service", carrierService }
                    }
                },
                { "deliveryAddress", new JObject {
                        { "address", $"{order.client.Street}" },
                        { "addressNumber", $"{order.client.StreetNumber}" },
                        { "neighborhood", $"{order.client.Neighborhood}" },
                        { "city", $"{order.client.City}" },
                        { "countryState", $"{order.client.State}" },
                        { "zipCode", $"{order.client.ZipCode.Trim()}" },
                        { "complement", $"{order.client.ComplementAddress}" },
                        { "reference", $"{order.client.ComplementAddress}" },
                        { "latitude", 0 },
                        { "longitude", 0 }
                    }
                },
                { "pickupAddress", new JObject {
                        { "address", $"{order.company.Street}" },
                        { "addressNumber", $"{order.company.StreetNumber}" },
                        { "neighborhood", $"{order.company.Neighborhood}" },
                        { "city", $"{order.company.City}" },
                        { "countryState", $"{order.company.State}" },
                        { "zipCode", $"{order.company.ZipCode.Trim()}" },
                        { "complement", $"{order.company.ComplementAddress}" },
                        { "reference", $"{order.company.ComplementAddress}" },
                        { "latitude", 0 },
                        { "longitude", 0 }
                    }
                },
                { "items", itensArray},
                { "invoiceKey", $"{order.invoice.KeyNfe}" },
                { "invoiceNumber", $"{order.invoice.Number}" },
                { "invoiceSeries", $"{order.invoice.Serie}" },
                { "dceAccessKey", String.Empty },
                { "dceNumber", String.Empty },
                { "dceSeries", String.Empty },
            };
        }
    }
}
