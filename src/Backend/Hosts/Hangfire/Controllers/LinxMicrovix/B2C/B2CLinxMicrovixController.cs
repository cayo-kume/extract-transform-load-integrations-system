//using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxCommerce;
//using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix
{
    [ApiController]
    [Authorize]
    [Route("LinxMicrovixJobs/B2CLinxMicrovix")]
    public class B2CLinxMicrovixController : Controller
    {
//        private readonly LinxAPIParam _linxMicrovixJobParameter;
//        private readonly List<LinxMethods>? _methods;
//        private readonly IConfiguration _configuration;

//        private readonly IB2CConsultaClientesService _b2cConsultaClientesService;
//        private readonly IB2CConsultaEmpresasService _b2cConsultaEmpresasService;
//        private readonly IB2CConsultaNFeService _b2cConsultaNFeService;
//        private readonly IB2CConsultaNFeSituacaoService _b2cConsultaNFeSituacaoService;
//        private readonly IB2CConsultaPedidosService _b2cConsultaPedidosService;
//        private readonly IB2CConsultaPedidosIdentificadorService _b2cConsultaPedidosIdentificadorService;
//        private readonly IB2CConsultaPedidosItensService _b2cConsultaPedidosItensService;
//        private readonly IB2CConsultaPedidosStatusService _b2cConsultaPedidosStatusService;
//        private readonly IB2CConsultaStatusService _b2cConsultaStatusService;

//        public B2CLinxMicrovixController
//        (
//            IConfiguration configuration,
//            IB2CConsultaClientesService b2cConsultaClientesService,
//            IB2CConsultaEmpresasService b2cConsultaEmpresasService,
//            IB2CConsultaNFeService b2cConsultaNFeService,
//            IB2CConsultaNFeSituacaoService b2cConsultaNFeSituacaoService,
//            IB2CConsultaPedidosService b2cConsultaPedidosService,
//            IB2CConsultaPedidosIdentificadorService b2cConsultaPedidosIdentificadorService,
//            IB2CConsultaPedidosItensService b2cConsultaPedidosItensService,
//            IB2CConsultaPedidosStatusService b2cConsultaPedidosStatusService,
//            IB2CConsultaStatusService b2cConsultaStatusService
//        )
//        {
//            _configuration = configuration;
//            _b2cConsultaClientesService = b2cConsultaClientesService;
//            _b2cConsultaEmpresasService = b2cConsultaEmpresasService;
//            _b2cConsultaNFeService = b2cConsultaNFeService;
//            _b2cConsultaNFeSituacaoService = b2cConsultaNFeSituacaoService;
//            _b2cConsultaPedidosService = b2cConsultaPedidosService;
//            _b2cConsultaPedidosIdentificadorService = b2cConsultaPedidosIdentificadorService;
//            _b2cConsultaPedidosItensService = b2cConsultaPedidosItensService;
//            _b2cConsultaPedidosStatusService = b2cConsultaPedidosStatusService;
//            _b2cConsultaStatusService = b2cConsultaStatusService;

//            _linxMicrovixJobParameter = new LinxAPIParam(
//                docMainCompany: _configuration
//                                .GetSection("B2CLinxMicrovix")
//                                .GetSection("DocMainCompany")
//                                .Value,

//                schema: "linx_microvix_commerce",

//                databaseName: _configuration
//                                .GetSection("ConfigureServer")
//                                .GetSection("Databases")
//                                .GetSection("LinxMicrovixCommerce")
//                                .Value,

//                untreatedDatabaseName: _configuration
//                                .GetSection("ConfigureServer")
//                                .GetSection("Databases")
//                                .GetSection("Untreated")
//                                .Value,

//                projectName: _configuration
//                                .GetSection("B2CLinxMicrovix")
//                                .GetSection("ProjectName")
//                                .Value,

//                parametersInterval: _configuration
//                                .GetSection("B2CLinxMicrovix")
//                                .GetSection("ParametersDateInterval")
//                                .Value,

//                parametersTableName: _configuration
//                                .GetSection("B2CLinxMicrovix")
//                                .GetSection("ParametersTableName")
//                                .Value,

//                keyAuthorization: _configuration
//                                .GetSection("B2CLinxMicrovix")
//                                .GetSection("Key")
//                                .Value,

//                userAuthentication: _configuration
//                                .GetSection("B2CLinxMicrovix")
//                                .GetSection("Authentication")
//                                .Value
//            );

//            _methods = _configuration
//                            .GetSection("B2CLinxMicrovix")
//                            .GetSection("Methods")
//                            .Get<List<LinxMethods>>();
//        }

//        #region Date Interval
//        [HttpPost("B2CConsultaClientes")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaClientes()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaClientes")
//                .FirstOrDefault();

//            var result = await _b2cConsultaClientesService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("B2CConsultaEmpresas")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaEmpresas()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaEmpresas")
//                .FirstOrDefault();

//            var result = await _b2cConsultaEmpresasService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("B2CConsultaNFe")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaNFe()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaNFe")
//                .FirstOrDefault();

//            var result = await _b2cConsultaNFeService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("B2CConsultaNFeSituacao")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaNFeSituacao()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaNFeSituacao")
//                .FirstOrDefault();

//            var result = await _b2cConsultaNFeSituacaoService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("B2CConsultaPedidos")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaPedidos()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidos")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("B2CConsultaPedidosIdentificador")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaPedidosIdentificador()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidosIdentificador")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosIdentificadorService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("B2CConsultaPedidosItens")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaPedidosItens()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidosItens")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosItensService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("B2CConsultaPedidosStatus")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaPedidosStatus()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidosStatus")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosStatusService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("B2CConsultaStatus")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> B2CConsultaStatus()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaStatus")
//                .FirstOrDefault();

//            var result = await _b2cConsultaStatusService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }
//        #endregion

//        #region Individual
//        [HttpPost("B2CConsultaClientesIndividual")]
//        public async Task<ActionResult> B2CConsultaClientesIndividual([Required][FromQuery] string? doc_cliente)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaClientes")
//                .FirstOrDefault();

//            var result = await _b2cConsultaClientesService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: doc_cliente
//            );

//            return Ok();
//        }

//        [HttpPost("B2CConsultaNFeIndividual")]
//        public async Task<ActionResult> B2CConsultaNFeIndividual([Required][FromQuery] string? id_pedido)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaNFe")
//                .FirstOrDefault();

//            var result = await _b2cConsultaNFeService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: id_pedido
//            );

//            return Ok();
//        }


//        [HttpPost("B2CConsultaPedidosIndividual")]
//        public async Task<ActionResult> B2CConsultaPedidosIndividual([Required][FromQuery] string? id_pedido)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidos")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: id_pedido
//            );

//            return Ok();
//        }

//        [HttpPost("B2CConsultaPedidosItensIndividual")]
//        public async Task<ActionResult> B2CConsultaPedidosItensIndividual([Required][FromQuery] string? id_pedido, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidosItens")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosItensService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: id_pedido
//            );

//            return Ok();
//        }

//        [HttpPost("B2CConsultaPedidosStatusIndividual")]
//        public async Task<ActionResult> B2CConsultaPedidosStatusIndividual([Required][FromQuery] string? id_pedido, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidosStatus")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosStatusService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: id_pedido
//            );

//            return Ok();
//        }
//        #endregion

//        #region Integrity Lock
//        [HttpPost("IntegrityLockB2CConsultaClientesRegisters")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockB2CConsultaClientesRegisters()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaClientes")
//                .FirstOrDefault();

//            var result = await _b2cConsultaClientesService.IntegrityLockB2CConsultaClientesRegisters(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("IntegrityLockB2CConsultaPedidosRegisters")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockB2CConsultaPedidosRegisters()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidos")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosService.IntegrityLockRegisters(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("IntegrityLockB2CConsultaPedidosIdentificadorRegisters")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockB2CConsultaPedidosIdentificadorRegisters()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidosIdentificador")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosIdentificadorService.IntegrityLockRegisters(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("IntegrityLockB2CConsultaPedidosStatusRegisters")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockB2CConsultaPedidosStatusRegisters()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidosStatus")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosStatusService.IntegrityLockRegisters(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("IntegrityLockB2CConsultaNFeRegisters")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockB2CConsultaNFeRegisters()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaNFe")
//                .FirstOrDefault();

//            var result = await _b2cConsultaNFeService.IntegrityLockRegisters(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("IntegrityLockB2CConsultaPedidosItensRegisters")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockB2CConsultaPedidosItensRegisters()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "B2CConsultaPedidosItens")
//                .FirstOrDefault();

//            var result = await _b2cConsultaPedidosItensService.IntegrityLockRegisters(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }
//        #endregion
    }
}
