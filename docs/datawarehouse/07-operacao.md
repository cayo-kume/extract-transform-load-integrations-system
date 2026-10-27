# DW do NEWBLOOMERS · 07. Ordem de execução e qualidade

> Guia 7 de 11 do guia do DW, versão 5. Seções deste arquivo: 10 e 11. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 10. Ordem de execução das procedures

### 10.1 Sequência de uma carga

A ordem importa porque cada passo lê o que o anterior gravou. O job que dispara os passos, a agenda e o tratamento de falhas ficam fora deste guia; aqui está só o que cada passo exige dos objetos do banco.

| # | Passo | Objetos do banco |
|---|---|---|
| 1 | Lê os watermarks de cada fonte e entidade | `dw_control.watermark` |
| 2 | Atualiza a `dw_landing` por MERGE, das tabelas confiáveis com `integrated_or_updated_at` acima do watermark menos 3 dias (5.2, regra 1), e avança o watermark após o sucesso de cada tabela | `dw_landing.sp_<tabela>_load` |
| 3 | Detecta exclusões de filhas: anti-join entre as chaves da landing e da tabela confiável; chave ausente recebe `dt_ausente_desde` (5.2, regra 3) | procedure de anti-join por tabela filha |
| 4 | Roda os snapshots SCD2, só sobre as linhas que o passo 2 gravou (seção 6). A cada hora para notas e pedidos, diária para cadastros (3.3) | `dw_history.sp_<entidade>_snapshot` |
| 5 | Carrega `dw_personal_data` direto das tabelas `totvs` (5.2, regra 6) | `dw_personal_data.sp_cliente_load` |
| 6 | Carrega dimensões (a de cliente recebe `@pepper`, 8.3), depois fatos, depois agregados | `datawarehouse.sp_<entidade>_load`, `datamart.sp_<agregado>_refresh` |
| 7 | Roda as checagens: conta as linhas de cada view e grava o resultado | `datawarehouse.vw_check_*`, `dw_control.check_result` |
| 8 | Registra início, mensagens e fim da execução | `auditing.sp_Log_Start`, `sp_Log_Message`, `sp_Log_End` (9.4) |

Vendas e pedidos passam por essa sequência a cada hora; cadastros, uma vez por dia. A primeira carga usa as janelas da 5.2, regra 5.

---

## 11. Qualidade e reconciliação

| Verificação | Onde | Quando | Limite |
|---|---|---|---|
| Schema e tipos | Constraints e DDL | Todo script | Bloqueia |
| Unicidade do grão | View de checagem (9.2) | Toda carga | Bloqueia |
| Integridade fato → dimensão | View de checagem, ou FK se a carga aguentar (8.4) | Toda carga | Bloqueia |
| Proporção de membro desconhecido | View de checagem | Toda carga | Alerta acima de 5% |
| Frescor da landing | Watermark comparado à hora atual | Antes da carga | Alerta se atrasar mais de 2 h |
| Exclusões de filhas | View de checagem sobre `dt_ausente_desde` (5.2, regra 3) | Diário | Alerta acima de 0,5% das chaves ativas |
| Total mensal por loja vs fechamento de referência | `datamart.vw_check_reconciliacao_faturado` (9.3) | Diário | Diferença acima de R$ 1,00 por loja e mês |

Os limites são ponto de partida. Ajustar após duas semanas de carga.
