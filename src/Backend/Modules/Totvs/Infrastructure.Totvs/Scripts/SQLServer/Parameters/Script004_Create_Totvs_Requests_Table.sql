/****** Log das chamadas sob demanda (src/Backend/Modules/Totvs/docs/PROPOSTA-PADRONIZACAO-TOTVS.md 5.5 e 9, fase 3) ******/
--log das chamadas sob demanda feitas pelo TotvsController na API da Totvs (GET por chave, com X-User-Name).
--tabela somente de inserção: gravada direto no schema totvs, sem untreated nem procedure de merge (igual ao MovideskRequests).
create table [totvs].TotvsRequests (
	id bigint identity(1,1) not null,
	request_date datetime not null,
	execution uniqueidentifier not null, --auditing.Logs.Execution da chamada
	id_job int not null,                 --auditing.Jobs.IdJob (IdType = 2)
	[user_name] nvarchar(100) not null,  --cabeçalho X-User-Name
	http_method varchar(10) not null,
	[route] nvarchar(400) not null,      --rota chamada na API da Totvs (sem o token)
	[doc_company] nvarchar(14) not null,
	field_key_value nvarchar(400) null,  --chave do registro (ex.: código da pessoa, loja|número do pedido)
	request_body nvarchar(max) null,     --corpo enviado (as buscas da Totvs são POST com filtro)
	response_body nvarchar(max) null,
	is_success bit not null,
	status_code int null,                --status da Totvs: 0 quando a chamada lançou exceção; null sem chamada à rota (login que falhou)
	integrated_or_updated_at datetime null, --horário da gravação, em Brasília, igual ao das tabelas de dados (lido pelo DW)
constraint [PK_TotvsRequests] primary key (
	id asc
));

create nonclustered index [IX_TotvsRequests_Company_Key] on [totvs].TotvsRequests ([doc_company], field_key_value);
create nonclustered index [IX_TotvsRequests_User_Date] on [totvs].TotvsRequests ([user_name], request_date);
