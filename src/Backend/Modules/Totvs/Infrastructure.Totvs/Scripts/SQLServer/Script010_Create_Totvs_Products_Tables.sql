/****** Produtos: tabelas untreated e totvs (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.4) ******/
--branchInfoCode (empresa base da chamada: uma por marca) entra em todas as chaves; as filhas do produto usam o código
--do produto, único no cadastro. Tamanhos do swagger da API Product onde ele informa; senão, os do código antigo,
--conferidos numa amostra real das duas marcas (23/10/2026).

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

create table [untreated].TotvsSkusGrids (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[sequence]                int,
	[grid]                    nvarchar(60)
);

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

create table [untreated].TotvsSkusObservations (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[sequence]                int,
	observation               nvarchar(max)
);

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

create table [untreated].TotvsSkusCompositions (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[sequence]                int,
	material                  nvarchar(60),
	percentage                decimal(18,4)
);

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

create table [untreated].TotvsSkusReferenceCategories (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[code]                    int,
	[name]                    nvarchar(250),
	[type]                    nvarchar(20)
);

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

create table [untreated].TotvsSkusIntegrationGroupCodes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	[code]                    int
);

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

create table [untreated].TotvsSkusColorsMeasurementsTableOmni (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	referenceId               int,
	colorCode                 nvarchar(60),
	sku                       nvarchar(60),
	tamanho                   nvarchar(60)
);

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

create table [untreated].TotvsProductsBarCodes (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	[code]                    nvarchar(60),
	quantity                  decimal(18,4),
	isMainCode                bit
);

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

create table [untreated].TotvsProductsManufacturers (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	[code]                    int,
	cpfCnpj                   nvarchar(14),
	[name]                    nvarchar(250)
);

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

create table [untreated].TotvsProductsBranchesBlocked (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	branchCode                int,
	isBlocked                 bit
);

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

create table [untreated].TotvsProductsConservationInstructionsItems (
	integrated_or_updated_at  datetime,
	doc_company               nvarchar(14),
	branchInfoCode            int,
	productCode               int,
	instructionCode           int,
	[order]                   int,
	[item]                    int
);

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

alter table [totvs].TotvsSkusGrids add constraint [FK_TotvsSkusGrids] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
alter table [totvs].TotvsSkusObservations add constraint [FK_TotvsSkusObservations] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
alter table [totvs].TotvsSkusDetails add constraint [FK_TotvsSkusDetails] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
alter table [totvs].TotvsSkusCompositions add constraint [FK_TotvsSkusCompositions] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
alter table [totvs].TotvsSkusReferenceCategories add constraint [FK_TotvsSkusReferenceCategories] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
alter table [totvs].TotvsSkusIntegrationGroupCodes add constraint [FK_TotvsSkusIntegrationGroupCodes] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
alter table [totvs].TotvsSkusColors add constraint [FK_TotvsSkusColors] foreign key (doc_company, branchInfoCode, referenceId) references [totvs].TotvsSkus (doc_company, branchInfoCode, referenceId);
alter table [totvs].TotvsSkusColorsMeasurementsTableOmni add constraint [FK_TotvsSkusColorsMeasurementsTableOmni] foreign key (doc_company, branchInfoCode, referenceId, colorCode) references [totvs].TotvsSkusColors (doc_company, branchInfoCode, referenceId, colorCode) on delete cascade;
alter table [totvs].TotvsSkusColorsMeasurementsTableOmniMedidas add constraint [FK_TotvsSkusColorsMeasurementsTableOmniMedidas] foreign key (doc_company, branchInfoCode, referenceId, colorCode, sku) references [totvs].TotvsSkusColorsMeasurementsTableOmni (doc_company, branchInfoCode, referenceId, colorCode, sku) on delete cascade;
alter table [totvs].TotvsSkusColorsProducts add constraint [FK_TotvsSkusColorsProducts] foreign key (doc_company, branchInfoCode, referenceId, colorCode) references [totvs].TotvsSkusColors (doc_company, branchInfoCode, referenceId, colorCode) on delete cascade;
alter table [totvs].TotvsProductsBarCodes add constraint [FK_TotvsProductsBarCodes] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsClassifications add constraint [FK_TotvsProductsClassifications] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsAdditionalFields add constraint [FK_TotvsProductsAdditionalFields] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsSuppliers add constraint [FK_TotvsProductsSuppliers] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsManufacturers add constraint [FK_TotvsProductsManufacturers] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsReferenceCodeSequences add constraint [FK_TotvsProductsReferenceCodeSequences] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsWebData add constraint [FK_TotvsProductsWebData] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsWebDetails add constraint [FK_TotvsProductsWebDetails] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsBranchesBlocked add constraint [FK_TotvsProductsBranchesBlocked] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsConservationInstructions add constraint [FK_TotvsProductsConservationInstructions] foreign key (doc_company, branchInfoCode, productCode) references [totvs].TotvsSkusColorsProducts (doc_company, branchInfoCode, [code]) on delete cascade;
alter table [totvs].TotvsProductsConservationInstructionsItems add constraint [FK_TotvsProductsConservationInstructionsItems] foreign key (doc_company, branchInfoCode, productCode, instructionCode) references [totvs].TotvsProductsConservationInstructions (doc_company, branchInfoCode, productCode, [code]) on delete cascade;
