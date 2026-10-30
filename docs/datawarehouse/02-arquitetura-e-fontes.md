# DW do NEWBLOOMERS · 02. Arquitetura e fontes

> Guia 2 de 12 do guia do DW, versão 7. Seções deste arquivo: 4 e 5. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 4. Arquitetura alvo

```
Integrações deste repositório (Totvs; Linx Microvix e Linx Commerce, legado; Movidesk e AfterSale)
   │  carga bruta em untreated.<tabela>; sp_<tabela>_Sync faz o MERGE
   ▼
totvs.*, linxmicrovix.*, linxcommerce.*, movidesk.*, aftersale.*     tabelas confiáveis (integrated_or_updated_at)
   │
   │  procedure de MERGE incremental (watermark, sobreposição de 3 dias), chamada pelo job
   ▼
dw_landing.*     estado que o DW viu, no mesmo banco
   │  procedure de snapshot SCD2 incremental (a cada hora; cadastros, saldos, custos e preços, diária; 3.3)
   ▼
dw_history.*  →  dw_intermediate.* (views)      tipagem, regras, unificação dos ERPs
   ▼
datawarehouse.*   (dimensões e fatos, estrela)   →   datamart.* (agregados de consumo)
   │
   ├──▶ dw_outbound.* (views por consumidor) ──▶ Oto, Pagarme, Dootax, Mobsim, logística ──▶ SaaS
   └──▶ ferramentas de BI (outra etapa)

Transversal: dw_reference.* (tabelas de referência)   dw_control.* (watermarks e checagens)   auditing.* (contrato do Shared)   dw_personal_data.* (carga direta das tabelas de origem; acesso restrito)
```

### 4.1 Schemas e responsabilidades

Cada schema tem uma função. A tabela diz o que ele guarda, o que não deve guardar, quem escreve e o que acontece se for perdido.

| Schema | Responsabilidade | Não faz | Quem escreve | Se perder |
|---|---|---|---|---|
| `dw_landing` | Espelho do estado atual das tabelas de origem, com o que o DW já ingeriu. Base de todo o DW | Não guarda versões, regras de negócio nem colunas identificadoras de pessoa (5.2, regra 6) | Procedures de MERGE, chamadas pelo job | Recarrega da origem, mas só com o watermark zerado (volta à janela inicial) |
| `dw_history` | Versões das linhas (SCD2), com `valid_from`, `valid_to` e `is_current`. Responde "como estava em tal data", inclusive para saldos, custos e preços | Não recebe carga direta: só o snapshot escreve. Versiona só o que está na `dw_landing`, então também não tem coluna identificadora de pessoa | Procedures de snapshot | **Não recupera.** A origem guarda só o estado atual. Prioridade no backup |
| `dw_staging` | Reservado. Na fase 1 a tipagem e a renomeação ficaram nas views de `dw_intermediate`, e o schema está vazio | Não guarda dados | — | — |
| `dw_intermediate` | Unificação de fontes (Totvs e Linx no mesmo formato, com `source_erp`), regras de negócio (status, devolução, realizado, canal, tipo e partes do número do pedido), antes do modelo estrela | Não é lido por quem consome o DW: só alimenta `datawarehouse` e `dw_outbound` | Procedures e views versionadas | Recria pelo script |
| `datawarehouse` | Modelo estrela: dimensões (SCD1 e SCD2) e fatos com grão definido e chaves substitutas | Não é o schema de consumo das ferramentas de BI, que leem `datamart` | Procedures `sp_*_Load` | Recarrega de `dw_landing` e `dw_history`. Sem `dw_history`, as dimensões SCD2 perdem o passado |
| `datamart` | Agregados e colunas de métrica prontas, como `DailySales`. É o schema de consumo das ferramentas de BI, tratadas em outra etapa | Não tem regra nova: só agrega o que já foi calculado | Procedures `sp_*_Refresh` | Recalcula de `datawarehouse` |
| `dw_outbound` | Views de consumo dos módulos de saída: o que cada módulo envia a um SaaS, no formato que ele precisa, com `updated_at` para a varredura incremental (4.2) | Não guarda dados nem regra nova: só projeta `dw_intermediate`, `datawarehouse` e, onde o SaaS exige identificação, `dw_personal_data` | Views versionadas | Recria pelo script |
| `dw_reference` | Tabelas de referência mantidas pelo negócio: feriados, regras de canal e de prefixo, exceções do mapa loja × ERP × período, códigos de custo, preço e estoque que alimentam as medidas (pergunta 33) | Não é calculado. Muda só por script novo, versionado | Scripts de seed (DbUp) | Recria do Git, mas os valores precisam de quem os mantém |
| `dw_control` | Controle das cargas: watermark por fonte e entidade (inclusive a varredura de cada consumidor) e resultado das checagens | Não guarda dado de negócio | Procedures de carga e de snapshot; o job, nas checagens; os módulos de saída, no watermark deles | Zerar o watermark refaz a carga pela janela inicial (seção 5.2, regra 5) |
| `auditing` | Execuções dos jobs: início, fim, mensagens e erros. Contrato do módulo Shared, compartilhado com as integrações | Não guarda resultado de checagem (vai para `dw_control.CheckResult`) | Procedures `sp_Log_*` | Perde o histórico de execuções, sem afetar dados. A limpeza já apaga após 15 dias |
| `dw_personal_data` | Dados pessoais em claro (nome, CPF/CNPJ, data de nascimento, e-mail, telefone, endereço), só a identificação atual, com acesso restrito ao grupo `dw_pii` (12.2) | Nenhum relatório lê por padrão. Não guarda versões. `datawarehouse` guarda só a chave, o hash e o documento mascarado | Só `dw_personal_data.sp_Customer_Load`, chamada pelo job (10.1), lendo direto das tabelas de origem | Recria da origem. As chaves em `datawarehouse` continuam válidas |

