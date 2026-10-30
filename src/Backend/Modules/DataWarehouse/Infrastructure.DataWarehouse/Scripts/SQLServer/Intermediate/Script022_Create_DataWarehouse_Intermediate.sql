/****** Camada intermediária da fase 1 (src/Backend/Modules/DataWarehouse/docs, 4.1, 7.2, 7.3 e 10.3) ******/
--views sobre a landing, no formato unificado (source_erp + source_key) e com as regras de negócio aplicadas. São views,
--e não tabelas, porque a carga dos fatos é recarga completa (pergunta 8): nada precisa ser guardado no meio.
--as linhas que a origem apagou (missing_since) ficam fora.

/****** dw_intermediate.vw_OrderParts: canal, tipo e partes do número do pedido (src/Backend/Modules/DataWarehouse/docs/regras-prefixo-pedidos.md) ******/
--lida pelos fatos (ciclo horário) e direto pelas views da logística (ciclo curto, 10.3). Vale a primeira regra de
--dw_reference.ChannelRules que casa; sem regra, canal e tipo ficam nulos (vw_Check_OrderTypeUnknown).
--a série é a da filial emitente (raiz do CNPJ); na reposição de lojas o WMS de produção usa a do cliente (§2.1).
create or alter view [dw_intermediate].vw_OrderParts
as
select o.doc_company,
       'totvs'                                as source_erp,
       o.branchCode                           as branch_code,
       o.orderCode                            as order_code,
       o.orderId                              as order_id,
       o.customerOrderCode                    as customer_order_code,
       o.integrationCode                      as integration_code,
       o.orderDate                            as order_datetime,
       cast(o.orderDate as date)              as order_date,
       o.statusOrder                          as status,
       o.customerCode                         as customer_code,
       o.sellerCode                           as seller_code,
       o.representativeCode                   as representative_code,
       o.representativeName                   as representative_name,
       o.operationCode                        as operation_code,
       o.operationName                        as operation_name,
       o.totalAmountOrder                     as total_amount,
       o.shippingCompanyCode                  as shipping_company_code,
       o.shippingCompanyName                  as shipping_company_name,
       o.freightType                          as freight_type,
       o.freightValue                         as freight_value,
       o.shippingAddressCityName              as shipping_city,
       o.shippingAddressStateAbbreviation     as shipping_state,
       r.channel,
       r.order_type,
       isnull(r.is_sale, 1)                   as is_sale,
       iif(o.statusOrder in ('PartiallyAnswered', 'Attended', 'Canceled', 'InComposition'), 0, 1) as is_captured,   --captado (7.3)
       bs.order_series,
       'T'                                    as erp_suffix,
       o.integrated_or_updated_at             as updated_at
  from [dw_landing].TotvsSaleOrders o
  left join [dw_landing].TotvsBranches b
    on b.doc_company = o.doc_company
   and b.[code] = o.branchCode
  left join [dw_reference].BrandSeries bs
    on bs.cnpj_root = left(b.cnpj, 8)
 outer apply (
	select top 1 x.channel, x.order_type, x.is_sale
	  from [dw_reference].ChannelRules x
	 where x.source_erp = 'totvs'
	   and case x.match_column
	            when 'integrationCode'    then o.integrationCode
	            when 'operationName'      then o.operationName
	            when 'representativeName' then o.representativeName
	       end like x.match_pattern
	 order by x.priority
 ) r
 where o.missing_since is null;
GO

