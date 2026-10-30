INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (pedidos de compra: 13556 - 13558)*/
(13004, 13556, 'sp_TotvsPurchaseOrders_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrders raw e trusted', 3, 1),
(13004, 13557, 'sp_TotvsPurchaseOrdersProducts_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrdersProducts raw e trusted', 3, 1),
(13004, 13558, 'sp_TotvsPurchaseOrdersRequests_Sync', 'Procedure de sincronização entre as tabelas TotvsPurchaseOrdersRequests raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13004, 'TotvsPostSearchPurchaseOrders', 'Integração dos pedidos de compra de todas as lojas (TotvsPurchaseOrders e filhas), a cada 3 minutos', 3, 2),

/*Leituras sob demanda da onda 2 - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0). Só o
  GetPurchaseOrderByKey grava (como o GetSalesOrderByKey); as outras devolvem o JSON e registram a chamada em totvs.TotvsRequests
  (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Person: 13030 - 13069; Product: 13100 - 13105; Purchase Order: 13185; Sales Order: 13190 - 13249)*/
(130, 13185, 'TotvsGetPurchaseOrderByKey', 'Integração de um pedido de compra por loja e número (GET PurchaseOrders/{docCompany}/{branchCode}/{orderCode})', 0, 2),

/*Escritas da API Purchase Order - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:PurchaseOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.3, fase PO-2)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13182, 'TotvsPostPurchaseOrdersChangeStatus', 'Troca da situação de um pedido de compra (POST PurchaseOrders/{docCompany}/ChangeStatus)', 0, 2),

/*Escritas de documento da API Purchase Order - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:PurchaseOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.1, §6.2 e §6.5, fase PO-3)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13180, 'TotvsPostPurchaseOrders', 'Inclusão de pedido de compra (POST PurchaseOrders/{docCompany})', 0, 2),
(130, 13181, 'TotvsPostPurchaseOrdersQuantityItems', 'Quantidade pedida dos itens de um pedido de compra (POST PurchaseOrders/{docCompany}/QuantityItems)', 0, 2),
(130, 13184, 'TotvsPostPurchaseOrderBatchLink', 'Vínculo de item de lote com pedido de compra (POST PurchaseOrders/{docCompany}/BatchLink)', 0, 2),

/*Cancelamento de pedido de compra (irreversível) - executado pelo TotvsController, não é recorrente (IntervalTimeInMinutes = 0); só roda com
  Totvs:Writes:PurchaseOrders:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §6.4, fase PO-4)*/
/*Range 13000 - 13499 (Purchase Order: 13180 - 13189)*/
(130, 13183, 'TotvsPostPurchaseOrdersCancel', 'Cancelamento de pedido de compra (POST PurchaseOrders/{docCompany}/Cancel)', 0, 2);
