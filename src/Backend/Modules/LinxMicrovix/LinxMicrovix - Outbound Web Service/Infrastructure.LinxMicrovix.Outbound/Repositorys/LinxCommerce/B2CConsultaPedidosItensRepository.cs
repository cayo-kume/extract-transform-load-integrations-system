using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;
using Domain.Core.Abstractions.Repositorys;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxCommerce
{
    public class B2CConsultaPedidosItensRepository : IB2CConsultaPedidosItensRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly IB2CConsultaPedidosItensCommandHandler _commandHandler;

        public B2CConsultaPedidosItensRepository(IIntegrationsCoreRepository coreRepository, IB2CConsultaPedidosItensCommandHandler commandHandler)
        {
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public bool BulkInsertIntoTableRaw(LinxAPIParam jobParameter, IList<B2CConsultaPedidosItens> records)
        {
            var table = _coreRepository.CreateSystemDataTable(jobParameter.tableName, new B2CConsultaPedidosItens());

            for (int i = 0; i < records.Count(); i++)
            {
                table.Rows.Add(records[i].lastupdateon, records[i].id_pedido_item, records[i].id_pedido, records[i].codigoproduto, records[i].quantidade, records[i].vl_unitario, records[i].timestamp, records[i].portal);
            }

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: table
            );

            return true;
        }

        public async Task<IEnumerable<string?>> GetRegistersExists(LinxAPIParam jobParameter, List<long?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<string?>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();
                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetRegistersExistsQuery(batch);
                    var result = await _coreRepository.GetRecords<string>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }

        public async Task<bool> InsertRecord(LinxAPIParam jobParameter, B2CConsultaPedidosItens? record)
        {
            string sql = _commandHandler.CreateInsertRecordQuery(jobParameter.tableName);
            return await _coreRepository.InsertRecord(sql: sql, entity: record);
        }
    }
}