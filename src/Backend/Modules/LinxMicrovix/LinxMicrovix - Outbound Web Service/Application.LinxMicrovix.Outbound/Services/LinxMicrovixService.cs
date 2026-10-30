using Application.LinxMicrovix.Outbound.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Entities.Parameters;
using Domain.LinxMicrovix.Outbound.Enums;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace Application.LinxMicrovix.Outbound.Services
{
    /// <summary>
    /// métodos em comum usados pelos services do módulo (mesmo papel do MovideskService e do TotvsService)
    /// </summary>
    public class LinxMicrovixService : ILinxMicrovixService
    {
        private readonly IMemoryCache _cache;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixRepository _linxMicrovixRepository;

        private const string ParametersKey = "LinxMicrovix:Parameters";

        // CNPJ entra interpolado nas queries: só passa com 14 dígitos
        private static readonly Regex CnpjPattern = new(@"^\d{14}$", RegexOptions.Compiled);

        // nomes de coluna vêm do código dos services (lista fechada), mas também entram interpolados: só identificadores
        private static readonly Regex ColumnPattern = new(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

        public LinxMicrovixService(IMemoryCache cache, ILoggerService logger, ILinxMicrovixRepository linxMicrovixRepository) =>
            (_cache, _logger, _linxMicrovixRepository) = (cache, logger, linxMicrovixRepository);

        public DateTime GetBatchTime() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public async Task<IEnumerable<Company>> GetCompanys()
        {
            if (_cache.TryGetValue(ParametersKey, out List<Company>? cached) && cached is not null)
                return cached;

            var companys = (await _linxMicrovixRepository.GetCompanys())
                .Where(x => x?.doc_company is not null && CnpjPattern.IsMatch(x.doc_company))
                .Select(x => x!)
                .ToList();

            // tabela vazia não entra no cache: um insert em LinxMicrovixParameters vale já na próxima execução
            if (companys.Count > 0)
                _cache.Set(ParametersKey, companys, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });

            return companys;
        }

        public async Task<Company?> GetCompany(string doc_company) =>
            (await GetCompanys()).FirstOrDefault(x => x.doc_company == doc_company);

        public void SaveRequest(EnumJob job, string user_name, string httpMethod, string route, string doc_company, string field_key_value, string? responseBody, bool isSuccess, string? requestBody = null) =>
            _linxMicrovixRepository.InsertIntoLinxMicrovixRequests(
                new Domain.LinxMicrovix.Outbound.Entities.Writes.Requests.Request(
                    execution: _logger.GetExecutionGuid() ?? Guid.Empty,
                    job: job,
                    user_name: user_name,
                    http_method: httpMethod,
                    route: route,
                    doc_company: doc_company,
                    field_key_value: field_key_value,
                    request_body: requestBody,
                    response_body: responseBody,
                    is_success: isSuccess
                )
            );

        public async Task<List<string>> GetStores(Company company, EnumApi api) =>
            (await _linxMicrovixRepository.GetStores(company.doc_company!, api))
                .Where(x => CnpjPattern.IsMatch(x))
                .ToList();

        public async Task<long> GetLastTimestamp(Company company, EnumJob method, string? cnpj_emp)
        {
            // a tabela tem o nome do método da API, que é o nome do job (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 2.3)
            if (method == EnumJob.None || !Enum.IsDefined(method))
                throw new ArgumentOutOfRangeException(nameof(method), method, "Método do Microvix sem job no EnumJob.");

            if (cnpj_emp is not null && !CnpjPattern.IsMatch(cnpj_emp))
                throw new ArgumentException($"CNPJ de loja inválido: {cnpj_emp}", nameof(cnpj_emp));

            return await _linxMicrovixRepository.GetLastTimestamp(company.doc_company!, method.ToString(), cnpj_emp);
        }

        public async Task<long> GetLastTimestamp(Company company, EnumJob method, string? cnpj_emp, string filterColumn, string filterValue)
        {
            if (method == EnumJob.None || !Enum.IsDefined(method))
                throw new ArgumentOutOfRangeException(nameof(method), method, "Método do Microvix sem job no EnumJob.");

            if (cnpj_emp is not null && !CnpjPattern.IsMatch(cnpj_emp))
                throw new ArgumentException($"CNPJ de loja inválido: {cnpj_emp}", nameof(cnpj_emp));

            if (!ColumnPattern.IsMatch(filterColumn))
                throw new ArgumentException($"Coluna inválida: {filterColumn}", nameof(filterColumn));

            return await _linxMicrovixRepository.GetLastTimestamp(company.doc_company!, method.ToString(), cnpj_emp, filterColumn, filterValue);
        }

        public async Task<List<string>> GetSourceValues(Company company, EnumJob source, string sourceColumn, string? cnpj_emp, EnumJob? changedSince)
        {
            if (source == EnumJob.None || !Enum.IsDefined(source))
                throw new ArgumentOutOfRangeException(nameof(source), source, "Método do Microvix sem job no EnumJob.");

            if (changedSince is not null && (changedSince == EnumJob.None || !Enum.IsDefined(changedSince.Value)))
                throw new ArgumentOutOfRangeException(nameof(changedSince), changedSince, "Método do Microvix sem job no EnumJob.");

            if (cnpj_emp is not null && !CnpjPattern.IsMatch(cnpj_emp))
                throw new ArgumentException($"CNPJ de loja inválido: {cnpj_emp}", nameof(cnpj_emp));

            if (!ColumnPattern.IsMatch(sourceColumn))
                throw new ArgumentException($"Coluna inválida: {sourceColumn}", nameof(sourceColumn));

            return (await _linxMicrovixRepository.GetSourceValues(company.doc_company!, source.ToString(), sourceColumn, cnpj_emp, changedSince?.ToString()))
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();
        }
    }
}
