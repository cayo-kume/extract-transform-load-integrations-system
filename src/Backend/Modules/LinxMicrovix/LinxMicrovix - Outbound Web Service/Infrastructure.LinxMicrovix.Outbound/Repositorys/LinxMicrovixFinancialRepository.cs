using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxMicrovix.Outbound.Repositorys
{
    /// <summary>
    /// gravação dos métodos do LinxMicrovixFinancialService: as respostas são planas, então cada método é uma tabela, com um bulk
    /// em untreated e uma merge em linxmicrovix (sem filhas, sem condição de exclusão)
    /// </summary>
    public class LinxMicrovixFinancialRepository : ILinxMicrovixFinancialRepository
    {
        private readonly ISharedRepository _coreRepository;

        public LinxMicrovixFinancialRepository(ISharedRepository coreRepository) =>
            _coreRepository = coreRepository;

        public async Task<bool> BulkInsertIntoLinxFaturas(List<LinxFaturas> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxFaturas(), tableName: "LinxFaturas");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxFaturasOrigem(List<LinxFaturasOrigem> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxFaturasOrigem(), tableName: "LinxFaturasOrigem");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxPortadores(List<LinxPortadores> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPortadores(), tableName: "LinxPortadores");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxAntecipacoesFinanceiras(List<LinxAntecipacoesFinanceiras> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxAntecipacoesFinanceiras(), tableName: "LinxAntecipacoesFinanceiras");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxAntecipacoesFinanceirasPlanos(List<LinxAntecipacoesFinanceirasPlanos> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxAntecipacoesFinanceirasPlanos(), tableName: "LinxAntecipacoesFinanceirasPlanos");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxDesfazimentosAntecipacoes(List<LinxDesfazimentosAntecipacoes> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxDesfazimentosAntecipacoes(), tableName: "LinxDesfazimentosAntecipacoes");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxDesfazimentosAntecipacoesDocumento(List<LinxDesfazimentosAntecipacoesDocumento> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxDesfazimentosAntecipacoesDocumento(), tableName: "LinxDesfazimentosAntecipacoesDocumento");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxDesfazimentosAntecipacoesItens(List<LinxDesfazimentosAntecipacoesItens> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxDesfazimentosAntecipacoesItens(), tableName: "LinxDesfazimentosAntecipacoesItens");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxCategoriasFinanceiras(List<LinxCategoriasFinanceiras> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxCategoriasFinanceiras(), tableName: "LinxCategoriasFinanceiras");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxCentroCusto(List<LinxCentroCusto> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxCentroCusto(), tableName: "LinxCentroCusto");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxPlanoContas(List<LinxPlanoContas> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxPlanoContas(), tableName: "LinxPlanoContas");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxLancContabil(List<LinxLancContabil> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxLancContabil(), tableName: "LinxLancContabil");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxHistoricosContabeis(List<LinxHistoricosContabeis> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxHistoricosContabeis(), tableName: "LinxHistoricosContabeis");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxExtratoBancarioLancamentos(List<LinxExtratoBancarioLancamentos> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxExtratoBancarioLancamentos(), tableName: "LinxExtratoBancarioLancamentos");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoLinxExtratoBancarioConciliacoes(List<LinxExtratoBancarioConciliacoes> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new LinxExtratoBancarioConciliacoes(), tableName: "LinxExtratoBancarioConciliacoes");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }
    }
}
