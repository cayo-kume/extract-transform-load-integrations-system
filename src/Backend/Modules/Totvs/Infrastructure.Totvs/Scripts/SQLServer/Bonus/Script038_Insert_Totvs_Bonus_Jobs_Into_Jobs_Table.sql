INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Leituras sob demanda da onda 2 - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0). Só o
  GetPurchaseOrderByKey grava (como o GetSalesOrderByKey); as outras devolvem o JSON e registram a chamada em totvs.TotvsRequests
  (docs/PLANO-TOTVS-METODOS-FALTANTES.md §8.1, T11)*/
/*Range 13000 - 13499 (Person: 13030 - 13069; Product: 13100 - 13105; Purchase Order: 13185; Sales Order: 13190 - 13249)*/
(130, 13038, 'TotvsPostListBalanceBonus', 'Saldo de bônus de uma pessoa nas lojas (POST ListBalanceBonus/{docCompany})', 0, 2),

/*Escritas de bônus da API Person (efeito financeiro) - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam
  com Totvs:Writes:Bonus:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.4.5, fase PE-7)*/
/*Range 13000 - 13499 (Person: 13030 - 13069)*/
(130, 13047, 'TotvsPostBonusDueDate', 'Crédito de bônus do cliente (POST BonusDueDate/{docCompany})', 0, 2),
(130, 13048, 'TotvsPostBonusConsume', 'Consumo de bônus do cliente (POST BonusConsume/{docCompany})', 0, 2);
