/****** Schemas do DW (docs/datawarehouse, 4.1 e 8.1) ******/
--schemas em snake_case; tabelas, views e procedures em PascalCase inglês (1.2, item 8).
--o migrator do DW roda por último (1.1), mas a conferência deixa o script independente da ordem.
if not exists (select 1 from sys.schemas where name = 'dw_landing')
    exec('create schema dw_landing');        --espelho do estado atual das tabelas de origem

if not exists (select 1 from sys.schemas where name = 'dw_history')
    exec('create schema dw_history');        --versões SCD2 das linhas da landing

if not exists (select 1 from sys.schemas where name = 'dw_staging')
    exec('create schema dw_staging');        --views de tipagem e renomeação

if not exists (select 1 from sys.schemas where name = 'dw_intermediate')
    exec('create schema dw_intermediate');   --unificação dos ERPs e regras de negócio

if not exists (select 1 from sys.schemas where name = 'datawarehouse')
    exec('create schema datawarehouse');     --modelo estrela: dimensões e fatos

if not exists (select 1 from sys.schemas where name = 'datamart')
    exec('create schema datamart');          --agregados lidos pelas ferramentas de BI

if not exists (select 1 from sys.schemas where name = 'dw_outbound')
    exec('create schema dw_outbound');       --views lidas pelos módulos de saída (4.2)

if not exists (select 1 from sys.schemas where name = 'dw_reference')
    exec('create schema dw_reference');      --tabelas de referência mantidas por script

if not exists (select 1 from sys.schemas where name = 'dw_control')
    exec('create schema dw_control');        --watermarks e resultado das checagens

if not exists (select 1 from sys.schemas where name = 'dw_personal_data')
    exec('create schema dw_personal_data');  --dado pessoal em claro, acesso restrito (seção 12)
