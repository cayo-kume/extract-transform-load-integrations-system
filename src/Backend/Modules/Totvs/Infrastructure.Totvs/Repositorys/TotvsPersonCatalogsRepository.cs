using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.People.Catalogs;

namespace Infrastructure.Totvs.Repositorys
{
    /// <summary>
    /// catálogos da API Person (fase PE-1): uma tabela cada, sem filhas
    /// </summary>
    public class TotvsPersonCatalogsRepository : ITotvsPersonCatalogsRepository
    {
        private readonly ITotvsCommandHandler _totvsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsPersonCatalogsRepository(ITotvsCommandHandler totvsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsCommandHandler, _coreRepository) = (totvsCommandHandler, coreRepository);

        public async Task<DateTime?> GetPersonClassificationsLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetPersonClassificationsLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<PersonClassification?>> GetPersonClassifications(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetPersonClassificationsQuery(doc_company);

            return await _coreRepository.GetRecords<PersonClassification>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsPersonClassifications(List<PersonClassification> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new PersonClassification(), tableName: "TotvsPersonClassifications");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetEmailTypesLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetEmailTypesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<EmailType?>> GetEmailTypes(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetEmailTypesQuery(doc_company);

            return await _coreRepository.GetRecords<EmailType>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsEmailTypes(List<EmailType> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new EmailType(), tableName: "TotvsEmailTypes");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetPhoneTypesLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetPhoneTypesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<PhoneType?>> GetPhoneTypes(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetPhoneTypesQuery(doc_company);

            return await _coreRepository.GetRecords<PhoneType>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsPhoneTypes(List<PhoneType> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new PhoneType(), tableName: "TotvsPhoneTypes");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetContactTypesLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetContactTypesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<ContactType?>> GetContactTypes(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetContactTypesQuery(doc_company);

            return await _coreRepository.GetRecords<ContactType>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsContactTypes(List<ContactType> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new ContactType(), tableName: "TotvsContactTypes");

            _coreRepository.FillSystemDataTable(table, data.Cast<object>().ToList());
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
