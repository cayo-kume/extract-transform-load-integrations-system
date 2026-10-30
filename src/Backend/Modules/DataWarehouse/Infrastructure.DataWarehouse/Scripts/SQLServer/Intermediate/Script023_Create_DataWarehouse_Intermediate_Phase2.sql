/****** dw_intermediate da fase 2: estoque, tickets e reembolsos (src/Backend/Modules/DataWarehouse/docs, 7.4, 8.5 e seção 2) ******/
--como na fase 1, são views e funções sem tabela: os fatos leem daqui. Nenhuma lê dw_personal_data: o vínculo com o cliente
--(doc_hash) é feito na procedure do fato, que recebe o pepper.

/****** dw_intermediate.fn_StockPosition: posição de estoque no fim de um dia ******/
--lê a versão de dw_history.TotvsBalances vigente no último instante do dia (valid_from < dia seguinte <= valid_to), com loja e
--produto da época e o custo da margem (papel margin_cost em ValueCodes, 8.7) vigente no mesmo instante. Posição diária sob
--demanda (8.5): select * from [dw_intermediate].fn_StockPosition('2026-10-15'). sp_FactMonthlyStock_Load usa a mesma função.
create or alter function [dw_intermediate].fn_StockPosition (
	@position_date date
)
returns table
as
return
	with as_of as (
		select dateadd(day, 1, cast(@position_date as datetime2(3))) as next_day
	)
	select @position_date as position_date,
	       b.doc_company, b.branchCode as branch_code, b.productCode as product_code, b.stockCode as stock_code,
	       b.stockDescription as stock_description,
	       isnull(st.sk, replicate('0', 32)) as store_sk,
	       isnull(pr.sk, replicate('0', 32)) as product_sk,
	       isnull(b.stock, 0) as quantity,
	       isnull(b.salesOrder, 0) as reserved_by_order,
	       isnull(b.purchaseOrder, 0) as incoming_from_purchase,
	       c.cost as unit_cost
	  from [dw_history].TotvsBalances b
	 cross join as_of a
	 outer apply (select top 1 d.sk from [datawarehouse].DimStore d
	               where d.doc_company = b.doc_company and d.branch_code = b.branchCode
	                 and d.valid_from < a.next_day and (d.valid_to is null or d.valid_to >= a.next_day)
	               order by d.valid_from desc) st
	 outer apply (select top 1 d.sk from [datawarehouse].DimProduct d
	               where d.doc_company = b.doc_company and d.source_erp = 'totvs' and d.product_code = b.productCode
	                 and d.valid_from < a.next_day and (d.valid_to is null or d.valid_to >= a.next_day)
	               order by d.valid_from desc) pr
	 outer apply (select top 1 h.cost
	                from [dw_history].TotvsCosts h
	                join [dw_reference].ValueCodes v on v.value_type = 'C' and v.code = h.costCode and v.[role] = 'margin_cost'
	               where h.doc_company = b.doc_company and h.branchCode = b.branchCode and h.productCode = b.productCode
	                 and h.valid_from < a.next_day and (h.valid_to is null or h.valid_to >= a.next_day)
	               order by h.valid_from desc) c
	 where b.valid_from < a.next_day
	   and (b.valid_to is null or b.valid_to >= a.next_day);
GO

