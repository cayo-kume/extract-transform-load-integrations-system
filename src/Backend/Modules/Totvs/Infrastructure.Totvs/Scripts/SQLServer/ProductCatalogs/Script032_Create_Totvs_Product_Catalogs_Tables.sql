-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; .docs/MIGRATOR-CSX.md)
/****** Catálogos da API Product: tabelas untreated e totvs (src/Backend/Modules/Totvs/.docs/PLANO-TOTVS-METODOS-FALTANTES.md §5.1, fase PR-1) ******/
--tamanhos do swagger da API Product (ou, sem tamanho no swagger, os das outras tabelas). Catálogos pequenos, lidos pela janela de
--alteração (T9), como as filiais; as cores, sem data na resposta, são lidas inteiras e comparadas pelo json_hash

if dbo.fn_Migrator_Creates(N'[untreated].TotvsClassificationTypes') = 1
create table [untreated].TotvsClassificationTypes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	[description]                     nvarchar(40),
	auxiliaryDescription              nvarchar(100),
	isGroup                           bit,
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsClassificationTypes') = 1
create table [totvs].TotvsClassificationTypes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	[description]                     nvarchar(40),
	auxiliaryDescription              nvarchar(100),
	isGroup                           bit,
	maxChangeFilterDate               datetime,
