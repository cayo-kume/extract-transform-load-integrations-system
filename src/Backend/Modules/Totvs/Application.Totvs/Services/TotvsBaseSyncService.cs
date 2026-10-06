using Application.Shared.Abstractions.Services;
using Domain.Shared.Abstractions.Api;
using Domain.Shared.Enums;
using Domain.Shared.Extensions;
using Domain.Totvs.Entities;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Application.Totvs.Services
{
    public abstract class TotvsBaseSyncService<TDto, TEntity, TKey>
        where TDto : class
        where TEntity : class
        where TKey : notnull
    {
        protected readonly IApiClient _apiClient;
        protected readonly ILoggerService _logger;
        protected readonly IMemoryCache _cache;
        protected readonly IValidator<TDto> _validator;

        protected TotvsBaseSyncService(
            IApiClient apiClient,
            ILoggerService logger,
            IMemoryCache cache,
            IValidator<TDto> validator)
        {
            _apiClient = apiClient;
            _logger = logger;
            _cache = cache;
            _validator = validator;
        }

        protected abstract EnumJob JobName { get; }
        protected abstract string CacheKey { get; }
        protected abstract string ApiRoute { get; }
        protected abstract string ExpandFields { get; }
        protected abstract int PageSize { get; }
        protected abstract DateTime StartDate { get; }

        protected abstract TKey GetKey(TDto dto);
        protected abstract string GetMaxChangeFilterDate(TDto dto);
        protected abstract TEntity ConvertToEntity(TDto dto);
        protected abstract string GetLogKey(TEntity entity);
        protected abstract string? GetLogResponse(TEntity entity);
        protected abstract string BuildValidationErrorMessage(TDto dto, string error);
        protected abstract Task<List<TDto?>> GetExistingFromDb(List<TKey> keys);
        protected abstract Task BulkInsert(List<TEntity> entities, Guid? executionGuid);
        protected abstract List<TDto> ExtractItems(string response, out int totalPages);

        protected virtual object BuildFilter(DateTime startDate, DateTime endDate) => new
        {
            branchCodeList = new int[] { 1 },
            change = new
            {
                startDate = $"{startDate:yyyy-MM-dd}T00:00:00.000Z",
                endDate = $"{endDate:yyyy-MM-dd}T23:59:59.000Z"
            }
        };

        protected async Task<Token?> AuthenticateAsync()
        {
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "newbloomerstrnapiv2" },
                { "client_secret", "4860687042" },
                { "username", "INT" },
                { "password", "INTV3" }
            };

            var response = await _apiClient.PostFormAsync(
                route: "/authorization/v2/token",
                formData: parameters
            );

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(response);
        }

        protected async Task<bool?> PostSearchCore()
        {
            _logger
               .Clear()
               .AddLog(JobName);

            var token = await AuthenticateAsync();

            var itens = new List<TDto>();
            int pageIndex = 1, pageCount = 1;

            while (pageIndex <= pageCount)
            {
                var objectRequest = new
                {
                    filter = BuildFilter(StartDate, DateTime.Now),
                    order = "maxChangeFilterDate",
                    page = pageIndex,
                    pageSize = PageSize,
                    expand = ExpandFields
                };

                var response = await _apiClient.PostAsync(
                    route: ApiRoute,
                    body: objectRequest,
                    headers: new Dictionary<string, string> { { "Authorization", $"Bearer {token.access_token}" } }
                );

                var items = ExtractItems(response, out int totalPages);
                if (pageCount == 1)
                    pageCount = totalPages;

                pageIndex++;
                itens.AddRange(items);
            }

            var cached = await _cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var list = await GetExistingFromDb(itens.Select(x => GetKey(x)).ToList());

                return list
                    .Where(x => x != null)
                    .GroupBy(x => GetKey(x!))
                    .ToDictionary(g => g.Key, g => g.First()!);
            });

            itens.RemoveAll(x =>
                cached.TryGetValue(GetKey(x), out var y) &&
                ConverterValidatorsExtensions.ConvertToDateTimeValidation<string?>(GetMaxChangeFilterDate(x))
                <= DateTime.Parse(GetMaxChangeFilterDate(y), new CultureInfo("en-US"))
            );

            var entityList = new List<TEntity>();

            foreach (var dto in itens)
            {
                var validations = _validator.Validate(dto);
                if (validations.Errors.Count() > 0)
                {
                    for (int j = 0; j < validations.Errors.Count(); j++)
                    {
                        _logger.AddMessage(BuildValidationErrorMessage(dto, validations.Errors[j].ToString()));
                    }
                }

                entityList.Add(ConvertToEntity(dto));
            }

            if (entityList.Count() > 0)
            {
                await BulkInsert(entityList, _logger.GetExecutionGuid());

                foreach (var item in itens.Where(x => x != null))
                    cached[GetKey(item)] = item;

                entityList.ForEach(e =>
                    _logger.AddRecord(GetLogKey(e), GetLogResponse(e))
                );
            }

            _logger.AddMessage($"Concluída com sucesso: {entityList.Count()} registro(s) novo(s) inserido(s)!");
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}
