/****** dw_intermediate do legado Linx Microvix (src/Backend/Modules/DataWarehouse/docs, 7.3, 7.5 e pergunta 34) ******/
--três views, sem tabela: o de/para de produto, a linha de venda e a linha de pedido do e-commerce, no formato dos fatos da
--fase 1. Quando o mesmo código de produto ou de cliente aparece em mais de um portal do Microvix, vale o de menor portal.

/****** dw_intermediate.vw_ProductMatch: produto Linx → produto Totvs ******/
--primeiro o par corrigido à mão em dw_reference.ProductErpMap (método manual); depois referência, cor e tamanho iguais, sem
--diferença de maiúsculas, acentos e espaços nas pontas (método text). Com mais de um produto Totvs igual, fica o da mesma
--empresa e, depois, o de menor empresa, matriz e código. Sem casamento, totvs_product_code é nulo.
create or alter view [dw_intermediate].vw_ProductMatch
as
with linx as (
	select p.doc_company, p.cod_produto, p.desc_marca,
	       upper(trim(p.referencia)) collate Latin1_General_100_CI_AI as ref_key,
	       upper(trim(p.desc_cor)) collate Latin1_General_100_CI_AI as color_key,
	       upper(trim(p.desc_tamanho)) collate Latin1_General_100_CI_AI as size_key,
	       dense_rank() over (partition by p.doc_company, p.cod_produto order by p.portal) as portal_rank
	  from [dw_landing].LinxProdutos p
),
totvs as (
	select p.doc_company, p.branchInfoCode, p.[code],
	       upper(trim(s.referenceCode)) collate Latin1_General_100_CI_AI as ref_key,
	       upper(trim(c.[name])) collate Latin1_General_100_CI_AI as color_key,
	       upper(trim(p.size)) collate Latin1_General_100_CI_AI as size_key
	  from [dw_landing].TotvsSkusColorsProducts p
	  join [dw_landing].TotvsSkus s on s.doc_company = p.doc_company and s.branchInfoCode = p.branchInfoCode and s.referenceId = p.referenceId
	  join [dw_landing].TotvsSkusColors c on c.doc_company = p.doc_company and c.branchInfoCode = p.branchInfoCode and c.referenceId = p.referenceId and c.colorCode = p.colorCode
),
text_match as (
	select l.doc_company, l.cod_produto, t.doc_company as totvs_doc_company, t.[code] as totvs_product_code,
	       row_number() over (partition by l.doc_company, l.cod_produto
	                          order by iif(t.doc_company = l.doc_company, 0, 1), t.doc_company, t.branchInfoCode, t.[code]) as rn
	  from linx l
	  join totvs t on t.ref_key = l.ref_key and t.color_key = l.color_key and t.size_key = l.size_key
	 where l.portal_rank = 1
)
select l.doc_company,
       cast(l.cod_produto as nvarchar(50)) as linx_product_code,
       upper(trim(l.desc_marca)) as brand,
       coalesce(iif(m.totvs_product_code is null, null, l.doc_company), tm.totvs_doc_company) as totvs_doc_company,
       coalesce(m.totvs_product_code, tm.totvs_product_code) as totvs_product_code,
       case when m.totvs_product_code is not null then 'manual' when tm.totvs_product_code is not null then 'text' end as match_method
  from linx l
  left join [dw_reference].ProductErpMap m on m.doc_company = l.doc_company and m.linx_product_code = cast(l.cod_produto as nvarchar(50))
  left join text_match tm on tm.doc_company = l.doc_company and tm.cod_produto = l.cod_produto and tm.rn = 1
 where l.portal_rank = 1;
GO

