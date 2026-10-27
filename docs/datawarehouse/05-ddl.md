# DW do NEWBLOOMERS · 05. DDL de referência

> Guia 5 de 11 do guia do DW, versão 5. Seções deste arquivo: 8. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 8. DDL de referência

Scripts em `src/Backend/Modules/DataWarehouse/Infrastructure.DataWarehouse/Scripts/SQLServer/`, numerados, com o `migrator.csx` do padrão do repositório, sem connection string padrão. Cada script roda uma vez. Os nomes precisam ser únicos no repositório, porque o DbUp grava o nome de cada arquivo. Procedures e views de negócio usam `CREATE OR ALTER`; alteração entra em script novo.

Horário padrão das colunas de carga, o mesmo das procedures de auditoria:

```sql
SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
```

### 8.1 Schemas e watermarks

```sql
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'dw_landing')
    EXEC('CREATE SCHEMA dw_landing');
-- repetir para: dw_history, dw_staging, dw_intermediate, datawarehouse, datamart, dw_reference, dw_control, dw_personal_data

CREATE TABLE dw_control.watermark (
    fonte              VARCHAR(50)  NOT NULL,
    entidade           VARCHAR(100) NOT NULL,
    ultimo_valor       DATETIME2(3) NOT NULL,   -- horário de Brasília: integrated_or_updated_at da origem (landing) ou carregado_em da landing (snapshot, seção 6)
    atualizado_em      DATETIME2(3) NOT NULL CONSTRAINT df_watermark_upd DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    CONSTRAINT pk_watermark PRIMARY KEY (fonte, entidade)
);
```

### 8.2 Landing de Totvs

O DDL da `dw_landing.totvs_invoices` está na seção 6. A `dw_landing.totvs_invoices_items` segue o mesmo padrão, com a PK de `totvs.TotvsInvoicesItems` (`doc_company`, `branchCode`, `invoiceSequence`, `invoiceDate`, `[sequence]`). A procedure de carga faz MERGE pela PK. As colunas são as da origem (`Script025_Create_Totvs_Invoices_Tables.sql`) que o DW usa, com os mesmos nomes, para manter a rastreabilidade. Colunas identificadoras de pessoa não entram em nenhuma landing (5.2, regra 6). Como a origem está no mesmo banco, o DW não passa pela área de carga bruta `untreated`.

### 8.3 Dimensão de cliente, com dado pessoal separado

```sql
CREATE TABLE datawarehouse.dim_cliente (
    sk              CHAR(32)     NOT NULL CONSTRAINT pk_dim_cliente PRIMARY KEY,
    doc_company     NVARCHAR(14) NOT NULL,
    cod_cliente     INT          NOT NULL,     -- code em TotvsPeople e TotvsLegalEntities; personCode nas notas
    doc_hash        CHAR(64)     NULL,         -- SHA-256(pepper + CPF/CNPJ); o pepper chega como parâmetro (12.3)
    doc_mascarado   VARCHAR(20)  NULL,         -- ex.: ***.***.***-12
    tipo_pessoa     CHAR(1)      NULL,         -- F / J
    uf              CHAR(2)      NULL,
    faixa_etaria    VARCHAR(20)  NULL          -- calculada de dw_personal_data.data_nascimento (TotvsPeople.birthDate)
);                                             -- SCD1 (pergunta 11): sem valid_from e valid_to

-- dw_personal_data.dim_cliente_identificado: só a identificação atual, sem versões. Só o grupo dw_pii lê (12.2).
-- Carregada por dw_personal_data.sp_cliente_load, direto de totvs.TotvsPeople, TotvsLegalEntities e das tabelas
-- de endereço, e-mail e telefone (registro padrão de cada uma), por MERGE na chave, com o watermark da 5.2.
CREATE TABLE dw_personal_data.dim_cliente_identificado (
    doc_company      NVARCHAR(14)  NOT NULL,
    cod_cliente      INT           NOT NULL,    -- code em TotvsPeople e TotvsLegalEntities
    tipo_pessoa      CHAR(1)       NOT NULL,    -- F (TotvsPeople) / J (TotvsLegalEntities)
    nome             NVARCHAR(60)  NULL,
    cpf_cnpj         NVARCHAR(14)  NULL,        -- em claro; é daqui que sai o doc_hash de dim_cliente
    data_nascimento  DATE          NULL,        -- vira faixa_etaria em dim_cliente
    email            NVARCHAR(60)  NULL,
    telefone         NVARCHAR(20)  NULL,
    endereco         NVARCHAR(120) NULL,
    cidade           NVARCHAR(60)  NULL,
    uf               CHAR(2)       NULL,        -- copiada para dim_cliente
    atualizado_em    DATETIME2(3)  NOT NULL,    -- integrated_or_updated_at da origem, horário de Brasília
    CONSTRAINT pk_dim_cliente_identificado PRIMARY KEY (doc_company, cod_cliente)
);
```

A carga de cliente roda em duas procedures, nessa ordem (10.1, passos 5 e 6): `dw_personal_data.sp_cliente_load`, que lê as tabelas `totvs` e grava a identificação em claro; e `datawarehouse.sp_dim_cliente_load @pepper`, que lê `dw_personal_data` e grava em `dim_cliente` só `doc_hash` (`CONVERT(CHAR(64), HASHBYTES('SHA2_256', CONCAT(@pepper, cpf_cnpj)), 2)`), `doc_mascarado`, `tipo_pessoa`, `uf` e `faixa_etaria`. O pepper chega como parâmetro, fornecido pelo job; ele não existe em tabela nem em script (12.3). Nenhuma coluna desta tabela existe em `dw_landing` ou `dw_history` (5.2, regra 6).

### 8.4 Fato de venda

```sql
CREATE TABLE datawarehouse.fato_venda_item (
    doc_company            NVARCHAR(14)   NOT NULL,
    branchCode             INT            NOT NULL,
    invoiceSequence        INT            NOT NULL,
    invoiceDate            DATETIME       NOT NULL,
    [sequence]             INT            NOT NULL,   -- sequência do item na nota (PK da origem)
    data_faturamento       DATE           NOT NULL,
    loja_sk                CHAR(32)       NOT NULL,
    canal_sk               CHAR(32)       NOT NULL,
    produto_sk             CHAR(32)       NOT NULL,
    cliente_sk             CHAR(32)       NOT NULL,
    vendedor_sk            CHAR(32)       NOT NULL,
    operacao_sk            CHAR(32)       NOT NULL,
    fl_cancelado           BIT            NOT NULL,
    fl_operacao_realizada  BIT            NOT NULL,
    quantidade             DECIMAL(18,6)  NOT NULL,
    valor_bruto            DECIMAL(18,6)  NOT NULL,
    valor_desconto         DECIMAL(18,6)  NOT NULL,
    valor_frete            DECIMAL(18,6)  NOT NULL,
    valor_faturado         DECIMAL(18,6)  NOT NULL,
    custo_medio            DECIMAL(18,6)  NULL,
    carregado_em           DATETIME2(3)   NOT NULL CONSTRAINT df_fvi_load DEFAULT (SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'),
    CONSTRAINT pk_fato_venda_item PRIMARY KEY (doc_company, branchCode, invoiceSequence, invoiceDate, [sequence])
);
```

- O grão é a PK da origem, garantido pela PK do próprio fato. Índice columnstore ou rowstore: decidir com a carga de teste.
- Quando o histórico Linx entrar, a PK ganha `fonte_erp` (ou o equivalente), para uma nota de cada ERP não colidir.
- FKs para as dimensões: validar o custo na carga. Se pesarem, a integridade fica nas checagens da seção 11.
