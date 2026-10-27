using Domain.Totvs.Entities.Reads.People.Catalogs;

namespace Domain.Totvs.Abstractions.Repositorys
{
    public interface ITotvsPersonCatalogsRepository
    {
        public Task<DateTime?> GetPersonClassificationsLastChange(string doc_company);
        public Task<IEnumerable<PersonClassification?>> GetPersonClassifications(string doc_company);
        public Task<bool> BulkInsertIntoTotvsPersonClassifications(List<PersonClassification> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetEmailTypesLastChange(string doc_company);
        public Task<IEnumerable<EmailType?>> GetEmailTypes(string doc_company);
        public Task<bool> BulkInsertIntoTotvsEmailTypes(List<EmailType> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetPhoneTypesLastChange(string doc_company);
        public Task<IEnumerable<PhoneType?>> GetPhoneTypes(string doc_company);
        public Task<bool> BulkInsertIntoTotvsPhoneTypes(List<PhoneType> data, Guid? parentExecutionGUID);

        public Task<DateTime?> GetContactTypesLastChange(string doc_company);
        public Task<IEnumerable<ContactType?>> GetContactTypes(string doc_company);
        public Task<bool> BulkInsertIntoTotvsContactTypes(List<ContactType> data, Guid? parentExecutionGUID);
    }
}
