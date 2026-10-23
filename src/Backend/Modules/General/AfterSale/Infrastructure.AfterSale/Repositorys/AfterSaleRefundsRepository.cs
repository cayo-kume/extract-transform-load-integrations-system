using Application.AfterSale.Abstractions.Handlers.Commands;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.Shared.Abstractions.Repositorys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AfterSale.Repositorys
{
    public class AfterSaleRefundsRepository : IAfterSaleRefundsRepository
    {
        private readonly IAfterSaleCommandHandler _afterSaleCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public AfterSaleRefundsRepository(IAfterSaleCommandHandler afterSaleCommandHandler, ISharedRepository coreRepository) =>
            (_afterSaleCommandHandler, _coreRepository) = (afterSaleCommandHandler, coreRepository);

        public async Task<IEnumerable<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund?>> GetSummarizedRefundsIdsForGetRefund()
        {
            string sql = _afterSaleCommandHandler.CreateGetSummarizedRefundsIdsForGetRefundQuery();

            return await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>(sql);
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund?>> GetRefundsByIds(string doc_company, List<string> refundsIds)
        {
            var list = new List<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund?>();
            int indice = refundsIds.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = refundsIds.Skip(i * 1000).Take(1000).ToList();
                    string sql = _afterSaleCommandHandler.CreateGetRefundsByIdsQuery(doc_company, top1000List);
                    var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund>(sql);
                    list.AddRange(result);
                }

                return list;
            }
            else
            {
                string sql = _afterSaleCommandHandler.CreateGetRefundsByIdsQuery(doc_company, refundsIds);
                var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund>(sql);
                list.AddRange(result);

                return list;
            }
        }

        public async Task<bool> BulkInsertIntoAfterSaleRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund> data, Guid? parentExecutionGUID)
        {
            var refundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Refund(), tableName: "AfterSaleRefunds");
            var couldChangeStatusForTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.CouldChangeStatusFor(), tableName: "AfterSaleRefundsCouldChangeStatusFor");
            var ordersPaymentsMethodsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.OrderPaymentMethod(), tableName: "AfterSaleRefundsOrdersPaymentsMethods");
            var statusHistoriesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.StatusHistories(), tableName: "AfterSaleRefundsStatusHistories");
            var totalAmountHistoriesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.TotalAmountHistories(), tableName: "AfterSaleRefundsTotalAmountHistories");
            var productsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Product(), tableName: "AfterSaleRefundsProducts");
            var productsImagesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.Image(), tableName: "AfterSaleRefundsProductsImages");
            var productsImagesUrlsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.GetRefund.ImageUrl(), tableName: "AfterSaleRefundsProductsImagesUrls");

            _coreRepository.FillSystemDataTable(refundsTable, data.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(couldChangeStatusForTable, data.SelectMany(x => x.could_change_status_for).ToList());
            _coreRepository.FillSystemDataTable(ordersPaymentsMethodsTable, data.SelectMany(x => x.order_payment_methods).ToList());
            _coreRepository.FillSystemDataTable(statusHistoriesTable, data.SelectMany(x => x.status_histories).ToList());
            _coreRepository.FillSystemDataTable(totalAmountHistoriesTable, data.SelectMany(x => x.total_amount_histories).ToList());
            _coreRepository.FillSystemDataTable(productsTable, data.SelectMany(x => x.products).ToList());
            _coreRepository.FillSystemDataTable(productsImagesTable, data.SelectMany(x => x.products).SelectMany(y => y.images).ToList());
            _coreRepository.FillSystemDataTable(productsImagesUrlsTable, data.SelectMany(x => x.products).SelectMany(y => y.image_url).ToList());

            _coreRepository.BulkInsertIntoTableRaw(refundsTable, "untreated");
            _coreRepository.BulkInsertIntoTableRaw(couldChangeStatusForTable, "untreated");
            _coreRepository.BulkInsertIntoTableRaw(ordersPaymentsMethodsTable, "untreated");
            _coreRepository.BulkInsertIntoTableRaw(statusHistoriesTable, "untreated");
            _coreRepository.BulkInsertIntoTableRaw(totalAmountHistoriesTable, "untreated");
            _coreRepository.BulkInsertIntoTableRaw(productsTable, "untreated");
            _coreRepository.BulkInsertIntoTableRaw(productsImagesTable, "untreated");
            _coreRepository.BulkInsertIntoTableRaw(productsImagesUrlsTable, "untreated");

            //as filhas referenciam o reembolso e as imagens referenciam o produto: merge do pai para os filhos
            await _coreRepository.CallDbProcMerge("aftersale", refundsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("aftersale", couldChangeStatusForTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("aftersale", ordersPaymentsMethodsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("aftersale", statusHistoriesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("aftersale", totalAmountHistoriesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("aftersale", productsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("aftersale", productsImagesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("aftersale", productsImagesUrlsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Status.Status?>> GetStatus(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetStatusRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Status.Status>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleStatusRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Status.Status> data, Guid? parentExecutionGUID)
        {
            var statusRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Status.Status(), tableName: "AfterSaleStatusRefunds");

            _coreRepository.FillSystemDataTable(statusRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(statusRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("aftersale", statusRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Actions.Action?>> GetActions(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetActionsRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Actions.Action>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleActionsRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Actions.Action> data, Guid? parentExecutionGUID)
        {
            var actionsRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Actions.Action(), tableName: "AfterSaleActionsRefunds");

            _coreRepository.FillSystemDataTable(actionsRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(actionsRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("aftersale", actionsRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank?>> GetBanks(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetBanksRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleBanksRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank> data, Guid? parentExecutionGUID)
        {
            var banksRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Banks.Bank(), tableName: "AfterSaleBanksRefunds");

            _coreRepository.FillSystemDataTable(banksRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(banksRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("aftersale", banksRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.Types.Type?>> GetTypes(string doc_company)
        {
            string sql = _afterSaleCommandHandler.CreateGetTypesRefundsQuery(doc_company);

            var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.Types.Type>(sql);

            return result.ToList();
        }

        public async Task<bool> BulkInsertIntoAfterSaleTypesRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.Types.Type> data, Guid? parentExecutionGUID)
        {
            var typesRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.Types.Type(), tableName: "AfterSaleTypesRefunds");

            _coreRepository.FillSystemDataTable(typesRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(typesRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("aftersale", typesRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund?>> GetSummarizedRefundsByIds(string doc_company, List<string> refundsIds)
        {
            var list = new List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund?>();
            int indice = refundsIds.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = refundsIds.Skip(i * 1000).Take(1000).ToList();
                    string sql = _afterSaleCommandHandler.CreateGetSummarizedRefundsByIdsQuery(doc_company, top1000List);
                    var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>(sql);
                    list.AddRange(result);
                }

                return list;
            }
            else
            {
                string sql = _afterSaleCommandHandler.CreateGetSummarizedRefundsByIdsQuery(doc_company, refundsIds);
                var result = await _coreRepository.GetRecords<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund>(sql);
                list.AddRange(result);

                return list;
            }
        }

        public async Task<bool> BulkInsertIntoAfterSaleSummarizedRefunds(List<Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund> data, Guid? parentExecutionGUID)
        {
            var summarizedRefundsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Reads.Refunds.SearchRefunds.Refund(), tableName: "AfterSaleSummarizedRefunds");

            _coreRepository.FillSystemDataTable(summarizedRefundsTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(summarizedRefundsTable, "untreated");

            await _coreRepository.CallDbProcMerge("aftersale", summarizedRefundsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<bool> BulkInsertIntoAfterSaleErrors(List<Domain.AfterSale.Entities.Errors.Error> data, Guid? parentExecutionGUID)
        {
            var errosTable = _coreRepository.CreateSystemDataTable(entity: new Domain.AfterSale.Entities.Errors.Error(), tableName: "AfterSaleErrors");

            _coreRepository.FillSystemDataTable(errosTable, data.Select(x => x).ToList());

            _coreRepository.BulkInsertIntoTableRaw(errosTable, "untreated");

            await _coreRepository.CallDbProcMerge("aftersale", errosTable.TableName, parentExecutionGUID);

            return true;
        }
    }
}
