INSERT INTO [auditing].[Catalog] (IdParent, IdJob, Title, [Description], IdType, IsOnDemand)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (pedidos de compra: 13556 - 13558)*/
(13004, 13556, 'sp_TotvsPurchaseOrders_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrders raw e trusted', 1, 0),
(13004, 13557, 'sp_TotvsPurchaseOrdersProducts_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrdersProducts raw e trusted', 1, 0),
(13004, 13558, 'sp_TotvsPurchaseOrdersRequests_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrdersRequests raw e trusted', 1, 0),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13004, 'TotvsPostSearchPurchaseOrders', 'Integração dos pedidos de compra de todas as lojas (TotvsPurchaseOrders e filhas), a cada 3 minutos', 2, 0),

/*Leituras sob demanda da onda 2 - executadas pelo TotvsController, não são recorrentes (IsOnDemand = 1). Só o
  GetPurchaseOrderByKey grava (como o GetSalesOrderByKey); as outras devolvem o JSON e registram a chamada em totvs.TotvsRequests
  (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Person: 13030 - 13069; Product: 13100 - 13105; Purchase Order: 13185; Sales Order: 13190 - 13249)*/
(130, 13185, 'TotvsGetPurchaseOrderByKey', 'Integração de um pedido de compra por loja e número (GET PurchaseOrders/{docCompany}/{branchCode}/{orderCode})', 2, 1),

/*Escritas da API Purchase Order - executadas pelo TotvsController, não são recorrentes (IsOnDemand = 1); só rodam com
  Totvs:Writes:PurchaseOrders:Enabled = true (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.3, fase PO-2)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13182, 'TotvsPostPurchaseOrdersChangeStatus', 'Troca da situação de um pedido de compra (POST PurchaseOrders/{docCompany}/ChangeStatus)', 2, 1),

/*Escritas de documento da API Purchase Order - executadas pelo TotvsController, não são recorrentes (IsOnDemand = 1); só rodam com
  Totvs:Writes:PurchaseOrders:Enabled = true (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.1, §6.2 e §6.5, fase PO-3)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13180, 'TotvsPostPurchaseOrders', 'Inclusão de pedido de compra (POST PurchaseOrders/{docCompany})', 2, 1),
(130, 13181, 'TotvsPostPurchaseOrdersQuantityItems', 'Quantidade pedida dos itens de um pedido de compra (POST PurchaseOrders/{docCompany}/QuantityItems)', 2, 1),
(130, 13184, 'TotvsPostPurchaseOrderBatchLink', 'Vínculo de item de lote com pedido de compra (POST PurchaseOrders/{docCompany}/BatchLink)', 2, 1),

/*Cancelamento de pedido de compra (irreversível) - executado pelo TotvsController, não é recorrente (IsOnDemand = 1); só roda com
  Totvs:Writes:PurchaseOrders:Enabled = true (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.4, fase PO-4)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13183, 'TotvsPostPurchaseOrdersCancel', 'Cancelamento de pedido de compra (POST PurchaseOrders/{docCompany}/Cancel)', 2, 1);
