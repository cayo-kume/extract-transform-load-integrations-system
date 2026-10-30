--log das chamadas de escrita (POST/PUT) feitas pelo AfterSaleController na API da AfterSale.
--tabela somente de inserção: gravada direto no schema aftersale, sem untreated nem procedure de merge.
create table [aftersale].AfterSaleRequests (
	id bigint identity(1,1) not null,
	request_date datetime not null,
	execution uniqueidentifier not null, --auditing.Logs.Execution da chamada
	id_job int not null,                 --auditing.Jobs.IdJob (IdType = 2)
	[user_name] nvarchar(100) not null,  --cabeçalho X-User-Name
	http_method varchar(10) not null,
	[route] nvarchar(400) not null,      --rota chamada na API da AfterSale
	[doc_company] nvarchar(14) not null,
	field_key_value nvarchar(400) null,  --chave do registro alterado (ex.: id da reversa)
	request_body nvarchar(max) null,     --json enviado, ou o XML inteiro na rota de invoice por arquivo
	response_body nvarchar(max) null,
	is_success bit not null,
	integrated_or_updated_at datetime null, --horário da gravação, em Brasília, igual ao das tabelas de dados (lido pelo DW)
constraint [PK_AfterSaleRequests] primary key (
	id asc
));

create nonclustered index [IX_AfterSaleRequests_Company_Key] on [aftersale].AfterSaleRequests ([doc_company], field_key_value);
create nonclustered index [IX_AfterSaleRequests_User_Date] on [aftersale].AfterSaleRequests ([user_name], request_date);
