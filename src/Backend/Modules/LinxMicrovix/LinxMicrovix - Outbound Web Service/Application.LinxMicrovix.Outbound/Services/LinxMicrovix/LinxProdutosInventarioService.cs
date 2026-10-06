using Application.Core.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Entities.Exceptions;
using Domain.Core.Enums;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Base;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.Services.LinxMicrovix
{
    public class LinxProdutosInventarioService : ILinxProdutosInventarioService
    {
        private readonly IAPICall _apiCall;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixServiceBase;
        private readonly ILinxProdutosInventarioRepository _linxProdutosInventarioRepository;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosInventario> _validator;
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxMicrovixCommandHandler _linxMicrovixCommandHandler;
        private static List<string?> _errorMessagesList { get; set; } = new List<string?>();
        //NÃO ADICIONADO SISTEMA DE CACHE POR CONTA DE REGRA DE NEGOCIO, A PROC DE SINCRONIZAÇÃO PRECISA RECEBER TODOS OS DADOS DA API EM TODAS AS EXECUÇÕES DO JOB

        public LinxProdutosInventarioService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            ILinxProdutosInventarioRepository linxProdutosInventarioRepository,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosInventario> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler
        )
        {
            _apiCall = apiCall;
            _validator = validator;
            _logger = logger;
            _linxMicrovixServiceBase = linxMicrovixServiceBase;
            _linxProdutosInventarioRepository = linxProdutosInventarioRepository;
            _coreRepository = coreRepository;
            _linxMicrovixCommandHandler = linxMicrovixCommandHandler;

        }

        public List<LinxProdutosInventario?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records)
        {
            var list = new List<LinxProdutosInventario>();

            for (int i = 0; i < records.Count(); i++)
            {
                try
                {
                    var entity = new Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosInventario(
                        cnpj_emp: records[i].Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                        cod_produto: records[i].Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault(),
                        cod_barra: records[i].Where(pair => pair.Key == "cod_barra").Select(pair => pair.Value).FirstOrDefault(),
                        cod_deposito: records[i].Where(pair => pair.Key == "cod_deposito").Select(pair => pair.Value).FirstOrDefault(),
                        quantidade: records[i].Where(pair => pair.Key == "quantidade").Select(pair => pair.Value).FirstOrDefault(),
                        empresa: records[i].Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                        portal: records[i].Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
                    );

                    var xml = records[i].Where(pair => pair.Key == "recordXml").Select(pair => pair.Value).FirstOrDefault();
                    var validations = _validator.Validate(entity);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = $"Error when convert record - cnpj_emp: {records[i].Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault()} | cod_produto: {records[i].Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault()} | cod_deposito: {records[i].Where(pair => pair.Key == "cod_deposito").Select(pair => pair.Value).FirstOrDefault()}" + "\n";

                        for (int j = 0; j < validations.Errors.Count(); j++)
                        {
                            var msg = validations.Errors[j].ErrorMessage;
                            var property = validations.Errors[j].PropertyName;
                            var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                            message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                        }

                        _errorMessagesList.Add(message);
                    }

                    list.Add(new LinxProdutosInventario(entity, xml));
                }
                catch (Exception ex)
                {
                    throw new GeneralException(
                        message: $"Error when convert record - cnpj_emp: {records[i].Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault()} | cod_produto: {records[i].Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault()} cod_deposito: {records[i].Where(pair => pair.Key == "cod_deposito").Select(pair => pair.Value).FirstOrDefault()}| ",
                            exceptionMessage: ex.StackTrace
                    );
                }
            }
            ;

            return list;
        }

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? identificador2, string? identificador3, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters
                .Replace("[0]", "0")
                .Replace("[data_inventario]", "2000-01-01")
                .Replace("[data_inventario]", identificador2)
                .Replace("[cod_produto]", identificador)
                .Replace("[cod_deposito]", identificador3),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObject(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxProdutosInventarioRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxProdutosInventario);

            var xmls = new List<Dictionary<string?, string?>>();

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var sqlCnpjs = _linxMicrovixCommandHandler.CreateGetMicrovixCompanysQuery();
            var cnpjs_emp = await _coreRepository.GetRecords<Company>(sqlCnpjs);

            var depositos = await _linxProdutosInventarioRepository.GetProductsDepositsIds(jobParameter);

            foreach (var cod_deposito in depositos)
            {
                foreach (var cnpj_emp in cnpjs_emp)
                {

                    var body = _linxMicrovixServiceBase.BuildBodyRequest(
                        parametersList: parameters.Replace("[cod_deposito]", cod_deposito).Replace("[data_inventario]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                        jobParameter: jobParameter,
                        cnpj_emp: cnpj_emp.Doc
                    );

                    string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                    var result = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);
                    xmls.AddRange(result);
                }
            }

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObject(jobParameter, xmls);

                if (listRecords.Count() > 0)
                {
                    _linxProdutosInventarioRepository.BulkInsertIntoTableRaw(records: listRecords, jobParameter: jobParameter);
                    await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());

                    for (int i = 0; i < listRecords.Count; i++)
                    {
                        _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"cnpj_emp: {listRecords[i].cnpj_emp} | cod_produto: {listRecords[i].cod_produto} | cod_deposito: {listRecords[i].cod_deposito}")).FirstOrDefault());
                    }
                    
                    _logger.AddMessage(
                        $"Concluída com sucesso: {listRecords.Count} registro(s) novo(s) inserido(s)!"
                    );
                }
            }

            _errorMessagesList.Clear();
            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}


