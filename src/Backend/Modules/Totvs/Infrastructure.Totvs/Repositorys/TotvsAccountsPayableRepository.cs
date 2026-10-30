using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.AccountsPayable;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsAccountsPayableRepository : ITotvsAccountsPayableRepository
    {
        private readonly ITotvsAccountsPayableCommandHandler _totvsAccountsPayableCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsAccountsPayableRepository(ITotvsAccountsPayableCommandHandler totvsAccountsPayableCommandHandler, ISharedRepository coreRepository) =>
            (_totvsAccountsPayableCommandHandler, _coreRepository) = (totvsAccountsPayableCommandHandler, coreRepository);

        public async Task<DateTime?> GetDuplicatesLastChange(string doc_company)
        {
            string sql = _totvsAccountsPayableCommandHandler.CreateGetDuplicatesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Duplicate?>> GetDuplicatesByKeys(string doc_company, List<(int branchCode, int supplierCode, long duplicateCode, int installmentCode)> keys)
        {
            var result = new List<Duplicate?>();

            // lotes de 1000 chaves (o máximo de linhas do VALUES)
            foreach (var chunk in keys.Distinct().Chunk(1000))
            {
                string sql = _totvsAccountsPayableCommandHandler.CreateGetDuplicatesByKeysQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Duplicate>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsDuplicates(List<Duplicate> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: duplicata antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Duplicate(), tableName: "TotvsDuplicates"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Expense(), tableName: "TotvsDuplicatesExpenses"), data.SelectMany(x => x.expense).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new DuplicateClassification(), tableName: "TotvsDuplicatesClassifications"), data.SelectMany(x => x.classifications).ToList()),
            };

            await BulkInsertAndMerge(tables, parentExecutionGUID);

            return true;
        }

        public async Task<bool> BulkInsertIntoTotvsCommissionsPaid(List<CommissionPaid> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: fechamento antes dos movimentos (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new CommissionPaid(), tableName: "TotvsCommissionsPaid"), data),
                (_coreRepository.CreateSystemDataTable(entity: new AccountMovement(), tableName: "TotvsCommissionsPaidAccountMovements"), data.SelectMany(x => x.accountMovements).ToList()),
            };

            await BulkInsertAndMerge(tables, parentExecutionGUID);

            return true;
        }

        /// <summary>
        /// bulk em untreated de todas as tabelas e depois os merges em totvs, na ordem da lista
        /// </summary>
        private async Task BulkInsertAndMerge(List<(DataTable table, IList rows)> tables, Guid? parentExecutionGUID)
        {
            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do registro
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);
        }
    }
}
