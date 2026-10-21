using Application.AfterSale.Abstractions.Handlers.Commands;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.AfterSale.Entities.Errors;
using Domain.AfterSale.Entities.Reads.Reverses.Status;
using Domain.AfterSale.Entities.Reads.Reverses.Transportations;
using Domain.Shared.Abstractions.Repositorys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AfterSale.Repositorys
{
    public class AfterSaleReversesRepository : IAfterSaleReversesRepository
    {
        private readonly IAfterSaleCommandHandler _afterSaleCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public AfterSaleReversesRepository(IAfterSaleCommandHandler afterSaleCommandHandler, ISharedRepository coreRepository) =>
            (_afterSaleCommandHandler, _coreRepository) = (afterSaleCommandHandler, coreRepository);

        public async Task<IEnumerable<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesIdsForGetReverse()
        {
            string sql = _afterSaleCommandHandler.CreateGetSummarizedReversesIdsForGetReverseQuery();

            return await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse>(sql);
        }

        public async Task<IEnumerable<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesIdsForGetCourierAttributes()
        {
            string sql = _afterSaleCommandHandler.CreateGetSummarizedReversesIdsForGetCourierAttributesQuery();

            return await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse>(sql);
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse?>> GetSummarizedReversesByIds(string doc_company, List<string> reversesIds)
        {
            var list = new List<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse?>();
            string identificadores = String.Empty;
            int indice = reversesIds.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = reversesIds.Skip(i * 1000).Take(1000).ToList();
                    string sql = _afterSaleCommandHandler.CreateGetSummarizedReversesByIdsQuery(doc_company, top1000List);
                    var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse>(sql);
                    list.AddRange(result);
                }

                return list;
            }
            else
            {
                string sql = _afterSaleCommandHandler.CreateGetSummarizedReversesByIdsQuery(doc_company, reversesIds);
                var AddRange = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse>(sql);
                list.AddRange(AddRange);

                return list;
            }
        }

        public async Task<bool> BulkInsertIntoAfterSaleSummarizedReverses(List<Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse> data, Guid? parentExecutionGUID)
        {
            var summarizedReversesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.SearchReverses.Reverse(), tableName: "AfterSaleSummarizedReverses");

            _coreRepository.FillSystemDataTable(summarizedReversesTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(summarizedReversesTable);

            await _coreRepository.CallDbProcMerge("general", summarizedReversesTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse?>> GetReversesByIds(string doc_company, List<string> reversesIds)
        {
            var list = new List<Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse?>();
            string identificadores = String.Empty;
            int indice = reversesIds.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = reversesIds.Skip(i * 1000).Take(1000).ToList();
                    string sql = _afterSaleCommandHandler.CreateGetReversesByIdsQuery(doc_company, top1000List);
                    var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse>(sql);
                    list.AddRange(result);
                }

                return list;
            }
            else
            {
                string sql = _afterSaleCommandHandler.CreateGetReversesByIdsQuery(doc_company, reversesIds);
                var AddRange = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse>(sql);
                list.AddRange(AddRange);

                return list;
            }
        }

        public async Task<bool> BulkInsertIntoAfterSaleReverses(List<Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse> data, Guid? parentExecutionGUID)
        {
            var reversesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Reverse(), tableName: "AfterSaleReverses");
            var customersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Customer(), tableName: "AfterSaleReversesCustomers");
            var destinationsDataTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.DestinationData(), tableName: "AfterSaleReversesDestinationsData");
            var trackingsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Tracking(), tableName: "AfterSaleReversesTrackings");
            var trackingsHistoriesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.TrackingHistory(), tableName: "AfterSaleReversesTrackingHistories");
            var destinationsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.ReverseDestination(), tableName: "AfterSaleReversesDestinations");
            var ecommerceOrdersTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.EcommerceOrder(), tableName: "AfterSaleReversesEcommerceOrders");
            var ecommerceOrdersPaymentsMethodsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.EcommerceOrderPaymentMethods(), tableName: "AfterSaleReversesEcommerceOrdersPaymentsMethods");
            var productsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Product(), tableName: "AfterSaleReversesProducts");
            var productsImagesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.Image(), tableName: "AfterSaleReversesProductsImages");
            var statusHistoriesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.GetReverse.StatusHistories(), tableName: "AfterSaleReversesStatusHistories");

            _coreRepository.FillSystemDataTable(reversesTable, data.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(customersTable, data.Select(x => x.customer).Where(x => x.doc_company != null).ToList());
            _coreRepository.FillSystemDataTable(destinationsDataTable, data.Select(x => x.destination_data).Where(x => x.doc_company != null).ToList());
            _coreRepository.FillSystemDataTable(trackingsTable, data.Select(x => x.tracking).Where(x => x.doc_company != null).ToList());
            _coreRepository.FillSystemDataTable(trackingsHistoriesTable, data.Select(x => x.tracking).SelectMany(y => y.tracking_history).ToList());
            _coreRepository.FillSystemDataTable(destinationsTable, data.Select(x => x.reverse_destination).Where(x => x.doc_company != null).ToList());
            _coreRepository.FillSystemDataTable(ecommerceOrdersTable, data.Select(x => x.ecommerce_order).Where(x => x.doc_company != null).ToList());
            _coreRepository.FillSystemDataTable(ecommerceOrdersPaymentsMethodsTable, data.Select(x => x.ecommerce_order).SelectMany(y => y.ecommerce_order_payment_methods).ToList());
            _coreRepository.FillSystemDataTable(productsTable, data.SelectMany(x => x.products).ToList());
            _coreRepository.FillSystemDataTable(productsImagesTable, data.SelectMany(x => x.products).SelectMany(y => y.images).ToList());
            _coreRepository.FillSystemDataTable(statusHistoriesTable, data.SelectMany(x => x.status_histories).ToList());

            _coreRepository.BulkInsertIntoTableRaw(reversesTable);
            _coreRepository.BulkInsertIntoTableRaw(customersTable);
            _coreRepository.BulkInsertIntoTableRaw(destinationsDataTable);
            _coreRepository.BulkInsertIntoTableRaw(trackingsTable);
            _coreRepository.BulkInsertIntoTableRaw(trackingsHistoriesTable);
            _coreRepository.BulkInsertIntoTableRaw(destinationsTable);
            _coreRepository.BulkInsertIntoTableRaw(ecommerceOrdersTable);
            _coreRepository.BulkInsertIntoTableRaw(ecommerceOrdersPaymentsMethodsTable);
            _coreRepository.BulkInsertIntoTableRaw(productsTable);
            _coreRepository.BulkInsertIntoTableRaw(productsImagesTable);
            _coreRepository.BulkInsertIntoTableRaw(statusHistoriesTable);

            await _coreRepository.CallDbProcMerge("general", reversesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", customersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", destinationsDataTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", trackingsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", trackingsHistoriesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", destinationsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", ecommerceOrdersTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", ecommerceOrdersPaymentsMethodsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", productsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", productsImagesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", statusHistoriesTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Status?>> GetStatus(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetStatusQuery(doc_company);

            var result = await _coreRepository.GetRecords<Status>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleStatusReverses(List<Domain.AfterSale.Entities.Reads.Reverses.Status.Status> data, Guid? parentExecutionGUID)
        {
            var statusReversesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.Status.Status(), tableName: "AfterSaleStatusReverses");

            _coreRepository.FillSystemDataTable(statusReversesTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(statusReversesTable);

            await _coreRepository.CallDbProcMerge("general", statusReversesTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Transportation?>> GetTransportations(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetTransportationsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Transportation>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleTransportationsReverses(List<Transportation> data, Guid? parentExecutionGUID)
        {
            var transportationsReversesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.Transportations.Transportation(), tableName: "AfterSaleTransportationsReverses");

            _coreRepository.FillSystemDataTable(transportationsReversesTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(transportationsReversesTable);

            await _coreRepository.CallDbProcMerge("general", transportationsReversesTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.CourierAttributes?>> GetCourierAttributesByReverseId(string doc_company, List<Int64> reversesIds)
        {
            var list = new List<Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.CourierAttributes?>();
            string identificadores = String.Empty;
            int indice = reversesIds.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = reversesIds.Skip(i * 1000).Take(1000).ToList();
                    string sql = _afterSaleCommandHandler.CreateGetCourierAttributesByReverseIdQuery(doc_company, top1000List);
                    var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.CourierAttributes>(sql);
                    list.AddRange(result);
                }

                return list;
            }
            else
            {
                string sql = _afterSaleCommandHandler.CreateGetCourierAttributesByReverseIdQuery(doc_company, reversesIds);
                var AddRange = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.CourierAttributes>(sql);
                list.AddRange(AddRange);

                return list;
            }
        }

        public async Task<bool> BulkInsertIntoAfterSaleCourierAttributes(List<Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.CourierAttributes> data, Guid? parentExecutionGUID)
        {
            var courierAttributesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.CourierAttributes(), tableName: "AfterSaleCourierAttributes");
            var fieldsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Reverses.CourierAttributes.Field(), tableName: "AfterSaleCourierAttributesFields");

            _coreRepository.FillSystemDataTable(courierAttributesTable, data.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(fieldsTable, data.SelectMany(x => x.fields).ToList());

            _coreRepository.BulkInsertIntoTableRaw(courierAttributesTable);
            _coreRepository.BulkInsertIntoTableRaw(fieldsTable);

            //a tabela de campos referencia a de courier attributes, por isso o merge da pai vem primeiro
            await _coreRepository.CallDbProcMerge("general", courierAttributesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", fieldsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<bool> BulkInsertIntoAfterSaleErrors(List<Error> data, Guid? parentExecutionGUID)
        {
            var errosTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Errors.Error(), tableName: "AfterSaleErrors");

            _coreRepository.FillSystemDataTable(errosTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(errosTable);

            await _coreRepository.CallDbProcMerge("general", errosTable.TableName, parentExecutionGUID);

            return true;
        }

        //public Task<bool> InsertIntoAfterSaleReversesCourierAttributes()
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<bool> InsertIntoAfterSaleStatus(List<Status> statusReverses, Guid? parentExecutionGUID)
        //{
        //    var statusReversesTable = _coreRepository.CreateSystemDataTable(entity: new Status(), tableName: "AfterSaleStatus");

        //    _coreRepository.FillSystemDataTable(statusReversesTable, statusReverses);

        //    _coreRepository.BulkInsertIntoTableRaw(statusReversesTable);

        //    await _coreRepository.CallDbProcMerge("general", statusReversesTable.TableName, parentExecutionGUID);

        //    return true;
        //}

        //public async Task<bool> InsertIntoAfterSaleTransportations(Transportations transportations, Guid? parentExecutionGUID)
        //{
        //    var transportationsTable = _coreRepository.CreateSystemDataTable(entity: new Transportations(), tableName: "AfterSaleTransportations");

        //    _coreRepository.FillSystemDataTable(transportationsTable, transportations.data);

        //    _coreRepository.BulkInsertIntoTableRaw(transportationsTable);

        //    await _coreRepository.CallDbProcMerge("general", transportationsTable.TableName, parentExecutionGUID);

        //    return true;
        //}

        //public async Task<bool> InsertIntoAfterSaleTransportations(List<Transportations> transportations, Guid? parentExecutionGUID)
        //{
        //    var transportationsTable = _coreRepository.CreateSystemDataTable(entity: new Transportations(), tableName: "AfterSaleTransportations");

        //    _coreRepository.FillSystemDataTable(transportationsTable, transportations);

        //    _coreRepository.BulkInsertIntoTableRaw(transportationsTable);

        //    await _coreRepository.CallDbProcMerge("general", transportationsTable.TableName, parentExecutionGUID);

        //    return true;
        //}
    }
}
