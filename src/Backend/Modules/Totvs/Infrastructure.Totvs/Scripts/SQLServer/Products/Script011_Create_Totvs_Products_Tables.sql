-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; docs/MIGRATOR-CSX.md)
/****** Produtos: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.4) ******/
--branchInfoCode (empresa base da chamada: uma por marca) entra em todas as chaves; as filhas do produto usam o código
--do produto, único no cadastro. Tamanhos do swagger da API Product onde ele informa; senão, os do código antigo,
--conferidos numa amostra real das duas marcas (23/10/2026).

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkus') = 1
create table [untreated].TotvsSkus (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	referenceCode             nvarchar(60),
	lastReferenceCode         nvarchar(60),
	insertDate                datetime,
	maxChangeFilterDate       datetime,
	[name]                    nvarchar(250),
	[description]             nvarchar(250),
	descriptive               nvarchar(max),
	gridCode                  int,
	weight                    decimal(18,4),
	height                    decimal(18,4),
	width                     decimal(18,4),
	[length]                  decimal(18,4),
	packWeight                decimal(18,4),
	packHeight                decimal(18,4),
	packWidth                 decimal(18,4),
	packLength                decimal(18,4)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkus') = 1
create table [totvs].TotvsSkus (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	referenceCode             nvarchar(60), --ReferenceCode na API
	lastReferenceCode         nvarchar(60),
	insertDate                datetime,
	maxChangeFilterDate       datetime,
	[name]                    nvarchar(250),
	[description]             nvarchar(250),
	descriptive               nvarchar(max),
	gridCode                  int,
	weight                    decimal(18,4),
	height                    decimal(18,4),
	width                     decimal(18,4),
	[length]                  decimal(18,4),
	packWeight                decimal(18,4),
	packHeight                decimal(18,4),
	packWidth                 decimal(18,4),
	packLength                decimal(18,4),
constraint [PK_TotvsSkus] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusGrids') = 1
create table [untreated].TotvsSkusGrids (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[sequence]                int,
	[grid]                    nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusGrids') = 1
create table [totvs].TotvsSkusGrids (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	[sequence]                int not null, --posição na lista (a API manda só os tamanhos)
	[grid]                    nvarchar(60),
constraint [PK_TotvsSkusGrids] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusObservations') = 1
create table [untreated].TotvsSkusObservations (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[sequence]                int,
	observation               nvarchar(max)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusObservations') = 1
create table [totvs].TotvsSkusObservations (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	[sequence]                int not null, --posição na lista (sem chave na API)
	observation               nvarchar(max),
constraint [PK_TotvsSkusObservations] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusDetails') = 1
create table [untreated].TotvsSkusDetails (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[sequence]                int,
	typeCode                  int,
	[type]                    nvarchar(60),
	auxiliaryType             nvarchar(60),
	title                     nvarchar(250),
	[description]             nvarchar(max)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusDetails') = 1
create table [totvs].TotvsSkusDetails (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	[sequence]                int not null, --posição na lista (typeCode e auxiliaryType se repetem)
	typeCode                  int,
	[type]                    nvarchar(60),
	auxiliaryType             nvarchar(60),
	title                     nvarchar(250),
	[description]             nvarchar(max),
constraint [PK_TotvsSkusDetails] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusCompositions') = 1
create table [untreated].TotvsSkusCompositions (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[sequence]                int,
	material                  nvarchar(60),
	percentage                decimal(18,4)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusCompositions') = 1
create table [totvs].TotvsSkusCompositions (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	[sequence]                int not null, --posição na lista (sem chave na API)
	material                  nvarchar(60),
	percentage                decimal(18,4),
constraint [PK_TotvsSkusCompositions] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusReferenceCategories') = 1
create table [untreated].TotvsSkusReferenceCategories (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[code]                    int,
	[name]                    nvarchar(250),
	[type]                    nvarchar(20)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusReferenceCategories') = 1
create table [totvs].TotvsSkusReferenceCategories (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	[code]                    int not null,
	[name]                    nvarchar(250),
	[type]                    nvarchar(20), --Category, SubCategory, Manufacturer, Filter
constraint [PK_TotvsSkusReferenceCategories] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusIntegrationGroupCodes') = 1
create table [untreated].TotvsSkusIntegrationGroupCodes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[code]                    int
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusIntegrationGroupCodes') = 1
create table [totvs].TotvsSkusIntegrationGroupCodes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	[code]                    int not null,
constraint [PK_TotvsSkusIntegrationGroupCodes] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusColors') = 1
create table [untreated].TotvsSkusColors (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	colorCode                 nvarchar(60),
	[name]                    nvarchar(60),
	groupName                 nvarchar(60),
	auxiliaryCode             nvarchar(60),
	auxiliaryName             nvarchar(60),
	pantoneCode               nvarchar(60),
	configurableField         nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusColors') = 1
create table [totvs].TotvsSkusColors (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	colorCode                 nvarchar(60) not null, --code da cor; nulo vira vazio
	[name]                    nvarchar(60),
	groupName                 nvarchar(60),
	auxiliaryCode             nvarchar(60),
	auxiliaryName             nvarchar(60),
	pantoneCode               nvarchar(60),
	configurableField         nvarchar(60),
constraint [PK_TotvsSkusColors] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	colorCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusColorsMeasurementsTableOmni') = 1
create table [untreated].TotvsSkusColorsMeasurementsTableOmni (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	colorCode                 nvarchar(60),
	sku                       nvarchar(60),
	tamanho                   nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusColorsMeasurementsTableOmni') = 1
create table [totvs].TotvsSkusColorsMeasurementsTableOmni (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	colorCode                 nvarchar(60) not null,
	sku                       nvarchar(60) not null, --nulo vira vazio
	tamanho                   nvarchar(60),
constraint [PK_TotvsSkusColorsMeasurementsTableOmni] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	colorCode asc,
	sku asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusColorsMeasurementsTableOmniMedidas') = 1
create table [untreated].TotvsSkusColorsMeasurementsTableOmniMedidas (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	colorCode                 nvarchar(60),
	sku                       nvarchar(60),
	[chave]                   nvarchar(60),
	[valor]                   nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusColorsMeasurementsTableOmniMedidas') = 1
create table [totvs].TotvsSkusColorsMeasurementsTableOmniMedidas (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	referenceId               int not null,
	colorCode                 nvarchar(60) not null,
	sku                       nvarchar(60) not null,
	[chave]                   nvarchar(60) not null, --nulo vira vazio
	[valor]                   nvarchar(60),
constraint [PK_TotvsSkusColorsMeasurementsTableOmniMedidas] primary key (
	doc_company asc,
	branchInfoCode asc,
	referenceId asc,
	colorCode asc,
	sku asc,
	[chave] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsSkusColorsProducts') = 1
create table [untreated].TotvsSkusColorsProducts (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	[code]                    int,
	referenceId               int,
	colorCode                 nvarchar(60),
	sku                       nvarchar(60),
	[name]                    nvarchar(250),
	ncm                       nvarchar(60),
	ipi                       decimal(18,4),
	cst                       nvarchar(60),
	cest                      nvarchar(60),
	prefixEanGtin             nvarchar(60),
	measuredUnit              nvarchar(60),
	[size]                    nvarchar(60),
	weight                    decimal(18,4),
	isActive                  bit,
	insertDate                datetime,
	isFinishedProduct         bit,
	isRawMaterial             bit,
	isBulkMaterial            bit,
	isOwnProduction           bit,
	isKit                     bit,
	integrationProductCode    int,
	minimumStockAmount        decimal(18,4),
	maximumStockAmount        decimal(18,4),
	idealStockAmount          decimal(18,4),
	salesStartDate            datetime,
	salesEndDate              datetime,
	billingForecastDate       datetime,
	isBlocked                 bit,
	balanceValidationType     nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsSkusColorsProducts') = 1
create table [totvs].TotvsSkusColorsProducts (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	[code]                    int not null, --código do produto, único no cadastro
	referenceId               int,
	colorCode                 nvarchar(60),
	sku                       nvarchar(60),
	[name]                    nvarchar(250),
	ncm                       nvarchar(60),
	ipi                       decimal(18,4),
	cst                       nvarchar(60),
	cest                      nvarchar(60),
	prefixEanGtin             nvarchar(60),
	measuredUnit              nvarchar(60),
	[size]                    nvarchar(60),
	weight                    decimal(18,4),
	isActive                  bit, --da empresa base
	insertDate                datetime,
	isFinishedProduct         bit,
	isRawMaterial             bit,
	isBulkMaterial            bit,
	isOwnProduction           bit,
	isKit                     bit,
	integrationProductCode    int,
	minimumStockAmount        decimal(18,4),
	maximumStockAmount        decimal(18,4),
	idealStockAmount          decimal(18,4),
	salesStartDate            datetime,
	salesEndDate              datetime,
	billingForecastDate       datetime,
	isBlocked                 bit,
	balanceValidationType     nvarchar(60),
constraint [PK_TotvsSkusColorsProducts] primary key (
	doc_company asc,
	branchInfoCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsBarCodes') = 1
create table [untreated].TotvsProductsBarCodes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	[code]                    nvarchar(60),
	quantity                  decimal(18,4),
	isMainCode                bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsBarCodes') = 1
create table [totvs].TotvsProductsBarCodes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	[code]                    nvarchar(60) not null, --nulo vira vazio
	quantity                  decimal(18,4),
	isMainCode                bit,
constraint [PK_TotvsProductsBarCodes] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsClassifications') = 1
create table [untreated].TotvsProductsClassifications (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	typeCode                  int,
	typeName                  nvarchar(60),
	typeNameAux               nvarchar(60),
	[code]                    nvarchar(60),
	[name]                    nvarchar(60),
	nameAux                   nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsClassifications') = 1
create table [totvs].TotvsProductsClassifications (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	typeCode                  int not null,
	typeName                  nvarchar(60),
	typeNameAux               nvarchar(60),
	[code]                    nvarchar(60),
	[name]                    nvarchar(60),
	nameAux                   nvarchar(60),
constraint [PK_TotvsProductsClassifications] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	typeCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsAdditionalFields') = 1
create table [untreated].TotvsProductsAdditionalFields (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	[code]                    int,
	[name]                    nvarchar(60),
	[type]                    nvarchar(20),
	[value]                   nvarchar(max)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsAdditionalFields') = 1
create table [totvs].TotvsProductsAdditionalFields (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	[code]                    int not null,
	[name]                    nvarchar(60),
	[type]                    nvarchar(20), --String, Number, Date
	[value]                   nvarchar(max),
constraint [PK_TotvsProductsAdditionalFields] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsSuppliers') = 1
create table [untreated].TotvsProductsSuppliers (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	[code]                    int,
	cpfCnpj                   nvarchar(14),
	[name]                    nvarchar(250),
	originalCode              nvarchar(60),
	isDefault                 bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsSuppliers') = 1
create table [totvs].TotvsProductsSuppliers (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	[code]                    int not null,
	cpfCnpj                   nvarchar(14),
	[name]                    nvarchar(250),
	originalCode              nvarchar(60),
	isDefault                 bit,
constraint [PK_TotvsProductsSuppliers] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsManufacturers') = 1
create table [untreated].TotvsProductsManufacturers (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	[code]                    int,
	cpfCnpj                   nvarchar(14),
	[name]                    nvarchar(250)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsManufacturers') = 1
create table [totvs].TotvsProductsManufacturers (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	[code]                    int not null,
	cpfCnpj                   nvarchar(14),
	[name]                    nvarchar(250),
constraint [PK_TotvsProductsManufacturers] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsReferenceCodeSequences') = 1
create table [untreated].TotvsProductsReferenceCodeSequences (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	referenceId               int,
	parentCode                int,
	[code]                    nvarchar(60),
	[name]                    nvarchar(250)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsReferenceCodeSequences') = 1
create table [totvs].TotvsProductsReferenceCodeSequences (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	referenceId               int not null, --da sequência, não da referência do produto
	parentCode                int not null,
	[code]                    nvarchar(60), --se repete no mesmo produto
	[name]                    nvarchar(250),
constraint [PK_TotvsProductsReferenceCodeSequences] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	referenceId asc,
	parentCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsWebData') = 1
create table [untreated].TotvsProductsWebData (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	[index]                   int,
	[description]             nvarchar(max),
	descriptive               nvarchar(max),
	weight                    decimal(18,4),
	height                    decimal(18,4),
	width                     decimal(18,4),
	[length]                  decimal(18,4),
	packWeight                decimal(18,4),
	packHeight                decimal(18,4),
	packWidth                 decimal(18,4),
	packLength                decimal(18,4),
	productPackWeight         decimal(18,4),
	packQuantity              decimal(18,4),
	[link]                    nvarchar(max),
	[url]                     nvarchar(max),
	[status]                  nvarchar(20),
	isUpdate                  bit,
	definition1               nvarchar(250),
	definition2               nvarchar(250),
	bonusQuantity             int
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsWebData') = 1
create table [totvs].TotvsProductsWebData (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	[index]                   int not null,
	[description]             nvarchar(max),
	descriptive               nvarchar(max),
	weight                    decimal(18,4),
	height                    decimal(18,4),
	width                     decimal(18,4),
	[length]                  decimal(18,4),
	packWeight                decimal(18,4),
	packHeight                decimal(18,4),
	packWidth                 decimal(18,4),
	packLength                decimal(18,4),
	productPackWeight         decimal(18,4),
	packQuantity              decimal(18,4),
	[link]                    nvarchar(max),
	[url]                     nvarchar(max),
	[status]                  nvarchar(20), --Active, Blocked
	isUpdate                  bit,
	definition1               nvarchar(250),
	definition2               nvarchar(250),
	bonusQuantity             int,
constraint [PK_TotvsProductsWebData] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	[index] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsWebDetails') = 1
create table [untreated].TotvsProductsWebDetails (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	descriptionType           int,
	[type]                    nvarchar(60),
	title                     nvarchar(250),
	[description]             nvarchar(max)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsWebDetails') = 1
create table [totvs].TotvsProductsWebDetails (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	descriptionType           int not null,
	[type]                    nvarchar(60),
	title                     nvarchar(250),
	[description]             nvarchar(max),
constraint [PK_TotvsProductsWebDetails] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	descriptionType asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsBranchesBlocked') = 1
create table [untreated].TotvsProductsBranchesBlocked (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	branchCode                int,
	isBlocked                 bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsBranchesBlocked') = 1
create table [totvs].TotvsProductsBranchesBlocked (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	branchCode                int not null,
	isBlocked                 bit,
constraint [PK_TotvsProductsBranchesBlocked] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	branchCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsConservationInstructions') = 1
create table [untreated].TotvsProductsConservationInstructions (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	[code]                    int,
	[description]             nvarchar(80),
	isDefault                 bit,
	grouperCode               int
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsConservationInstructions') = 1
create table [totvs].TotvsProductsConservationInstructions (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	[code]                    int not null,
	[description]             nvarchar(80),
	isDefault                 bit, --default na API
	grouperCode               int,
constraint [PK_TotvsProductsConservationInstructions] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsConservationInstructionsItems') = 1
create table [untreated].TotvsProductsConservationInstructionsItems (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	instructionCode           int,
	[order]                   int,
	[item]                    int
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsConservationInstructionsItems') = 1
create table [totvs].TotvsProductsConservationInstructionsItems (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14) not null,
	branchInfoCode            int not null, --empresa base da chamada (matriz da marca)
	productCode               int not null,
	instructionCode           int not null,
	[order]                   int not null,
	[item]                    int,
constraint [PK_TotvsProductsConservationInstructionsItems] primary key (
	doc_company asc,
	branchInfoCode asc,
	productCode asc,
	instructionCode asc,
	[order] asc
));

/****** Chaves estrangeiras ******/
--cascade a partir das cores: uma cor ou um produto que sai da referência leva junto as netas, e o merge das cores roda
--antes do merge dos produtos (sem cascade a FK impediria apagar a cor). As filhas diretas da referência não precisam:
--a referência nunca é apagada por este fluxo.

if object_id(N'[totvs].[FK_TotvsSkusGrids]') is null and object_id(N'[totvs].TotvsSkusGrids') is not null and object_id(N'[totvs].TotvsSkus') is not null
alter table [totvs].TotvsSkusGrids add constraint [FK_TotvsSkusGrids] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
if object_id(N'[totvs].[FK_TotvsSkusObservations]') is null and object_id(N'[totvs].TotvsSkusObservations') is not null and object_id(N'[totvs].TotvsSkus') is not null
alter table [totvs].TotvsSkusObservations add constraint [FK_TotvsSkusObservations] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
if object_id(N'[totvs].[FK_TotvsSkusDetails]') is null and object_id(N'[totvs].TotvsSkusDetails') is not null and object_id(N'[totvs].TotvsSkus') is not null
alter table [totvs].TotvsSkusDetails add constraint [FK_TotvsSkusDetails] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
if object_id(N'[totvs].[FK_TotvsSkusCompositions]') is null and object_id(N'[totvs].TotvsSkusCompositions') is not null and object_id(N'[totvs].TotvsSkus') is not null
alter table [totvs].TotvsSkusCompositions add constraint [FK_TotvsSkusCompositions] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
if object_id(N'[totvs].[FK_TotvsSkusReferenceCategories]') is null and object_id(N'[totvs].TotvsSkusReferenceCategories') is not null and object_id(N'[totvs].TotvsSkus') is not null
alter table [totvs].TotvsSkusReferenceCategories add constraint [FK_TotvsSkusReferenceCategories] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
if object_id(N'[totvs].[FK_TotvsSkusIntegrationGroupCodes]') is null and object_id(N'[totvs].TotvsSkusIntegrationGroupCodes') is not null and object_id(N'[totvs].TotvsSkus') is not null
alter table [totvs].TotvsSkusIntegrationGroupCodes add constraint [FK_TotvsSkusIntegrationGroupCodes] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
if object_id(N'[totvs].[FK_TotvsSkusColors]') is null and object_id(N'[totvs].TotvsSkusColors') is not null and object_id(N'[totvs].TotvsSkus') is not null
alter table [totvs].TotvsSkusColors add constraint [FK_TotvsSkusColors] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
if object_id(N'[totvs].[FK_TotvsSkusColorsMeasurementsTableOmni]') is null and object_id(N'[totvs].TotvsSkusColorsMeasurementsTableOmni') is not null and object_id(N'[totvs].TotvsSkusColors') is not null
alter table [totvs].TotvsSkusColorsMeasurementsTableOmni add constraint [FK_TotvsSkusColorsMeasurementsTableOmni] foreign key (doc_company, branchInfoCode, referenceId, colorCode) references [totvs].TotvsSkusColors (doc_company, branchInfoCode, referenceId, colorCode) on delete cascade;
if object_id(N'[totvs].[FK_TotvsSkusColorsMeasurementsTableOmniMedidas]') is null and object_id(N'[totvs].TotvsSkusColorsMeasurementsTableOmniMedidas') is not null and object_id(N'[totvs].TotvsSkusColorsMeasurementsTableOmni') is not null
alter table [totvs].TotvsSkusColorsMeasurementsTableOmniMedidas add constraint [FK_TotvsSkusColorsMeasurementsTableOmniMedidas] foreign key (doc_company, branchInfoCode, referenceId, colorCode, sku) references [totvs].TotvsSkusColorsMeasurementsTableOmni (doc_company, branchInfoCode, referenceId, colorCode, sku) on delete cascade;
if object_id(N'[totvs].[FK_TotvsSkusColorsProducts]') is null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null and object_id(N'[totvs].TotvsSkusColors') is not null
alter table [totvs].TotvsSkusColorsProducts add constraint [FK_TotvsSkusColorsProducts] foreign key (doc_company, branchInfoCode, referenceId, colorCode) references [totvs].TotvsSkusColors (doc_company, branchInfoCode, referenceId, colorCode) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsBarCodes]') is null and object_id(N'[totvs].TotvsProductsBarCodes') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsBarCodes add constraint [FK_TotvsProductsBarCodes] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsClassifications]') is null and object_id(N'[totvs].TotvsProductsClassifications') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsClassifications add constraint [FK_TotvsProductsClassifications] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsAdditionalFields]') is null and object_id(N'[totvs].TotvsProductsAdditionalFields') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsAdditionalFields add constraint [FK_TotvsProductsAdditionalFields] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsSuppliers]') is null and object_id(N'[totvs].TotvsProductsSuppliers') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsSuppliers add constraint [FK_TotvsProductsSuppliers] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsManufacturers]') is null and object_id(N'[totvs].TotvsProductsManufacturers') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsManufacturers add constraint [FK_TotvsProductsManufacturers] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsReferenceCodeSequences]') is null and object_id(N'[totvs].TotvsProductsReferenceCodeSequences') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsReferenceCodeSequences add constraint [FK_TotvsProductsReferenceCodeSequences] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsWebData]') is null and object_id(N'[totvs].TotvsProductsWebData') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsWebData add constraint [FK_TotvsProductsWebData] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsWebDetails]') is null and object_id(N'[totvs].TotvsProductsWebDetails') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsWebDetails add constraint [FK_TotvsProductsWebDetails] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsBranchesBlocked]') is null and object_id(N'[totvs].TotvsProductsBranchesBlocked') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsBranchesBlocked add constraint [FK_TotvsProductsBranchesBlocked] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsConservationInstructions]') is null and object_id(N'[totvs].TotvsProductsConservationInstructions') is not null and object_id(N'[totvs].TotvsSkusColorsProducts') is not null
alter table [totvs].TotvsProductsConservationInstructions add constraint [FK_TotvsProductsConservationInstructions] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
if object_id(N'[totvs].[FK_TotvsProductsConservationInstructionsItems]') is null and object_id(N'[totvs].TotvsProductsConservationInstructionsItems') is not null and object_id(N'[totvs].TotvsProductsConservationInstructions') is not null
alter table [totvs].TotvsProductsConservationInstructionsItems add constraint [FK_TotvsProductsConservationInstructionsItems] foreign key (doc_company, branchInfoCode, productCode, instructionCode) references [totvs].TotvsProductsConservationInstructions (doc_company, branchInfoCode, productCode, [code]) on delete cascade;

go

/****** Saldos: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.9, fase PR-2) ******/
--uma linha por loja + produto + código (todos os códigos de preço, custo e saldo do cadastro). branchCode vem antes do produto
--na chave: a janela, a carga inicial e o cache de estado consultam por loja. Os dados do produto já estão em TotvsSkus.

if dbo.fn_Migrator_Creates(N'[untreated].TotvsBalances') = 1
create table [untreated].TotvsBalances (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	productCode                         int,
	stockCode                           int,
	stockDescription                    nvarchar(60),
	[stock]                             decimal(18,6),
	salesOrder                          decimal(18,6),
	inputTransaction                    decimal(18,6),
	outputTransaction                   decimal(18,6),
	productionPlanning                  decimal(18,6),
	purchaseOrder                       decimal(18,6),
	productionOrderProgress             decimal(18,6),
	productionOrderWaitLib              decimal(18,6),
	stockTemp                           decimal(18,6),
	maxChangeFilterDate                 datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsBalances') = 1
create table [totvs].TotvsBalances (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	productCode                         int not null,
	stockCode                           int not null,
	stockDescription                    nvarchar(60),
	[stock]                             decimal(18,6),
	salesOrder                          decimal(18,6),
	inputTransaction                    decimal(18,6),
	outputTransaction                   decimal(18,6),
	productionPlanning                  decimal(18,6),
	purchaseOrder                       decimal(18,6),
	productionOrderProgress             decimal(18,6),
	productionOrderWaitLib              decimal(18,6),
	stockTemp                           decimal(18,6),
	maxChangeFilterDate                 datetime,
constraint [PK_TotvsBalances] primary key (
	doc_company asc,
	branchCode asc,
	productCode asc,
	stockCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsBalancesLocations') = 1
create table [untreated].TotvsBalancesLocations (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	productCode                         int,
	locationCode                        nvarchar(20),
	[description]                       nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsBalancesLocations') = 1
create table [totvs].TotvsBalancesLocations (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	productCode                         int not null,
	locationCode                        nvarchar(20) not null,
	[description]                       nvarchar(60),
constraint [PK_TotvsBalancesLocations] primary key (
	doc_company asc,
	branchCode asc,
	productCode asc,
	locationCode asc
));

--TotvsBalancesLocations é do produto na loja, não de um tipo de saldo: sem FK (a chave de TotvsBalances tem o stockCode)

go

/****** Similares e lotes: tabelas untreated e totvs (src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.2, fase PR-2) ******/
--sem data de alteração útil nas respostas: comparados pelo json_hash (T9). Os similares guardam em changeWindowEnd o fim da
--janela que os trouxe, de onde a próxima leitura começa; os lotes são lidos inteiros

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsSimilar') = 1
create table [untreated].TotvsProductsSimilar (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	productCode                         int,
	groupSequenceCode                   int,
	changeWindowEnd                     datetime,
	json_hash                           nvarchar(64)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsSimilar') = 1
create table [totvs].TotvsProductsSimilar (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	productCode                         int not null,
	groupSequenceCode                   int,
	changeWindowEnd                     datetime,
	json_hash                           nvarchar(64),
constraint [PK_TotvsProductsSimilar] primary key (
	doc_company asc,
	productCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductsSimilarItems') = 1
create table [untreated].TotvsProductsSimilarItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	productCode                         int,
	listType                            nvarchar(20),
	similarProductCode                  int
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductsSimilarItems') = 1
create table [totvs].TotvsProductsSimilarItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	productCode                         int not null,
	listType                            nvarchar(20) not null,
	similarProductCode                  int not null,
constraint [PK_TotvsProductsSimilarItems] primary key (
	doc_company asc,
	productCode asc,
	listType asc,
	similarProductCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsBatches') = 1
create table [untreated].TotvsBatches (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	batchCode                           int,
	productCode                         int,
	batchProvider                       nvarchar(40),
	batchClient                         nvarchar(40),
	batchManufacturingDate              datetime,
	batchExpirationDate                 datetime,
	batchType                           nvarchar(30),
	json_hash                           nvarchar(64)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsBatches') = 1
create table [totvs].TotvsBatches (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	batchCode                           int not null,
	productCode                         int not null,
	batchProvider                       nvarchar(40),
	batchClient                         nvarchar(40),
	batchManufacturingDate              datetime,
	batchExpirationDate                 datetime,
	batchType                           nvarchar(30),
	json_hash                           nvarchar(64),
constraint [PK_TotvsBatches] primary key (
	doc_company asc,
	branchCode asc,
	batchCode asc,
	productCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsBatchesItems') = 1
create table [untreated].TotvsBatchesItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	batchCode                           int,
	productCode                         int,
	batchItemSequential                 int,
	[balance]                           decimal(18,6),
	quantityItem                        decimal(18,6),
	quantityIssued                      decimal(18,6),
	quantityTare                        decimal(18,6),
	quantityWeigth                      decimal(18,6),
	quantityMeters                      decimal(18,6),
	quantityCanceled                    decimal(18,6),
	quantityYield                       decimal(18,6),
	itemProvider                        nvarchar(40),
	itemClient                          nvarchar(40),
	locationBatch                       nvarchar(5),
	[status]                            nvarchar(30),
	conditioningType                    nvarchar(10),
	nuance                              nvarchar(20)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsBatchesItems') = 1
create table [totvs].TotvsBatchesItems (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	batchCode                           int not null,
	productCode                         int not null,
	batchItemSequential                 int not null,
	[balance]                           decimal(18,6),
	quantityItem                        decimal(18,6),
	quantityIssued                      decimal(18,6),
	quantityTare                        decimal(18,6),
	quantityWeigth                      decimal(18,6),
	quantityMeters                      decimal(18,6),
	quantityCanceled                    decimal(18,6),
	quantityYield                       decimal(18,6),
	itemProvider                        nvarchar(40),
	itemClient                          nvarchar(40),
	locationBatch                       nvarchar(5),
	[status]                            nvarchar(30),
	conditioningType                    nvarchar(10),
	nuance                              nvarchar(20),
constraint [PK_TotvsBatchesItems] primary key (
	doc_company asc,
	branchCode asc,
	batchCode asc,
	productCode asc,
	batchItemSequential asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsBatchesItemsAdditionalFields') = 1
create table [untreated].TotvsBatchesItemsAdditionalFields (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14),
	branchCode                          int,
	batchCode                           int,
	productCode                         int,
	batchItemSequential                 int,
	[code]                              int,
	[description]                       nvarchar(60),
	[value]                             nvarchar(250)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsBatchesItemsAdditionalFields') = 1
create table [totvs].TotvsBatchesItemsAdditionalFields (
	integrated_or_updated_at            datetime,
	doc_company                         nvarchar(14) not null,
	branchCode                          int not null,
	batchCode                           int not null,
	productCode                         int not null,
	batchItemSequential                 int not null,
	[code]                              int not null,
	[description]                       nvarchar(60),
	[value]                             nvarchar(250),
constraint [PK_TotvsBatchesItemsAdditionalFields] primary key (
	doc_company asc,
	branchCode asc,
	batchCode asc,
	productCode asc,
	batchItemSequential asc,
	[code] asc
));

/****** Chaves estrangeiras ******/
if object_id(N'[totvs].[FK_TotvsProductsSimilarItems]') is null and object_id(N'[totvs].TotvsProductsSimilarItems') is not null and object_id(N'[totvs].TotvsProductsSimilar') is not null
alter table [totvs].TotvsProductsSimilarItems add constraint [FK_TotvsProductsSimilarItems] foreign key (doc_company, productCode) references [totvs].TotvsProductsSimilar (doc_company, productCode);
if object_id(N'[totvs].[FK_TotvsBatchesItems]') is null and object_id(N'[totvs].TotvsBatchesItems') is not null and object_id(N'[totvs].TotvsBatches') is not null
alter table [totvs].TotvsBatchesItems add constraint [FK_TotvsBatchesItems] foreign key (doc_company, branchCode, batchCode, productCode) references [totvs].TotvsBatches (doc_company, branchCode, batchCode, productCode);
if object_id(N'[totvs].[FK_TotvsBatchesItemsAdditionalFields]') is null and object_id(N'[totvs].TotvsBatchesItemsAdditionalFields') is not null and object_id(N'[totvs].TotvsBatchesItems') is not null
alter table [totvs].TotvsBatchesItemsAdditionalFields add constraint [FK_TotvsBatchesItemsAdditionalFields] foreign key (doc_company, branchCode, batchCode, productCode, batchItemSequential) references [totvs].TotvsBatchesItems (doc_company, branchCode, batchCode, productCode, batchItemSequential);
