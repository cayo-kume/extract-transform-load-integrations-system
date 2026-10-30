/****** Landing das origens da fase 2 (docs/datawarehouse, seção 2, 5.1 e 7.4) ******/
--saldos da Totvs, tickets do Movidesk e reembolsos e reversas do AfterSale. Mesmo catálogo e mesmo gerador da fase 1 (9.5).
--dado pessoal identificador fica fora (5.2, regra 6):
--  - MovideskTickets: assunto e cópia (texto livre e e-mails do cliente);
--  - MovideskTicketsActions: descrição da ação (texto livre da conversa);
--  - AfterSaleRefunds: nome, e-mail e documento do cliente, conta de cashback, voucher e link do pedido;
--  - AfterSaleRefundsProducts: comentários do cliente e links da nota de devolução;
--  - AfterSaleReverses: nota do cliente (XML), links com acesso do cliente, cartão de postagem e contato da loja.
--o documento do cliente vai só para dw_personal_data.ServiceCustomerIdentified (Script032).
--a landing de saldos nasce desligada (is_enabled = 0): o job só a lê depois que a carga inicial dos saldos na Totvs terminar
--(1.2, item 15), com update [dw_control].LandingSource set is_enabled = 1 where source_table = 'TotvsBalances'.

insert into [dw_control].LandingSource (source_schema, source_table, included_columns, excluded_columns, source_updated_column, detect_missing, history_frequency, phase, is_enabled)
values
	--saldos: estado atual por loja, produto e depósito, histórico diário (7.4)
	('totvs', 'TotvsBalances', null, null, 'maxChangeFilterDate', 0, 'daily', 2, 0),

	--tickets: as filhas que o merge do Movidesk apaga (@delete_by_ticket) têm detecção de exclusão
	('movidesk', 'MovideskTickets', null, 'subject, cc', 'lastUpdate', 0, null, 2, 1),
	('movidesk', 'MovideskTicketsActions', null, 'description, htmlDescription', null, 1, null, 2, 1),
	('movidesk', 'MovideskTicketsClients', null, null, null, 1, null, 2, 1),

	--reembolsos e reversas: o merge do AfterSale não apaga filhas
	('aftersale', 'AfterSaleRefunds', null,
		'customer_first_name, customer_last_name, customer_email, customer_document, cashback_account, voucher_redemption_code, voucher_giftcard_id, external_order_url',
		'updated_at', 0, null, 2, 1),
	('aftersale', 'AfterSaleRefundsProducts', null,
		'comments, product_received_comment, returned_invoice_url, returned_invoice_url_xml',
		null, 0, null, 2, 1),
	('aftersale', 'AfterSaleReverses', null,
		'ecommerce_phone, ecommerce_email, customer_nfd_attached, customer_nfd_xml_attached, customer_url, timeline_url, collect_scheduling_link, correction_letter_link, courier_data_posting_card, courier_data_encrypted_posting_card',
		'updated_at', 0, null, 2, 1);
GO

declare @schema sysname, @table sysname;
declare sources cursor local fast_forward for
	select source_schema, source_table from [dw_control].LandingSource where phase = 2 order by source_schema, source_table;

open sources;
fetch next from sources into @schema, @table;

while @@fetch_status = 0
begin
	exec [dw_landing].sp_Create_Landing @source_schema = @schema, @source_table = @table;
	fetch next from sources into @schema, @table;
end

close sources;
deallocate sources;
