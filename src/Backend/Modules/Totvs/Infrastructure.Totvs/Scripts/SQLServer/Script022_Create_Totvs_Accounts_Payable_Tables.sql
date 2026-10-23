/****** Contas a pagar: tabelas untreated e totvs (docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.2.7) ******/
--tamanhos do swagger da API Accounts Payable. Duplicata: uma linha por parcela (o mesmo duplicateCode volta uma vez por
--parcela). Comissão: uma linha por comissionado do fechamento (a rota não tem filtro de alteração nem maxChangeFilterDate).

create table [untreated].TotvsDuplicates (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14),
	maxChangeFilterDate            datetime,
	branchCode                     int,
	duplicateCode                  bigint,
	supplierCode                   int,
	supplierCpfCnpj                nvarchar(14),
	installmentCode                int,
	bearerCode                     int,
	entryDate                      datetime,
	issueDate                      datetime,
	dueDate                        datetime,
	settlementDate                 datetime,
	arrivalDate                    datetime,
	[status]                       nvarchar(20),
	duplicateValue                 decimal(18,6),
	feesValue                      decimal(18,6),
	discountValue                  decimal(18,6),
	paidValue                      decimal(18,6),
	inclusionType                  nvarchar(40),
	userInclusionCode              int,
	userInclusionName              nvarchar(60)
);

create table [totvs].TotvsDuplicates (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14) not null,
	maxChangeFilterDate            datetime,
	branchCode                     int not null,
	duplicateCode                  bigint not null,
	supplierCode                   int not null,
	supplierCpfCnpj                nvarchar(14),
	installmentCode                int not null,
	bearerCode                     int,
	entryDate                      datetime,
	issueDate                      datetime,
	dueDate                        datetime,
	settlementDate                 datetime,
	arrivalDate                    datetime,
	[status]                       nvarchar(20),
	duplicateValue                 decimal(18,6),
	feesValue                      decimal(18,6),
	discountValue                  decimal(18,6),
	paidValue                      decimal(18,6),
	inclusionType                  nvarchar(40),
	userInclusionCode              int,
	userInclusionName              nvarchar(60),
constraint [PK_TotvsDuplicates] primary key (
	doc_company asc,
	branchCode asc,
	supplierCode asc,
	duplicateCode asc,
	installmentCode asc
));

create table [untreated].TotvsDuplicatesExpenses (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14),
	branchCode                     int,
	supplierCode                   int,
	duplicateCode                  bigint,
	installmentCode                int,
	expenseCode                    int,
	expenseName                    nvarchar(100),
	costCenterCode                 int,
	proratedPercentage             decimal(18,6),
	proratedValue                  decimal(18,6)
);

create table [totvs].TotvsDuplicatesExpenses (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14) not null,
	branchCode                     int not null,
	supplierCode                   int not null,
	duplicateCode                  bigint not null,
	installmentCode                int not null,
	expenseCode                    int not null,
	expenseName                    nvarchar(100),
	costCenterCode                 int not null,
	proratedPercentage             decimal(18,6),
	proratedValue                  decimal(18,6),
constraint [PK_TotvsDuplicatesExpenses] primary key (
	doc_company asc,
	branchCode asc,
	supplierCode asc,
	duplicateCode asc,
	installmentCode asc,
	expenseCode asc,
	costCenterCode asc
));

create table [untreated].TotvsDuplicatesClassifications (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14),
	branchCode                     int,
	supplierCode                   int,
	duplicateCode                  bigint,
	installmentCode                int,
	classificationType             int,
	classificationCode             nvarchar(10),
	classificationTypeDescription  nvarchar(60),
	classificationDescription      nvarchar(60)
);

create table [totvs].TotvsDuplicatesClassifications (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14) not null,
	branchCode                     int not null,
	supplierCode                   int not null,
	duplicateCode                  bigint not null,
	installmentCode                int not null,
	classificationType             int not null,
	classificationCode             nvarchar(10) not null,
	classificationTypeDescription  nvarchar(60),
	classificationDescription      nvarchar(60),
constraint [PK_TotvsDuplicatesClassifications] primary key (
	doc_company asc,
	branchCode asc,
	supplierCode asc,
	duplicateCode asc,
	installmentCode asc,
	classificationType asc,
	classificationCode asc
));