/****** dw_intermediate.vw_LinxSalesItem: linha de produto vendido do Microvix (7.3, ramo Linx) ******/
--uma linha de LinxMovimento. Faturada: não cancelada, não excluída, autorizada na Sefaz e venda não estornada
--(LinxMovimentoPrincipal). Venda: operação S (saída) e DS (devolução de venda, negativa); E e DE ficam com is_sale = 0.
--Canal: e-commerce quando a linha veio de um pedido B2C (id_venda_produto_b2c), senão loja. Valor líquido e custo da
--própria linha (preco_custo), preço de tabela da época. Um CNPJ com período em dw_reference.StoreErpMap para o Linx só
--entra dentro do período (7.5).
create or alter view [dw_intermediate].vw_LinxSalesItem
as
select 'linxmicrovix' as source_erp,
       concat_ws('|', m.doc_company, m.cnpj_emp, m.transacao) as source_key,
       m.doc_company,
       m.cnpj_emp,
       m.portal,
       cast(m.data_documento as date) as invoice_date,
       m.data_documento as invoice_datetime,
       m.transacao,
       m.identificador,
       m.documento,
       m.cod_produto,
       cast(m.codigo_cliente as nvarchar(50)) as customer_code,
       concat(m.cnpj_emp, '-', m.cod_vendedor) as seller_code,
       nullif(trim(m.cod_natureza_operacao), '') as operation_code,
       iif(m.id_venda_produto_b2c is null, 'store', 'ecommerce') as channel,
       iif(m.id_venda_produto_b2c is null, null, 'ST') as order_type,
       cast(iif(m.operacao in ('S', 'DS'), 1, 0) as bit) as is_sale,
       iif(m.operacao in ('DS', 'E'), -1, 1) as sign,
       cast(iif(m.cancelado = 'S' or m.excluido = 'S' or isnull(p.estornada, 0) = 1, 1, 0) as bit) as is_cancelled,
       cast(iif(m.cancelado = 'N' and isnull(m.excluido, 'N') = 'N' and m.desc_sefaz_situacao = 'Autorizada' and isnull(p.estornada, 0) = 0, 1, 0) as bit) as is_invoiced,
       isnull(m.quantidade, 0) as quantity,
       isnull(m.preco_unitario * m.quantidade, 0) as gross_amount,
       isnull(m.desconto, 0) as discount_amount,
       isnull(m.frete, 0) as freight_amount,
       isnull(m.valor_icms, 0) + isnull(m.valor_pis, 0) + isnull(m.valor_cofins, 0) + isnull(m.valor_ipi, 0) as tax_amount,
       isnull(m.valor_liquido, 0) as net_amount,
       m.preco_tabela_epoca as list_price,
       m.preco_custo as unit_cost
  from [dw_landing].LinxMovimento m
 outer apply (select top 1 x.estornada from [dw_landing].LinxMovimentoPrincipal x
               where x.doc_company = m.doc_company and x.cnpj_emp = m.cnpj_emp and x.identificador = m.identificador
               order by x.estornada desc) p
 where not exists (select 1 from [dw_reference].StoreErpMap r where r.cnpj = m.cnpj_emp and r.source_erp = 'linxmicrovix')
    or exists (select 1 from [dw_reference].StoreErpMap r
                where r.cnpj = m.cnpj_emp and r.source_erp = 'linxmicrovix'
                  and cast(m.data_documento as date) >= r.valid_from and (r.valid_to is null or cast(m.data_documento as date) < r.valid_to));
GO

/****** dw_intermediate.vw_LinxOrderItem: linha de pedido do e-commerce do Microvix (B2C) ******/
--loja pela venda de origem (cnpj_origem_venda), senão pela empresa do pedido. Captado: pedido ativo.
create or alter view [dw_intermediate].vw_LinxOrderItem
as
select 'linxmicrovix' as source_erp,
       concat_ws('|', i.doc_company, i.portal, i.id_pedido_item) as source_key,
       i.doc_company,
       coalesce(nullif(p.cnpj_origem_venda, ''), p.cnpj_emp) as store_cnpj,
       cast(p.dt_pedido as date) as order_date,
       p.id_pedido as order_code,
       i.codigoproduto as product_code,
       p.order_id as customer_order_code,
       cast(p.cod_cliente_erp as nvarchar(50)) as customer_code,
       concat(p.cnpj_emp, '-', p.cod_vendedor) as seller_code,
       cast(p.id_status as nvarchar(30)) as [status],
       cast(isnull(p.ativo, 0) as bit) as is_captured,
       cast(isnull(i.quantidade, 0) as decimal(18,6)) as quantity,
       i.vl_unitario as price,
       isnull(i.quantidade * i.vl_unitario, 0) as amount
  from [dw_landing].LinxB2CPedidosItens i
  join [dw_landing].LinxB2CPedidos p on p.doc_company = i.doc_company and p.portal = i.portal and p.id_pedido = i.id_pedido;
GO
