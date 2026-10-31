using Application.DataWarehouse.Abstractions.Services;
using Application.Shared.Abstractions.Services;
using Domain.DataWarehouse.Abstractions.Repositorys;
using Domain.DataWarehouse.Entities.Control;
using Domain.DataWarehouse.Enums;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Application.Shared.Attributes;

namespace Application.DataWarehouse.Services
{
    /// <summary>
    /// job do DW: chama as procedures na ordem da seção 10.1 do guia (src/Backend/Modules/DataWarehouse/docs/07-operacao.md) e grava o resultado
    /// das checagens em dw_control.CheckResult. As procedures fazem o trabalho; aqui ficam só a ordem, o log e a trava
    /// </summary>
    public class DataWarehouseService : IDataWarehouseService
    {
        private readonly ILoggerService _logger;
        private readonly IDataWarehouseRepository _dataWarehouseRepository;
        private readonly IConfiguration _configuration;

        // uma carga por vez no processo: os ciclos horário, diário e curto escrevem nas mesmas landings. Entre processos, o
        // agendador que for codificado precisa da mesma trava (DisableConcurrentExecution, no Hangfire)
        private static readonly SemaphoreSlim RunLock = new(1, 1);

        // origens do ciclo curto da logística (10.3)
        private static readonly HashSet<string> ShortCycleTables =
        [
            "TotvsSaleOrders", "TotvsSaleOrdersProducts", "TotvsInvoices", "TotvsInvoicesItems", "TotvsInvoicesSalesOrders", "TotvsPeople", "TotvsLegalEntities"
        ];

        // checagens da seção 11: nome da view e se bloqueia
        private static readonly (string View, bool Blocking)[] Checks =
        [
            ("[datawarehouse].vw_Check_FactSalesItemGrain", true),
            ("[datawarehouse].vw_Check_ErpOverlap", true),
            ("[datawarehouse].vw_Check_ProductMatch", false),
            ("[datawarehouse].vw_Check_UnknownMembers", false),
            ("[datawarehouse].vw_Check_ItemProducts", false),
            ("[datawarehouse].vw_Check_OrderTypeUnknown", false),
            ("[datawarehouse].vw_Check_MissingChildren", false),
            ("[datamart].vw_Check_InvoicedReconciliation", false),
            ("[datawarehouse].vw_Check_StockNegativeOrNoCost", false)
        ];

        // fuso das datas do DW (horário de Brasília): decide o dia 1 da posição de estoque
        private static readonly TimeZoneInfo Brasilia = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

        public DataWarehouseService(ILoggerService logger, IDataWarehouseRepository dataWarehouseRepository, IConfiguration configuration) =>
            (_logger, _dataWarehouseRepository, _configuration) = (logger, dataWarehouseRepository, configuration);

        [JobVersion("1.0.0")]
        public Task<bool?> RunHourly() =>
            Run(EnumJob.DataWarehouseHourly, daily: false);

        [JobVersion("1.0.0")]
        public Task<bool?> RunDaily() =>
            Run(EnumJob.DataWarehouseDaily, daily: true);

        public async Task<bool?> RunMonthlyStock(DateOnly? positionDate)
        {
            if (!await RunLock.WaitAsync(0))
                return null;

            _logger
                .Clear()
                .AddLog(EnumJob.DataWarehouseMonthlyStock);

            try
            {
                await Step("[datawarehouse].sp_FactMonthlyStock_Load",
                    new Dictionary<string, object?> { { "position_date", positionDate?.ToDateTime(TimeOnly.MinValue) } });

                var blocked = await RunChecks();

                _logger.AddMessage(blocked ? "Concluída com checagem bloqueante: veja dw_control.CheckResult." : "Concluída com sucesso!");

                return !blocked;
            }
            catch (Exception ex)
            {
                _logger.AddMessage($"Posição de estoque interrompida: {ex.Message}", String.Empty, ex.Message, ex.StackTrace ?? String.Empty);

                return false;
            }
            finally
            {
                await CommitLogAndRelease();
            }
        }

