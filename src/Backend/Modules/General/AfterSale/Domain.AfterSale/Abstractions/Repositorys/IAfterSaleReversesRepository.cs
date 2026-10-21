using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Abstractions.Repositorys
{
    public interface IAfterSaleReversesRepository
    {
        public Task<bool> BulkInsertIntoAfterSaleErrors(List<Entities.Errors.Error> data, Guid? parentExecutionGUID);

        public Task<IEnumerable<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesIdsForGetReverse();
        public Task<List<Entities.Reads.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesByIds(string doc_company, List<string> reversesIds);
        public Task<bool> BulkInsertIntoAfterSaleSummarizedReverses(List<Entities.Reads.Reverses.SearchReverses.Reverse> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Reverses.GetReverse.Reverse?>> GetReversesByIds(string doc_company, List<string> reversesIds);
        public Task<bool> BulkInsertIntoAfterSaleReverses(List<Entities.Reads.Reverses.GetReverse.Reverse> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Reverses.Status.Status?>> GetStatus(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleStatusReverses(List<Entities.Reads.Reverses.Status.Status> data, Guid? parentExecutionGUID);

        public Task<List<Entities.Reads.Reverses.Transportations.Transportation?>> GetTransportations(string doc_company);
        public Task<bool> BulkInsertIntoAfterSaleTransportationsReverses(List<Entities.Reads.Reverses.Transportations.Transportation> data, Guid? parentExecutionGUID);

        public Task<IEnumerable<Entities.Reads.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesIdsForGetCourierAttributes();
        public Task<List<Entities.Reads.Reverses.CourierAttributes.CourierAttributes?>> GetCourierAttributesByReverseId(string doc_company, List<Int64> reversesIds);
        public Task<bool> BulkInsertIntoAfterSaleCourierAttributes(List<Entities.Reads.Reverses.CourierAttributes.CourierAttributes> data, Guid? parentExecutionGUID);
    }
}
