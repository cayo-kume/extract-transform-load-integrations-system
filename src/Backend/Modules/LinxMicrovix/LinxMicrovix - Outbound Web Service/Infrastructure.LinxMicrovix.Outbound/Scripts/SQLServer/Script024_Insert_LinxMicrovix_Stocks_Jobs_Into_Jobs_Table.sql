INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (estoque e balanços: 8544 - 8552)*/
(8044, 8544, 'sp_LinxProdutosDetalhes_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosDetalhes raw e trusted', 120, 1),
(8045, 8545, 'sp_LinxProdutosDetalhesSimplificado_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosDetalhesSimplificado raw e trusted', 120, 1),
(8046, 8546, 'sp_LinxProdutosDetalhesDepositos_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosDetalhesDepositos raw e trusted', 120, 1),
(8047, 8547, 'sp_LinxProdutosDepositos_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosDepositos raw e trusted', 120, 1),
(8048, 8548, 'sp_LinxProdutosInventario_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosInventario raw e trusted', 120, 1),
(8049, 8549, 'sp_LinxListagemBalanco_Sync', 'Procedure de sincronização entre as tabelas LinxListagemBalanco raw e trusted', 120, 1),
(8050, 8550, 'sp_LinxBalancos_Sync', 'Procedure de sincronização entre as tabelas LinxBalancos raw e trusted', 120, 1),
(8051, 8551, 'sp_LinxBalancosArquivos_Sync', 'Procedure de sincronização entre as tabelas LinxBalancosArquivos raw e trusted', 120, 1),
(8052, 8552, 'sp_LinxBalancosSecoes_Sync', 'Procedure de sincronização entre as tabelas LinxBalancosSecoes raw e trusted', 120, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8044, 'LinxProdutosDetalhes', 'Integração de dados da tabela LinxProdutosDetalhes, a cada 2 horas', 120, 2),
(80, 8045, 'LinxProdutosDetalhesSimplificado', 'Integração de dados da tabela LinxProdutosDetalhesSimplificado, a cada 2 horas', 120, 2),
(80, 8046, 'LinxProdutosDetalhesDepositos', 'Integração de dados da tabela LinxProdutosDetalhesDepositos, a cada 2 horas', 120, 2),
(80, 8047, 'LinxProdutosDepositos', 'Integração de dados da tabela LinxProdutosDepositos, a cada 2 horas', 120, 2),
(80, 8048, 'LinxProdutosInventario', 'Integração de dados da tabela LinxProdutosInventario, a cada 2 horas', 120, 2),
(80, 8049, 'LinxListagemBalanco', 'Integração de dados da tabela LinxListagemBalanco, a cada 2 horas', 120, 2),
(80, 8050, 'LinxBalancos', 'Integração de dados da tabela LinxBalancos, a cada 2 horas', 120, 2),
(80, 8051, 'LinxBalancosArquivos', 'Integração de dados da tabela LinxBalancosArquivos, a cada 2 horas', 120, 2),
(80, 8052, 'LinxBalancosSecoes', 'Integração de dados da tabela LinxBalancosSecoes, a cada 2 horas', 120, 2);
