using Application.Totvs.Abstractions.Services;

namespace WebApi.Scheduling.RecurringJobs
{
    /// <summary>
    /// jobs recorrentes da Totvs: IdJob de auditing.Jobs → método do service (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md
    /// §4.2). Se roda, quando e em que fila vem da tabela.
    /// </summary>
    public static class TotvsRecurringJobs
    {
        public static IEnumerable<RecurringJobDefinition> All =>
        [
            RecurringJobDefinition.For<ITotvsAccountsPayableService>(13008, s => s.PostSearchDuplicates()),
            RecurringJobDefinition.For<ITotvsAccountsPayableService>(13009, s => s.PostSearchComissionsPaid()),

            RecurringJobDefinition.For<ITotvsInvoicesService>(13003, s => s.PostSearchInvoices()),
            RecurringJobDefinition.For<ITotvsInvoicesService>(13250, s => s.GetInvoicesDisable()),
            RecurringJobDefinition.For<ITotvsInvoicesService>(13251, s => s.GetCostCenter()),

            RecurringJobDefinition.For<ITotvsPeopleService>(13001, s => s.PostSearchLegalEntity()),
            RecurringJobDefinition.For<ITotvsPeopleService>(13002, s => s.PostSearchCustomers()),
            RecurringJobDefinition.For<ITotvsPeopleService>(13007, s => s.GetBranchesList()),

            RecurringJobDefinition.For<ITotvsPersonCatalogsService>(13032, s => s.GetClassifications()),
            RecurringJobDefinition.For<ITotvsPersonCatalogsService>(13033, s => s.GetEmailTypes()),
            RecurringJobDefinition.For<ITotvsPersonCatalogsService>(13034, s => s.GetPhoneTypes()),
            RecurringJobDefinition.For<ITotvsPersonCatalogsService>(13035, s => s.GetContactTypes()),

            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13080, s => s.GetClassificationType()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13081, s => s.GetClassifications()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13082, s => s.GetCategory()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13083, s => s.GetGrid()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13084, s => s.GetMeasurementUnit()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13085, s => s.PostSearchColors()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13086, s => s.GetAdditionalFieldsTypes()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13087, s => s.GetProductGrouperConfiguration()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13088, s => s.GetInstructionItems()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13089, s => s.PostCompositions()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13090, s => s.GetPriceTablesHeaders()),
            RecurringJobDefinition.For<ITotvsProductCatalogsService>(13091, s => s.GetPriceTableScales()),

            RecurringJobDefinition.For<ITotvsProductPricesService>(13015, s => s.PostSearchPrices()),
            RecurringJobDefinition.For<ITotvsProductPricesService>(13016, s => s.PostSearchCosts()),
            RecurringJobDefinition.For<ITotvsProductPricesService>(13092, s => s.PostSearchPriceTables()),
            RecurringJobDefinition.For<ITotvsProductPricesService>(13093, s => s.GetPromotion()),

            RecurringJobDefinition.For<ITotvsProductsService>(13000, s => s.PostSearchReferences()),
            RecurringJobDefinition.For<ITotvsProductsService>(13017, s => s.PostSearchBalances()),
            RecurringJobDefinition.For<ITotvsProductsService>(13094, s => s.GetSimilar()),
            RecurringJobDefinition.For<ITotvsProductsService>(13095, s => s.PostSearchBatch()),
            RecurringJobDefinition.For<ITotvsProductsService>(13097, s => s.PostSearchProductCodes()),

            RecurringJobDefinition.For<ITotvsPurchaseOrdersService>(13004, s => s.PostSearchPurchaseOrders()),

            RecurringJobDefinition.For<ITotvsRepresentativesService>(13030, s => s.PostSearchRepresentatives()),
            RecurringJobDefinition.For<ITotvsRepresentativesService>(13031, s => s.GetSearchReceptionSale()),

            RecurringJobDefinition.For<ITotvsSalesOrdersService>(13005, s => s.PostSearchSalesOrders()),
            RecurringJobDefinition.For<ITotvsSalesOrdersService>(13190, s => s.GetDiscountType()),
            RecurringJobDefinition.For<ITotvsSalesOrdersService>(13191, s => s.GetClassifications()),
            RecurringJobDefinition.For<ITotvsSalesOrdersService>(13192, s => s.GetBatchItems()),
            RecurringJobDefinition.For<ITotvsSalesOrdersService>(13194, s => s.GetBillingSuggestions()),

            RecurringJobDefinition.For<ITotvsSellersService>(13006, s => s.PostSearchSellers()),
            RecurringJobDefinition.For<ITotvsSellersService>(13070, s => s.GetOperationalArea())
        ];
    }
}
