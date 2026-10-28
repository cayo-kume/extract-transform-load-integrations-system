INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas da API Person - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:People:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.4.4, fase PE-5)*/
/*Range 13000 - 13499 (Person: 13030 - 13069)*/
(130, 13044, 'TotvsPostMessages', 'Inclusão de mensagens de uma pessoa (POST Messages/{docCompany})', 0, 2),
(130, 13045, 'TotvsPutChangeMessages', 'Alteração de uma mensagem da pessoa (PUT Messages/{docCompany}/Change)', 0, 2),
(130, 13046, 'TotvsDeleteMessages', 'Exclusão de uma mensagem da pessoa (DELETE Messages/{docCompany}/{personCode}/{sequence})', 0, 2);
