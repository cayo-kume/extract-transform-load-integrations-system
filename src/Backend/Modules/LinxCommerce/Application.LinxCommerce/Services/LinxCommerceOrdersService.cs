using Application.LinxCommerce.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Enums;
using Domain.LinxCommerce.Extensions;
using Domain.Shared.Extensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Application.Shared.Attributes;

namespace Application.LinxCommerce.Services
{
    /// <summary>
    /// pedidos e status de pedido do Linx Commerce (API Sales). Cada método faz as próprias chamadas; o comum (horário do lote,
    /// clientes, busca paginada e Get) vem do ILinxCommerceService
    /// </summary>
    public class LinxCommerceOrdersService : ILinxCommerceOrdersService
    {
        private readonly ILoggerService _logger;
        private readonly IMemoryCache _cache;
        private readonly ILinxCommerceService _linxCommerceService;
        private readonly ILinxCommerceOrdersRepository _linxCommerceOrdersRepository;
        private readonly IValidator<Domain.LinxCommerce.Dtos.Reads.Orders.Order> _orderValidator;
        private readonly IValidator<Domain.LinxCommerce.Dtos.Reads.Orders.OrderStatus> _orderStatusValidator;

        private const string OrdersKey = "LinxCommerce:Orders";

        // carga inicial (banco vazio), como as notas e os pedidos do Totvs (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §10.2, pendência 3)
        private static readonly DateTime OrdersInitialLoad = new(2025, 1, 1);

        public LinxCommerceOrdersService(
            ILoggerService logger,
            IMemoryCache cache,
            ILinxCommerceService linxCommerceService,
            ILinxCommerceOrdersRepository linxCommerceOrdersRepository,
            IValidator<Domain.LinxCommerce.Dtos.Reads.Orders.Order> orderValidator,
            IValidator<Domain.LinxCommerce.Dtos.Reads.Orders.OrderStatus> orderStatusValidator
        ) =>
            (_logger, _cache, _linxCommerceService, _linxCommerceOrdersRepository, _orderValidator, _orderStatusValidator) =
            (logger, cache, linxCommerceService, linxCommerceOrdersRepository, orderValidator, orderStatusValidator);

