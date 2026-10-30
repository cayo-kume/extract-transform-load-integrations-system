/****** Checagens da fase 2 (docs/datawarehouse, 9.2 e seção 11) ******/
--uma checagem nova (estoque) e duas da fase 1 refeitas para cobrir as origens e o fato novos. Nenhuma bloqueia.

/****** datawarehouse.vw_Check_StockNegativeOrNoCost: posição de estoque negativa ou sem custo ******/
--só a última posição carregada, por loja: quantas linhas têm saldo negativo e quantas têm saldo sem custo da margem.
--Alerta mensal (seção 11); o estoque negativo costuma ser ajuste pendente na loja.
create or alter view [datawarehouse].vw_Check_StockNegativeOrNoCost
as
select f.position_date, f.doc_company, f.branch_code,
       sum(iif(f.quantity < 0, 1, 0)) as negative_rows,
       sum(iif(f.quantity > 0 and f.unit_cost is null, 1, 0)) as no_cost_rows
  from [datawarehouse].FactMonthlyStock f
 where f.position_date = (select max(position_date) from [datawarehouse].FactMonthlyStock)
 group by f.position_date, f.doc_company, f.branch_code
having sum(iif(f.quantity < 0, 1, 0)) > 0
    or sum(iif(f.quantity > 0 and f.unit_cost is null, 1, 0)) > 0;
GO
