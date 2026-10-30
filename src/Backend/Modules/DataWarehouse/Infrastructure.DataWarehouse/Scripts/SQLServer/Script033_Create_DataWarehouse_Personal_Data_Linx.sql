/****** Dado pessoal do legado Linx Microvix (docs/datawarehouse, 5.2 regra 6, 7.5 e 8.3) ******/
/****** dw_personal_data.sp_LinxCustomer_Load ******/
--MERGE dos clientes de linxmicrovix.LinxClientesFornec em dw_personal_data.DimCustomerIdentified, com source_erp =
--linxmicrovix, alterados desde o watermark menos 3 dias. Fornecedores e transportadores (tipo_cadastro F e T) ficam fora.
--Com o mesmo código em mais de um portal, vale o de menor portal. DimCustomer calcula o doc_hash desses clientes como os da
--Totvs, e é pelo hash que o cliente Linx se liga ao da Totvs (7.5). Watermark: source = dw_personal_data, entity = LinxCustomers.
create or alter procedure [dw_personal_data].sp_LinxCustomer_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @since datetime2(3), @from datetime, @until datetime;

	exec [dw_control].sp_Watermark_Get N'dw_personal_data', N'LinxCustomers', @since output;
	set @from = isnull(dateadd(day, -3, @since), '19000101');
	set @rows = 0;

	select c.*
	  into #changed
	  from (select x.*, row_number() over (partition by x.doc_company, x.cod_cliente order by x.portal) as rn
	          from [linxmicrovix].LinxClientesFornec x
	         where isnull(x.tipo_cadastro, 'C') not in ('F', 'T')
	           and isnull(x.integrated_or_updated_at, '19000101') >= @from) c
	 where c.rn = 1;

	select @until = max(integrated_or_updated_at) from #changed;

	if @until is null
		return;

	begin transaction;

	merge [dw_personal_data].DimCustomerIdentified as target
	using (
		select c.doc_company, 'linxmicrovix' as source_erp, cast(c.cod_cliente as nvarchar(50)) as customer_code,
		       iif(c.tipo_cliente = 'J', 'J', 'F') as person_type,
		       left(coalesce(nullif(trim(c.nome_cliente), ''), c.razao_cliente), 100) as [name],
		       nullif(replace(replace(replace(replace(c.doc_cliente, '.', ''), '-', ''), '/', ''), ' ', ''), '') as tax_id,
		       cast(c.data_nascimento as date) as birth_date,
		       left(nullif(trim(c.email_cliente), ''), 100) as email,
		       left(coalesce(nullif(trim(c.cel_cliente), ''), nullif(trim(c.fone_cliente), '')), 20) as phone,
		       nullif(trim(concat(c.endereco_cliente, iif(nullif(c.numero_rua_cliente, '') is null, '', concat(', ', c.numero_rua_cliente)),
		                          iif(nullif(c.complement_end_cli, '') is null, '', concat(' - ', c.complement_end_cli)))), '') as [address],
		       c.bairro_cliente as neighborhood, c.cidade_cliente as city, left(nullif(trim(c.uf_cliente), ''), 3) as [state],
		       left(nullif(replace(c.cep_cliente, '-', ''), ''), 8) as zip_code,
		       isnull(c.integrated_or_updated_at, '19000101') as updated_at
		  from #changed c
	) as source
	on (target.doc_company = source.doc_company and target.source_erp = source.source_erp and target.customer_code = source.customer_code)
	when matched then
		update set person_type = source.person_type, [name] = source.[name], tax_id = source.tax_id, birth_date = source.birth_date,
		           email = source.email, phone = source.phone, [address] = source.[address], neighborhood = source.neighborhood,
		           city = source.city, [state] = source.[state], zip_code = source.zip_code, updated_at = source.updated_at
	when not matched by target then
		insert (doc_company, source_erp, customer_code, person_type, [name], tax_id, birth_date, email, phone, [address], neighborhood, city, [state], zip_code, updated_at)
		values (source.doc_company, source.source_erp, source.customer_code, source.person_type, source.[name], source.tax_id, source.birth_date, source.email,
		        source.phone, source.[address], source.neighborhood, source.city, source.[state], source.zip_code, source.updated_at);

	set @rows = @@rowcount;

	exec [dw_control].sp_Watermark_Set N'dw_personal_data', N'LinxCustomers', @until;

	commit transaction;
end;
GO
