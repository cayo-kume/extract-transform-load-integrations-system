/****** Log das chamadas sob demanda (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 5.5 e 9, fase 3) ******/
--log das chamadas sob demanda feitas pelo LinxCommerceController na API do Linx Commerce (GET por chave, com X-User-Name).
--tabela somente de inserção: gravada direto no schema linxcommerce, sem untreated nem procedure de merge (igual ao TotvsRequests).
create table [linxcommerce].LinxCommerceRequests (
	id bigint identity(1,1) not null,
	request_date datetime not null,
	execution uniqueidentifier not null, --auditing.Logs.Execution da chamada
	id_job int not null,                 --auditing.Jobs.IdJob (IdType = 2)
	[user_name] nvarchar(100) not null,  --cabeçalho X-User-Name
	http_method varchar(10) not null,
	[route] nvarchar(400) not null,      --rota chamada no Linx Commerce (v1/..., sem o endereço do cliente nem a autenticação)
	[doc_company] nvarchar(14) not null,
	field_key_value nvarchar(400) null,  --chave do registro (OrderID, número do pedido, CustomerID, ProductID, SalesRepresentativeID)
	request_body nvarchar(max) null,     --corpo enviado (as rotas do Linx Commerce são POST)
	response_body nvarchar(max) null,    --sem os campos sensíveis (senha, hash, cartão e PIX)
	is_success bit not null,             --resposta 2xx com JSON válido
	status_code int null,                --status do Linx Commerce; 0 quando a chamada lançou exceção
	integrated_or_updated_at datetime null, --horário da gravação, em Brasília, igual ao das tabelas de dados (lido pelo DW)
constraint [PK_LinxCommerceRequests] primary key (
	id asc
));

create nonclustered index [IX_LinxCommerceRequests_Company_Key] on [linxcommerce].LinxCommerceRequests ([doc_company], field_key_value);
create nonclustered index [IX_LinxCommerceRequests_User_Date] on [linxcommerce].LinxCommerceRequests ([user_name], request_date);
