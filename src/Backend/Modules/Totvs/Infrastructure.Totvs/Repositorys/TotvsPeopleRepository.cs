using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.People.Branches;
using Domain.Totvs.Entities.Reads.People.Customers;
using Domain.Totvs.Entities.Reads.People.LegalEntities;
using System.Collections;
using Persons = Domain.Totvs.Entities.Reads.People.Persons;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    public class TotvsPeopleRepository : ITotvsPeopleRepository
    {
        private readonly ITotvsCommandHandler _totvsCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsPeopleRepository(ITotvsCommandHandler totvsCommandHandler, ISharedRepository coreRepository) =>
            (_totvsCommandHandler, _coreRepository) = (totvsCommandHandler, coreRepository);

        public async Task<DateTime?> GetBranchesLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetBranchesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Branch?>> GetBranchesByCodes(string doc_company, List<int> codes)
        {
            var result = new List<Branch?>();

            // lotes de 1000 códigos, como no GetPeopleByIds do Movidesk
            foreach (var chunk in codes.Distinct().Chunk(1000))
            {
                string sql = _totvsCommandHandler.CreateGetBranchesByCodesQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Branch>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsBranches(List<Branch> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: filial antes dos endereços (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Branch(), tableName: "TotvsBranches"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Address(), tableName: "TotvsBranchesAddresses"), data.SelectMany(x => x.addresses).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge dos endereços roda mesmo sem linhas no lote: é ele que apaga os endereços removidos
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetCustomersLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetCustomersLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Customer?>> GetCustomersByCodes(string doc_company, List<int> codes)
        {
            var result = new List<Customer?>();

            // lotes de 1000 códigos, como no GetPeopleByIds do Movidesk
            foreach (var chunk in codes.Distinct().Chunk(1000))
            {
                string sql = _totvsCommandHandler.CreateGetCustomersByCodesQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Customer>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsPeople(List<Customer> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: pessoa antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Customer(), tableName: "TotvsPeople"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Address(), tableName: "TotvsPeopleAddresses"), data.SelectMany(x => x.addresses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Phone(), tableName: "TotvsPeoplePhones"), data.SelectMany(x => x.phones).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Email(), tableName: "TotvsPeopleEmails"), data.SelectMany(x => x.emails).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Observation(), tableName: "TotvsPeopleObservations"), data.SelectMany(x => x.observations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.CustomerObservation(), tableName: "TotvsPeopleCustomerObservations"), data.SelectMany(x => x.customerObservations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.AdditionalField(), tableName: "TotvsPeopleAdditionalFields"), data.SelectMany(x => x.additionalFields).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Classification(), tableName: "TotvsPeopleClassifications"), data.SelectMany(x => x.classifications).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Reference(), tableName: "TotvsPeopleReferences"), data.SelectMany(x => x.references).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Related(), tableName: "TotvsPeopleRelateds"), data.SelectMany(x => x.relateds).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Statistic(), tableName: "TotvsPeopleStatistics"), data.SelectMany(x => x.statistics).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Representative(), tableName: "TotvsPeopleRepresentatives"), data.SelectMany(x => x.representatives).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Familiar(), tableName: "TotvsPeopleFamiliars"), data.SelectMany(x => x.familiars).ToList()),
            };

            await BulkInsertAndMerge(tables, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetLegalEntitiesLastChange(string doc_company)
        {
            string sql = _totvsCommandHandler.CreateGetLegalEntitiesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<LegalEntity?>> GetLegalEntitiesByCodes(string doc_company, List<int> codes)
        {
            var result = new List<LegalEntity?>();

            // lotes de 1000 códigos, como no GetPeopleByIds do Movidesk
            foreach (var chunk in codes.Distinct().Chunk(1000))
            {
                string sql = _totvsCommandHandler.CreateGetLegalEntitiesByCodesQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<LegalEntity>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsLegalEntities(List<LegalEntity> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: pessoa antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new LegalEntity(), tableName: "TotvsLegalEntities"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Address(), tableName: "TotvsLegalEntitiesAddresses"), data.SelectMany(x => x.addresses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Phone(), tableName: "TotvsLegalEntitiesPhones"), data.SelectMany(x => x.phones).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Email(), tableName: "TotvsLegalEntitiesEmails"), data.SelectMany(x => x.emails).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Observation(), tableName: "TotvsLegalEntitiesObservations"), data.SelectMany(x => x.observations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.CustomerObservation(), tableName: "TotvsLegalEntitiesCustomerObservations"), data.SelectMany(x => x.customerObservations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.AdditionalField(), tableName: "TotvsLegalEntitiesAdditionalFields"), data.SelectMany(x => x.additionalFields).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Classification(), tableName: "TotvsLegalEntitiesClassifications"), data.SelectMany(x => x.classifications).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Reference(), tableName: "TotvsLegalEntitiesReferences"), data.SelectMany(x => x.references).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Related(), tableName: "TotvsLegalEntitiesRelateds"), data.SelectMany(x => x.relateds).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Partner(), tableName: "TotvsLegalEntitiesPartners"), data.SelectMany(x => x.partners).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Contact(), tableName: "TotvsLegalEntitiesContacts"), data.SelectMany(x => x.contacts).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Statistic(), tableName: "TotvsLegalEntitiesStatistics"), data.SelectMany(x => x.statistics).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.SocialNetwork(), tableName: "TotvsLegalEntitiesSocialNetworks"), data.SelectMany(x => x.socialNetworks).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.Representative(), tableName: "TotvsLegalEntitiesRepresentatives"), data.SelectMany(x => x.representatives).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Persons.PaymentMethod(), tableName: "TotvsLegalEntitiesPaymentMethods"), data.SelectMany(x => x.paymentMethods).ToList()),
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

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram da pessoa
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);
        }
    }
}
