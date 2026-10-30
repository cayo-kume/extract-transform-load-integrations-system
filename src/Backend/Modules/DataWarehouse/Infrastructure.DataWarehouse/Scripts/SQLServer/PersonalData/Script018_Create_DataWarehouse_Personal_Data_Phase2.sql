/****** Dado pessoal da fase 2: documento do cliente do atendimento e do pós-venda (docs/datawarehouse, 5.2 regra 6, 8.3 e 12) ******/
--Movidesk e AfterSale trazem o CPF/CNPJ do cliente. Ele não entra na landing (Script014): fica só aqui, carregado direto das
--tabelas de origem, e os fatos levam só o doc_hash, que liga o ticket e o reembolso ao cliente da Totvs em DimCustomer.
create table [dw_personal_data].ServiceCustomerIdentified (
	source_system    varchar(20)    not null,   --movidesk, aftersale
	doc_company      nvarchar(14)   not null,
	customer_id      nvarchar(64)   not null,   --MovideskPeople.id; AfterSaleRefunds.customer_id
	tax_id           nvarchar(14)   null,       --CPF/CNPJ em claro, só dígitos
	updated_at       datetime2(3)   not null,
constraint [pk_ServiceCustomerIdentified] primary key (source_system asc, doc_company asc, customer_id asc)
);
GO

/****** datawarehouse.fn_DocHash: SHA-256(pepper + CPF/CNPJ) ******/
--a mesma conta de sp_DimCustomer_Load (Script031): o mesmo documento dá o mesmo hash em qualquer origem. Tira pontos, traços,
--barras e espaços, porque Movidesk e AfterSale podem trazer o documento formatado; a Totvs já traz só dígitos.
create or alter function [datawarehouse].fn_DocHash (
	@pepper    nvarchar(200),
	@document  nvarchar(20)
)
returns char(64)
with schemabinding
as
begin
	declare @digits nvarchar(20) = nullif(replace(replace(replace(replace(@document, '.', ''), '-', ''), '/', ''), ' ', ''), '');

	return iif(@digits is null, null, convert(char(64), hashbytes('SHA2_256', concat(@pepper, @digits)), 2));
end;
GO

/****** dw_personal_data.sp_ServiceCustomer_Load ******/
--MERGE do documento das pessoas do Movidesk e dos clientes dos reembolsos do AfterSale alterados desde o watermark menos 3 dias
--(5.2, regra 1). Do Movidesk entram só os clientes (profileType 2 e 3); agentes são funcionários. Watermark:
--source = dw_personal_data, entity = ServiceCustomers.
create or alter procedure [dw_personal_data].sp_ServiceCustomer_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @since datetime2(3), @from datetime, @until datetime;

	exec [dw_control].sp_Watermark_Get N'dw_personal_data', N'ServiceCustomers', @since output;
	set @from = isnull(dateadd(day, -3, @since), '19000101');
	set @rows = 0;

	select source_system, doc_company, customer_id, tax_id, changed_at
	  into #changed
	  from (
		select 'movidesk' as source_system, p.doc_company, p.id as customer_id,
		       nullif(replace(replace(replace(replace(p.cpfCnpj, '.', ''), '-', ''), '/', ''), ' ', ''), '') as tax_id,
		       isnull(p.integrated_or_updated_at, '19000101') as changed_at
		  from [movidesk].MovideskPeople p
		 where p.profileType in (2, 3)
		   and isnull(p.integrated_or_updated_at, '19000101') >= @from
		union all
		--o mesmo cliente aparece em vários reembolsos: vale o documento do reembolso alterado por último
		select 'aftersale', r.doc_company, cast(r.customer_id as nvarchar(64)),
		       nullif(replace(replace(replace(replace(r.customer_document, '.', ''), '-', ''), '/', ''), ' ', ''), ''),
		       isnull(r.integrated_or_updated_at, '19000101')
		  from (select *, row_number() over (partition by doc_company, customer_id order by integrated_or_updated_at desc, id desc) as rn
		          from [aftersale].AfterSaleRefunds
		         where customer_id is not null
		           and isnull(integrated_or_updated_at, '19000101') >= @from) r
		 where r.rn = 1
	  ) x;

	select @until = max(changed_at) from #changed;

	if @until is null
		return;

	begin transaction;

	merge [dw_personal_data].ServiceCustomerIdentified as target
	using #changed as source
	on (target.source_system = source.source_system and target.doc_company = source.doc_company and target.customer_id = source.customer_id)
	when matched then
		update set tax_id = source.tax_id, updated_at = source.changed_at
	when not matched by target then
		insert (source_system, doc_company, customer_id, tax_id, updated_at)
		values (source.source_system, source.doc_company, source.customer_id, source.tax_id, source.changed_at);

	set @rows = @@rowcount;

	exec [dw_control].sp_Watermark_Set N'dw_personal_data', N'ServiceCustomers', @until;

	commit transaction;
end;
GO
