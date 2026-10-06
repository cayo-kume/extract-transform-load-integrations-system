using Application.Stone.Abstractions.Handlers.Commands;
using Dapper;
using Domain.Core.Abstractions.Repositorys;
using Domain.Stone.Abstractions.Repository;
using Domain.Stone.Entities;
using Infrastructure.Core.Abstractions;
using Microsoft.Win32;

namespace Infrastructure.Stone.Repository
{
    public class StoneRepository : IStoneRepository
    {
        private readonly IIntegrationsCoreRepository _coreRepository;
        private readonly ISQLServerConnection _sqlServerConnection;
        private readonly IStoneLogisticaCommandHandler _commandHandler;

        public StoneRepository(IIntegrationsCoreRepository coreRepository, ISQLServerConnection sqlServerConnection, IStoneLogisticaCommandHandler commandHandler)
        {
            _sqlServerConnection = sqlServerConnection;
            _coreRepository = coreRepository;
            _commandHandler = commandHandler;
        }

        public async Task<bool> BulkInsertIntoTableRaw(IList<Order> records, Guid? parentExecutionGUID)
        {
            var orderTable = _coreRepository.CreateSystemDataTable("StoneLogisticaOrders", new Order());
            var customerTable = _coreRepository.CreateSystemDataTable("StoneLogisticaCustomers", new Customer());
            var senderTable = _coreRepository.CreateSystemDataTable("StoneLogisticaSenders", new Sender());
            var addressTable = _coreRepository.CreateSystemDataTable("StoneLogisticaAddresses", new Address());
            var itemTable = _coreRepository.CreateSystemDataTable("StoneLogisticaItems", new Item());
            var eventTable = _coreRepository.CreateSystemDataTable("StoneLogisticaEvents", new Event());
            var ownerTable = _coreRepository.CreateSystemDataTable("StoneLogisticaOwner", new Owner());
            var errorsTable = _coreRepository.CreateSystemDataTable("StoneLogisticaErrors", new Error());

            var orders = records.Select(x => x).ToList();
            var errors = records.Select(x => x.error).Where(x => x.orderNumber != null);
            var owner = records.Select(x => x.owner).Where(x => x.document != null).GroupBy(x => x.ownerId).Select(x => x.FirstOrDefault()).FirstOrDefault();
            var customers = records.Select(x => x.customer).Where(x => x.document != null).GroupBy(x => x.document).Select(x => x.FirstOrDefault()).ToList();
            var senders = records.Select(x => x.sender).Where(x => x.document != null).GroupBy(x => x.document).Select(x => x.FirstOrDefault()).ToList();
            var addresses = records.SelectMany(x => x.addresses).Where(x => x.chave != null).GroupBy(x => x.chave).Select(x => x.FirstOrDefault());

            var owners = new List<Owner>();
            owners.Add(owner);
            owners.Add(new Owner());
            customers.Add(new Customer());

            _coreRepository.FillSystemDataTable(ownerTable, owners);
            _coreRepository.FillSystemDataTable(errorsTable, errors.ToList());
            _coreRepository.FillSystemDataTable(orderTable, orders);
            _coreRepository.FillSystemDataTable(customerTable, customers);
            _coreRepository.FillSystemDataTable(senderTable, senders);
            _coreRepository.FillSystemDataTable(addressTable, addresses.ToList());
            _coreRepository.FillSystemDataTable(eventTable, records.SelectMany(x => x.events).ToList());
            _coreRepository.FillSystemDataTable(itemTable, records.SelectMany(x => x.items).ToList());

            _coreRepository.BulkInsertIntoTableRaw(errorsTable);
            _coreRepository.BulkInsertIntoTableRaw(orderTable);
            _coreRepository.BulkInsertIntoTableRaw(customerTable);
            _coreRepository.BulkInsertIntoTableRaw(senderTable);
            _coreRepository.BulkInsertIntoTableRaw(ownerTable);
            _coreRepository.BulkInsertIntoTableRaw(addressTable);
            _coreRepository.BulkInsertIntoTableRaw(eventTable);
            _coreRepository.BulkInsertIntoTableRaw(itemTable);

            await _coreRepository.CallDbProcMerge("general", errorsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", ownerTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", senderTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", customerTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", orderTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", itemTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", addressTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", eventTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<IEnumerable<Parameters?>> GetParameters()
        {
            string sql = _commandHandler.CreateGetParametersQuery();
            return await _coreRepository.GetRecords<Parameters>(sql);
        }

        public async Task<IEnumerable<Zpl?>> GetExistingReferenceKeys()
        {
            string sql = _commandHandler.CreateGetExistingReferenceKeysQuery();
            return await _coreRepository.GetRecords<Zpl>(sql);
        }

        public async Task<IEnumerable<OrdersToBeSent?>> GetRegistersExists()
        {
            string sql = _commandHandler.CreateGetRegistersExistsQuery();

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    //return await conn.QueryAsync<OrdersToBeSent>(sql);

                    var result = await conn.QueryAsync<OrdersToBeSent, Product, Client, ShippingCompany, Company, Invoice, OrdersToBeSent>(sql, (order, product, client, shippingCompany, company, invoice) =>
                    {
                        order.itens.Add(product);
                        order.client = client;
                        order.shippingCompany = shippingCompany;
                        order.company = company;
                        order.invoice = invoice;
                        return order;
                    }, splitOn: "Code, Code, Code, Code, Number", commandTimeout: 360);

                    return result.GroupBy(p => p.Number).Select(g =>
                    {
                        var groupedOrder = g.FirstOrDefault();
                        groupedOrder.itens = g.Select(p => p.itens.Single()).ToList();
                        return groupedOrder;
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Domain.Stone.Entities.Order?>> GetRegistersExists(List<Guid> registros)
        {
            string sql = _commandHandler.CreateGetRegistersExistsQuery(registros);
            return await _coreRepository.GetRecords<Domain.Stone.Entities.Order>(sql);
        }

        public Task<bool> InsertOrder(Order? record)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertZpls(List<Zpl> records)
        {
            var orderTable = _coreRepository.CreateSystemDataTable("StoneLogisticaZplLabels", new Zpl());
            var errorsTable = _coreRepository.CreateSystemDataTable("StoneLogisticaErrors", new Error());

            var orders = records.Select(x => x).Where(x => x.zpl != null).ToList();
            var errors = records.Select(x => x.error).Where(x => x.orderNumber != null);

            _coreRepository.FillSystemDataTable(orderTable, orders);
            _coreRepository.FillSystemDataTable(errorsTable, errors.ToList());

            _coreRepository.BulkInsertIntoTableRaw(orderTable, "general");
            _coreRepository.BulkInsertIntoTableRaw(errorsTable);

            await _coreRepository.CallDbProcMerge("general", errorsTable.TableName);

            return true;
        }
    }
}
