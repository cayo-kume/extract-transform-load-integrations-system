/****** Landing das origens Totvs da fase 1 (src/Backend/Modules/DataWarehouse/docs, seção 2 e 5.1) ******/
--catálogo e geração: uma landing e uma procedure de carga por tabela; nas filhas que o merge da Totvs apaga
--(@delete_by_invoice, @delete_by_order, @delete_by_product, @delete_by_reference, gridCode), também a detecção de exclusão.
--dado pessoal identificador fica fora (5.2, regra 6): em pessoas e empresas, só as colunas listadas em included_columns;
--em notas, pedidos e fornecedores, as colunas de documento, nome, endereço e telefone vão em excluded_columns.
--cidade e UF do cliente ficam: não identificam a pessoa e servem para a análise por região.

insert into [dw_control].LandingSource (source_schema, source_table, included_columns, excluded_columns, source_updated_column, detect_missing, history_frequency, phase)
values
	--notas
	('totvs', 'TotvsInvoices', null,
		'personName, personPersonName, personPersonCpfCnpj, personRgIe, personAddress, personAddressNumber, personComplement, personPublicPlace, personNeighborhood, personCep, personPostOfficeBox, personFoneNumber, sellerCpf, observationNFE',
		'maxChangeFilterDate', 0, 'hourly', 1),
	('totvs', 'TotvsInvoicesItems',         null, null, null, 1, null, 1),
	('totvs', 'TotvsInvoicesItemsProducts', null, null, null, 1, null, 1),
	('totvs', 'TotvsInvoicesItemsTaxes',    null, null, null, 1, null, 1),
	('totvs', 'TotvsInvoicesPayments',      null, null, null, 1, null, 1),
	('totvs', 'TotvsInvoicesSalesOrders',   null, null, null, 1, null, 1),

	--pedidos
	('totvs', 'TotvsSaleOrders', null,
		'customerName, customerCpfCnpj, guideCpfCnpj, sellerCpfCnpj, representativeCpfCnpj, shippingAddressPublicPlace, shippingAddressAddress, shippingAddressNumber, shippingAddressComplement, shippingAddressNeighborhood, shippingAddressCep, shippingAddressPostOfficeBox, shippingAddressReference',
		'maxChangeFilterDate', 0, 'hourly', 1),
	('totvs', 'TotvsSaleOrdersProducts', null, null, 'lastChangeDate', 1, null, 1),

	--pessoas e empresas: só chave, datas e atributos não identificadores
	('totvs', 'TotvsPeople',
		'isInactive, maritalStatus, gender, nationality, branchInsertCode, codeMainRelated, insertDate, isCustomer, isSupplier, isRepresentative, isPurchasingGuide, isShippingCompany, isEmployee, employeeStatus, customerStatus, preferencesPaymentConditionCode, preferencesPriceTableCode',
		null, 'maxChangeFilterDate', 0, 'daily', 1),
	('totvs', 'TotvsLegalEntities',
		'isInactive, branchInsertCode, uf, codeActivity, codeActivityCnae, codeActivityCnae2, typeTaxRegime, codeMainRelated, insertDate, isCustomer, isSupplier, isRepresentative, isPurchasingGuide, isShippingCompany, customerStatus, status, preferencesPaymentConditionCode, preferencesPriceTableCode',
		null, 'maxChangeFilterDate', 0, 'daily', 1),

	--produtos
	('totvs', 'TotvsSkus',                    null, null, 'maxChangeFilterDate', 0, 'daily', 1),
	('totvs', 'TotvsSkusColors',              null, null, null, 0, null, 1),
	('totvs', 'TotvsSkusColorsProducts',      null, null, null, 0, 'daily', 1),
	('totvs', 'TotvsSkusReferenceCategories', null, null, null, 1, null, 1),
	('totvs', 'TotvsCategories',              null, null, 'maxChangeFilterDate', 0, null, 1),
	('totvs', 'TotvsColors',                  null, null, null, 0, null, 1),
	('totvs', 'TotvsGrids',                   null, null, 'maxChangeFilterDate', 0, null, 1),
	('totvs', 'TotvsGridsSizes',              null, null, null, 1, null, 1),
	('totvs', 'TotvsProductsSuppliers',       null, 'cpfCnpj', null, 1, null, 1),

	--lojas e vendedores
	('totvs', 'TotvsBranches', null, null, 'maxChangeFilterDate', 0, 'daily', 1),
	('totvs', 'TotvsSellers',  null, null, 'maxChangeFilterDate', 0, null, 1),

	--custos e preços: estado atual, histórico diário (7.4)
	('totvs', 'TotvsCosts',  null, null, 'maxChangeFilterDate', 0, 'daily', 1),
	('totvs', 'TotvsPrices', null, null, 'maxChangeFilterDate', 0, 'daily', 1);
GO

declare @table sysname;
declare sources cursor local fast_forward for
	select source_table from [dw_control].LandingSource where source_schema = 'totvs' and phase = 1 order by source_table;

open sources;
fetch next from sources into @table;

while @@fetch_status = 0
begin
	exec [dw_landing].sp_Create_Landing @source_schema = 'totvs', @source_table = @table;
	fetch next from sources into @table;
end

close sources;
deallocate sources;
