//using Application.LinxMicrovix.Outbound.Services.LinxMicrovix;
//using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix;
//using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hangfire.Controllers.LinxMicrovix.ERP
{
    [ApiController]
    [Authorize]
    [Route("LinxMicrovixJobs/LinxMicrovix")]
    public class LinxMicrovixController : Controller
    {
//        private readonly LinxAPIParam _linxMicrovixJobParameter;
//        private readonly List<LinxMethods>? _methods;
//        private readonly IConfiguration _configuration;

//        private readonly ILinxProdutosTabelasPrecosService _linxProdutosTabelasPrecosService;
//        private readonly ILinxClientesEnderecosEntregaService _linxClientesEnderecosEntregaService;
//        private readonly ILinxClientesFornecService _linxClientesFornecService;
//        private readonly ILinxMovimentoService _linxMovimentoService;
//        private readonly ILinxMovimentoTrocasService _linxMovimentoTrocasService;
//        private readonly ILinxMovimentoPlanosService _linxMovimentoPlanosService;
//        private readonly ILinxMovimentoCartoesService _linxMovimentoCartoesService;
//        private readonly ILinxNaturezaOperacaoService _linxNaturezaOperacaoService;
//        private readonly ILinxPlanosService _linxPlanosService;
//        private readonly ILinxPedidosVendaService _linxPedidosVendaService;
//        private readonly ILinxPedidosCompraService _linxPedidosCompraService;
//        private readonly ILinxProdutosService _linxProdutosService;
//        private readonly ILinxProdutosInventarioService _linxProdutosInventarioService;
//        private readonly ILinxProdutosDetalhesService _linxProdutosDetalhesService;
//        private readonly ILinxProdutosDepositosService _linxProdutosDepositosService;
//        private readonly ILinxProdutosTabelasService _linxProdutosTabelasService;
//        private readonly ILinxProdutosPromocoesService _linxProdutosPromocoesService;
//        private readonly ILinxProdutosCamposAdicionaisService _linxProdutosCamposAdicionaisService;
//        private readonly ILinxGrupoLojasService _linxGrupoLojasService;
//        private readonly ILinxLojasService _linxLojasService;
//        private readonly ILinxSetoresService _linxSetoresService;
//        private readonly ILinxVendedoresService _linxVendedoresService;
//        private readonly ILinxXMLDocumentosService _linxXMLDocumentosService;
//        private readonly ILinxRotinaOrigemService _linxRotinaOrigemService;
//        private readonly ILinxCfopFiscalService _linxCfopFiscalService;
//        private readonly ILinxUsuariosService _linxUsuariosService;
//        private readonly ILinxProdutosCodBarService _linxProdutosCodBarService;
//        private readonly ILinxReshopSaldoService _linxReshopSaldoService;

//        public LinxMicrovixController(
//            IConfiguration configuration,
//            ILinxProdutosTabelasPrecosService linxProdutosTabelasPrecosService,
//            ILinxClientesEnderecosEntregaService linxClientesEnderecosEntregaService,
//            ILinxClientesFornecService linxClientesFornecService,
//            ILinxMovimentoService linxMovimentoService,
//            ILinxMovimentoTrocasService linxMovimentoTrocasService,
//            ILinxMovimentoPlanosService linxMovimentoPlanosService,
//            ILinxMovimentoCartoesService linxMovimentoCartoesService,
//            ILinxNaturezaOperacaoService linxNaturezaOperacaoService,
//            ILinxPlanosService linxPlanosService,
//            ILinxProdutosService linxProdutosService,
//            ILinxProdutosInventarioService linxProdutosInventarioService,
//            ILinxProdutosDetalhesService linxProdutosDetalhesService,
//            ILinxProdutosDepositosService linxProdutosDepositosService,
//            ILinxProdutosTabelasService linxProdutosTabelasService,
//            ILinxProdutosPromocoesService linxProdutosPromocoesService,
//            ILinxProdutosCamposAdicionaisService linxProdutosCamposAdicionaisService,
//            ILinxPedidosVendaService linxPedidosVendaService,
//            ILinxPedidosCompraService linxPedidosCompraService,
//            ILinxLojasService linxLojasService,
//            ILinxGrupoLojasService linxGrupoLojasService,
//            ILinxSetoresService linxSetoresService,
//            ILinxVendedoresService linxVendedoresService,
//            ILinxXMLDocumentosService linxXMLDocumentosService,
//            ILinxRotinaOrigemService linxRotinaOrigemService,
//            ILinxCfopFiscalService linxCfopFiscalService,
//            ILinxUsuariosService linxUsuariosService,
//            ILinxProdutosCodBarService linxProdutosCodBarService,
//            ILinxReshopSaldoService linxReshopSaldoService
//        )
//        {
//            _configuration = configuration;
//            _linxProdutosTabelasPrecosService = linxProdutosTabelasPrecosService;
//            _linxClientesEnderecosEntregaService = linxClientesEnderecosEntregaService;
//            _linxClientesFornecService = linxClientesFornecService;
//            _linxMovimentoService = linxMovimentoService;
//            _linxMovimentoTrocasService = linxMovimentoTrocasService;
//            _linxMovimentoPlanosService = linxMovimentoPlanosService;
//            _linxMovimentoCartoesService = linxMovimentoCartoesService;
//            _linxNaturezaOperacaoService = linxNaturezaOperacaoService;
//            _linxPlanosService = linxPlanosService;
//            _linxProdutosService = linxProdutosService;
//            _linxProdutosInventarioService = linxProdutosInventarioService;
//            _linxProdutosDetalhesService = linxProdutosDetalhesService;
//            _linxProdutosDepositosService = linxProdutosDepositosService;
//            _linxProdutosTabelasService = linxProdutosTabelasService;
//            _linxProdutosPromocoesService = linxProdutosPromocoesService;
//            _linxProdutosCamposAdicionaisService = linxProdutosCamposAdicionaisService;
//            _linxPedidosVendaService = linxPedidosVendaService;
//            _linxPedidosCompraService = linxPedidosCompraService;
//            _linxGrupoLojasService = linxGrupoLojasService;
//            _linxLojasService = linxLojasService;
//            _linxSetoresService = linxSetoresService;
//            _linxVendedoresService = linxVendedoresService;
//            _linxXMLDocumentosService = linxXMLDocumentosService;
//            _linxRotinaOrigemService = linxRotinaOrigemService;
//            _linxCfopFiscalService = linxCfopFiscalService;
//            _linxUsuariosService = linxUsuariosService;
//            _linxProdutosCodBarService = linxProdutosCodBarService;
//            _linxReshopSaldoService = linxReshopSaldoService;

//            _linxMicrovixJobParameter = new LinxAPIParam(
//                docMainCompany: _configuration
//                                .GetSection("LinxMicrovix")
//                                .GetSection("DocMainCompany")
//                                .Value,

//                schema: "linx_microvix_erp",

//                databaseName: _configuration
//                                .GetSection("ConfigureServer")
//                                .GetSection("Databases")
//                                .GetSection("LinxMicrovix")
//                                .Value,

//                untreatedDatabaseName: _configuration
//                                .GetSection("ConfigureServer")
//                                .GetSection("Databases")
//                                .GetSection("Untreated")
//                                .Value,

//                projectName: _configuration
//                                .GetSection("LinxMicrovix")
//                                .GetSection("ProjectName")
//                                .Value,

//                parametersInterval: _configuration
//                                .GetSection("LinxMicrovix")
//                                .GetSection("ParametersDateInterval")
//                                .Value,

//                parametersTableName: _configuration
//                                .GetSection("LinxMicrovix")
//                                .GetSection("ParametersTableName")
//                                .Value,

//                keyAuthorization: _configuration
//                                .GetSection("LinxMicrovix")
//                                .GetSection("Key")
//                                .Value,

//                userAuthentication: _configuration
//                                .GetSection("LinxMicrovix")
//                                .GetSection("Authentication")
//                                .Value
//            );

//            _methods = _configuration
//                            .GetSection("LinxMicrovix")
//                            .GetSection("Methods")
//                            .Get<List<LinxMethods>>();
//        }

//        #region Date Interval
//        [HttpPost("LinxClientesFornec")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxClientesFornec()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxClientesFornec")
//                .FirstOrDefault();

//            var result = await _linxClientesFornecService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxClientesEnderecosEntrega")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxClientesEnderecosEntrega()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxClientesEnderecosEntrega")
//                .FirstOrDefault();

//            var result = await _linxClientesEnderecosEntregaService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxMovimento")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxMovimento()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxMovimento")
//                .FirstOrDefault();

//            var result = await _linxMovimentoService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxMovimentoCartoes")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxMovimentoCartoes()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxMovimentoCartoes")
//                .FirstOrDefault();

//            var result = await _linxMovimentoCartoesService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxMovimentoPlanos")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxMovimentoPlanos()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxMovimentoPlanos")
//                .FirstOrDefault();

//            var result = await _linxMovimentoPlanosService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxMovimentoTrocas")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxMovimentoTrocas()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxMovimentoTrocas")
//                .FirstOrDefault();

//            var result = await _linxMovimentoTrocasService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxGrupoLojas")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxGrupoLojas()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxGrupoLojas")
//                .FirstOrDefault();

//            var result = await _linxGrupoLojasService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxLojas")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxLojas()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxLojas")
//                .FirstOrDefault();

//            var result = await _linxLojasService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxPlanos")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxPlanos()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxPlanos")
//                .FirstOrDefault();

//            var result = await _linxPlanosService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxNaturezaOperacao")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxNaturezaOperacao()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxNaturezaOperacao")
//                .FirstOrDefault();

//            var result = await _linxNaturezaOperacaoService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosCodBar")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutosCodBar()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosCodBar")
//                .FirstOrDefault();

//            var result = await _linxProdutosCodBarService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosTabelas")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutosTabelas()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosTabelas")
//                .FirstOrDefault();

//            var result = await _linxProdutosTabelasService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosTabelasPrecos")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutosTabelasPrecos()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosTabelasPrecos")
//                .FirstOrDefault();

//            var result = await _linxProdutosTabelasPrecosService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosDepositos")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutosDepositos()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosDepositos")
//                .FirstOrDefault();

//            var result = await _linxProdutosDepositosService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosInventario")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutosInventario()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosInventario")
//                .FirstOrDefault();

//            var result = await _linxProdutosInventarioService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosDetalhes")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutosDetalhes()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosDetalhes")
//                .FirstOrDefault();

//            var result = await _linxProdutosDetalhesService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosPromocoes")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutosPromocoes()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosPromocoes")
//                .FirstOrDefault();

//            var result = await _linxProdutosPromocoesService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosCamposAdicionais")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutosCamposAdicionais()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosCamposAdicionais")
//                .FirstOrDefault();

//            var result = await _linxProdutosCamposAdicionaisService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutos")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxProdutos()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutos")
//                .FirstOrDefault();

//            var result = await _linxProdutosService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxPedidosVenda")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxPedidosVenda()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxPedidosVenda")
//                .FirstOrDefault();

//            var result = await _linxPedidosVendaService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxPedidosCompra")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxPedidosCompra()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxPedidosCompra")
//                .FirstOrDefault();

//            var result = await _linxPedidosCompraService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxSetores")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxSetores()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxSetores")
//                .FirstOrDefault();

//            var result = await _linxSetoresService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxVendedores")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxVendedores()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxVendedores")
//                .FirstOrDefault();

//            var result = await _linxVendedoresService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxUsuarios")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxUsuarios()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxUsuarios")
//                .FirstOrDefault();

//            var result = await _linxUsuariosService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxRotinaOrigem")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxRotinaOrigem()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxRotinaOrigem")
//                .FirstOrDefault();

//            var result = await _linxRotinaOrigemService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxCfopFiscal")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxCfopFiscal()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxCfopFiscal")
//                .FirstOrDefault();

//            var result = await _linxCfopFiscalService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxXMLDocumentos")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> LinxXMLDocumentos()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxXMLDocumentos")
//                .FirstOrDefault();

//            var result = await _linxXMLDocumentosService.GetRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }
//        #endregion

//        #region Individual
//        [HttpPost("LinxClientesFornecIndividual")]
//        public async Task<ActionResult> LinxClientesFornecIndividual([Required][FromQuery] string? doc_cliente)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxClientesFornec")
//                .FirstOrDefault();

//            var result = await _linxClientesFornecService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: doc_cliente
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxMovimentoIndividual")]
//        public async Task<ActionResult> LinxMovimentoIndividual([Required][FromQuery] string? documento, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxMovimento")
//                .FirstOrDefault();

//            var result = await _linxMovimentoService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: documento,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxMovimentoCartoesIndividual")]
//        public async Task<ActionResult> LinxMovimentoCartoesIndividual([Required][FromQuery] string? identificador, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxMovimentoCartoes")
//                .FirstOrDefault();

//            var result = await _linxMovimentoCartoesService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: identificador,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxMovimentoPlanosIndividual")]
//        public async Task<ActionResult> LinxMovimentoPlanosIndividual([Required][FromQuery] string? identificador, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxMovimentoPlanos")
//                .FirstOrDefault();

//            var result = await _linxMovimentoPlanosService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: identificador,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxMovimentoTrocasIndividual")]
//        public async Task<ActionResult> LinxMovimentoTrocasIndividual([Required][FromQuery] string? doc_origem, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxMovimentoTrocas")
//                .FirstOrDefault();

//            var result = await _linxMovimentoTrocasService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: doc_origem,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxPlanosIndividual")]
//        public async Task<ActionResult> LinxPlanosIndividual([Required][FromQuery] string? plano)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxPlanos")
//                .FirstOrDefault();

//            var result = await _linxPlanosService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: plano
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxNaturezaOperacaoIndividual")]
//        public async Task<ActionResult> LinxNaturezaOperacaoIndividual([Required][FromQuery] string? cod_natureza_operacao)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxNaturezaOperacao")
//                .FirstOrDefault();

//            var result = await _linxNaturezaOperacaoService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_natureza_operacao
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosCodBarIndividual")]
//        public async Task<ActionResult> LinxProdutosCodBarIndividual([Required][FromQuery] string? cod_produto)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosCodBar")
//                .FirstOrDefault();

//            var result = await _linxProdutosCodBarService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_produto
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosTabelasIndividual")]
//        public async Task<ActionResult> LinxProdutosTabelasIndividual([Required][FromQuery] string? id_tabela, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosTabelas")
//                .FirstOrDefault();

//            var result = await _linxProdutosTabelasService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: id_tabela,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosTabelasPrecosIndividual")]
//        public async Task<ActionResult> LinxProdutosTabelasPrecosIndividual([Required][FromQuery] string? id_tabela, [Required][FromQuery] string? cod_produto, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosTabelasPrecos")
//                .FirstOrDefault();

//            var result = await _linxProdutosTabelasPrecosService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_produto,
//                identificador2: id_tabela,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosDepositosIndividual")]
//        public async Task<ActionResult> LinxProdutosDepositosIndividual([Required][FromQuery] string? cod_produto)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosDepositos")
//                .FirstOrDefault();

//            var result = await _linxProdutosDepositosService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_produto
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosInventarioIndividual")]
//        public async Task<ActionResult> LinxProdutosInventarioIndividual([Required][FromQuery] string? cod_produto, [Required][FromQuery] string? data_inventario, [Required][FromQuery] string? cod_deposito, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosInventario")
//                .FirstOrDefault();

//            var result = await _linxProdutosInventarioService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_produto,
//                identificador2: data_inventario,
//                identificador3: cod_deposito,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosDetalhesIndividual")]
//        public async Task<ActionResult> LinxProdutosDetalhesIndividual([Required][FromQuery] string? cod_produto, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosDetalhes")
//                .FirstOrDefault();

//            var result = await _linxProdutosDetalhesService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_produto,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosDetalhesWithZeroStock")]
//        public async Task<ActionResult> LinxProdutosDetalhesWithZeroStock()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosDetalhes")
//                .FirstOrDefault();

//            var result = await _linxProdutosDetalhesService.GetZeroStockRecords(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosCamposAdicionaisIndividual")]
//        public async Task<ActionResult> LinxProdutosCamposAdicionaisIndividual([Required][FromQuery] string? cod_produto)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutosCamposAdicionais")
//                .FirstOrDefault();

//            var result = await _linxProdutosCamposAdicionaisService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_produto
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxProdutosIndividual")]
//        public async Task<ActionResult> LinxProdutosIndividual([Required][FromQuery] string? cod_produto)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxProdutos")
//                .FirstOrDefault();

//            var result = await _linxProdutosService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_produto
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxPedidosVendaIndividual")]
//        public async Task<ActionResult> LinxPedidosVendaIndividual([Required][FromQuery] string? cod_pedido, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxPedidosVenda")
//                .FirstOrDefault();

//            var result = await _linxPedidosVendaService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_pedido,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxSetoresIndividual")]
//        public async Task<ActionResult> LinxSetoresIndividual([Required][FromQuery] string? id_setor)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxSetores")
//                .FirstOrDefault();

//            var result = await _linxSetoresService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: id_setor
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxXMLDocumentosIndividual")]
//        public async Task<ActionResult> LinxXMLDocumentosIndividual([Required][FromQuery] string? documento, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxXMLDocumentos")
//                .FirstOrDefault();

//            var result = await _linxXMLDocumentosService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: documento,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("LinxVendedoresIndividual")]
//        public async Task<ActionResult> LinxVendedoresIndividual([Required][FromQuery] string? cod_vendedor, [Required][FromQuery] string? cnpj_emp)
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxVendedores")
//                .FirstOrDefault();

//            var result = await _linxVendedoresService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName,
//                    parametersInterval: "parameters_individual"
//                ),
//                identificador: cod_vendedor,
//                cnpj_emp: cnpj_emp
//            );

//            return Ok($"Records integrated successfully.");
//        }
//        #endregion

//        #region LinxReshopExcel
//        [HttpPost("Import")]
//        public async Task<ActionResult> Import()
//        {
//            var result = await _linxReshopSaldoService.ExecuteAsync();

//            if (result)
//                return Ok("LinxReshopExcel import completed successfully.");

//            return BadRequest("LinxReshopExcel import failed. Check logs for details.");
//        }
//        #endregion

//        #region Integrity Lock
//        [HttpPost("IntegrityLockLinxPedidosCompraRegisters")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockLinxPedidosCompraRegisters()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxPedidosCompra")
//                .FirstOrDefault();

//            var result = await _linxPedidosCompraService.IntegrityLockRegisters(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }

//        [HttpPost("IntegrityLockLinxClientesFornecRegisters")]
//#if RELEASE
//        [ApiExplorerSettings(IgnoreApi = true)]
//#endif
//        public async Task<ActionResult> IntegrityLockLinxClientesFornecRegisters()
//        {
//            var method = _methods
//                .Where(m => m.MethodName == "LinxClientesFornec")
//                .FirstOrDefault();

//            var result = await _linxClientesFornecService.GetRecord(
//                _linxMicrovixJobParameter.SetParameters(
//                    jobName: method.MethodName,
//                    tableName: method.MethodName
//                )
//            );

//            return Ok($"Records integrated successfully.");
//        }
//        #endregion
    }
}
