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

                    totalInseridos += await SaveSalesRepresentatives(company.doc_company!, batchTime, salesRepresentatives, salesRepresentativesJson);
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
        /// busca um vendedor pelo SalesRepresentativeID, grava e registra a chamada em linxcommerce.LinxCommerceRequests
        /// </summary>
        public async Task<bool?> GetSalesRepresentative(string user_name, string doc_company, int salesRepresentativeId)
        {
            _logger
                .Clear()
                .AddLog(EnumJob.LinxCommerceGetSalesRepresentative);

            try
            {
                var company = await _linxCommerceService.GetCompany(doc_company);

                if (company is null)
                {
                    // nenhuma chamada à API: não há o que registrar em LinxCommerceRequests (igual ao Totvs)
                    _logger.AddMessage($"Empresa {doc_company} não cadastrada em linxcommerce.LinxCommerceParameters.");
                    return null;
                }

                var response = await _linxCommerceService.ReadOnDemand<Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives.GetSalesRepresentative>(
                    company,
                    EnumJob.LinxCommerceGetSalesRepresentative,
                    user_name,
                    "Sales/API.svc/web/GetSalesRepresentative",
                    new { SalesRepresentativeID = salesRepresentativeId },
                    salesRepresentativeId.ToString()
                );

                var salesRepresentative = response.Dto?.SalesRepresentative;

                // "não existe" é o 404 ou a resposta 2xx sem o vendedor pedido (a conferir com a API real)
                if (response.StatusCode == 404 || (response.IsValid && ConverterValidatorsExtensions.ConvertToInt32Validation(salesRepresentative?.SalesRepresentativeID) != salesRepresentativeId))
                {
                    _logger.AddMessage($"Vendedor {salesRepresentativeId} não encontrado no Linx Commerce (empresa {doc_company}).");
                    return false;
                }

                if (!response.IsValid)
                    return null;   // a mensagem já foi para o log

                await SaveSalesRepresentatives(doc_company, _linxCommerceService.GetBatchTime(), [salesRepresentative!], new Dictionary<int, string> { { salesRepresentativeId, response.Json! } });

                _logger.AddMessage($"Concluída com sucesso: vendedor {salesRepresentativeId} inserido ou atualizado!");

                return true;
            }
            finally
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
        }

        /// <summary>
        /// valida, monta as entidades (com o mesmo horário de lote) e grava. Devolve a quantidade de vendedores gravados.
        /// </summary>
        private async Task<int> SaveSalesRepresentatives(
            string doc_company,
            DateTime batchTime,
            List<Domain.LinxCommerce.Dtos.Reads.SalesRepresentatives.SalesRepresentative> salesRepresentatives,
            IReadOnlyDictionary<int, string> salesRepresentativesJson)
        {
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

                _listSomenteNovos.Add(new Domain.LinxCommerce.Entities.Reads.SalesRepresentatives.SalesRepresentative(doc_company, batchTime, salesRepresentative));
            }

            if (_listSomenteNovos.Count == 0)
                return 0;

            await _linxCommerceSalesRepresentativesRepository.BulkInsertIntoLinxCommerceSalesRepresentatives(_listSomenteNovos, _logger.GetExecutionGuid());

            foreach (var record in _listSomenteNovos)
                if (salesRepresentativesJson.TryGetValue(record.SalesRepresentativeID, out var json))
                    _logger.AddRecord(record.SalesRepresentativeID.ToString(), json);

            return _listSomenteNovos.Count;
        }
    }
}
