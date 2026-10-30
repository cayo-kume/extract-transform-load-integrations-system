INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (empresas, vendedores, transportadores e planos do b2c: 8691 - 8699)*/
(8250, 8691, 'sp_B2CConsultaEmpresas_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaEmpresas raw e trusted', 120, 1),
(8251, 8692, 'sp_B2CConsultaCNPJsChave_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaCNPJsChave raw e trusted', 120, 1),
(8252, 8693, 'sp_B2CConsultaVendedores_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaVendedores raw e trusted', 120, 1),
(8253, 8694, 'sp_B2CConsultaTransportadores_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaTransportadores raw e trusted', 120, 1),
(8254, 8695, 'sp_B2CConsultaFormasPagamento_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaFormasPagamento raw e trusted', 120, 1),
(8255, 8696, 'sp_B2CConsultaPlanos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPlanos raw e trusted', 120, 1),
(8256, 8697, 'sp_B2CConsultaPlanosParcelas_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPlanosParcelas raw e trusted', 120, 1),
(8257, 8698, 'sp_B2CConsultaTiposCobrancaFrete_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaTiposCobrancaFrete raw e trusted', 120, 1),
(8258, 8699, 'sp_B2CConsultaTipoEncomenda_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaTipoEncomenda raw e trusted', 120, 1),

/*Integrações*/
/*Range 8250 - 8499 (B2C; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8250, 'B2CConsultaEmpresas', 'Integração de dados da tabela B2CConsultaEmpresas, a cada 2 horas', 120, 2),
(80, 8251, 'B2CConsultaCNPJsChave', 'Integração de dados da tabela B2CConsultaCNPJsChave, a cada 2 horas', 120, 2),
(80, 8252, 'B2CConsultaVendedores', 'Integração de dados da tabela B2CConsultaVendedores, a cada 2 horas', 120, 2),
(80, 8253, 'B2CConsultaTransportadores', 'Integração de dados da tabela B2CConsultaTransportadores, a cada 2 horas', 120, 2),
(80, 8254, 'B2CConsultaFormasPagamento', 'Integração de dados da tabela B2CConsultaFormasPagamento, a cada 2 horas', 120, 2),
(80, 8255, 'B2CConsultaPlanos', 'Integração de dados da tabela B2CConsultaPlanos, a cada 2 horas', 120, 2),
(80, 8256, 'B2CConsultaPlanosParcelas', 'Integração de dados da tabela B2CConsultaPlanosParcelas, a cada 2 horas', 120, 2),
(80, 8257, 'B2CConsultaTiposCobrancaFrete', 'Integração de dados da tabela B2CConsultaTiposCobrancaFrete, a cada 2 horas', 120, 2),
(80, 8258, 'B2CConsultaTipoEncomenda', 'Integração de dados da tabela B2CConsultaTipoEncomenda, a cada 2 horas', 120, 2);
