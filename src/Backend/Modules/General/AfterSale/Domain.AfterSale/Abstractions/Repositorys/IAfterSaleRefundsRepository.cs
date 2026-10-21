using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Abstractions.Repositorys
{
    public interface IAfterSaleRefundsRepository
    {
        public Task<List<Entities.Reads.Refunds.Status.Status?>> GetStatus(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleStatusRefunds(List<Entities.Reads.Refunds.Status.Status> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Refunds.Actions.Action?>> GetActions(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleActionsRefunds(List<Entities.Reads.Refunds.Actions.Action> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Refunds.Banks.Bank?>> GetBanks(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleBanksRefunds(List<Entities.Reads.Refunds.Banks.Bank> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Refunds.Types.Type?>> GetTypes(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleTypesRefunds(List<Entities.Reads.Refunds.Types.Type> data, Guid? parentExecutionGUID);

        public Task<bool> InsertIntoAfterSaleRefunds();

        public Task<List<Entities.Reads.Refunds.SearchRefunds.Refund?>> GetSummarizedRefundsByIds(string doc_company, List<string> refundsIds);
        public Task<bool> BulkInsertIntoAfterSaleSummarizedRefunds(List<Entities.Reads.Refunds.SearchRefunds.Refund> data, Guid? parentExecutionGUID);
        public Task<bool> BulkInsertIntoAfterSaleErrors(List<Entities.Errors.Error> data, Guid? parentExecutionGUID);
    }
}
