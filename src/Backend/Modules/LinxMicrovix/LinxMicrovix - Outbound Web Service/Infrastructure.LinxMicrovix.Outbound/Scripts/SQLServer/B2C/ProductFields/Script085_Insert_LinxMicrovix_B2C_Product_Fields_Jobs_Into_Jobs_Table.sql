INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (campos adicionais de produto do b2c: 8721 - 8728)*/
(8280, 8721, 'sp_B2CConsultaProdutosCamposAdicionaisDetalhes_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosCamposAdicionaisDetalhes raw e trusted', 120, 1),
(8281, 8722, 'sp_B2CConsultaProdutosCamposAdicionaisNomes_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosCamposAdicionaisNomes raw e trusted', 120, 1),
(8282, 8723, 'sp_B2CConsultaProdutosCamposAdicionaisValores_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosCamposAdicionaisValores raw e trusted', 120, 1),
(8283, 8724, 'sp_B2CConsultaB2CProdutosCamposAdicionais_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaB2CProdutosCamposAdicionais raw e trusted', 120, 1),
(8284, 8725, 'sp_B2CConsultaB2CProdutosCamposAdicionaisDados_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaB2CProdutosCamposAdicionaisDados raw e trusted', 120, 1),
(8285, 8726, 'sp_B2CConsultaB2CProdutosCamposAdicionaisMedia_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaB2CProdutosCamposAdicionaisMedia raw e trusted', 120, 1),
(8286, 8727, 'sp_B2CConsultaB2CProdutosCamposAdicionaisMediaDados_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaB2CProdutosCamposAdicionaisMediaDados raw e trusted', 120, 1),
(8287, 8728, 'sp_B2CConsultaB2CProdutosCamposAdicionaisOpcoes_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaB2CProdutosCamposAdicionaisOpcoes raw e trusted', 120, 1),

/*Integrações*/
/*Range 8250 - 8499 (B2C; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8280, 'B2CConsultaProdutosCamposAdicionaisDetalhes', 'Integração de dados da tabela B2CConsultaProdutosCamposAdicionaisDetalhes, a cada 2 horas', 120, 2),
(80, 8281, 'B2CConsultaProdutosCamposAdicionaisNomes', 'Integração de dados da tabela B2CConsultaProdutosCamposAdicionaisNomes, a cada 2 horas', 120, 2),
(80, 8282, 'B2CConsultaProdutosCamposAdicionaisValores', 'Integração de dados da tabela B2CConsultaProdutosCamposAdicionaisValores, a cada 2 horas', 120, 2),
(80, 8283, 'B2CConsultaB2CProdutosCamposAdicionais', 'Integração de dados da tabela B2CConsultaB2CProdutosCamposAdicionais, a cada 2 horas', 120, 2),
(80, 8284, 'B2CConsultaB2CProdutosCamposAdicionaisDados', 'Integração de dados da tabela B2CConsultaB2CProdutosCamposAdicionaisDados, a cada 2 horas', 120, 2),
(80, 8285, 'B2CConsultaB2CProdutosCamposAdicionaisMedia', 'Integração de dados da tabela B2CConsultaB2CProdutosCamposAdicionaisMedia, a cada 2 horas', 120, 2),
(80, 8286, 'B2CConsultaB2CProdutosCamposAdicionaisMediaDados', 'Integração de dados da tabela B2CConsultaB2CProdutosCamposAdicionaisMediaDados, a cada 2 horas', 120, 2),
(80, 8287, 'B2CConsultaB2CProdutosCamposAdicionaisOpcoes', 'Integração de dados da tabela B2CConsultaB2CProdutosCamposAdicionaisOpcoes, a cada 2 horas', 120, 2);
