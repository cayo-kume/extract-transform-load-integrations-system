using Application.Oto.Interfaces.Handlers.Commands;
using Dapper;
using Domain.Core.Abstractions.Repositorys;
using Domain.Oto.Entities;
using Domain.Oto.Interfaces.Repositorys;
using Domain.Oto.Models;
using Infrastructure.Core.Abstractions;

namespace Infrastructure.Oto.Repositorys
{
    public class OtoRepository : IOtoRepository
    {
        private readonly IOtoCommandHandler _otoCommandHandler;
        private readonly ISQLServerConnection _sqlServerConnection;
        private readonly IIntegrationsCoreRepository _coreRepository;

        public OtoRepository(IOtoCommandHandler otoCommandHandler, ISQLServerConnection sqlServerConnection, IIntegrationsCoreRepository coreRepository) =>
            (_otoCommandHandler, _sqlServerConnection, _coreRepository) = (otoCommandHandler, sqlServerConnection, coreRepository);

        public bool BulkInsertIntoTableRaw(List<SentRecordLog> records, Guid? parentExecutionGUID)
        {
            var sentRecordLogsTable = _coreRepository.CreateSystemDataTable("OttoSentRecordsLog", new SentRecordLog());

            var sentRecordLogs = records.Select(x => x).ToList();

            _coreRepository.FillSystemDataTable(sentRecordLogsTable, records);

            _coreRepository.BulkInsertIntoTableRaw(sentRecordLogsTable, "general");

            return true;
        }

        public async Task<IEnumerable<Client?>> GetClients()
        {
            string sql = _otoCommandHandler.CreateGetClientsQuery();

            return await _coreRepository.GetRecords<Client>(sql);
        }

        public async Task<IEnumerable<Client?>> GetClientsCashBack()
        {
            string sql = _otoCommandHandler.CreateGetClientsCashBackQuery();

            return await _coreRepository.GetRecords<Client>(sql);
        }

        public async Task<IEnumerable<Company?>> GetCompanys()
        {
            string sql = _otoCommandHandler.CreateGetCompanysQuery();

            return await _coreRepository.GetRecords<Company>(sql);
        }

        public async Task<List<Order?>> GetOrders()
        {
            string sql = _otoCommandHandler.CreateGetOrdersQuery();

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.QueryAsync<Order, Client, Product, Company, Order>(sql, (order, client, product, company) =>
                    {
                        order.itens.Add(product);
                        order.client = client;
                        order.company = company;
                        return order;
                    }, splitOn: "Code, Code, Code", commandTimeout: 360);

                    var orders = result.GroupBy(p => p.Number).Select(g =>
                    {
                        var groupedOrder = g.FirstOrDefault();
                        groupedOrder.itens = g.Select(p => p.itens.Single()).ToList();
                        return groupedOrder;
                    });

                    return orders.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Parameters?>> GetParameters()
        {
            string sql = _otoCommandHandler.CreateGetParametersQuery();

            return await _coreRepository.GetRecords<Parameters>(sql);
        }

        public async Task<IEnumerable<Product?>> GetProducts()
        {
            string sql = _otoCommandHandler.CreateGetProductsQuery();

            return await _coreRepository.GetRecords<Product>(sql);
        }

        public async Task<IEnumerable<SaleRepresentative?>> GetSaleRepresentatives()
        {
            string sql = _otoCommandHandler.CreateGetSaleRepresentativesQuery();

            return await _coreRepository.GetRecords<SaleRepresentative>(sql);
        }
    }
}
