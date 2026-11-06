-- roda em toda migração: cada tabela só é criada se ainda não existe e está entre as dos jobs escolhidos (dbo.fn_Migrator_Creates, Shared Script010; .docs/MIGRATOR-CSX.md)
/****** Produtos e SKUs: tabelas untreated e linxcommerce (src/Backend/Modules/LinxCommerce/.docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.2) ******/
--colunas e tamanhos tirados do código antigo, sem resposta real da API: a conferir antes de ligar o job (§9).
--datas da API em datetimeoffset (o Linx Commerce manda o fuso); integrated_or_updated_at é o horário do lote, em Brasília

--produto (POST Catalog/API.svc/web/GetProduct); as listas de ids ficam como texto
if dbo.fn_Migrator_Creates(N'[untreated].LinxCommerceProducts') = 1
create table [untreated].LinxCommerceProducts (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	[ProductID]              int,
	[Name]                   nvarchar(400),
	[SKU]                    nvarchar(100),
	[IntegrationID]          nvarchar(100),
	[BrandID]                int,
	[RatingSetID]            int,
	[CatalogItemType]        int,
	[PurchasingPolicyID]     int,
	[DefinitionID]           int,
	[AcceptanceTermID]       int,
	[IsFreeShipping]         bit,
	[IsVisible]              bit,
	[VisibleFrom]            datetimeoffset,
	[VisibleTo]              datetimeoffset,
	[IsSearchable]           bit,
	[IsUponRequest]          bit,
	[DisplayAvailability]    nvarchar(50),
	[DisplayStockQty]        bit,
	[DisplayPrice]           nvarchar(50),
	[IsNew]                  bit,
	[NewFrom]                datetimeoffset,
	[NewTo]                  datetimeoffset,
	[IsGift]                 bit,
	[UseAcceptanceTerm]      bit,
	[IsDeleted]              bit,
	[SendToMarketplace]      nvarchar(50),
	[PageTitle]              nvarchar(400),
	[UrlFriendly]            nvarchar(400),
	[SearchKeywords]         nvarchar(max),
	[MetaDescription]        nvarchar(max),
	[MetaKeywords]           nvarchar(max),
	[ShortDescription]       nvarchar(max),
	[LongDescription]        nvarchar(max),
	[WarrantyDescription]    nvarchar(max),
	[SkusIDs]                nvarchar(max),
	[FlagsIDs]               nvarchar(max),
	[MediasIDs]              nvarchar(max),
	[CategoriesIDs]          nvarchar(max),
	[ShippingRegionsIDs]     nvarchar(max),
	[TagsIDs]                nvarchar(max),
	[CreatedDate]            datetimeoffset,
	[CreatedBy]              nvarchar(100),
	[ModifiedDate]           datetimeoffset,
	[ModifiedBy]             nvarchar(100)
);

if dbo.fn_Migrator_Creates(N'[linxcommerce].LinxCommerceProducts') = 1
create table [linxcommerce].LinxCommerceProducts (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	[ProductID]              int not null,
	[Name]                   nvarchar(400),
	[SKU]                    nvarchar(100),
	[IntegrationID]          nvarchar(100),
	[BrandID]                int,
	[RatingSetID]            int,
	[CatalogItemType]        int,
	[PurchasingPolicyID]     int,
	[DefinitionID]           int,
	[AcceptanceTermID]       int,
	[IsFreeShipping]         bit,
	[IsVisible]              bit,
	[VisibleFrom]            datetimeoffset,
	[VisibleTo]              datetimeoffset,
	[IsSearchable]           bit,
	[IsUponRequest]          bit,
	[DisplayAvailability]    nvarchar(50),
	[DisplayStockQty]        bit,
	[DisplayPrice]           nvarchar(50),
	[IsNew]                  bit,
	[NewFrom]                datetimeoffset,
	[NewTo]                  datetimeoffset,
	[IsGift]                 bit,
	[UseAcceptanceTerm]      bit,
	[IsDeleted]              bit,
	[SendToMarketplace]      nvarchar(50),
	[PageTitle]              nvarchar(400),
	[UrlFriendly]            nvarchar(400),
	[SearchKeywords]         nvarchar(max),
	[MetaDescription]        nvarchar(max),
	[MetaKeywords]           nvarchar(max),
	[ShortDescription]       nvarchar(max),
	[LongDescription]        nvarchar(max),
	[WarrantyDescription]    nvarchar(max),
	[SkusIDs]                nvarchar(max),
	[FlagsIDs]               nvarchar(max),
	[MediasIDs]              nvarchar(max),
	[CategoriesIDs]          nvarchar(max),
	[ShippingRegionsIDs]     nvarchar(max),
	[TagsIDs]                nvarchar(max),
	[CreatedDate]            datetimeoffset,
	[CreatedBy]              nvarchar(100),
	[ModifiedDate]           datetimeoffset,
	[ModifiedBy]             nvarchar(100),
constraint [PK_LinxCommerceProducts] primary key (
	doc_company asc,
	ProductID asc
));

