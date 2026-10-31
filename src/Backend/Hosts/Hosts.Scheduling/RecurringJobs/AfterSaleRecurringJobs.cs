using Application.AfterSale.Abstractions.Services;

namespace Hosts.Scheduling.RecurringJobs
{
    /// <summary>
    /// jobs recorrentes do AfterSale: IdJob de auditing.Jobs → método do service (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md
    /// §4.2). Se roda, quando e em que fila vem da tabela.
    /// </summary>
    public static class AfterSaleRecurringJobs
    {
        public static IEnumerable<RecurringJobDefinition> All =>
        [
            RecurringJobDefinition.For<IAfterSaleMeService>(5000, s => s.GetMe()),

            RecurringJobDefinition.For<IAfterSaleRefundsService>(5011, s => s.GetRefund()),
            RecurringJobDefinition.For<IAfterSaleRefundsService>(5012, s => s.GetRefunds()),
            RecurringJobDefinition.For<IAfterSaleRefundsService>(5013, s => s.GetStatus()),
            RecurringJobDefinition.For<IAfterSaleRefundsService>(5014, s => s.GetActions()),
            RecurringJobDefinition.For<IAfterSaleRefundsService>(5015, s => s.GetBanks()),
            RecurringJobDefinition.For<IAfterSaleRefundsService>(5016, s => s.GetTypes()),

            RecurringJobDefinition.For<IAfterSaleReversesService>(5001, s => s.GetReverse()),
            RecurringJobDefinition.For<IAfterSaleReversesService>(5002, s => s.GetReverses()),
            RecurringJobDefinition.For<IAfterSaleReversesService>(5003, s => s.GetTransportations()),
            RecurringJobDefinition.For<IAfterSaleReversesService>(5004, s => s.GetCourierAttributes()),
            RecurringJobDefinition.For<IAfterSaleReversesService>(5009, s => s.GetStatus())
        ];
    }
}
