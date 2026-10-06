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
    public class LinxProdutosDetalhesService : ILinxProdutosDetalhesService
    {
        private readonly IAPICall _apiCall;
        private readonly ILoggerService _logger;
        private readonly ILinxMicrovixService _linxMicrovixServiceBase;
        private readonly IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDetalhes> _validator;
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ILinxMicrovixCommandHandler _linxMicrovixCommandHandler;
        private readonly ILinxProdutosDetalhesRepository _linxProdutosDetalhesRepository;
        private static List<string?> _errorMessagesList { get; set; } = new List<string?>();
        //NÃO ADICIONADO SISTEMA DE CACHE POR CONTA DE REGRA DE NEGOCIO, A PROC DE SINCRONIZAÇÃO PRECISA RECEBER TODOS OS DADOS DA API EM TODAS AS EXECUÇÕES DO JOB

        public LinxProdutosDetalhesService(
            IAPICall apiCall,
            ILoggerService logger,
            ILinxMicrovixService linxMicrovixServiceBase,
            IIntegrationsCoreRepository coreRepository,
            IValidator<Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDetalhes> validator,
            ILinxMicrovixCommandHandler linxMicrovixCommandHandler,
            ILinxProdutosDetalhesRepository linxProdutosDetalhesRepository
        )
        {
            _apiCall = apiCall;
            _validator = validator;
            _logger = logger;
            _linxMicrovixServiceBase = linxMicrovixServiceBase;
            _coreRepository = coreRepository;
            _linxMicrovixCommandHandler = linxMicrovixCommandHandler;
            _linxProdutosDetalhesRepository = linxProdutosDetalhesRepository;
        }

        public List<LinxProdutosDetalhes?> DeserializeXMLToObject(LinxAPIParam jobParameter, List<Dictionary<string?, string?>> records)
        {
            var list = new List<LinxProdutosDetalhes>();

            for (int i = 0; i < records.Count(); i++)
            {
                try
                {
                    var entity = new Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosDetalhes(
                        cnpj_emp: records[i].Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault(),
                        cod_produto: records[i].Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault(),
                        cod_barra: records[i].Where(pair => pair.Key == "cod_barra").Select(pair => pair.Value).FirstOrDefault(),
                        quantidade: records[i].Where(pair => pair.Key == "quantidade").Select(pair => pair.Value).FirstOrDefault(),
                        preco_custo: records[i].Where(pair => pair.Key == "preco_custo").Select(pair => pair.Value).FirstOrDefault(),
                        preco_venda: records[i].Where(pair => pair.Key == "preco_venda").Select(pair => pair.Value).FirstOrDefault(),
                        custo_medio: records[i].Where(pair => pair.Key == "custo_medio").Select(pair => pair.Value).FirstOrDefault(),
                        id_config_tributaria: records[i].Where(pair => pair.Key == "id_config_tributaria").Select(pair => pair.Value).FirstOrDefault(),
                        desc_config_tributaria: records[i].Where(pair => pair.Key == "desc_config_tributaria").Select(pair => pair.Value).FirstOrDefault(),
                        despesas1: records[i].Where(pair => pair.Key == "despesas1").Select(pair => pair.Value).FirstOrDefault(),
                        qtde_minima: records[i].Where(pair => pair.Key == "qtde_minima").Select(pair => pair.Value).FirstOrDefault(),
                        qtde_maxima: records[i].Where(pair => pair.Key == "qtde_maxima").Select(pair => pair.Value).FirstOrDefault(),
                        ipi: records[i].Where(pair => pair.Key == "ipi").Select(pair => pair.Value).FirstOrDefault(),
                        empresa: records[i].Where(pair => pair.Key == "empresa").Select(pair => pair.Value).FirstOrDefault(),
                        timestamp: records[i].Where(pair => pair.Key == "timestamp").Select(pair => pair.Value).FirstOrDefault(),
                        portal: records[i].Where(pair => pair.Key == "portal").Select(pair => pair.Value).FirstOrDefault()
                    );

                    var xml = records[i].Where(pair => pair.Key == "recordXml").Select(pair => pair.Value).FirstOrDefault();
                    var validations = _validator.Validate(entity);

                    if (validations.Errors.Count() > 0)
                    {
                        var message = $"Error when convert record - cnpj_emp: {records[i].Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault()} | cod_produto: {records[i].Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault()}" + "\n";

                        for (int j = 0; j < validations.Errors.Count(); j++)
                        {
                            var msg = validations.Errors[j].ErrorMessage;
                            var property = validations.Errors[j].PropertyName;
                            var value = validations.Errors[j].FormattedMessagePlaceholderValues.Where(x => x.Key == "PropertyValue").FirstOrDefault().Value;
                            message += $"{msg.Replace("[0]", $"{property}: {value}")}";
                        }

                        _errorMessagesList.Add(message);
                    }

                    list.Add(new LinxProdutosDetalhes(entity, xml));
                }
                catch (Exception ex)
                {
                    throw new GeneralException(
                        message: $"Error when convert record - cnpj_emp: {records[i].Where(pair => pair.Key == "cnpj_emp").Select(pair => pair.Value).FirstOrDefault()} | cod_produto: {records[i].Where(pair => pair.Key == "cod_produto").Select(pair => pair.Value).FirstOrDefault()}",
                            exceptionMessage: ex.StackTrace
                    );
                }
            }
            ;

            return list;
        }

        public async Task<bool> GetRecord(LinxAPIParam jobParameter, string? identificador, string? cnpj_emp)
        {
            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var body = _linxMicrovixServiceBase.BuildBodyRequest(
                parametersList: parameters
                .Replace("[0]", "0")
                .Replace("[data_mov_ini]", "2000-01-01")
                .Replace("[data_mov_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                .Replace("[cod_produto]", identificador),
                jobParameter: jobParameter,
                cnpj_emp: cnpj_emp);

            string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
            var xmls = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObject(jobParameter, xmls);

                foreach (var record in listRecords)
                {
                    await _linxProdutosDetalhesRepository.InsertRecord(record: record, jobParameter: jobParameter);
                }

                await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());
            }

            return true;
        }

        public async Task<bool> GetRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxProdutosDetalhes);

            var xmls = new List<Dictionary<string?, string?>>();

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            var sqlCnpjs = _linxMicrovixCommandHandler.CreateGetMicrovixCompanysQuery();
            var cnpjs_emp = await _coreRepository.GetRecords<Company>(sqlCnpjs);

            foreach (var cnpj_emp in cnpjs_emp)
            {
                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                            parametersList: parameters.Replace("[0]", "0").Replace("[data_mov_ini]", $"2000-01-01").Replace("[data_mov_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}"),
                            jobParameter: jobParameter,
                            cnpj_emp: cnpj_emp.Doc
                        );

                string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                var result = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);
                xmls.AddRange(result);
            }

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObject(jobParameter, xmls);

                if (listRecords.Count() > 0)
                {
                    _linxProdutosDetalhesRepository.BulkInsertIntoTableRaw(records: listRecords, jobParameter: jobParameter);
                    await _coreRepository.CallDbProcMerge(jobParameter.schema, jobParameter.tableName, _logger.GetExecutionGuid());

                    for (int i = 0; i < listRecords.Count; i++)
                    {
                        _logger.AddMessage(_errorMessagesList.Where(x => x.Contains($"cnpj_emp: {listRecords[i].cnpj_emp} | cod_produto: {listRecords[i].cod_produto}")).FirstOrDefault());
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

        public async Task<bool> GetZeroStockRecords(LinxAPIParam jobParameter)
        {
            _logger
               .Clear()
               .AddLog(EnumJob.LinxProdutosDetalhes);

            var xmls = new List<Dictionary<string?, string?>>();
            var produtos = await _linxProdutosDetalhesRepository.GetRegistersExists(jobParameter);

            string sql = _linxMicrovixCommandHandler.CreateGetParametersQuery(jobParameter.parametersInterval, jobParameter.ParametersTableName, jobParameter.jobName);
            string? parameters = await _coreRepository.GetRecord<string>(sql);

            foreach (var produto in produtos)
            {
                var body = _linxMicrovixServiceBase.BuildBodyRequest(
                                parametersList: parameters
                                .Replace("[0]", "0")
                                .Replace("[data_mov_ini]", "2000-01-01")
                                .Replace("[data_mov_fim]", $"{DateTime.Today.ToString("yyyy-MM-dd")}")
                                .Replace("[cod_produto]", $"{produto.cod_produto}"),
                            jobParameter: jobParameter,
                            cnpj_emp: produto.cnpj_emp);

                string? response = await _apiCall.PostAsync(jobParameter: jobParameter, body: body);
                var result = _linxMicrovixServiceBase.DeserializeResponseToXML(jobParameter, response);
                xmls.AddRange(result);
            }

            if (xmls.Count() > 0)
            {
                var listRecords = DeserializeXMLToObject(jobParameter, xmls);

                if (listRecords.Count() > 0)
                {
                    _linxProdutosDetalhesRepository.BulkInsertIntoTableRaw(records: listRecords, jobParameter: jobParameter);
                }

                _logger.AddMessage(
                    $"Concluída com sucesso: {listRecords.Count} registro(s) novo(s) inserido(s)!"
                );
            }

            _logger.SetLogEndDate();
            await _logger.CommitAllChanges();

            return true;
        }
    }
}