/****** dw_intermediate.vw_Ticket: um ticket do Movidesk por linha ******/
--origem e tipo pelas tabelas de domínio do módulo (movidesk.MovideskTicketsOrigins e MovideskTicketsTypes, carregadas por
--script, sem integração). Cliente principal: o de menor id entre os clientes ativos do ticket. Ações: 1 interna, 2 pública.
create or alter view [dw_intermediate].vw_Ticket
as
select t.doc_company,
       t.id as ticket_id,
       t.protocol,
       cast(t.createdDate as date) as created_date,
       t.createdDate as created_at,
       t.resolvedIn as resolved_at,
       t.closedIn as closed_at,
       t.canceledIn as canceled_at,
       t.reopenedIn as reopened_at,
       t.lastActionDate as last_action_at,
       ty.[description] as ticket_type,
       o.[name] as origin,
       t.category,
       t.urgency,
       t.[status],
       t.baseStatus as base_status,
       t.justification,
       t.ownerTeam as owner_team,
       t.ownerId as owner_id,
       t.serviceFirstLevel as service_first_level,
       t.serviceSecondLevel as service_second_level,
       t.serviceThirdLevel as service_third_level,
       t.contactForm as contact_form,
       t.slaAgreement as sla_agreement,
       cl.clientId as client_id,
       cast(isnull(t.isDeleted, 0) as bit) as is_deleted,
       cast(iif(t.baseStatus in ('Resolved', 'Closed'), 1, 0) as bit) as is_resolved,
       cast(isnull(t.resolvedInFirstCall, 0) as bit) as resolved_in_first_call,
       --SLA cumprido: nulo sem prazo ou sem a data que o mede
       cast(case when t.slaResponseDate is null or t.slaRealResponseDate is null then null
                 when t.slaRealResponseDate <= t.slaResponseDate then 1 else 0 end as bit) as is_sla_response_met,
       cast(case when t.slaSolutionDate is null or coalesce(t.resolvedIn, t.closedIn) is null then null
                 when coalesce(t.resolvedIn, t.closedIn) <= t.slaSolutionDate then 1 else 0 end as bit) as is_sla_solution_met,
       isnull(t.actionCount, 0) as action_count,
       isnull(a.public_actions, 0) as public_action_count,
       isnull(a.internal_actions, 0) as internal_action_count,
       t.lifetimeWorkingTime as lifetime_working_minutes,
       t.stoppedTimeWorkingTime as stopped_working_minutes,
       t.slaResponseTime as sla_response_minutes,
       t.slaSolutionTime as sla_solution_minutes,
       t.chatWaitingTime as chat_waiting_seconds,
       t.chatTalkTime as chat_talk_seconds
  from [dw_landing].MovideskTickets t
  left join [movidesk].MovideskTicketsTypes ty on ty.id = t.[type]
  left join [movidesk].MovideskTicketsOrigins o on o.id = t.origin
 outer apply (select top 1 c.clientId
                from [dw_landing].MovideskTicketsClients c
               where c.doc_company = t.doc_company and c.ticketId = t.id and c.missing_since is null and isnull(c.isDeleted, 0) = 0
               order by c.clientId) cl
 outer apply (select sum(iif(x.[type] = 2, 1, 0)) as public_actions, sum(iif(x.[type] = 1, 1, 0)) as internal_actions
                from [dw_landing].MovideskTicketsActions x
               where x.doc_company = t.doc_company and x.ticketId = t.id and x.missing_since is null and isnull(x.isDeleted, 0) = 0) a;
GO

/****** dw_intermediate.vw_Refund: um reembolso do AfterSale por linha ******/
--com a reversa (marca, tipo, frete pelo cliente) e o resumo dos produtos: linhas, quantidades e o motivo do produto de menor
--id, que é o motivo principal do reembolso.
create or alter view [dw_intermediate].vw_Refund
as
select r.doc_company,
       r.id as refund_id,
       cast(r.created_at as date) as created_date,
       r.created_at,
       r.updated_at,
       r.customer_id,
       r.[type] as refund_type,
       r.[action] as refund_action,
       r.status_id,
       r.status_name,
       r.shipping_method,
       r.refunded_shipping_type,
       r.product_price_type,
       r.seller_info,
       r.reverse_id,
       isnull(v.reverse_type_name, r.reverse_reverse_type) as reverse_type,
       r.reverse_status_name,
       r.reverse_created_at,
       v.brand_name,
       v.origin as reverse_origin,
       cast(isnull(v.freight_by_customer, 0) as bit) as freight_by_customer,
       r.reverse_ecommerce_order_id as ecommerce_order_id,
       r.reverse_ecommerce_order as ecommerce_order,
       r.reverse_ecommerce_order_seller_invoice_number as seller_invoice_number,
       p.main_reason,
       p.main_subreason,
       cast(isnull(r.free_shipping, 0) as bit) as free_shipping,
       isnull(p.product_lines, 0) as product_lines,
       isnull(p.requested_qty, 0) as requested_qty,
       isnull(p.received_qty, 0) as received_qty,
       isnull(r.requested_raw_amount, 0) as requested_raw_amount,
       isnull(r.requested_amount, 0) as requested_amount,
       isnull(r.requested_shipping_amount, 0) as requested_shipping_amount,
       isnull(r.requested_total_amount, 0) as requested_total_amount,
       isnull(r.received_raw_amount, 0) as received_raw_amount,
       isnull(r.received_amount, 0) as received_amount,
       isnull(r.bonus_amount, 0) as bonus_amount,
       isnull(r.added_taxes_amount, 0) as added_taxes_amount,
       isnull(r.insurance_amount, 0) as insurance_amount,
       isnull(r.total_amount, 0) as total_amount
  from [dw_landing].AfterSaleRefunds r
  left join [dw_landing].AfterSaleReverses v on v.doc_company = r.doc_company and v.id = r.reverse_id
 outer apply (select count(*) as product_lines, sum(x.requested_qty) as requested_qty, sum(x.received_qty) as received_qty,
                     max(iif(x.rn = 1, x.reason_description, null)) as main_reason,
                     max(iif(x.rn = 1, x.subreason_description, null)) as main_subreason
                from (select y.requested_qty, y.received_qty, y.reason_description, y.subreason_description,
                             row_number() over (order by y.id) as rn
                        from [dw_landing].AfterSaleRefundsProducts y
                       where y.doc_company = r.doc_company and y.refund_id = r.id) x) p;
GO
