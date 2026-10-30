/****** Dado pessoal em claro (src/Backend/Modules/DataWarehouse/docs, 5.2 regra 6, 8.3 e seção 12) ******/
--só a identificação atual, sem versões, lida direto das tabelas de origem (nunca da landing, que não tem essas colunas).
--só o grupo dw_pii lê este schema; as únicas saídas são dw_outbound.OtoCustomers e dw_outbound.LogisticsShipments (4.2).

create table [dw_personal_data].DimCustomerIdentified (
	doc_company      nvarchar(14)   not null,
	source_erp       varchar(20)    not null,   --totvs (linxmicrovix e linxcommerce na fase 3)
	customer_code    nvarchar(50)   not null,   --code em TotvsPeople e TotvsLegalEntities (personCode nas notas)
	person_type      char(1)        not null,   --F (TotvsPeople) / J (TotvsLegalEntities)
	name             nvarchar(100)  null,
	tax_id           nvarchar(14)   null,       --CPF/CNPJ em claro; é daqui que sai o doc_hash de DimCustomer
	birth_date       date           null,       --vira age_range em DimCustomer
	email            nvarchar(100)  null,       --o padrão (isDefault), senão o de menor sequência
	phone            nvarchar(20)   null,       --idem
	address          nvarchar(250)  null,       --o endereço de menor tipo e sequência: logradouro, número e complemento
	neighborhood     nvarchar(65)   null,
	city             nvarchar(60)   null,
	state            nvarchar(3)    null,       --copiada para DimCustomer
	zip_code         nvarchar(8)    null,
	updated_at       datetime2(3)   not null,   --maior integrated_or_updated_at da pessoa e das filhas, horário de Brasília
constraint [pk_DimCustomerIdentified] primary key (doc_company asc, source_erp asc, customer_code asc)
);

--endereço de entrega do pedido, que a logística envia à transportadora (4.2, 10.3). Sai da landing de TotvsSaleOrders
create table [dw_personal_data].OrderShippingAddress (
	doc_company      nvarchar(14)   not null,
	branch_code      int            not null,
	order_code       int            not null,
	customer_name    nvarchar(60)   null,
	customer_tax_id  nvarchar(14)   null,
	address_type     nvarchar(20)   null,
	address          nvarchar(250)  null,
	neighborhood     nvarchar(65)   null,
	city             nvarchar(60)   null,
	ibge_city_code   int            null,
	state            nvarchar(3)    null,
	zip_code         nvarchar(8)    null,
	country          nvarchar(60)   null,
	reference        nvarchar(60)   null,
	updated_at       datetime2(3)   not null,
constraint [pk_OrderShippingAddress] primary key (doc_company asc, branch_code asc, order_code asc)
);
GO

