INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas de cadastro da API Product - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:Products:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.5, §5.6 e §5.8, fase PR-5)*/
/*Range 13000 - 13499 (Product: 13100 - 13179)*/
(130, 13120, 'TotvsPostBarcodes', 'Inclusão de códigos de barras de produtos (POST Products/{docCompany}/BarCodes)', 0, 2),
(130, 13121, 'TotvsPostUpdateBarcodes', 'Alteração de códigos de barras de produtos (POST Products/{docCompany}/BarCodes/Update)', 0, 2),
(130, 13122, 'TotvsPostCreateClassificationRelationship', 'Inclusão de classificações de produtos (POST Products/{docCompany}/ClassificationRelationships/Create)', 0, 2),
(130, 13123, 'TotvsPostUpdateClassificationRelationship', 'Alteração de classificações de produtos (POST Products/{docCompany}/ClassificationRelationships/Update)', 0, 2),
(130, 13125, 'TotvsPostClassifications', 'Inclusão ou alteração de tipo de classificação de produto (POST Products/{docCompany}/Classifications)', 0, 2),
(130, 13127, 'TotvsPostReferences', 'Inclusão de referência com os SKUs (POST Products/{docCompany}/References)', 0, 2),
(130, 13135, 'TotvsPostCreateBatch', 'Inclusão de lotes e itens de lote (POST Products/{docCompany}/Batches/Create)', 0, 2),
(130, 13136, 'TotvsPutBatchInfo', 'Alteração de itens de lote (PUT Products/{docCompany}/BatchInfo)', 0, 2),
(130, 13137, 'TotvsPostBatchPreferentialSalesOrder', 'Pedidos de venda preferenciais de um item de lote (POST Products/{docCompany}/BatchPreferentialSalesOrder)', 0, 2);
