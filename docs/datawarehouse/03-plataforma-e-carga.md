# DW do NEWBLOOMERS · 03. Plataforma e carga

> Guia 3 de 12 do guia do DW, versão 7. Seções deste arquivo: 3.1 a 3.4 e 3.6. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 3. Decisões técnicas

### 3.1 Plataforma: Azure SQL Database, banco zerado

- **Decidido: o DW vive no banco zerado deste repositório, junto com os módulos de integração.** Azure SQL Database, o mesmo tipo de banco dos módulos: o repositório já usa T-SQL, e PK, FK e UNIQUE são impostas de verdade.
  - Os módulos já gravam nesse banco (`untreated`, `totvs`, `linxmicrovix`, `linxcommerce`, `movidesk`, `aftersale`, `auditing`). O DW é mais um módulo: scripts DbUp, schemas próprios e sem cópia entre bancos.
  - *Consequência, ordem:* as views do DW referenciam tabelas de origem, e o SQL Server valida views na criação. Por isso o migrator do DW roda por último (1.1).
- **Nomes dos schemas:**
  - `datawarehouse`: dimensões e fatos, o modelo estrela.
  - `datamart`: agregados e colunas de métrica que as ferramentas de BI leem, como `datamart.DailySales`. Escolhido em vez de `mart` para manter o nome completo, em linha com `datawarehouse` (pergunta 23).
  - `dw_outbound`: views que os módulos de saída leem (4.2).
  - Os demais schemas do DW levam o prefixo `dw_` e nome por extenso (4.1). Schemas em `snake_case`; tabelas, views e procedures em PascalCase inglês; colunas em `snake_case` inglês (1.2, item 8).

### 3.2 Ingestão: MERGE por procedures no próprio banco (decidido)

- **Decidido:** a carga é um MERGE dentro do banco, feito por procedure: a `dw_landing` recebe as linhas das tabelas confiáveis cujo `integrated_or_updated_at` é maior que o watermark, com 3 dias de sobreposição. Nada é transferido entre bancos, porque a origem está no mesmo banco. Um job chama as procedures em ordem fixa (10.1).
- **Decidido: a `dw_landing` é o espelho do estado que o DW viu**, em vez de ler as tabelas confiáveis direto. Assim, uma mudança no módulo de origem não altera o DW sem aviso, as exclusões de filhas ficam controladas (5.2, regra 3), o dado pessoal identificador fica fora (regra 6) e o snapshot incremental tem o `loaded_at` para filtrar (seção 6). As tabelas do legado Linx entram pelo mesmo MERGE, numa carga única (7.5, 10.2).
- **Os módulos de saída leem o DW pelo mesmo princípio:** views em `dw_outbound` com `updated_at`, e o watermark de cada consumidor em `dw_control.Watermark` (4.2). A logística lê um ciclo curto de 3 minutos da landing (10.3); os demais, o ciclo horário.

### 3.3 Histórico: snapshots SCD2 em T-SQL

- Cada linha de histórico tem validade: `valid_from`, `valid_to` e `is_current`. Uma procedure compara o hash SHA-256 das colunas de negócio da `dw_landing` com a versão vigente, só para as linhas que a carga gravou desde o snapshot anterior (`loaded_at` acima de um watermark próprio). Quando o hash muda, fecha a versão antiga e abre uma nova (exemplo na seção 6).
- A data da mudança vem da origem (`source_updated_at`), e não da hora em que o snapshot rodou.
- Não exige mudança nos módulos de origem.

**Frequência do snapshot (pergunta 9, decidida): a cada hora para notas e pedidos, junto com a carga; diária para cadastros.** O que a escolha significa:

O snapshot guarda a versão que existe no banco quando ele roda. Se uma nota muda duas vezes entre dois snapshots, a versão do meio não aparece. Exemplo: uma nota é emitida às 10h e cancelada às 15h. Com snapshot diário às 23h, o histórico tem só a versão "cancelada". A data do cancelamento não se perde, porque vem da origem. O que se perde é o estado "emitida".

As opções consideradas para notas e pedidos:

- **(a) Diária.** Custo mínimo. Serve para fechamento e consultas de fim de dia. Não responde "quantas notas foram emitidas e canceladas no mesmo dia?".
- **(b) A cada hora, junto com a carga de vendas.** Captura as mudanças do dia. Custa pouco se o snapshot for incremental, como na seção 6: compara só as linhas que a carga acabou de gravar na landing, o volume de uma hora de alterações, e não a tabela inteira. Comparar a tabela inteira a cada hora não seria barato, e o custo cresceria com o histórico. Medir na carga de teste.
- **(c) Histórico gravado pelo próprio ETL,** em cada procedure de sincronização. Captura tudo, mas muda o padrão de todos os módulos.

Decidido: (b) para notas e pedidos, e (a) para cadastros. Como a carga de vendas é horária, o snapshot de notas e pedidos roda junto com ela. Saldos, custos e preços (`TotvsBalances`, `TotvsCosts`, `TotvsPrices`) seguem a opção (a), no fim do dia: eles mudam a cada venda, e uma versão por venda multiplicaria o histórico sem necessidade (7.4, pergunta 10).

### 3.4 Transformação: T-SQL versionado (decidido)

- **Decidido:** a transformação fica em T-SQL, nos scripts `migrator.csx` do módulo. O job chama as procedures em ordem fixa (10.1). Não há dbt.
- **Por que:** o README diz "develop completely with .NET". T-SQL é a linguagem que o repositório já usa para cargas (`sp_*_Sync`) e auditoria (`sp_Log_*`), e não exige runtime novo (Python, ODBC ou container).
- **O que se perde em relação ao dbt:**
  - grafo de dependências (lineage) e documentação gerada automaticamente. A documentação passa a ser escrita neste guia e nos comentários dos scripts;
  - materialização incremental pronta. Cada procedure de carga faz o MERGE por chave, com o watermark;
  - framework de testes. As checagens viram views que devolvem as linhas com erro (9.2). O repositório não tem framework de teste SQL hoje; tSQLt é uma opção, a decidir (arquivo 12).
- **O que exige disciplina:**
  - procedures e views de negócio em scripts numerados, com `CREATE OR ALTER`. O DbUp executa cada script uma vez, então toda alteração entra em script novo (1.3);
  - nomes padronizados: `dw_landing.sp_<Tabela>_Load`, `dw_history.sp_<Tabela>_Snapshot`, `dw_personal_data.sp_<Entidade>_Load`, `datawarehouse.sp_<Tabela>_Load`, `datamart.sp_<Tabela>_Refresh`, `dw_intermediate.sp_<Entidade>_Load`; views `dw_staging.<Tabela>`, `dw_outbound.<Consumidor><Entidade>`, checagens `datawarehouse.vw_Check_<Nome>`;
  - ordem de execução fixa (10.1);
  - a regra de chave substituta e de hash é escrita do mesmo jeito em todas as procedures, como descrito na seção 7.1.

### 3.6 Modelagem: Kimball

- Estrela em `datawarehouse`, agregados em `datamart`. Sem Data Vault: não há fontes em número suficiente, nem exigência de auditoria por linha, que justifiquem a complexidade. Os snapshots já dão a rastreabilidade.
- Reconsiderar Data Vault se o número de fontes dobrar e surgir exigência regulatória de auditoria.
