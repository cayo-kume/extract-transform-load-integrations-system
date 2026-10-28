INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas de dados fiscais, preço, custo e promoção da API Product (efeito comercial e fiscal) - executadas pelo TotvsController, não são
  recorrentes (IntervalTimeInMinutes = 0); só rodam com Totvs:Writes:Products:Enabled = true e, nas de valor, com os códigos e lojas permitidos
  (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.6 e §5.7, fase PR-6)*/
/*Range 13000 - 13499 (Product: 13100 - 13179)*/
(130, 13128, 'TotvsPutProducts', 'Dados fiscais e indicadores de um produto numa empresa (PUT Products/{docCompany}/{code}/{branchCode})', 0, 2),
(130, 13129, 'TotvsPutBranchInfo', 'Indicadores dos produtos numa empresa (PUT Products/{docCompany}/BranchInfo/{branchCode})', 0, 2),
(130, 13130, 'TotvsPutData', 'Dados fiscais dos produtos (PUT Products/{docCompany}/Data)', 0, 2),
(130, 13131, 'TotvsPostCreateValues', 'Inclusão de preço ou custo (POST Products/{docCompany}/Values/Create)', 0, 2),
(130, 13132, 'TotvsPostUpdateValues', 'Alteração de preço ou custo (POST Products/{docCompany}/Values/Update)', 0, 2),
(130, 13133, 'TotvsPostUpdatePromotionValues', 'Alteração de valores de promoção (POST Products/{docCompany}/PromotionValues/Update)', 0, 2),
(130, 13134, 'TotvsPostPromotionItems', 'Inclusão de produtos numa promoção (POST Products/{docCompany}/PromotionItems)', 0, 2);
