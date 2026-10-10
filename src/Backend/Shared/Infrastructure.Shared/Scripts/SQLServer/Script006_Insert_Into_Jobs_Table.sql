--INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
--VALUES
/*Procedures de Sincronizações*/

/*Integrações*/

--(40, 1500, 'P_server_maintenance', 'Atualizações periodicas do servidor', 3, 1),
--(40, 1501, 'P_index_maintenance', 'Atualização periodica de índices', 3, 1),
--(40, 1502, 'P_statistics_maintenance', 'Atualização periodica de estatísticas', 3, 1)
--(13, 410,'CheckIntegration', 'Procedure recorrente que indica se a integração rodou para todos os registros', 3, 1),

--(13, 400,'P_', 'Procedures executadas no schema general', 3, 1),

--(13, 401,'P_Volo_Integracao_Clientes', 'Procedure de exportação de clientes para Volo', 3, 1),
--(13, 402,'P_Volo_Integracao_Pedidos_Ecommerce', 'Procedure de exportação de pedidos e-commerce para Volo', 3, 1),
--(13, 403,'P_Volo_Integracao_Pedidos_Venda_Direta', 'Procedure de exportação de pedidos Venda Direta para Volo', 3, 1),
--(13, 404,'P_Volo_Integracao_Pedidos_Venda_Facil', 'Procedure de exportação de pedidos Venda Facil para Volo', 3, 1),
--(13, 405,'P_Volo_Integracao_Interfaces_Pedidos', 'Procedure de exportação de pedidos para Volo', 3, 1),
--(13, 406,'P_Volo_Integracao_Pedidos_Sincronizacao', 'Procedure de sincronização de pedidos Volo', 3, 1),

--(13, 432,'P_Volo_Integracao_Pedidos_Reversas', 'Procedure de exportação de Reversas para Volo', 3, 1),
--(13, 433,'P_Volo_Integracao_Pedidos_Recebimentos', 'Procedure de exportação de Recebimentos para Volo', 3, 1),

--(13, 450,'P_It4_Wms_Documento_Sincronizacao', 'Procedure de sicronização entre as tabelas It4_Wms_Documento raw e trusted', 3, 1),
--(13, 451,'P_It4_Wms_Documento_Item_Sincronizacao', 'Procedure de sicronização entre as tabelas It4_Wms_Documento_Item raw e trusted', 3, 1),

--(14, 500,'P_', 'Procedures executadas no schema operations', 3, 1),
--(15, 600,'P_', 'Procedures executadas no schema crm', 3, 1),

--(13, 422,'P_WebAppImpressaoRomaneioEntregaTransportadora_Sincronizacao', 'Procedure de sicronização entre as tabelas WebAppImpressaoRomaneioEntregaTransportadora raw e trusted', 3, 1),

--(16, 1600, 'P_DeliveryLists_Sincronizacao', 'Procedure de sicronização entre as tabelas DeliveryLists raw e trusted', 3, 1),
--(16, 1601, 'P_DeliveryListsOrders_Sincronizacao', 'Procedure de sicronização entre as tabelas DeliveryListsOrders raw e trusted', 3, 1),