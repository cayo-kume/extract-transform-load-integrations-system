/****** Vendedores: tabelas untreated e linxcommerce (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.2) ******/
--colunas e tamanhos tirados do código antigo, sem resposta real da API: a conferir antes de ligar o job (§9).
--datas da API em datetimeoffset (o Linx Commerce manda o fuso); integrated_or_updated_at é o horário do lote, em Brasília

--vendedor (POST Sales/API.svc/web/GetSalesRepresentative), com os objetos 1:1 em colunas e as listas de ids como texto
create table [untreated].LinxCommerceSalesRepresentatives (
	integrated_or_updated_at            datetime,
	[doc_company]                       nvarchar(14),
	[SalesRepresentativeID]             int,
	[Name]                              nvarchar(250),
	[FriendlyCode]                      nvarchar(100),
	[ImageUrl]                          nvarchar(1000),
	[IntegrationID]                     nvarchar(100),
	[AllowQuoteDeletion]                bit,
	[BusinessContractID]                int,
	[SalesRepresentativeType]           nvarchar(50),
	[Status]                            nvarchar(50),
	[OrderTypeItems]                    nvarchar(max),
	[IdentificationType]                nvarchar(50),
	[IdentificationDocumentNumber]      nvarchar(20),
	[CommissionTotalCommission]         decimal(19,6),
	[CommissionDeliveryCommission]      decimal(19,6),
	[ContactEmail]                      nvarchar(250),
	[ContactPhone]                      nvarchar(50),
	[ContactCellPhone]                  nvarchar(50),
	[ShippingRegionSelectedMode]        nvarchar(50),
	[ShippingRegionShippingRegionID]    int,
	[ShippingRegionPointOfSalesList]    nvarchar(max),
	[MaxDiscountType]                   nvarchar(50),
	[MaxDiscountAmount]                 decimal(19,6),
	[WebSiteSettingsWebSiteFilter]      nvarchar(50),
	[WebSiteSettingsWebSiteGroups]      nvarchar(max),
	[WebSiteSettingsWebSites]           nvarchar(max),
	[PortfolioHasPortfolio]             bit,
	[PortfolioPortfolioAssociationType] nvarchar(50)
);

create table [linxcommerce].LinxCommerceSalesRepresentatives (
	integrated_or_updated_at            datetime,
	[doc_company]                       nvarchar(14) not null,
	[SalesRepresentativeID]             int not null,
	[Name]                              nvarchar(250),
	[FriendlyCode]                      nvarchar(100),
	[ImageUrl]                          nvarchar(1000),
	[IntegrationID]                     nvarchar(100),
	[AllowQuoteDeletion]                bit,
	[BusinessContractID]                int,
	[SalesRepresentativeType]           nvarchar(50),
	[Status]                            nvarchar(50),
	[OrderTypeItems]                    nvarchar(max),
	[IdentificationType]                nvarchar(50),
	[IdentificationDocumentNumber]      nvarchar(20),
	[CommissionTotalCommission]         decimal(19,6),
	[CommissionDeliveryCommission]      decimal(19,6),
	[ContactEmail]                      nvarchar(250),
	[ContactPhone]                      nvarchar(50),
	[ContactCellPhone]                  nvarchar(50),
	[ShippingRegionSelectedMode]        nvarchar(50),
	[ShippingRegionShippingRegionID]    int,
	[ShippingRegionPointOfSalesList]    nvarchar(max),
	[MaxDiscountType]                   nvarchar(50),
	[MaxDiscountAmount]                 decimal(19,6),
	[WebSiteSettingsWebSiteFilter]      nvarchar(50),
	[WebSiteSettingsWebSiteGroups]      nvarchar(max),
	[WebSiteSettingsWebSites]           nvarchar(max),
	[PortfolioHasPortfolio]             bit,
	[PortfolioPortfolioAssociationType] nvarchar(50),
constraint [PK_LinxCommerceSalesRepresentatives] primary key (
	doc_company asc,
	SalesRepresentativeID asc
));

--endereço do vendedor (a API não tem id do endereço: a chave é a posição na lista)
create table [untreated].LinxCommerceSalesRepresentativesAddresses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	SalesRepresentativeID    int,
	AddressIndex             int,
	[IsMainAddress]          bit,
	[Name]                   nvarchar(250),
	[ContactName]            nvarchar(250),
	[AddressLine]            nvarchar(400),
	[Number]                 nvarchar(50),
	[Neighbourhood]          nvarchar(250),
	[City]                   nvarchar(250),
	[State]                  nvarchar(50),
	[PostalCode]             nvarchar(20),
	[AddressNotes]           nvarchar(400),
	[Landmark]               nvarchar(400),
	[Latitude]               decimal(12,8),
	[Longitude]              decimal(12,8)
);

create table [linxcommerce].LinxCommerceSalesRepresentativesAddresses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	SalesRepresentativeID    int not null,
	AddressIndex             int not null,
	[IsMainAddress]          bit,
	[Name]                   nvarchar(250),
	[ContactName]            nvarchar(250),
	[AddressLine]            nvarchar(400),
	[Number]                 nvarchar(50),
	[Neighbourhood]          nvarchar(250),
	[City]                   nvarchar(250),
	[State]                  nvarchar(50),
	[PostalCode]             nvarchar(20),
	[AddressNotes]           nvarchar(400),
	[Landmark]               nvarchar(400),
	[Latitude]               decimal(12,8),
	[Longitude]              decimal(12,8),
constraint [PK_LinxCommerceSalesRepresentativesAddresses] primary key (
	doc_company asc,
	SalesRepresentativeID asc,
	AddressIndex asc
));

--cliente da carteira do vendedor (Portfolio.Customers)
create table [untreated].LinxCommerceSalesRepresentativesCustomers (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	SalesRepresentativeID    int,
	[CustomerID]             nvarchar(50),
	[Status]                 nvarchar(50),
	[IsMaxDiscountEnabled]   nvarchar(10)
);

create table [linxcommerce].LinxCommerceSalesRepresentativesCustomers (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	SalesRepresentativeID    int not null,
	[CustomerID]             nvarchar(50) not null,
	[Status]                 nvarchar(50),
	[IsMaxDiscountEnabled]   nvarchar(10),
constraint [PK_LinxCommerceSalesRepresentativesCustomers] primary key (
	doc_company asc,
	SalesRepresentativeID asc,
	CustomerID asc
));

/****** Chaves estrangeiras: filhas -> pai ******/

alter table [linxcommerce].LinxCommerceSalesRepresentativesAddresses add constraint [FK_LinxCommerceSalesRepresentativesAddresses] foreign key (doc_company, SalesRepresentativeID) references [linxcommerce].LinxCommerceSalesRepresentatives (doc_company, SalesRepresentativeID);
alter table [linxcommerce].LinxCommerceSalesRepresentativesCustomers add constraint [FK_LinxCommerceSalesRepresentativesCustomers] foreign key (doc_company, SalesRepresentativeID) references [linxcommerce].LinxCommerceSalesRepresentatives (doc_company, SalesRepresentativeID);
