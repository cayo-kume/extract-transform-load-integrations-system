INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Manifestação do destinatário (efeito fiscal a confirmar) - executada pelo TotvsController, não é recorrente (IntervalTimeInMinutes = 0); só roda
  com Totvs:Writes:Invoices:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.10, fase FI-3)*/
/*Range 13000 - 13499 (Fiscal: 13250 - 13299)*/
(130, 13258, 'TotvsPostInvoicesManifestations', 'Manifestação do destinatário (POST InvoicesManifestations/{docCompany})', 0, 2);
