using Application.LinxCommerce.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Enums;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Services
{
    /// <summary>
    /// vendedores do Linx Commerce (API Sales), só leitura. Cada método faz as próprias chamadas; o comum vem do ILinxCommerceService
    /// </summary>
    public class LinxCommerceSalesRepresentativesService : ILinxCommerceSalesRepresentativesService
    {
        private readonly ILoggerService _logger;
        private readonly ILinxCommerceService _linxCommerceService;
        private readonly ILinxCommerceSalesRepresentativesRepository _linxCommerceSalesRepresentativesRepository;
        private readonly IValidator<Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives.SalesRepresentative> _salesRepresentativeValidator;

        public LinxCommerceSalesRepresentativesService(
            ILoggerService logger,
            ILinxCommerceService linxCommerceService,
            ILinxCommerceSalesRepresentativesRepository linxCommerceSalesRepresentativesRepository,
            IValidator<Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives.SalesRepresentative> salesRepresentativeValidator
        ) =>
            (_logger, _linxCommerceService, _linxCommerceSalesRepresentativesRepository, _salesRepresentativeValidator) =
            (logger, linxCommerceService, linxCommerceSalesRepresentativesRepository, salesRepresentativeValidator);

        public async Task<bool?> SearchSalesRepresentative()
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceSearchSalesRepresentative);

            var totalInseridos = 0;

            try
            {
                foreach (var company in await _linxCommerceService.GetCompanys())
                {
                    var batchTime = _linxCommerceService.GetBatchTime();

                    // lidos inteiros, uma vez por dia: o código antigo não filtrava, e o filtro por data está a conferir (4.2, item 3)
                    var found = await _linxCommerceService.Search<Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives.SearchSalesRepresentativeResult>(
                        company,
                        "Sales/API.svc/web/SearchSalesRepresentative",
                        "",
                        "SalesRepresentativeID"
                    );

                    if (found is null)
                        continue;

                    var ids = found
                        .Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.dto.SalesRepresentativeID))
                        .Where(x => x != 0)
                        .Distinct()
                        .ToList();

                    if (ids.Count == 0)
                        continue;

                    var salesRepresentatives = new List<Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives.SalesRepresentative>();
                    var salesRepresentativesJson = new Dictionary<int, string>();
                    var failed = false;

                    foreach (var id in ids)
                    {
                        // o GetSalesRepresentative recebe { SalesRepresentativeID } e devolve { SalesRepresentative }, como no código antigo
                        var response = await _linxCommerceService.Get<Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives.GetSalesRepresentative>(
                            company,
                            "Sales/API.svc/web/GetSalesRepresentative",
                            new { SalesRepresentativeID = id },
                            $"vendedor {id}"
                        );

                        var salesRepresentative = response?.dto.SalesRepresentative;

                        if (salesRepresentative is null || ConverterValidatorsExtensions.ConvertToInt32Validation(salesRepresentative.SalesRepresentativeID) != id)
                        {
                            if (response is not null)
                                _logger.AddMessage($"GetSalesRepresentative do vendedor {id} (empresa {company.doc_company}) não devolveu o vendedor {id}.");

                            failed = true;
                            break;
                        }

                        salesRepresentatives.Add(salesRepresentative);
                        salesRepresentativesJson[id] = response!.Value.json;
                    }

                    // leitura incompleta não é gravada (mesma regra das outras leituras)
                    if (failed)
                        continue;

                    var _listSomenteNovos = new List<Domain.LinxCommerce.Entities.Reads.SalesRepresentatives.SalesRepresentative>();

                    foreach (var salesRepresentative in salesRepresentatives)
                    {
                        var validations = _salesRepresentativeValidator.Validate(salesRepresentative);

                        if (validations.Errors.Count > 0)
                        {
                            var message = String.Empty;
                            validations.Errors.ForEach(s => message += $"vendedor {salesRepresentative.SalesRepresentativeID}, {s.PropertyName}: {s.ErrorMessage}\n");
                            _logger.AddMessage(message);
                        }

                        _listSomenteNovos.Add(new Domain.LinxCommerce.Entities.Reads.SalesRepresentatives.SalesRepresentative(company.doc_company!, batchTime, salesRepresentative));
                    }

                    await _linxCommerceSalesRepresentativesRepository.BulkInsertIntoLinxCommerceSalesRepresentatives(_listSomenteNovos, _logger.GetExecutionGuid());

                    foreach (var record in _listSomenteNovos)
                        if (salesRepresentativesJson.TryGetValue(record.SalesRepresentativeID, out var json))
                            _logger.AddRecord(record.SalesRepresentativeID.ToString(), json);

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
    }
}
