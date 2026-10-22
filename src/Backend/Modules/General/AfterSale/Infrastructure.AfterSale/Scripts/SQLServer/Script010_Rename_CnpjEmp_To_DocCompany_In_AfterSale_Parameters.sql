--O Script001 passou a criar a coluna de CNPJ de AfterSaleParameters como doc_company (antes cnpj_emp), e as queries do
--AfterSaleCommandHandler leem DOC_COMPANY. Bancos que rodaram o Script001 antigo continuam com cnpj_emp: renomeia só
--nesse caso (o DbUp não executa de novo um script que já rodou).
if col_length('general.AfterSaleParameters', 'cnpj_emp') is not null
	and col_length('general.AfterSaleParameters', 'doc_company') is null
	exec sp_rename 'general.AfterSaleParameters.cnpj_emp', 'doc_company', 'COLUMN';
