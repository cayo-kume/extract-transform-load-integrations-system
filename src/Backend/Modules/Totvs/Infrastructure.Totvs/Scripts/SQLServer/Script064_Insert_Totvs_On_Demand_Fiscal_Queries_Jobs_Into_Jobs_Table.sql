INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Leituras sob demanda sem persistência - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0) e não gravam
  tabela: devolvem o JSON e registram a chamada em totvs.TotvsRequests (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Fiscal: 13250 - 13299)*/
(130, 13255, 'TotvsGetSearchInvoicesItemDetail', 'Itens de uma nota com transações e notas referenciadas por produto (GET Invoices/.../Items e ElectronicInvoices/.../Items)', 0, 2),
(130, 13256, 'TotvsGetInvoicesPendingConditionalProducts', 'Saldo de produtos em condicional de uma pessoa numa loja (GET ConditionalProducts/{docCompany}/{branchCode})', 0, 2),
(130, 13257, 'TotvsPostSearchInvoiceProducts', 'Produtos das notas por filtro (POST InvoiceProducts/{docCompany}/Search)', 0, 2);
