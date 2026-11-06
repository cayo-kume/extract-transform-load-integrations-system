/****** Seleção de tabelas dos migrators dos módulos (.docs/MIGRATOR-CSX.md, "Escolha por job") ******/
-- Os scripts de tabelas e de procedures de sincronização dos services rodam em toda migração (RunAlways no DbUp) e só
-- criam o que ainda não existe e está na seleção. Antes de rodar os scripts, o migrator.csx do módulo grava aqui os
-- nomes das tabelas dos jobs escolhidos no tools/migrators/Invoke-Migrators.ps1 (variável ETL_MIGRATOR_TABLES), ou '*'
-- para todas (o migrator rodado à mão, sem a variável). O nome é o da tabela sem o schema: vale para a untreated e a
-- trusted.

if object_id(N'[dbo].[MigratorTables]') is null
create table [dbo].[MigratorTables] (
	[Name] nvarchar(128) not null,
constraint [PK_MigratorTables] primary key ([Name])
);
go

-- 1 se a tabela (com o schema, ex.: '[linxmicrovix].LinxMovimentoRemessas') ainda não existe e está na seleção
create or alter function [dbo].[fn_Migrator_Creates] (@table nvarchar(300)) returns bit as
begin
	if object_id(@table) is not null
		return 0;

	if exists (select 1 from [dbo].[MigratorTables] where [Name] in (N'*', parsename(@table, 1)))
		return 1;

	return 0;
end
go
