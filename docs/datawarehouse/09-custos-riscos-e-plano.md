# DW do NEWBLOOMERS · 09. Riscos e plano

> Guia 9 de 12 do guia do DW, versão 7. Seções deste arquivo: 15 e 16. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 15. Riscos da modelagem

A mitigação de cada risco já está no texto; as soluções a implementar no futuro estão no arquivo 12.

| # | Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|---|
| R1 | Histórico começa tarde e a versão anterior se perde | Alta | Alto | Snapshots desde a primeira carga, nas tabelas que já estão no padrão. O DW começa do zero (pergunta 1) |
| R2 | Duas versões da mesma linha com o mesmo `source_updated_at` colidem na PK de `dw_history` | Baixa | Alto | Confirmar na carga de teste que `maxChangeFilterDate` muda a cada alteração; se não mudar, incluir `loaded_at` na PK (seção 6) |
| R3 | Cadastros de produto e vendedor incompletos na primeira carga: vendas de 2025 apontam para membros ausentes | Baixa | Alto | Janelas desde 2000, como pessoas (5.2, regra 5); checagem de membro desconhecido (seção 11) |
| R4 | Rateio do item pelos produtos não fecha (`TotvsInvoicesItems` × `TotvsInvoicesItemsProducts`) | Média | Médio | Grão item × produto com rateio proporcional ao líquido (7.2); `vw_Check_ItemProducts` (9.3); item sem produto vai para o membro desconhecido |
| R5 | O mesmo CNPJ soma Linx e Totvs no mesmo dia | Média | Alto | Corte por CNPJ na última venda Linx e primeira nota Totvs; exceções em `dw_reference.StoreErpMap`; `vw_Check_ErpOverlap` bloqueia (7.5, 9.3) |
| R6 | Colunas e chaves do Linx Commerce mudam na primeira resposta real | Alta | Médio | Modelar o Linx Commerce só depois da carga real (1.2, item 17); os scripts do DW para ele ficam para o fim da fase 3 |
| R7 | Histórico de saldos cresce além do banco | Média | Alto | Snapshot diário, não por venda; medir versões por dia na carga de teste antes de definir retenção (7.4, pergunta 14) |
| R8 | Custo, preço ou estoque com o código errado | Média | Alto | `dw_reference.ValueCodes` mapeia cada código a um papel, com o catálogo da PRDFL003 (8.7); as procedures leem o papel, não o código; dois custos com a mesma descrição (`CUSTO ICMS`, 1 e 8) são conferidos na carga de teste; checagem de estoque negativo ou sem custo (seção 11) |
| R9 | De/para de produto Linx casa pouco (referência, cor ou tamanho escritos de outro jeito) | Média | Médio | Normalização de texto antes do casamento; `vw_Check_ProductMatch` por marca; `dw_reference.ProductErpMap` para os casos que o texto não resolve (7.5, pergunta 34) |
| R10 | Pedido sem tipo reconhecido sai sem prefixo para o WMS (inconsistência 8 de `docs/regras-prefixo-pedidos.md`) | Média | Alto | Tipo em `dw_reference.ChannelRules`; `vw_Check_OrderTypeUnknown` alerta e bloqueia a view do WMS acima do limite (9.3, seção 11) |
| R11 | Um consumidor lê o DW atrasado ou com a landing parada e deixa de enviar registros ao SaaS | Média | Alto | `updated_at` nas views, watermark por consumidor com 3 dias de sobreposição, checagem de consumidor atrasado (4.2, seção 11); ciclo curto de 3 minutos para a logística (10.3, pergunta 35) |
| R12 | Ordem errada dos migrators quebra a criação das views | Média | Médio | Ordem fixa documentada (1.1); testar num banco local descartável antes de rodar em produção |
| R13 | Script DbUp editado depois de executado não se aplica onde já rodou | Média | Médio | Alteração sempre em script novo, com `CREATE OR ALTER` ou `ALTER TABLE` (1.3) |
| R14 | Procedures T-SQL sem framework de teste viram código difícil de manter | Média | Médio | Views de checagem (9.2); convenção de nomes (seção 9) |
| R15 | Grão do fato duplicado por chave de origem repetida | Baixa | Alto | A PK do fato é (`source_erp`, `source_key`) (8.4); `vw_Check_FactSalesItemGrain` bloqueia a carga (9.2) |
| R16 | Dado pessoal exposto | Baixa | Alto | Colunas identificadoras só em `dw_personal_data` (5.2, regra 6), inclusive as do Linx; hash com pepper, mascaramento e `GRANT` por schema; só duas views de saída com dado em claro, do Oto e da logística, lidas por identidades no grupo `dw_pii` (seção 12) |
| R17 | Retenção de 15 dias do `auditing` apaga o histórico de execuções | Alta | Médio | Resultados das checagens em `dw_control.CheckResult`, com retenção própria (3.9) |

