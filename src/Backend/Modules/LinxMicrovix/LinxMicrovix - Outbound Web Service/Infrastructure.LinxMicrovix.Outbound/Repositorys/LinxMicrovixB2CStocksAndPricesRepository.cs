using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.StocksAndPrices;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxMicrovix.Outbound.Repositorys
{
    /// <summary>
    /// gravação dos métodos do LinxMicrovixB2CStocksAndPricesService: as respostas são planas, então cada método é uma tabela, com um bulk
    /// em untreated e uma merge em linxmicrovix (sem filhas, sem condição de exclusão)
    /// </summary>
    public class LinxMicrovixB2CStocksAndPricesRepository : ILinxMicrovixB2CStocksAndPricesRepository
    {
        private readonly ISharedRepository _coreRepository;

        public LinxMicrovixB2CStocksAndPricesRepository(ISharedRepository coreRepository) =>
            _coreRepository = coreRepository;

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosCustos(List<B2CConsultaProdutosCustos> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosCustos(), tableName: "B2CConsultaProdutosCustos");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosDepositos(List<B2CConsultaProdutosDepositos> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosDepositos(), tableName: "B2CConsultaProdutosDepositos");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosDetalhes(List<B2CConsultaProdutosDetalhes> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosDetalhes(), tableName: "B2CConsultaProdutosDetalhes");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosDetalhesDepositos(List<B2CConsultaProdutosDetalhesDepositos> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosDetalhesDepositos(), tableName: "B2CConsultaProdutosDetalhesDepositos");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosPromocao(List<B2CConsultaProdutosPromocao> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosPromocao(), tableName: "B2CConsultaProdutosPromocao");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosTabelas(List<B2CConsultaProdutosTabelas> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosTabelas(), tableName: "B2CConsultaProdutosTabelas");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosTabelasPrecos(List<B2CConsultaProdutosTabelasPrecos> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosTabelasPrecos(), tableName: "B2CConsultaProdutosTabelasPrecos");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }
    }
}