**Schemas do banco que não são do DW:**

- `totvs`, `linxmicrovix`, `linxcommerce`, `movidesk` e `aftersale` são as tabelas confiáveis dos módulos. O DW só lê deles.
- `untreated` é a área de carga bruta dos módulos, truncada pela manutenção (`sp_Etl_Tables_Maintenance`). O DW não lê dele.
- `general` existe, mas sem tabelas de dados: só o schema criado pelo Shared.
- `linx_*`, `operations` e os demais são legado, sem tabelas criadas neste repositório (seção 1.1). O Oto e a Stone ainda leem `linx_commerce`, fora deste banco, até lerem `dw_outbound`.

**Ponto de atenção:** `dw_history` é o único schema com dado de negócio que não se reconstrói a partir da origem. Por isso o backup do banco (PITR) é o que protege o DW. Não há restauração por schema.

### 4.2 Consumidores do DW: os módulos de saída

Oto, Pagarme, Dootax, Mobsim e os módulos de logística (Jadlog, TotalExpress, Stone e FlashCourier) enviam pedidos, clientes e produtos aos SaaS contratados. Hoje cada um varre o banco de um jeito (`docs/regras-prefixo-pedidos.md`, §2 e §4, mostra a varredura de pedidos do WMS e do Oto, cada uma com a própria regra). A refatoração faz todos lerem do DW, pelo schema `dw_outbound`:

- **Uma view por consumidor e entidade**, em PascalCase inglês: `dw_outbound.OtoOrders`, `OtoCustomers`, `WmsOrders`, `WmsReceipts`, `LogisticsShipments`, `PagarmePayments`, `MarketplaceProducts`. Cada view projeta `dw_intermediate` (pedidos e notas no formato unificado, já com canal, tipo e partes do número) e `datawarehouse` (dimensões), e devolve só as colunas que o SaaS recebe.
- **Varredura incremental por watermark:** toda view expõe `updated_at` (o maior `integrated_or_updated_at` das linhas de origem que a compõem) e o módulo guarda o próprio watermark em `dw_control.Watermark`, com `source = <módulo>` e `entity = <view>`, como o DW faz com as origens, lendo e gravando por `dw_control.sp_Watermark_Get` e `sp_Watermark_Set`. A regra de 3 dias de sobreposição (5.2, regra 1) vale para eles também.
- **Número do pedido:** o DW calcula as partes (`order_series`, `order_type`, `branch_code`, `order_code`, `erp_suffix`) em `dw_intermediate.vw_OrderParts`, uma view sobre a landing que aplica `dw_reference.ChannelRules` (tipo) e `dw_reference.BrandSeries` (série pela raiz do CNPJ); o passo 6 da carga grava o resultado em `FactOrderItem`, pela view `dw_intermediate.vw_OrderItem`, e as views de saída que precisam de frescor leem `vw_OrderParts` direto. Cada view monta o formato que o SaaS espera (`MI-VD12345T` para o WMS, `MI-VD-1-12345` para o Oto). A regra de série e tipo fica num lugar só (7.1), o que resolve as inconsistências 1 a 4 e 8 do levantamento.
- **Dado pessoal:** o Oto recebe nome, e-mail e telefone do cliente, e a logística recebe nome, endereço e telefone do destinatário. `dw_outbound.OtoCustomers` e `dw_outbound.LogisticsShipments` são as únicas views que leem `dw_personal_data`, e as identidades do Oto e dos módulos de logística são os únicos consumidores no grupo `dw_pii` (12.2). As demais views levam só `doc_hash` e `doc_masked`.
- **Frescor (pergunta 35, decidida):** as views mostram o que a `dw_landing` já carregou. A logística precisa dos 3 minutos dos jobs da Totvs; os demais consumidores leem o ciclo horário. Por isso há um **ciclo curto** (10.3): a cada 3 minutos, só a landing de pedidos, notas e clientes e a carga de `dw_personal_data` (pelos endereços de entrega), e as views da logística leem a landing por `dw_intermediate.vw_OrderParts`, uma view que aplica canal, tipo e partes do número sem depender do passo 6 da carga horária.
- **O que não muda:** os módulos continuam escrevendo nos SaaS pelos próprios services e registrando cada chamada nas tabelas `*Requests` (1.2, item 10); o DW não envia nada.

---

## 5. Fontes e cargas incrementais

### 5.1 Watermarks