/****** dw_intermediate.vw_SalesItem: linha de produto vendido (7.2, 7.3) ******/
--grão item × produto (TotvsInvoicesItemsProducts). Frete e impostos do item são rateados pelos produtos, pelo valor
--líquido (sem valor, pela quantidade). Item sem produto entra uma vez, com product_code nulo (membro desconhecido).
--canal: o do pedido que a nota fatura (TotvsInvoicesSalesOrders); sem pedido, a regra de PDV; sem as duas, loja.
--preço e custo: a versão de dw_history vigente na data da nota, pelos papéis de dw_reference.ValueCodes (8.7).
create or alter view [dw_intermediate].vw_SalesItem
as
with lines as (
	select p.doc_company, p.branchCode, p.invoiceSequence, p.invoiceDate, p.itemSequence, p.productCode,
	       p.quantity, p.grossValue, p.discountValue, p.netValue,
	       coalesce(
	           p.netValue / nullif(sum(p.netValue) over (partition by p.doc_company, p.branchCode, p.invoiceSequence, p.invoiceDate, p.itemSequence), 0),
	           p.quantity / nullif(sum(p.quantity) over (partition by p.doc_company, p.branchCode, p.invoiceSequence, p.invoiceDate, p.itemSequence), 0),
	           1.0 / count(*) over (partition by p.doc_company, p.branchCode, p.invoiceSequence, p.invoiceDate, p.itemSequence)
	       ) as share
	  from [dw_landing].TotvsInvoicesItemsProducts p
	 where p.missing_since is null
	union all
	select i.doc_company, i.branchCode, i.invoiceSequence, i.invoiceDate, i.[sequence], null,
	       i.quantity, i.grossValue, i.discountValue, i.netValue, 1.0
	  from [dw_landing].TotvsInvoicesItems i
	 where i.missing_since is null
	   and not exists (select 1 from [dw_landing].TotvsInvoicesItemsProducts p
	                    where p.doc_company = i.doc_company and p.branchCode = i.branchCode and p.invoiceSequence = i.invoiceSequence
	                      and p.invoiceDate = i.invoiceDate and p.itemSequence = i.[sequence] and p.missing_since is null)
)
select 'totvs'                                                       as source_erp,
       concat_ws('|', l.doc_company, l.branchCode, l.invoiceSequence, convert(char(8), l.invoiceDate, 112), l.itemSequence, isnull(cast(l.productCode as varchar(20)), '-')) as source_key,
       l.doc_company,
       cast(n.invoiceDate as date)                                   as invoice_date,
       n.invoiceDate                                                 as invoice_datetime,
       l.branchCode                                                  as branch_code,
       l.invoiceSequence                                             as invoice_sequence,
       l.itemSequence                                                as item_sequence,
       l.productCode                                                 as product_code,
       concat_ws('|', so.orderBranchCode, so.orderCode)              as source_order,
       n.personCode                                                  as customer_code,
       op.seller_code,
       n.operationCode                                               as operation_code,
       coalesce(op.channel, pdv.channel, 'store')                    as channel,
       iif(op.order_code is not null, op.order_type, pdv.order_type) as order_type,
       coalesce(op.is_sale, pdv.is_sale, 1)                          as is_sale,
       iif(n.invoiceStatus in ('Canceled', 'Cancelled', 'Deleted'), 1, 0) as is_cancelled,
       iif(n.invoiceStatus in ('Authorized', 'Generated', 'Issued', 'Normal')
           and n.eletronicElectronicInvoiceStatus in ('Authorized', 'Generated'), 1, 0) as is_invoiced,                --faturado (7.3)
       iif(n.eletronicElectronicInvoiceStatus = 'Authorized'
           or (n.invoiceStatus not in ('Canceled', 'Cancelled', 'Deleted') and n.inclusionComponentCode in ('PEDFP144', 'PDVFM001')), 1, 0) as is_realized_operation,
       iif(n.operationType = 'Input', -1, 1)                         as sign,   --entrada e devolução negativas (7.3)
       l.quantity,
       l.grossValue                                                  as gross_amount,
       l.discountValue                                               as discount_amount,
       l.netValue                                                    as net_amount,
       isnull(i.freightValue, 0) * l.share                           as freight_amount,
       t.tax_value * l.share                                         as tax_amount,
       lp.price                                                      as list_price,
       orp.price                                                     as original_price,
       pp.price                                                      as promotional_price,
       c.cost                                                        as unit_cost
  from lines l
  join [dw_landing].TotvsInvoices n
    on n.doc_company = l.doc_company and n.branchCode = l.branchCode and n.invoiceSequence = l.invoiceSequence and n.invoiceDate = l.invoiceDate
  left join [dw_landing].TotvsInvoicesItems i
    on i.doc_company = l.doc_company and i.branchCode = l.branchCode and i.invoiceSequence = l.invoiceSequence and i.invoiceDate = l.invoiceDate and i.[sequence] = l.itemSequence
 outer apply (
	select sum(x.taxValue) as tax_value
	  from [dw_landing].TotvsInvoicesItemsTaxes x
	 where x.doc_company = l.doc_company and x.branchCode = l.branchCode and x.invoiceSequence = l.invoiceSequence
	   and x.invoiceDate = l.invoiceDate and x.itemSequence = l.itemSequence and x.missing_since is null
 ) t
 outer apply (
	select top 1 x.orderBranchCode, x.orderCode
	  from [dw_landing].TotvsInvoicesSalesOrders x
	 where x.doc_company = l.doc_company and x.branchCode = l.branchCode and x.invoiceSequence = l.invoiceSequence
	   and x.invoiceDate = l.invoiceDate and x.missing_since is null
	 order by x.orderBranchCode, x.orderCode
 ) so
  left join [dw_intermediate].vw_OrderParts op
    on op.doc_company = l.doc_company and op.branch_code = so.orderBranchCode and op.order_code = so.orderCode
 outer apply (
	select top 1 x.channel, x.order_type, x.is_sale
	  from [dw_reference].ChannelRules x
	 where x.source_erp = 'totvs' and x.match_column = 'inclusionComponentCode' and n.inclusionComponentCode like x.match_pattern
	 order by x.priority
 ) pdv
 outer apply (
	select top 1 h.price
	  from [dw_history].TotvsPrices h
	  join [dw_reference].ValueCodes v on v.value_type = 'P' and v.code = h.priceCode
	 where v.[role] = iif(coalesce(op.channel, pdv.channel) = 'ecommerce', 'list_price_ecommerce', 'list_price_store')
	   and h.doc_company = l.doc_company and h.branchCode = l.branchCode and h.productCode = l.productCode
	   and h.valid_from <= n.invoiceDate and (h.valid_to is null or h.valid_to > n.invoiceDate)
 ) lp
 outer apply (
	select top 1 h.price
	  from [dw_history].TotvsPrices h
	  join [dw_reference].ValueCodes v on v.value_type = 'P' and v.code = h.priceCode and v.[role] = 'original_price'
	 where h.doc_company = l.doc_company and h.branchCode = l.branchCode and h.productCode = l.productCode
	   and h.valid_from <= n.invoiceDate and (h.valid_to is null or h.valid_to > n.invoiceDate)
 ) orp
 outer apply (
	select top 1 h.price
	  from [dw_history].TotvsPrices h
	  join [dw_reference].ValueCodes v on v.value_type = 'P' and v.code = h.priceCode and v.[role] = 'promotional_price'
	 where h.doc_company = l.doc_company and h.branchCode = l.branchCode and h.productCode = l.productCode
	   and h.valid_from <= n.invoiceDate and (h.valid_to is null or h.valid_to > n.invoiceDate)
 ) pp
 outer apply (
	select top 1 h.cost
	  from [dw_history].TotvsCosts h
	  join [dw_reference].ValueCodes v on v.value_type = 'C' and v.code = h.costCode and v.[role] = 'margin_cost'
	 where h.doc_company = l.doc_company and h.branchCode = l.branchCode and h.productCode = l.productCode
	   and h.valid_from <= n.invoiceDate and (h.valid_to is null or h.valid_to > n.invoiceDate)
 ) c
 where n.missing_since is null;
