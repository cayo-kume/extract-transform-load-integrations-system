using Application.LinxCommerce.Abstractions.Handlers.Commands;
using Domain.LinxCommerce.Abstractions.Repositorys;
using Domain.LinxCommerce.Entities.Reads.Orders;
using Domain.Shared.Abstractions.Repositorys;
using System.Collections;
using System.Data;

namespace Infrastructure.LinxCommerce.Repositorys
{
    public class LinxCommerceOrdersRepository : ILinxCommerceOrdersRepository
    {
        private readonly ILinxCommerceOrdersCommandHandler _linxCommerceOrdersCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public LinxCommerceOrdersRepository(ILinxCommerceOrdersCommandHandler linxCommerceOrdersCommandHandler, ISharedRepository coreRepository) =>
            (_linxCommerceOrdersCommandHandler, _coreRepository) = (linxCommerceOrdersCommandHandler, coreRepository);

        public async Task<DateTimeOffset?> GetOrdersLastModifiedDate(string doc_company)
        {
            string sql = _linxCommerceOrdersCommandHandler.CreateGetOrdersLastModifiedDateQuery(doc_company);

            return await _coreRepository.GetRecord<DateTimeOffset?>(sql);
        }

        public async Task<IEnumerable<Order?>> GetOrdersByIds(string doc_company, List<Guid> ids)
        {
            var result = new List<Order?>();

            // lotes de 1000 ids, como no GetPeopleByIds do Movidesk
            foreach (var chunk in ids.Distinct().Chunk(1000))
            {
                string sql = _linxCommerceOrdersCommandHandler.CreateGetOrdersByIdsQuery(doc_company, chunk.ToList());

                result.AddRange(await _coreRepository.GetRecords<Order>(sql));
            }

            return result;
        }

        public async Task<bool> BulkInsertIntoLinxCommerceOrders(List<Order> data, Guid? parentExecutionGUID)
        {
            // ordem = ordem dos merges: pedido antes das filhas (FK e condição de exclusão do merge)
            var tables = new List<(DataTable table, IList rows)>
            {
                (_coreRepository.CreateSystemDataTable(entity: new Order(), tableName: "LinxCommerceOrders"), data),
                (_coreRepository.CreateSystemDataTable(entity: new OrderItem(), tableName: "LinxCommerceOrdersItems"), data.SelectMany(x => x.items).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderTag(), tableName: "LinxCommerceOrdersTags"), data.SelectMany(x => x.tags).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderAddress(), tableName: "LinxCommerceOrdersAddresses"), data.SelectMany(x => x.addresses).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderPaymentMethod(), tableName: "LinxCommerceOrdersPaymentMethods"), data.SelectMany(x => x.paymentMethods).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderDeliveryMethod(), tableName: "LinxCommerceOrdersDeliveryMethods"), data.SelectMany(x => x.deliveryMethods).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderDiscount(), tableName: "LinxCommerceOrdersDiscounts"), data.SelectMany(x => x.discounts).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderShipment(), tableName: "LinxCommerceOrdersShipments"), data.SelectMany(x => x.shipments).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderPackage(), tableName: "LinxCommerceOrdersShipmentsPackages"), data.SelectMany(x => x.packages).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderInvoice(), tableName: "LinxCommerceOrdersInvoices"), data.SelectMany(x => x.invoices).ToList()),
                (_coreRepository.CreateSystemDataTable(entity: new OrderProperty(), tableName: "LinxCommerceOrdersProperties"), data.SelectMany(x => x.properties).ToList()),
            };

            foreach (var (table, rows) in tables)
            {
                // FillSystemDataTable lê as propriedades do tipo real do primeiro item, então List<object> funciona
                _coreRepository.FillSystemDataTable(table, rows.Cast<object>().ToList());
                _coreRepository.BulkInsertIntoTableRaw(table, "untreated");
            }

            // o merge das filhas roda mesmo sem linhas no lote: é ele que apaga as que saíram do pedido
            foreach (var (table, _) in tables)
                await _coreRepository.CallDbProcMerge("linxcommerce", table.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<bool> BulkInsertIntoLinxCommerceOrderStatus(List<OrderStatus> data, Guid? parentExecutionGUID)
        {
            var table = _coreRepository.CreateSystemDataTable(entity: new OrderStatus(), tableName: "LinxCommerceOrderStatus");

            _coreRepository.FillSystemDataTable(table, data);
            _coreRepository.BulkInsertIntoTableRaw(table, "untreated");

            await _coreRepository.CallDbProcMerge("linxcommerce", table.TableName, parentExecutionGUID);

            return true;
        }
    }
}
