/****** integrated_or_updated_at em MovideskRequests: padrão das tabelas de dados, para o log de chamadas ser auditado e lido pelo DW (docs/datawarehouse, 1.2 item 10) ******/
--horário da gravação, em Brasília, igual ao das tabelas de dados; a entidade Request preenche nas linhas novas.
--nas linhas anteriores recebe o request_date, para nenhuma ficar nula.
alter table [movidesk].MovideskRequests add integrated_or_updated_at datetime null;
go

update [movidesk].MovideskRequests set integrated_or_updated_at = request_date where integrated_or_updated_at is null;