        public async Task<bool?> RunLinxLegacy()
        {
            if (!await RunLock.WaitAsync(0))
                return null;

            _logger
                .Clear()
                .AddLog(EnumJob.DataWarehouseLinxLegacy);

            try
            {
                var sources = (await _dataWarehouseRepository.GetLandingSourcesBySchema("linxmicrovix")).ToList();

                // passos 2 e 4 da 10.1, só para o Linx: landing e o snapshot único
                foreach (var source in sources)
                    await Step($"[dw_landing].[sp_{source.source_table}_Load]");

                foreach (var source in sources.Where(x => x.history_frequency == "once"))
                    await Step($"[dw_history].[sp_{source.source_table}_Snapshot]");

                // passo 5: clientes Linx
                await Step("[dw_personal_data].sp_LinxCustomer_Load");

                // passo 6: as dimensões refazem Totvs e Linx juntos; depois as linhas Linx dos fatos e o agregado
                await Step("[datawarehouse].sp_DimStore_Load");
                await Step("[datawarehouse].sp_DimProduct_Load");
                await PepperStep("[datawarehouse].sp_DimCustomer_Load");
                await Step("[datawarehouse].sp_DimSeller_Load");
                await Step("[datawarehouse].sp_DimOperation_Load");
                await Step("[datawarehouse].sp_DimBrand_Load");
                await Step("[dw_intermediate].sp_LinxLegacy_Load");
                await Step("[datamart].sp_DailySales_Refresh");

                // passo 7
                var blocked = await RunChecks();

                _logger.AddMessage(blocked ? "Concluída com checagem bloqueante: veja dw_control.CheckResult." : "Concluída com sucesso!");

                return !blocked;
            }
            catch (Exception ex)
            {
                _logger.AddMessage($"Carga do legado interrompida: {ex.Message}", String.Empty, ex.Message, ex.StackTrace ?? String.Empty);

                return false;
            }
            finally
            {
                await CommitLogAndRelease();
            }
        }

        [JobVersion("1.0.0")]
        public async Task<bool?> RunShortCycle()
        {
            if (!await RunLock.WaitAsync(0))
                return null;

            _logger
                .Clear()
                .AddLog(EnumJob.DataWarehouseShortCycle);

            try
            {
                var sources = (await _dataWarehouseRepository.GetLandingSources()).Where(x => ShortCycleTables.Contains(x.source_table));

                foreach (var source in sources)
                    await Step($"[dw_landing].[sp_{source.source_table}_Load]");

                await Step("[dw_personal_data].sp_Customer_Load");
                await Step("[dw_personal_data].sp_OrderShippingAddress_Load");

                _logger.AddMessage("Concluída com sucesso!");

                return true;
            }
            catch (Exception ex)
            {
                _logger.AddMessage($"Ciclo curto interrompido: {ex.Message}", String.Empty, ex.Message, ex.StackTrace ?? String.Empty);

                return false;
            }
            finally
            {
                await CommitLogAndRelease();
            }
        }

        // ------------------------------------------------------------------------------------------------------