--atributo (metadado) do produto
if dbo.fn_Migrator_Creates(N'[untreated].LinxCommerceProductsMetadataValues') = 1
create table [untreated].LinxCommerceProductsMetadataValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ProductID                int,
	[PropertyMetadataID]     int,
	[PropertyName]           nvarchar(250),
	[DisplayName]            nvarchar(250),
	[PropertyGroup]          nvarchar(250),
	[FormattedValue]         nvarchar(max)
);

if dbo.fn_Migrator_Creates(N'[linxcommerce].LinxCommerceProductsMetadataValues') = 1
create table [linxcommerce].LinxCommerceProductsMetadataValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ProductID                int not null,
	[PropertyMetadataID]     int not null,
	[PropertyName]           nvarchar(250),
	[DisplayName]            nvarchar(250),
	[PropertyGroup]          nvarchar(250),
	[FormattedValue]         nvarchar(max),
constraint [PK_LinxCommerceProductsMetadataValues] primary key (
	doc_company asc,
	ProductID asc,
	PropertyMetadataID asc
));

--mídia do produto, com Image e Video (1:1) em colunas, nulas quando a mídia não é daquele tipo
if dbo.fn_Migrator_Creates(N'[untreated].LinxCommerceProductsMedias') = 1
create table [untreated].LinxCommerceProductsMedias (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ProductID                int,
	[MediaID]                int,
	[Type]                   nvarchar(50),
	[Index]                  nvarchar(50),
	[IsDeleted]              bit,
	[CreatedDate]            datetimeoffset,
	[ParentMediaID]          int,
	[OriginalFileName]       nvarchar(400),
	[ImageRelativePath]      nvarchar(1000),
	[ImageAbsolutePath]      nvarchar(1000),
	[ImageExtension]         nvarchar(20),
	[ImageMaxWidth]          decimal(19,6),
	[ImageMaxHeight]         decimal(19,6),
	[ImageWidth]             decimal(19,6),
	[ImageHeight]            decimal(19,6),
	[ImageMediaSizeType]     int,
	[VideoTitle]             nvarchar(400),
	[VideoUrl]               nvarchar(1000)
);

if dbo.fn_Migrator_Creates(N'[linxcommerce].LinxCommerceProductsMedias') = 1
create table [linxcommerce].LinxCommerceProductsMedias (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ProductID                int not null,
	[MediaID]                int not null,
	[Type]                   nvarchar(50),
	[Index]                  nvarchar(50),
	[IsDeleted]              bit,
	[CreatedDate]            datetimeoffset,
	[ParentMediaID]          int,
	[OriginalFileName]       nvarchar(400),
	[ImageRelativePath]      nvarchar(1000),
	[ImageAbsolutePath]      nvarchar(1000),
	[ImageExtension]         nvarchar(20),
	[ImageMaxWidth]          decimal(19,6),
	[ImageMaxHeight]         decimal(19,6),
	[ImageWidth]             decimal(19,6),
	[ImageHeight]            decimal(19,6),
	[ImageMediaSizeType]     int,
	[VideoTitle]             nvarchar(400),
	[VideoUrl]               nvarchar(1000),
constraint [PK_LinxCommerceProductsMedias] primary key (
	doc_company asc,
	ProductID asc,
	MediaID asc
));

--associação de mídia do produto
if dbo.fn_Migrator_Creates(N'[untreated].LinxCommerceProductsMediaAssociations') = 1
create table [untreated].LinxCommerceProductsMediaAssociations (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ProductID                int,
	[ID]                     int,
	[MediaID]                int,
	[Path]                   nvarchar(1000),
	[Order]                  int
);