constraint [PK_TotvsClassificationTypes] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductClassifications') = 1
create table [untreated].TotvsProductClassifications (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	typeCode                          int,
	typeName                          nvarchar(40),
	typeNameAux                       nvarchar(100),
	[code]                            nvarchar(10),
	[name]                            nvarchar(60),
	nameAux                           nvarchar(100),
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductClassifications') = 1
create table [totvs].TotvsProductClassifications (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	typeCode                          int not null,
	typeName                          nvarchar(40),
	typeNameAux                       nvarchar(100),
	[code]                            nvarchar(10) not null,
	[name]                            nvarchar(60),
	nameAux                           nvarchar(100),
	maxChangeFilterDate               datetime,
constraint [PK_TotvsProductClassifications] primary key (
	doc_company asc,
	typeCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsCategories') = 1
create table [untreated].TotvsCategories (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	[name]                            nvarchar(100),
	parentCategoryCode                int,
	categoryType                      nvarchar(30),
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsCategories') = 1
create table [totvs].TotvsCategories (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	[name]                            nvarchar(100),
	parentCategoryCode                int,
	categoryType                      nvarchar(30),
	maxChangeFilterDate               datetime,
constraint [PK_TotvsCategories] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsGrids') = 1
create table [untreated].TotvsGrids (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	[name]                            nvarchar(60),
	[type]                            nvarchar(30),
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsGrids') = 1
create table [totvs].TotvsGrids (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	[name]                            nvarchar(60),
	[type]                            nvarchar(30),
	maxChangeFilterDate               datetime,
constraint [PK_TotvsGrids] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsGridsSizes') = 1
create table [untreated].TotvsGridsSizes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	gridCode                          int,
	[sequence]                        int,
	sizeName                          nvarchar(20)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsGridsSizes') = 1
create table [totvs].TotvsGridsSizes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	gridCode                          int not null,
	[sequence]                        int not null,
	sizeName                          nvarchar(20),
constraint [PK_TotvsGridsSizes] primary key (
	doc_company asc,
	gridCode asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsMeasurementUnits') = 1
create table [untreated].TotvsMeasurementUnits (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            nvarchar(10),
	[description]                     nvarchar(60),
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsMeasurementUnits') = 1
create table [totvs].TotvsMeasurementUnits (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            nvarchar(10) not null,
	[description]                     nvarchar(60),
	maxChangeFilterDate               datetime,
constraint [PK_TotvsMeasurementUnits] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsMeasurementUnitsVariations') = 1
create table [untreated].TotvsMeasurementUnitsVariations (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	measurementUnitCode               nvarchar(10),
	[code]                            nvarchar(40),
	[description]                     nvarchar(60),
	isTribXml                         bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsMeasurementUnitsVariations') = 1
create table [totvs].TotvsMeasurementUnitsVariations (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	measurementUnitCode               nvarchar(10) not null,
	[code]                            nvarchar(40) not null,
	[description]                     nvarchar(60),
	isTribXml                         bit,
constraint [PK_TotvsMeasurementUnitsVariations] primary key (
	doc_company asc,
	measurementUnitCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsColors') = 1
create table [untreated].TotvsColors (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	colorCode                         nvarchar(10),
	colorName                         nvarchar(40),
	json_hash                         nvarchar(64)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsColors') = 1
create table [totvs].TotvsColors (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	colorCode                         nvarchar(10) not null,
	colorName                         nvarchar(40),
	json_hash                         nvarchar(64),
constraint [PK_TotvsColors] primary key (
	doc_company asc,
	colorCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsColorsAdditionalInformation') = 1
create table [untreated].TotvsColorsAdditionalInformation (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	colorCode                         nvarchar(10),
	branchCode                        int,
	groupName                         nvarchar(10),
	auxiliaryCode                     nvarchar(10),
	auxiliaryName                     nvarchar(80),
	pantoneCode                       nvarchar(40),
	colorType                         nvarchar(2),
	colorTypeName                     nvarchar(40),
	isActive                          bit,
	isFinishedProduct                 bit,
	isRawMaterial                     bit,
	isBulkMaterial                    bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsColorsAdditionalInformation') = 1
create table [totvs].TotvsColorsAdditionalInformation (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	colorCode                         nvarchar(10) not null,
	branchCode                        int not null,
	groupName                         nvarchar(10),
	auxiliaryCode                     nvarchar(10),
	auxiliaryName                     nvarchar(80),
	pantoneCode                       nvarchar(40),
	colorType                         nvarchar(2),
	colorTypeName                     nvarchar(40),
	isActive                          bit,
	isFinishedProduct                 bit,
	isRawMaterial                     bit,
	isBulkMaterial                    bit,
constraint [PK_TotvsColorsAdditionalInformation] primary key (
	doc_company asc,
	colorCode asc,
	branchCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsAdditionalFieldsTypes') = 1
create table [untreated].TotvsAdditionalFieldsTypes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	[description]                     nvarchar(60),
	[title]                           nvarchar(30),
	[type]                            nvarchar(30),
	[size]                            int,
	[decimal]                         int,
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsAdditionalFieldsTypes') = 1
create table [totvs].TotvsAdditionalFieldsTypes (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	[description]                     nvarchar(60),
	[title]                           nvarchar(30),
	[type]                            nvarchar(30),
	[size]                            int,
	[decimal]                         int,
	maxChangeFilterDate               datetime,
constraint [PK_TotvsAdditionalFieldsTypes] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductGrouperConfigurations') = 1
create table [untreated].TotvsProductGrouperConfigurations (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	grouperCode                       int,
	[description]                     nvarchar(40),
	[type]                            nvarchar(30),
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductGrouperConfigurations') = 1
create table [totvs].TotvsProductGrouperConfigurations (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	grouperCode                       int not null,
	[description]                     nvarchar(40),
	[type]                            nvarchar(30),
	maxChangeFilterDate               datetime,
constraint [PK_TotvsProductGrouperConfigurations] primary key (
	doc_company asc,
	grouperCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsProductGrouperConfigurationsItems') = 1
create table [untreated].TotvsProductGrouperConfigurationsItems (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	grouperCode                       int,
	[code]                            int,
	[order]                           int,
	[description]                     nvarchar(40),
	isMandatory                       bit
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsProductGrouperConfigurationsItems') = 1
create table [totvs].TotvsProductGrouperConfigurationsItems (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	grouperCode                       int not null,
	[code]                            int not null,
	[order]                           int,
	[description]                     nvarchar(40),
	isMandatory                       bit,
constraint [PK_TotvsProductGrouperConfigurationsItems] primary key (
	doc_company asc,
	grouperCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsInstructionItems') = 1
create table [untreated].TotvsInstructionItems (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	[description]                     nvarchar(200),
	grouper                           int,
	grouperDescription                nvarchar(40),
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsInstructionItems') = 1
create table [totvs].TotvsInstructionItems (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	[description]                     nvarchar(200),
	grouper                           int,
	grouperDescription                nvarchar(40),
	maxChangeFilterDate               datetime,
constraint [PK_TotvsInstructionItems] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsCompositions') = 1
create table [untreated].TotvsCompositions (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	[description]                     nvarchar(80),
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsCompositions') = 1
create table [totvs].TotvsCompositions (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	[description]                     nvarchar(80),
	maxChangeFilterDate               datetime,
constraint [PK_TotvsCompositions] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsCompositionsFibers') = 1
create table [untreated].TotvsCompositionsFibers (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	compositionCode                   int,
	[code]                            int,
	[description]                     nvarchar(40),
	percentage                        decimal(18,6)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsCompositionsFibers') = 1
create table [totvs].TotvsCompositionsFibers (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	compositionCode                   int not null,
	[code]                            int not null,
	[description]                     nvarchar(40),
	percentage                        decimal(18,6),
constraint [PK_TotvsCompositionsFibers] primary key (
	doc_company asc,
	compositionCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPriceTables') = 1
create table [untreated].TotvsPriceTables (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	[code]                            int,
	[description]                     nvarchar(60),
	codeName                          nvarchar(20),
	[type]                            nvarchar(40),
	maxChangeFilterDate               datetime,
	startDate                         datetime,
	endDate                           datetime,
	variationValue                    decimal(18,6),
	variationPercentage               decimal(18,6)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPriceTables') = 1
create table [totvs].TotvsPriceTables (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	[code]                            int not null,
	[description]                     nvarchar(60),
	codeName                          nvarchar(20),
	[type]                            nvarchar(40),
	maxChangeFilterDate               datetime,
	startDate                         datetime,
	endDate                           datetime,
	variationValue                    decimal(18,6),
	variationPercentage               decimal(18,6),
constraint [PK_TotvsPriceTables] primary key (
	doc_company asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPriceTablesSalesOrderClassifications') = 1
create table [untreated].TotvsPriceTablesSalesOrderClassifications (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	priceTableCode                    int,
	typeCode                          int,
	typeName                          nvarchar(40),
	[code]                            nvarchar(10),
	[name]                            nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPriceTablesSalesOrderClassifications') = 1
create table [totvs].TotvsPriceTablesSalesOrderClassifications (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	priceTableCode                    int not null,
	typeCode                          int not null,
	typeName                          nvarchar(40),
	[code]                            nvarchar(10) not null,
	[name]                            nvarchar(60),
constraint [PK_TotvsPriceTablesSalesOrderClassifications] primary key (
	doc_company asc,
	priceTableCode asc,
	typeCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPriceTablesPersonClassifications') = 1
create table [untreated].TotvsPriceTablesPersonClassifications (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	priceTableCode                    int,
	typeCode                          int,
	typeName                          nvarchar(40),
	[code]                            nvarchar(10),
	[name]                            nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPriceTablesPersonClassifications') = 1
create table [totvs].TotvsPriceTablesPersonClassifications (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	priceTableCode                    int not null,
	typeCode                          int not null,
	typeName                          nvarchar(40),
	[code]                            nvarchar(10) not null,
	[name]                            nvarchar(60),
constraint [PK_TotvsPriceTablesPersonClassifications] primary key (
	doc_company asc,
	priceTableCode asc,
	typeCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPriceTablesPaymentConditions') = 1
create table [untreated].TotvsPriceTablesPaymentConditions (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	priceTableCode                    int,
	[code]                            int,
	[name]                            nvarchar(60)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPriceTablesPaymentConditions') = 1
create table [totvs].TotvsPriceTablesPaymentConditions (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	priceTableCode                    int not null,
	[code]                            int not null,
	[name]                            nvarchar(60),
constraint [PK_TotvsPriceTablesPaymentConditions] primary key (
	doc_company asc,
	priceTableCode asc,
	[code] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPriceTablesAveragePeriods') = 1
create table [untreated].TotvsPriceTablesAveragePeriods (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	priceTableCode                    int,
	[quantity]                        int,
	variationValue                    decimal(18,6),
	variationPercentage               decimal(18,6)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPriceTablesAveragePeriods') = 1
create table [totvs].TotvsPriceTablesAveragePeriods (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	priceTableCode                    int not null,
	[quantity]                        int not null,
	variationValue                    decimal(18,6),
	variationPercentage               decimal(18,6),
constraint [PK_TotvsPriceTablesAveragePeriods] primary key (
	doc_company asc,
	priceTableCode asc,
	[quantity] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPriceTablesAveragePeriodQuantities') = 1
create table [untreated].TotvsPriceTablesAveragePeriodQuantities (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	priceTableCode                    int,
	[sequence]                        int,
	averagePeriod                     int,
	variationQuantity                 decimal(18,6),
	variationPercentage               decimal(18,6)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPriceTablesAveragePeriodQuantities') = 1
create table [totvs].TotvsPriceTablesAveragePeriodQuantities (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	priceTableCode                    int not null,
	[sequence]                        int not null,
	averagePeriod                     int,
	variationQuantity                 decimal(18,6),
	variationPercentage               decimal(18,6),
constraint [PK_TotvsPriceTablesAveragePeriodQuantities] primary key (
	doc_company asc,
	priceTableCode asc,
	[sequence] asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPriceTableScales') = 1
create table [untreated].TotvsPriceTableScales (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	scaleCode                         int,
	scaleDescription                  nvarchar(60),
	maxChangeFilterDate               datetime
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPriceTableScales') = 1
create table [totvs].TotvsPriceTableScales (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	scaleCode                         int not null,
	scaleDescription                  nvarchar(60),
	maxChangeFilterDate               datetime,
constraint [PK_TotvsPriceTableScales] primary key (
	doc_company asc,
	scaleCode asc
));

if dbo.fn_Migrator_Creates(N'[untreated].TotvsPriceTableScalesItems') = 1
create table [untreated].TotvsPriceTableScalesItems (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14),
	scaleCode                         int,
	[quantity]                        int,
	variationValue                    decimal(18,6),
	variationPercentage               decimal(18,6)
);

if dbo.fn_Migrator_Creates(N'[totvs].TotvsPriceTableScalesItems') = 1
create table [totvs].TotvsPriceTableScalesItems (
	integrated_or_updated_at          datetime,
	doc_company                       nvarchar(14) not null,
	scaleCode                         int not null,
	[quantity]                        int not null,
	variationValue                    decimal(18,6),
	variationPercentage               decimal(18,6),
constraint [PK_TotvsPriceTableScalesItems] primary key (
	doc_company asc,
	scaleCode asc,
	[quantity] asc
));

/****** Chaves estrangeiras ******/
if object_id(N'[totvs].[FK_TotvsGridsSizes]') is null and object_id(N'[totvs].TotvsGridsSizes') is not null and object_id(N'[totvs].TotvsGrids') is not null
alter table [totvs].TotvsGridsSizes add constraint [FK_TotvsGridsSizes] foreign key (doc_company, gridCode) references [totvs].TotvsGrids (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsMeasurementUnitsVariations]') is null and object_id(N'[totvs].TotvsMeasurementUnitsVariations') is not null and object_id(N'[totvs].TotvsMeasurementUnits') is not null
alter table [totvs].TotvsMeasurementUnitsVariations add constraint [FK_TotvsMeasurementUnitsVariations] foreign key (doc_company, measurementUnitCode) references [totvs].TotvsMeasurementUnits (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsColorsAdditionalInformation]') is null and object_id(N'[totvs].TotvsColorsAdditionalInformation') is not null and object_id(N'[totvs].TotvsColors') is not null
alter table [totvs].TotvsColorsAdditionalInformation add constraint [FK_TotvsColorsAdditionalInformation] foreign key (doc_company, colorCode) references [totvs].TotvsColors (doc_company, colorCode);
if object_id(N'[totvs].[FK_TotvsProductGrouperConfigurationsItems]') is null and object_id(N'[totvs].TotvsProductGrouperConfigurationsItems') is not null and object_id(N'[totvs].TotvsProductGrouperConfigurations') is not null
alter table [totvs].TotvsProductGrouperConfigurationsItems add constraint [FK_TotvsProductGrouperConfigurationsItems] foreign key (doc_company, grouperCode) references [totvs].TotvsProductGrouperConfigurations (doc_company, grouperCode);
if object_id(N'[totvs].[FK_TotvsCompositionsFibers]') is null and object_id(N'[totvs].TotvsCompositionsFibers') is not null and object_id(N'[totvs].TotvsCompositions') is not null
alter table [totvs].TotvsCompositionsFibers add constraint [FK_TotvsCompositionsFibers] foreign key (doc_company, compositionCode) references [totvs].TotvsCompositions (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsPriceTablesSalesOrderClassifications]') is null and object_id(N'[totvs].TotvsPriceTablesSalesOrderClassifications') is not null and object_id(N'[totvs].TotvsPriceTables') is not null
alter table [totvs].TotvsPriceTablesSalesOrderClassifications add constraint [FK_TotvsPriceTablesSalesOrderClassifications] foreign key (doc_company, priceTableCode) references [totvs].TotvsPriceTables (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsPriceTablesPersonClassifications]') is null and object_id(N'[totvs].TotvsPriceTablesPersonClassifications') is not null and object_id(N'[totvs].TotvsPriceTables') is not null
alter table [totvs].TotvsPriceTablesPersonClassifications add constraint [FK_TotvsPriceTablesPersonClassifications] foreign key (doc_company, priceTableCode) references [totvs].TotvsPriceTables (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsPriceTablesPaymentConditions]') is null and object_id(N'[totvs].TotvsPriceTablesPaymentConditions') is not null and object_id(N'[totvs].TotvsPriceTables') is not null
alter table [totvs].TotvsPriceTablesPaymentConditions add constraint [FK_TotvsPriceTablesPaymentConditions] foreign key (doc_company, priceTableCode) references [totvs].TotvsPriceTables (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsPriceTablesAveragePeriods]') is null and object_id(N'[totvs].TotvsPriceTablesAveragePeriods') is not null and object_id(N'[totvs].TotvsPriceTables') is not null
alter table [totvs].TotvsPriceTablesAveragePeriods add constraint [FK_TotvsPriceTablesAveragePeriods] foreign key (doc_company, priceTableCode) references [totvs].TotvsPriceTables (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsPriceTablesAveragePeriodQuantities]') is null and object_id(N'[totvs].TotvsPriceTablesAveragePeriodQuantities') is not null and object_id(N'[totvs].TotvsPriceTables') is not null
alter table [totvs].TotvsPriceTablesAveragePeriodQuantities add constraint [FK_TotvsPriceTablesAveragePeriodQuantities] foreign key (doc_company, priceTableCode) references [totvs].TotvsPriceTables (doc_company, [code]);
if object_id(N'[totvs].[FK_TotvsPriceTableScalesItems]') is null and object_id(N'[totvs].TotvsPriceTableScalesItems') is not null and object_id(N'[totvs].TotvsPriceTableScales') is not null
alter table [totvs].TotvsPriceTableScalesItems add constraint [FK_TotvsPriceTableScalesItems] foreign key (doc_company, scaleCode) references [totvs].TotvsPriceTableScales (doc_company, scaleCode);
