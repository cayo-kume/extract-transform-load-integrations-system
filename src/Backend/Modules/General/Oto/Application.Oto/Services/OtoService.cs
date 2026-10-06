using Application.Core.Abstractions.Services;
using Application.Oto.Factories;
using Application.Oto.Interfaces.Services;
using Domain.Core.Abstractions.Api;
using Domain.Core.Enums;
using Domain.Oto.Dtos;
using Domain.Oto.Entities;
using Domain.Oto.Interfaces.Repositorys;
using Domain.Oto.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Oto.Services
{
    public class OtoService : IOtoService
    {
        private readonly IApiClient _apiClient;
        private readonly ILoggerService _logger;
        private readonly IOtoRepository _otoRepository;

        public OtoService([FromKeyedServices("OttoAPI")] IApiClient apiClient, IOtoRepository otoRepository, ILoggerService logger) =>
            (_apiClient, _otoRepository, _logger) = (apiClient, otoRepository, logger);

        public async Task<bool> SendCustomers()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.OtoSendCustomers);

            try
            {
                var customers = await _otoRepository.GetClients();
                var parameters = await _otoRepository.GetParameters();
                
                var listSuccess = new List<Response>();
                var listSomenteNovos = new List<SentRecordLog>();

                if (customers.Count() > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        var token = await GenerateToken(parameter.username, parameter.password);
                        var records = new List<SentRecordLog>();
                        var jArrayObj = new JArray();

                        foreach (var customer in customers)
                        {
                            var jObject = BuildObjects.BuildCustomerObject(customer);
                            jArrayObj.Add(jObject);
                            records.Add(new SentRecordLog(parameter.username, customer.Code, "C", jObject.ToString(Newtonsoft.Json.Formatting.None)));
                        }

                        var rootObject = new JObject
                        {
                            { "data", jArrayObj }
                        };

                        string? response = await _apiClient.PostAsync(
                            route: "v1/customers",
                            body: rootObject,
                            headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                        );

                        var result = JsonConvert.DeserializeObject<Response>(response);

                        if (result.success)
                        {
                            foreach (var record in records)
                            {
                                listSomenteNovos.Add(new SentRecordLog(result.requestId, record.branch, record.documento, record.tipo, record.payload));
                            }

                            listSuccess.Add(result);
                        }
                    }
                }

                //Só armazena o envio do log se der sucesso no envio das duas empresas
                if (listSuccess.All(x => x.success))
                    _otoRepository.BulkInsertIntoTableRaw(listSomenteNovos, _logger.GetExecutionGuid());

                _logger.AddMessage(
                    $"Concluída com sucesso: {listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
                );

                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SendCustomersCashBack()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.OtoSendCustomers);

            try
            {
                var customers = await _otoRepository.GetClientsCashBack();
                var parameters = await _otoRepository.GetParameters();

                var listSuccess = new List<Response>();
                var listSomenteNovos = new List<SentRecordLog>();

                if (customers.Count() > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        var token = await GenerateToken(parameter.username, parameter.password);
                        var records = new List<SentRecordLog>();
                        var groupedCustomers = new List<Client>();
                        var jArrayObj = new JArray();

                        if (parameter.username == "misha")
                            groupedCustomers = customers.Where(x => x.branch.Contains("Misha")).ToList();
                        else
                            groupedCustomers = customers.Where(x => x.branch.Contains("Open Era")).ToList();

                        foreach (var customer in groupedCustomers)
                        {
                            var jObject = BuildObjects.BuildCustomerObject(customer);
                            jArrayObj.Add(jObject);
                            records.Add(new SentRecordLog(parameter.username, customer.Code, "CB", jObject.ToString(Newtonsoft.Json.Formatting.None)));
                        }

                        var rootObject = new JObject
                        {
                            { "data", jArrayObj }
                        };

                        if (groupedCustomers.Count() == 0)
                            continue;

                        string? response = await _apiClient.PostAsync(
                            route: "v1/customers",
                            body: rootObject,
                            headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                        );

                        var result = JsonConvert.DeserializeObject<Response>(response);

                        if (result.success)
                        {
                            foreach (var record in records)
                            {
                                listSomenteNovos.Add(new SentRecordLog(result.requestId, record.branch, record.documento, record.tipo, record.payload));
                            }

                            listSuccess.Add(result);
                        }
                    }
                }

                //Só armazena o envio do log se der sucesso no envio das duas empresas
                if (listSuccess.All(x => x.success))
                    _otoRepository.BulkInsertIntoTableRaw(listSomenteNovos, _logger.GetExecutionGuid());

                _logger.AddMessage(
                    $"Concluída com sucesso: {listSomenteNovos.Count} registro(s) novo(s) inserido(s)!"
                );

                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SendOrders()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.OtoSendOrders);

            try
            {
                var orders = await _otoRepository.GetOrders();
                var parameters = await _otoRepository.GetParameters();
                
                if (orders.Count() > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        var token = await GenerateToken(parameter.username, parameter.password);
                        var groupedOrders = new List<Order>();

                        if (parameter.username == "misha")
                            groupedOrders = orders.Where(x => x.Number.Contains("MI-")).ToList();
                        else
                            groupedOrders = orders.Where(x => x.Number.Contains("OA-")).ToList();

                        const int maxBatchSize = 10000;
                        int index = 0;

                        while (index < groupedOrders.Count)
                        {
                            var records = new List<SentRecordLog>();
                            var listSomenteNovos = new List<SentRecordLog>();
                            var ordersArray = new JArray();
                            var clientsArray = new JArray();
                            var ordersItemsArray = new JArray();

                            //Acumula pedidos no lote enquanto nenhum dos 3 arrays ultrapassar maxBatchSize.
                            //A guarda ordersArray.Count > 0 garante que cada lote tenha ao menos 1 pedido
                            //(caso um único pedido já exceda o limite de itens, ele segue sozinho no lote).
                            while (index < groupedOrders.Count)
                            {
                                var order = groupedOrders[index];
                                int itemsCount = order.itens?.Count() ?? 0;

                                if (ordersArray.Count > 0 && (
                                    ordersArray.Count + 1 > maxBatchSize ||
                                    clientsArray.Count + 1 > maxBatchSize ||
                                    ordersItemsArray.Count + itemsCount > maxBatchSize))
                                {
                                    break;
                                }

                                var orderJObject = BuildObjects.BuildOrderObject(order);
                                ordersArray.Add(orderJObject);
                                records.Add(new SentRecordLog(parameter.username, order.Number, "PE", orderJObject.ToString(Newtonsoft.Json.Formatting.None)));

                                var clientJObject = BuildObjects.BuildCustomerObject(order.client);
                                clientsArray.Add(clientJObject);
                                records.Add(new SentRecordLog(parameter.username, order.client.Code, "C", clientJObject.ToString(Newtonsoft.Json.Formatting.None)));

                                foreach (var item in order.itens)
                                {
                                    var ordersItemsJObject = BuildObjects.BuildOrderItemObject(item, order.date.ToString("yyyy-MM-dd"), order.Number, order.company?.Code, order.seller_id);
                                    ordersItemsArray.Add(ordersItemsJObject);
                                    records.Add(new SentRecordLog(parameter.username, $"{order.Number} | {item.Code}", "PI", ordersItemsJObject.ToString(Newtonsoft.Json.Formatting.None)));
                                }

                                index++;
                            }

                            var ordersItemsRootObject = new JObject
                            {
                                { "data", ordersItemsArray }
                            };

                            var ordersRootObject = new JObject
                            {
                                { "data", ordersArray }
                            };

                            var clientsRootObject = new JObject
                            {
                                { "data", clientsArray }
                            };

                            string? ordersItemsResponse = await _apiClient.PostAsync(
                                route: "v1/order_items",
                                body: ordersItemsRootObject,
                                headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                            );

                            string? ordersResponse = await _apiClient.PostAsync(
                                route: "v1/orders",
                                body: ordersRootObject,
                                headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                            );

                            string? clientsResponse = await _apiClient.PostAsync(
                                route: "v1/customers",
                                body: clientsRootObject,
                                headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                            );

                            var ordersItemsResult = JsonConvert.DeserializeObject<Response>(ordersItemsResponse);
                            var ordersResult = JsonConvert.DeserializeObject<Response>(ordersResponse);
                            var clientsResult = JsonConvert.DeserializeObject<Response>(clientsResponse);

                            //Só registra o log do lote se os 3 end-points tiverem sucesso;
                            //se qualquer um falhar, o lote é descartado e segue para o próximo (ou próxima marca).
                            if (ordersResult.success && ordersItemsResult.success && clientsResult.success)
                            {
                                foreach (var record in records.Where(x => x.tipo == "PE"))
                                {
                                    listSomenteNovos.Add(new SentRecordLog(ordersResult.requestId, record.branch, record.documento, record.tipo, record.payload));
                                    _logger.AddRecord(record.documento, record.payload);
                                }

                                foreach (var record in records.Where(x => x.tipo == "PI"))
                                {
                                    listSomenteNovos.Add(new SentRecordLog(ordersItemsResult.requestId, record.branch, record.documento, record.tipo, record.payload));
                                }

                                foreach (var record in records.Where(x => x.tipo == "C"))
                                {
                                    listSomenteNovos.Add(new SentRecordLog(clientsResult.requestId, record.branch, record.documento, record.tipo, record.payload));
                                }

                                _otoRepository.BulkInsertIntoTableRaw(listSomenteNovos, _logger.GetExecutionGuid());
                            }
                        }
                    }
                }

                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> SendProducts()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.OtoSendProducts);

            try
            {
                var products = await _otoRepository.GetProducts();
                var parameters = await _otoRepository.GetParameters();

                var listSomenteNovos = new List<SentRecordLog>();
                var listSuccess = new List<Response>();

                if (products.Count() > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        var token = await GenerateToken(parameter.username, parameter.password);
                        var records = new List<SentRecordLog>();
                        var groupedProducts = new List<Product>();
                        var jArrayObj = new JArray();

                        if (parameter.username == "misha")
                            groupedProducts = products.Where(x => x.desc_setor.Contains("MISHA")).ToList();
                        else
                            groupedProducts = products.Where(x => x.desc_setor.Contains("OPEN ERA")).ToList();

                        foreach (var product in groupedProducts)
                        {
                            var jObject = BuildObjects.BuildProductObject(product);
                            jArrayObj.Add(jObject);
                            records.Add(new SentRecordLog(parameter.username, product.Code.ToString(), "P", jObject.ToString(Newtonsoft.Json.Formatting.None)));
                        }

                        var rootObject = new JObject
                        {
                            { "data", jArrayObj }
                        };

                        string? response = await _apiClient.PostAsync(
                            route: "v1/products",
                            body: rootObject,
                            headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                        );

                        var result = JsonConvert.DeserializeObject<Response>(response);

                        if (result.success)
                        {
                            foreach (var record in records)
                            {
                                listSomenteNovos.Add(new SentRecordLog(result.requestId, record.branch, record.documento, record.tipo, record.payload));
                            }

                            listSuccess.Add(result);
                        }
                    } 
                }

                //Só armazena o envio do log se der sucesso no envio das duas empresas
                if (listSuccess.All(x => x.success))
                    _otoRepository.BulkInsertIntoTableRaw(listSomenteNovos, _logger.GetExecutionGuid());

                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SendSellers()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.OtoSendSellers);

            try
            {
                var sellers = await _otoRepository.GetSaleRepresentatives();
                var parameters = await _otoRepository.GetParameters();

                var listSomenteNovos = new List<SentRecordLog>();
                var listSuccess = new List<Response>();

                if (sellers.Count() > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        var token = await GenerateToken(parameter.username, parameter.password);
                        var records = new List<SentRecordLog>();
                        var groupedSellers = new List<SaleRepresentative>();
                        var jArrayObj = new JArray();

                        if (parameter.username == "misha")
                            groupedSellers = sellers.Where(x => x.reason_company.Contains("MNR")).ToList();
                        else
                            groupedSellers = sellers.Where(x => x.reason_company.Contains("NEWFIT") || x.reason_company.Contains("NEW FIT")).ToList();

                        foreach (var seller in sellers)
                        {
                            var jObject = BuildObjects.BuildSellerObject(seller);

                            jArrayObj.Add(jObject);
                            records.Add(new SentRecordLog(parameter.username, seller.Code.ToString(), "S", jObject.ToString(Newtonsoft.Json.Formatting.None)));
                        }

                        var rootObject = new JObject
                        {
                            { "data", jArrayObj }
                        };

                        string? response = await _apiClient.PostAsync(
                            route: "v1/sellers",
                            body: rootObject,
                            headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                        );

                        var result = JsonConvert.DeserializeObject<Response>(response);

                        if (result.success)
                        {
                            foreach (var record in records)
                            {
                                listSomenteNovos.Add(new SentRecordLog(result.requestId, record.branch, record.documento, record.tipo, record.payload));
                            }

                            listSuccess.Add(result);
                        }
                    } 
                }

                _otoRepository.BulkInsertIntoTableRaw(listSomenteNovos, _logger.GetExecutionGuid());

                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> SendStores()
        {
            _logger
               .Clear()
               .AddLog(EnumJob.OtoSendStores);

            try
            {
                var companies = await _otoRepository.GetCompanys();
                var parameters = await _otoRepository.GetParameters();

                var listSomenteNovos = new List<SentRecordLog>();

                if (companies.Count() > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        var token = await GenerateToken(parameter.username, parameter.password);
                        var records = new List<SentRecordLog>();
                        var groupedCompanies = new List<Company>();
                        var jArrayObj = new JArray();

                        if (parameter.username == "misha")
                            groupedCompanies = companies.Where(x => x.Name.Contains("MNR")).ToList();
                        else
                            groupedCompanies = companies.Where(x => x.Name.ToUpper().Contains("NEWFIT") || x.Name.ToUpper().Contains("NEW FIT")).ToList();

                        foreach (var company in groupedCompanies)
                        {
                            var jObject = BuildObjects.BuildStoreObject(company);
                            jArrayObj.Add(jObject);
                            records.Add(new SentRecordLog(parameter.username, company.Code.ToString(), "L", jObject.ToString(Newtonsoft.Json.Formatting.None)));
                        }

                        var rootObject = new JObject
                        {
                            { "data", jArrayObj }
                        };

                        string? response = await _apiClient.PostAsync(
                            route: "v1/stores",
                            body: rootObject,
                            headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                        );

                        var result = JsonConvert.DeserializeObject<Response>(response);

                        if (result.success)
                        {
                            foreach (var record in records)
                            {
                                listSomenteNovos.Add(new SentRecordLog(result.requestId, record.branch, record.documento, record.tipo, record.payload));
                            }
                        }
                    }
                }

                _otoRepository.BulkInsertIntoTableRaw(listSomenteNovos, _logger.GetExecutionGuid());

                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();

                return true;
            }
            catch
            {
                throw;
            }
        }

        private async Task<Token?> GenerateToken(string? username, string? password)
        {
            var jObject = new JObject
            {
                { "username", $"{username}" },
                { "password", $"{password}" }
            };

            var result = await _apiClient.PostAsync(
                route: "auth/login",
                body: jObject
            );

            return JsonConvert.DeserializeObject<Token>(result);
        }
    }
}