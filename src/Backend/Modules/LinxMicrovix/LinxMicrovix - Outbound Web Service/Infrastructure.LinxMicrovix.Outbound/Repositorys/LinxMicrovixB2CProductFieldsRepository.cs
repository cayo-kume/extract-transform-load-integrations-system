using Domain.LinxMicrovix.Outbound.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.ProductFields;
using Domain.Shared.Abstractions.Repositorys;

namespace Infrastructure.LinxMicrovix.Outbound.Repositorys
{
    /// <summary>
    /// gravação dos métodos do LinxMicrovixB2CProductFieldsService: as respostas são planas, então cada método é uma tabela, com um bulk
    /// em untreated e uma merge em linxmicrovix (sem filhas, sem condição de exclusão)
    /// </summary>
    public class LinxMicrovixB2CProductFieldsRepository : ILinxMicrovixB2CProductFieldsRepository
    {
        private readonly ISharedRepository _coreRepository;

        public LinxMicrovixB2CProductFieldsRepository(ISharedRepository coreRepository) =>
            _coreRepository = coreRepository;

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosCamposAdicionaisDetalhes(List<B2CConsultaProdutosCamposAdicionaisDetalhes> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosCamposAdicionaisDetalhes(), tableName: "B2CConsultaProdutosCamposAdicionaisDetalhes");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosCamposAdicionaisNomes(List<B2CConsultaProdutosCamposAdicionaisNomes> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosCamposAdicionaisNomes(), tableName: "B2CConsultaProdutosCamposAdicionaisNomes");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaProdutosCamposAdicionaisValores(List<B2CConsultaProdutosCamposAdicionaisValores> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaProdutosCamposAdicionaisValores(), tableName: "B2CConsultaProdutosCamposAdicionaisValores");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionais(List<B2CConsultaB2CProdutosCamposAdicionais> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaB2CProdutosCamposAdicionais(), tableName: "B2CConsultaB2CProdutosCamposAdicionais");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisDados(List<B2CConsultaB2CProdutosCamposAdicionaisDados> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaB2CProdutosCamposAdicionaisDados(), tableName: "B2CConsultaB2CProdutosCamposAdicionaisDados");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisMedia(List<B2CConsultaB2CProdutosCamposAdicionaisMedia> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaB2CProdutosCamposAdicionaisMedia(), tableName: "B2CConsultaB2CProdutosCamposAdicionaisMedia");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisMediaDados(List<B2CConsultaB2CProdutosCamposAdicionaisMediaDados> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaB2CProdutosCamposAdicionaisMediaDados(), tableName: "B2CConsultaB2CProdutosCamposAdicionaisMediaDados");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }

        public async Task<bool> BulkInsertIntoB2CConsultaB2CProdutosCamposAdicionaisOpcoes(List<B2CConsultaB2CProdutosCamposAdicionaisOpcoes> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new B2CConsultaB2CProdutosCamposAdicionaisOpcoes(), tableName: "B2CConsultaB2CProdutosCamposAdicionaisOpcoes");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            return await _coreRepository.CallDbProcMerge("linxmicrovix", table.TableName, parentExecutionGUID);
        }
    }
}
