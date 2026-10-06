using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Abstractions.Repositorys
{
    public interface IAfterSaleReversesRepository
    {
        public Task<bool> BulkInsertIntoAfterSaleErrors(List<Entities.Errors.Error> data, Guid? parentExecutionGUID);

        public Task<IEnumerable<Domain.AfterSale.Entities.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesIdsForGetReverse();
        public Task<List<Entities.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesByIds(List<string> reversesIds);
        public Task<bool> BulkInsertIntoAfterSaleSummarizedReverses(List<Entities.Reverses.SearchReverses.Reverse> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reverses.GetReverse.Reverse?>> GetReversesByIds(List<string> reversesIds);
        public Task<bool> BulkInsertIntoAfterSaleReverses(List<Entities.Reverses.GetReverse.Reverse> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reverses.Status.Status?>> GetStatus(/*string doc_company*/);
        public Task<bool> BulkInsertIntoAfterSaleStatusReverses(List<Entities.Reverses.Status.Status> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reverses.Transportations.Transportation?>> GetTransportations();
        public Task<bool> BulkInsertIntoAfterSaleTransportationsReverses(List<Entities.Reverses.Transportations.Transportation> data, Guid? parentExecutionGUID);

        public Task<IEnumerable<Int64>> GetSummarizedReversesIdsForGetCourierAttributes();
        public Task<List<Entities.Reverses.CourierAttributes.CourierAttributes?>> GetCourierAttributesByReverseId(List<Int64> reversesIds);
        //public Task<bool> InsertIntoAfterSaleReversesCourierAttributes();
    }
}
