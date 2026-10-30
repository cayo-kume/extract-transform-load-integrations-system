/****** Dimensões da fase 4: representante, área operacional e promoção (docs/datawarehouse, 7.6) ******/
--SCD1, refeitas da landing a cada execução, com membro desconhecido (sk de 32 zeros). DimSupplier passa a ter também os
--fornecedores dos pedidos de compra e das duplicatas, que nem sempre estão em TotvsProductsSuppliers.

create table [datawarehouse].DimSalesRepresentative (
	sk                    char(32)       not null,
	doc_company           nvarchar(14)   not null,
	representative_code   int            not null,
	name                  nvarchar(100)  null,
	supervisor_code       int            null,
	supervisor_name       nvarchar(100)  null,
	branch_insert_code    int            null,
	is_inactive           bit            null,
constraint [pk_DimSalesRepresentative] primary key (sk asc)
);

create table [datawarehouse].DimRegion (
	sk                       char(32)       not null,
	doc_company              nvarchar(14)   not null,
	operational_area_code    int            not null,
	name                     nvarchar(100)  null,
	area_type                nvarchar(30)   null,
	group_code               int            null,
	group_description        nvarchar(100)  null,
	number_of_days           int            null,
constraint [pk_DimRegion] primary key (sk asc)
);

--uma linha por promoção, loja e produto, como na origem: a promoção da Totvs vale por loja e produto
create table [datawarehouse].DimPromotion (
	sk                  char(32)        not null,
	doc_company         nvarchar(14)    not null,
	branch_code         int             not null,
	promotion_code      int             not null,
	product_code        int             not null,
	description         nvarchar(40)    null,
	start_date          datetime        null,
	end_date            datetime        null,
	previous_value      decimal(18,6)   null,
	promotion_value     decimal(18,6)   null,
	[status]            nvarchar(10)    null,
constraint [pk_DimPromotion] primary key (sk asc)
);
GO

/****** datawarehouse.sp_DimSalesRepresentative_Load ******/
create or alter procedure [datawarehouse].sp_DimSalesRepresentative_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimSalesRepresentative;

	insert into [datawarehouse].DimSalesRepresentative (sk, doc_company, representative_code, name)
	values (replicate('0', 32), N'0', 0, N'NOT INFORMED');

	insert into [datawarehouse].DimSalesRepresentative (sk, doc_company, representative_code, name, supervisor_code, supervisor_name, branch_insert_code, is_inactive)
	select convert(char(32), hashbytes('MD5', concat_ws('|', r.doc_company, r.[code])), 2), r.doc_company, r.[code], r.[name],
	       r.supervisorCode, r.supervisorName, r.branchInsertCode, r.isInactive
	  from [dw_landing].TotvsRepresentatives r;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimRegion_Load ******/
create or alter procedure [datawarehouse].sp_DimRegion_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimRegion;

	insert into [datawarehouse].DimRegion (sk, doc_company, operational_area_code, name)
	values (replicate('0', 32), N'0', 0, N'NOT INFORMED');

	insert into [datawarehouse].DimRegion (sk, doc_company, operational_area_code, name, area_type, group_code, group_description, number_of_days)
	select convert(char(32), hashbytes('MD5', concat_ws('|', a.doc_company, a.operationalAreaCode)), 2), a.doc_company, a.operationalAreaCode,
	       a.operationalAreaName, a.operationalAreaType, a.operationalAreaGroupCode, a.operationalAreaGroupDescription, a.numberOfDays
	  from [dw_landing].TotvsOperationalAreas a;

	set @rows = @@rowcount;

	commit transaction;
end;
GO

/****** datawarehouse.sp_DimPromotion_Load ******/
create or alter procedure [datawarehouse].sp_DimPromotion_Load
	@rows int = null output
as
begin
	set nocount on;
	set xact_abort on;

	begin transaction;

	delete from [datawarehouse].DimPromotion;

	insert into [datawarehouse].DimPromotion (sk, doc_company, branch_code, promotion_code, product_code, description)
	values (replicate('0', 32), N'0', 0, 0, 0, N'NOT INFORMED');

	insert into [datawarehouse].DimPromotion (sk, doc_company, branch_code, promotion_code, product_code, description, start_date, end_date,
	                                          previous_value, promotion_value, [status])
	select convert(char(32), hashbytes('MD5', concat_ws('|', p.doc_company, p.branchCode, p.promotionCode, p.productCode)), 2),
	       p.doc_company, p.branchCode, p.promotionCode, p.productCode, p.promotionDescription, p.startDate, p.endDate,
	       p.previousValue, p.promotionValue, p.[status]
	  from [dw_landing].TotvsPromotions p;

	set @rows = @@rowcount;

	commit transaction;
end;
GO