create table [untreated].TotvsCommissionsPaid (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14),
	closingBranchCode              int,
	closingDate                    datetime,
	closingCode                    int,
	commissionedCpfCnpj            nvarchar(14),
	commissionedName               nvarchar(60),
	commissionStatus               int,
	valueBilling                   decimal(18,6),
	valueBillingCommission         decimal(18,6),
	valueReceiving                 decimal(18,6),
	valueReceivingCommission       decimal(18,6),
	irrfValue                      decimal(18,6),
	issqnValue                     decimal(18,6),
	valueRebate                    decimal(18,6),
	valueRebateBilling             decimal(18,6),
	valueRebateReceiving           decimal(18,6),
	discountValue                  decimal(18,6),
	netValue                       decimal(18,6),
	accountBalanceValue            decimal(18,6),
	grossValue                     decimal(18,6),
	returnValue                    decimal(18,6)
);

create table [totvs].TotvsCommissionsPaid (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14) not null,
	closingBranchCode              int not null,
	closingDate                    datetime,
	closingCode                    int not null,
	commissionedCpfCnpj            nvarchar(14) not null,
	commissionedName               nvarchar(60),
	commissionStatus               int,
	valueBilling                   decimal(18,6),
	valueBillingCommission         decimal(18,6),
	valueReceiving                 decimal(18,6),
	valueReceivingCommission       decimal(18,6),
	irrfValue                      decimal(18,6),
	issqnValue                     decimal(18,6),
	valueRebate                    decimal(18,6),
	valueRebateBilling             decimal(18,6),
	valueRebateReceiving           decimal(18,6),
	discountValue                  decimal(18,6),
	netValue                       decimal(18,6),
	accountBalanceValue            decimal(18,6),
	grossValue                     decimal(18,6),
	returnValue                    decimal(18,6),
constraint [PK_TotvsCommissionsPaid] primary key (
	doc_company asc,
	closingBranchCode asc,
	closingCode asc,
	commissionedCpfCnpj asc
));

create table [untreated].TotvsCommissionsPaidAccountMovements (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14),
	closingBranchCode              int,
	closingCode                    int,
	commissionedCpfCnpj            nvarchar(14),
	[sequence]                     int,
	movementDate                   datetime,
	auxiliaryHistoryDescription    nvarchar(60),
	documentDescription            nvarchar(15),
	historyCode                    int,
	historyDescription             nvarchar(40),
	launchValue                    decimal(18,6)
);

create table [totvs].TotvsCommissionsPaidAccountMovements (
	integrated_or_updated_at       datetime,
	doc_company                    nvarchar(14) not null,
	closingBranchCode              int not null,
	closingCode                    int not null,
	commissionedCpfCnpj            nvarchar(14) not null,
	[sequence]                     int not null,
	movementDate                   datetime,
	auxiliaryHistoryDescription    nvarchar(60),
	documentDescription            nvarchar(15),
	historyCode                    int,
	historyDescription             nvarchar(40),
	launchValue                    decimal(18,6),
constraint [PK_TotvsCommissionsPaidAccountMovements] primary key (
	doc_company asc,
	closingBranchCode asc,
	closingCode asc,
	commissionedCpfCnpj asc,
	[sequence] asc
));

/****** Chaves estrangeiras: filhas -> registro ******/

alter table [totvs].TotvsDuplicatesExpenses add constraint [FK_TotvsDuplicatesExpenses] foreign key (doc_company, branchCode, supplierCode, duplicateCode, installmentCode) references [totvs].TotvsDuplicates (doc_company, branchCode, supplierCode, duplicateCode, installmentCode);
alter table [totvs].TotvsDuplicatesClassifications add constraint [FK_TotvsDuplicatesClassifications] foreign key (doc_company, branchCode, supplierCode, duplicateCode, installmentCode) references [totvs].TotvsDuplicates (doc_company, branchCode, supplierCode, duplicateCode, installmentCode);
alter table [totvs].TotvsCommissionsPaidAccountMovements add constraint [FK_TotvsCommissionsPaidAccountMovements] foreign key (doc_company, closingBranchCode, closingCode, commissionedCpfCnpj) references [totvs].TotvsCommissionsPaid (doc_company, closingBranchCode, closingCode, commissionedCpfCnpj);
