--log das chamadas sob demanda feitas pelo MovideskController na API da Movidesk.
--tabela somente de inserção: gravada direto no schema movidesk, sem untreated nem procedure de merge.
create table [movidesk].MovideskRequests (
	id bigint identity(1,1) not null,
	request_date datetime not null,
	execution uniqueidentifier not null, --auditing.Logs.Execution da chamada
	id_job int not null,                 --auditing.Jobs.IdJob (IdType = 2)
	[user_name] nvarchar(100) not null,  --cabeçalho X-User-Name
	http_method varchar(10) not null,
	[route] nvarchar(400) not null,      --rota chamada na API da Movidesk (sem o token)
	[doc_company] nvarchar(14) not null,
	field_key_value nvarchar(400) null,  --chave do registro (ex.: número do ticket)
	request_body nvarchar(max) null,     --corpo enviado; nulo nas chamadas GET
	response_body nvarchar(max) null,
	is_success bit not null,
constraint [PK_MovideskRequests] primary key (
	id asc
));

create nonclustered index [IX_MovideskRequests_Company_Key] on [movidesk].MovideskRequests ([doc_company], field_key_value);
create nonclustered index [IX_MovideskRequests_User_Date] on [movidesk].MovideskRequests ([user_name], request_date);
