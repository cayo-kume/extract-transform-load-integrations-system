INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Integrações sob demanda - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 13000 - 13499 (ids reservados em docs/PROPOSTA-PADRONIZACAO-TOTVS.md §7; sob demanda a partir de 13010)*/
(130, 13012, 'TotvsGetSalesOrderByKey', 'Integração de um pedido de venda por loja e número (GET SalesOrders/{docCompany}/{branchCode}/{orderCode})', 0, 2),
(130, 13013, 'TotvsGetInvoiceByKey', 'Integração de uma nota fiscal por loja e sequência (GET Invoices/{docCompany}/{branchCode}/{invoiceSequence})', 0, 2);
