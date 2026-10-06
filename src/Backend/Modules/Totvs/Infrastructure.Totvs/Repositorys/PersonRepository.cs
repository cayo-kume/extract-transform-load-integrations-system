using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Person.Customer;
using Domain.Totvs.Entities.Person.LegalEntity;

namespace Infrastructure.Totvs.Repositorys
{
    public class PersonRepository : IPersonRepository
    {
        private readonly IPersonCommandHandler _commandHandler;
        private readonly IIntegrationsSharedRepository _coreRepository;

        public PersonRepository(IIntegrationsSharedRepository coreRepository, IPersonCommandHandler commandHandler) =>
            (_coreRepository, _commandHandler) = (coreRepository, commandHandler);

        public async Task<bool> BulkInsertIntoTableCustomersRaw(List<Domain.Totvs.Entities.Person.Customer.Customer> registros, Guid? parentExecutionGUID)
        {
            var customersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Customer(), tableName: "People");
            var customersRepresentativesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Representative(), tableName: "PeopleRepresentatives");
            var customersClassificationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Classification(), tableName: "PeopleClassifications");
            var customersEmailsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Email(), tableName: "PeopleEmails");
            var customersPhonesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Phone(), tableName: "PeoplePhones");
            var customersAdditionalFieldsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.AdditionalField(), tableName: "PeopleAdditionalFields");
            var customersStatisticsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Statistic(), tableName: "PeopleStatistics");
            var customersFamiliarsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Familiar(), tableName: "PeopleFamiliars");
            var customersCustomerObservationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.CustomerObservation(), tableName: "PeopleCustomerObservations");
            var customersReferencesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Reference(), tableName: "PeopleReferences");
            var customersObservationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Observation(), tableName: "PeopleObservations");
            var customersAddressesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Address(), tableName: "PeopleAddresses");
            var customersRelatedsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Customer.Related(), tableName: "PeopleRelateds");

            _coreRepository.FillSystemDataTable(customersTable, registros.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(customersRepresentativesTable, registros.Select(y => y.representatives).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersClassificationsTable, registros.Select(y => y.classifications).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersEmailsTable, registros.Select(y => y.emails).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersPhonesTable, registros.Select(y => y.phones).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersAdditionalFieldsTable, registros.Select(y => y.additionalFields).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersStatisticsTable, registros.Select(y => y.statistics).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersFamiliarsTable, registros.Select(y => y.familiars).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersCustomerObservationsTable, registros.Select(y => y.customerObservations).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersReferencesTable, registros.Select(y => y.references).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersObservationsTable, registros.Select(y => y.observations).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersAddressesTable, registros.Select(y => y.addresses).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(customersRelatedsTable, registros.Select(y => y.relateds).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());

            _coreRepository.BulkInsertIntoTableRaw(customersTable);
            _coreRepository.BulkInsertIntoTableRaw(customersRepresentativesTable);
            _coreRepository.BulkInsertIntoTableRaw(customersClassificationsTable);
            _coreRepository.BulkInsertIntoTableRaw(customersEmailsTable);
            _coreRepository.BulkInsertIntoTableRaw(customersPhonesTable);
            _coreRepository.BulkInsertIntoTableRaw(customersAdditionalFieldsTable);
            _coreRepository.BulkInsertIntoTableRaw(customersStatisticsTable);
            _coreRepository.BulkInsertIntoTableRaw(customersFamiliarsTable);
            _coreRepository.BulkInsertIntoTableRaw(customersCustomerObservationsTable);
            _coreRepository.BulkInsertIntoTableRaw(customersReferencesTable);
            _coreRepository.BulkInsertIntoTableRaw(customersObservationsTable);
            _coreRepository.BulkInsertIntoTableRaw(customersAddressesTable);
            _coreRepository.BulkInsertIntoTableRaw(customersRelatedsTable);

            await _coreRepository.CallDbProcMerge("totvs", customersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersRepresentativesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersClassificationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersEmailsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersPhonesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersAdditionalFieldsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersStatisticsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersFamiliarsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersCustomerObservationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersReferencesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersObservationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersAddressesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", customersRelatedsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<bool> BulkInsertIntoTableLegalEntitiesRaw(List<Domain.Totvs.Entities.Person.LegalEntity.LegalEntity> registros, Guid? parentExecutionGUID)
        {
            var legalEntityTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.LegalEntity(), tableName: "LegalEntities");
            var legalEntitiesAdditionalFieldsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.AdditionalField(), tableName: "LegalEntitiesAdditionalFields");
            var legalEntitiesAddressesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Address(), tableName: "LegalEntitiesAddresses");
            var legalEntitiesClassificationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Classification(), tableName: "LegalEntitiesClassifications");
            var legalEntitiesContactsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Contact(), tableName: "LegalEntitiesContacts");
            var legalEntitiesCustomerObservationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.CustomerObservation(), tableName: "LegalEntitiesCustomerObservations");
            var legalEntitiesEmailsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Email(), tableName: "LegalEntitiesEmails");
            var legalEntitiesObservationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Observation(), tableName: "LegalEntitiesObservations");
            var legalEntitiesPartnersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Partner(), tableName: "LegalEntitiesPartners");
            var legalEntitiesPaymentMethodsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.PaymentMethod(), tableName: "LegalEntitiesPaymentMethods");
            var legalEntitiesPhonesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Phone(), tableName: "LegalEntitiesPhones");
            var legalEntitiesReferencesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Reference(), tableName: "LegalEntitiesReferences");
            var legalEntitiesRelatedsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Related(), tableName: "LegalEntitiesRelateds");
            var legalEntitiesRepresentativesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Representative(), tableName: "LegalEntitiesRepresentatives");
            var legalEntitiesSocialNetworksTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.SocialNetwork(), tableName: "LegalEntitiesSocialNetworks");
            var legalEntitiesStatisticsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.LegalEntity.Statistic(), tableName: "LegalEntitiesStatistics");

            _coreRepository.FillSystemDataTable(legalEntityTable, registros.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesAdditionalFieldsTable, registros.Select(y => y.additionalFields).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesAddressesTable, registros.Select(y => y.addresses).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesClassificationsTable, registros.Select(y => y.classifications).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesContactsTable, registros.Select(y => y.contacts).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesCustomerObservationsTable, registros.Select(y => y.customerObservations).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesEmailsTable, registros.Select(y => y.emails).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesObservationsTable, registros.Select(y => y.observations).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesPartnersTable, registros.Select(y => y.partners).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesPaymentMethodsTable, registros.Select(y => y.paymentMethods).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesPhonesTable, registros.Select(y => y.phones).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesReferencesTable, registros.Select(y => y.references).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesRelatedsTable, registros.Select(y => y.relateds).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesRepresentativesTable, registros.Select(y => y.representatives).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesSocialNetworksTable, registros.Select(y => y.socialNetworks).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());
            _coreRepository.FillSystemDataTable(legalEntitiesStatisticsTable, registros.Select(y => y.statistics).Where(x => x.Count > 0).Select(z => z.FirstOrDefault()).ToList());

            _coreRepository.BulkInsertIntoTableRaw(legalEntityTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesAdditionalFieldsTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesAddressesTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesClassificationsTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesContactsTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesCustomerObservationsTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesEmailsTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesObservationsTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesPartnersTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesPaymentMethodsTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesPhonesTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesReferencesTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesRelatedsTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesRepresentativesTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesSocialNetworksTable);
            _coreRepository.BulkInsertIntoTableRaw(legalEntitiesStatisticsTable);

            await _coreRepository.CallDbProcMerge("totvs", legalEntityTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesAdditionalFieldsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesAddressesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesClassificationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesContactsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesCustomerObservationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesEmailsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesObservationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesPartnersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesPaymentMethodsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesPhonesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesReferencesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesRelatedsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesRepresentativesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", legalEntitiesSocialNetworksTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.Totvs.Dtos.Person.Customer.Item?>> GetExistingCustomers(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.Person.Customer.Item>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingCustomersQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.Person.Customer.Item>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }

        public async Task<List<Domain.Totvs.Dtos.Person.LegalEntity.Item?>> GetExistingLegalEntities(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.Person.LegalEntity.Item>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingLegalEntitiesQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.Person.LegalEntity.Item>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }

        public async Task<bool> BulkInsertIntoTableBranchesRaw(List<Domain.Totvs.Entities.Person.Branch.Branch> registros, Guid? parentExecutionGUID)
        {
            var branchesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Branch.Branch(), tableName: "Branches");
            var branchesAddressesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Person.Branch.Address(), tableName: "BranchesAddresses");

            _coreRepository.FillSystemDataTable(branchesTable, registros);
            _coreRepository.FillSystemDataTable(branchesAddressesTable, registros.SelectMany(b => b.addresses).ToList());

            _coreRepository.BulkInsertIntoTableRaw(branchesTable);
            _coreRepository.BulkInsertIntoTableRaw(branchesAddressesTable);

            await _coreRepository.CallDbProcMerge("totvs", branchesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("totvs", branchesAddressesTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.Totvs.Dtos.Person.Branch.Branch?>> GetExistingBranches(List<string?> registros)
        {
            const int batchSize = 1000;
            var resultList = new List<Domain.Totvs.Dtos.Person.Branch.Branch>();

            for (int i = 0; i < registros.Count; i += batchSize)
            {
                var batch = registros.Skip(i).Take(batchSize).ToList();

                if (batch.Any())
                {
                    string sql = _commandHandler.CreateGetExistingBranchesQuery(batch);
                    var result = await _coreRepository.GetRecords<Domain.Totvs.Dtos.Person.Branch.Branch>(sql);
                    resultList.AddRange(result);
                }
            }

            return resultList;
        }
    }
}
