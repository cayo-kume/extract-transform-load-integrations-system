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

/****** datamart.vw_Check_InvoicedReconciliation: com o legado Linx ******/
--a da fase 1 comparava DailySales só com a Totvs; agora as linhas Linx entram dos dois lados. A loja é identificada por
--origem, empresa e código, porque o código de uma loja só do Linx pode repetir o de uma loja da Totvs.
create or alter view [datamart].vw_Check_InvoicedReconciliation
as
with mart as (
	select s.source_erp, s.doc_company, s.branch_code, datefromparts(year(d.sale_date), month(d.sale_date), 1) as [month], sum(d.invoiced_amount) as amount
	  from [datamart].DailySales d
	  join [datawarehouse].DimStore s on s.sk = d.store_sk
	 group by s.source_erp, s.doc_company, s.branch_code, datefromparts(year(d.sale_date), month(d.sale_date), 1)
),
landing as (
	select source_erp, doc_company, branch_code, [month], sum(amount) as amount
	  from (
		select 'totvs' as source_erp, v.doc_company, v.branch_code, datefromparts(year(v.invoice_date), month(v.invoice_date), 1) as [month],
		       v.sign * v.net_amount as amount
		  from [dw_intermediate].vw_SalesItem v
		 where v.is_invoiced = 1
		   and v.is_sale = 1
		union all
		select st.source_erp, st.doc_company, st.branch_code, datefromparts(year(v.invoice_date), month(v.invoice_date), 1),
		       v.sign * v.net_amount
		  from [dw_intermediate].vw_LinxSalesItem v
		 outer apply (select top 1 d.source_erp, d.doc_company, d.branch_code
		                from [datawarehouse].DimStore d
		               where d.cnpj = v.cnpj_emp
		                 and d.valid_from <= v.invoice_datetime and (d.valid_to is null or d.valid_to > v.invoice_datetime)
		               order by iif(d.source_erp = 'totvs', 0, 1), d.valid_from desc) st
		 where v.is_invoiced = 1
		   and v.is_sale = 1
	  ) x
	 group by source_erp, doc_company, branch_code, [month]
)
select coalesce(m.doc_company, l.doc_company) as doc_company, coalesce(m.branch_code, l.branch_code) as branch_code,
       coalesce(m.[month], l.[month]) as [month], m.amount as datamart_amount, l.amount as landing_amount
  from mart m
  full join landing l
    on l.source_erp = m.source_erp and l.doc_company = m.doc_company and l.branch_code = m.branch_code and l.[month] = m.[month]
 where abs(isnull(m.amount, 0) - isnull(l.amount, 0)) > 1.00;
GO
