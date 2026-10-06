using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Enums;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.PurchaseOrder;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Totvs.Services
{
    public class PurchaseOrderService : TotvsBaseSyncService<Domain.Totvs.Dtos.PurchaseOrder.Item, PurchaseOrder, string>, IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public PurchaseOrderService(
            [FromKeyedServices("TotvsHmg")] IApiClient apiClient,
            ILoggerService logger,
            IValidator<Domain.Totvs.Dtos.PurchaseOrder.Item> purchaseOrderValidator,
            IPurchaseOrderRepository purchaseOrderRepository,
            IMemoryCache cache)
            : base(apiClient, logger, cache, purchaseOrderValidator)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        protected override EnumJob JobName => EnumJob.TotvsPurchaseOrder;
        protected override string CacheKey => "Totvs:PurchaseOrders";
        protected override string ApiRoute => "/purchase-order/v2/search";
        protected override string ExpandFields => "items";
        protected override int PageSize => 100;
        protected override DateTime StartDate => DateTime.Now.AddMonths(-5);

        protected override string GetKey(Domain.Totvs.Dtos.PurchaseOrder.Item dto) => dto.orderCode;

        protected override string GetMaxChangeFilterDate(Domain.Totvs.Dtos.PurchaseOrder.Item dto) => dto.maxChangeFilterDate;

        protected override PurchaseOrder ConvertToEntity(Domain.Totvs.Dtos.PurchaseOrder.Item dto) => new PurchaseOrder(dto);

        protected override string GetLogKey(PurchaseOrder entity) => entity.orderCode.ToString();

        protected override string? GetLogResponse(PurchaseOrder entity) =>
            entity.Responses
                .Where(pair => pair.Key == entity.orderCode.ToString())
                .Select(pair => pair.Value)
                .FirstOrDefault();

        protected override string BuildValidationErrorMessage(Domain.Totvs.Dtos.PurchaseOrder.Item dto, string error) =>
            $"Error when convert record - orderCode: {dto.orderCode} | branchCode: {dto.branchCode}\n{error}";

        protected override async Task<List<Domain.Totvs.Dtos.PurchaseOrder.Item?>> GetExistingFromDb(List<string> keys) =>
            await _purchaseOrderRepository.GetExistingPurchaseOrders(keys.Cast<string?>().ToList());

        protected override Task BulkInsert(List<PurchaseOrder> entities, Guid? executionGuid) =>
            _purchaseOrderRepository.BulkInsertIntoTablePurchaseOrdersRaw(entities, executionGuid);

        protected override List<Domain.Totvs.Dtos.PurchaseOrder.Item> ExtractItems(string response, out int totalPages)
        {
            var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.PurchaseOrder.Root>(response);
            totalPages = root.totalPages;
            return root.items.ToList();
        }

        public async Task<bool?> PostSearchPurchaseOrders() => await PostSearchCore();
    }
}
