/****** Dimensões da fase 1 (docs/datawarehouse, 7.1 e 8.3) ******/
--chave substituta: MD5 da chave natural (7.1), com valid_from nas SCD2. Membro desconhecido em todas: sk com 32 zeros,
--chave natural 0 e descrição NOT INFORMED; os fatos gravam esse sk quando a chave não casa.
--as SCD2 (DimStore, DimProduct) são reconstruídas do dw_history a cada carga: a sk é determinística, então os fatos não quebram.
--a primeira versão de cada chave vale desde 1900-01-01, para que vendas anteriores ao primeiro snapshot achem a dimensão.

create table [datawarehouse].DimDate (
	[date]            date           not null,
	date_key          int            not null,   --yyyymmdd
	[year]            smallint       not null,
	[quarter]         tinyint        not null,
	[month]           tinyint        not null,
	month_name        nvarchar(20)   not null,
	[day]             tinyint        not null,
	iso_week          tinyint        not null,
	weekday           tinyint        not null,   --1 segunda ... 7 domingo
	weekday_name      nvarchar(20)   not null,
	is_weekend        bit            not null,
	is_holiday        bit            not null,
	holiday_name      nvarchar(100)  null,
	is_business_day   bit            not null,
constraint [pk_DimDate] primary key ([date] asc)
);

create table [datawarehouse].DimStore (
	sk                char(32)       not null,
	doc_company       nvarchar(14)   not null,
	branch_code       int            not null,
	cnpj              nvarchar(14)   null,
	name              nvarchar(60)   null,
	fantasy_name      nvarchar(60)   null,
	group_name        nvarchar(100)  null,
	brand             nvarchar(60)   null,
	order_series      varchar(5)     null,
	valid_from        datetime2(3)   not null,
	valid_to          datetime2(3)   null,
	is_current        bit            not null,
constraint [pk_DimStore] primary key (sk asc)
);

create table [datawarehouse].DimProduct (
	sk                  char(32)       not null,
	doc_company         nvarchar(14)   not null,
	source_erp          varchar(20)    not null,
	product_code        nvarchar(50)   not null,
	branch_info_code    int            null,
	sku                 nvarchar(60)   null,
	product_name        nvarchar(250)  null,
	reference_id        int            null,
	reference_code      nvarchar(60)   null,
	reference_name      nvarchar(250)  null,
	color_code          nvarchar(60)   null,
	color_name          nvarchar(60)   null,
	size                nvarchar(60)   null,
	category_name       nvarchar(250)  null,
	category_type       nvarchar(20)   null,
	brand               nvarchar(60)   null,
	ncm                 nvarchar(60)   null,
	is_active           bit            null,
	is_blocked          bit            null,
	matched_product_sk  char(32)       null,   --produto Totvs equivalente ao produto Linx (pergunta 34, fase 3)
	valid_from          datetime2(3)   not null,
	valid_to            datetime2(3)   null,
	is_current          bit            not null,
constraint [pk_DimProduct] primary key (sk asc)
);

