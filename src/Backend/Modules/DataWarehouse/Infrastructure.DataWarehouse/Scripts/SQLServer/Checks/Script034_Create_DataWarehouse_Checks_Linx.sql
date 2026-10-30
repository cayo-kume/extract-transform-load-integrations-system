/****** Checagens do legado Linx (docs/datawarehouse, 7.5, 9.3 e seção 11) ******/

/****** datawarehouse.vw_Check_ErpOverlap: CNPJ e dia com venda faturada nos dois ERPs ******/
--o mesmo dia de uma loja não pode somar Linx e Totvs (7.5). Bloqueia. A correção é um período em dw_reference.StoreErpMap
--para o CNPJ, que corta as linhas Linx fora dele (vw_LinxSalesItem).
create or alter view [datawarehouse].vw_Check_ErpOverlap
as
select s.cnpj, f.invoice_date,
       sum(iif(f.source_erp = 'totvs', 1, 0)) as totvs_rows,
       sum(iif(f.source_erp = 'linxmicrovix', 1, 0)) as linx_rows
  from [datawarehouse].FactSalesItem f
  join [datawarehouse].DimStore s on s.sk = f.store_sk
 where f.is_invoiced = 1
   and f.is_sale = 1
   and s.cnpj is not null
 group by s.cnpj, f.invoice_date
having count(distinct f.source_erp) > 1;
GO

/****** datawarehouse.vw_Check_ProductMatch: produtos Linx sem de/para, por marca ******/
--proporção de produtos Linx que não casaram com um produto Totvs (pergunta 34). Alerta; a meta sai da primeira execução.
--Os pares que o texto não resolve vão para dw_reference.ProductErpMap.
create or alter view [datawarehouse].vw_Check_ProductMatch
as
select isnull(brand, N'NOT INFORMED') as brand,
       count(*) as linx_products,
       sum(iif(totvs_product_code is null, 1, 0)) as unmatched_products,
       cast(100.0 * sum(iif(totvs_product_code is null, 1, 0)) / count(*) as decimal(9,2)) as unmatched_percent
  from [dw_intermediate].vw_ProductMatch
 group by isnull(brand, N'NOT INFORMED')
having sum(iif(totvs_product_code is null, 1, 0)) > 0;
GO
