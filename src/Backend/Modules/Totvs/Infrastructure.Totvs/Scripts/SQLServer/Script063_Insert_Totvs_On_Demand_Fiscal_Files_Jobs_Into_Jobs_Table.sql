INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Leituras sob demanda sem persistência - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0) e não gravam
  tabela: devolvem o conteúdo e registram a chamada em totvs.TotvsRequests (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Fiscal: 13250 - 13299)*/
(130, 13252, 'TotvsGetXmlContents', 'XML de uma NF-e/NFC-e pela chave de acesso (GET ElectronicInvoices/{docCompany}/{accessKey}/Xml)', 0, 2),
(130, 13253, 'TotvsPostSearchDanfe', 'PDF do DANFE de uma NF-e pela chave de acesso (GET ElectronicInvoices/{docCompany}/{accessKey}/Danfe)', 0, 2),
(130, 13254, 'TotvsPostTransactionPrint', 'PDF da impressão da transação de uma nota (GET Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Print)', 0, 2);