        /// <summary>
        /// sequência da 10.1. O passo 1 (ler os watermarks) e o avanço deles ficam dentro das procedures de carga
        /// </summary>
        private async Task<bool?> Run(EnumJob job, bool daily)
        {
            if (!await RunLock.WaitAsync(0))
                return null;

            _logger
                .Clear()
                .AddLog(job);

            try
            {
                var sources = (await _dataWarehouseRepository.GetLandingSources()).ToList();

                // passo 2: landing
                foreach (var source in sources)
                    await Step($"[dw_landing].[sp_{source.source_table}_Load]");

                // passo 3: filhas apagadas na origem (uma vez por dia)
                if (daily)
                    foreach (var source in sources.Where(x => x.detect_missing))
                        await Step($"[dw_landing].[sp_{source.source_table}_DetectMissing]");

                // passo 4: snapshots (os diários só no ciclo diário)
                foreach (var source in sources.Where(x => x.history_frequency == "hourly" || (daily && x.history_frequency == "daily")))
                    await Step($"[dw_history].[sp_{source.source_table}_Snapshot]");

                // passo 5: dado pessoal
                await Step("[dw_personal_data].sp_Customer_Load");
                await Step("[dw_personal_data].sp_OrderShippingAddress_Load");
                await Step("[dw_personal_data].sp_ServiceCustomer_Load");

                // passo 6: dimensões, fatos e agregados
                if (daily)
                    await Step("[datawarehouse].sp_DimDate_Load");

                await Step("[datawarehouse].sp_DimStore_Load");
                await Step("[datawarehouse].sp_DimProduct_Load");
                await PepperStep("[datawarehouse].sp_DimCustomer_Load");
                await Step("[datawarehouse].sp_DimSeller_Load");
                await Step("[datawarehouse].sp_DimSupplier_Load");
                await Step("[datawarehouse].sp_DimPaymentMethod_Load");
                await Step("[datawarehouse].sp_DimOperation_Load");
                await Step("[datawarehouse].sp_DimChannel_Load");
                await Step("[datawarehouse].sp_DimBrand_Load");
                await Step("[datawarehouse].sp_DimSalesRepresentative_Load");
                await Step("[datawarehouse].sp_DimRegion_Load");
                await Step("[datawarehouse].sp_DimPromotion_Load");
                await Step("[datawarehouse].sp_FactSalesItem_Load");
                await Step("[datawarehouse].sp_FactOrderItem_Load");
                await Step("[datawarehouse].sp_FactInvoicePayment_Load");
                await PepperStep("[datawarehouse].sp_FactTicket_Load");
                await PepperStep("[datawarehouse].sp_FactRefund_Load");
                await Step("[datawarehouse].sp_FactPurchaseOrderItem_Load");
                await Step("[datawarehouse].sp_FactAccountsPayable_Load");
                await PepperStep("[datawarehouse].sp_FactCommission_Load");
                await Step("[datawarehouse].sp_FactStoreReception_Load");
                await Step("[datawarehouse].sp_FactApiRequest_Load");

                // posição de estoque do mês que fechou: no ciclo diário do dia 1 (8.5)
                if (daily && TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Brasilia).Day == 1)
                    await Step("[datawarehouse].sp_FactMonthlyStock_Load");

                await Step("[datamart].sp_DailySales_Refresh");

                // passo 7: checagens
                var blocked = await RunChecks();

                _logger.AddMessage(blocked ? "Concluída com checagem bloqueante: veja dw_control.CheckResult." : "Concluída com sucesso!");

                return !blocked;
            }
            catch (Exception ex)
            {
                _logger.AddMessage($"Carga interrompida: {ex.Message}", String.Empty, ex.Message, ex.StackTrace ?? String.Empty);

                return false;
            }
            finally
            {
                await CommitLogAndRelease();
            }
        }

        /// <summary>
        /// a trava sai mesmo se a gravação do log falhar; senão as próximas chamadas ficariam em 409 até o processo reiniciar
        /// </summary>
        private async Task CommitLogAndRelease()
        {
            try
            {
                _logger.SetLogEndDate();
                await _logger.CommitAllChanges();
            }
            finally
            {
                RunLock.Release();
            }
        }

        /// <summary>
        /// procedures que calculam o doc_hash (DimCustomer, FactTicket, FactRefund, FactCommission). O pepper vem da configuração
        /// (DataWarehouse:Pepper) e só passa como parâmetro (12.3). Sem ele, a tabela fica como está e o resto da carga segue
        /// </summary>
        private async Task PepperStep(string procedureName)
        {
            var pepper = _configuration["DataWarehouse:Pepper"];

            if (String.IsNullOrWhiteSpace(pepper))
            {
                _logger.AddMessage($"DataWarehouse:Pepper não configurado: {procedureName} não foi executada.");
                return;
            }

            await Step(procedureName, new Dictionary<string, object?> { { "pepper", pepper } });
        }

        private async Task<int> Step(string procedureName, IDictionary<string, object?>? parameters = null)
        {
            var watch = Stopwatch.StartNew();
            var rows = await _dataWarehouseRepository.ExecuteProcedure(procedureName, parameters);

            _logger.AddMessage($"{procedureName}: {rows} linha(s) em {watch.Elapsed.TotalSeconds:0.0} s");

            return rows;
        }

        private async Task<bool> RunChecks()
        {
            var blocked = false;

            foreach (var (view, blocking) in Checks)
            {
                var errors = await _dataWarehouseRepository.CountRows(view);
                var name = view.Split('.').Last();

                await _dataWarehouseRepository.InsertCheckResult(new CheckResult(name, errors));

                if (errors > 0)
                {
                    _logger.AddMessage($"Checagem {name}: {errors} linha(s) com erro{(blocking ? " (bloqueia)" : String.Empty)}.");
                    blocked |= blocking;
                }
            }

            return blocked;
        }
    }
}
