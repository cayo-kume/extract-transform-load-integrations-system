using Application.Totvs.Abstractions.Handlers.Commands;
using Domain.Shared.Abstractions.Repositorys;
using Domain.Totvs.Abstractions.Repositorys;
using Domain.Totvs.Entities.Reads.People.ReceptionSales;
using Domain.Totvs.Entities.Reads.People.Representatives;
using System.Collections;
using System.Data;

namespace Infrastructure.Totvs.Repositorys
{
    /// <summary>
    /// representantes e recepções de venda (fase PE-2, src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.1)
    /// </summary>
    public class TotvsRepresentativesRepository : ITotvsRepresentativesRepository
    {
        private readonly ITotvsRepresentativesCommandHandler _totvsRepresentativesCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public TotvsRepresentativesRepository(ITotvsRepresentativesCommandHandler totvsRepresentativesCommandHandler, ISharedRepository coreRepository) =>
            (_totvsRepresentativesCommandHandler, _coreRepository) = (totvsRepresentativesCommandHandler, coreRepository);

        public async Task<DateTime?> GetRepresentativesLastChange(string doc_company)
        {
            string sql = _totvsRepresentativesCommandHandler.CreateGetRepresentativesLastChangeQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<Representative?>> GetRepresentativesByCodes(string doc_company, List<int> codes)
        {
            var result = new List<Representative?>();

            // lotes de 1000 códigos, como no GetPeopleByIds do Movidesk
            foreach (var chunk in codes.Distinct().Chunk(1000))
            {
                string sql = _totvsRepresentativesCommandHandler.CreateGetRepresentativesByCodesQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Representative>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoTotvsRepresentatives(List<Representative> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: representante e depois as filhas (FKs e condição de exclusão do merge). As filhas comuns com a
            // PF usam as entidades de People/Persons em tabelas próprias
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Representative(), tableName: "TotvsRepresentatives"), data),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.People.Persons.Address(), tableName: "TotvsRepresentativesAddresses"), data.SelectMany(x => x.addresses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.People.Persons.Phone(), tableName: "TotvsRepresentativesPhones"), data.SelectMany(x => x.phones).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.People.Persons.Email(), tableName: "TotvsRepresentativesEmails"), data.SelectMany(x => x.emails).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.People.Persons.Observation(), tableName: "TotvsRepresentativesObservations"), data.SelectMany(x => x.observations).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.People.Persons.AdditionalField(), tableName: "TotvsRepresentativesAdditionalFields"), data.SelectMany(x => x.additionalFields).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new Domain.Totvs.Entities.Reads.People.Persons.Classification(), tableName: "TotvsRepresentativesClassifications"), data.SelectMany(x => x.classifications).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new RepresentativeCustomer(), tableName: "TotvsRepresentativesCustomers"), data.SelectMany(x => x.customers).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new RepresentativePriceTable(), tableName: "TotvsRepresentativesPriceTables"), data.SelectMany(x => x.priceTables).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new RepresentativeSalesRegion(), tableName: "TotvsRepresentativesSalesRegions"), data.SelectMany(x => x.salesRegions).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new RepresentativeParameter(), tableName: "TotvsRepresentativesParameters"), data.SelectMany(x => x.representativeParameters).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do representante
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<DateTime?> GetReceptionSalesLastMovementDate(string doc_company)
        {
            string sql = _totvsRepresentativesCommandHandler.CreateGetReceptionSalesLastMovementDateQuery(doc_company);

            return await _coreRepository.GetRecord<DateTime?>(sql);
        }

        public async Task<IEnumerable<ReceptionSale?>> GetReceptionSalesHashes(string doc_company, DateTime startMovementDate, DateTime endMovementDate)
        {
            string sql = _totvsRepresentativesCommandHandler.CreateGetReceptionSalesHashesQuery(doc_company, startMovementDate, endMovementDate);

            return await _coreRepository.GetRecords<ReceptionSale>(sql);
        }

        public async Task<bool> BulkInsertIntoTotvsReceptionSales(List<ReceptionSale> data, Guid? parentExecutionGUID)
        {
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new ReceptionSale(), tableName: "TotvsReceptionSales"), data),
                (_coreRepository.CreateSystemDataTable(entity: new ReceptionSaleDocumentType(), tableName: "TotvsReceptionSalesDocumentTypes"), data.SelectMany(x => x.documentTypes).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("totvs", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
