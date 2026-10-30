/****** Membros Linx nas dimensões (docs/datawarehouse, 7.1, 7.5 e pergunta 34) ******/
--as procedures de dimensão da fase 1 passam a carregar também o legado Linx Microvix, lido de dw_landing e dw_history. Como
--elas rodam a cada hora, os membros Linx são refeitos junto com os da Totvs, e o de/para de produto (matched_product_sk)
--aponta sempre para a versão vigente do produto Totvs. Enquanto a carga do legado não rodar, as tabelas Linx estão vazias e
--nada muda. Os códigos Linx entram com source_erp = linxmicrovix na chave, porque colidem com os da Totvs (7.1).

/****** DimStore: origem da loja ******/
--a loja Linx com CNPJ na Totvs é a mesma linha da Totvs (7.5); só a loja que nunca teve CNPJ na Totvs vira membro próprio.
alter table [datawarehouse].DimStore add
	source_erp varchar(20) not null constraint df_DimStore_source_erp default ('totvs');
GO

/****** DimOperation: código como texto ******/
--a natureza de operação do Linx é texto (cod_natureza_operacao). A sk é o MD5 do código como texto, então a da Totvs não muda.
alter table [datawarehouse].DimOperation alter column operation_code nvarchar(20) not null;
GO

/****** datawarehouse.sp_DimStore_Load (SCD2: Totvs e lojas só do Linx) ******/
create or alter procedure [datawarehouse].sp_DimStore_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @linx int;

	begin transaction;

	delete from [datawarehouse].DimStore;

	insert into [datawarehouse].DimStore (sk, doc_company, branch_code, cnpj, name, fantasy_name, group_name, brand, order_series, valid_from, valid_to, is_current, source_erp)
	values (replicate('0', 32), N'0', 0, null, N'NOT INFORMED', N'NOT INFORMED', null, null, null, '19000101', null, 1, '0');

	insert into [datawarehouse].DimStore (sk, doc_company, branch_code, cnpj, name, fantasy_name, group_name, brand, order_series, valid_from, valid_to, is_current, source_erp)
	select convert(char(32), hashbytes('MD5', concat_ws('|', h.doc_company, h.[code], convert(varchar(23), h.valid_from, 121))), 2),
	       h.doc_company, h.[code], h.cnpj, h.[description], h.fantasyName, h.branchGroupName, b.brand, b.order_series,
	       iif(row_number() over (partition by h.doc_company, h.[code] order by h.valid_from) = 1, '19000101', h.valid_from),
	       h.valid_to, h.is_current, 'totvs'
	  from [dw_history].TotvsBranches h
	  left join [dw_reference].BrandSeries b on b.cnpj_root = left(h.cnpj, 8);

	set @rows = @@rowcount;

	--lojas só do Linx: branch_code é o código da empresa no Microvix
	insert into [datawarehouse].DimStore (sk, doc_company, branch_code, cnpj, name, fantasy_name, group_name, brand, order_series, valid_from, valid_to, is_current, source_erp)
	select convert(char(32), hashbytes('MD5', concat_ws('|', h.doc_company, 'linxmicrovix', h.cnpj_emp, convert(varchar(23), h.valid_from, 121))), 2),
	       h.doc_company, isnull(h.empresa, 0), h.cnpj_emp, h.nome_emp, h.nome_emp, h.subrede, b.brand, b.order_series,
	       iif(row_number() over (partition by h.doc_company, h.cnpj_emp order by h.valid_from) = 1, '19000101', h.valid_from),
	       h.valid_to, h.is_current, 'linxmicrovix'
	  from [dw_history].LinxLojas h
	  left join [dw_reference].BrandSeries b on b.cnpj_root = left(h.cnpj_emp, 8)
	 where not exists (select 1 from [dw_history].TotvsBranches t where t.cnpj = h.cnpj_emp);

	set @linx = @@rowcount;
	set @rows += @linx;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimProduct_Load (SCD2: Totvs e Linx, com o de/para) ******/
--Totvs como na fase 1. Linx: de dw_history.LinxProdutos, com matched_product_sk = a versão vigente do produto Totvs que
--dw_intermediate.vw_ProductMatch casou. Sem casamento, nulo; a comparação entre os anos usa COALESCE(matched_product_sk, sk).
create or alter procedure [datawarehouse].sp_DimProduct_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	declare @linx int;

	begin transaction;

	delete from [datawarehouse].DimProduct where source_erp in ('totvs', 'linxmicrovix', '0');

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

	with linx as (
		select h.*,
		       dense_rank() over (partition by h.doc_company, h.cod_produto order by h.portal) as portal_rank
		  from [dw_history].LinxProdutos h
	)
	insert into [datawarehouse].DimProduct (sk, doc_company, source_erp, product_code, sku, product_name, reference_code, reference_name,
	                                        color_code, color_name, size, category_name, category_type, brand, ncm, is_active, matched_product_sk,
	                                        valid_from, valid_to, is_current)
	select convert(char(32), hashbytes('MD5', concat_ws('|', l.doc_company, 'linxmicrovix', l.cod_produto, convert(varchar(23), l.valid_from, 121))), 2),
	       l.doc_company, 'linxmicrovix', cast(l.cod_produto as nvarchar(50)), l.cod_barra, l.nome, l.referencia, l.descricao_basica,
	       cast(l.id_cor as nvarchar(30)), l.desc_cor, l.desc_tamanho, l.desc_linha, 'Line', upper(trim(l.desc_marca)), l.ncm,
	       cast(iif(l.desativado = 'S', 0, 1) as bit), t.sk,
	       iif(row_number() over (partition by l.doc_company, l.cod_produto order by l.valid_from) = 1, '19000101', l.valid_from),
	       l.valid_to, l.is_current
	  from linx l
	  left join [dw_intermediate].vw_ProductMatch m on m.doc_company = l.doc_company and m.linx_product_code = cast(l.cod_produto as nvarchar(50))
	  left join [datawarehouse].DimProduct t on t.source_erp = 'totvs' and t.is_current = 1
	                                         and t.doc_company = m.totvs_doc_company and t.product_code = cast(m.totvs_product_code as nvarchar(50))
	 where l.portal_rank = 1;

	set @linx = @@rowcount;
	set @rows += @linx;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimCustomer_Load (SCD1, de dw_personal_data, com o pepper) ******/