        [JobVersion("1.0.0")]
        public async Task<bool?> SearchOrders()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceSearchOrders);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxCommerceService.GetCompanys())
                {
                    var batchTime = _linxCommerceService.GetBatchTime();

                    // uma hora de folga sobre a maior alteração gravada; banco vazio: a carga inicial
                    var lastModified = await _linxCommerceOrdersRepository.GetOrdersLastModifiedDate(company.doc_company!);
                    var since = lastModified.HasValue ? LinxCommerceConverters.ToBrasilia(lastModified.Value).AddHours(-1) : OrdersInitialLoad;

                    var found = await _linxCommerceService.Search<Domain.LinxCommerce.Dtos.Reads.Orders.SearchOrdersResult>(
                        company,
                        "Sales/API.svc/web/SearchOrders",
                        _linxCommerceService.ModifiedDateWindow(since, batchTime),
                        "OrderNumber"
                    );

                    // leitura incompleta não é gravada: a próxima janela sai do MAX(ModifiedDate) e pularia o que faltou
                    if (found is null)
                        continue;

                    // o mesmo pedido pode vir em duas páginas se mudar de posição durante a leitura: fica a última versão
                    var changed = found
                        .Select(x => (id: ConverterValidatorsExtensions.ConvertToGuidValidation(x.dto.OrderID), modified: LinxCommerceConverters.ToNullableDateTimeOffset(x.dto.ModifiedDate)))
                        .Where(x => x.id != Guid.Empty)
                        .GroupBy(x => x.id)
                        .Select(g => g.Last())
                        .ToList();

                    if (changed.Count == 0)
                        continue;

                    // cache de estado: OrderID -> ModifiedDate gravada, por empresa; fica só o novo ou com alteração mais recente
                    // os ids que ainda não estão no cache são lidos do banco, em lotes, e entram nele: um pedido que não estava
                    // na primeira execução também é encontrado
                    var ids = changed.Select(x => x.id).ToList();
                    var ordersCache = _cache.GetOrCreate($"{OrdersKey}:{company.doc_company!}", entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                        entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                        return new Dictionary<Guid, DateTimeOffset?>();
                    })!;

                    var missing = ids.Where(x => !ordersCache.ContainsKey(x)).ToList();

                    if (missing.Count > 0)
                        foreach (var stored in (await _linxCommerceOrdersRepository.GetOrdersByIds(company.doc_company!, missing)).Where(x => x is not null))
                            ordersCache[stored!.OrderID] = stored.ModifiedDate;

                    changed.RemoveAll(x => x.modified.HasValue && ordersCache.TryGetValue(x.id, out var stored) && stored.HasValue && x.modified <= stored);

                    var orders = new List<Domain.LinxCommerce.Dtos.Reads.Orders.Order>();
                    var ordersJson = new Dictionary<Guid, string>();
                    var failed = false;

                    // a busca traz só a lista: o pedido completo vem do GetOrder (a conferir se o SearchOrders já traz tudo, 4.2 item 5)
                    foreach (var (id, _) in changed)
                    {
                        var order = await _linxCommerceService.Get<Domain.LinxCommerce.Dtos.Reads.Orders.Order>(company, "Sales/API.svc/web/GetOrder", id.ToString(), $"pedido {id}");

                        if (order is null || ConverterValidatorsExtensions.ConvertToGuidValidation(order.Value.dto.OrderID) != id)
                        {
                            if (order is not null)
                                _logger.AddMessage($"GetOrder do pedido {id} (empresa {company.doc_company}) devolveu outro OrderID: {order.Value.dto.OrderID}.");

                            failed = true;
                            break;
                        }

                        orders.Add(order.Value.dto);
                        ordersJson[id] = order.Value.json;
                    }

                    // um pedido que falhou ficaria para trás da próxima janela: nada da empresa é gravado
                    if (failed)
                        continue;

                    totalInseridos += await SaveOrders(company.doc_company!, batchTime, orders, ordersJson, ordersCache);
                }

                _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
                );

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        [JobVersion("1.0.0")]
        public async Task<bool?> SearchOrderStatus()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceSearchOrderStatus);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxCommerceService.GetCompanys())
                {
                    var batchTime = _linxCommerceService.GetBatchTime();

                    // tabela pequena, lida inteira: a merge não regrava o que não mudou. O código antigo chamava o GetOrderStatus de cada
                    // status e descartava a resposta; a busca já traz todos os campos
                    var found = await _linxCommerceService.Search<Domain.LinxCommerce.Dtos.Reads.Orders.OrderStatus>(
                        company,
                        "Sales/API.svc/web/SearchOrderStatus",
                        "",
                        "OrderStatusID"
                    );

                    if (found is null || found.Count == 0)
                        continue;

                    var _listSomenteNovos = new List<Domain.LinxCommerce.Entities.Reads.Orders.OrderStatus>();
                    var statusJson = new Dictionary<int, string>();

                    foreach (var (dto, json) in found.GroupBy(x => x.dto.OrderStatusID).Select(g => g.Last()))
                    {
                        var validations = _orderStatusValidator.Validate(dto);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"status de pedido {dto.OrderStatusID}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        var status = new Domain.LinxCommerce.Entities.Reads.Orders.OrderStatus(company.doc_company!, batchTime, dto);

                        // sem a chave a linha violaria a PK
                        if (status.OrderStatusID == 0)
                            continue;

                        _listSomenteNovos.Add(status);
                        statusJson[status.OrderStatusID] = json;
                    }

                    if (_listSomenteNovos.Count == 0)
                        continue;

                    await _linxCommerceOrdersRepository.BulkInsertIntoLinxCommerceOrderStatus(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                        _logger.AddRecord(record.OrderStatusID.ToString(), statusJson[record.OrderStatusID]);

                    totalInseridos += _listSomenteNovos.Count;
                }

                _logger.AddMessage(
                    $"Concluída com sucesso: {totalInseridos} registro(s) novo(s) ou atualizado(s) inserido(s)!"
                );

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// busca um pedido pelo OrderID, grava (sempre, sem consultar o cache) e registra a chamada em linxcommerce.LinxCommerceRequests
        /// </summary>
        public async Task<bool?> GetOrder(string user_name, string doc_company, Guid orderId) =>
            await GetOrderOnDemand(EnumJob.LinxCommerceGetOrder, user_name, doc_company, "Sales/API.svc/web/GetOrder", orderId.ToString(), $"pedido {orderId}",
                order => ConverterValidatorsExtensions.ConvertToGuidValidation(order.OrderID) == orderId);

        /// <summary>
        /// busca um pedido pelo número (o corpo é o número, como no código antigo), grava e registra a chamada
        /// </summary>
        public async Task<bool?> GetOrderByNumber(string user_name, string doc_company, string orderNumber) =>
            await GetOrderOnDemand(EnumJob.LinxCommerceGetOrderByNumber, user_name, doc_company, "Sales/API.svc/web/GetOrderByNumber", orderNumber, $"pedido número {orderNumber}",
                order => order.OrderNumber == orderNumber && ConverterValidatorsExtensions.ConvertToGuidValidation(order.OrderID) != Guid.Empty);

        // ------------------------------------------------------------------------------------------------------

        /// <summary>
        /// o fluxo das duas rotas sob demanda de pedido. "Não existe" é o 404 ou a resposta 2xx sem o pedido pedido (sem resposta
        /// real, a forma como o Linx Commerce responde um id inexistente está a conferir, src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md §9)
        /// </summary>
        private async Task<bool?> GetOrderOnDemand(EnumJob job, string user_name, string doc_company, string route, string body, string key, Func<Domain.LinxCommerce.Dtos.Reads.Orders.Order, bool> isRequested)
        {
            _logger
                .Clear()
                .AddLog(job);

            try
            {
                var company = await _linxCommerceService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em LinxCommerceRequests (igual ao Totvs)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em linxcommerce.LinxCommerceParameters.");
                    return null;
                }

                // a chave em LinxCommerceRequests é o próprio id ou número, como nas outras rotas sob demanda
                var response = await _linxCommerceService.ReadOnDemand<Domain.LinxCommerce.Dtos.Reads.Orders.Order>(company, job, user_name, route, body, body);

                if (response.StatusCode == 404 || (response.IsValid && !isRequested(response.Dto!)))
                {
                    _logger.AddMessage($"{key} não encontrado no Linx Commerce (empresa {doc_company}).");
                    return false;
                }

                if (!response.IsValid)
                    return null;   // a mensagem já foi para o log

                var order = response.Dto!;
                var orderId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.OrderID);

                // atualiza o cache do SearchOrders, se ele estiver carregado, para a próxima execução não buscar o pedido de novo
                _cache.TryGetValue($"{OrdersKey}:{doc_company}", out Dictionary<Guid, DateTimeOffset?>? ordersCache);

                await SaveOrders(doc_company, _linxCommerceService.GetBatchTime(), [order], new Dictionary<Guid, string> { { orderId, response.Json! } }, ordersCache ?? []);

                _logger.AddMessage($"Concluída com sucesso: {key} inserido ou atualizado!");

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote), grava e atualiza o cache da empresa.
        /// Devolve a quantidade de pedidos gravados.
        /// </summary>
        private async Task<int> SaveOrders(
            string doc_company,
            DateTime batchTime,
            List<Domain.LinxCommerce.Dtos.Reads.Orders.Order> orders,
            IReadOnlyDictionary<Guid, string> ordersJson,
            Dictionary<Guid, DateTimeOffset?> ordersCache)
        {
            var _listSomenteNovos = new List<Domain.LinxCommerce.Entities.Reads.Orders.Order>();

            foreach (var order in orders)
            {
                var validations = _orderValidator.Validate(order);

                if (validations.Errors.Count > 0)
                {
                    var message = String.Empty;
                    validations.Errors.ForEach(s => message += $"pedido {order.OrderNumber} ({order.OrderID}), {s.PropertyName}: {s.ErrorMessage}\n");
                    _logger.AddMessage(message);
                }

                _listSomenteNovos.Add(new Domain.LinxCommerce.Entities.Reads.Orders.Order(doc_company, batchTime, order));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxCommerceOrdersRepository.BulkInsertIntoLinxCommerceOrders(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
            {
                if (ordersJson.TryGetValue(record.OrderID, out var json))
                    _logger.AddRecord(record.OrderID.ToString(), json);

                ordersCache[record.OrderID] = record.ModifiedDate;
            }

            return _listSomenteNovos.Count;
        }
    }
}
