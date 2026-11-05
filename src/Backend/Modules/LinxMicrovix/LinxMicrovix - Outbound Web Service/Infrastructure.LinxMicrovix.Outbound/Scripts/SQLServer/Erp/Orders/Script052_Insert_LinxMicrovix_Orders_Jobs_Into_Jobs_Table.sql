INSERT INTO [auditing].[Catalog] (IdParent, IdJob, Title, [Description], IdType, IsOnDemand)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (pedidos de venda: 8642 - 8651)*/
(8142, 8642, 'sp_LinxPedidosVenda_Sync', 'Procedure de sincronização entre as tabelas LinxPedidosVenda raw e trusted', 1, 0),
(8143, 8643, 'sp_LinxPlanosPedidoVenda_Sync', 'Procedure de sincronização entre as tabelas LinxPlanosPedidoVenda raw e trusted', 1, 0),
(8144, 8644, 'sp_LinxPedidosVendaChecklistEntregaLocal_Sync', 'Procedure de sincronização entre as tabelas LinxPedidosVendaChecklistEntregaLocal raw e trusted', 1, 0),
(8145, 8645, 'sp_LinxPedidosVendaChecklistEntregaArmazenamento_Sync', 'Procedure de sincronização entre as tabelas LinxPedidosVendaChecklistEntregaArmazenamento raw e trusted', 1, 0),
(8146, 8646, 'sp_LinxPedidosVendaChecklistEntregaDificuldade_Sync', 'Procedure de sincronização entre as tabelas LinxPedidosVendaChecklistEntregaDificuldade raw e trusted', 1, 0),
(8147, 8647, 'sp_LinxOrcamentoPosicao_Sync', 'Procedure de sincronização entre as tabelas LinxOrcamentoPosicao raw e trusted', 1, 0),
(8148, 8648, 'sp_LinxB2CPedidos_Sync', 'Procedure de sincronização entre as tabelas LinxB2CPedidos raw e trusted', 1, 0),
(8149, 8649, 'sp_LinxB2CPedidosItens_Sync', 'Procedure de sincronização entre as tabelas LinxB2CPedidosItens raw e trusted', 1, 0),
(8150, 8650, 'sp_LinxB2CPedidosStatus_Sync', 'Procedure de sincronização entre as tabelas LinxB2CPedidosStatus raw e trusted', 1, 0),
(8151, 8651, 'sp_LinxB2CStatus_Sync', 'Procedure de sincronização entre as tabelas LinxB2CStatus raw e trusted', 1, 0),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8142, 'LinxPedidosVenda', 'Integração de dados da tabela LinxPedidosVenda, a cada 3 minutos', 2, 0),
(80, 8143, 'LinxPlanosPedidoVenda', 'Integração de dados da tabela LinxPlanosPedidoVenda, a cada 3 minutos', 2, 0),
(80, 8144, 'LinxPedidosVendaChecklistEntregaLocal', 'Integração de dados da tabela LinxPedidosVendaChecklistEntregaLocal, a cada 3 minutos', 2, 0),
(80, 8145, 'LinxPedidosVendaChecklistEntregaArmazenamento', 'Integração de dados da tabela LinxPedidosVendaChecklistEntregaArmazenamento, a cada 3 minutos', 2, 0),
(80, 8146, 'LinxPedidosVendaChecklistEntregaDificuldade', 'Integração de dados da tabela LinxPedidosVendaChecklistEntregaDificuldade, a cada 3 minutos', 2, 0),
(80, 8147, 'LinxOrcamentoPosicao', 'Integração de dados da tabela LinxOrcamentoPosicao, a cada 3 minutos', 2, 0),
(80, 8148, 'LinxB2CPedidos', 'Integração de dados da tabela LinxB2CPedidos, a cada 3 minutos', 2, 0),
(80, 8149, 'LinxB2CPedidosItens', 'Integração de dados da tabela LinxB2CPedidosItens, a cada 3 minutos', 2, 0),
(80, 8150, 'LinxB2CPedidosStatus', 'Integração de dados da tabela LinxB2CPedidosStatus, a cada 3 minutos', 2, 0),
(80, 8151, 'LinxB2CStatus', 'Integração de dados da tabela LinxB2CStatus, a cada 3 minutos', 2, 0),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IsOnDemand = 1)*/
(80, 8206, 'LinxPedidosVendaByCodPedido', 'Integração de LinxPedidosVenda por codPedido (GET LinxPedidosVenda/{docCompany}/{cnpjEmp}/{codPedido})', 2, 1);