| Fonte (schema.tabela) | Situação | Watermark usado pelo DW | Observação |
|---|---|---|---|
| `totvs.TotvsInvoices`, `TotvsInvoicesItems`, `TotvsInvoicesItemsProducts`, `TotvsInvoicesItemsTaxes`, `TotvsInvoicesPayments`, `TotvsInvoicesSalesOrders` | Padrão | `integrated_or_updated_at` | Origem: `maxChangeFilterDate` na nota (horário de Brasília); as filhas seguem a nota (`@delete_by_invoice`) |
| `totvs.TotvsSaleOrders` | Padrão | `integrated_or_updated_at` | Origem: `maxChangeFilterDate`. `integrationCode` define o canal (pergunta 15); `representativeName` e `operationName` definem o tipo do pedido (1.2, item 11) |
| `totvs.TotvsSaleOrdersProducts` | Padrão | `integrated_or_updated_at` | Origem: `lastChangeDate` (não tem `maxChangeFilterDate`) |
| `totvs.TotvsPeople`, `TotvsLegalEntities`, `TotvsSkus`, `TotvsSkusColors`, `TotvsSkusColorsProducts`, `TotvsSellers`, `TotvsBranches`, `TotvsCategories`, `TotvsGrids`, `TotvsColors`, `TotvsGridsSizes`, `TotvsSkusReferenceCategories` | Padrão | `integrated_or_updated_at` | Origem: `maxChangeFilterDate` nos pais; as filhas (cores, produtos, grades, categorias da referência) seguem o pai |
| `totvs.TotvsBalances`, `TotvsCosts`, `TotvsPrices` | Padrão | `integrated_or_updated_at` | Origem: `maxChangeFilterDate`. Estado atual por loja, produto e código (`stockCode`, `costCode`, `priceCode`); lidos a cada 3 min (1.2, item 15) |
| `totvs.TotvsPurchaseOrders`, `TotvsPurchaseOrdersProducts`, `TotvsDuplicates`, `TotvsCommissionsPaid`, `TotvsReceptionSales`, `TotvsRepresentatives`, `TotvsOperationalAreas`, `TotvsPriceTables`, `TotvsPriceTablesItems`, `TotvsPromotions` | Padrão | `integrated_or_updated_at` | Fase 4. Origem: `maxChangeFilterDate` (pedidos de compra, duplicatas, representantes, tabelas de preço, promoções); `closingDate` (comissões); `movementDate` (recepção) |
| `totvs.TotvsRequests`, `linxmicrovix.LinxMicrovixRequests`, `linxcommerce.LinxCommerceRequests`, `movidesk.MovideskRequests`, `aftersale.AfterSaleRequests` | Padrão desde 29/10/2026 | `integrated_or_updated_at` | Fase 4, `FactApiRequest`. Origem: `request_date`. Linhas nunca mudam depois de gravadas |
| `totvs.TotvsPeopleAddresses`, `TotvsPeopleEmails`, `TotvsPeoplePhones`, `TotvsLegalEntitiesAddresses`, `TotvsLegalEntitiesEmails`, `TotvsLegalEntitiesPhones` | Padrão | `integrated_or_updated_at` | Só alimentam `dw_personal_data` (5.2, regra 6). Não têm `dw_landing` nem `dw_history` |
| `linxmicrovix.LinxMovimento`, `LinxMovimentoPrincipal`, `LinxMovimentoDevolucoesItens`, `LinxLojas`, `LinxVendedores`, `LinxClientesFornec`, `LinxProdutos`, `LinxB2CPedidos`, `LinxB2CPedidosItens` | Padrão | `integrated_or_updated_at` | Legado (fase 3). Origem: `timestamp` do Microvix (inteiro crescente por linha), com `dt_update` em movimento, produto, cliente e vendedor. A primeira execução traz o histórico inteiro; depois não há linha nova na Newbloomers (1.2, item 7). O DW faz uma carga única e mantém o watermark, caso a origem receba correções |
| `linxcommerce.LinxCommerceOrders`, `LinxCommerceOrdersItems`, `LinxCommerceOrdersInvoices`, `LinxCommerceCustomers`, `LinxCommerceSkus`, `LinxCommerceProducts` | Padrão, sem carga real | `integrated_or_updated_at` | Legado (fase 3). Origem: `ModifiedDate` (pedidos, clientes, produtos) e `UpdatedAt` (notas do pedido). Colunas e chaves a confirmar na primeira resposta real (1.2, item 17) |
| `movidesk.MovideskTickets`, `MovideskTicketsActions`, `MovideskTicketsClients` | Padrão | `integrated_or_updated_at` | Origem: `lastUpdate` (tickets); as filhas, que o merge apaga, têm detecção de exclusão. O documento do cliente sai de `MovideskPeople` direto para `dw_personal_data` (8.3) |
| `aftersale.AfterSaleRefunds`, `AfterSaleRefundsProducts`, `AfterSaleReverses` | Padrão | `integrated_or_updated_at` | Origem: `updated_at` (reembolsos e reversas). O merge não apaga filhas |
| Oto, Pagarme, Dootax, Mobsim, logística | Consumidores, sem tabelas de dados | — | Leem `dw_outbound` (4.2) |

### 5.2 Regras de carga