/****** dw_personal_data.sp_Customer_Load ******/
--MERGE das pessoas (F) e empresas (J) da Totvs que mudaram, ou cujo endereço, e-mail ou telefone mudou, desde o watermark
--menos 3 dias (5.2, regra 1). Watermark: source = dw_personal_data, entity = TotvsCustomers.
create or alter procedure [dw_personal_data].sp_Customer_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @since datetime2(3), @from datetime, @until datetime;

	exec [dw_control].sp_Watermark_Get N'dw_personal_data', N'TotvsCustomers', @since output;
	set @from = isnull(dateadd(day, -3, @since), '19000101');
	set @rows = 0;

	--chaves alteradas na pessoa ou nas filhas
	select doc_company, code, person_type, max(changed_at) as changed_at
	  into #changed
	  from (
		select doc_company, [code], 'F' as person_type, integrated_or_updated_at as changed_at from [totvs].TotvsPeople where isnull(integrated_or_updated_at, '19000101') >= @from
		union all select doc_company, personCode, 'F', integrated_or_updated_at from [totvs].TotvsPeopleAddresses where isnull(integrated_or_updated_at, '19000101') >= @from
		union all select doc_company, personCode, 'F', integrated_or_updated_at from [totvs].TotvsPeopleEmails where isnull(integrated_or_updated_at, '19000101') >= @from
		union all select doc_company, personCode, 'F', integrated_or_updated_at from [totvs].TotvsPeoplePhones where isnull(integrated_or_updated_at, '19000101') >= @from
		union all select doc_company, [code], 'J', integrated_or_updated_at from [totvs].TotvsLegalEntities where isnull(integrated_or_updated_at, '19000101') >= @from
		union all select doc_company, personCode, 'J', integrated_or_updated_at from [totvs].TotvsLegalEntitiesAddresses where isnull(integrated_or_updated_at, '19000101') >= @from
		union all select doc_company, personCode, 'J', integrated_or_updated_at from [totvs].TotvsLegalEntitiesEmails where isnull(integrated_or_updated_at, '19000101') >= @from
		union all select doc_company, personCode, 'J', integrated_or_updated_at from [totvs].TotvsLegalEntitiesPhones where isnull(integrated_or_updated_at, '19000101') >= @from
	  ) x
	 group by doc_company, code, person_type;

	select @until = max(changed_at) from #changed;

	if @until is null
		return;

	begin transaction;

	merge [dw_personal_data].DimCustomerIdentified as target
	using (
		select p.doc_company, 'totvs' as source_erp, cast(p.[code] as nvarchar(50)) as customer_code, 'F' as person_type,
		       p.[name], p.cpf as tax_id, cast(p.birthDate as date) as birth_date,
		       e.email, ph.[number] as phone,
		       nullif(trim(concat(a.publicPlace, ' ', a.[address], iif(a.addressNumber is null, '', concat(', ', a.addressNumber)), iif(nullif(a.complement, '') is null, '', concat(' - ', a.complement)))), '') as [address],
		       a.neighborhood, a.cityName as city, a.stateAbbreviation as [state], a.cep as zip_code,
		       isnull(c.changed_at, '19000101') as updated_at
		  from #changed c
		  join [totvs].TotvsPeople p on p.doc_company = c.doc_company and p.[code] = c.code and c.person_type = 'F'
		  outer apply (select top 1 * from [totvs].TotvsPeopleAddresses x where x.doc_company = p.doc_company and x.personCode = p.[code] order by x.addressTypeCode, x.sequenceCode) a
		  outer apply (select top 1 x.email from [totvs].TotvsPeopleEmails x where x.doc_company = p.doc_company and x.personCode = p.[code] order by x.isDefault desc, x.[sequence]) e
		  outer apply (select top 1 x.[number] from [totvs].TotvsPeoplePhones x where x.doc_company = p.doc_company and x.personCode = p.[code] order by x.isDefault desc, x.[sequence]) ph
		union all
		select l.doc_company, 'totvs', cast(l.[code] as nvarchar(50)), 'J',
		       l.[name], l.cnpj, null,
		       e.email, ph.[number],
		       nullif(trim(concat(a.publicPlace, ' ', a.[address], iif(a.addressNumber is null, '', concat(', ', a.addressNumber)), iif(nullif(a.complement, '') is null, '', concat(' - ', a.complement)))), ''),
		       a.neighborhood, a.cityName, isnull(a.stateAbbreviation, l.uf), a.cep,
		       isnull(c.changed_at, '19000101')
		  from #changed c
		  join [totvs].TotvsLegalEntities l on l.doc_company = c.doc_company and l.[code] = c.code and c.person_type = 'J'
		  outer apply (select top 1 * from [totvs].TotvsLegalEntitiesAddresses x where x.doc_company = l.doc_company and x.personCode = l.[code] order by x.addressTypeCode, x.sequenceCode) a
		  outer apply (select top 1 x.email from [totvs].TotvsLegalEntitiesEmails x where x.doc_company = l.doc_company and x.personCode = l.[code] order by x.isDefault desc, x.[sequence]) e
		  outer apply (select top 1 x.[number] from [totvs].TotvsLegalEntitiesPhones x where x.doc_company = l.doc_company and x.personCode = l.[code] order by x.isDefault desc, x.[sequence]) ph
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

	exec [dw_control].sp_Watermark_Set N'dw_personal_data', N'TotvsCustomers', @until;

	commit transaction;
end;
GO

/****** dw_personal_data.sp_OrderShippingAddress_Load ******/
--MERGE do endereço de entrega dos pedidos alterados desde o watermark menos 3 dias. Lê totvs.TotvsSaleOrders, porque a
--landing não tem essas colunas (5.2, regra 6). Roda no ciclo curto da logística (10.3). Watermark: entity = TotvsSaleOrders.
create or alter procedure [dw_personal_data].sp_OrderShippingAddress_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @since datetime2(3), @from datetime, @until datetime;

	exec [dw_control].sp_Watermark_Get N'dw_personal_data', N'TotvsSaleOrders', @since output;
	set @from = isnull(dateadd(day, -3, @since), '19000101');
	set @rows = 0;

	select @until = max(integrated_or_updated_at) from [totvs].TotvsSaleOrders where isnull(integrated_or_updated_at, '19000101') >= @from;

	if @until is null
		return;

	begin transaction;

	merge [dw_personal_data].OrderShippingAddress as target
	using (
		select o.doc_company, o.branchCode as branch_code, o.orderCode as order_code,
		       o.customerName as customer_name, o.customerCpfCnpj as customer_tax_id, o.shippingAddressAddressType as address_type,
		       nullif(trim(concat(o.shippingAddressPublicPlace, ' ', o.shippingAddressAddress, iif(o.shippingAddressNumber is null, '', concat(', ', o.shippingAddressNumber)), iif(nullif(o.shippingAddressComplement, '') is null, '', concat(' - ', o.shippingAddressComplement)))), '') as [address],
		       o.shippingAddressNeighborhood as neighborhood, o.shippingAddressCityName as city, o.shippingAddressIbgeCityCode as ibge_city_code,
		       o.shippingAddressStateAbbreviation as [state], o.shippingAddressCep as zip_code, o.shippingAddressCountryName as country,
		       o.shippingAddressReference as reference, isnull(o.integrated_or_updated_at, '19000101') as updated_at
		  from [totvs].TotvsSaleOrders o
		 where isnull(o.integrated_or_updated_at, '19000101') >= @from
	) as source
	on (target.doc_company = source.doc_company and target.branch_code = source.branch_code and target.order_code = source.order_code)
	when matched and target.updated_at <> source.updated_at then
		update set customer_name = source.customer_name, customer_tax_id = source.customer_tax_id, address_type = source.address_type,
		           [address] = source.[address], neighborhood = source.neighborhood, city = source.city, ibge_city_code = source.ibge_city_code,
		           [state] = source.[state], zip_code = source.zip_code, country = source.country, reference = source.reference, updated_at = source.updated_at
	when not matched by target then
		insert (doc_company, branch_code, order_code, customer_name, customer_tax_id, address_type, [address], neighborhood, city, ibge_city_code, [state], zip_code, country, reference, updated_at)
		values (source.doc_company, source.branch_code, source.order_code, source.customer_name, source.customer_tax_id, source.address_type, source.[address],
		        source.neighborhood, source.city, source.ibge_city_code, source.[state], source.zip_code, source.country, source.reference, source.updated_at);

	set @rows = @@rowcount;

	exec [dw_control].sp_Watermark_Set N'dw_personal_data', N'TotvsSaleOrders', @until;

	commit transaction;
end;
