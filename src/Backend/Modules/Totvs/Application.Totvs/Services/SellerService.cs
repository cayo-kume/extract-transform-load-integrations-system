using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Enums;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Seller;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Totvs.Services
{
    public class SellerService : TotvsBaseSyncService<Domain.Totvs.Dtos.Seller.Item, Seller, string>, ISellerService
    {
        private readonly ISellerRepository _sellerRepository;

        public SellerService(
            [FromKeyedServices("TotvsHmg")] IApiClient apiClient,
            ILoggerService logger,
            IValidator<Domain.Totvs.Dtos.Seller.Item> sellerValidator,
            ISellerRepository sellerRepository,
            IMemoryCache cache)
            : base(apiClient, logger, cache, sellerValidator)
        {
            _sellerRepository = sellerRepository;
        }

        protected override EnumJob JobName => EnumJob.TotvsFiscal;
        protected override string CacheKey => "Totvs:Sellers";
        protected override string ApiRoute => "/seller/v2/search";
        protected override string ExpandFields => "branchInformations,customers";
        protected override int PageSize => 100;
        protected override DateTime StartDate => DateTime.Now.AddMonths(-5);

        protected override string GetKey(Domain.Totvs.Dtos.Seller.Item dto) => dto.sellerCode;

        protected override string GetMaxChangeFilterDate(Domain.Totvs.Dtos.Seller.Item dto) => dto.maxChangeFilterDate;

        protected override Seller ConvertToEntity(Domain.Totvs.Dtos.Seller.Item dto) => new Seller(dto);

        protected override string GetLogKey(Seller entity) => entity.sellerCode.ToString();

        protected override string? GetLogResponse(Seller entity) =>
            entity.Responses
                .Where(pair => pair.Key == entity.sellerCode.ToString())
                .Select(pair => pair.Value)
                .FirstOrDefault();

        protected override string BuildValidationErrorMessage(Domain.Totvs.Dtos.Seller.Item dto, string error) =>
            $"Error when convert record - sellerCode: {dto.sellerCode} | sellerName: {dto.sellerName}\n{error}";

        protected override async Task<List<Domain.Totvs.Dtos.Seller.Item?>> GetExistingFromDb(List<string> keys) =>
            await _sellerRepository.GetExistingSellers(keys.Cast<string?>().ToList());

        protected override Task BulkInsert(List<Seller> entities, Guid? executionGuid) =>
            _sellerRepository.BulkInsertIntoTableSellersRaw(entities, executionGuid);

        protected override List<Domain.Totvs.Dtos.Seller.Item> ExtractItems(string response, out int totalPages)
        {
            var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.Seller.Root>(response);
            totalPages = root.totalPages;
            return root.items;
        }

        public async Task<bool?> PostSearchSellers() => await PostSearchCore();
    }
}
