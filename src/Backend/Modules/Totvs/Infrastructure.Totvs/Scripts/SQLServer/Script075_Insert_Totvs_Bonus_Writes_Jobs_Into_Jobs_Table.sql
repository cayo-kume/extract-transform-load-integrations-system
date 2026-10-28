INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas de bônus da API Person (efeito financeiro) - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam
  com Totvs:Writes:Bonus:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.4.5, fase PE-7)*/
/*Range 13000 - 13499 (Person: 13030 - 13069)*/
(130, 13047, 'TotvsPostBonusDueDate', 'Crédito de bônus do cliente (POST BonusDueDate/{docCompany})', 0, 2),
(130, 13048, 'TotvsPostBonusConsume', 'Consumo de bônus do cliente (POST BonusConsume/{docCompany})', 0, 2);
