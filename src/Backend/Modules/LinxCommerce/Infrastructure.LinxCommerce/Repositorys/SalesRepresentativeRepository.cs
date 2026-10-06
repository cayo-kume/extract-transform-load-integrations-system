using Dapper;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Entities.Exceptions;
using Domain.LinxCommerce.Entities.Parameters;
using Domain.LinxCommerce.Entities.SalesRepresentative;
using Domain.LinxCommerce.Interfaces.Repositorys;
using System.Data.SqlClient;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class SalesRepresentativeRepository : ISalesRepresentativeRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;

        public SalesRepresentativeRepository(IIntegrationsCoreRepository coreRepository) =>
            (_coreRepository) = (coreRepository);

        public bool BulkInsertIntoTableRaw(LinxCommerceJobParameter jobParameter, List<SalesRepresentative> records, Guid? parentExecutionGUID)
        {
            try
            {
                var salesRepresentativeTable = _coreRepository.CreateSystemDataTable("SalesRepresentative", new SalesRepresentative());
                var salesRepresentativeContactTable = _coreRepository.CreateSystemDataTable("SalesRepresentativeContactData", new SalesRepresentativeContactData());
                var salesRepresentativeAddressesTable = _coreRepository.CreateSystemDataTable("SalesRepresentativeAddress", new SalesRepresentativeAddress());
                var salesRepresentativeIdentificationTable = _coreRepository.CreateSystemDataTable("SalesRepresentativeIdentification", new SalesRepresentativeIdentification());
                var salesRepresentativeComissionTable = _coreRepository.CreateSystemDataTable("SalesRepresentativeComission", new SalesRepresentativeComission());
                var salesRepresentativeMaxDiscountTable = _coreRepository.CreateSystemDataTable("SalesRepresentativeMaxDiscount", new SalesRepresentativeMaxDiscount());
                var salesRepresentativeWebSiteSettingsTable = _coreRepository.CreateSystemDataTable(tableName: "SalesRepresentativeWebSiteSettings", entity: new SalesRepresentativeWebSiteSettings(), columnNames: ["WebSiteGroups", "WebSites"], columnTypes: [typeof(string), typeof(string)]);
                var salesRepresentativePortfolioTable = _coreRepository.CreateSystemDataTable("SalesRepresentativePortfolio", new SalesRepresentativePortfolio());
                var salesRepresentativeCustomerRelationTable = _coreRepository.CreateSystemDataTable("SalesRepresentativeCustomerRelation", new SalesRepresentativeCustomerRelation());
                var salesRepresentativeShippingRegionTable = _coreRepository.CreateSystemDataTable(tableName: "SalesRepresentativeShippingRegion", entity: new SalesRepresentativeShippingRegion(), columnNames: ["PointOfSalesList"], columnTypes: [typeof(string)]);

                for (int i = 0; i < records.Count(); i++)
                {
                    salesRepresentativeTable.Rows.Add(records[i].SalesRepresentativeID, records[i].Name, records[i].FriendlyCode, records[i].ImageUrl, records[i].IntegrationID,
                        records[i].AllowQuoteDeletion, records[i].BusinessContractID, records[i].SalesRepresentativeType, records[i].Status);

                    if (records[i].Addresses.Count() > 0)
                    {
                        for (int j = 0; j < records[i].Addresses.Count(); j++)
                        {
                            salesRepresentativeAddressesTable.Rows.Add(records[i].SalesRepresentativeID, records[i].Addresses[j].IsMainAddress, records[i].Addresses[j].Name,
                                records[i].Addresses[j].AddressLine, records[i].Addresses[j].City, records[i].Addresses[j].Neighbourhood, records[i].Addresses[j].Number,
                                records[i].Addresses[j].State, records[i].Addresses[j].AddressNotes, records[i].Addresses[j].Landmark, records[i].Addresses[j].ContactName,
                                records[i].Addresses[j].Latitude, records[i].Addresses[j].Longitude, records[i].Addresses[j].PostalCode);
                        }
                    }

                    if (records[i].ShippingRegion != null)
                    {
                        salesRepresentativeShippingRegionTable.Rows.Add(records[i].SalesRepresentativeID, records[i].ShippingRegion.SelectedMode, records[i].ShippingRegion.ShippingRegionID, records[i].ShippingRegion.PointOfSales);
                    }

                    if (records[i].Contact != null)
                    {
                        salesRepresentativeContactTable.Rows.Add(records[i].SalesRepresentativeID, records[i].Contact.Email, records[i].Contact.Phone, records[i].Contact.CellPhone);
                    }

                    if (records[i].Identification != null)
                    {
                        salesRepresentativeIdentificationTable.Rows.Add(records[i].SalesRepresentativeID, records[i].Identification.Type, records[i].Identification.DocumentNumber);
                    }

                    if (records[i].Commission != null)
                    {
                        salesRepresentativeComissionTable.Rows.Add(records[i].SalesRepresentativeID, records[i].Commission.TotalCommission, records[i].Commission.DeliveryCommission);
                    }

                    if (records[i].WebSiteSettings != null)
                    {
                        salesRepresentativeWebSiteSettingsTable.Rows.Add(records[i].SalesRepresentativeID, records[i].WebSiteSettings.WebSiteFilter, records[i].WebSiteSettings.WebSiteGroup, records[i].WebSiteSettings.WebSite);
                    }

                    if (records[i].Portfolio != null)
                    {
                        salesRepresentativePortfolioTable.Rows.Add(records[i].SalesRepresentativeID, records[i].Portfolio.HasPortfolio, records[i].Portfolio.PortfolioAssociationType);

                        if (records[i].Portfolio.Customers.Count() > 0)
                        {
                            for (int h = 0; h < records[i].Portfolio.Customers.Count(); h++)
                            {
                                salesRepresentativeCustomerRelationTable.Rows.Add(records[i].Portfolio.Customers[h].CustomerID, records[i].Portfolio.Customers[h].Status, records[i].Portfolio.Customers[h].IsMaxDiscountEnabled,
                                    records[i].SalesRepresentativeID);
                            }
                        }
                    }

                    if (records[i].MaxDiscount != null)
                    {
                        salesRepresentativeMaxDiscountTable.Rows.Add(records[i].MaxDiscount.Type, records[i].MaxDiscount.Amount, records[i].SalesRepresentativeID);
                    }
                }

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentative", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeAddressesTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativeAddress", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeShippingRegionTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativeShippingRegion", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeContactTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativeContactData", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeIdentificationTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativeIdentification", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeComissionTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativeComission", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativePortfolioTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativePortfolio", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeCustomerRelationTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativeCustomerRelation", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeWebSiteSettingsTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativeWebSiteSettings", parentExecutionGUID);

                _coreRepository.BulkInsertIntoTableRaw(dataTable: salesRepresentativeMaxDiscountTable);
                _coreRepository.CallDbProcMerge(jobParameter.schema, "SalesRepresentativeMaxDiscount", parentExecutionGUID);

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<SalesRepresentative>> GetRegistersExists(IEnumerable<int> ordersIds)
        {
            string? identifiers = string.Empty;

            for (int i = 0; i < ordersIds.Count(); i++)
            {

                if (i == ordersIds.Count() - 1)
                    identifiers += $"{ordersIds.ElementAt(i)}";
                else
                    identifiers += $"{ordersIds.ElementAt(i)}, ";
            }

            string? sql = $@"SELECT DISTINCT 
                             A.SalesRepresentativeID,
                             A.[Name],
                             A.FriendlyCode,
				             A.ImageUrl,
                             A.IntegrationID,
				             A.AllowQuoteDeletion,
				             A.BusinessContractID,
                             A.SalesRepresentativeType,
                             A.[Status],
                             
                             B.[Type],
                             B.DocumentNumber,
                             
                             C.TotalCommission,
                             C.DeliveryCommission,
                             
                             D.Email,
                             D.Phone,
                             D.CellPhone,
                             
                             E.SelectedMode,
                             E.ShippingRegionID,
                             E.PointOfSalesList As PointOfSales,
                             
                             G.[Type],
                             G.Amount,
                             
                             I.WebSiteFilter,
				             I.WebSiteGroups As WebSiteGroup,
                             I.WebSites As WebSite
                             
                             FROM [linx_commerce].[SalesRepresentative] A (NOLOCK)
                             LEFT JOIN [linx_commerce].[SalesRepresentativeIdentification] B (NOLOCK) ON A.SalesRepresentativeID = B.SalesRepresentativeID
                             LEFT JOIN [linx_commerce].[SalesRepresentativeComission] C (NOLOCK) ON A.SalesRepresentativeID = C.SalesRepresentativeID
				             LEFT JOIN [linx_commerce].[SalesRepresentativeContactData] D (NOLOCK) ON A.SalesRepresentativeID = D.SalesRepresentativeID
				             LEFT JOIN [linx_commerce].[SalesRepresentativeShippingRegion] E (NOLOCK) ON A.SalesRepresentativeID = E.SalesRepresentativeID
				             LEFT JOIN [linx_commerce].[SalesRepresentativeMaxDiscount] G (NOLOCK) ON A.SalesRepresentativeID = G.SalesRepresentativeID
				             LEFT JOIN [linx_commerce].[SalesRepresentativeWebSiteSettings] I (NOLOCK) ON A.SalesRepresentativeID = I.SalesRepresentativeID
                             
                             WHERE
                             A.SalesRepresentativeID IN ({identifiers})";

            string? _sql = $@"SELECT DISTINCT
                              A.SalesRepresentativeID,

                              B.HasPortfolio,
                              B.SalesRepresentativeID,
                              B.PortfolioAssociationType,
        
                              TRIM(C.CustomerID) As CustomerID,
                              C.SalesRepresentativeID,
                              TRIM(C.Status) As Status,
                              TRIM(C.IsMaxDiscountEnabled) As IsMaxDiscountEnabled

                              FROM [linx_commerce].[SalesRepresentative] A (NOLOCK)
                              LEFT JOIN [linx_commerce].[SalesRepresentativePortfolio] B (NOLOCK) ON A.SalesRepresentativeID = B.SalesRepresentativeID
                              LEFT JOIN [linx_commerce].[SalesRepresentativeCustomerRelation] C (NOLOCK) ON A.SalesRepresentativeID = C.SalesRepresentativeID

                              WHERE
                              A.SalesRepresentativeID IN ({identifiers})";

            string? __sql = $@"SELECT DISTINCT
                              D.IsMainAddress,
                              D.Name,
                              D.AddressLine,
                              D.City,
                              D.Neighbourhood,
                              D.Number,
                              D.State,
                              D.AddressNotes,
                              D.Landmark,
                              D.ContactName,
                              D.SalesRepresentativeID,
                              D.Latitude,
                              D.Longitude,
                              TRIM(D.PostalCode) As PostalCode

                              FROM [linx_commerce].[SalesRepresentativeAddress] D (NOLOCK)

                              WHERE
                              D.SalesRepresentativeID IN ({identifiers})";

            try
            {
                using (var conn = _coreRepository.GetIDbConnection())
                {
                    var result = await conn.QueryAsync<
                        SalesRepresentative,
                        SalesRepresentativeIdentification,
                        SalesRepresentativeComission,
                        SalesRepresentativeContactData,
                        SalesRepresentativeShippingRegion,
                        SalesRepresentativeMaxDiscount,
                        SalesRepresentativeWebSiteSettings,
                        SalesRepresentative
                    >(sql, (salesRepresentative, salesRepresentativeIdentification, salesRepresentativeComission, salesRepresentativeContactData, salesRepresentativeShippingRegion, salesRepresentativeMaxDiscount, salesRepresentativeWebSiteSettings) =>
                    {
                        salesRepresentative.Identification = salesRepresentativeIdentification;
                        salesRepresentative.Commission = salesRepresentativeComission;
                        salesRepresentative.Contact = salesRepresentativeContactData;
                        salesRepresentative.ShippingRegion = salesRepresentativeShippingRegion;
                        salesRepresentative.MaxDiscount = salesRepresentativeMaxDiscount;
                        salesRepresentative.WebSiteSettings = salesRepresentativeWebSiteSettings;

                        return salesRepresentative;
                    }, splitOn: "Type, TotalCommission, Email, SelectedMode, Type, WebSiteFilter", commandTimeout: 360);

                    var _result = await conn.QueryAsync<
                        SalesRepresentative,
                        SalesRepresentativePortfolio,
                        SalesRepresentativeCustomerRelation,
                        SalesRepresentative
                    >(_sql, (_salesRepresentative, salesRepresentativePortfolio, salesRepresentativeCustomerRelation) =>
                    {
                        _salesRepresentative.Portfolio = salesRepresentativePortfolio;
                        _salesRepresentative.Portfolio.Customers.Add(salesRepresentativeCustomerRelation);

                        return _salesRepresentative;
                    }, splitOn: "HasPortfolio, CustomerID", commandTimeout: 360);

                    var _resultGroupedByCustomers = _result
                        .GroupBy(p => p.SalesRepresentativeID)
                        .Select(g =>
                        {
                            var groupedResult = g.First();
                            groupedResult.Portfolio.Customers = g.Select(p => p.Portfolio.Customers.Single()).ToList();

                            return groupedResult;
                        })
                        .ToList();

                    var __result = await conn.QueryAsync<SalesRepresentativeAddress>(__sql);

                    foreach (var salesRepresentative in result)
                    {
                        salesRepresentative.Portfolio = _resultGroupedByCustomers
                                                            .Select(s => s.Portfolio)
                                                            .Where(h => h.SalesRepresentativeID == salesRepresentative.SalesRepresentativeID)
                                                            .FirstOrDefault();

                        salesRepresentative.Addresses = __result.Where(p => p.SalesRepresentativeID == salesRepresentative.SalesRepresentativeID).ToList();
                    }

                    return result.ToList();
                }
            }
            catch (SqlException ex)
            {
                throw new SQLCommandException(
                    message: $"Error when trying to get records that already exist in trusted table - {ex.Message}",
                    exceptionMessage: ex.StackTrace,
                    commandSQL: sql
                );
            }
        }
    }
}
