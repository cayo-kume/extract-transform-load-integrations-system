using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.AccountsPayable;

namespace Infrastructure.Totvs.Repositorys
{
    public class AccountsPayableRepository : IAccountsPayableRepository
    {
        private readonly IAccountsPayableCommandHandler _commandHandler;
        private readonly IIntegrationsSharedRepository _coreRepository;

        public AccountsPayableRepository(IIntegrationsSharedRepository coreRepository, IAccountsPayableCommandHandler commandHandler) =>
            (_coreRepository, _commandHandler) = (coreRepository, commandHandler);

        public async Task<bool> BulkInsertIntoTableDuplicatesRaw(List<Duplicate> registros, Guid? parentExecutionGUID)
        {
            var duplicatesTable = _coreRepository.CreateSystemDataTable(entity: new Duplicate(), tableName: "Duplicates");
            var duplicatesExpensesTable = _coreRepository.CreateSystemDataTable(entity: new Expense(), tableName: "DuplicatesExpenses");

            _coreRepository.FillSystemDataTable(duplicatesTable, registros);
            _coreRepository.FillSystemDataTable(duplicatesExpensesTable, registros.SelectMany(d => d.expense).ToList());

            _coreRepository.BulkInsertIntoTableRaw(duplicatesTable);
            _coreRepository.BulkInsertIntoTableRaw(duplicatesExpensesTable);

            await _coreRepository.CallDbProcMerge("totvs", duplicatesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", duplicatesExpensesTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.Totvs.Dtos.AccountsPayable.Duplicate?>> GetExistingDuplicates(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.AccountsPayable.Duplicate>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingDuplicatesQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.AccountsPayable.Duplicate>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }

        public async Task<bool> BulkInsertIntoTableComissionsPaidRaw(List<ComissionPaid> registros, Guid? parentExecutionGUID)
        {
            var comissionsPaidTable = _coreRepository.CreateSystemDataTable(entity: new ComissionPaid(), tableName: "ComissionsPaid");

            _coreRepository.FillSystemDataTable(comissionsPaidTable, registros);

            _coreRepository.BulkInsertIntoTableRaw(comissionsPaidTable);

            await _coreRepository.CallDbProcMerge("totvs", comissionsPaidTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid?>> GetExistingComissionsPaid(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingComissionsPaidQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.AccountsPayable.ComissionPaid>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }
    }
}
