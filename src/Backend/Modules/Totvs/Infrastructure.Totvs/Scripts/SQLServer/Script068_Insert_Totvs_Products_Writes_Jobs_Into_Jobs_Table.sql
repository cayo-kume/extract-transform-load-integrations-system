INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Escritas da API Product - executadas pelo TotvsController, não são recorrentes (IntervalTimeInMinutes = 0); só rodam com
  Totvs:Writes:Products:Enabled = true (docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.5, fase PR-4)*/
/*Range 13000 - 13499 (Product: 13100 - 13179)*/
(130, 13124, 'TotvsPostColors', 'Inclusão ou alteração de uma cor (POST Products/{docCompany}/Colors)', 0, 2),
(130, 13126, 'TotvsPostGrids', 'Inclusão de uma grade de tamanhos (POST Products/{docCompany}/Grids)', 0, 2);
