/****** Log das chamadas sob demanda (src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §9, fase 3) ******/
--log das chamadas sob demanda feitas pelo LinxMicrovixController na API do Microvix (GET por chave, com X-User-Name).
--tabela somente de inserção: gravada direto no schema linxmicrovix, sem untreated nem procedure de merge (igual ao TotvsRequests).
create table [linxmicrovix].LinxMicrovixRequests (
	id bigint identity(1,1) not null,
	request_date datetime not null,
	execution uniqueidentifier not null, --auditing.Logs.Execution da chamada
	id_job int not null,                 --auditing.Jobs.IdJob (IdType = 2)
	[user_name] nvarchar(100) not null,  --cabeçalho X-User-Name
	http_method varchar(10) not null,
	[route] nvarchar(400) not null,      --comando chamado na API do Microvix (ex.: LinxMovimento)
	[doc_company] nvarchar(14) not null,
	field_key_value nvarchar(400) null,  --chave da busca (ex.: cnpj da loja|documento)
	request_body nvarchar(max) null,     --XML enviado, com a chave do portal trocada por (oculta)
	response_body nvarchar(max) null,
	is_success bit not null,
	integrated_or_updated_at datetime null, --horário da gravação, em Brasília, igual ao das tabelas de dados (lido pelo DW)
constraint [PK_LinxMicrovixRequests] primary key (
	id asc
));

create nonclustered index [IX_LinxMicrovixRequests_Company_Key] on [linxmicrovix].LinxMicrovixRequests ([doc_company], field_key_value);
create nonclustered index [IX_LinxMicrovixRequests_User_Date] on [linxmicrovix].LinxMicrovixRequests ([user_name], request_date);