--como na fase 1, mais sexo e situação do cliente Linx, lidos da landing de LinxClientesFornec.
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
			       d.person_type,
			       coalesce(p.gender, case lx.sexo when 'M' then N'Male' when 'F' then N'Female' end) as gender,
			       d.[state],
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
			       coalesce(p.customerStatus, l.customerStatus, case lx.ativo when 'S' then N'Active' when 'N' then N'Inactive' end) as customer_status,
			       d.updated_at
			  from [dw_personal_data].DimCustomerIdentified d
			  left join [dw_landing].TotvsPeople p on d.source_erp = 'totvs' and d.person_type = 'F' and p.doc_company = d.doc_company and cast(p.[code] as nvarchar(50)) = d.customer_code
			  left join [dw_landing].TotvsLegalEntities l on d.source_erp = 'totvs' and d.person_type = 'J' and l.doc_company = d.doc_company and cast(l.[code] as nvarchar(50)) = d.customer_code
			 outer apply (select top 1 x.sexo, x.ativo from [dw_landing].LinxClientesFornec x
			               where d.source_erp = 'linxmicrovix' and x.doc_company = d.doc_company and cast(x.cod_cliente as nvarchar(50)) = d.customer_code
			               order by x.portal) lx
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

/****** datawarehouse.sp_DimSeller_Load (Totvs e Linx) ******/
--o vendedor Linx é da loja: seller_code = cnpj_emp-cod_vendedor.
create or alter procedure [datawarehouse].sp_DimSeller_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimSeller where source_erp in ('totvs', 'linxmicrovix', '0');

	insert into [datawarehouse].DimSeller (sk, doc_company, source_erp, seller_code, name)
	values (replicate('0', 32), N'0', '0', N'0', N'NOT INFORMED');

	insert into [datawarehouse].DimSeller (sk, doc_company, source_erp, seller_code, name)
	select convert(char(32), hashbytes('MD5', concat_ws('|', s.doc_company, 'totvs', s.sellerCode)), 2),
	       s.doc_company, 'totvs', cast(s.sellerCode as nvarchar(50)), s.sellerName
	  from [dw_landing].TotvsSellers s
	union all
	select convert(char(32), hashbytes('MD5', concat_ws('|', v.doc_company, 'linxmicrovix', concat(v.cnpj_emp, '-', v.cod_vendedor))), 2),
	       v.doc_company, 'linxmicrovix', concat(v.cnpj_emp, '-', v.cod_vendedor), v.nome_vendedor
	  from [dw_landing].LinxVendedores v;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimOperation_Load (Totvs e Linx) ******/
--Linx: natureza de operação de LinxMovimento (cod_natureza_operacao, natureza_operacao) e o tipo (operacao: S, E, DS, DE).
create or alter procedure [datawarehouse].sp_DimOperation_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimOperation where source_erp in ('totvs', 'linxmicrovix', '0');

	insert into [datawarehouse].DimOperation (sk, doc_company, source_erp, operation_code, name, operation_type)
	values (replicate('0', 32), N'0', '0', N'0', N'NOT INFORMED', null);

	insert into [datawarehouse].DimOperation (sk, doc_company, source_erp, operation_code, name, operation_type)
	select convert(char(32), hashbytes('MD5', concat_ws('|', o.doc_company, o.source_erp, o.operation_code)), 2), o.doc_company, o.source_erp, o.operation_code,
	       max(o.name), max(o.operation_type)
	  from (
		select doc_company, 'totvs' as source_erp, cast(operationCode as nvarchar(20)) as operation_code, operatioName as name, operationType as operation_type
		  from [dw_landing].TotvsInvoices where operationCode is not null
		union all
		select doc_company, 'totvs', cast(operationCode as nvarchar(20)), operationName, null from [dw_landing].TotvsSaleOrders where operationCode is not null
		union all
		select doc_company, 'linxmicrovix', trim(cod_natureza_operacao), natureza_operacao, operacao
		  from [dw_landing].LinxMovimento where nullif(trim(cod_natureza_operacao), '') is not null
	  ) o
	 group by o.doc_company, o.source_erp, o.operation_code;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimBrand_Load (séries e marcas do Linx) ******/
--as marcas de dw_reference.BrandSeries, mais as marcas dos produtos Linx (desc_marca) que não estão lá.
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
	  from (
		select brand, order_series from [dw_reference].BrandSeries
		union all
		select distinct upper(trim(p.desc_marca)), null
		  from [dw_landing].LinxProdutos p
		 where nullif(trim(p.desc_marca), '') is not null
		   and not exists (select 1 from [dw_reference].BrandSeries s where s.brand = upper(trim(p.desc_marca)))
	  ) b
	 group by b.brand;

	set @rows = @@rowcount;

	commit transaction;
end;
GO
