using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Enums;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.SalesOrder;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Totvs.Services
{
    public class SalesOrderService : TotvsBaseSyncService<Domain.Totvs.Dtos.SalesOrder.Item, SaleOrder, string>, ISalesOrderService
    {
        private readonly ISalesOrderRepository _salesOrderRepository;

        public SalesOrderService(
            [FromKeyedServices("TotvsHmg")] IApiClient apiClient,
            ILoggerService logger,
            IValidator<Domain.Totvs.Dtos.SalesOrder.Item> salesOrderValidator,
            ISalesOrderRepository salesOrderRepository,
            IMemoryCache cache)
            : base(apiClient, logger, cache, salesOrderValidator)
        {
            _salesOrderRepository = salesOrderRepository;
        }

        protected override EnumJob JobName => EnumJob.TotvsSalesOrder;
        protected override string CacheKey => "Totvs:SalesOrders";
        protected override string ApiRoute => "/sales-order/v2/orders/search";
        protected override string ExpandFields => "items,invoices,shippingAddress,observations,invoiceObservations,representativeObservations,classifications,discounts,commissioneds,counts";
        protected override int PageSize => 100;
        protected override DateTime StartDate => DateTime.Now.AddMonths(-5);

        protected override string GetKey(Domain.Totvs.Dtos.SalesOrder.Item dto) => dto.orderCode;

        protected override string GetMaxChangeFilterDate(Domain.Totvs.Dtos.SalesOrder.Item dto) => dto.maxChangeFilterDate;

        protected override SaleOrder ConvertToEntity(Domain.Totvs.Dtos.SalesOrder.Item dto) => new SaleOrder(dto);

        protected override string GetLogKey(SaleOrder entity) => entity.orderCode.ToString();

        protected override string? GetLogResponse(SaleOrder entity) =>
            entity.Responses
                .Where(pair => pair.Key == entity.orderCode.ToString())
                .Select(pair => pair.Value)
                .FirstOrDefault();

        protected override string BuildValidationErrorMessage(Domain.Totvs.Dtos.SalesOrder.Item dto, string error) =>
            $"Error when convert record - orderCode: {dto.orderCode} | branchCode: {dto.branchCode}\n{error}";

        protected override async Task<List<Domain.Totvs.Dtos.SalesOrder.Item?>> GetExistingFromDb(List<string> keys) =>
            await _salesOrderRepository.GetExistingSalesOrders(keys.Cast<string?>().ToList());

        protected override Task BulkInsert(List<SaleOrder> entities, Guid? executionGuid) =>
            _salesOrderRepository.BulkInsertIntoTableSalesOrdersRaw(entities, executionGuid);

        protected override List<Domain.Totvs.Dtos.SalesOrder.Item> ExtractItems(string response, out int totalPages)
        {
            var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.SalesOrder.Root>(response);
            totalPages = root.totalPages;
            return root.items.ToList();
        }

        public async Task<bool?> PostSearchSalesOrders() => await PostSearchCore();
    }
}