if dbo.fn_Migrator_Creates(N'[linxcommerce].LinxCommerceProductsMediaAssociations') = 1
create table [linxcommerce].LinxCommerceProductsMediaAssociations (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ProductID                int not null,
	[ID]                     int not null,
	[MediaID]                int,
	[Path]                   nvarchar(1000),
	[Order]                  int,
constraint [PK_LinxCommerceProductsMediaAssociations] primary key (
	doc_company asc,
	ProductID asc,
	ID asc
));

--SKU (POST Catalog/API.svc/web/GetSKU); o id do SKU é o ProductID
if dbo.fn_Migrator_Creates(N'[untreated].LinxCommerceSkus') = 1
create table [untreated].LinxCommerceSkus (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	[ProductID]              int,
	[Name]                   nvarchar(400),
	[SKU]                    nvarchar(100),
	[IntegrationID]          nvarchar(100),
	[UPC]                    nvarchar(50),
	[DisplayCondition]       bit,
	[DefinitionID]           int,
	[ConditionID]            int,
	[UnitOfMeasureID]        int,
	[PurchasingPolicyID]     int,
	[ManageStock]            bit,
	[MinimumQtyAllowed]      decimal(19,6),
	[MaximumQtyAllowed]      decimal(19,6),
	[Preorderable]           bit,
	[PreorderDate]           datetimeoffset,
	[PreorderLimit]          decimal(19,6),
	[Backorderable]          bit,
	[BackorderLimit]         decimal(19,6),
	[WrappingQty]            nvarchar(50),
	[Weight]                 decimal(19,6),
	[Height]                 decimal(19,6),
	[Width]                  decimal(19,6),
	[Depth]                  decimal(19,6),
	[Cost]                   decimal(19,6),
	[Tax]                    decimal(19,6),
	[IsVisible]              bit,
	[VisibleFrom]            datetimeoffset,
	[VisibleTo]              datetimeoffset,
	[IsDeleted]              bit,
	[SuppliersIDs]           nvarchar(max),
	[ParentsIDs]             nvarchar(max),
	[CreatedDate]            datetimeoffset,
	[CreatedBy]              nvarchar(100),
	[ModifiedDate]           datetimeoffset,
	[ModifiedBy]             nvarchar(100)
);

if dbo.fn_Migrator_Creates(N'[linxcommerce].LinxCommerceSkus') = 1
create table [linxcommerce].LinxCommerceSkus (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	[ProductID]              int not null,
	[Name]                   nvarchar(400),
	[SKU]                    nvarchar(100),
	[IntegrationID]          nvarchar(100),
	[UPC]                    nvarchar(50),
	[DisplayCondition]       bit,
	[DefinitionID]           int,
	[ConditionID]            int,
	[UnitOfMeasureID]        int,
	[PurchasingPolicyID]     int,
	[ManageStock]            bit,
	[MinimumQtyAllowed]      decimal(19,6),
	[MaximumQtyAllowed]      decimal(19,6),
	[Preorderable]           bit,
	[PreorderDate]           datetimeoffset,
	[PreorderLimit]          decimal(19,6),
	[Backorderable]          bit,
	[BackorderLimit]         decimal(19,6),
	[WrappingQty]            nvarchar(50),
	[Weight]                 decimal(19,6),
	[Height]                 decimal(19,6),
	[Width]                  decimal(19,6),
	[Depth]                  decimal(19,6),
	[Cost]                   decimal(19,6),
	[Tax]                    decimal(19,6),
	[IsVisible]              bit,
	[VisibleFrom]            datetimeoffset,
	[VisibleTo]              datetimeoffset,
	[IsDeleted]              bit,
	[SuppliersIDs]           nvarchar(max),
	[ParentsIDs]             nvarchar(max),
	[CreatedDate]            datetimeoffset,
	[CreatedBy]              nvarchar(100),
	[ModifiedDate]           datetimeoffset,
	[ModifiedBy]             nvarchar(100),
constraint [PK_LinxCommerceSkus] primary key (
	doc_company asc,
	ProductID asc
));

--atributo (metadado) do SKU
if dbo.fn_Migrator_Creates(N'[untreated].LinxCommerceSkusMetadataValues') = 1
create table [untreated].LinxCommerceSkusMetadataValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ProductID                int,
	[PropertyMetadataID]     int,
	[PropertyName]           nvarchar(250),
	[DisplayName]            nvarchar(250),
	[PropertyGroup]          nvarchar(250),
	[InputType]              int,
	[IntegrationID]          nvarchar(100),
	[Value]                  nvarchar(max),
	[FormattedValue]         nvarchar(max),
	[SerializedValue]        nvarchar(max),
	[SerializedBlobValue]    nvarchar(max)
);

