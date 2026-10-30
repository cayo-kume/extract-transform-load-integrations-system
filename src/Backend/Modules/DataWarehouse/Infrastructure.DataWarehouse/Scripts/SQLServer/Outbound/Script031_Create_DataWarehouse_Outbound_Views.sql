/****** Views de saída da fase 1 (src/Backend/Modules/DataWarehouse/docs, 4.2, 8.6 e 10.3) ******/
--o que os módulos de saída enviam aos SaaS. Cada view expõe updated_at: o módulo lê updated_at > watermark - 3 dias e
--grava o próprio watermark por dw_control.sp_Watermark_Set (source = módulo, entity = view).
--o número do pedido é montado aqui, no formato de cada consumidor, a partir das partes de vw_OrderParts.

/****** dw_outbound.WmsOrders: pedidos para o WMS (formato MI-VD12345T; src/Backend/Modules/DataWarehouse/docs/regras-prefixo-pedidos.md, §2.1) ******/
--código = customerOrderCode quando existe, senão orderCode, sempre com o sufixo T; e-commerce (ST) sai sem o tipo.
--os tipos que o WMS recebe: e-commerce, venda direta, reposição de lojas e venda fácil.
create or alter view [dw_outbound].WmsOrders
as
select o.doc_company,
       o.branch_code,
       o.order_code,
       concat(o.order_series, iif(o.order_type = 'ST', '', o.order_type), coalesce(nullif(o.customer_order_code, ''), cast(o.order_code as nvarchar(20))), o.erp_suffix) as document_number,
       o.order_series,
       o.order_type,
       o.channel,
       b.cnpj                  as branch_tax_id,
       o.customer_code,
       o.order_date,
       o.status,
       o.total_amount,
       o.updated_at
  from [dw_intermediate].vw_OrderParts o
  left join [dw_landing].TotvsBranches b
    on b.doc_company = o.doc_company
   and b.[code] = o.branch_code
 where o.order_type in ('ST', 'VD', 'LJ', 'VF')
   and o.order_series is not null;   --sem série, o número sairia sem prefixo (inconsistência 8): fica fora e aparece em vw_Check_OrderTypeUnknown
GO

/****** dw_outbound.LogisticsShipments: pedidos para as transportadoras (ciclo curto de 3 minutos, 10.3) ******/
--lê a landing pela vw_OrderParts, sem depender da carga horária dos fatos, e o endereço de entrega de dw_personal_data:
--é uma das duas views com dado pessoal em claro (12.1), lida só pelas identidades da logística (12.2).
create or alter view [dw_outbound].LogisticsShipments
as
select o.doc_company,
       o.branch_code,
       o.order_code,
       concat(o.order_series, iif(o.order_type = 'ST', '', o.order_type), coalesce(nullif(o.customer_order_code, ''), cast(o.order_code as nvarchar(20))), o.erp_suffix) as document_number,
       o.order_series,
       o.order_type,
       o.channel,
       o.status,
       o.order_date,
       o.total_amount,
       o.freight_type,
       o.freight_value,
       o.shipping_company_code,
       o.shipping_company_name,
       a.customer_name         as recipient_name,
       a.customer_tax_id       as recipient_tax_id,
       c.phone                 as recipient_phone,
       c.email                 as recipient_email,
       a.[address],
       a.neighborhood,
       a.city,
       a.ibge_city_code,
       a.[state],
       a.zip_code,
       a.country,
       a.reference,
       iif(isnull(a.updated_at, '19000101') > isnull(o.updated_at, '19000101'), a.updated_at, o.updated_at) as updated_at
  from [dw_intermediate].vw_OrderParts o
  join [dw_personal_data].OrderShippingAddress a
    on a.doc_company = o.doc_company
   and a.branch_code = o.branch_code
   and a.order_code = o.order_code
  left join [dw_personal_data].DimCustomerIdentified c
    on c.doc_company = o.doc_company
   and c.source_erp = 'totvs'
   and c.customer_code = cast(o.customer_code as nvarchar(50))
 where o.is_sale = 1;