---

## 16. Plano de construção do banco

1. **(Fase 0, feita em 29/10/2026.)** Criar o módulo `DataWarehouse` com `migrator.csx` sem connection string padrão, e os scripts 001 a 003: schemas, `dw_control.Watermark`, `sp_Watermark_Get`, `sp_Watermark_Set` e `dw_control.CheckResult` (seção 9). Fora da modelagem, mantido a pedido: remover a connection string padrão e a impressão da string dos 42 `migrator.csx` existentes (14 em cada pasta: `Scripts/SQLServer`, `Scripts/MySQL` e `Scripts/PostgreSQL`); no lugar, quando o argumento faltar ou vier incompleto, o script para com uma mensagem dizendo que a connection string é obrigatória e quais partes faltam (servidor, banco, usuário e senha ou autenticação integrada), sem imprimir valores. O `migrator.csx` do DW é o modelo. Na mesma passada, dar a eles o tempo limite de 10 minutos por comando do migrator do DW: com o banco inteiro, a primeira geração de procedure do LinxCommerce passou dos 30 s padrão do DbUp no LocalDB, e só passou na segunda execução.
2. **(Objetos do banco feitos em 29/10/2026; os snapshots passam a rodar quando o job da fase 1 existir.)** Subir o banco pelos migrators na ordem da 1.1 (Shared, Totvs, LinxMicrovix, LinxCommerce, Movidesk, AfterSale, DataWarehouse), primeiro num banco local descartável, depois em produção; os cinco scripts `ALTER` das tabelas `*Requests` (1.2, item 10) rodam junto com os módulos. Criar a landing (scripts 010 e 011) e o history (020) e **ligar os snapshots** nas tabelas Totvs: `TotvsInvoices`, `TotvsSaleOrders`, `TotvsBranches` (`DimStore` é SCD2, 7.1), `TotvsPeople`, `TotvsLegalEntities`, `TotvsSkus`, `TotvsSkusColorsProducts` (`DimProduct` é SCD2) e, no fim do dia, `TotvsBalances`, `TotvsCosts` e `TotvsPrices` (7.4). O histórico começa aqui: é o único passo que não pode ser refeito depois. A landing de saldos só começa depois que a carga inicial dos saldos na Totvs terminar (1.2, item 15).
3. **(Objetos do banco e job feitos em 29/10/2026; falta a carga inicial, a validação dos totais e o primeiro consumidor.)** **Fase 1:** referência e jobs (005, 006), views de intermediate (026; o 025 não foi criado, seção 9), `dw_personal_data` (030), dimensões (031), fatos de venda, pedido e pagamento (040), `datamart.DailySales` (050), as primeiras views de `dw_outbound` (055), views de checagem (060) e seeds (070: `ChannelRules` com as regras de `docs/regras-prefixo-pedidos.md`, `ValueCodes` da pergunta 33). Carga inicial pelas janelas da 5.2, regra 5. Validar os totais com quem usa os números (7.3) e o rateio item × produto (9.3). Migrar o primeiro módulo de saída para ler `dw_outbound` (4.2); se for um módulo de logística, ligar o ciclo curto de 3 minutos (10.3).
4. **Fase 2:** Movidesk e AfterSale (`FactTicket` e `FactRefund`); `dw_history.TotvsBalances`, `TotvsCosts` e `TotvsPrices` (021) e `FactMonthlyStock` (041), com a medição de volume da pergunta 14.
5. **Fase 3:** legado Linx (7.5): landing e history do Microvix (012, 022), membros Linx das dimensões, de/para de produto (`sp_ProductMatch_Load`), `sp_LinxLegacy_Load` e os fatos (042), com as checagens de sobreposição por CNPJ e de produtos casados. Linx Commerce (013) só depois da primeira carga real do módulo.
6. **Fase 4:** demais domínios Totvs e auditoria de envios (7.6): landing (014) e fatos e dimensões (043) de compras, contas a pagar, comissões, recepção de loja, representantes, regiões, promoções e `FactApiRequest`.
7. Os demais módulos de saída passam a ler `dw_outbound` conforme forem migrados para o padrão, até dezembro de 2026; logística e Pagarme ganham fatos próprios se um dia criarem tabelas de dados neste repositório (1.1).
