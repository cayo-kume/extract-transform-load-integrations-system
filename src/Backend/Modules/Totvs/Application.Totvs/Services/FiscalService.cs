using Application.Shared.Abstractions.Services;
using Application.Totvs.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Enums;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Fiscal;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Totvs.Services
{
    public class FiscalService : TotvsBaseSyncService<Domain.Totvs.Dtos.Fiscal.Item, Invoice, string>, IFiscalService
    {
        private readonly IFiscalRepository _fiscalRepository;

        public FiscalService(
            [FromKeyedServices("TotvsHmg")] IApiClient apiClient,
            ILoggerService logger,
            IValidator<Domain.Totvs.Dtos.Fiscal.Item> invoiceValidator,
            IFiscalRepository fiscalRepository,
            IMemoryCache cache)
            : base(apiClient, logger, cache, invoiceValidator)
        {
            _fiscalRepository = fiscalRepository;
        }

        protected override EnumJob JobName => EnumJob.TotvsFiscal;
        protected override string CacheKey => "Totvs:Invoices";
        protected override string ApiRoute => "/fiscal/v2/invoices/search";
        protected override string ExpandFields => "eletronic,ecf,sat,shippingCompany,person,productionOrder,salesOrder,items,products,referencedInvoices,taxes,barCodes,rateDifferential,payments,digitalWalletInformation,observationNF,referencedTaxInvoice";
        protected override int PageSize => 100;
        protected override DateTime StartDate => DateTime.Now.AddMonths(-5);

        protected override string GetKey(Domain.Totvs.Dtos.Fiscal.Item dto) => dto.invoiceSequence;

        protected override string GetMaxChangeFilterDate(Domain.Totvs.Dtos.Fiscal.Item dto) => dto.maxChangeFilterDate;

        protected override Invoice ConvertToEntity(Domain.Totvs.Dtos.Fiscal.Item dto) => new Invoice(dto);

        protected override string GetLogKey(Invoice entity) => entity.invoiceSequence.ToString();

        protected override string? GetLogResponse(Invoice entity) =>
            entity.Responses
                .Where(pair => pair.Key == entity.invoiceSequence.ToString())
                .Select(pair => pair.Value)
                .FirstOrDefault();

        protected override string BuildValidationErrorMessage(Domain.Totvs.Dtos.Fiscal.Item dto, string error) =>
            $"Error when convert record - invoiceCode: {dto.invoiceSequence} | branchCnpj: {dto.branchCnpj}\n{error}";

        protected override async Task<List<Domain.Totvs.Dtos.Fiscal.Item?>> GetExistingFromDb(List<string> keys) =>
            await _fiscalRepository.GetExistingInvoices(keys.Cast<string?>().ToList());

        protected override Task BulkInsert(List<Invoice> entities, Guid? executionGuid) =>
            _fiscalRepository.BulkInsertIntoTableInvoicesRaw(entities, executionGuid);

        protected override List<Domain.Totvs.Dtos.Fiscal.Item> ExtractItems(string response, out int totalPages)
        {
            var root = Newtonsoft.Json.JsonConvert.DeserializeObject<Domain.Totvs.Dtos.Fiscal.Root>(response);
            totalPages = root.totalPages;
            return root.items.ToList();
        }

        public async Task<bool?> PostSearchInvoices() => await PostSearchCore();
    }
}
