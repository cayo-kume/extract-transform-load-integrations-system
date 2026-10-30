INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (produtos do b2c: 8707 - 8716)*/
(8266, 8707, 'sp_B2CConsultaProdutos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutos raw e trusted', 120, 1),
(8267, 8708, 'sp_B2CConsultaProdutosAssociados_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosAssociados raw e trusted', 120, 1),
(8268, 8709, 'sp_B2CConsultaProdutosCampanhas_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosCampanhas raw e trusted', 120, 1),
(8269, 8710, 'sp_B2CConsultaProdutosCodebar_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosCodebar raw e trusted', 120, 1),
(8270, 8711, 'sp_B2CConsultaProdutosDimensoes_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosDimensoes raw e trusted', 120, 1),
(8271, 8712, 'sp_B2CConsultaProdutosInformacoes_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosInformacoes raw e trusted', 120, 1),
(8272, 8713, 'sp_B2CConsultaProdutosStatus_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosStatus raw e trusted', 120, 1),
(8273, 8714, 'sp_B2CConsultaProdutosFlags_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosFlags raw e trusted', 120, 1),
(8274, 8715, 'sp_B2CConsultaProdutosTags_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosTags raw e trusted', 120, 1),
(8275, 8716, 'sp_B2CConsultaProdutosPalavrasChavePesquisa_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosPalavrasChavePesquisa raw e trusted', 120, 1),

/*Integrações*/
/*Range 8250 - 8499 (B2C; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8266, 'B2CConsultaProdutos', 'Integração de dados da tabela B2CConsultaProdutos, a cada 2 horas', 120, 2),
(80, 8267, 'B2CConsultaProdutosAssociados', 'Integração de dados da tabela B2CConsultaProdutosAssociados, a cada 2 horas', 120, 2),
(80, 8268, 'B2CConsultaProdutosCampanhas', 'Integração de dados da tabela B2CConsultaProdutosCampanhas, a cada 2 horas', 120, 2),
(80, 8269, 'B2CConsultaProdutosCodebar', 'Integração de dados da tabela B2CConsultaProdutosCodebar, a cada 2 horas', 120, 2),
(80, 8270, 'B2CConsultaProdutosDimensoes', 'Integração de dados da tabela B2CConsultaProdutosDimensoes, a cada 2 horas', 120, 2),
(80, 8271, 'B2CConsultaProdutosInformacoes', 'Integração de dados da tabela B2CConsultaProdutosInformacoes, a cada 2 horas', 120, 2),
(80, 8272, 'B2CConsultaProdutosStatus', 'Integração de dados da tabela B2CConsultaProdutosStatus, a cada 2 horas', 120, 2),
(80, 8273, 'B2CConsultaProdutosFlags', 'Integração de dados da tabela B2CConsultaProdutosFlags, a cada 2 horas', 120, 2),
(80, 8274, 'B2CConsultaProdutosTags', 'Integração de dados da tabela B2CConsultaProdutosTags, a cada 2 horas', 120, 2),
(80, 8275, 'B2CConsultaProdutosPalavrasChavePesquisa', 'Integração de dados da tabela B2CConsultaProdutosPalavrasChavePesquisa, a cada 2 horas', 120, 2);
