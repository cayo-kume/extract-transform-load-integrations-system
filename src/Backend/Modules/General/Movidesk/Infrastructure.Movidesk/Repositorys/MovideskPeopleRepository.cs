using Application.Movidesk.Abstractions.Handlers.Commands;
using Domain.Movidesk.Abstractions.Repositorys;
using Domain.Movidesk.Entities.Reads.People;
using Domain.Shared.Abstractions.Repositorys;
using System.Collections;
using System.Data;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskPeopleRepository : IMovideskPeopleRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public MovideskPeopleRepository(IMovideskCommandHandler movideskCommandHandler, ISharedRepository coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);

        public async Task<DateTime?> GetPeopleLastChange(string doc_company)
        {
            string sql = _movideskCommandHandler.CreateGetPeopleLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Person?>> GetPeopleByIds(string doc_company, List<string> ids)
        {
            var result = new List<Person?>();

            // lotes de 1000 ids, como no GetActionsHtmlDescriptions dos tickets
            foreach (var chunk in ids.Distinct().Chunk(1000))
            {
                string sql = _movideskCommandHandler.CreateGetPeopleByIdsQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Person>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoMovideskPeople(List<Person> data, Guid? parentExecutionGUID)
        {
            var relationships = data.SelectMany(x => x.relationships).ToList();
            var customFieldValues = data.SelectMany(x => x.customFieldValues).ToList();

            // ordem = ordem dos merges: pessoa, nível do meio, depois filhas e netas (FKs e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Person(), tableName: "MovideskPeople"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Relationship(), tableName: "MovideskPeopleRelationships"), relationships),
                (_coreRepository.CreateSystemDataTable(entity: new CustomFieldValue(), tableName: "MovideskPeopleCustomFieldValues"), customFieldValues),
                (_coreRepository.CreateSystemDataTable(entity: new Address(), tableName: "MovideskPeopleAddresses"), data.SelectMany(x => x.addresses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Contact(), tableName: "MovideskPeopleContacts"), data.SelectMany(x => x.contacts).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Email(), tableName: "MovideskPeopleEmails"), data.SelectMany(x => x.emails).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Team(), tableName: "MovideskPeopleTeams"), data.SelectMany(x => x.teams).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Service(), tableName: "MovideskPeopleRelationshipsServices"), relationships.SelectMany(x => x.services).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new CustomFieldValueItem(), tableName: "MovideskPeopleCustomFieldValuesItems"), customFieldValues.SelectMany(x => x.items).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Asset(), tableName: "MovideskPeopleAssets"), data.SelectMany(x => x.atAssets).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge roda mesmo para tabelas sem linhas no lote: é ele que apaga as filhas removidas
            // (ex.: a pessoa ficou sem equipes -> nenhuma linha em untreated.MovideskPeopleTeams, mas as equipes antigas saem)
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("general", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
