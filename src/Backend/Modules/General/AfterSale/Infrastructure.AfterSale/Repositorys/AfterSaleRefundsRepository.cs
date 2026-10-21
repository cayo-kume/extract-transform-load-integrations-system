using Application.AfterSale.Abstractions.Handlers.Commands;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Repositorys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AfterSale.Repositorys
{
    public class AfterSaleRefundsRepository : IAfterSaleRefundsRepository
    {
        private readonly IAfterSaleCommandHandler _afterSaleCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public AfterSaleRefundsRepository(IAfterSaleCommandHandler afterSaleCommandHandler, ISharedRepository coreRepository) =>
            (_afterSaleCommandHandler, _coreRepository) = (afterSaleCommandHandler, coreRepository);

        public Task<bool> InsertIntoAfterSaleRefunds()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Status.Status?>> GetStatus(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetStatusRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Status.Status>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleStatusRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Status.Status> data, Guid? parentExecutionGUID)
        {
            var statusRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Status.Status(), tableName: "AfterSaleStatusRefunds");

            _coreRepository.FillSystemDataTable(statusRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(statusRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("general", statusRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Actions.Action?>> GetActions(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetActionsRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Actions.Action>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleActionsRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Actions.Action> data, Guid? parentExecutionGUID)
        {
            var actionsRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Actions.Action(), tableName: "AfterSaleActionsRefunds");

            _coreRepository.FillSystemDataTable(actionsRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(actionsRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("general", actionsRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank?>> GetBanks(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetBanksRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleBanksRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank> data, Guid? parentExecutionGUID)
        {
            var banksRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank(), tableName: "AfterSaleBanksRefunds");

            _coreRepository.FillSystemDataTable(banksRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(banksRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("general", banksRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Types.Type?>> GetTypes(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetTypesRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Types.Type>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleTypesRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Types.Type> data, Guid? parentExecutionGUID)
        {
            var typesRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Types.Type(), tableName: "AfterSaleTypesRefunds");

            _coreRepository.FillSystemDataTable(typesRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(typesRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("general", typesRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund?>> GetSummarizedRefundsByIds(string doc_company, List<string> refundsIds)
        {
            var list = new List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund?>();
            int indice = refundsIds.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = refundsIds.Skip(i * 1000).Take(1000).ToList();
                    string sql = _afterSaleCommandHandler.CreateGetSummarizedRefundsByIdsQuery(doc_company, top1000List);
                    var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>(sql);
                    list.AddRange(result);
                }

                return list;
            }
            else
            {
                string sql = _afterSaleCommandHandler.CreateGetSummarizedRefundsByIdsQuery(doc_company, refundsIds);
                var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>(sql);
                list.AddRange(result);

                return list;
            }
        }

        public async Task<bool> BulkInsertIntoAfterSaleSummarizedRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund> data, Guid? parentExecutionGUID)
        {
            var summarizedRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund(), tableName: "AfterSaleSummarizedRefunds");

            _coreRepository.FillSystemDataTable(summarizedRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(summarizedRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("general", summarizedRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<bool> BulkInsertIntoAfterSaleErrors(List<Domain.AfterSale.Entities.Errors.Error> data, Guid? parentExecutionGUID)
        {
            var errosTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Errors.Error(), tableName: "AfterSaleErrors");

            _coreRepository.FillSystemDataTable(errosTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(errosTable, "untreated");

            await _coreRepository.CallDbProcMerge("general", errosTable.TableName, parentExecutionGUID);

            return true;
        }
    }
}
