# DW do NEWBLOOMERS · 02. Arquitetura e fontes

> Guia 2 de 11 do guia do DW, versão 5. Seções deste arquivo: 4 e 5. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 4. Arquitetura alvo

```
Integrações deste repositório (Totvs; Movidesk e AfterSale)
   │  carga bruta em untreated.<tabela>; sp_<tabela>_Sync faz o MERGE
   ▼
totvs.*, movidesk.*, aftersale.*     tabelas confiáveis (integrated_or_updated_at)
   │
   │  procedure de MERGE incremental (watermark, sobreposição de 3 dias), chamada pelo job
   ▼
dw_landing.*     estado que o DW viu, no mesmo banco
   │  procedure de snapshot SCD2 incremental (a cada hora; cadastros, diária; 3.3)
   ▼
dw_history.*  →  dw_staging (views)  →  dw_intermediate.*      tipagem, regras, unificação
   ▼
datawarehouse.*   (dimensões e fatos, estrela)   →   datamart.* (agregados de consumo)

Entram quando tiverem tabelas de dados neste repositório: Linx (só histórico), e-commerce, estoque, logística, Pagarme (1.1).
Transversal: dw_reference.* (tabelas de referência)   dw_control.* (watermarks e checagens)   auditing.* (contrato do Shared)   dw_personal_data.* (carga direta de totvs.*; acesso restrito)
```

### 4.1 Schemas e responsabilidades

Cada schema tem uma função. A tabela diz o que ele guarda, o que não deve guardar, quem escreve e o que acontece se for perdido.

| Schema | Responsabilidade | Não faz | Quem escreve | Se perder |
|---|---|---|---|---|
| `dw_landing` | Espelho do estado atual das tabelas de origem, com o que o DW já ingeriu. Base de todo o DW | Não guarda versões, regras de negócio nem colunas identificadoras de pessoa (5.2, regra 6) | Procedures de MERGE, chamadas pelo job | Recarrega da origem, mas só com o watermark zerado (volta à janela inicial) |
| `dw_history` | Versões das linhas (SCD2), com `valid_from`, `valid_to` e `is_current`. Responde "como estava em tal data" | Não recebe carga direta: só o snapshot escreve. Versiona só o que está na `dw_landing`, então também não tem coluna identificadora de pessoa | Procedures de snapshot | **Não recupera.** A origem guarda só o estado atual. Prioridade no backup |
| `dw_staging` | Views de tipagem e renomeação sobre `dw_landing` e `dw_history`, no vocabulário do DW | Não guarda dados | Views versionadas | Recria pelo script |
| `dw_intermediate` | Unificação de fontes e aplicação de regras de negócio (status, devolução, realizado), antes do modelo estrela | Não é lido por quem consome o DW: só alimenta `datawarehouse` | Procedures e views versionadas | Recria pelo script |
| `datawarehouse` | Modelo estrela: dimensões (SCD1 e SCD2) e fatos com grão definido e chaves substitutas | Não é o schema de consumo das ferramentas de BI, que leem `datamart` | Procedures `sp_*_load` | Recarrega de `dw_landing` e `dw_history`. Sem `dw_history`, as dimensões SCD2 perdem o passado |
| `datamart` | Agregados e colunas de métrica prontas, como `vendas_diarias`. É o schema de consumo: as ferramentas de BI, tratadas em outra etapa, leem só daqui | Não tem regra nova: só agrega o que já foi calculado | Procedures `sp_*_refresh` | Recalcula de `datawarehouse` |
| `dw_reference` | Tabelas de referência mantidas pelo negócio: feriados, regras de canal, mapa loja × ERP × período | Não é calculado. Muda só por script novo, versionado | Scripts de seed (DbUp) | Recria do Git, mas os valores precisam de quem os mantém |
| `dw_control` | Controle das cargas: watermark por fonte e entidade e resultado das checagens | Não guarda dado de negócio | Procedures de carga e de snapshot; o job, nas checagens | Zerar o watermark refaz a carga pela janela inicial (seção 5.2, regra 5) |
| `auditing` | Execuções dos jobs: início, fim, mensagens e erros. Contrato do módulo Shared, compartilhado com as integrações | Não guarda resultado de checagem (vai para `dw_control.check_result`) | Procedures `sp_Log_*` | Perde o histórico de execuções, sem afetar dados. A limpeza já apaga após 15 dias |
| `dw_personal_data` | Dados pessoais em claro (nome, CPF/CNPJ, data de nascimento, e-mail, telefone, endereço), só a identificação atual, com acesso restrito ao grupo `dw_pii` (12.2) | Nenhum relatório lê por padrão. Não guarda versões. `datawarehouse` guarda só a chave, o hash e o documento mascarado | Só `dw_personal_data.sp_cliente_load`, chamada pelo job (10.1), lendo direto das tabelas `totvs` | Recria da origem. As chaves em `datawarehouse` continuam válidas |

**Schemas do banco que não são do DW:**

- `totvs`, `movidesk` e `aftersale` são as tabelas confiáveis dos módulos. O DW só lê deles.
- `untreated` é a área de carga bruta dos módulos, truncada pela manutenção (`sp_Etl_Tables_Maintenance`). O DW não lê dele.
- `general` existe, mas sem tabelas de dados: só o schema criado pelo Shared.
- `linx_*`, `operations` e os demais são legado, sem tabelas criadas neste repositório (seção 1.1).

**Ponto de atenção:** `dw_history` é o único schema com dado de negócio que não se reconstrói a partir da origem. Por isso o backup do banco (PITR) é o que protege o DW. Não há restauração por schema.

