using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Entities.Exceptions;
using Domain.Core.Enums;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace Application.LinxMicrovix.Outbound.WebService.Services.LinxMicrovix
{
    public abstract class LinxMicrovixBaseSyncService<TDto, TEntity>
        where TDto : class
        where TEntity : class
    {
        protected readonly IAPICall _apiCall;
        protected readonly ILoggerService _logger;
        protected readonly ILinxMicrovixService _linxMicrovixServiceBase;
        protected readonly IIntegrationsCoreRepository _coreRepository;
        protected readonly ILinxMicrovixCommandHandler _linxMicrovixCommandHandler;
        protected readonly IMemoryCache _cache;
        protected readonly IValidator<TDto> _validator;
        protected List<string?> _errorMessagesList = new List<string?>();

        protected LinxMicrovixBaseSyncService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            IMemoryCache cache,
            IValidator<TDto> validator)
        {
            _apiCall = apiCall;
            _logger = logger;
            _linxMicrovixServiceBase = linxMicrovixServiceBase;
            _coreRepository = coreRepository;
            _linxMicrovixCommandHandler = linxMicrovixCommandHandler;
            _cache = cache;
            _validator = validator;
        }

        protected abstract EnumJob JobName { get; }
        protected abstract string CacheKey { get; }

        protected abstract TDto CreateDto(Dictionary<string?, string?> record);
        protected abstract TEntity CreateEntity(TDto dto, string? xml);
        protected abstract string BuildErrorContext(Dictionary<string?, string?> record);
        protected abstract string? GetRecordKey(TEntity entity);
        protected abstract string? GetRecordXml(TEntity entity);
        protected abstract string BuildEntityLogContext(TEntity entity);
        protected abstract Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<TEntity> records);
        protected abstract void BulkInsertIntoTableRaw(List<TEntity> records, LinxAPIParam jobParameter);

        protected virtual string BuildRequestBodyParameters(string parameters, LinxAPIParam jobParameter) =>
            parameters.Replace("[0]", "0");

        protected List<TEntity> DeserializeXMLToObjectCore(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records)
        {
            var list = new List<TEntity>();

            for (int i = 0; i < records.Count(); i++)
            {
                try
                {
                    var dto = CreateDto(records[i]);
                    var xml = records[i].Where(pair => pair.Key == "recordXml").Select(pair => pair.Value).FirstOrDefault();

                    var validations = _validator.Validate(dto);
                    if (validations.Errors.Count() > 0)
                    {
                        var message = $"Error when convert record - {BuildErrorContext(records[i])}\n";

                        for (int j = 0; j < validations.Errors.Count(); j++)
                        {
                            var msg = validations.Errors[j].ErrorMessage;
                            var property = validations.Errors[j].PropertyName;
                            var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                            message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                        }

                        _errorMessagesList.Add(message);
                    }

                    list.Add(CreateEntity(dto, xml));
                }
                catch (Exception ex)
                {
                    throw new GeneralException(
                        message: $"Error when convert record - {BuildErrorContext(records[i])}",
                        exceptionMessage: ex.StackTrace
                    );
                }
            }

            return list;
        }

        protected async Task<bool> GetRecordsCore(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(JobName);

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: BuildRequestBodyParameters(parameters, jobParameter),
                cnpj_emp: jobParameter.DocMainCompany,
                jobParameter: jobParameter
            );

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObjectCore(jobParameter, xmls);

                var recordKeys = await _cache.GetOrCreateAsync(CacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                    var list = await GetRegistersExists(jobParameter, listRecords);
                    return new HashSet<string?>(list);
                });

                var _listSomenteNovos = listRecords.Where(x => !recordKeys.Contains(GetRecordKey(x))).ToList();

                if (_listSomenteNovos.Count() > 0)
                {
                    BulkInsertIntoTableRaw(_listSomenteNovos, jobParameter);
                    await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());

                    foreach (var item in _listSomenteNovos)
                        recordKeys.Add(GetRecordKey(item));

                    for (int i = 0; i < _listSomenteNovos.Count; i++)
                    {
                        _logger.AddRecord(GetRecordKey(_listSomenteNovos[i]), GetRecordXml(_listSomenteNovos[i]));
                        _logger.AddMessage(_errorMessagesList.Where(x => x != null && x.Contains(BuildEntityLogContext(_listSomenteNovos[i]))).FirstOrDefault());
                    }
                }
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}
