INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 13000 - 13499 (Accounts Payable: 13018 - 13020, docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13018, 'TotvsPostDuplicates', 'Inclusão de duplicata a pagar (POST Duplicates/{docCompany})', 0, 2),
(130, 13019, 'TotvsPostGroupDuplicates', 'Agrupamento de duplicatas a pagar (POST GroupDuplicates/{docCompany})', 0, 2),
(130, 13020, 'TotvsPostDuplicatesInstallmentsStage', 'Troca do estágio de uma parcela a pagar (POST Duplicates/{docCompany}/Installments/Stage)', 0, 2);