GO

/****** dw_intermediate.vw_OrderItem: linha de produto do pedido (7.2) ******/
create or alter view [dw_intermediate].vw_OrderItem
as
select 'totvs'                                                                 as source_erp,
       concat_ws('|', p.doc_company, p.branchCode, p.orderCode, p.productCode) as source_key,
       p.doc_company,
       o.order_date,
       o.order_datetime,
       p.branchCode                                                            as branch_code,
       p.orderCode                                                             as order_code,
       p.productCode                                                           as product_code,
       o.customer_order_code,
       o.customer_code,
       o.seller_code,
       o.operation_code,
       o.channel,
       o.order_type,
       o.is_sale,
       o.is_captured,
       o.status,
       o.order_series,
       o.erp_suffix,
       p.quantity,
       p.settledQuantity                                                       as settled_quantity,
       p.canceledQuantity                                                      as canceled_quantity,
       p.pendingQuantity                                                       as pending_quantity,
       p.originalPrice                                                         as original_price,
       p.price,
       p.discountPercentage                                                    as discount_percentage,
       p.quantity * p.price                                                    as amount,
       iif(isnull(p.integrated_or_updated_at, '19000101') > isnull(o.updated_at, '19000101'), p.integrated_or_updated_at, o.updated_at) as updated_at
  from [dw_landing].TotvsSaleOrdersProducts p
  join [dw_intermediate].vw_OrderParts o
    on o.doc_company = p.doc_company and o.branch_code = p.branchCode and o.order_code = p.orderCode
 where p.missing_since is null;