if dbo.fn_Migrator_Creates(N'[linxcommerce].LinxCommerceSkusMetadataValues') = 1
create table [linxcommerce].LinxCommerceSkusMetadataValues (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ProductID                int not null,
	[PropertyMetadataID]     int not null,
	[PropertyName]           nvarchar(250),
	[DisplayName]            nvarchar(250),
	[PropertyGroup]          nvarchar(250),
	[InputType]              int,
	[IntegrationID]          nvarchar(100),
	[Value]                  nvarchar(max),
	[FormattedValue]         nvarchar(max),
	[SerializedValue]        nvarchar(max),
	[SerializedBlobValue]    nvarchar(max),
constraint [PK_LinxCommerceSkusMetadataValues] primary key (
	doc_company asc,
	ProductID asc,
	PropertyMetadataID asc
));

--produto pai do SKU
if dbo.fn_Migrator_Creates(N'[untreated].LinxCommerceSkusParentRelations') = 1
create table [untreated].LinxCommerceSkusParentRelations (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	ProductID                int,
	[ParentID]               int,
	[ParentSKU]              nvarchar(100),
	[ParentIntegrationID]    nvarchar(100)
);

if dbo.fn_Migrator_Creates(N'[linxcommerce].LinxCommerceSkusParentRelations') = 1
create table [linxcommerce].LinxCommerceSkusParentRelations (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	ProductID                int not null,
	[ParentID]               int not null,
	[ParentSKU]              nvarchar(100),
	[ParentIntegrationID]    nvarchar(100),
constraint [PK_LinxCommerceSkusParentRelations] primary key (
	doc_company asc,
	ProductID asc,
	ParentID asc
));

/****** Chaves estrangeiras: filhas -> pai ******/

if object_id(N'[linxcommerce].[FK_LinxCommerceProductsMetadataValues]') is null and object_id(N'[linxcommerce].LinxCommerceProductsMetadataValues') is not null and object_id(N'[linxcommerce].LinxCommerceProducts') is not null
alter table [linxcommerce].LinxCommerceProductsMetadataValues add constraint [FK_LinxCommerceProductsMetadataValues] foreign key (doc_company, ProductID) references [linxcommerce].LinxCommerceProducts (doc_company, ProductID);
if object_id(N'[linxcommerce].[FK_LinxCommerceProductsMedias]') is null and object_id(N'[linxcommerce].LinxCommerceProductsMedias') is not null and object_id(N'[linxcommerce].LinxCommerceProducts') is not null
alter table [linxcommerce].LinxCommerceProductsMedias add constraint [FK_LinxCommerceProductsMedias] foreign key (doc_company, ProductID) references [linxcommerce].LinxCommerceProducts (doc_company, ProductID);
if object_id(N'[linxcommerce].[FK_LinxCommerceProductsMediaAssociations]') is null and object_id(N'[linxcommerce].LinxCommerceProductsMediaAssociations') is not null and object_id(N'[linxcommerce].LinxCommerceProducts') is not null
alter table [linxcommerce].LinxCommerceProductsMediaAssociations add constraint [FK_LinxCommerceProductsMediaAssociations] foreign key (doc_company, ProductID) references [linxcommerce].LinxCommerceProducts (doc_company, ProductID);
if object_id(N'[linxcommerce].[FK_LinxCommerceSkusMetadataValues]') is null and object_id(N'[linxcommerce].LinxCommerceSkusMetadataValues') is not null and object_id(N'[linxcommerce].LinxCommerceSkus') is not null
alter table [linxcommerce].LinxCommerceSkusMetadataValues add constraint [FK_LinxCommerceSkusMetadataValues] foreign key (doc_company, ProductID) references [linxcommerce].LinxCommerceSkus (doc_company, ProductID);
if object_id(N'[linxcommerce].[FK_LinxCommerceSkusParentRelations]') is null and object_id(N'[linxcommerce].LinxCommerceSkusParentRelations') is not null and object_id(N'[linxcommerce].LinxCommerceSkus') is not null
alter table [linxcommerce].LinxCommerceSkusParentRelations add constraint [FK_LinxCommerceSkusParentRelations] foreign key (doc_company, ProductID) references [linxcommerce].LinxCommerceSkus (doc_company, ProductID);