1. **Janela de sobreposição de 3 dias.** Cada carga relê os últimos 3 dias do `integrated_or_updated_at`. O valor é o horário em que a fatia foi gravada. No Totvs, `GetBatchTime()` é chamado ao salvar, uma vez por fatia (`TotvsSalesOrdersService.cs:226`). Assim, uma linha que só fica visível para o DW depois de um commit tardio cai na janela da carga seguinte. As cargas são idempotentes (MERGE por chave), então reler não duplica. A mesma regra vale para a varredura dos módulos de saída sobre `dw_outbound` (4.2).
2. **Fuso e tipo.** Padrão do repositório: horário de Brasília sem fuso. A `dw_landing` do DW segue esse padrão, e `datetimeoffset` é convertido para Brasília na entrada. Nada fica em UTC. A Totvs compara as datas no horário dela, então o watermark enviado à Totvs fica nesse horário. O `timestamp` do Microvix é um inteiro, não uma data: a `dw_landing` guarda `integrated_or_updated_at` como watermark e o `timestamp` como atributo.
3. **Exclusões.** Cabeçalhos não são apagados: a exclusão de nota é estado (regra 4). Para filhas, o merge da origem remove as linhas que saíram de um pai do lote (1.2, item 2), nos três módulos migrados. A `dw_landing` só vê isso se comparar com a origem: uma procedure diária faz o anti-join entre as chaves das filhas na `dw_landing` e na tabela confiável. Chave que sumiu recebe `missing_since`, e uma checagem alerta se o número passar do limite.
4. **Status, não exclusão.** Nota excluída na Totvs fica com `invoiceStatus = 'Deleted'` (`Script026`), e essa linha continua no histórico. No Linx Microvix, o equivalente é `cancelado = 'S'` em `LinxMovimento` e `estornada` em `LinxMovimentoPrincipal`.
5. **Carga inicial.** No banco vazio, a Totvs tem janelas próprias, definidas nos services: notas e pedidos desde 01/01/2025 (`TotvsInvoicesService.cs:85` e `TotvsSalesOrdersService.cs:77`), produtos, vendedores e pessoas desde 2000 (`TotvsProductsService.cs:91`, `TotvsSellersService.cs:71` e `TotvsPeopleService.cs:83`). Como o DW começa do zero (pergunta 1), as janelas de notas e pedidos cobrem o fechamento de 2026 e a comparação com 2025, mês a mês. As janelas são por data de alteração: um cadastro entra se foi criado ou alterado desde 2000. Um cadastro criado antes de 2000 e nunca alterado não entra; a checagem de membro desconhecido (seção 11) pega esse caso. Na primeira carga, a carga de produtos usa janelas de 30 dias por matriz (Misha e Open Era), cerca de 650 janelas por empresa. As cargas seguintes voltam para 7 dias. No Linx Microvix, a primeira execução de cada método lê desde 01/01/2000 com `timestamp = 0` (README do módulo), então o legado chega inteiro de uma vez.
6. **Dado pessoal identificador.** Nome, CPF/CNPJ, RG, filiação, data de nascimento, e-mail, telefone e endereço não entram em `dw_landing` nem em `dw_history`. A landing de `TotvsPeople` e `TotvsLegalEntities` recebe só a chave, as datas e os atributos não identificadores que o modelo usa (situação, gênero); as tabelas de endereço, e-mail e telefone (`TotvsPeopleAddresses`, `TotvsPeopleEmails`, `TotvsPeoplePhones` e as equivalentes de `TotvsLegalEntities`) não têm landing. As notas e os pedidos da Totvs também trazem o cliente: em `TotvsInvoices` ficam fora `personName`, `personPersonName`, `personPersonCpfCnpj`, `personRgIe`, o endereço (`personAddress`, `personAddressNumber`, `personComplement`, `personPublicPlace`, `personNeighborhood`, `personCep`, `personPostOfficeBox`), `personFoneNumber`, `sellerCpf` e `observationNFE` (texto livre); em `TotvsSaleOrders`, `customerName`, os CPF/CNPJ de cliente, guia, vendedor e representante e o endereço de entrega (`shippingAddressAddress` e as demais colunas de rua, número, complemento, bairro, CEP e caixa postal). Cidade, UF e código IBGE ficam: não identificam a pessoa e servem para a análise por região. O endereço de entrega do pedido, que a logística envia à transportadora (4.2), vai para `dw_personal_data`. A lista exata está no catálogo `dw_control.LandingSource` (9.5). O mesmo vale para `LinxClientesFornec`, `LinxVendedores` e `LinxCommerceCustomers`, e para as colunas de cliente que `LinxCommerceOrders` repete no pedido (`CustomerName`, `CustomerEmail`, `CustomerCPF`, `CustomerCNPJ`, `CustomerPhone`, `CustomerBirthDate`). Essas colunas vão direto das tabelas de origem para `dw_personal_data`, pela mesma regra de watermark e sobreposição, sem versões: só a identificação atual. Na carga de `DimCustomer`, UF, faixa etária, `doc_hash` e `doc_masked` são derivados de `dw_personal_data`, com o pepper (8.3 e 12.3). Assim o dado identificado existe no DW num lugar só (12.1), e o `doc_hash` liga o mesmo cliente entre os ERPs. As únicas saídas do dado em claro são `dw_outbound.OtoCustomers` e `dw_outbound.LogisticsShipments` (4.2).
