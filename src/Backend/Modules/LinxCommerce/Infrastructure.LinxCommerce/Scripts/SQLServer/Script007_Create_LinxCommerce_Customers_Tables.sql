/****** Clientes: tabelas untreated e linxcommerce (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.2) ******/
--colunas e tamanhos tirados do código antigo, sem resposta real da API: a conferir antes de ligar o job (§9).
--datas da API em datetimeoffset (o Linx Commerce manda o fuso); integrated_or_updated_at é o horário do lote, em Brasília

--cliente (POST Profile/API.svc/web/GetPerson ou GetCompany), com Contact e EmailConfirmation (1:1) em colunas e sem senha nem hash
create table [untreated].LinxCommerceCustomers (
	integrated_or_updated_at            datetime,
	[doc_company]                       nvarchar(14),
	[CustomerID]                        int,
	[CustomerType]                      nvarchar(10),
	[CustomerStatusID]                  int,
	[WebSiteID]                         nvarchar(50),
	[Name]                              nvarchar(250),
	[Surname]                           nvarchar(250),
	[TradingName]                       nvarchar(250),
	[Email]                             nvarchar(250),
	[Cpf]                               nvarchar(20),
	[Cnpj]                              nvarchar(20),
	[RG]                                nvarchar(30),
	[Gender]                            nvarchar(10),
	[BirthDate]                         datetimeoffset,
	[CreatedDate]                       datetimeoffset,
	[ContactPhone]                      nvarchar(50),
	[ContactPhone2]                     nvarchar(50),
	[ContactCellPhone]                  nvarchar(50),
	[ContactFax]                        nvarchar(50),
	[EmailConfirmationStatus]           nvarchar(50),
	[EmailConfirmationConfirmationDate] datetimeoffset
);

create table [linxcommerce].LinxCommerceCustomers (
	integrated_or_updated_at            datetime,
	[doc_company]                       nvarchar(14) not null,
	[CustomerID]                        int not null,
	[CustomerType]                      nvarchar(10),
	[CustomerStatusID]                  int,
	[WebSiteID]                         nvarchar(50),
	[Name]                              nvarchar(250),
	[Surname]                           nvarchar(250),
	[TradingName]                       nvarchar(250),
	[Email]                             nvarchar(250),
	[Cpf]                               nvarchar(20),
	[Cnpj]                              nvarchar(20),
	[RG]                                nvarchar(30),
	[Gender]                            nvarchar(10),
	[BirthDate]                         datetimeoffset,
	[CreatedDate]                       datetimeoffset,
	[ContactPhone]                      nvarchar(50),
	[ContactPhone2]                     nvarchar(50),
	[ContactCellPhone]                  nvarchar(50),
	[ContactFax]                        nvarchar(50),
	[EmailConfirmationStatus]           nvarchar(50),
	[EmailConfirmationConfirmationDate] datetimeoffset,
constraint [PK_LinxCommerceCustomers] primary key (
	doc_company asc,
	CustomerID asc
));

--endereço do cliente
create table [untreated].LinxCommerceCustomersAddresses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	CustomerID               int,
	[ID]                     int,
	[MainAddress]            bit,
	[Name]                   nvarchar(250),
	[ContactName]            nvarchar(250),
	[AddressLine]            nvarchar(400),
	[Number]                 nvarchar(50),
	[Neighbourhood]          nvarchar(250),
	[City]                   nvarchar(250),
	[State]                  nvarchar(50),
	[PostalCode]             nvarchar(20),
	[AddressNotes]           nvarchar(400),
	[Landmark]               nvarchar(400)
);

create table [linxcommerce].LinxCommerceCustomersAddresses (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	CustomerID               int not null,
	[ID]                     int not null,
	[MainAddress]            bit,
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
constraint [PK_LinxCommerceCustomersAddresses] primary key (
	doc_company asc,
	CustomerID asc,
	ID asc
));

--grupo do cliente (hoje gravado como texto em Customer.CustomerGroupID)
create table [untreated].LinxCommerceCustomersGroups (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14),
	CustomerID               int,
	[CustomerGroupID]        nvarchar(50),
	[Title]                  nvarchar(250)
);

create table [linxcommerce].LinxCommerceCustomersGroups (
	integrated_or_updated_at datetime,
	[doc_company]            nvarchar(14) not null,
	CustomerID               int not null,
	[CustomerGroupID]        nvarchar(50) not null,
	[Title]                  nvarchar(250),
constraint [PK_LinxCommerceCustomersGroups] primary key (
	doc_company asc,
	CustomerID asc,
	CustomerGroupID asc
));

/****** Chaves estrangeiras: filhas -> pai ******/

alter table [linxcommerce].LinxCommerceCustomersAddresses add constraint [FK_LinxCommerceCustomersAddresses] foreign key (doc_company, CustomerID) references [linxcommerce].LinxCommerceCustomers (doc_company, CustomerID);
alter table [linxcommerce].LinxCommerceCustomersGroups add constraint [FK_LinxCommerceCustomersGroups] foreign key (doc_company, CustomerID) references [linxcommerce].LinxCommerceCustomers (doc_company, CustomerID);
