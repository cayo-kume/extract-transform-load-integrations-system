using Application.DataWarehouse.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers.DataWarehouse
{
    /// <summary>
    /// rotas que disparam as cargas do DW (src/Backend/Modules/DataWarehouse/docs, seção 10). Até os agendadores existirem, são o disparo manual
    /// e o ponto que o agendador vai chamar: horária (vendas e pedidos), diária (com os snapshots de cadastros, custos e preços)
    /// e o ciclo curto de 3 minutos da logística
    /// </summary>
    [ApiController]
    //[Authorize]
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Route("DataWarehouseJobs/DataWarehouse")]
    public class DataWarehouseController : Controller
    {
        private readonly IDataWarehouseService _dataWarehouseService;

        public DataWarehouseController(IDataWarehouseService dataWarehouseService) =>
            _dataWarehouseService = dataWarehouseService;

        [HttpPost("RunHourly")]
        public async Task<ActionResult> RunHourly() =>
            RunResponse(await _dataWarehouseService.RunHourly());

        [HttpPost("RunDaily")]
        public async Task<ActionResult> RunDaily() =>
            RunResponse(await _dataWarehouseService.RunDaily());

        [HttpPost("RunShortCycle")]
        public async Task<ActionResult> RunShortCycle() =>
            RunResponse(await _dataWarehouseService.RunShortCycle());

        /// <summary>
        /// refaz a posição de estoque de um fim de mês (positionDate=yyyy-MM-dd); sem data, o último dia do mês anterior
        /// </summary>
        [HttpPost("RunMonthlyStock")]
        public async Task<ActionResult> RunMonthlyStock([FromQuery] DateOnly? positionDate) =>
            RunResponse(await _dataWarehouseService.RunMonthlyStock(positionDate));

        /// <summary>
        /// carga única do legado Linx Microvix (10.2); pode ser reexecutada
        /// </summary>
        [HttpPost("RunLinxLegacy")]
        public async Task<ActionResult> RunLinxLegacy() =>
            RunResponse(await _dataWarehouseService.RunLinxLegacy());

        /// <summary>
        /// true concluída (200), false com erro ou checagem bloqueante (500, detalhe no log da execução), null já havia uma carga
        /// rodando (409)
        /// </summary>
        private ActionResult RunResponse(bool? result) =>
            result switch
            {
                true => Ok("Data warehouse load completed successfully."),
                false => StatusCode(500, "Data warehouse load failed or a blocking check did not pass. See the execution log and dw_control.CheckResult."),
                null => Conflict("Another data warehouse load is running.")
            };
    }
}
