using Application.LinxMicrovix.Outbound.WebService.Abstractions.Api;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxCommerce;
using Application.LinxMicrovix.Outbound.WebService.Abstractions.Services.LinxMicrovix;
using Application.LinxMicrovix.Outbound.WebService.Handlers.Commands;
using Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxCommerce;
using Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix;
using Application.LinxMicrovix.Outbound.WebService.Services;
using Application.LinxMicrovix.Outbound.WebService.Services.LinxMicrovix;
using Application.LinxMicrovix.Outbound.WebService.Validations.LinxCommerce;
using Application.LinxMicrovix.Outbound.WebService.Validations.LinxMicrovix;
using Domain.Core.Abstractions.Api;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxCommerce;
using Domain.LinxMicrovix.Outbound.WebService.Abstractions.Repositorys.LinxMicrovix;
using FluentValidation;
using Infrastructure.Core.Api;
using Infrastructure.LinxMicrovix.Outbound.WebService.Api;
using Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxCommerce;
using Infrastructure.LinxMicrovix.Outbound.WebService.Repository.LinxMicrovix;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Application.LinxMicrovix.Outbound.Services.LinxMicrovix;
using Infrastructure.LinxMicrovix.Outbound.Repositorys.LinxMicrovix;

namespace Infrastructure.Outbound.WebService.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScopedB2CLinxMicrovixServices(this IServiceCollection services)
        {
            services.AddScoped<ILinxMicrovixCommandHandler, LinxMicrovixCommandHandler>();
            services.AddScoped<ILinxMicrovixService, LinxMicrovixService>();
            services.AddScoped<IAPICall, APICall>();

            services.AddScoped<IB2CConsultaClientesService, B2CConsultaClientesService>();
            services.AddScoped<IB2CConsultaClientesRepository, B2CConsultaClientesRepository>();
            services.AddScoped<IB2CConsultaClientesCommandHandler, B2CConsultaClientesCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaClientesValidator>();

            services.AddScoped<IB2CConsultaEmpresasService, B2CConsultaEmpresasService>();
            services.AddScoped<IB2CConsultaEmpresasRepository, B2CConsultaEmpresasRepository>();
            services.AddScoped<IB2CConsultaEmpresasCommandHandler, B2CConsultaEmpresasCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaEmpresasValidator>();

            services.AddScoped<IB2CConsultaNFeService, B2CConsultaNFeService>();
            services.AddScoped<IB2CConsultaNFeRepository, B2CConsultaNFeRepository>();
            services.AddScoped<IB2CConsultaNFeCommandHandler, B2CConsultaNFeCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaNFeValidator>();

            services.AddScoped<IB2CConsultaNFeSituacaoService, B2CConsultaNFeSituacaoService>();
            services.AddScoped<IB2CConsultaNFeSituacaoRepository, B2CConsultaNFeSituacaoRepository>();
            services.AddScoped<IB2CConsultaNFeSituacaoCommandHandler, B2CConsultaNFeSituacaoCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaNFeSituacaoValidator>();

            services.AddScoped<IB2CConsultaPedidosService, B2CConsultaPedidosService>();
            services.AddScoped<IB2CConsultaPedidosRepository, B2CConsultaPedidosRepository>();
            services.AddScoped<IB2CConsultaPedidosCommandHandler, B2CConsultaPedidosCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaPedidosValidator>();

            services.AddScoped<IB2CConsultaPedidosIdentificadorService, B2CConsultaPedidosIdentificadorService>();
            services.AddScoped<IB2CConsultaPedidosIdentificadorRepository, B2CConsultaPedidosIdentificadorRepository>();
            services.AddScoped<IB2CConsultaPedidosIdentificadorCommandHandler, B2CConsultaPedidosIdentificadorCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaPedidosIdentificadorValidator>();

            services.AddScoped<IB2CConsultaPedidosItensService, B2CConsultaPedidosItensService>();
            services.AddScoped<IB2CConsultaPedidosItensRepository, B2CConsultaPedidosItensRepository>();
            services.AddScoped<IB2CConsultaPedidosItensCommandHandler, B2CConsultaPedidosItensCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaPedidosItensValidator>();

            services.AddScoped<IB2CConsultaPedidosStatusService, B2CConsultaPedidosStatusService>();
            services.AddScoped<IB2CConsultaPedidosStatusRepository, B2CConsultaPedidosStatusRepository>();
            services.AddScoped<IB2CConsultaPedidosStatusCommandHandler, B2CConsultaPedidosStatusCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaPedidosStatusValidator>();

            services.AddScoped<IB2CConsultaStatusService, B2CConsultaStatusService>();
            services.AddScoped<IB2CConsultaStatusRepository, B2CConsultaStatusRepository>();
            services.AddScoped<IB2CConsultaStatusCommandHandler, B2CConsultaStatusCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<B2CConsultaStatusValidator>();

            return services;
        }

        public static IServiceCollection AddScopedLinxMicrovixServices(this IServiceCollection services)
        {
            services.AddHttpClient("LinxMicrovixAPI", client =>
            {
                client.BaseAddress = new Uri("https://webapi.microvix.com.br/1.0/api/integracao/");
                client.Timeout = new TimeSpan(0, 20, 0);
            });

            services.AddKeyedScoped<IApiClient>("LinxMicrovixAPI", (sp, _) =>
                new ApiClient(
                    sp.GetRequiredService<IHttpClientFactory>(),
                    sp.GetRequiredService<ILogger<ApiClient>>(),
                    "LinxMicrovixAPI"
                )
            );

            services.AddScoped<ILinxMicrovixCommandHandler, LinxMicrovixCommandHandler>();
            services.AddScoped<ILinxMicrovixService, LinxMicrovixService>();
            services.AddScoped<IAPICall, APICall>();

            services.AddScoped<ILinxClientesFornecService, LinxClientesFornecService>();
            services.AddScoped<ILinxClientesFornecRepository, LinxClientesFornecRepository>();
            services.AddScoped<ILinxClientesFornecCommandHandler, LinxClientesFornecCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxClientesFornecValidator>();

            services.AddScoped<ILinxClientesEnderecosEntregaService, LinxClientesEnderecosEntregaService>();
            services.AddScoped<ILinxClientesEnderecosEntregaRepository, LinxClientesEnderecosEntregaRepository>();
            services.AddScoped<ILinxClientesEnderecosEntregaCommandHandler, LinxClientesEnderecosEntregaCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxClientesEnderecosEntregaValidator>();

            services.AddScoped<ILinxMovimentoService, LinxMovimentoService>();
            services.AddScoped<ILinxMovimentoRepository, LinxMovimentoRepository>();
            services.AddScoped<ILinxMovimentoCommandHandler, LinxMovimentoCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxMovimentoValidator>();

            services.AddScoped<ILinxMovimentoCartoesService, LinxMovimentoCartoesService>();
            services.AddScoped<ILinxMovimentoCartoesRepository, LinxMovimentoCartoesRepository>();
            services.AddScoped<ILinxMovimentoCartoesCommandHandler, LinxMovimentoCartoesCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxMovimentoCartoesValidator>();

            services.AddScoped<ILinxMovimentoPlanosService, LinxMovimentoPlanosService>();
            services.AddScoped<ILinxMovimentoPlanosRepository, LinxMovimentoPlanosRepository>();
            services.AddScoped<ILinxMovimentoPlanosCommandHandler, LinxMovimentoPlanosCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxMovimentoPlanosValidator>();

            services.AddScoped<ILinxMovimentoTrocasService, LinxMovimentoTrocasService>();
            services.AddScoped<ILinxMovimentoTrocasRepository, LinxMovimentoTrocasRepository>();
            services.AddScoped<ILinxMovimentoTrocasCommandHandler, LinxMovimentoTrocasCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxMovimentoTrocasValidator>();

            services.AddScoped<ILinxPlanosService, LinxPlanosService>();
            services.AddScoped<ILinxPlanosRepository, LinxPlanosRepository>();
            services.AddScoped<ILinxPlanosCommandHandler, LinxPlanosCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxPlanosValidator>();

            services.AddScoped<ILinxNaturezaOperacaoService, LinxNaturezaOperacaoService>();
            services.AddScoped<ILinxNaturezaOperacaoRepository, LinxNaturezaOperacaoRepository>();
            services.AddScoped<ILinxNaturezaOperacaoCommandHandler, LinxNaturezaOperacaoCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxNaturezaOperacaoValidator>();

            services.AddScoped<ILinxProdutosService, LinxProdutosService>();
            services.AddScoped<ILinxProdutosRepository, LinxProdutosRepository>();
            services.AddScoped<ILinxProdutosCommandHandler, LinxProdutosCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosValidator>();

            services.AddScoped<ILinxProdutosCodBarService, LinxProdutosCodBarService>();
            services.AddScoped<ILinxProdutosCodBarRepository, LinxProdutosCodBarRepository>();
            services.AddScoped<ILinxProdutosCodBarCommandHandler, LinxProdutosCodBarCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosCodBarValidator>();

            services.AddScoped<ILinxProdutosTabelasService, LinxProdutosTabelasService>();
            services.AddScoped<ILinxProdutosTabelasRepository, LinxProdutosTabelasRepository>();
            services.AddScoped<ILinxProdutosTabelasCommandHandler, LinxProdutosTabelasCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosTabelasValidator>();

            services.AddScoped<ILinxProdutosTabelasPrecosService, LinxProdutosTabelasPrecosService>();
            services.AddScoped<ILinxProdutosTabelasPrecosRepository, LinxProdutosTabelasPrecosRepository>();
            services.AddScoped<ILinxProdutosTabelasPrecosCommandHandler, LinxProdutosTabelasPrecosCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosTabelasPrecosValidator>();

            services.AddScoped<ILinxProdutosInventarioService, LinxProdutosInventarioService>();
            services.AddScoped<ILinxProdutosInventarioRepository, LinxProdutosInventarioRepository>();
            services.AddScoped<ILinxProdutosInventarioCommandHandler, LinxProdutosInventarioCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosInventarioValidator>();

            services.AddScoped<ILinxProdutosDetalhesService, LinxProdutosDetalhesService>();
            services.AddScoped<ILinxProdutosDetalhesRepository, LinxProdutosDetalhesRepository>();
            services.AddScoped<ILinxProdutosDetalhesCommandHandler, LinxProdutosDetalhesCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosDetalhesValidator>();

            services.AddScoped<ILinxProdutosDepositosService, LinxProdutosDepositosService>();
            services.AddScoped<ILinxProdutosDepositosRepository, LinxProdutosDepositosRepository>();
            services.AddScoped<ILinxProdutosDepositosCommandHandler, LinxProdutosDepositosCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosDepositosValidator>();

            services.AddScoped<ILinxProdutosPromocoesService, LinxProdutosPromocoesService>();
            services.AddScoped<ILinxProdutosPromocoesRepository, LinxProdutosPromocoesRepository>();
            services.AddScoped<ILinxProdutosPromocoesCommandHandler, LinxProdutosPromocoesCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosPromocoesValidator>();

            services.AddScoped<ILinxProdutosCamposAdicionaisService, LinxProdutosCamposAdicionaisService>();
            services.AddScoped<ILinxProdutosCamposAdicionaisRepository, LinxProdutosCamposAdicionaisRepository>();
            services.AddScoped<ILinxProdutosCamposAdicionaisCommandHandler, LinxProdutosCamposAdicionaisCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxProdutosCamposAdicionaisValidator>();

            services.AddScoped<ILinxPedidosVendaService, LinxPedidosVendaService>();
            services.AddScoped<ILinxPedidosVendaRepository, LinxPedidosVendaRepository>();
            services.AddScoped<ILinxPedidosVendaCommandHandler, LinxPedidosVendaCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxPedidosVendaValidator>();

            services.AddScoped<ILinxPedidosCompraService, LinxPedidosCompraService>();
            services.AddScoped<ILinxPedidosCompraRepository, LinxPedidosCompraRepository>();
            services.AddScoped<ILinxPedidosCompraCommandHandler, LinxPedidosCompraCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxPedidosCompraValidator>();

            services.AddScoped<ILinxGrupoLojasService, LinxGrupoLojasService>();
            services.AddScoped<ILinxGrupoLojasRepository, LinxGrupoLojasRepository>();
            services.AddScoped<ILinxGrupoLojasCommandHandler, LinxGrupoLojasCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxGrupoLojasValidator>();

            services.AddScoped<ILinxLojasService, LinxLojasService>();
            services.AddScoped<ILinxLojasRepository, LinxLojasRepository>();
            services.AddScoped<ILinxLojasCommandHandler, LinxLojasCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxLojasValidator>();

            services.AddScoped<ILinxSetoresService, LinxSetoresService>();
            services.AddScoped<ILinxSetoresRepository, LinxSetoresRepository>();
            services.AddScoped<ILinxSetoresCommandHandler, LinxSetoresCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxSetoresValidator>();

            services.AddScoped<ILinxVendedoresService, LinxVendedoresService>();
            services.AddScoped<ILinxVendedoresRepository, LinxVendedoresRepository>();
            services.AddScoped<ILinxVendedoresCommandHandler, LinxVendedoresCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxVendedoresValidator>();

            services.AddScoped<ILinxUsuariosService, LinxUsuariosService>();
            services.AddScoped<ILinxUsuariosRepository, LinxUsuariosRepository>();
            services.AddScoped<ILinxUsuariosCommandHandler, LinxUsuariosCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxUsuariosValidator>();

            services.AddScoped<ILinxCfopFiscalService, LinxCfopFiscalService>();
            services.AddScoped<ILinxCfopFiscalRepository, LinxCfopFiscalRepository>();
            services.AddScoped<ILinxCfopFiscalCommandHandler, LinxCfopFiscalCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxCfopFiscalValidator>();

            services.AddScoped<ILinxRotinaOrigemService, LinxRotinaOrigemService>();
            services.AddScoped<ILinxRotinaOrigemRepository, LinxRotinaOrigemRepository>();
            services.AddScoped<ILinxRotinaOrigemCommandHandler, LinxRotinaOrigemCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxRotinaOrigemValidator>();

            services.AddScoped<ILinxXMLDocumentosService, LinxXMLDocumentosService>();
            services.AddScoped<ILinxXMLDocumentosRepository, LinxXMLDocumentosRepository>();
            services.AddScoped<ILinxXMLDocumentosCommandHandler, LinxXMLDocumentosCommandHandler>();
            services.AddValidatorsFromAssemblyContaining<LinxXMLDocumentosValidator>();

            // LinxReshopExcel (migrado de newbloomers-workers)
            services.AddScoped<ILinxReshopSaldoService, LinxReshopSaldoService>();
            services.AddScoped<ILinxReshopSaldoRepository, LinxReshopSaldoRepository>();

            return services;
        }
    }
}
