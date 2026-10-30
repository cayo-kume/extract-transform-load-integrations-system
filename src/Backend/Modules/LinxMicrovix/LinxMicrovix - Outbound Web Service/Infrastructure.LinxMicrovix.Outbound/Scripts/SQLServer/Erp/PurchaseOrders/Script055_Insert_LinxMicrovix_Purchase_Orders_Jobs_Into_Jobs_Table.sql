INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (pedidos de compra: 8652 - 8654)*/
(8152, 8652, 'sp_LinxPedidosCompra_Sync', 'Procedure de sincronização entre as tabelas LinxPedidosCompra raw e trusted', 3, NULL, 'default', 1),
(8153, 8653, 'sp_LinxPedidosCompraStatus_Sync', 'Procedure de sincronização entre as tabelas LinxPedidosCompraStatus raw e trusted', 3, NULL, 'default', 1),
(8154, 8654, 'sp_LinxPedidosCompraNumeroFornecedor_Sync', 'Procedure de sincronização entre as tabelas LinxPedidosCompraNumeroFornecedor raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8152, 'LinxPedidosCompra', 'Integração de dados da tabela LinxPedidosCompra, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8153, 'LinxPedidosCompraStatus', 'Integração de dados da tabela LinxPedidosCompraStatus, a cada 3 minutos', 3, NULL, 'default', 2),
(80, 8154, 'LinxPedidosCompraNumeroFornecedor', 'Integração de dados da tabela LinxPedidosCompraNumeroFornecedor, a cada 3 minutos', 3, NULL, 'default', 2);