create table [datawarehouse].DimCustomer (
	sk                char(32)       not null,
	doc_company       nvarchar(14)   not null,
	source_erp        varchar(20)    not null,
	customer_code     nvarchar(50)   not null,
	doc_hash          char(64)       null,      --SHA-256(pepper + CPF/CNPJ); igual nos ERPs para o mesmo documento
	doc_masked        varchar(20)    null,      --***.***.***-12 / **.***.***/****-12
	person_type       char(1)        null,      --F / J
	gender            nvarchar(20)   null,
	state             nvarchar(3)    null,
	age_range         varchar(20)    null,
	customer_status   nvarchar(20)   null,
	updated_at        datetime2(3)   not null,
constraint [pk_DimCustomer] primary key (sk asc)
);

create table [datawarehouse].DimSeller (
	sk                char(32)       not null,
	doc_company       nvarchar(14)   not null,
	source_erp        varchar(20)    not null,
	seller_code       nvarchar(50)   not null,
	name              nvarchar(250)  null,
constraint [pk_DimSeller] primary key (sk asc)
);

create table [datawarehouse].DimSupplier (
	sk                char(32)       not null,
	doc_company       nvarchar(14)   not null,
	supplier_code     int            not null,
	name              nvarchar(250)  null,
constraint [pk_DimSupplier] primary key (sk asc)
);

create table [datawarehouse].DimPaymentMethod (
	sk                   char(32)       not null,
	doc_company          nvarchar(14)   not null,
	source_erp           varchar(20)    not null,
	document_type_code   int            not null,
	document_type        nvarchar(30)   null,
constraint [pk_DimPaymentMethod] primary key (sk asc)
);

create table [datawarehouse].DimOperation (
	sk                char(32)       not null,
	doc_company       nvarchar(14)   not null,
	source_erp        varchar(20)    not null,
	operation_code    int            not null,
	name              nvarchar(100)  null,
	operation_type    nvarchar(20)   null,
constraint [pk_DimOperation] primary key (sk asc)
);

create table [datawarehouse].DimChannel (
	sk                char(32)       not null,
	channel           varchar(30)    not null,
	order_type        varchar(5)     null,
	is_sale           bit            not null,
constraint [pk_DimChannel] primary key (sk asc)
);

create table [datawarehouse].DimBrand (
	sk                char(32)       not null,
	brand             nvarchar(60)   not null,
	order_series      varchar(5)     null,
constraint [pk_DimBrand] primary key (sk asc)
);
GO

/****** datawarehouse.sp_DimDate_Load ******/
create or alter procedure [datawarehouse].sp_DimDate_Load
	@from date = '20100101',
	@to   date = '20351231',
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimDate where [date] between @from and @to;

	with days as (
		select top (datediff(day, @from, @to) + 1) dateadd(day, row_number() over (order by (select null)) - 1, @from) as [date]
		  from sys.all_objects a cross join sys.all_objects b
	)
	insert into [datawarehouse].DimDate ([date], date_key, [year], [quarter], [month], month_name, [day], iso_week, weekday, weekday_name, is_weekend, is_holiday, holiday_name, is_business_day)
	select d.[date],
	       year(d.[date]) * 10000 + month(d.[date]) * 100 + day(d.[date]),
	       year(d.[date]), datepart(quarter, d.[date]), month(d.[date]),
	       choose(month(d.[date]), N'janeiro', N'fevereiro', N'março', N'abril', N'maio', N'junho', N'julho', N'agosto', N'setembro', N'outubro', N'novembro', N'dezembro'),
	       day(d.[date]), datepart(iso_week, d.[date]),
	       w.weekday,
	       choose(w.weekday, N'segunda-feira', N'terça-feira', N'quarta-feira', N'quinta-feira', N'sexta-feira', N'sábado', N'domingo'),
	       iif(w.weekday >= 6, 1, 0),
	       iif(h.holiday_date is null, 0, 1),
	       h.name,
	       iif(w.weekday >= 6 or h.holiday_date is not null, 0, 1)
	  from days d
	  cross apply (select (datediff(day, '19000101', d.[date]) % 7) + 1 as weekday) w   --1900-01-01 foi segunda-feira
	  left join [dw_reference].Holidays h on h.holiday_date = d.[date];

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimStore_Load (SCD2, do dw_history.TotvsBranches) ******/
create or alter procedure [datawarehouse].sp_DimStore_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimStore;

	insert into [datawarehouse].DimStore (sk, doc_company, branch_code, cnpj, name, fantasy_name, group_name, brand, order_series, valid_from, valid_to, is_current)
	values (replicate('0', 32), N'0', 0, null, N'NOT INFORMED', N'NOT INFORMED', null, null, null, '19000101', null, 1);

	insert into [datawarehouse].DimStore (sk, doc_company, branch_code, cnpj, name, fantasy_name, group_name, brand, order_series, valid_from, valid_to, is_current)
	select convert(char(32), hashbytes('MD5', concat_ws('|', h.doc_company, h.[code], convert(varchar(23), h.valid_from, 121))), 2),
	       h.doc_company, h.[code], h.cnpj, h.[description], h.fantasyName, h.branchGroupName, b.brand, b.order_series,
	       iif(row_number() over (partition by h.doc_company, h.[code] order by h.valid_from) = 1, '19000101', h.valid_from),
	       h.valid_to, h.is_current
	  from [dw_history].TotvsBranches h
	  left join [dw_reference].BrandSeries b on b.cnpj_root = left(h.cnpj, 8);

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimProduct_Load (SCD2, do dw_history.TotvsSkusColorsProducts) ******/
--referência, cor e categoria vêm da landing (estado atual); as versões são as do produto. Se o mesmo código de produto
--aparece em duas matrizes (branchInfoCode), fica a de menor código: as notas só trazem o productCode.
create or alter procedure [datawarehouse].sp_DimProduct_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimProduct where source_erp in ('totvs', '0');

	insert into [datawarehouse].DimProduct (sk, doc_company, source_erp, product_code, product_name, valid_from, valid_to, is_current)
	values (replicate('0', 32), N'0', '0', N'0', N'NOT INFORMED', '19000101', null, 1);

	with products as (
		select h.*,
		       dense_rank() over (partition by h.doc_company, h.[code] order by h.branchInfoCode) as matrix_rank
		  from [dw_history].TotvsSkusColorsProducts h
	)
	insert into [datawarehouse].DimProduct (sk, doc_company, source_erp, product_code, branch_info_code, sku, product_name, reference_id, reference_code, reference_name,
	                                        color_code, color_name, size, category_name, category_type, brand, ncm, is_active, is_blocked, valid_from, valid_to, is_current)
	select convert(char(32), hashbytes('MD5', concat_ws('|', p.doc_company, 'totvs', p.[code], convert(varchar(23), p.valid_from, 121))), 2),
	       p.doc_company, 'totvs', cast(p.[code] as nvarchar(50)), p.branchInfoCode, p.sku, p.[name], p.referenceId, s.referenceCode, s.[name],
	       p.colorCode, c.[name], p.size, rc.[name], rc.[type], b.brand, p.ncm, p.isActive, p.isBlocked,
	       iif(row_number() over (partition by p.doc_company, p.[code] order by p.valid_from) = 1, '19000101', p.valid_from),
	       p.valid_to, p.is_current
	  from products p
	  left join [dw_landing].TotvsSkus s on s.doc_company = p.doc_company and s.branchInfoCode = p.branchInfoCode and s.referenceId = p.referenceId
	  left join [dw_landing].TotvsSkusColors c on c.doc_company = p.doc_company and c.branchInfoCode = p.branchInfoCode and c.referenceId = p.referenceId and c.colorCode = p.colorCode
	  outer apply (select top 1 x.[name], x.[type] from [dw_landing].TotvsSkusReferenceCategories x
	                where x.doc_company = p.doc_company and x.branchInfoCode = p.branchInfoCode and x.referenceId = p.referenceId and x.missing_since is null
	                order by x.[code]) rc
	  left join [dw_landing].TotvsBranches br on br.doc_company = p.doc_company and br.[code] = p.branchInfoCode
	  left join [dw_reference].BrandSeries b on b.cnpj_root = left(br.cnpj, 8)
	 where p.matrix_rank = 1;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimCustomer_Load (SCD1, de dw_personal_data, com o pepper) ******/
--incremental pelo updated_at de dw_personal_data.DimCustomerIdentified (watermark source = datawarehouse, entity = DimCustomer).
--o pepper chega como parâmetro e não é gravado em lugar nenhum (12.3, pergunta 32). Sem pepper, não carrega.
create or alter procedure [datawarehouse].sp_DimCustomer_Load
	@pepper nvarchar(200),
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	if nullif(@pepper, '') is null
		throw 50030, 'sp_DimCustomer_Load: o pepper é obrigatório; sem ele o doc_hash não liga os ERPs e não protege o documento.', 1;

	declare @since datetime2(3), @from datetime2(3), @until datetime2(3),
	        @today date = cast(sysutcdatetime() at time zone 'UTC' at time zone 'E. South America Standard Time' as date);

	exec [dw_control].sp_Watermark_Get N'datawarehouse', N'DimCustomer', @since output;
	set @from = isnull(dateadd(day, -3, @since), '19000101');
	set @rows = 0;

	select @until = max(updated_at) from [dw_personal_data].DimCustomerIdentified where updated_at >= @from;

	begin transaction;

	if not exists (select 1 from [datawarehouse].DimCustomer where sk = replicate('0', 32))
		insert into [datawarehouse].DimCustomer (sk, doc_company, source_erp, customer_code, updated_at)
		values (replicate('0', 32), N'0', '0', N'0', '19000101');

	if @until is not null
	begin
		merge [datawarehouse].DimCustomer as target
		using (
			select convert(char(32), hashbytes('MD5', concat_ws('|', d.doc_company, d.source_erp, d.customer_code)), 2) as sk,
			       d.doc_company, d.source_erp, d.customer_code,
			       iif(nullif(d.tax_id, '') is null, null, convert(char(64), hashbytes('SHA2_256', concat(@pepper, d.tax_id)), 2)) as doc_hash,
			       case len(d.tax_id)
			            when 11 then concat('***.***.***-', right(d.tax_id, 2))
			            when 14 then concat('**.***.***/****-', right(d.tax_id, 2))
			       end as doc_masked,
			       d.person_type, p.gender, d.[state],
			       case
			            when d.birth_date is null then null
			            when datediff(year, d.birth_date, @today) - iif(dateadd(year, datediff(year, d.birth_date, @today), d.birth_date) > @today, 1, 0) < 18 then '< 18'
			            when datediff(year, d.birth_date, @today) - iif(dateadd(year, datediff(year, d.birth_date, @today), d.birth_date) > @today, 1, 0) < 25 then '18-24'
			            when datediff(year, d.birth_date, @today) - iif(dateadd(year, datediff(year, d.birth_date, @today), d.birth_date) > @today, 1, 0) < 35 then '25-34'
			            when datediff(year, d.birth_date, @today) - iif(dateadd(year, datediff(year, d.birth_date, @today), d.birth_date) > @today, 1, 0) < 45 then '35-44'
			            when datediff(year, d.birth_date, @today) - iif(dateadd(year, datediff(year, d.birth_date, @today), d.birth_date) > @today, 1, 0) < 55 then '45-54'
			            when datediff(year, d.birth_date, @today) - iif(dateadd(year, datediff(year, d.birth_date, @today), d.birth_date) > @today, 1, 0) < 65 then '55-64'
			            else '65+'
			       end as age_range,
			       coalesce(p.customerStatus, l.customerStatus) as customer_status,
			       d.updated_at
			  from [dw_personal_data].DimCustomerIdentified d
			  left join [dw_landing].TotvsPeople p on d.source_erp = 'totvs' and d.person_type = 'F' and p.doc_company = d.doc_company and cast(p.[code] as nvarchar(50)) = d.customer_code
			  left join [dw_landing].TotvsLegalEntities l on d.source_erp = 'totvs' and d.person_type = 'J' and l.doc_company = d.doc_company and cast(l.[code] as nvarchar(50)) = d.customer_code
			 where d.updated_at >= @from
		) as source
		on (target.sk = source.sk)
		when matched then
			update set doc_hash = source.doc_hash, doc_masked = source.doc_masked, person_type = source.person_type, gender = source.gender,
			           [state] = source.[state], age_range = source.age_range, customer_status = source.customer_status, updated_at = source.updated_at
		when not matched by target then
			insert (sk, doc_company, source_erp, customer_code, doc_hash, doc_masked, person_type, gender, [state], age_range, customer_status, updated_at)
			values (source.sk, source.doc_company, source.source_erp, source.customer_code, source.doc_hash, source.doc_masked, source.person_type, source.gender,
			        source.[state], source.age_range, source.customer_status, source.updated_at);

		set @rows = @@rowcount;

		exec [dw_control].sp_Watermark_Set N'datawarehouse', N'DimCustomer', @until;
	end

	commit transaction;
end;
GO

/****** Dimensões SCD1 reconstruídas da landing: vendedor, fornecedor, forma de pagamento, operação, canal e marca ******/
create or alter procedure [datawarehouse].sp_DimSeller_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimSeller where source_erp in ('totvs', '0');

	insert into [datawarehouse].DimSeller (sk, doc_company, source_erp, seller_code, name)
	values (replicate('0', 32), N'0', '0', N'0', N'NOT INFORMED');

	insert into [datawarehouse].DimSeller (sk, doc_company, source_erp, seller_code, name)
	select convert(char(32), hashbytes('MD5', concat_ws('|', s.doc_company, 'totvs', s.sellerCode)), 2),
	       s.doc_company, 'totvs', cast(s.sellerCode as nvarchar(50)), s.sellerName
	  from [dw_landing].TotvsSellers s;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

create or alter procedure [datawarehouse].sp_DimSupplier_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimSupplier;

	insert into [datawarehouse].DimSupplier (sk, doc_company, supplier_code, name)
	values (replicate('0', 32), N'0', 0, N'NOT INFORMED');

	insert into [datawarehouse].DimSupplier (sk, doc_company, supplier_code, name)
	select convert(char(32), hashbytes('MD5', concat_ws('|', s.doc_company, s.[code])), 2), s.doc_company, s.[code], max(s.[name])
	  from [dw_landing].TotvsProductsSuppliers s
	 where s.missing_since is null
	 group by s.doc_company, s.[code];

	set @rows = @@rowcount;

	commit transaction;
end;
GO

create or alter procedure [datawarehouse].sp_DimPaymentMethod_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimPaymentMethod where source_erp in ('totvs', '0');

	insert into [datawarehouse].DimPaymentMethod (sk, doc_company, source_erp, document_type_code, document_type)
	values (replicate('0', 32), N'0', '0', 0, N'NOT INFORMED');

	insert into [datawarehouse].DimPaymentMethod (sk, doc_company, source_erp, document_type_code, document_type)
	select convert(char(32), hashbytes('MD5', concat_ws('|', p.doc_company, 'totvs', p.documentTypeCode)), 2), p.doc_company, 'totvs', p.documentTypeCode, max(p.documentType)
	  from [dw_landing].TotvsInvoicesPayments p
	 where p.documentTypeCode is not null
	 group by p.doc_company, p.documentTypeCode;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

create or alter procedure [datawarehouse].sp_DimOperation_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimOperation where source_erp in ('totvs', '0');

	insert into [datawarehouse].DimOperation (sk, doc_company, source_erp, operation_code, name, operation_type)
	values (replicate('0', 32), N'0', '0', 0, N'NOT INFORMED', null);

	insert into [datawarehouse].DimOperation (sk, doc_company, source_erp, operation_code, name, operation_type)
	select convert(char(32), hashbytes('MD5', concat_ws('|', o.doc_company, 'totvs', o.operation_code)), 2), o.doc_company, 'totvs', o.operation_code, max(o.name), max(o.operation_type)
	  from (
		select doc_company, operationCode as operation_code, operatioName as name, operationType as operation_type from [dw_landing].TotvsInvoices where operationCode is not null
		union all
		select doc_company, operationCode, operationName, null from [dw_landing].TotvsSaleOrders where operationCode is not null
	  ) o
	 group by o.doc_company, o.operation_code;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

create or alter procedure [datawarehouse].sp_DimChannel_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimChannel;

	insert into [datawarehouse].DimChannel (sk, channel, order_type, is_sale)
	values (replicate('0', 32), 'NOT INFORMED', null, 1);

	--as combinações das regras e a venda de loja sem regra (nota sem pedido e sem PDV)
	insert into [datawarehouse].DimChannel (sk, channel, order_type, is_sale)
	select convert(char(32), hashbytes('MD5', concat_ws('|', c.channel, isnull(c.order_type, '-'))), 2), c.channel, c.order_type, max(cast(c.is_sale as int))
	  from (
		select channel, order_type, is_sale from [dw_reference].ChannelRules
		union all
		select 'store', null, 1
	  ) c
	 group by c.channel, c.order_type;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

create or alter procedure [datawarehouse].sp_DimBrand_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimBrand;

	insert into [datawarehouse].DimBrand (sk, brand, order_series)
	values (replicate('0', 32), N'NOT INFORMED', null);

	insert into [datawarehouse].DimBrand (sk, brand, order_series)
	select convert(char(32), hashbytes('MD5', b.brand), 2), b.brand, min(b.order_series)
	  from [dw_reference].BrandSeries b
	 group by b.brand;

	set @rows = @@rowcount;

	commit transaction;
end;
