# DW do NEWBLOOMERS · 06. Procedures, checagens e projeto

> Guia 6 de 11 do guia do DW, versão 5. Seções deste arquivo: 9. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 9. Projeto no repositório

```
src/Backend/Modules/DataWarehouse/
├── Application.DataWarehouse/        job que chama as procedures na ordem da 10.1 e grava o resultado das checagens
├── Domain.DataWarehouse/             contratos e entidades (watermark, resultado de checagem)
└── Infrastructure.DataWarehouse/
    └── Scripts/SQLServer/            migrator.csx e scripts numerados (tabelas, procedures, views, seeds)
```

- Os projetos novos entram em `etl-integrations-system.slnx`, com a convenção `Application.X`, `Domain.X` e `Infrastructure.X` do repositório.
- **Scripts previstos** (o nome do módulo está no nome do arquivo, como nos demais, para não colidir no banco):

| Script | Conteúdo |
|---|---|
| `Script001_Create_DataWarehouse_Schemas.sql` | Schemas `dw_landing`, `dw_history`, `dw_staging`, `dw_intermediate`, `datawarehouse`, `datamart`, `dw_reference`, `dw_control`, `dw_personal_data` |
| `Script002_Create_DataWarehouse_Watermarks.sql` | `dw_control.watermark` |
| `Script010_Create_DataWarehouse_Landing_Totvs.sql` | `dw_landing.totvs_*` (uma tabela por fonte) |
| `Script011_Create_DataWarehouse_Landing_Procedures.sql` | Procedures de MERGE da landing |
| `Script020_Create_DataWarehouse_History_Totvs.sql` | `dw_history.totvs_*` e procedures de snapshot |
| `Script030_Create_DataWarehouse_PersonalData.sql` | `dw_personal_data.dim_cliente_identificado` e `dw_personal_data.sp_cliente_load` (8.3) |
| `Script031_Create_DataWarehouse_Dimensions.sql` | Dimensões (tabelas e procedures de carga; `sp_dim_cliente_load` lê `dw_personal_data` e recebe o pepper) |
| `Script040_Create_DataWarehouse_Facts_Totvs.sql` | Fatos de venda, pedido e pagamento |
| `Script050_Create_DataWarehouse_Datamart.sql` | Agregados |
| `Script060_Create_DataWarehouse_Checks.sql` | Views de checagem (`datawarehouse.vw_check_*`) e `dw_control.check_result` |
| `Script070_Insert_DataWarehouse_Ref.sql` | Seeds: `dw_reference.feriados`, `dw_reference.regra_canal`, `dw_reference.mapa_loja_erp` |

- **Numeração por camada:** 001–009 schemas e watermarks; 010–019 `dw_landing`; 020–029 `dw_history`; 030–039 dado pessoal e dimensões; 040–049 fatos; 050–059 `datamart`; 060–069 checagens; 070 em diante, seeds e alterações posteriores, sempre em script novo.
- **Nomes:** `dw_landing.<tabela>` espelha a tabela de origem; `dw_history.<tabela>` guarda as versões; procedures `dw_landing.sp_<tabela>_load`, `dw_history.sp_<entidade>_snapshot`, `dw_personal_data.sp_<entidade>_load`, `datawarehouse.sp_<entidade>_load` e `datamart.sp_<agregado>_refresh`; checagens `datawarehouse.vw_check_<nome>`.
- No Windows, o caminho do `migrator.csx` passa de 260 caracteres. Usar `subst` para rodar, como descrito em `docs/MIGRATOR-CSX.md`.

### 9.1 Exemplo de procedure de fato (MERGE)

```sql
CREATE OR ALTER PROCEDURE datawarehouse.sp_fato_venda_item_load
AS
BEGIN
    SET NOCOUNT ON;

    MERGE datawarehouse.fato_venda_item AS destino
    USING (
        SELECT v.doc_company,
               v.branchCode,
               v.invoiceSequence,
               v.invoiceDate,
               v.[sequence],
               v.data_faturamento,
               CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT_WS('|', v.doc_company, v.branchCode)), 2)                 AS loja_sk,
               CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT_WS('|', v.doc_company, v.productCode)), 2)               AS produto_sk
               -- demais chaves e medidas
          FROM dw_intermediate.venda_item AS v     -- recarga completa a cada execução (pergunta 8); otimizar depois de medir
    ) AS origem
       ON destino.doc_company = origem.doc_company
      AND destino.branchCode = origem.branchCode
      AND destino.invoiceSequence = origem.invoiceSequence
      AND destino.invoiceDate = origem.invoiceDate
      AND destino.[sequence] = origem.[sequence]
    WHEN MATCHED THEN
        UPDATE SET destino.data_faturamento = origem.data_faturamento,
                   destino.loja_sk = origem.loja_sk,
                   destino.produto_sk = origem.produto_sk
    WHEN NOT MATCHED THEN
        INSERT (doc_company, branchCode, invoiceSequence, invoiceDate, [sequence], data_faturamento, loja_sk, produto_sk)
        VALUES (origem.doc_company, origem.branchCode, origem.invoiceSequence, origem.invoiceDate, origem.[sequence],
                origem.data_faturamento, origem.loja_sk, origem.produto_sk);
END;
```

Os nomes de coluna são ilustrativos. A chave do MERGE é a PK da origem, e a checagem de grão (9.2) confirma que ela é única. Sem filtro de janela: a recarga é completa a cada execução (pergunta 8), e a otimização vem depois da medição.

### 9.2 Checagens (substituem os testes do dbt)

Cada checagem é uma view que devolve as linhas com erro. O job conta as linhas de cada view, grava o resultado em `dw_control.check_result` e falha se alguma tiver linhas.

```sql
CREATE OR ALTER VIEW datawarehouse.vw_check_fato_venda_grao AS
SELECT doc_company, branchCode, invoiceSequence, invoiceDate, [sequence], COUNT(*) AS linhas
  FROM datawarehouse.fato_venda_item
 GROUP BY doc_company, branchCode, invoiceSequence, invoiceDate, [sequence]
HAVING COUNT(*) > 1;
```

Limites com porcentagem (ex.: membro desconhecido acima de 5%) entram como `HAVING` ou como comparação no próprio job.

```sql
CREATE TABLE dw_control.check_result (
    check_name       VARCHAR(100) NOT NULL,
    executed_at      DATETIME2(3) NOT NULL,   -- horário de Brasília
    linhas_com_erro  INT          NOT NULL,
    passou           BIT          NOT NULL,
    CONSTRAINT pk_check_result PRIMARY KEY (check_name, executed_at)
);
```

### 9.3 Checagem de reconciliação

`datamart.vw_check_reconciliacao_faturado` compara o total mensal por loja de `datamart.vendas_diarias` com a soma direta de `dw_landing.totvs_invoices_items`, pela mesma regra, e devolve as lojas em que a diferença passa de R$ 1,00. Mostra se a divergência com o fechamento de referência é real ou só diferença de definição (seção 7.3).

### 9.4 Registro de execuções

O job grava início e fim com `auditing.sp_Log_Start` e `auditing.sp_Log_End`, e uma mensagem por procedure com `auditing.sp_Log_Message`. O resultado de cada checagem vai para `dw_control.check_result`. A retenção de cada um é decidida na seção 3.9.