---

## 5. Fontes e cargas incrementais

### 5.1 Watermarks

| Fonte (schema.tabela) | Situação | Watermark usado pelo DW | Observação |
|---|---|---|---|
| `totvs.TotvsInvoices`, `totvs.TotvsInvoicesItems`, `totvs.TotvsInvoicesPayments` | Padrão | `integrated_or_updated_at` | Origem: `maxChangeFilterDate` (horário de Brasília). A tabela de nota também tem `lastchangeDate` |
| `totvs.TotvsSaleOrders` | Padrão | `integrated_or_updated_at` | Origem: `maxChangeFilterDate` |
| `totvs.TotvsSaleOrdersProducts` | Padrão | `integrated_or_updated_at` | Origem: `lastChangeDate` (não tem `maxChangeFilterDate`) |
| `totvs.TotvsPeople`, `TotvsLegalEntities`, `TotvsSkus`, `TotvsSellers`, `TotvsBranches` | Padrão | `integrated_or_updated_at` | Origem: `maxChangeFilterDate` (confirmar em cada tabela na carga de teste) |
| `totvs.TotvsPeopleAddresses`, `TotvsPeopleEmails`, `TotvsPeoplePhones`, `TotvsLegalEntitiesAddresses`, `TotvsLegalEntitiesEmails`, `TotvsLegalEntitiesPhones` | Padrão | `integrated_or_updated_at` | Só alimentam `dw_personal_data` (5.2, regra 6). Não têm `dw_landing` nem `dw_history` |
| `movidesk.MovideskTickets`, `MovideskTicketsActions`, `MovideskPeopleEmails` | Padrão | `integrated_or_updated_at` | Origem a confirmar (fase 2) |
| `aftersale.AfterSaleRefunds`, `AfterSaleReverses` | Padrão | `integrated_or_updated_at` | Origem a confirmar (fase 2) |
| Linx Microvix e Linx Commerce (histórico), e-commerce, estoque, logística, Pagarme | Sem tabelas de dados neste repositório | — | Entram quando o módulo criar as tabelas (1.1). Linx: carga única do legado, sem watermark de origem (pergunta 13) |

### 5.2 Regras de carga

1. **Janela de sobreposição de 3 dias.** Cada carga relê os últimos 3 dias do `integrated_or_updated_at`. O valor é o horário em que a fatia foi gravada. No Totvs, `GetBatchTime()` é chamado ao salvar, uma vez por fatia (`TotvsSalesOrdersService.cs:226`). Assim, uma linha que só fica visível para o DW depois de um commit tardio cai na janela da carga seguinte. As cargas são idempotentes (MERGE por chave), então reler não duplica.
2. **Fuso e tipo.** Padrão do repositório: horário de Brasília sem fuso. A `dw_landing` do DW segue esse padrão, e `datetimeoffset` é convertido para Brasília na entrada. Nada fica em UTC. A Totvs compara as datas no horário dela, então o watermark enviado à Totvs fica nesse horário.
3. **Exclusões.** Cabeçalhos não são apagados: a exclusão de nota é estado (regra 4). Para filhas, o merge da origem remove as linhas que saíram de um pai do lote (1.2, item 2). A `dw_landing` só vê isso se comparar com a origem: uma procedure diária faz o anti-join entre as chaves das filhas na `dw_landing` e na tabela confiável. Chave que sumiu recebe `dt_ausente_desde`, e uma checagem alerta se o número passar do limite.
4. **Status, não exclusão.** Nota excluída na Totvs fica com `invoiceStatus = 'Deleted'` (`Script026`), e essa linha continua no histórico.
5. **Carga inicial.** No banco vazio, a Totvs tem janelas próprias, definidas nos services: notas e pedidos desde 01/01/2025 (`TotvsInvoicesService.cs:85` e `TotvsSalesOrdersService.cs:77`), produtos, vendedores e pessoas desde 2000 (`TotvsProductsService.cs:91`, `TotvsSellersService.cs:71` e `TotvsPeopleService.cs:83`). Como o DW começa do zero (pergunta 1), as janelas de notas e pedidos cobrem o fechamento de 2026 e a comparação com 2025, mês a mês. As janelas são por data de alteração: um cadastro entra se foi criado ou alterado desde 2000. Um cadastro criado antes de 2000 e nunca alterado não entra; a checagem de membro desconhecido (seção 11) pega esse caso. Na primeira carga, a carga de produtos usa janelas de 30 dias por matriz (Misha e Open Era), cerca de 650 janelas por empresa. As cargas seguintes voltam para 7 dias.
6. **Dado pessoal identificador.** Nome, CPF/CNPJ, RG, filiação, data de nascimento, e-mail, telefone e endereço não entram em `dw_landing` nem em `dw_history`. A landing de `TotvsPeople` e `TotvsLegalEntities` recebe só a chave, as datas e os atributos não identificadores que o modelo usa (situação, gênero); as tabelas de endereço, e-mail e telefone (`TotvsPeopleAddresses`, `TotvsPeopleEmails`, `TotvsPeoplePhones` e as equivalentes de `TotvsLegalEntities`) não têm landing. Essas colunas vão direto das tabelas `totvs` para `dw_personal_data`, pela mesma regra de watermark e sobreposição, sem versões: só a identificação atual. Na carga de `dim_cliente`, UF, faixa etária, `doc_hash` e `doc_mascarado` são derivados de `dw_personal_data`, com o pepper (8.3 e 12.3). Assim o dado identificado existe no DW num lugar só (12.1).
