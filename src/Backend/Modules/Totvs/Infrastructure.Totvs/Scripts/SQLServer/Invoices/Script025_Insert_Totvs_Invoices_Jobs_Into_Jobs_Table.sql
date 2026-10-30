INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (notas fiscais: 13575 - 13584)*/
(13003, 13575, 'sp_TotvsInvoices_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoices raw e trusted', 3, 1),
(13003, 13576, 'sp_TotvsInvoicesItems_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItems raw e trusted', 3, 1),
(13003, 13577, 'sp_TotvsInvoicesItemsProducts_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItemsProducts raw e trusted', 3, 1),
(13003, 13578, 'sp_TotvsInvoicesItemsProductsReferenced_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItemsProductsReferenced raw e trusted', 3, 1),
(13003, 13579, 'sp_TotvsInvoicesItemsTaxes_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItemsTaxes raw e trusted', 3, 1),
(13003, 13580, 'sp_TotvsInvoicesItemsBarCodes_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesItemsBarCodes raw e trusted', 3, 1),
(13003, 13581, 'sp_TotvsInvoicesPayments_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesPayments raw e trusted', 3, 1),
(13003, 13582, 'sp_TotvsInvoicesSalesOrders_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesSalesOrders raw e trusted', 3, 1),
(13003, 13583, 'sp_TotvsInvoicesObservationsNF_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesObservationsNF raw e trusted', 3, 1),
(13003, 13584, 'sp_TotvsInvoicesReferencedTaxes_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesReferencedTaxes raw e trusted', 3, 1),

/*Integrações*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7)*/
(130, 13003, 'TotvsPostSearchInvoices', 'Integração das notas fiscais de todas as lojas (TotvsInvoices e filhas), a cada 3 minutos', 3, 2),

/*Integrações sob demanda - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7; sob demanda a partir de 13010)*/
(130, 13013, 'TotvsGetInvoiceByKey', 'Integração de uma nota fiscal por loja e sequência (GET Invoices/{docCompany}/{branchCode}/{invoiceSequence})', 0, 2),

/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (inutilizações e centros de custo: 13648 - 13650)*/
(13250, 13648, 'sp_TotvsInvoicesDisable_Sync', 'Procedure de sincronização entre as tabelas TotvsInvoicesDisable raw e trusted', 120, 1),
(13251, 13649, 'sp_TotvsCostCenters_Sync', 'Procedure de sincronização entre as tabelas TotvsCostCenters raw e trusted', 120, 1),
(13251, 13650, 'sp_TotvsCostCentersClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsCostCentersClassifications raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Fiscal: 13250 - 13299, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13250, 'TotvsGetInvoicesDisable', 'Integração das faixas de numeração inutilizadas de todas as lojas (TotvsInvoicesDisable), a cada 2 horas', 120, 2),
(130, 13251, 'TotvsGetCostCenter', 'Integração dos centros de custo, com as classificações (TotvsCostCenters e filha), a cada 2 horas', 120, 2),

/*Leituras sob demanda sem persistência - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0) e não gravam
  tabela: devolvem o conteúdo e registram a chamada em totvs.TotvsRequests (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Fiscal: 13250 - 13299)*/
(130, 13252, 'TotvsGetXmlContents', 'XML de uma NF-e/NFC-e pela chave de acesso (GET ElectronicInvoices/{docCompany}/{accessKey}/Xml)', 0, 2),
(130, 13253, 'TotvsPostSearchDanfe', 'PDF do DANFE de uma NF-e pela chave de acesso (GET ElectronicInvoices/{docCompany}/{accessKey}/Danfe)', 0, 2),
(130, 13254, 'TotvsPostTransactionPrint', 'PDF da impressão da transação de uma nota (GET Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Print)', 0, 2),

/*Leituras sob demanda sem persistência - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0) e não gravam
  tabela: devolvem o JSON e registram a chamada em totvs.TotvsRequests (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Fiscal: 13250 - 13299)*/
(130, 13255, 'TotvsGetSearchInvoicesItemDetail', 'Itens de uma nota com transações e notas referenciadas por produto (GET Invoices/.../Items e ElectronicInvoices/.../Items)', 0, 2),
(130, 13256, 'TotvsGetInvoicesPendingConditionalProducts', 'Saldo de produtos em condicional de uma pessoa numa loja (GET ConditionalProducts/{docCompany}/{branchCode})', 0, 2),
(130, 13257, 'TotvsPostSearchInvoiceProducts', 'Produtos das notas por filtro (POST InvoiceProducts/{docCompany}/Search)', 0, 2),

/*Manifestação do destinatário (efeito fiscal a confirmar) - executada pelo TotvsController, não é recorrente (IntervalTimeInMinutes = 0); só roda
  com Totvs:Writes:Invoices:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.10, fase FI-3)*/
/*Range 13000 - 13499 (Fiscal: 13250 - 13299)*/
(130, 13258, 'TotvsPostInvoicesManifestations', 'Manifestação do destinatário (POST InvoicesManifestations/{docCompany})', 0, 2);
