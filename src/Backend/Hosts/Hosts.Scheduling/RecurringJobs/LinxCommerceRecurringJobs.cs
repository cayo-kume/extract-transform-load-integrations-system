using Application.LinxCommerce.Abstractions.Services;

namespace Hosts.Scheduling.RecurringJobs
{
    /// <summary>
    /// jobs recorrentes do Linx Commerce: IdJob de auditing.Jobs → método do service (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md
    /// §4.2). Se roda, quando e em que fila vem da tabela.
    /// </summary>
    public static class LinxCommerceRecurringJobs
    {
        public static IEnumerable<RecurringJobDefinition> All =>
        [
            RecurringJobDefinition.For<ILinxCommerceCatalogService>(7003, s => s.SearchProduct()),
            RecurringJobDefinition.For<ILinxCommerceCatalogService>(7004, s => s.SearchSKU()),

            RecurringJobDefinition.For<ILinxCommerceCustomersService>(7002, s => s.SearchCustomer()),

            RecurringJobDefinition.For<ILinxCommerceOrdersService>(7000, s => s.SearchOrders()),
            RecurringJobDefinition.For<ILinxCommerceOrdersService>(7001, s => s.SearchOrderStatus()),

            RecurringJobDefinition.For<ILinxCommerceSalesRepresentativesService>(7005, s => s.SearchSalesRepresentative())
        ];
    }
}
