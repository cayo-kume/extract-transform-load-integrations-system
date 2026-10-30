/****** Landing das origens da fase 4 (docs/datawarehouse, 7.6) ******/
--compras, contas a pagar, recepção de loja, representantes, áreas operacionais, promoções e as cinco tabelas de log das
--chamadas às APIs (*Requests). Mesmo catálogo e mesmo gerador das fases anteriores (9.5), ligadas no job agendado.
--dado pessoal identificador fica fora (5.2, regra 6):
--  - TotvsPurchaseOrders e TotvsDuplicates: CPF/CNPJ do fornecedor e do comprador;
--  - TotvsReceptionSales: nome do cliente e a observação (texto livre);
--  - TotvsRepresentatives: CPF/CNPJ do representante;
--  - *Requests: corpo da requisição e da resposta, que levam o dado enviado ao SaaS (8.3).
--TotvsCommissionsPaid não tem landing: o CPF/CNPJ do comissionado está na chave da origem, e a landing repete a chave.
--FactCommission lê a origem e guarda só o doc_hash (Script043).

insert into [dw_control].LandingSource (source_schema, source_table, included_columns, excluded_columns, source_updated_column, detect_missing, history_frequency, phase, is_enabled)
values
	--compras: os produtos do pedido são apagados pelo merge da Totvs quando o pedido muda
	('totvs', 'TotvsPurchaseOrders', null, 'supplierCpfCnpj, buyerCpfCnpj', 'maxChangeFilterDate', 0, null, 4, 1),
	('totvs', 'TotvsPurchaseOrdersProducts', null, null, null, 1, null, 4, 1),

	--contas a pagar
	('totvs', 'TotvsDuplicates', null, 'supplierCpfCnpj', 'maxChangeFilterDate', 0, null, 4, 1),

	--recepção de loja, representantes, áreas e promoções
	('totvs', 'TotvsReceptionSales', null, 'customerName, observation', null, 0, null, 4, 1),
	('totvs', 'TotvsRepresentatives', null, 'cpfCnpj', 'maxChangeFilterDate', 0, null, 4, 1),
	('totvs', 'TotvsOperationalAreas', null, null, null, 0, null, 4, 1),
	('totvs', 'TotvsPromotions', null, null, 'maxChangeFilterDate', 0, null, 4, 1),

	--log das chamadas às APIs dos SaaS (1.2, item 10)
	('totvs', 'TotvsRequests', null, 'request_body, response_body', null, 0, null, 4, 1),
	('linxmicrovix', 'LinxMicrovixRequests', null, 'request_body, response_body', null, 0, null, 4, 1),
	('linxcommerce', 'LinxCommerceRequests', null, 'request_body, response_body', null, 0, null, 4, 1),
	('movidesk', 'MovideskRequests', null, 'request_body, response_body', null, 0, null, 4, 1),
	('aftersale', 'AfterSaleRequests', null, 'request_body, response_body', null, 0, null, 4, 1);
GO

declare @schema sysname, @table sysname;
declare sources cursor local fast_forward for
	select source_schema, source_table from [dw_control].LandingSource where phase = 4 order by source_schema, source_table;

open sources;
fetch next from sources into @schema, @table;

while @@fetch_status = 0
begin
	exec [dw_landing].sp_Create_Landing @source_schema = @schema, @source_table = @table;
	fetch next from sources into @schema, @table;
end

close sources;
deallocate sources;
