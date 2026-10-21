using Application.AfterSale.Abstractions.Handlers.Commands;
using Domain.AfterSale.Abstractions.Repositorys;
using Domain.AfterSale.Entities.Reads.Me;
using Domain.Shared.Abstractions.Repositorys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AfterSale.Repositorys
{
    public class AfterSaleMeRepository : IAfterSaleMeRepository
    {
        private readonly IAfterSaleCommandHandler _afterSaleCommandHandler;
        private readonly ISharedRepository _coreRepository;

        public AfterSaleMeRepository(IAfterSaleCommandHandler afterSaleCommandHandler, ISharedRepository coreRepository) =>
            (_afterSaleCommandHandler, _coreRepository) = (afterSaleCommandHandler, coreRepository);

        public async Task<bool> BulkInsertIntoTableAfterSaleEcommerce(List<Ecommerce> data, Guid? parentExecutionGUID)
        {
            var ecommercesTable = _coreRepository.CreateSystemDataTable(entity: new Ecommerce(), tableName: "AfterSaleEcommerces");
            var messagesPagesTable = _coreRepository.CreateSystemDataTable(entity: new MessagePage(), tableName: "AfterSaleEcommercesMessagesPages");
            var messagesMailsTable = _coreRepository.CreateSystemDataTable(entity: new MessageMail(), tableName: "AfterSaleEcommercesMessagesMails");
            var reasonsTable = _coreRepository.CreateSystemDataTable(entity: new Reason(), tableName: "AfterSaleEcommercesReasons");
            var subReasonsTable = _coreRepository.CreateSystemDataTable(entity: new SubReason(), tableName: "AfterSaleEcommercesSubReasons");
            var stringValuesTable = _coreRepository.CreateSystemDataTable(entity: new StringListValue(), tableName: "AfterSaleEcommercesStringValues");

            _coreRepository.FillSystemDataTable(ecommercesTable, data);
            _coreRepository.FillSystemDataTable(messagesPagesTable, data.Select(x => x.messages_pages).ToList());
            _coreRepository.FillSystemDataTable(messagesMailsTable, data.Select(x => x.messages_mails).ToList());
            _coreRepository.FillSystemDataTable(reasonsTable, data.SelectMany(x => x.reasons).ToList());
            _coreRepository.FillSystemDataTable(subReasonsTable, data.SelectMany(x => x.reasons).SelectMany(x => x.subreasons).ToList());
            _coreRepository.FillSystemDataTable(stringValuesTable, data.SelectMany(x => x.ecommerce_string_values).ToList());

            _coreRepository.BulkInsertIntoTableRaw(ecommercesTable);
            _coreRepository.BulkInsertIntoTableRaw(messagesPagesTable);
            _coreRepository.BulkInsertIntoTableRaw(messagesMailsTable);
            _coreRepository.BulkInsertIntoTableRaw(reasonsTable);
            _coreRepository.BulkInsertIntoTableRaw(subReasonsTable);
            _coreRepository.BulkInsertIntoTableRaw(stringValuesTable);

            await _coreRepository.CallDbProcMerge("general", ecommercesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", messagesPagesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", messagesMailsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", reasonsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", subReasonsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", stringValuesTable.TableName, parentExecutionGUID);

            return true;
        }
    }
}
