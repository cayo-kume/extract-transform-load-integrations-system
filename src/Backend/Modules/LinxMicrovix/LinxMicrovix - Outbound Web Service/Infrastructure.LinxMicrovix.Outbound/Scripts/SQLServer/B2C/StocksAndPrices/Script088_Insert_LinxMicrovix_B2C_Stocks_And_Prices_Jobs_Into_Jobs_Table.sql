INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (estoque, custos e preços do b2c: 8743 - 8749)*/
(8302, 8743, 'sp_B2CConsultaProdutosCustos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosCustos raw e trusted', 120, 1),
(8303, 8744, 'sp_B2CConsultaProdutosDepositos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosDepositos raw e trusted', 120, 1),
(8304, 8745, 'sp_B2CConsultaProdutosDetalhes_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosDetalhes raw e trusted', 120, 1),
(8305, 8746, 'sp_B2CConsultaProdutosDetalhesDepositos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosDetalhesDepositos raw e trusted', 120, 1),
(8306, 8747, 'sp_B2CConsultaProdutosPromocao_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosPromocao raw e trusted', 120, 1),
(8307, 8748, 'sp_B2CConsultaProdutosTabelas_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosTabelas raw e trusted', 120, 1),
(8308, 8749, 'sp_B2CConsultaProdutosTabelasPrecos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosTabelasPrecos raw e trusted', 120, 1),

/*Integrações*/
/*Range 8250 - 8499 (B2C; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8302, 'B2CConsultaProdutosCustos', 'Integração de dados da tabela B2CConsultaProdutosCustos, a cada 2 horas', 120, 2),
(80, 8303, 'B2CConsultaProdutosDepositos', 'Integração de dados da tabela B2CConsultaProdutosDepositos, a cada 2 horas', 120, 2),
(80, 8304, 'B2CConsultaProdutosDetalhes', 'Integração de dados da tabela B2CConsultaProdutosDetalhes, a cada 2 horas', 120, 2),
(80, 8305, 'B2CConsultaProdutosDetalhesDepositos', 'Integração de dados da tabela B2CConsultaProdutosDetalhesDepositos, a cada 2 horas', 120, 2),
(80, 8306, 'B2CConsultaProdutosPromocao', 'Integração de dados da tabela B2CConsultaProdutosPromocao, a cada 2 horas', 120, 2),
(80, 8307, 'B2CConsultaProdutosTabelas', 'Integração de dados da tabela B2CConsultaProdutosTabelas, a cada 2 horas', 120, 2),
(80, 8308, 'B2CConsultaProdutosTabelasPrecos', 'Integração de dados da tabela B2CConsultaProdutosTabelasPrecos, a cada 2 horas', 120, 2);
