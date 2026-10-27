INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (recepção de venda: 13600 - 13601)*/
(13031, 13600, 'sp_TotvsReceptionSales_Sync', 'Procedure de sincronização entre as tabelas TotvsReceptionSales raw e trusted', 3, 1),
(13031, 13601, 'sp_TotvsReceptionSalesDocumentTypes_Sync', 'Procedure de sincronização entre as tabelas TotvsReceptionSalesDocumentTypes raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (Person: 13030 - 13069, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13031, 'TotvsGetSearchReceptionSale', 'Integração das recepções de venda (TotvsReceptionSales e formas de pagamento), a cada 3 minutos', 3, 2);
