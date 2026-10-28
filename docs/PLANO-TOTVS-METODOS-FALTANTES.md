# Plano de execução — métodos da documentação oficial que faltam no módulo Totvs

Documento de planejamento. **Nenhum código foi alterado.** Decisões fechadas em 27/10/2026 (§9.2). O plano é montado e executado **um serviço (API) por vez**, na
mesma lógica da fase 2 da [`PROPOSTA-PADRONIZACAO-TOTVS.md`](PROPOSTA-PADRONIZACAO-TOTVS.md): para cada API, o
swagger oficial é comparado com o que o módulo já implementa, e cada rota que falta ganha a sua especificação.

| Ordem | API | Swagger oficial (versão) | Seção | Jobs |
|---|---|---|---|---|
| 1 | Accounts Payable | [`accounts-payable/v2`](https://apitotvsmoda.bhan.com.br/api/totvsmoda/accounts-payable/v2/swagger/index.html) (2.1.09) | §2 | 13018–13020 |
| 2 | Person | [`person/v2`](https://apitotvsmoda.bhan.com.br/api/totvsmoda/person/v2/swagger/index.html) (2.8.26) | §3 | 13030–13069 |
| 3 | Seller | [`seller/v2`](https://apitotvsmoda.bhan.com.br/api/totvsmoda/seller/v2/swagger/index.html) (2.8.02) | §4 | 13070–13079 |
| 4 | Product | [`product/v2`](https://apitotvsmoda.bhan.com.br/api/totvsmoda/product/v2/swagger/index.html) (2.8.31) | §5 | 13080–13179 (preços, custos e saldos: 13015–13017, fase 3) |
| 5 | Purchase Order | [`purchase-order/v2`](https://apitotvsmoda.bhan.com.br/api/totvsmoda/purchase-order/v2/swagger/index.html) (2.8.06) | §6 | 13180–13189 |
| 6 | Sales Order | [`sales-order/v2`](https://apitotvsmoda.bhan.com.br/api/totvsmoda/sales-order/v2/swagger/index.html) (2.8.33) | §7 | 13190–13249 |
| 7 | Fiscal | [`fiscal/v2`](https://apitotvsmoda.bhan.com.br/api/totvsmoda/fiscal/v2/swagger/index.html) (2.8.18) | §8 | 13250–13279 |

Os 7 swaggers oficiais foram baixados em 26/10/2026 e comparados com os usados na fase 2: **nenhuma rota nova ou
removida**. Cada seção foi montada a partir do swagger, **sem chamar a API**; o que é suposição está marcado "a confirmar
com chamada real". A consolidação (prioridades, decisões que valem para todas as APIs e a numeração) está na §9.

Rotas que faltam, por tipo:

| API | Rotas | Implementadas | Fase 3 | Leitura recorrente | Catálogo | Sem persistência | Não implementar | Escrita |
|---|---|---|---|---|---|---|---|---|
| Accounts Payable | 5 | 2 | — | — | — | — | — | 3 |
| Person | 21 | 3 | — | 2 | 4 | 3 | — | 9 |
| Seller | 5 | 1 | — | — | 4 (num job só) | — | — | — |
| Product | 47 | 1 | 3 | 6 (1 de conciliação, 1 alternativa aos saldos) | 12 | 6 | 1 (`products/search`) | 18 |
| Purchase Order | 6 | 1 | — | — | — | — | — | 5 |
| Sales Order | 26 | 1 | — | 3 | 2 | 2 | — | 18 |
| Fiscal | 11 | 1 | — | 1 | 1 | 6 | 1 (`digital-certificates`) | 1 |
| **Total** | **121** | **10** | **3** | **12** | **23** | **17** | **2** | **54** |

A contagem prévia deste documento punha como escrita algumas rotas `POST` que são consultas (`list-balance-bonus`,
`compositions`, `omni-changed-balances`, `danfe-search`, `transaction/print`, `invoice-products/search`); aqui elas estão
como leitura.

---

## 1. Base de escrita (comum a todas as APIs)

O módulo Totvs hoje **só lê**. As três rotas que faltam no Accounts Payable são as primeiras escritas, então a primeira
entrega monta a base que as outras APIs reusam. O desenho é o das escritas do Movidesk
(`docs/IMPLEMENTACAO-MOVIDESK-*-ESCRITA.md`), adaptado ao que a Totvs faz diferente.

### 1.1 Peças novas

| Peça | Onde | O que é |
|---|---|---|
| `Dtos/Writes/<Recurso>/...` | `Domain.Totvs` | Corpo enviado à Totvs, com os **tipos da API** (`long`, `int`, `decimal?`, `DateTime`, enums), uma classe por arquivo. Diferente dos `Dtos/Reads`, que são todos `string`, porque aqui o JSON é montado por nós |
| `Dtos/Writes/Result` | `Domain.Totvs` | Retorno do service para o controller: `success`, `statusCode` da Totvs (0 quando a API nem foi chamada), `message` e `data` (o corpo da Totvs). Igual ao `Result` do Movidesk |
| `Validations/Writes/<Recurso>/...` | `Application.Totvs` | FluentValidation do corpo recebido, **executada no controller** antes de chamar o service: corpo inválido volta **400** sem chamar a API |
| `EnumJob` | `Domain.Totvs` | Um job sob demanda por rota de escrita, a partir de **13018** (13015–13017 ficam reservados para preços, custos e saldos) |
| `Script0NN_Insert_Totvs_<Api>_Writes_Jobs_Into_Jobs_Table.sql` | `Infrastructure.Totvs` | Os jobs de escrita, intervalo 0, um script por API (o próximo número livre é **032**) |
| Rotas no `TotvsController` | host | `POST` com `X-User-Name` obrigatório e `{docCompany}` (`^\d{14}$`), como as leituras sob demanda |

Nenhuma tabela nova: a escrita só grava em `totvs.TotvsRequests` (já existe, `Script028`), com o corpo enviado em
`request_body`.

### 1.2 Fluxo de uma escrita

1. o controller valida o corpo (FluentValidation) e devolve **400** com a lista de erros, sem chamar o service;
2. o service abre o log (`auditing.Logs`) com o job da rota e busca a empresa (`ITotvsService.GetCompany`). Empresa não
   cadastrada: `Result` com `success = false` e `statusCode = 0`, **sem chamar a API** e sem registro em `TotvsRequests`;
3. confere o que só o banco sabe (ex.: o `branchCnpj` do corpo é uma loja da empresa em `totvs.TotvsBranches`). Se não for,
   mesmo retorno do passo 2, com a mensagem;
4. chama a rota com `IApiClient.SendAsync`. **Nova tentativa só em 401** (renova o token) **e 429** (espera o
   `Retry-After`): nos dois casos a Totvs não processou o pedido. **Em 502/503/504 não há nova tentativa**, diferente das
   leituras: a escrita não é idempotente, e um gateway que cai depois de a Totvs gravar faria uma segunda inclusão;
5. grava a chamada em `totvs.TotvsRequests` (usuário, `POST`, rota, CNPJ, chave, `request_body`, resposta e sucesso),
   também quando a chamada lança exceção antes de ter resposta;
6. fecha o log num `finally` e devolve o `Result`.

| Resposta da Totvs | Resposta da rota |
|---|---|
| 200 / 201 | **200** com o `Result` (corpo da Totvs em `data`) |
| 400 (`DomainNotificationMessage[]`: `code`, `message`, `detailedMessage`) | **422** com as mensagens da Totvs |
| 429 depois da nova tentativa | **429** com o `Retry-After` |
| 5xx, exceção ou resposta inválida | **502**, e a mensagem no log (a escrita pode ter sido feita: conferir antes de repetir) |
| Empresa ou loja não cadastrada (passos 2 e 3) | **422** |

### 1.3 Pontos a confirmar antes do primeiro teste

* **Enums no corpo.** O swagger declara os enums como `integer` mas lista só os nomes (`"Duplicate"`, `"Real"`...). Na
  leitura, a Totvs devolve os nomes, mas o `measurementTableOmniType` só foi aceito como número (o nome deu 400, §5.2.4 da
  proposta). Os DTOs de escrita usam um `enum` C# com o **número da descrição do swagger** (ex.: `Duplicate = 1`,
  `FreightBill = 18`) e um conversor que serializa como número; o primeiro teste confirma se a API aceita.
* **Ambiente.** Uma escrita de teste em produção cria uma duplicata de verdade no financeiro, então os testes de escrita
  rodam só em **homologação**. **Confirmado em 27/10/2026:** o ambiente de homologação é o gateway
  `https://api-gateway.t8671ea.external.saas.totvsmoda.cloudtotvs.com.br/api/totvsmoda/`, com um `client_id` próprio
  de homologação; o login (`authorization/v2/token`, `grant_type=password`) funcionou com um usuário do ERP de
  homologação. No módulo, é só esse endereço em `Totvs:BaseUrl` no `appsettings` do ambiente de testes e as credenciais
  de homologação no `Script003` desse banco (fora do git). As URLs `apitotvsmoda.bhan.com.br` e `www30.bhan.com.br:9443`
  servem a mesma API, com o `client_id` de produção. O usuário usado no teste é pessoal e administrador: para os testes
  do módulo, o recomendado é um usuário próprio de integração, com o privilégio "Padrão" (exigido pela rota de token) e
  acesso às lojas de teste.
* **Autorização.** O `[Authorize]` do controller está comentado (decisão 6 da proposta) e **continua comentado**
  (D2/T2, 27/10/2026): a autenticação dos endpoints de inbound vai ser outro mecanismo, ainda a definir. Para leitura
  isso só expõe dados; com escrita, qualquer um que alcance o host pode incluir duplicatas no ERP. Por isso a
  implementação e os testes em homologação seguem, mas **publicar rotas de escrita em produção** espera esse mecanismo.

---

## 2. Accounts Payable

Swagger oficial: `https://apitotvsmoda.bhan.com.br/api/totvsmoda/accounts-payable/v2/swagger/v1/swagger.json`, versão
**2.1.09**, baixado em 26/10/2026. As rotas são as mesmas do swagger usado na fase 2 (nenhuma rota nova ou removida).

| Rota | Situação |
|---|---|
| `POST duplicates/search` | Implementada (`PostSearchDuplicates`, job 13008) |
| `POST comissions-paid/search` | Implementada (`PostSearchComissionsPaid`, job 13009) |
| `POST duplicates` | **Falta** — inclusão de duplicata (§2.1) |
| `POST group-duplicates` | **Falta** — agrupamento de duplicatas (§2.2) |
| `POST duplicates/installments/stage` | **Falta** — alteração do estágio de uma parcela (§2.3) |

Os nomes dos métodos seguem a regra da proposta (decisão 18: o verbo e o caminho da documentação oficial), como
`PostSearchDuplicates` para `POST duplicates/search`. Os três métodos novos ficam no `TotvsAccountsPayableService`.

### 2.1 `PostDuplicates` — `POST accounts-payable/v2/duplicates`

| Item | Especificação |
|---|---|
| Rota do controller | `POST TotvsJobs/Totvs/Duplicates/{docCompany}` (a rota do job continua `POST Duplicates`, sem parâmetro) |
| Corpo | `CreatedDuplicateCommand` (abaixo) |
| Resposta da Totvs | **201** `SuccessProcessingModel` (`processingDate`); 400 `DomainNotificationMessage[]` |
| Job | 13018 `TotvsPostDuplicates` (sob demanda) |
| Chave em `TotvsRequests` | `branchCnpj|supplierCpfCnpj|duplicateCode` |

**DTOs (`Dtos/Writes/AccountsPayable`)**

| Classe | Campos (`*` obrigatório no swagger) |
|---|---|
| `Duplicate` | `branchCnpj*` (14), `supplierCpfCnpj*` (14), `duplicateCode*` (`long`, até 10 dígitos), `installments*` |
| `Installment` | `installmentCode*` (até 3), `bearerCode*` (até 4), `issueDate*`, `dueDate*`, `arrivalDate*`, `document*` (`DocumentType`), `prevision*` (`PrevisionType`), `stage*` (`StageType`), `duplicateValue*`; opcionais: descontos pontual e por adiantamento (valores, percentuais, datas e tipos `AdvanceDiscountType`), multa, mora dia, despesa financeira, acréscimo, juros/outros, data de saída para o fornecedor, indenização, `isAccumulatedDiscount`, `taxValue`, `taxCalculationBasePercentage`; listas `expenses*`, `observations`, `classifications`, `additionalFields`, `taxDuplicates`, `businessUnits`; objetos `billingAccept`, `taxAdditional` |
| `Expense` | `expenseCode*` (até 8), `costCenterCode*` (até 9), `proratedPercentage` ou `proratedValue` |
| `ObservationInstallment` | `observation` (até 80) |
| `BillingAccept` | `barCode`, `assignorCnpj` |
| `DuplicateClassification` | `classificationType` (até 4), `classificationCode` (até 10), descrições |
| `DuplicateField` | `typeFieldCode` (até 4), `descriptionField` (até 80), `numericField`, `dateField` |
| `TaxAdditional` | `revenue` (até 6), `calculationPeriod`, `competenceDate`, `reference` (até 20), `grossRevenueValue`, `isThirteenthSalary`, `otherEntitiesValue` |
| `TaxDuplicate` | `tax` (até 2), `branchCnpj`, `suplierCpfCnpj` (a grafia é a da API), `duplicateCodeTax`, `installmentCodeTax`, `ratePercentage`, `taxValue`, `status` (`StatusTaxDuplicateType`), bases de cálculo, `retentionType` (`RetentionType`), `dirfRevenue` (até 4) |
| `BusinessUnit` | `classificationCode` (até 10), `proratedPercentage` |
| Enums | `DocumentType` (48 valores, 1 = Duplicata ... ), `PrevisionType` (1 Previsão, 2 Real, 3 Consignação), `StageType` (1, 2, 3, 4, 5, 10, 20, 30, 40, 41, 42...), `AdvanceDiscountType` (0 a 7), `StatusTaxDuplicateType` (0 a 2), `RetentionType` (1 a 6) |

**Validações (`DuplicateValidator` e filhos)**

* obrigatórios do swagger; CNPJ e CPF/CNPJ só com dígitos (11 ou 14);
* tamanhos do swagger (códigos pelo número de dígitos; valores com 12 inteiros e 2 decimais; percentuais com 3 inteiros e
  4 ou 6 decimais, conforme o campo);
* pelo menos uma parcela, `installmentCode` sem repetição, e cada parcela com pelo menos uma despesa;
* despesa com **exatamente um** de `proratedPercentage` e `proratedValue` (a descrição do swagger manda isso, embora o
  schema marque o percentual como obrigatório); a soma dos percentuais de rateio da parcela é 100;
* `dueDate` não anterior a `issueDate`;
* enums só com os valores do swagger.

**Service:** fluxo da §1.2. Passo 3: o `branchCnpj` precisa ser uma loja da empresa em `totvs.TotvsBranches`.
Depois da inclusão, o banco local não é atualizado na hora: a duplicata chega pela próxima execução do
`PostSearchDuplicates` (até 3 minutos), como as alterações do Movidesk que não devolvem o registro.

### 2.2 `PostGroupDuplicates` — `POST accounts-payable/v2/group-duplicates`

| Item | Especificação |
|---|---|
| Rota do controller | `POST TotvsJobs/Totvs/GroupDuplicates/{docCompany}` |
| Corpo | `GroupDuplicateCommand`: `branchCnpj*`, `supplierCpfCnpj*`, `duplicateCode*` (a duplicata nova, que agrupa), `document` (`GroupDocumentType`: 1 Duplicata, 18 Conhecimento de frete), `dueDate*`, `groupInstallments*` (lista de `GroupDuplicateModel`: `branchCnpj*`, `supplierCpfCnpj*`, `duplicateCode*`, `installmentCode*`, as parcelas que serão agrupadas) |
| Resposta da Totvs | **201** `GroupedDuplicateResponseModel` (`branchCode`, `supplierCode`, `duplicateCode`, `installmentCode` da duplicata gerada); 400 `DomainNotificationMessage[]` |
| Job | 13019 `TotvsPostGroupDuplicates` (sob demanda) |
| Chave em `TotvsRequests` | `branchCnpj|supplierCpfCnpj|duplicateCode` |

**Validações:** obrigatórios e tamanhos do swagger; pelo menos **duas** parcelas em `groupInstallments` e sem repetição
(agrupar uma parcela só não faz sentido; confirmar no primeiro teste se a API também recusa); `document` só 1 ou 18.

**Service:** fluxo da §1.2, conferindo no passo 3 o `branchCnpj` do corpo e o de cada parcela agrupada. A resposta traz a
chave da duplicata gerada, que vai no `Result` e na mensagem do log. As parcelas agrupadas e a duplicata nova chegam ao
banco pela próxima execução do `PostSearchDuplicates`.

### 2.3 `PostDuplicatesInstallmentsStage` — `POST accounts-payable/v2/duplicates/installments/stage`

| Item | Especificação |
|---|---|
| Rota do controller | `POST TotvsJobs/Totvs/Duplicates/{docCompany}/Installments/Stage` |
| Corpo | `ChangeInstallmentStageCommand`: `branchCnpj*`, `supplierCpfCnpj` **ou** `supplierCode` (exatamente um), `duplicateCode*`, `installmentCode*`, `newStage*` (`ChangeInstallmentStageType`: 1 Título não conferido, 2 Liberado para pagamento, 5 Título aceito) |
| Resposta da Totvs | **200** `SuccessProcessingModel` (`processingDate`); 400 `DomainNotificationMessage[]` |
| Job | 13020 `TotvsPostDuplicatesInstallmentsStage` (sob demanda) |
| Chave em `TotvsRequests` | `branchCnpj|fornecedor|duplicateCode|installmentCode` |

**Validações:** obrigatórios; exatamente um entre `supplierCpfCnpj` e `supplierCode` (a API devolve erro se vierem os
dois); `newStage` só 1, 2 ou 5.

**Service:** fluxo da §1.2. As transições válidas (1 → 2 ou 5; 5 → 1 ou 2; 2 → 1 ou 5) dependem do estágio atual, que o
corpo não traz. A Totvs valida a transição e responde 400; localmente, o service pode conferir antes pelo `stage` de
`totvs.TotvsDuplicates` e devolver 422 sem chamar a API quando a transição é inválida. **Proposta:** conferir só quando a
parcela existe no banco, porque a gravação local pode estar até 3 minutos atrasada (decisão D4, §2.6).

### 2.4 Arquivos

| Projeto | Arquivo | Mudança |
|---|---|---|
| `Domain.Totvs` | `Dtos/Writes/Result.cs` | novo (base de escrita) |
| `Domain.Totvs` | `Dtos/Writes/AccountsPayable/*.cs` | novos: as classes de 2.1, 2.2 e 2.3 e os enums |
| `Domain.Totvs` | `Enums/EnumJob.cs` | 13018, 13019, 13020 |
| `Application.Totvs` | `Validations/Writes/AccountsPayable/*.cs` | novos validadores |
| `Application.Totvs` | `Abstractions/Services/ITotvsAccountsPayableService.cs`, `Services/TotvsAccountsPayableService.cs` | três métodos novos |
| `Application.Totvs` | `Abstractions/Handlers/Commands/ITotvsCommandHandler.cs`, `Handlers/Commands/TotvsCommandHandler.cs` | query de loja por CNPJ e, se aprovada a D4, do estágio atual de uma parcela |
| `Domain.Totvs` / `Infrastructure.Totvs` | `ITotvsRepository`/`TotvsRepository`, `ITotvsAccountsPayableRepository`/`TotvsAccountsPayableRepository` | as consultas acima |
| `Infrastructure.Totvs` | `Scripts/SQLServer/Script032_Insert_Totvs_Accounts_Payable_Writes_Jobs_Into_Jobs_Table.sql` | jobs 13018–13020 |
| `Infrastructure.Totvs` | `DependencyInjection.cs` | nada: os validadores já são registrados pelo assembly |
| Host | `Controllers/Totvs/TotvsController.cs` | três rotas `POST` e o `WriteResponse` (§1.2) |
| Módulo | `README.md`, `Responsed Jsons/` | seção de escrita; exemplos de corpo e de resposta (dados fictícios) |
| `docs/` | `PROPOSTA-PADRONIZACAO-TOTVS.md` | escrita no escopo (§1, §6.4, §7) |

### 2.5 Testes

1. **Sem API:** console com corpos válidos e inválidos para cada validador; conferir que o JSON serializado bate com o
   schema do swagger (nomes, enums como número, datas no formato `yyyy-MM-ddTHH:mm:ss`, nulos omitidos).
2. **Na API de homologação** (§1.3), nesta ordem, com dados criados para o teste:
   * `PostDuplicates` com uma parcela e uma despesa; conferir a duplicata pelo `duplicates/search`;
   * um corpo com erro de regra (ex.: fornecedor inexistente) para ver o formato do 400 e o mapeamento para 422;
   * `PostDuplicatesInstallmentsStage` 1 → 5 → 2, e uma transição inválida (2 → 2);
   * `PostGroupDuplicates` com duas parcelas criadas no primeiro passo;
   * o enum enviado como nome, uma vez, para registrar o que a API faz.
3. **Pelo host:** as três rotas com e sem `X-User-Name`, CNPJ inválido, corpo inválido (400) e empresa não cadastrada
   (422); conferir `totvs.TotvsRequests` e `auditing.*`.

### 2.6 Decisões para aprovar

| # | Pergunta | Proposta |
|---|---|---|
| D1 | Os testes de escrita rodam em qual ambiente? | **Respondida (27/10/2026):** em homologação, no gateway `api-gateway.t8671ea.external.saas.totvsmoda.cloudtotvs.com.br` (§1.3); falta só o usuário de integração próprio |
| D2 | Ligar o `[Authorize]` antes de publicar escrita? | **Respondida (27/10/2026):** **não ligar** agora; a autenticação dos endpoints de inbound vai ser outro mecanismo, a definir (T2) |
| D3 | Depois da escrita, reler a duplicata e gravar na hora, ou esperar o job de 3 min? | Esperar o job: a Totvs não devolve a duplicata no 201, e a releitura custaria uma busca por escrita |
| D4 | Conferir a transição de estágio localmente antes de chamar a API? | Só quando a parcela já está no banco; senão, deixar a Totvs validar |
| D5 | Fluxo comum de envio: um método privado `Send` no service (como `SendTicket` do Movidesk) ou a chamada escrita em cada método (decisão 15 da proposta)? | Escrita em cada método, como nas leituras, para manter a decisão 15 |
| D6 | Enums como número no corpo | **Respondida (27/10/2026):** sim, até o teste mostrar o contrário |

### 2.7 Ordem de execução

1. Base de escrita (§1.1): `Result`, conversor de enum, `WriteResponse` no controller, jobs no `EnumJob` e no `Script032`;
2. `PostDuplicatesInstallmentsStage` (o corpo menor; valida a base de ponta a ponta);
3. `PostDuplicates`;
4. `PostGroupDuplicates` (depende de duplicatas criadas no passo 3 para o teste);
5. README, `Responsed Jsons`, proposta; teste 3 de 2.5 pelo host.

---

## 3. Person

Swagger oficial: `https://apitotvsmoda.bhan.com.br/api/totvsmoda/person/v2/swagger/index.html`, versão **2.8.26** (a mesma
da fase 2; nenhuma rota nova ou removida). 21 rotas: 3 implementadas, 18 faltam.

| Rota | Situação |
|---|---|
| `POST individuals/search` | Implementada (`PostSearchCustomers`, job 13002; sob demanda `GetCustomerByCode`, 13010) |
| `POST legal-entities/search` | Implementada (`PostSearchLegalEntity`, job 13001; sob demanda `GetLegalEntityByCode`, 13011) |
| `GET branchesList` | Implementada (`GetBranchesList`, job 13007) |
| `POST representatives/search` | **Falta** — leitura recorrente (§3.1) |
| `GET reception-sale/search` | **Falta** — leitura recorrente (§3.1) |
| `GET classifications` | **Falta** — catálogo (§3.2) |
| `GET email-types` | **Falta** — catálogo (§3.2) |
| `GET phone-types` | **Falta** — catálogo (§3.2) |
| `GET contact-types` | **Falta** — catálogo (§3.2) |
| `GET branches/{branchId}` | **Falta** — sob demanda, sem persistência (§3.3) |
| `GET person-statistics` | **Falta** — sob demanda, sem persistência (§3.3) |
| `POST list-balance-bonus` | **Falta** — sob demanda, sem persistência (§3.3; é `POST`, mas é consulta) |
| `POST individual-customers` | **Falta** — escrita (§3.4.1) |
| `POST legal-customers` | **Falta** — escrita (§3.4.1) |
| `POST customer-representatives` | **Falta** — escrita (§3.4.2) |
| `POST classification-relationship/remove` | **Falta** — escrita (§3.4.3) |
| `POST messages` | **Falta** — escrita (§3.4.4) |
| `PUT messages/change` | **Falta** — escrita (§3.4.4) |
| `DELETE messages` | **Falta** — escrita (§3.4.4) |
| `POST bonus-due-date` | **Falta** — escrita com efeito financeiro (§3.4.5) |
| `POST bonus-consume` | **Falta** — escrita com efeito financeiro (§3.4.5) |

A contagem prévia (8 leituras, 10 escritas) punha o `list-balance-bonus` nas escritas por ser `POST`; aqui ele é
consulta: **9 leituras e 9 escritas**.

Nomes (decisão 18): o sufixo de ação do caminho vai para a frente, como em `search` (`reception-sale/search` →
`GetSearchReceptionSale`, `classification-relationship/remove` → `PostRemoveClassificationRelationship`,
`messages/change` → `PutChangeMessages`). Onde ficam os métodos: decisão PE9 (§3.7).

Jobs: intervalo **13030–13069**. 13030–13039 leituras, 13040–13049 escritas, o resto reservado. Procedures de merge:
"a numerar" (faixa 13500–13999).

---

### 3.1 Leituras recorrentes

| Método | Rota da API | Rota do controller | Tabelas (chave) | Job | Intervalo | Carga inicial |
|---|---|---|---|---|---|---|
| `PostSearchRepresentatives` | `POST representatives/search` | `POST Representatives` | `TotvsRepresentatives` (`doc_company`, `code`) e 10 filhas | 13030 | 120 min | desde 01/01/2000 |
| `GetSearchReceptionSale` | `GET reception-sale/search` | `GET ReceptionSales` | `TotvsReceptionSales` (`doc_company`, `branchCode`, `movementDate`, `sequence`) e 1 filha | 13031 | 3 min | desde 01/01/2025 (se aprovada, PE2) |

#### 3.1.1 `PostSearchRepresentatives`

**Filtro enviado** (`RepresentativeSearchInDto`):
* `filter.change`: `startDate`/`endDate` (fatias de 30 dias, como PF e PJ) e **todos** os `in*` = `true`
  (`inClassification`, `inAddress`, `inPhone`, `inObservation`, `inRepresentative`, `inCustomer`, `inSalesRegion`,
  `inPriceTable`, `inRepresentativeParameter`); `branchRepresentativeParameterList` vazio = todas as empresas;
* `expand`: `addresses,classifications,emails,additionalFields,phones,observations,pricetables,customers,salesRegions,representativeParameters`
  (o swagger repete `addresses`);
* `option.branchRepresentativeParameterList`: as lojas da empresa (`GetBranchCodes`), senão os parâmetros não vêm;
* `pageSize` 500 (máximo do swagger), `order` `maxChangeFilterDate`.
* Sem filtro de loja obrigatório no swagger (diferente das buscas de documentos).

**Tabelas** (`untreated` + `totvs`, exclusão no merge quando o pai é regravado). **Na implementação (§10.3)** a coluna das
filhas é `personCode` (o representante é uma pessoa do PESFM010), e as seis filhas comuns com a PF reusam os DTOs, entidades e
validadores de `People/Persons` com as mesmas colunas das `TotvsPeople*`:

| Tabela | Chave | Origem |
|---|---|---|
| `TotvsRepresentatives` | `doc_company`, `code` | `code`, `cpfCnpj`, `isInactive`, `name`, `supervisorCode`, `supervisorName`, `branchInsertCode`, `maxChangeFilterDate` |
| `TotvsRepresentativesAddresses` | + `addressTypeCode`, `sequenceCode` | `ViewAddressModel` (igual a `TotvsPeopleAddresses`) |
| `TotvsRepresentativesClassifications` | + `typeCode`, `code` | `ViewClassificationModel` |
| `TotvsRepresentativesEmails` | + `sequence` | `EmailDataModel` |
| `TotvsRepresentativesPhones` | + `sequence` | `PhoneDataModel` (o JSON traz `Sequence` com maiúscula) |
| `TotvsRepresentativesAdditionalFields` | + `code` | `ViewAdditionalFieldModel` |
| `TotvsRepresentativesCustomers` | + `customerCode` | `customers` (`code`, `name`, `cpfCnpj`) |
| `TotvsRepresentativesPriceTables` | + `priceTableCode` | `priceTables` (`code`, `name`) |
| `TotvsRepresentativesObservations` | + `sequence` (anulável: nulo vira a posição) | `ObservationModel` |
| `TotvsRepresentativesSalesRegions` | + `salesRegionCode`, `salesOrderClassificationTypeCode`, `salesOrderClassificationCode` | `RepresentativeSaleRegionModel` (chave a confirmar com chamada real) |
| `TotvsRepresentativesParameters` | + `branchCode`, `parameterCode` | `PersonBranchParameterModel` (`parameterValue` até 2000) |

**Cache:** `Totvs:Representatives:<cnpj>` (código → registro), como `Totvs:Customers`. **Janela:** `MAX(maxChangeFilterDate)`
menos 1 hora.

**Riscos / a confirmar:** limite de período do `change` (não documentado; 30 dias como PF/PJ); o tamanho de `customers`
(um representante pode ter milhares de clientes, e a alteração de um vínculo regrava a lista inteira); se o filtro
`change` sem `representativeCodeList` traz todos os representantes. Proposta extra: `GetRepresentativeByCode` (13039),
igual ao `GetCustomerByCode` (decisão PE10).

#### 3.1.2 `GetSearchReceptionSale`

Recepção de venda (`PESFL023`: entrada do cliente na loja, com vendedor, representante ou guia).

**Parâmetros:** `StartMovementDate`/`EndMovementDate` (data do movimento), `StartRegistrationDate`/`EndRegistrationDate`
(cadastro), `ReceptionTypeList` (`L` loja, `P` pedido; se vai a letra ou o nome — `Store`/`Order` — a confirmar),
`Page`, `PageSize` 1000. Os `ValidationResult.*` da lista de parâmetros são lixo do swagger e não vão.

**Janela:** a resposta **não tem `maxChangeFilterDate` nem a data de cadastro**, então o estado não sai do banco. Proposta:
reler a cada execução o movimento de **D-2 até hoje** (`StartMovementDate`), com hash do JSON de cada recepção para só
regravar o que mudou. Carga inicial desde 01/01/2025, em fatias de 30 dias. **Na implementação (§10.3)**, pela T9, o hash fica
numa coluna `json_hash` da própria tabela, lido do banco para a fatia, e a janela começa 2 dias antes do maior movimento
gravado.

**Tabelas:** `TotvsReceptionSales` (`doc_company`, `branchCode`, `movementDate`, `sequence`; `customerCode`,
`customerName`, `sellerCode`/`Name`, `representativeCode`/`Name`, `guideCode`/`Name`, `receptionType`, `checkInTime`,
`observation`) e `TotvsReceptionSalesDocumentTypes` (+ `documentType`, de `documentTypeList`). Os `phones` do cliente
**não** são gravados: já estão em `TotvsPeoplePhones` pelo `customerCode`.

**Riscos / a confirmar:** a rota não tem filtro de loja (não se sabe se devolve todas as lojas do usuário ou só a do
token); utilidade para o negócio (PE2).

---

### 3.2 Catálogos

Quatro `GET` de domínio pequeno, mesmo desenho: job de **120 min**. **Implementado na fase PE-1 (§10.2)** pela T9: como a
resposta traz o `maxChangeFilterDate`, a janela sai do banco (`MAX(maxChangeFilterDate)` − 1 h; banco vazio, desde 2000),
como nas filiais, em vez da leitura completa com hash proposta aqui. Um registro apagado na Totvs fica no banco. `PageSize` 1000.

| Método | Rota da API | Rota do controller | Tabela (chave) | Colunas | Job |
|---|---|---|---|---|---|
| `GetClassifications` | `GET classifications` | `GET People/Classifications` | `TotvsPersonClassifications` (`doc_company`, `typeCode`, `code`) | `typeDescription` (40), `code` (10), `description` (60), `maxChangeFilterDate` | 13032 |
| `GetEmailTypes` | `GET email-types` | `GET People/EmailTypes` | `TotvsEmailTypes` (`doc_company`, `code`) | `description` (40), `isDefault`, `maxChangeFilterDate` | 13033 |
| `GetPhoneTypes` | `GET phone-types` | `GET People/PhoneTypes` | `TotvsPhoneTypes` (`doc_company`, `code`) | `description` (60), `isDefault`, `maxChangeFilterDate` | 13034 |
| `GetContactTypes` | `GET contact-types` | `GET People/ContactTypes` | `TotvsContactTypes` (`doc_company`, `code`) | `typeName` (60; ver abaixo), `maxChangeFilterDate` | 13035 |

Rotas e tabela de classificação com o prefixo da API (T13): `GET Classifications` e `TotvsClassifications` colidiriam com os
catálogos de classificação de Product e de Sales Order.

Uso: dão nome aos códigos que as escritas de cliente recebem (`phones.typeCode`, `emails.typeCode`, `contacts.typeCode`,
`classifications`) e servem para validar esses códigos no passo 3 da §1.2 (PE6).

**A confirmar:** o `contact-types` declara como resposta o `PhoneTypesResponseModel` (descrições do `PESFL019`, de
telefone); o formato real precisa de uma chamada. O `TypeCodeList` não é enviado (lê tudo).

---

### 3.3 Leituras sob demanda sem persistência

Rotas `GET`/`POST` no controller com `X-User-Name` e `{docCompany}`; o service abre o log, busca a empresa, chama a API,
registra em `totvs.TotvsRequests` e **devolve o corpo da Totvs** (200; 404 quando a Totvs responde 404 ou lista vazia;
422 empresa não cadastrada ou resposta inválida). Nada é gravado em tabela: são consultas do momento.

| Método | Rota da API | Rota do controller | Entrada | Por que sem persistência | Job | Chave em `TotvsRequests` |
|---|---|---|---|---|---|---|
| `GetBranches` | `GET branches/{branchId}` | `GET Branches/{docCompany}/{branchId}` (`branchId`: 1–4 dígitos ou CNPJ de 14) | código ou CNPJ da empresa | `TotvsBranches` já é mantida pelo job de 120 min, e o `BranchOutDto` não tem `description`, `branchGroupName`, `lastchange` nem `maxChangeFilterDate`: regravar apagaria colunas (PE1) | 13036 | `branchId` |
| `GetPersonStatistics` | `GET person-statistics` | `GET PersonStatistics/{docCompany}?customerCode=&customerCpfCnpj=&branchCode=` | `CustomerCode` **ou** `CustomerCpfCnpj` (até 14), `BranchCode` (lista) | totais calculados no momento (atraso médio, parcelas em aberto, maior débito); diferentes dos campos de `TotvsPeopleStatistics`, que vêm por loja no `expand` da PF | 13037 | `customerCode` ou CPF/CNPJ `|` lojas |
| `PostListBalanceBonus` | `POST list-balance-bonus` | `POST ListBalanceBonus/{docCompany}` | `personCpf` (até 14), `branchList[]` (`branchCode` até 4 **ou** `branchCnpj` 14) | saldo de bônus do momento, muda a cada venda | 13038 | `personCpf|lojas` |

**Validações:**
* `GetBranches`: `branchId` só dígitos, 1–4 ou 14.
* `GetPersonStatistics`: exatamente um entre código (> 0) e CPF/CNPJ (11 ou 14 dígitos); o swagger não diz "um ou
  outro" (a confirmar). `BranchCode` vazio: mandar as lojas da empresa (se a API exige, a confirmar).
* `PostListBalanceBonus`: `personCpf` obrigatório (11 ou 14 dígitos: o tamanho 14 sugere CNPJ, a confirmar); cada item
  de `branchList` com um entre código e CNPJ; as lojas precisam estar em `totvs.TotvsBranches` da empresa.

**Riscos / a confirmar (`list-balance-bonus`):** a API valida o parâmetro de empresa `TP_BONUS_DESCONTO` (3 ou 14) e
exige o atributo **`Branch` na requisição do token**. O `TotvsService.GetToken` de hoje não manda loja e guarda um token
por CNPJ (`Totvs:Token:<cnpj>`): é preciso um `GetToken(company, branchCode)` com cache `Totvs:Token:<cnpj>:<loja>` (PE3).
O nome exato do campo no formulário do token é a confirmar. Leitura sem efeito, mas expõe saldo de cliente. **Conferido (§10.13):** o
`list-balance-bonus` responde com o token comum da empresa, e com a loja no formulário do token (`branch`, `Branch`, `branchCode`
ou `BranchCode`) o resultado é o mesmo: o token por loja da PE3 não foi preciso para a leitura (fica a conferir nas escritas da PE-7).

---

### 3.4 Escritas

Todas pela base da §1 (validação no controller, fluxo da §1.2, sem nova tentativa em 5xx, `TotvsRequests` com
`request_body`). Nenhuma tabela nova: o resultado volta ao banco pelas leituras recorrentes (PF/PJ a cada 3 min, com
`change` em todos os `in*`, inclusive `inClassification` e `inRepresentative`). Todos os campos dos DTOs de escrita são
**anuláveis e omitidos quando nulos**, mesmo os que o swagger declara sem `nullable` (ver 3.4.1).

| Método | Rota da API | Rota do controller | Resposta da Totvs | Job | Chave em `TotvsRequests` | Risco |
|---|---|---|---|---|---|---|
| `PostIndividualCustomers` | `POST individual-customers` | `POST IndividualCustomers/{docCompany}` | **200** `CustomerInsertOutDto` (`customerCode`) | 13040 | `cpf` (e o `customerCode` da resposta) | **Alto**: cria ou altera cliente |
| `PostLegalCustomers` | `POST legal-customers` | `POST LegalCustomers/{docCompany}` | **200** `CustomerInsertOutDto` | 13041 | `cnpj` | **Alto**: cria ou altera cliente |
| `PostCustomerRepresentatives` | `POST customer-representatives` | `POST CustomerRepresentatives/{docCompany}` | **201** `SuccessProcessingModel` | 13042 | representante `|` quantidade de clientes | Médio: muda a carteira do representante |
| `PostRemoveClassificationRelationship` | `POST classification-relationship/remove` | `POST ClassificationRelationship/{docCompany}/Remove` | **201** `SuccessProcessingModel` | 13043 | `personCode` | Médio: tira classificação do cliente |
| `PostMessages` | `POST messages` | `POST Messages/{docCompany}` | **201** `MessageResponseModel` (`items[]`: `sequence`, `description`, `priority`) | 13044 | pessoa | Baixo |
| `PutChangeMessages` | `PUT messages/change` | `PUT Messages/{docCompany}/Change` | **200** `SuccessProcessingModel` | 13045 | pessoa `|` `sequence` | Baixo |
| `DeleteMessages` | `DELETE messages` | `DELETE Messages/{docCompany}/{personCode}/{sequence}` | **200** `SuccessProcessingModel` | 13046 | `personCode|sequence` | Médio: exclusão definitiva |
| `PostBonusDueDate` | `POST bonus-due-date` | `POST BonusDueDate/{docCompany}` | **201** `SuccessProcessingModel` | 13047 | `branchCode|personCode|movementDate` | **Alto, financeiro**: gera crédito |
| `PostBonusConsume` | `POST bonus-consume` | `POST BonusConsume/{docCompany}` | **201** `SuccessProcessingModel` | 13048 | `branchCode|pessoa|usedValue` | **Alto, financeiro**: debita saldo |

Erros 400 de todas: `DomainNotificationMessage[]` (`code`, `message`, `detailedMessage`) → 422, como na §1.2.

#### 3.4.1 `PostIndividualCustomers` e `PostLegalCustomers`

**Semântica (descrição do swagger):** a mesma rota cria **ou altera**. Campo ausente = não muda; para **zerar**: string
`""`, número `0`, enum `"NotInformed"` ou `""`. A lista `classifications` só inclui (para tirar, 3.4.3). O corpo não
tem código do cliente: a Totvs deve achar o cadastro pelo `cpf`/`cnpj` (a confirmar).

**DTOs (`Dtos/Writes/People`)**

| Classe | Campos (tamanhos do swagger) |
|---|---|
| `IndividualCustomer` | `branchInsertCode` (4; ver "Estranho"), `insertDate`, `name` (60), `cpf` (11), `rg` (20), `rgFederalAgency` (10), `birthDate`, `gender` (`GenderType`), `isInactive`, `maritalStatus` (`MaritalStatusType`), `motherName`/`fatherName` (60), `nationality` (30), `homeTown` (60), `monthlyIncome` (12,2), `occupation` (60), `hireDate`, `workPlace` (60), `ctps`/`ctpsSerial` (10), `homePage` (60), `employee` (10), `employeeIsInactive`, `registrationStatus` (`Normal`, `InAnalysis`), `isBloqued`, `isFinalCNSR`, `suframaCode` (9), `taxCodeNumber` (20); listas `phones`, `addresses`, `emails`, `references`, `observations`, `additionalFields`, `classifications`, `limits`, `socialNetworks`, `bankAccounts`; objeto `individualAdditionals` |
| `LegalCustomer` | `branchInsertCode` (4), `insertDate`, `cnpj` (14), `name`/`fantasyName` (60), `isInactive`, `uf` (`StateType`), `numberStateRegistration` (18), `dateFoundation`, `codeActivity` (2), `numberEmployees` (`NumberEmployerType`), `monthlyInvoicing`/`shareCapital` (12,2), `homePage` (100), `registrationStatus`, `codeActivityCnae`/`codeActivityCnae2` (7), `typeTaxRegime` (`TaxRegimeType`), `typeSubTributary` (`DescriptionSubTributaryType`), `suframaCode` (9), `registrationMunicipal` (18), `descriptionJuntaCial` (20), `dateRegJuntaCial`, `foreignIdentification` (20), `initialDateActivity`/`finalDateActivity`, `isCommercialExp`, `isFinalCNSR`; listas `phones`, `addresses`, `emails`, `partners`, `observations`, `additionalFields`, `classifications`, `references`, `socialNetworks`, `contacts`, `limits`; objeto `preferences` |
| `PhoneIn` | `typeCode` (2), `number` (20), `branchLine` (4), `isDefault` |
| `AddressIn` | `addressType` (`AddressType`), `sequence` (3), `postOfficeBox` (9), `cep` (8), `cityName` (60), `stateAbbreviation` (3), `publicPlace` (10), `address` (60), `number` (5), `reference` (60), `complement` (65), `neighborhood` (60) |
| `EmailIn` | `typeCode` (2), `email` (60), `isDefault` |
| `ReferenceIn` | `sequence` (3), `type` (`ReferenceType`), `description` (20), `phoneNumber` (20), `responsiblePersonName` (60), `isInactive` |
| `ObservationIn` | `observation` (80), `sequence` (9) |
| `AdditionalFieldIn` | `code` (9), `value` (texto até 80, número 12,6 ou data `DD/MM/YYYY`: `object` serializado conforme o tipo) |
| `ClassificationIn` | `classificationTypeCode` (4), `classificationCode` (10) |
| `LimitIn` | `branchCode` (4), `limitValue`, `saleLimitValue`, `monthlyLimitValue` (14,2) |
| `SocialNetworkIn` | `sequence` (3; só na alteração), `typeCode` (2), `address` (20) |
| `IndividualAdditional` (PF) | os 50 campos do `IndividualAdditionalInDto` (escolaridade, filhos, residência, CNH, título, militar, PIS, RIC, passaporte, `socialName` 70...), com os enums `EducationLevelType`, `HomeType`, `CarType`, `DriverLicenseCategoryType`, `MilitaryCertificateType`, `MilitaryCategoryType` |
| `BankAccountIn` (PF) | `holderName` (40), `additionalData` (80), `bankNumber` (5), `branchNumber` (4), `accountNumber` (20), `personType`, `accountDigit` (2), `accountType`, `purposeType`, `compensationType` (3), `checkType`, `pixKeyType`, `pixKey` (100) |
| `PartnerIn` (PJ) | `cpfPartner` (14), `partnerCode` (9), `percentageParticipation` (3,2) |
| `ContactIn` (PJ) | `sequence` (3; só na alteração), `typeCode` (2), `name` (60), `function` (20), `phoneNumber`/`cellNumber` (20), `email` (60), `birthDate`, `isDefault` |
| `Preferences` (PJ) | `paymentConditionCode` (4), `shippingCompanyCode` (9), `freightType` (`FreightType`), `bridgeShippingCompanyCode` (9), `maximumAverageTerm` (4), `priceTableCode` (9), `orderPriority` (2), `bearerCode` (4) |

**Validações (`IndividualCustomerValidator`, `LegalCustomerValidator` e filhos):**
* `branchInsertCode` obrigatório (o `branchInsertCnpj` citado na descrição não existe no schema) e loja da empresa
  (passo 3 da §1.2);
* `cpf` (11) / `cnpj` (14) obrigatório, só dígitos e com dígito verificador (é a chave do cadastro);
* na **inclusão** (o service não sabe se é inclusão; proposta: exigir sempre) `name`; datas no formato `yyyy-MM-dd`;
* tamanhos do swagger; enums só com os valores do swagger;
* no máximo um `isDefault = true` em `phones`, `emails` e `contacts`;
* `percentageParticipation` dos sócios até 100 e soma até 100;
* `additionalFields.value` coerente com o tipo (texto 80, número 12,6, data `DD/MM/YYYY`);
* `typeCode` de telefone, e-mail e contato e `classificationTypeCode`/`classificationCode` conferidos nos catálogos de
  §3.2 **quando** o catálogo está gravado (PE6).

**Service:** fluxo da §1.2. A resposta traz o `customerCode`, que vai no `Result`, na mensagem do log e em
`field_key_value`. O registro chega às tabelas pelo próximo `PostSearchCustomers`/`PostSearchLegalEntity` (até 3 min);
o chamador pode forçar com `GET Customers/{docCompany}/{code}` (PE11).

**Riscos:**
* **Altera cliente real**, inclusive o que está em uso nas lojas: `isInactive`, `isBloqued` e `registrationStatus =
  InAnalysis` bloqueiam a venda; `limits` muda o **limite de crédito** (efeito financeiro).
* **Zerar sem querer:** campos que o swagger declara sem `nullable` (`insertDate`, `isCommercialExp` da PJ,
  `references.sequence`, `socialNetworks.sequence`/`typeCode`, `partners.partnerCode`, `limits.*`,
  `additionalFields.code`) seriam serializados com o valor padrão do C# (`0001-01-01`, `false`, `0`) e, numa
  alteração, `0`/`false` **zera** o campo. Por isso todos ficam anuláveis e com nulos omitidos; o primeiro teste confere
  o JSON.
* **Enums:** a própria descrição manda `"NotInformed"`, ou seja, **pelo nome**, ao contrário da proposta da §1.3 (número).
  Testar o nome primeiro nesta rota (PE6).
* **Dados pessoais (LGPD):** o `request_body` em `TotvsRequests` guardaria CPF, RG, CNH, passaporte, conta bancária e
  chave PIX (PE7).
* Sem rota de exclusão de cliente: um cadastro criado por engano só pode ser inativado.

#### 3.4.2 `PostCustomerRepresentatives`

**Corpo** (`InsertRelateCustomerRepresentativeCommand` → `CustomerRepresentative`): `representativeCode` (9) **ou**
`representativeCpfCnpj` (14), exatamente um; `customerCodeList` **ou** `customerCpfCnpjList`, exatamente uma, não vazia.

**Validações:** os dois "um ou outro" da descrição (o schema não marca nada como obrigatório; `representativeCode` é
`int` sem `nullable`, então vai anulável e omitido); códigos > 0; CPF/CNPJ com 11 ou 14 dígitos; sem repetição; tamanho
máximo da lista não documentado (proposta: 500, a confirmar).

**Riscos:** muda a carteira do representante, que define comissão e roteamento de pedidos; **a API não tem rota para
desfazer** o vínculo. Volta ao banco por `TotvsPeopleRepresentatives`/`TotvsLegalEntitiesRepresentatives` (job de 3
min) e por `TotvsRepresentativesCustomers` (§3.1.1, 120 min).

#### 3.4.3 `PostRemoveClassificationRelationship`

**Corpo** (`RemoveClassificationRelationshipCommand` → `RemoveClassificationRelationship`): `personCode*` (9),
`classifications[]` (`classificationTypeCode*` até 4, `classificationCode` até 10).

**Validações:** `personCode` > 0; pelo menos uma classificação (o schema aceita lista nula; remover "nada" não faz
sentido); sem repetição.

**Riscos:** classificações segmentam clientes (campanhas, preço, regras de loja). Se `classificationCode` nulo remove
**todas** as classificações do tipo é a confirmar em homologação; proposta: exigir o código até confirmar. É o par da
inclusão feita por 3.4.1 (que só inclui).

#### 3.4.4 `PostMessages`, `PutChangeMessages`, `DeleteMessages`

Mensagens da pessoa (`PESFM152`), exibidas ao atender o cliente.

| Método | Corpo / parâmetros | Validações |
|---|---|---|
| `PostMessages` | `personCode` (9) **ou** `personCpfCnpj` (14); `messages[]`: `description` (200), `priority` (`int`, cadastro `PESFL151`) | um ou outro; pelo menos uma mensagem; `description` obrigatória e até 200; `priority` > 0 |
| `PutChangeMessages` | `filter`: `personCode` **ou** `personCpfCnpj`, `sequence*`; `description`, `status` (`MessageStatusType`: 1 Não lida, 2 Lida, 3 Cancelada), `priority` | um ou outro; `sequence` > 0; pelo menos um entre `description`, `status`, `priority`; `description` até 200 (o schema não diz o tamanho; usar o do `POST`) |
| `DeleteMessages` | query `PersonCode*`, `Sequence*`, `LastChangeUser` (sem descrição) | `personCode` e `sequence` > 0 (a descrição cita `personCpfCnpj`, que não existe como parâmetro) |

**Service:** fluxo da §1.2. O `POST` devolve a `sequence` de cada mensagem, que vai no `Result` e no `response_body`.

**Persistência:** nenhuma. **Não há rota de leitura de mensagens** nesta API nem `expand` de mensagens nas buscas de
PF/PJ: a única fonte das `sequence` para alterar ou excluir depois é a resposta do `POST` guardada em `TotvsRequests`
(PE8).

**Riscos:** baixo no `POST`/`PUT` (texto visto pela loja; pode expor conteúdo indevido ao cliente). `DELETE` é
**definitivo** e sem leitura para conferir antes; preferir `PUT` com `status = Canceled` (PE8). `LastChangeUser` é um
código de usuário da Totvs não documentado: não enviar até confirmar.

#### 3.4.5 `PostBonusDueDate` e `PostBonusConsume` (efeito financeiro)

Bônus desconto do cliente (`PESFC054`): crédito com validade que o cliente usa como desconto ou forma de pagamento.

| Método | Corpo (`*` obrigatório no swagger) |
|---|---|
| `PostBonusDueDate` (`BonusDueDateCommand` → `BonusDueDate`) | `branchCode*` (4), `personCode*` (9), `bonusType*` (2; 1–15, lista da descrição), `movementDate*`, `bonusValue*` (14,2), `usedValue` (14,2), `startDate*`, `endDate*`; origem opcional: `transactionBranchCode` (4), `transactionDate`, `transactionNumber` (9), `invoiceBranchCode` (4), `invoiceClientCode` (9), `invoiceNumber` (10), `installmentNumber` (3), `settlementBranchCode` (4), `settlementDate`, `settlementSequenceNumber` (9), `historic` |
| `PostBonusConsume` (`UpdateBonusConsumptionCommand` → `BonusConsumption`) | `personCode*` (9) **ou** `personCpfCnpj` (14), `branchCode*`, `usedValue*` (9,2) |

**Validações:**
* `bonusValue` > 0 e `usedValue` ≥ 0 e ≤ `bonusValue`; `startDate` ≤ `endDate`; `endDate` não anterior a hoje;
  `bonusType` 1–15; grupos de origem completos (transação: empresa + data + número; fatura: empresa + cliente + número
  + parcela; liquidação: empresa + data + sequência);
* `PostBonusConsume`: `usedValue` > 0 e até 9 inteiros; exatamente um entre `personCode` e `personCpfCnpj` (o schema
  marca `personCode` obrigatório, a descrição diz "um ou outro": vale a descrição, a confirmar);
* `branchCode` loja da empresa (passo 3 da §1.2).

**Service:** fluxo da §1.2, com o **token da loja** (`Branch` na requisição do token; PE3). `PostBonusConsume`: antes
de chamar, consulta o saldo pelo `list-balance-bonus` e devolve 422 sem chamar a API se o saldo da loja for menor que
`usedValue` (PE4).

**Riscos:**
* `PostBonusDueDate` **dá dinheiro ao cliente** (crédito de desconto); `PostBonusConsume` **tira** saldo do cliente.
* **Nenhuma das duas tem estorno** nesta API: um consumo ou crédito errado só se desfaz no ERP.
* **Não idempotentes:** repetir depois de um 502 duplica o crédito ou o débito. A regra da §1.2 (sem nova tentativa em
  5xx) é essencial aqui, e o 502 da rota precisa dizer "conferir o saldo antes de repetir".
* Dependem do parâmetro `TP_BONUS_DESCONTO` da loja (14 para consumir; para o vencimento não está documentado) e do
  token por loja.
* `X-User-Name` não é autenticação: com o `[Authorize]` comentado, qualquer um que alcance o host gera ou consome
  bônus (PE5).

---

### 3.5 Arquivos

| Projeto | Arquivo | Mudança |
|---|---|---|
| `Domain.Totvs` | `Dtos/Reads/People/Representatives/*.cs`, `Dtos/Reads/People/ReceptionSales/*.cs`, `Dtos/Reads/People/Catalogs/*.cs`, `Dtos/Reads/People/Branches/BranchById.cs`, `Dtos/Reads/People/Statistics/PersonStatistics.cs`, `Dtos/Reads/People/Bonus/BalanceBonus.cs` | novos (todos `string`) |
| `Domain.Totvs` | `Entities/Reads/People/Representatives/*.cs`, `.../ReceptionSales/*.cs`, `.../Catalogs/*.cs` | novas entidades |
| `Domain.Totvs` | `Dtos/Writes/People/*.cs` (clientes PF/PJ e filhas, `CustomerRepresentative`, `RemoveClassificationRelationship`, mensagens, bônus, `ListBalanceBonus` e os enums) | novos |
| `Domain.Totvs` | `Enums/EnumJob.cs` | 13030–13038 (e 13039, se PE10), 13040–13048 |
| `Application.Totvs` | `Validations/Reads/People/...` (novos DTOs de leitura), `Validations/Writes/People/*.cs` | novos validadores |
| `Application.Totvs` | `Abstractions/Services/*`, `Services/*` | métodos novos, nos services da PE9 |
| `Application.Totvs` | `Services/TotvsService.cs`, `Abstractions/Services/ITotvsService.cs` | `GetToken(company, branchCode)` com cache por loja (PE3) |
| `Application.Totvs` | `Abstractions/Handlers/Commands/ITotvsCommandHandler.cs`, `Handlers/Commands/TotvsCommandHandler.cs` | queries de última alteração e cache dos representantes, recepções e catálogos |
| `Domain.Totvs` / `Infrastructure.Totvs` | `ITotvsPeopleRepository`/`TotvsPeopleRepository` (ou um repositório por service novo) | bulk insert e merges das tabelas novas |
| `Infrastructure.Totvs` | `Scripts/SQLServer/Script0NN_Create_Totvs_Representatives_Tables.sql`, `Script0NN_Create_Totvs_Representatives_Sync_Procedures.sql`, `Script0NN_Insert_Totvs_Representatives_Jobs_Into_Jobs_Table.sql` | 11 tabelas, procedures (a numerar), job 13030 |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Reception_Sales_Tables.sql`, `..._Sync_Procedures.sql`, `..._Jobs_Into_Jobs_Table.sql` | 2 tabelas, job 13031 (PE2) |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Person_Catalogs_Tables.sql`, `..._Sync_Procedures.sql`, `..._Jobs_Into_Jobs_Table.sql` | 4 tabelas, jobs 13032–13035 |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_On_Demand_Person_Jobs_Into_Jobs_Table.sql` | 13036–13038 (13039) |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_Person_Writes_Jobs_Into_Jobs_Table.sql` | 13040–13048, intervalo 0 |
| `Infrastructure.Totvs` | `DependencyInjection.cs` | os services novos (PE9) |
| Host | `Controllers/Totvs/TotvsController.cs` | 6 rotas de leitura recorrente/catálogo, 3 sob demanda, 9 de escrita |
| Módulo | `README.md`, `Responsed Jsons/` | tabelas, jobs, rotas; exemplos (dados fictícios) de representantes, recepção, catálogos, estatísticas, saldo e das escritas |

### 3.6 Testes

1. **Sem API:** validadores com corpos válidos e inválidos; o JSON de cada escrita conferido com o schema (nomes, nulos
   **omitidos**, nenhum `0`/`false`/`0001-01-01` sem querer, datas `yyyy-MM-dd` onde a descrição pede).
2. **Leituras na API** (sem efeito, podem rodar em produção):
   * os quatro catálogos, conferindo o formato real do `contact-types` e se `maxChangeFilterDate` vem com as datas;
   * `representatives/search` com uma janela pequena e o `expand` completo: chaves das filhas, tamanho de `customers`,
     limite do período do `change`;
   * `reception-sale/search` de um dia: lojas devolvidas, formato do `ReceptionTypeList` e do `movementDate`;
   * `branches/{branchId}` por código e por CNPJ; `person-statistics` por código e por CPF;
   * `list-balance-bonus` com e sem `Branch` no token, para ver a mensagem de erro.
3. **Escritas, só em homologação** (§1.3, D1 da §2.6), com uma pessoa de teste criada no primeiro passo:
   * `PostIndividualCustomers` inclusão mínima; depois alteração de um campo só, conferindo que os outros não mudaram
     (pelo `individuals/search`); um reset com `""`; um enum pelo nome e pelo número;
   * `PostLegalCustomers` idem, conferindo `isCommercialExp` omitido;
   * `PostMessages` → `PutChangeMessages` (status `Read`, depois `Canceled`) → `DeleteMessages`;
   * `PostCustomerRepresentatives` com um cliente; `PostRemoveClassificationRelationship` de uma classificação incluída
     pela PF, e uma vez com `classificationCode` nulo;
   * `PostBonusDueDate` de valor pequeno; `PostListBalanceBonus`; `PostBonusConsume` parcial e um acima do saldo
     (deve dar 422 local e, sem a pré-checagem, ver o 400 da Totvs).
4. **Pelo host:** rotas com e sem `X-User-Name`, CNPJ inválido, corpo inválido (400), empresa não cadastrada (422);
   conferir `totvs.TotvsRequests` (inclusive o mascaramento da PE7) e `auditing.*`.

### 3.7 Decisões para aprovar

| # | Pergunta | Proposta |
|---|---|---|
| PE1 | `GetBranches` grava em `TotvsBranches`? | Não: devolve o corpo. O `BranchOutDto` não tem `description`, `branchGroupName` nem datas, e o job de 120 min já mantém a tabela. Para regravar uma loja na hora, um sob demanda pelo `branchesList` com `BranchCodeList` (fora desta lista) |
| PE2 | Implementar a recepção de venda? Com que janela? | **Respondida (27/10/2026):** implementar mesmo sem uso pela Newbloomers (T14). Janela proposta: reler D-2 até hoje a cada 3 min com hash, sem os telefones |
| PE3 | Token por loja para os bônus (`Branch` no token) | `GetToken(company, branchCode)` com cache `Totvs:Token:<cnpj>:<loja>`; o `GetToken(company)` atual fica como está |
| PE4 | Conferir o saldo antes do `PostBonusConsume`? | Sim, pelo `list-balance-bonus`; a Totvs continua sendo a palavra final |
| PE5 | Publicar as rotas de bônus junto com as outras? | Não: só depois da autenticação dos endpoints de inbound (T2, a definir) e atrás de uma chave `Totvs:EnableBonusWrites` (falsa por padrão) |
| PE6 | Escrita de cliente: enum pelo nome ou número? Validar códigos nos catálogos? | Nome nesta rota (a descrição usa `"NotInformed"`), confirmado no teste 3; validar nos catálogos só quando gravados |
| PE7 | `request_body` de clientes com CPF, RG, documentos, conta e PIX em `TotvsRequests` | Gravar, mas mascarando `bankAccounts` (conta, dígito, `pixKey`) e os documentos de `individualAdditionals` |
| PE8 | Mensagens: sem rota de leitura, guardar as `sequence`? `DELETE` ou cancelar? | Sem tabela: as `sequence` ficam na resposta em `TotvsRequests` (busca por `field_key_value`); expor o `DELETE`, mas documentar `PUT` com `Canceled` como o caminho normal |
| PE9 | Onde ficam os 18 métodos? (`TotvsPeopleService` já passa de 1000 linhas) | `TotvsRepresentativesService` (representantes, recepção, vínculo cliente-representante), `TotvsPersonCatalogsService` (4 catálogos), `TotvsBonusService` (3 rotas de bônus, isola o efeito financeiro e o token por loja); `GetBranches`, `GetPersonStatistics` e as escritas de cliente, classificação e mensagem no `TotvsPeopleService` |
| PE10 | Leitura sob demanda de representante por código (`GetRepresentativeByCode`, 13039)? | Sim, mesmo padrão do `GetCustomerByCode` (`representativeCodeList`, `pageSize` 1) |
| PE11 | Depois de criar/alterar cliente, reler na hora? | Não (como a D3 da §2.6): o job de 3 min traz; a resposta devolve o `customerCode` para o chamador usar o `GET Customers/{docCompany}/{code}` se precisar |

### 3.8 Ordem de execução

1. Catálogos (13032–13035): leitura, tabelas pequenas, servem às validações das escritas;
2. `PostSearchRepresentatives` (13030) e, se aprovado, `GetRepresentativeByCode` (13039);
3. Sem persistência: `GetBranches`, `GetPersonStatistics`; depois o token por loja (PE3) e `PostListBalanceBonus`;
4. `GetSearchReceptionSale` (13031, PE2);
5. Escritas de baixo risco (depois da base da §1): `PostMessages` → `PutChangeMessages` → `DeleteMessages`;
6. `PostRemoveClassificationRelationship`, `PostCustomerRepresentatives`;
7. `PostIndividualCustomers`, `PostLegalCustomers` (os corpos maiores; dependem do teste de serialização e de enum);
8. Bônus por último (a publicação em produção espera a autenticação de inbound, T2): `PostBonusDueDate` (cria o saldo do teste) e `PostBonusConsume`;
9. README, `Responsed Jsons`, proposta; teste 4 de 3.6 pelo host.

### 3.9 Estranho no swagger

* `branchInsertCode` (PF e PJ) "obrigatório caso o campo `branchInsertCnpj` não seja informado", mas `branchInsertCnpj`
  não existe no schema.
* `bonus-consume`: `personCode` marcado obrigatório e a descrição diz "um ou outro" com `personCpfCnpj`.
* `DELETE messages`: a descrição de `PersonCode` cita `personCpfCnpj`, que não é parâmetro; `Sequence` e
  `LastChangeUser` sem descrição.
* `contact-types` devolve o `PhoneTypesResponseModel` (descrições de telefone, `PESFL019`) e o `TypeCodeList` tem
  "Tamanho máximo: 2 caracteres"; o `TypeCodeList` do `phone-types` fala em "tipo de e-mails".
* Representantes: `expand` lista `addresses` duas vezes; `option.branchStaticDataList` fala de um `expand`
  `statistics` que o representante não tem; a descrição de `maxChangeFilterDate` é "Código da empresa".
* Escrita de cliente: o reset de enum por `"NotInformed"` indica enum **pelo nome** no corpo (contra o número da §1.3);
  `insertDate` e `isCommercialExp` sem `nullable` ("Tamanho máximo: 1 caractere" num booleano); `publicPlace` e
  `address` descritos os dois como "Logradouro"; `stateAbbreviation` até 3 na entrada e 2 na saída; `classifications`
  apontando para o `PESFM059` (é o `PESFM022`).
* `bonusType` 2 e 3 têm a mesma descrição; `list-balance-bonus` aceita `personCpf` de 14 caracteres.
* `list-balance-bonus` e `bonus-consume` exigem o atributo `Branch` no token, que nenhuma outra rota pede e o
  `authorization/v2/token` usado hoje não manda.
* `reception-sale/search` é `GET` e não tem filtro de loja; as rotas `GET` trazem `ValidationResult.*` como parâmetros
  de query (lixo do modelo interno).
* Clientes respondem **200** na criação; as demais escritas, **201**.

---

## 4. Seller

Swagger oficial: `https://apitotvsmoda.bhan.com.br/api/totvsmoda/seller/v2/swagger/index.html`, versão **2.8.02** (a mesma
da fase 2; nenhuma rota nova ou removida).

| Rota | Situação |
|---|---|
| `POST search` | Implementada (`PostSearchSellers`, job 13006) |
| `GET operational-area` | **Falta** — regiões de atuação (§4.1) |
| `GET operational-area-state` | **Falta** — estados de cada região (§4.1) |
| `GET operational-area-cep` | **Falta** — CEPs de cada região (§4.1) |
| `GET operational-area-city` | **Falta** — cidades de cada região (§4.1) |

### 4.1 Regiões de atuação — tipo: **catálogo** (120 min)

As quatro rotas são um mesmo cadastro (componentes PESFL035 e PESFM006): a região (`operational-area`) e a sua abrangência
geográfica por estado, CEP e cidade. **Nenhuma delas liga a região a um vendedor**: as respostas não têm `sellerCode`, e o
`SellerDto` do `POST search` também não tem região. O "de vendedores" do título é só o nome do cadastro no ERP.

| Rota | Método | Parâmetros usados | Item da resposta | Chave natural |
|---|---|---|---|---|
| `GET operational-area` | `GetOperationalArea` (lê as 4 rotas, abaixo) | `Page`, `PageSize` (máx. 1000) | `OperationalAreaModel`: `operationalAreaCode`, `operationalAreaName`, `operationalAreaType` (`Representative`, `ShippingCompany`, `DeliveryDate`, `CreditLocation`), `operationalAreaGroupCode`, `operationalAreaGroupDescription`, `numberOfDays`, `maxChangeFilterDate` | `operationalAreaCode` |
| `GET operational-area-state` | (no mesmo método) | idem | `stateCode`, `stateName`, `operationalAreaCode`, `operationalAreaName`, `maxChangeFilterDate` | `operationalAreaCode` + `stateCode` |
| `GET operational-area-cep` | (no mesmo método) | idem | `operationalAreaCode`, `operationalAreaName`, `cep`, `maxChangeFilterDate` | `operationalAreaCode` + `cep` |
| `GET operational-area-city` | (no mesmo método) | idem | `cityCode`, `cityName`, `stateCode`, `stateAbbreviation`, `stateName`, `operationalAreaCode`, `operationalAreaName`, `maxChangeFilterDate` | `operationalAreaCode` + `cityCode` |

**Por que catálogo e não leitura recorrente incremental.** As rotas aceitam `StartChangeDate`/`EndChangeDate` (query) e
devolvem `maxChangeFilterDate`, então dariam uma janela incremental. Mas o que muda nas três rotas de abrangência é
sobretudo **exclusão** (um CEP ou cidade que sai da região), e uma busca por data de alteração não devolve o que foi
excluído. O domínio é pequeno (regiões são dezenas; CEPs podem ser milhares, **a confirmar com chamada real**), então a
proposta é ler **tudo** a cada 120 min, sem filtro de data, e só regravar a região que mudou, pelo desenho de hash das
comissões pagas (`Totvs:CommissionsPaidHash`).

**Desenho proposto (decisão DS1):** um método e um job só, porque a exclusão das filhas depende do pai ser regravado:

1. fluxo da leitura recorrente do README (log, empresa por empresa, token). **Não precisa de lojas**: as rotas não têm
   `branchCodeList` (o cadastro é do ERP, não da loja), então uma empresa sem lojas em `TotvsBranches` não é pulada;
2. lê as quatro rotas, cada uma página a página (`PageSize` 1000, até `hasNext = false`). Uma página com erro interrompe a
   empresa (nada é gravado dela nesta execução: a abrangência incompleta apagaria filhas);
3. agrupa por `operationalAreaCode`: a região com os seus estados, CEPs e cidades, cada lista ordenada pela chave;
4. calcula o hash do JSON agrupado de cada região e compara com o cache `Totvs:OperationalAreasHash:<cnpj>` (memória,
   2 h / 30 min, como as comissões: depois de reiniciar o host, a primeira execução regrava tudo). **Na implementação
   (§10.4)**, pela T9, o hash fica na coluna `json_hash` de `TotvsOperationalAreas`, lido do banco, sem cache de memória;
5. as regiões que mudaram vão para `untreated` e `sp_TotvsOperationalAreas*_Sync`, pai e filhas com o mesmo horário de
   lote: o merge apaga a filha que saiu da região (regra geral das filhas do README);
6. uma filha cujo `operationalAreaCode` não veio na rota `operational-area` é registrada em `auditing.Messages` e
   descartada (a FK recusaria). Uma região excluída na Totvs **fica no banco** (registros principais nunca são apagados);
7. `auditing.Records` com a chave `operationalAreaCode` e o JSON agrupado.

Como as filhas vêm por região e o `operationalAreaName` repete o do pai, as filhas não gravam o nome da região.

**Rota no controller:** `GET TotvsJobs/Totvs/OperationalAreas` (leitura recorrente, como `GET Branches`).

**Persistência** (`Script0NN_Create_Totvs_Operational_Areas_Tables.sql`, `untreated` + `totvs`; o swagger não dá tamanho
de texto nas respostas, os tamanhos abaixo seguem os das outras tabelas e se conferem no primeiro JSON real):

| Tabela | Chave | Colunas |
|---|---|---|
| `TotvsOperationalAreas` | `doc_company`, `operationalAreaCode` | `operationalAreaName` nvarchar(100), `operationalAreaType` nvarchar(30) (o nome do enum), `operationalAreaGroupCode` int, `operationalAreaGroupDescription` nvarchar(100), `numberOfDays` int |
| `TotvsOperationalAreasStates` | `doc_company`, `operationalAreaCode`, `stateCode` | `stateName` nvarchar(60) |
| `TotvsOperationalAreasCeps` | `doc_company`, `operationalAreaCode`, `cep` nvarchar(10) | — (o formato do `cep`, com ou sem traço, **a confirmar com chamada real**) |
| `TotvsOperationalAreasCities` | `doc_company`, `operationalAreaCode`, `cityCode` | `cityName` nvarchar(100), `stateCode` int, `stateAbbreviation` nvarchar(2), `stateName` nvarchar(60) |

Sem `maxChangeFilterDate`: sem filtro de data a API não o preenche. As três filhas têm FK para `TotvsOperationalAreas`.

**DTOs (`Dtos/Reads/Sellers`, todos `string`):** `OperationalArea`, `OperationalAreaState`, `OperationalAreaCep`,
`OperationalAreaCity` e o envelope `Page<T>` existente. Entidades em `Entities/Reads/Sellers` com os mesmos nomes; a
entidade `OperationalArea` leva as três listas com `[SkipProperty]`.

**Validações (`Validations/Reads/Sellers`):** `operationalAreaCode`, `stateCode`, `cityCode` inteiros > 0; `cep` com 8
dígitos; `stateAbbreviation` com 2 letras; `operationalAreaType` um dos quatro nomes. Inconsistência vai para
`auditing.Messages` sem barrar a gravação (regra das leituras).

**Job:** **13070** `TotvsGetOperationalArea`, 120 min. Procedures `sp_TotvsOperationalAreas_Sync`,
`sp_TotvsOperationalAreasStates_Sync`, `sp_TotvsOperationalAreasCeps_Sync`, `sp_TotvsOperationalAreasCities_Sync`: ids
na faixa 13500+, **a numerar**, com `IdParent = 13070`. 13071–13079 ficam reservados (13071–13073 se a DS1 for a opção B).

**Observações e riscos**
* Só leitura, sem efeito no ERP.
* Utilidade: a Newbloomers vende em lojas próprias; regiões de representante, transportadora, prazo de entrega e local
  de crédito são de venda por atacado. Antes de implementar, uma chamada real para ver se a base tem regiões (se vier
  `count = 0`, como as comissões, a decisão DS2 é adiar).
* `OperationalAreaType` no query é `integer` com nomes no enum (mesma questão da §1.3); como a proposta não filtra por
  tipo, não é enviado.
* O swagger declara `ValidationResult.IsValid`, `ValidationResult.Errors` e `ValidationResult.RuleSetsExecuted` como
  parâmetros de query das quatro rotas: é o modelo interno da Totvs vazando no swagger; **não enviar**.
* Os filtros de data das rotas de cidade e CEP citam o "bloco Estado do componente PESFM006" (texto copiado da rota de
  estado), e a de região cita "PEDFL035" em vez de PESFL035. Como a proposta não usa as datas, não afeta.
* A unicidade de `operationalAreaCode` entre tipos (o código é único no cadastro ou por tipo?) **a confirmar com chamada
  real**. As rotas de abrangência só filtram por código, o que indica código único; se não for, a chave ganha
  `operationalAreaType` e as filhas ficam ambíguas.

### 4.2 Arquivos

| Projeto | Arquivo | Mudança |
|---|---|---|
| `Domain.Totvs` | `Dtos/Reads/Sellers/OperationalArea*.cs` | 4 DTOs novos |
| `Domain.Totvs` | `Entities/Reads/Sellers/OperationalArea*.cs` | 4 entidades novas |
| `Domain.Totvs` | `Enums/EnumJob.cs` | 13070 |
| `Domain.Totvs` / `Infrastructure.Totvs` | `ITotvsSellersRepository` / `TotvsSellersRepository` | bulk insert e merge das 4 tabelas |
| `Application.Totvs` | `Validations/Reads/Sellers/OperationalArea*Validator.cs` | 4 validadores |
| `Application.Totvs` | `ITotvsSellersService` / `TotvsSellersService` | `GetOperationalArea` |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Operational_Areas_Tables.sql`, `Script0NN_Create_Totvs_Operational_Areas_Sync_Procedures.sql`, `Script0NN_Insert_Totvs_Operational_Areas_Jobs_Into_Jobs_Table.sql` | trio de scripts |
| Host | `Controllers/Totvs/TotvsController.cs` | `GET OperationalAreas`; registro do job recorrente de 120 min |
| Módulo | `README.md`, `Responsed Jsons/OperationalAreas/GetOperationalArea/` | tabela de leituras, chave de cache, exemplo das 4 respostas (dados fictícios) |

### 4.3 Testes

1. **Sem banco:** console com uma chamada a cada rota (leitura, pode ser em produção): conferir volume (`count`), formato
   do `cep`, se `operationalAreaType` vem pelo nome, unicidade do `operationalAreaCode`, e se filhas citam região que não
   vem na rota principal.
2. **Com banco:** primeira execução grava tudo; segunda não grava nada (hash); tirar um CEP da região na Totvs (ou simular
   no JSON) e conferir que só aquela região é regravada e o CEP é apagado.
3. **Pelo host:** `GET OperationalAreas`; `auditing.Logs`, `auditing.Messages`, `auditing.Records`.

### 4.4 Decisões para aprovar

| # | Pergunta | Proposta |
|---|---|---|
| DS1 | Um job que lê as 4 rotas e grava região + 3 filhas (A), ou 4 jobs com tabelas independentes (B)? | **A**: a exclusão de um CEP/cidade/estado vem do merge das filhas quando o pai é regravado. Em B cada tabela é raiz, e o merge genérico não apaga raiz: precisaria de uma procedure de "troca completa" por empresa |
| DS2 | Implementar agora? | **Respondida (27/10/2026):** implementar mesmo que a base não tenha regiões (T14) |
| DS3 | Leitura completa a cada 120 min (hash) ou incremental pelas datas? | Completa: a incremental não vê exclusões. Reavaliar se o volume de CEPs passar de algumas dezenas de páginas |

### 4.5 Ordem de execução

1. Teste 1 de 4.3 (decide DS2 e os tamanhos);
2. scripts, DTOs, entidades, validadores e repositório;
3. `GetOperationalArea` e a rota; testes 2 e 3;
4. README e `Responsed Jsons`.

---

---

## 5. Product

Swagger oficial: `https://apitotvsmoda.bhan.com.br/api/totvsmoda/product/v2/swagger/index.html`, versão **2.8.31** (a mesma
da fase 2; nenhuma rota nova ou removida). Numeração da seção provisória (ordem 4 da tabela do plano): renumerar na
consolidação.

**47 rotas:** 1 implementada, 3 planejadas na fase 3, 43 faltam. A contagem prévia do plano (23 leituras / 20 escritas)
põe `POST compositions` e `POST omni-changed-balances` como escrita; as duas são **consultas** (corpo é filtro). A conta
certa é **25 leituras e 18 escritas**.

| Rota | Situação |
|---|---|
| `POST references/search` | Implementada (`PostSearchReferences`, job 13000; `GetProductByCode`, job 13014) |
| `POST prices/search` | Planejada fase 3 (`PostSearchPrices`, 13015) — ver 5.9 |
| `POST costs/search` | Planejada fase 3 (`PostSearchCosts`, 13016) — ver 5.9 |
| `POST balances/search` | Planejada fase 3 (`PostSearchBalances`, 13017) — ver 5.9 |
| `GET classificationType` | **Falta** — catálogo (5.1) |
| `GET classifications` | **Falta** — catálogo (5.1) |
| `GET category` | **Falta** — catálogo (5.1) |
| `GET grid` | **Falta** — catálogo (5.1) |
| `GET measurement-unit` | **Falta** — catálogo (5.1) |
| `POST colors/search` | **Falta** — catálogo (5.1) |
| `GET additional-fields-types` | **Falta** — catálogo (5.1) |
| `GET product-grouper-configuration` | **Falta** — catálogo (5.1) |
| `GET instruction-items` | **Falta** — catálogo (5.1) |
| `POST compositions` | **Falta** — catálogo (5.1) |
| `GET price-tables-headers` | **Falta** — catálogo (5.1) |
| `GET price-table-scales` | **Falta** — catálogo (5.1) |
| `POST price-tables/search` | **Falta** — leitura recorrente (5.2) |
| `GET promotion` | **Falta** — leitura recorrente (5.2) |
| `GET similar` | **Falta** — leitura recorrente (5.2) |
| `POST batch/search` | **Falta** — leitura recorrente, condicionada (5.2) |
| `POST omni-changed-balances` | **Falta** — alternativa ao `PostSearchBalances` (5.2) |
| `POST product-codes/search` | **Falta** — conferência (5.2) |
| `POST products/search` | **Falta** — coberta pelas referências: não implementar (5.3) |
| `GET products/{code}/{branchCode}` | **Falta** — sob demanda sem persistência (5.4) |
| `GET kardex-movement` | **Falta** — sob demanda sem persistência (5.4) |
| `GET grid-frequency` | **Falta** — sob demanda sem persistência (5.4) |
| `GET measurement-conversions` | **Falta** — sob demanda sem persistência (5.4) |
| `GET composition-product` | **Falta** — sob demanda sem persistência (5.4) |
| `GET composition-group-product` | **Falta** — sob demanda sem persistência (5.4) |
| `POST colors` | **Falta** — escrita, cadastro mestre (5.5) |
| `POST grids` | **Falta** — escrita, cadastro mestre (5.5) |
| `POST classifications` | **Falta** — escrita, cadastro mestre (5.5) |
| `POST barcodes` | **Falta** — escrita, códigos de barras (5.5) |
| `POST barcodes/update` | **Falta** — escrita, códigos de barras (5.5) |
| `POST classification-relationship/create` | **Falta** — escrita, classificação do produto (5.5) |
| `POST classification-relationship/update` | **Falta** — escrita, classificação do produto (5.5) |
| `PUT products/{code}/{branchCode}` | **Falta** — escrita, dados fiscais/por empresa (5.6) |
| `PUT branch-info/{branchCode}` | **Falta** — escrita, dados por empresa em massa (5.6) |
| `PUT data` | **Falta** — escrita, dados fiscais em massa (5.6) |
| `POST references` | **Falta** — escrita, inclusão de referência e SKUs (5.6) |
| `POST values/create` | **Falta** — escrita, preço/custo (5.7) |
| `POST values/update` | **Falta** — escrita, preço/custo (5.7) |
| `POST promotion-values/update` | **Falta** — escrita, promoção (5.7) |
| `POST promotion-items` | **Falta** — escrita, promoção (5.7) |
| `POST batch/create` | **Falta** — escrita, lote, condicionada (5.8) |
| `PUT batch-info` | **Falta** — escrita, lote, condicionada (5.8) |
| `POST batch-preferentialsalesorder` | **Falta** — escrita, lote, condicionada (5.8) |

Nomes pela decisão 18: o segmento final de ação vai para a frente, como em `duplicates/search` → `PostSearchDuplicates`
(`values/create` → `PostCreateValues`, `batch/search` → `PostSearchBatch`). `PUT data` fica `PutData`: dentro do
`TotvsProductsService` não há ambiguidade.

**Regras comuns desta API**

* **Sem divisão por marca.** Os catálogos, cores, grades e classificações são do cadastro inteiro da empresa (não têm
  `branchInfoCode`): chave `doc_company` + código. Só as referências usam a matriz da marca.
* **Lojas:** rotas que exigem empresa (`promotion`, `price-tables/search`) usam todas as lojas de `totvs.TotvsBranches`.
* **Escritas com `branchCode`:** o passo 3 do fluxo da §1.2 confere o `branchCode` (não o CNPJ) em `totvs.TotvsBranches`.
* **Enums com letra.** `ProductValueType` (`Cost`/`Price`, descrição "C - Custo. P - Preço") e `PromotionStatusType`
  (`Open`/`Blocked`/`Closed`, descrição "A/B/E") são declarados `integer`, mas a descrição dá **letras**, não números. O
  conversor da §1.3 (número da descrição) não se aplica: o primeiro teste manda `"P"`, o nome e o número, e registra o que
  a API aceita. Os demais enums de escrita têm número na descrição (`TaxStatusCodeType` 0–8, `SpedItemType` 0–10 e 99,
  `BatchType` 1–5, `BatchItemStatusType` 1, 2, 4, 10, `ProductWebStatusType` 1–2, `InspectionType` 0–2).
* Os parâmetros `ValidationResult.IsValid/Errors/RuleSetsExecuted` que aparecem em vários `GET` são vazamento do modelo
  interno da Totvs: não são enviados.

---

### 5.1 Catálogos (job de 120 min)

Domínios pequenos, todos com `StartChangeDate`/`EndChangeDate` e `maxChangeFilterDate` na resposta (exceto cores). Por
isso **não usam hash**: seguem o desenho do `GetBranchesList` — janela desde `MAX(maxChangeFilterDate)` − 1 h (banco vazio:
desde 01/01/2000), **uma chamada paginada, sem fatias**, cache de estado `Totvs:<Catálogo>:<cnpj>` (código → registro)
para descartar o que não mudou, bulk em `untreated` e `sp_<Tabela>_Sync`. Um job por rota, como o resto do módulo
(decisão PR4). Procedures: a numerar.

| Método | Rota da API | Rota no controller | Tabelas (chave) | Job |
|---|---|---|---|---|
| `GetClassificationType` | `GET classificationType` (`PageSize` máx. 100) | `GET Products/ClassificationTypes` | `TotvsClassificationTypes` (`doc_company`, `code`) | 13080 |
| `GetClassifications` | `GET classifications` (sem `TypeCodeList`: todos) | `GET Products/Classifications` | `TotvsProductClassifications` (`doc_company`, `typeCode`, `code`; T13, para não colidir com `TotvsPersonClassifications`) | 13081 |
| `GetCategory` | `GET category` | `GET Products/Categories` | `TotvsCategories` (`doc_company`, `code`) | 13082 |
| `GetGrid` | `GET grid` | `GET Products/Grids` | `TotvsGrids` (`doc_company`, `code`); `TotvsGridsSizes` (`doc_company`, `gridCode`, `sequence`) | 13083 |
| `GetMeasurementUnit` | `GET measurement-unit`, `Expand=additionalVariations` | `GET Products/MeasurementUnits` | `TotvsMeasurementUnits` (`doc_company`, `code`); `TotvsMeasurementUnitsVariations` (`doc_company`, `measurementUnitCode`, `code`) | 13084 |
| `PostSearchColors` | `POST colors/search`, `expand=additionalColorInformation`, `pageSize` 500 | `POST Products/Colors` | `TotvsColors` (`doc_company`, `colorCode`); `TotvsColorsAdditionalInformation` (`doc_company`, `colorCode`, `branchCode`) | 13085 |
| `GetAdditionalFieldsTypes` | `GET additional-fields-types` (máx. 100) | `GET Products/AdditionalFieldsTypes` | `TotvsAdditionalFieldsTypes` (`doc_company`, `code`) | 13086 |
| `GetProductGrouperConfiguration` | `GET product-grouper-configuration` (máx. 100) | `GET Products/GrouperConfigurations` | `TotvsProductGrouperConfigurations` (`doc_company`, `grouperCode`); `...Items` (`doc_company`, `grouperCode`, `code`) | 13087 |
| `GetInstructionItems` | `GET instruction-items` (máx. 100), **sem** `Expand` de imagem (PR13) | `GET Products/InstructionItems` | `TotvsInstructionItems` (`doc_company`, `code`) | 13088 |
| `PostCompositions` | `POST compositions`, `expand=fibers` (máx. 100) | `POST Products/Compositions` | `TotvsCompositions` (`doc_company`, `code`); `TotvsCompositionsFibers` (`doc_company`, `compositionCode`, `code`) | 13089 |
| `GetPriceTablesHeaders` | `GET price-tables-headers` | `GET Products/PriceTablesHeaders` | `TotvsPriceTables` (`doc_company`, `code`) e 5 filhas (abaixo) | 13090 |
| `GetPriceTableScales` | `GET price-table-scales` | `GET Products/PriceTableScales` | `TotvsPriceTableScales` (`doc_company`, `scaleCode`); `...Items` (`doc_company`, `scaleCode`, `quantity`) | 13091 |

Detalhes que não são óbvios:

* **Cores sem data.** `ColorDataResponseModel` traz só `colorCode`, `colorName` e `additionalColorInformation`, sem
  `maxChangeFilterDate`, embora o filtro aceite `change`. Proposta: leitura **completa** a cada execução e o desenho das
  comissões — hash do JSON de cada cor em `Totvs:ColorsHash:<cnpj>`, só regrava a cor cujo hash mudou. As cores das
  referências (`TotvsSkusColors`) continuam como estão; esta tabela é o cadastro mestre (PRDFL025), com as informações por
  empresa. **Na implementação (§10.5)**, pela T9, o hash fica na coluna `json_hash` de `TotvsColors`, lido do banco, e a
  leitura usa `filter: {}` (o `filter` é obrigatório; vazio, devolve todas as cores).
* **Grade:** `grid` é lista de textos (tamanhos), sem chave: `sequence` = posição. Mesmo dado de `TotvsSkusGrids`, mas
  por grade, não por referência; serve para validar `sizeName` no `PostReferences`.
* **Unidade de medida:** a chave da variação (`code`, até 40) é suposição — a confirmar com chamada real; se repetir, usar
  `sequence`. **Conferido (§10.5):** a base tem uma variação só (`PAR` → `PAIR`); fica `code`.
* **Classificações:** `TypeCodeList` aceita até 120 itens; sem ele vêm todas. Uma troca de nome do tipo (PRDFL011) talvez
  não mude a `maxChangeFilterDate` da classificação (PRDFL012): o nome do tipo vale o de `TotvsClassificationTypes`.
* **Cabeçalho de tabela de preço:** filhas `TotvsPriceTablesSalesOrderClassifications` (`typeCode`, `code`),
  `TotvsPriceTablesPersonClassifications` (`typeCode`, `code`), `TotvsPriceTablesPaymentConditions` (`code`),
  `TotvsPriceTablesAveragePeriods` (`quantity`), `TotvsPriceTablesAveragePeriodQuantities` (`sequence`), todas com
  `doc_company` + `priceTableCode` antes. `type` (`PriceTableType`) gravado pelo nome. A confirmar se alteração só numa
  filha muda a `maxChangeFilterDate` do cabeçalho; se não mudar, trocar para o desenho de hash. Só faz sentido se a empresa
  usa tabela de preço (PR5).
* **Itens de instrução:** `image` e `globalImage` são `string` (provavelmente base64): fora do `Expand` (PR13).

Entidades com os mesmos nomes da API; enums (`categoryType`, `type`) gravados pelo nome, como no resto do módulo.

### 5.2 Leituras recorrentes

| Método | Rota da API | Rota no controller | Tabelas (chave) | Job | Intervalo |
|---|---|---|---|---|---|
| `PostSearchPriceTables` | `POST price-tables/search` | `POST Products/PriceTables` | `TotvsPriceTablesItems` (`doc_company`, `priceTableCode`, `productCode`); `TotvsPriceTablesItemsPrices` (+ `branchCode`, `scaleCode`) | 13092 | 120 min (PR5) |
| `GetPromotion` | `GET promotion` (uma chamada por loja) | `GET Products/Promotions` | `TotvsPromotions` (`doc_company`, `branchCode`, `promotionCode`, `productCode`) | 13093 | 120 min |
| `GetSimilar` | `GET similar` (máx. 100) | `GET Products/Similar` | `TotvsProductsSimilar` (`doc_company`, `productCode`); `TotvsProductsSimilarItems` (`doc_company`, `productCode`, `listType`, `similarProductCode`) | 13094 | 120 min |
| `PostSearchBatch` | `POST batch/search` | `POST Products/Batches` | `TotvsBatches` (`doc_company`, `branchCode`, `batchCode`, `productCode`); `TotvsBatchesItems` (+ `batchItemSequential`); `TotvsBatchesItemsAdditionalFields` (+ `code`) | 13095 | 120 min (PR6) |
| `PostOmniChangedBalances` | `POST omni-changed-balances` | — (só se PR2 escolher) | reusa `TotvsBalances` da fase 3 | 13096 (reservado) | 3 min |
| `PostSearchProductCodes` | `POST product-codes/search` | `POST Products/ProductCodes` | nenhuma (conferência) | 13097 | diário (PR3) |

**`PostSearchPriceTables`**

* Corpo: `filter` (`ProductFilterModel`) com `change.startDate/endDate`, `change.inPriceTableItem = true`,
  `change.branchPriceTableCodeList` (lojas) e `change.priceTableCodeList` (a tabela); `option.branchCodeList*` (lojas da
  empresa) e `option.priceTableCode*` (**uma tabela por chamada**); `pageSize` 1000.
* As tabelas vêm de `TotvsPriceTables` (por isso o `GetPriceTablesHeaders` roda antes, como filiais antes das buscas):
  proposta, só as com `endDate` nula ou ≥ hoje.
* Janela por tabela: `MAX(maxChangeFilterDate)` da tabela − 1 h; banco vazio desde 01/01/2000 em fatias de 30 dias, depois
  7, como os produtos. Limite do `change` desta rota: a confirmar (referências aceitam 1 ano).
* A paginação é por produto (`productCode` + lista `prices` por loja e escala), como em `prices/search`; a confirmar se
  um produto pode vir partido entre páginas (as referências vêm).
* As filhas (`prices`) são apagadas no merge quando o pai é regravado (regra geral), porque a resposta traz o produto com
  todas as lojas pedidas.

**`GetPromotion`**

* `BranchCode*` obrigatório: uma chamada por loja de `TotvsBranches`; `PageSize` 500; janela `StartChangeDate`/`EndChangeDate`.
* Cada item é uma linha promoção × produto (`previousValue`, `promotionValue`, `startDate`, `endDate`, `status` A/B/E).
* **Risco de perder alteração:** o swagger diz que `maxChangeFilterDate` é o "maior valor entre DT_CADASTRO da promoção e
  do item", e o filtro é "data de cadastro". Mudança de valor ou de situação pode não entrar na janela. Proposta (PR15):
  além da janela, reler a cada execução as promoções `Status=Open` com `EndDate` ≥ hoje. A confirmar com chamada real.
  **Na implementação (§10.7)** o filtro de data da rota dá erro na Totvs (`ORA-00920`): cada loja é lida inteira e o hash de
  cada item (`json_hash`, T9) pega também a troca de valor e de situação, o que resolve a PR15 sem a releitura das abertas.
* O preço promocional **vigente** por produto já virá do `PostSearchPrices` (`isPromotionalPrice`, 5.9); esta tabela é o
  cadastro das campanhas (período, valor anterior, situação), que serve de base para as escritas de 5.7.

**`GetSimilar`**

* Sem `ProductsCode`: a confirmar se lista todos os produtos com similar. Janela por `lastChageDate` (grafia da API; não
  há `maxChangeFilterDate`): guardado na coluna `lastChageDate` e usado no `MAX` da janela.
* `listType`: `Product` (`similarProductList`) ou `ReferenceGroup` (`similarRefereceGroupProductList`, grafia da API).
  `similarGroupProductList` (produtos da mesma grade) **não é gravada**: sai de `TotvsSkusColorsProducts` por `referenceId`.

**`PostSearchBatch`** (só se a empresa usa lote, PR6)

* `filter*`: `branchCode` (uma chamada por loja), `updatedDateStart`/`updatedDateEnd`; `productCodeList` até 500;
  `expand` `"additionalFieds"` (grafia do swagger; a resposta usa `additionalFields`: mandar as duas grafias no primeiro
  teste).
* **A resposta não tem `maxChangeFilterDate`.** Janela: início da última execução com sucesso do job em `auditing.Logs`
  − 1 h; banco vazio: últimos 10 meses. A confirmar se a rota aceita janela sem `batchCode`.
* `batchCode` é anulável no schema; a chave assume que a resposta sempre o traz (a confirmar).

**`PostOmniChangedBalances`**: mesma finalidade do `balances/search` com `change.inStock`, mas feita para "saldos por
alteração": `filter.change.balances[]` (`branchCode` + `stockCodeList`), `hasStock`, `option.isTransaction`,
`isSalesOrder`, `salesOrderStatusList`, `pageSize` 500, e devolve `avaliableStock` (grafia da API). Não verifica saldo
"consolidado". **Proposta (PR2):** não criar job separado; na fase 3, o `PostSearchBalances` testa as duas e fica com uma,
gravando em `TotvsBalances`. O id 13096 fica reservado.

**`PostSearchProductCodes`**: devolve só `productCode` + `maxChangeFilterDate`, `pageSize` até **100 000**. Não é
cadastro; serve para **conferir** o que o job de referências perdeu. Proposta (PR3): uma vez por dia, por marca (filtro de
classificação 6000, igual ao `PostSearchReferences`), compara com `TotvsSkusColorsProducts` e chama o
`GetProductByCode` para cada código que falta (registra em `auditing.Messages` quantos foram recuperados).

### 5.3 `products/search` — não implementar

`ProductDataModel` é o mesmo dado das referências **achatado por produto**: usa o mesmo `ReferenceOptionModel` no
`option` e o mesmo `ProductFilterModel`, e todos os campos já estão em `TotvsSkusColorsProducts` (`ncm`, `ipi`, `cst`,
`cest`, `prefixEanGtin`, `isActive`..., estoques mínimo/máximo/ideal, datas de comercialização, `isBlocked`) e nas filhas
(`TotvsProductsBarCodes`, `...Classifications`, `...AdditionalFields`, `...Suppliers`, `...Manufacturers`,
`...ReferenceCodeSequences`, `...WebData`, `...WebDetails`, `...BranchesProductBlocked`, `...ConservationInstructions`).
O `expand` dela é um subconjunto do das referências; a única coisa a mais é `additionalColorInformation` por empresa,
que o `PostSearchColors` (5.1) cobre. Uma segunda leitura do mesmo cadastro em outra forma duplicaria tabelas e chamadas
sem dado novo (decisão PR1).

### 5.4 Leituras sob demanda sem persistência

Consultas pontuais: o resultado é do momento (movimento, conversão, código lido no caixa) ou já está nas referências.
Não gravam tabela; o service devolve o corpo da Totvs no `Result.data` (o `Result` da §1.1) e grava a chamada em
`totvs.TotvsRequests` (com `response_body`). Exigem `X-User-Name` e `{docCompany}`, como as leituras sob demanda atuais.
Respostas: **200** com o corpo; **404** quando a Totvs responde 404 ou a lista vem vazia; **422** para empresa não
cadastrada ou 400 da Totvs. Fluxo das leituras sob demanda do README, sem o passo de gravação. Na nova tentativa valem as
regras das leituras (401, 429 e 502/503/504), porque não alteram nada.

| Método | Rota da API | Rota no controller | Parâmetros e validações | Chave em `TotvsRequests` | Job |
|---|---|---|---|---|---|
| `GetProducts` | `GET products/{code}/{branchCode}` | `GET Products/{docCompany}/Codes/{code}?branchCode=` | `code*` texto (código do produto, pack, barras ou RFID; até 40, a confirmar), `branchCode` opcional (só para item de lote) | `code` | 13100 |
| `GetKardexMovement` | `GET kardex-movement` | `GET Products/{docCompany}/Kardex?productCode&branchCode&startDate&endDate&balanceType` | todos obrigatórios no controller (o swagger não marca nenhum; a confirmar); `branchCode` loja da empresa; `endDate − startDate` ≤ **180 dias**; `balanceType` até 3 dígitos | `productCode|branchCode|startDate|endDate` | 13101 |
| `GetGridFrequency` | `GET grid-frequency` | `GET Products/{docCompany}/{productCode}/GridFrequency` | `ProductCode*` até 9 dígitos; lê todas as páginas | `productCode` | 13102 |
| `GetMeasurementConversions` | `GET measurement-conversions` | `GET Products/{docCompany}/MeasurementConversions?productCode&groupSequence&onlyDefault` | exatamente um entre `productCode` e `groupSequence` (o swagger não diz; a confirmar) | `productCode` ou `g:groupSequence` | 13103 |
| `GetCompositionProduct` | `GET composition-product` | `GET Products/{docCompany}/CompositionProduct?productCodeList&groupCodeList&referenceCodeList` | pelo menos uma das três listas (regra da descrição); `PageSize` 100, lê todas as páginas | as listas, separadas por `;` | 13104 |
| `GetCompositionGroupProduct` | `GET composition-group-product` | `GET Products/{docCompany}/CompositionGroupProduct?...` | idem | idem | 13105 |

Observações:

* **`GetProducts`:** o caminho oficial tem `{branchCode}`, mas o swagger declara `branchCode` como **query** e opcional (no
  `PUT` do mesmo caminho ele é de caminho e obrigatório). Proposta: montar `products/{code}/{branchCode}` quando vier e
  testar também `products/{code}?branchCode=`; registrar o que funciona. Uso principal: resolver um código lido (barras,
  pack, RFID com `statusRfid`, item de lote) para o produto. A resposta é paginada (`items`), porque um pack traz vários.
  Não grava: é um resolvedor; o produto em si vem por `GetProductByCode`.
* **`GetKardexMovement`:** não é paginada; uma loja e um produto por chamada; `movementDate` vem como texto. É histórico
  de estoque para consulta (auditoria de saldo), não cadastro: gravar seria uma tabela de movimentos sem chave confiável
  (`documentNumber` + `historyCode` pode repetir) — se a área precisar de kardex no BI, é outra decisão.
* **Composições:** a descrição de **cada uma** das duas rotas recomenda a outra ("Caso utilize composição por grupo, não é
  recomendado..."), contraditório. As referências já trazem `composition` (`TotvsSkusCompositions`, por referência). Estas
  duas ficam só como consulta; testar as duas e registrar qual a empresa usa.
* **`GetMeasurementConversions`:** campos sem descrição no swagger; o `balances/search` também devolve
  `measurementConversion` por saldo (5.9).

### 5.5 Escritas de cadastro mestre, códigos de barras e classificação

Fluxo da §1.2. Um método por rota no service; DTOs em `Dtos/Writes/Products/<Recurso>/`; validadores em
`Validations/Writes/Products/<Recurso>/`. Rotas `POST`/`PUT` com `X-User-Name` e `{docCompany}`. Corpos com lista de
produtos: chave em `TotvsRequests` = códigos dos produtos separados por vírgula, cortados em 400 caracteres.

| Método | Rota da API | Rota no controller | Resposta Totvs | Job | Chave |
|---|---|---|---|---|---|
| `PostColors` | `POST colors` | `POST Products/{docCompany}/Colors` | 201 `SuccessProcessingModel` | 13124 | `code` |
| `PostGrids` | `POST grids` | `POST Products/{docCompany}/Grids` | 201 `InsertGridResponseModel` (`gridCode`, `name`) | 13126 | `gridCode` ou `name` |
| `PostClassifications` | `POST classifications` | `POST Products/{docCompany}/Classifications` | 201 | 13125 | `typeCode` |
| `PostBarcodes` | `POST barcodes` | `POST Products/{docCompany}/BarCodes` | 201 | 13120 | produtos |
| `PostUpdateBarcodes` | `POST barcodes/update` | `POST Products/{docCompany}/BarCodes/Update` | 200 | 13121 | produtos |
| `PostCreateClassificationRelationship` | `POST classification-relationship/create` | `POST Products/{docCompany}/ClassificationRelationships/Create` | 201 | 13122 | produtos/`r:referenceId` |
| `PostUpdateClassificationRelationship` | `POST classification-relationship/update` | `POST Products/{docCompany}/ClassificationRelationships/Update` | 200 | 13123 | idem |

**Corpos e validações**

| Corpo (classes) | Campos (`*` obrigatório) | Validações |
|---|---|---|
| `UpsertColorCommand`, `UpsertColorInfoAdditionalModel` | `code*` (10), `name*` (40); `colorInfoAdditionals`: `branchCode*` (4 dígitos), `auxiliaryCode` (10), `auxiliaryName` (80), `pantoneCode` (40), `isFinishedProduct`, `isRawMaterial`, `isBulkMaterial` | tamanhos; `branchCode` loja da empresa (passo 3) e sem repetição |
| `InsertGridCommand`, `GridSizeModel` | `gridCode` (6 dígitos, opcional), `name*` (40), `type` (40), `gridSizes*` (`name*`, 10) | pelo menos um tamanho; tamanhos sem repetição |
| `UpsertTypeClassificationCommand`, `UpsertClassificationModel` | `typeCode*` (4 dígitos), `description` (40), `auxiliaryDescription` (100); `classifications`: `code`, `description`, `auxiliaryDescription` (sem tamanho no swagger; usar 10/40/100 dos outros modelos, a confirmar) | `code` sem repetição; **`typeCode` 6000 recusado** (PR10) |
| `InsertProductBarCodeCommand`, `ProductBarCodeModel`, `BarCodeInsertModel` | `products`: `productCode*` (9 dígitos), `barCodes`: `barCode*` (40), `quantity*` (**`integer`** no schema, "9 inteiros e 3 decimais" na descrição), `isMainCode`, `inRfid` | pelo menos um produto e um código; `barCode` sem repetição no corpo; no máximo um `isMainCode = true` por produto; `quantity` > 0 |
| `UpdateProductBarCodeCommand`, `ProductBarCodeUpdateModel`, `BarCodeUpdateModel` | `productCode*`, `barCodes`: `barCode*` (a chave: não troca o código, só `quantity` (`number`) e `isMainCode`) | idem |
| `InsertProductClassificationCommand` / `UpdateProductClassificationCommand`, `ProductClassificationModel`, `ProductClassificationInsertModel` | `products`: `productCode` **ou** `referenceId` (classificação do produto ou do grupo; a descrição não diz, a confirmar), `classifications`: `typeCode*` (4 dígitos), `code*` (10) | exatamente um entre `productCode` e `referenceId`; `typeCode` sem repetição por produto; **`typeCode` 6000 recusado** (PR10); se `TotvsClassifications` estiver carregada, o par `typeCode`/`code` precisa existir nela |

**Riscos por grupo**

* **Cores e grades — baixo.** Cadastro mestre. `POST colors` é **inclusão e alteração**: mandar um `code` existente
  **renomeia a cor em todas as referências**. Grade é só inclusão (sem rota de exclusão: um erro fica no ERP).
* **Tipos de classificação (`POST classifications`) — médio.** Também é inclusão/alteração: renomear um tipo ou uma
  classificação muda relatórios, BI e categorização de e-commerce de todos os produtos que a usam.
* **Códigos de barras — médio/alto.** É o que o caixa e o WMS leem: um código no produto errado vende/movimenta o produto
  errado. O código **padrão** (`isMainCode`) é o `productSku` da API, que é o `sku` de `TotvsSkusColorsProducts` e a chave
  de integrações de e-commerce: trocar o padrão troca o SKU. `inRfid = true` grava o RFID como "Em andamento".
* **Classificação do produto — médio, e alto para a 6000.** A integração separa as marcas pela classificação **6000**
  (código `"2"` Misha, `"4"` Open Era) no `PostSearchReferences`: alterar essa classificação move o produto de marca (ou o
  tira das duas) na próxima carga. Daí a trava PR10.

Depois da escrita o banco local não é atualizado na hora: chega pelo job do catálogo (120 min) ou do
`PostSearchReferences` (120 min); quem precisa antes chama `GET Products/{docCompany}/{code}` (`GetProductByCode`).

### 5.6 Escritas de dados do produto e inclusão de referência

| Método | Rota da API | Rota no controller | Resposta Totvs | Job | Chave |
|---|---|---|---|---|---|
| `PutProducts` | `PUT products/{code}/{branchCode}` | `PUT Products/{docCompany}/{code}/{branchCode}` | 200 `SuccessProcessingModel` | 13128 | `code|branchCode` |
| `PutBranchInfo` | `PUT branch-info/{branchCode}` | `PUT Products/{docCompany}/BranchInfo/{branchCode}` | 200 `SuccessProcessingWithAditionalDataModel` (`processingDate`, `aditionalData` sem tipo) | 13129 | `branchCode|filtro` |
| `PutData` | `PUT data` | `PUT Products/{docCompany}/Data` | 200 | 13130 | filtro |
| `PostReferences` | `POST references` | `POST Products/{docCompany}/References` | 201 `InsertReferenceResultModel` (`skus`: `productCode`, `colorCode`, `sizeName`) | 13127 | `referenceSettingsCode|groupCode` dos níveis |

`TotvsRequests.http_method` grava `PUT` nas três primeiras (o verbo da documentação, decisão 18). A regra de nova
tentativa da §1.2 vale igual.

**Corpos e validações**

| Corpo | Campos | Validações |
|---|---|---|
| `ProductBranchUpdateRequestModel` (`PutProducts`) | caminho `code*`, `branchCode*`; corpo todo opcional: `ncmCode` (20), `cstCode` (`TaxStatusCodeType` 0–8), `weight` (9,3), `isOwnProduction`, `isInactive`, `isFinishedProduct`, `isRawMaterial`, `isBulkMaterial` | pelo menos um campo; `branchCode` loja da empresa; NCM só dígitos (8) — a confirmar se a Totvs aceita com ponto |
| `UpsertProductBranchInfoCommand`, `ProductDataFilter` (`PutBranchInfo`) | caminho `branchCode*`; `filter`: `productCodeList` (até **500**), `barCodeList` (até **500**), `groupCode` (10), `referenceCode` (60); `isOwnProduction`, `isInactive`, `isFinishedProduct`, `isRawMaterial`, `isBulkMaterial`, `spedItemType` (`SpedItemType` 0–10, 99) | filtro com **exatamente um** dos quatro (PR9: na primeira entrega só `productCodeList`/`barCodeList`); pelo menos um campo de dado |
| `UpdateProductDataCommand` (`PutData`) | `filter` (`ProductDataFilter`); `ncmCode` (20), `measuredUnit` (3), `cstCode`, `weight` (9,3), `cestCode` (20), `prefixEanGtin` (20) | idem filtro; pelo menos um campo; `prefixEanGtin` **não aceita remoção** (descrição): recusar texto vazio; `measuredUnit` em `TotvsMeasurementUnits` quando carregada |
| `InsertReferenceCommand` e filhos (`PostReferences`) | ver abaixo | ver abaixo |

**`PostReferences`** — classes `InsertReferenceCommand`, `GroupModel`, `BranchModel`, `InsertProductModel`,
`ProductClassificationInsertModel`, `InsertProductAdditionalFieldModel`, `ProductSupplierInsertModel`,
`ProductManufacturerInsertModel`, `BarCodeInsertModel`, `InsertProductLocationModel`, `ProductValueInsertModel`,
`InsertProductImageModel`, `InsertReferenteWebModel`, `InsertReferenteWebDetailModel` (grafia da API) e os enums.

* Raiz: `referenceSettingsCode*` (máscara, 6 dígitos), `referenceName` (100; se vazio a máscara monta), `gridCode*`
  (6 dígitos), `observation`, `groups*` (`levelNumber*` sem repetição, `groupCode*` 10, `groupName*` 40), `branchs*`
  (`branchCode` 4, indicadores, `isFractionalQuantity`, `spedItemType`, `inspectionType`, `batchType`), `skus*`,
  `values`, `images`, `referenceWeb` (`status*`, `description` 255, medidas 9,3, `descriptiveList`: `type*` 2 dígitos).
* SKU: `productCode` (9, opcional: a Totvs gera), `productName*` (60), `colorCode*` (10), `sizeName*` (10),
  `measuredUnit*` (3), `ncmCode` (20), `cstCode`, `weight` (9,3), `prefixEanGtin` (20), `cestCode` (20),
  `conservationInstructionCode` (9); listas `classifications`, `additionalFields` (`code*` 4, `value*` texto, número ou
  data `DD/MM/YYYY` — o DTO usa `object` e o validador confere o tipo pelo `TotvsAdditionalFieldsTypes`), `suppliers`
  (`supplierCpfCnpj*` 14, `originalCode` 60, `isMainSupplier`), `manufacturers` (`manufacturerCpfCnpj*` 14), `barCodes`,
  `locations` (`branchCode*`, `locationCode*` 15), `values`, `images` (`code*` 6, `imageTypeCode` 2).
* Validações: a quantidade de `groups` = níveis da máscara (regra da descrição; a máscara não é lida por nenhuma rota, a
  Totvs valida); par `colorCode`+`sizeName` sem repetição; `sizeName` na grade (`TotvsGridsSizes`) e `colorCode` em
  `TotvsColors` quando carregadas; CPF/CNPJ só dígitos (11 ou 14); um só `isMainSupplier` e um só `isMainCode` por SKU;
  todos os `branchCode` são lojas da empresa; a classificação **6000** é **obrigatória** com `"2"` ou `"4"` (senão a
  referência nunca entra no `PostSearchReferences`); `values` na raiz e nos SKUs **recusados** enquanto PR11 não liberar.
* O 201 traz os `productCode` gerados: vão no `Result` e no log. A referência chega ao banco pelo `PostSearchReferences`
  (até 120 min) ou por `GetProductByCode` chamado pelo sistema de origem.

**Riscos por grupo**

* **Dados fiscais (`PutProducts`, `PutData`) — alto.** NCM, CST, CEST e unidade entram no cálculo de impostos da nota:
  um NCM errado gera NF-e rejeitada ou imposto errado (IPI vem da TIPI pelo NCM). `PutData` aceita até 500 produtos ou
  **um grupo/referência inteiro** numa chamada.
* **Dados por empresa (`PutProducts`, `PutBranchInfo`) — alto.** `isInactive = true` tira o produto de venda na loja;
  `spedItemType` muda o SPED fiscal. Com `groupCode`/`referenceCode` no filtro, altera todos os produtos do grupo na loja
  (PR9).
* **Inclusão de referência — médio/alto.** Cria produto vendável, sem rota de exclusão; com `values`, já entra com preço
  (risco de 5.7); com `barCodes`, risco de 5.5.

### 5.7 Escritas de preço, custo e promoção (efeito comercial direto)

| Método | Rota da API | Rota no controller | Resposta Totvs | Job | Chave |
|---|---|---|---|---|---|
| `PostCreateValues` | `POST values/create` | `POST Products/{docCompany}/Values/Create` | 201 | 13131 | produtos |
| `PostUpdateValues` | `POST values/update` | `POST Products/{docCompany}/Values/Update` | 200 | 13132 | produtos |
| `PostUpdatePromotionValues` | `POST promotion-values/update` | `POST Products/{docCompany}/PromotionValues/Update` | 200 | 13133 | `branchCode|promotionCode` |
| `PostPromotionItems` | `POST promotion-items` | `POST Products/{docCompany}/PromotionItems` | 201 | 13134 | `branchCode|promotionCode` |

**Corpos**

* `InsertProductValueCommand` / `UpdateProductValueCommand` → `ProductValueModel` (`productCode*`, `values`) →
  `ProductValueInsertModel`: `branchCode*`, `valueType*` (`ProductValueType`, **letra C/P**, ver regras comuns),
  `valueCode*` (o tipo de preço/custo do PRDFL003), `value*` (`number`, sem tamanho no swagger).
* `UpdatePromotionValueCommand` / `InsertPromotionItemCommand`: `branchCode*` (4), `promotionCode*` (6), `items*`:
  `productCode*` (9), `promotionValue*` (12 inteiros, 6 decimais).

**Validações**

* obrigatórios e tamanhos; `branchCode` loja da empresa; sem repetição de (`productCode`, `branchCode`, `valueType`,
  `valueCode`) ou de `productCode` na promoção; `value`/`promotionValue` > 0;
* `valueCode` e `branchCode` numa **lista permitida em configuração** (`Totvs:Writes:Values:AllowedCodes`, PR7): impede
  gravar num tipo de preço que a empresa não usa (ex.: tabela de atacado no lugar do varejo);
* **trava de variação** (PR7): com o valor atual em `TotvsPrices`/`TotvsCosts` (fase 3), recusar (422) variação acima de
  um percentual configurado, salvo `?confirmVariation=true`; sem valor local, recusar `values/update` (não há como
  conferir);
* promoção: se `TotvsPromotions` estiver carregada, a promoção precisa existir na loja com situação `A` (aberta) e
  `promotionValue` < `previousValue` (a confirmar se a Totvs aceita promoção acima do preço);
* tamanho da lista: até 500 produtos por chamada (não está no swagger; proposta, para limitar o estrago de um erro).

**Riscos**

* **`values/create|update` — o mais alto da API.** O preço gravado vale na hora para os PDVs da loja e para o
  e-commerce que lê o preço da Totvs; um valor errado vende com prejuízo ou trava venda. O custo muda margem, CMV e a
  valorização do estoque (fiscal/contábil). A diferença entre `create` e `update` (o `create` falha se o valor já existe?)
  é a confirmar: o service não troca uma pela outra sozinho.
* **`promotion-values/update` e `promotion-items` — alto.** Mudam o preço de venda na loja dentro do período da
  campanha; `promotion-items` põe produtos numa campanha já em andamento.
* Não há rota de "desfazer": a correção é outra escrita. Por isso a ordem (5.12) só libera estas rotas depois das
  leituras de preço, custo e promoção e com a trava de configuração (PR8).

### 5.8 Escritas de lote (só se a empresa usa lote, PR6)

| Método | Rota da API | Rota no controller | Resposta Totvs | Job | Chave |
|---|---|---|---|---|---|
| `PostCreateBatch` | `POST batch/create` | `POST Products/{docCompany}/Batches/Create` | 201 | 13135 | `branchCode|productCode|batchCode` |
| `PutBatchInfo` | `PUT batch-info` | `PUT Products/{docCompany}/BatchInfo` | 200 | 13136 | itens |
| `PostBatchPreferentialSalesOrder` | `POST batch-preferentialsalesorder` | `POST Products/{docCompany}/BatchPreferentialSalesOrder` | 200 (`processingDate`, `deletedRecords`, `insertedRecords`) | 13137 | `branchCode|batchCode|sequence` ou `branchCode|batchItemBarCode` |

* `InsertBatchProductCommand` → `BatchProductModel` (`branchCode*`, `productCode*`, `balanceCode*` 3, `batchCode` 9
  opcional — gerado pela Totvs, textos 40, datas, `batchType`) → `BatchItemModel` (`status`, quantidades 15, `nuance` 20,
  `quantityCone`, `locationBatch` 5, `branchSalesOrder` 4 + `salesOrder` 9 juntos, `typeTare`, `additionalFields`
  (`code*`, `value*`), `invoice` → `BatchInvoiceInspectionModel`: `supplierCode` **ou** `supplierCpfCnpj` (exatamente um,
  regra da descrição), `invoiceCode*`, `inspectionLayout*`, `quantityInspection*`, `inspectionFields*`
  (`field*`, `inspectionValue*`)). Campos de saída (`balance`, `quantityIssued`, `quantityCanceled`) também aparecem no
  modelo de inclusão: não são enviados.
* `UpdateBatchInfoCommand` → `UpdateBatchItemModel`: (`branchCode` + `batchCode` + `sequence`) **ou** `itemClient`
  (regra da descrição) e `dataChanged` (`UpdateBatchInfoModel`, pelo menos um campo).
* `BatchItemPreferentialSalesOrderCommand`: `branchCode*`; (`batchCode` + `sequence`) **ou** `batchItemBarCode` (40);
  `preferentialSalesOrder`: `branchCode*` e `orderCode` ou `orderId` (100) (a confirmar se é um ou outro).
* **Riscos — médio/alto.** Lote mexe em saldo de matéria-prima e inspeção de qualidade. O
  `batch-preferentialsalesorder` **apaga antes todos** os pedidos preferenciais do item, e uma lista vazia só apaga:
  proposta, recusar lista vazia sem `?clear=true`.

### 5.9 O que o swagger mostra para a fase 3 (preços, custos e saldos)

Sem especificar de novo; registro para as partes 4 a 6:

* **`PostSearchPrices` (13015):** `option.prices*` é lista de `PriceInfoModel` com `branchCode*` e `priceCodeList*`
  **por loja** (uma entrada por loja, todas numa chamada) e os indicadores `isPromotionalPrice` e `isScheduledPrice`;
  há ainda `option.digitalPromotionPrices` (`branchCodeList*`, `isInformationOtherDigitalPromotions`). Na resposta:
  `promotionalPrice`, `promotionalInformation`, `informationOtherPromotions` por preço e `digitalPromotionPrices` por
  produto. Pelo filtro, `change.inPrice` **não** pega mudança de promoção: é preciso mandar também `inPromotionalPrice`
  (e `inScheduledPrice`/`inDigitalPromotionPrice`, se forem lidos), cada um exigindo `branchPriceCodeList` e
  `priceCodeList`. `isIgnoreStandardPrice` tira produtos de "produto padrão" das três alterações.
* **`PostSearchCosts` (13016):** `option.costs*` com `branchCode*` e `costCodeList*` por loja. A descrição de
  `change.inCost` manda informar "branchCostCodeList e **priceCodeList**" — provável erro; mandar `costCodeList`.
* **`PostSearchBalances` (13017):** `option.balances*` com `branchCode*`, `stockCodeList*` e indicadores
  `isSalesOrder` (+ `salesOrderStatusList`), `isTransaction`, `isPurchaseOrder`, `isProductionOrder`,
  `isProductionPlanning` (exige `cycleProductionPlanningCodeList`). A resposta traz, além do planejado, `stockTemp`
  (pendente sem os pedidos do Omni) e `measurementConversion` por saldo (lista: filha nova ou fora do escopo). A rota
  `omni-changed-balances` é uma alternativa (PR2, 5.2).
* Os valores de 5.7 dependem destas leituras (trava de variação), por isso vêm depois delas.

### 5.10 Arquivos

| Projeto | Arquivo | Mudança |
|---|---|---|
| `Domain.Totvs` | `Dtos/Reads/Products/Catalogs/*.cs`, `Dtos/Reads/Products/PriceTables/*.cs`, `.../Promotions`, `.../Similars`, `.../Batches` | DTOs das leituras de 5.1 e 5.2 (strings, uma classe por arquivo) |
| `Domain.Totvs` | `Entities/Reads/Products/...` (mesmas pastas) | entidades das tabelas novas |
| `Domain.Totvs` | `Dtos/Writes/Products/<Recurso>/*.cs` | corpos de 5.5 a 5.8 e enums (`ProductValueType` com o conversor que o teste decidir) |
| `Domain.Totvs` | `Enums/EnumJob.cs` | 13080–13097, 13100–13105, 13120–13137 |
| `Application.Totvs` | `Abstractions/Services/ITotvsProductCatalogsService.cs`, `Services/TotvsProductCatalogsService.cs` | novo (PR12): os 12 catálogos de 5.1 |
| `Application.Totvs` | `Abstractions/Services/ITotvsProductsService.cs`, `Services/TotvsProductsService.cs` | leituras de 5.2 e 5.4, escritas de 5.5, 5.6 e 5.8 |
| `Application.Totvs` | `Abstractions/Services/ITotvsProductPricesService.cs`, `Services/TotvsProductPricesService.cs` | novo (PR12): `PostSearchPriceTables`, `GetPromotion`, escritas de 5.7 |
| `Application.Totvs` | `Validations/Reads/Products/...`, `Validations/Writes/Products/...` | validadores |
| `Application.Totvs` | `Handlers/Commands/TotvsCommandHandler.cs` (+ interface) | queries das tabelas novas, loja por `branchCode`, valor atual de preço/custo, promoção por loja |
| `Domain.Totvs` / `Infrastructure.Totvs` | `ITotvsProductCatalogsRepository`/`TotvsProductCatalogsRepository` (novos), `ITotvsProductsRepository`/`TotvsProductsRepository` | bulk, merges e consultas |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Product_Catalogs_Tables.sql`, `Script0NN_Create_Totvs_Product_Catalogs_Sync_Procedures.sql`, `Script0NN_Insert_Totvs_Product_Catalogs_Jobs_Into_Jobs_Table.sql` | 5.1 (procedures a numerar) |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Product_Prices_Tables.sql`, `..._Sync_Procedures.sql`, `..._Jobs_Into_Jobs_Table.sql` | tabelas de preço e promoções |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Product_Similar_And_Batches_Tables.sql`, `..._Sync_Procedures.sql`, `..._Jobs_Into_Jobs_Table.sql` | similares e lotes (lotes só com PR6) |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_On_Demand_Product_Queries_Jobs_Into_Jobs_Table.sql` | 13100–13105 |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_Product_Writes_Jobs_Into_Jobs_Table.sql` | 13120–13137 |
| `Infrastructure.Totvs` | `DependencyInjection.cs` | dois services e um repositório novos |
| Host | `Controllers/Totvs/TotvsController.cs` | rotas de 5.1 a 5.8 (`WriteResponse` da §1.2) |
| Host | `appsettings*.json` | `Totvs:Writes:<Grupo>:Enabled` (PR8), `Totvs:Writes:Values:AllowedCodes` e percentual da trava (PR7) |
| Módulo | `README.md`, `Responsed Jsons/Products/...` | tabelas, jobs, rotas; uma resposta por rota (dados fictícios) |

### 5.11 Testes

1. **Sem API:** console com corpos válidos e inválidos para cada validador (filtro "um ou outro", listas de 500,
   classificação 6000, `prefixEanGtin` vazio, variação de preço); JSON serializado contra o schema (nomes como no swagger,
   inclusive as grafias `branchs`, `aditionalData`; nulos omitidos).
2. **Leituras na API** (não alteram nada): cada catálogo com e sem janela, conferindo `maxChangeFilterDate`; cores sem
   data (confirmar); `GetPromotion` de uma loja, alterando uma promoção na Totvs para ver se a janela a pega (PR15);
   `products/{code}` com `branchCode` no caminho e na query; `kardex-movement` com 181 dias (esperado 400); as duas rotas
   de composição; `price-tables/search` de uma tabela; `omni-changed-balances` × `balances/search` para PR2.
3. **Escritas só em homologação** (§1.3, D1 da §2.6), com dados criados para o teste, nesta ordem: `PostGrids` e `PostColors`
   (cor nova); `PostClassifications` num tipo de teste; `PostReferences` com um SKU (sem `values`), conferindo pelo
   `GetProductByCode`; `PostBarcodes` e `PostUpdateBarcodes` nesse SKU; classificação do produto; `PutProducts` (peso) e
   `PutData` com `productCodeList` de um item; `PostCreateValues`/`PostUpdateValues` num tipo de preço de teste, com o
   `valueType` como letra, nome e número (registrar o aceito); promoção de teste; um corpo com erro de regra para o 400 →
   422.
4. **Pelo host:** rotas com e sem `X-User-Name`, CNPJ inválido, corpo inválido (400), empresa ou loja não cadastrada
   (422), trava de configuração desligada; conferir `totvs.TotvsRequests` e `auditing.*`.

### 5.12 Decisões para aprovar

| # | Pergunta | Proposta |
|---|---|---|
| PR1 | Implementar `products/search`? | Não: é o mesmo dado das referências, por produto (5.3) |
| PR2 | `omni-changed-balances` vira job próprio? | Não; a fase 3 testa as duas rotas e o `PostSearchBalances` usa uma (13096 reservado) |
| PR3 | `product-codes/search` como conferência diária das referências? | Sim, uma vez por dia por marca, recuperando faltantes pelo `GetProductByCode` |
| PR4 | Catálogos: um job por rota ou um job agregador? | Um job por rota (padrão do módulo), todos de 120 min |
| PR5 | A empresa usa tabela de preço (PEDFM011) e promoção (PRDFM002)? | **Respondida (27/10/2026):** implementar mesmo sem uso (T14), a 120 min |
| PR6 | A empresa usa lote (PRDFM059)? | **Respondida (27/10/2026):** implementar `PostSearchBatch` e as escritas da 5.8 mesmo sem uso (T14) |
| PR7 | Liberar escrita de preço/custo? Com que travas? | Só depois de `PostSearchPrices`/`Costs`; lista de `valueCode`/lojas permitidos e trava de variação em configuração |
| PR8 | Trava de configuração por grupo de escrita? | Sim: `Totvs:Writes:<Grupo>:Enabled`, desligada por padrão; desligada devolve 422 sem chamar a API |
| PR9 | `PutData`/`PutBranchInfo` por `groupCode`/`referenceCode` (grupo inteiro)? | Primeira entrega só com `productCodeList`/`barCodeList` (até 500) |
| PR10 | Bloquear escrita na classificação 6000 (marca)? | Sim: 422; mudança de marca só pelo ERP |
| PR11 | `PostReferences` com `values` (preço no cadastro)? | Recusar até PR7 aprovada |
| PR12 | Dividir o `TotvsProductsService`? | Sim: `TotvsProductCatalogsService` (catálogos) e `TotvsProductPricesService` (tabelas de preço, promoções, escritas de valor); o resto fica no atual |
| PR13 | `instruction-items` com `Expand=image,globalImage`? | Não: imagem não entra no banco |
| PR14 | Leituras sem persistência devolvem o JSON da Totvs ao chamador? | Sim, no `Result.data`, e gravam `response_body` em `TotvsRequests` |
| PR15 | Promoções: reler as abertas a cada execução, além da janela? | Sim, até a chamada real mostrar que a janela pega alteração de valor e situação |

### 5.13 Ordem de execução

1. Catálogos sem filhas e de maior uso: `GetClassificationType`, `GetClassifications`, `GetCategory`, `GetGrid`,
   `GetMeasurementUnit`, `PostSearchColors` (o hash), `GetAdditionalFieldsTypes`; depois `PostCompositions`,
   `GetProductGrouperConfiguration`, `GetInstructionItems`. Base para validar as escritas.
2. Sob demanda sem persistência: `GetProducts` (resolver código), `GetKardexMovement`, `GetGridFrequency`,
   `GetMeasurementConversions`, composições.
3. `PostSearchProductCodes` (conferência, PR3) e `GetSimilar`.
4. Fase 3 (preços, custos e saldos, com 5.9), se ainda não feita.
5. Se PR5: `GetPriceTablesHeaders`, `GetPriceTableScales`, `PostSearchPriceTables`, `GetPromotion`.
6. Escritas de menor risco (depois da base de escrita da §1 e de D1/D2 da §2.6): `PostGrids`, `PostColors`, `PostClassifications`,
   `PostBarcodes`/`PostUpdateBarcodes`, classificação do produto.
7. Dados do produto: `PutProducts`, `PutData`, `PutBranchInfo` (com PR9).
8. `PostReferences` (sem `values`).
9. Preço, custo e promoção (5.7), com PR7 e PR8, por último.
10. Se PR6: `PostSearchBatch` e as escritas de lote.
11. README, `Responsed Jsons`, proposta; teste 4 de 5.11 pelo host.

---

## 6. Purchase Order

Swagger oficial: `https://apitotvsmoda.bhan.com.br/api/totvsmoda/purchase-order/v2/swagger/index.html`, versão **2.8.06**
(a mesma da fase 2; nenhuma rota nova ou removida).

| Rota | Situação |
|---|---|
| `POST search` | Implementada (`PostSearchPurchaseOrders`, job 13004) |
| `POST orders` | **Falta** — inclusão de pedido de compra (§6.1) |
| `POST quantity-items` | **Falta** — quantidade solicitada dos itens (§6.2) |
| `POST orders/change-status` | **Falta** — situação do pedido (§6.3) |
| `POST orders/cancel` | **Falta** — cancelamento (§6.4) |
| `POST purchase-order/batch-link` | **Falta** — vínculo de item de lote com pedido e nota (§6.5) |

As cinco são **escritas** (fluxo da §1.2). Métodos no `TotvsPurchaseOrdersService`.

**Nomes.** A regra literal daria `PostOrders`, `PostOrdersCancel`, `PostOrdersChangeStatus` e `PostQuantityItems`, mas a
API Sales Order tem as mesmas rotas (`orders/cancel`, `orders/change-status`, `quantity-items`), e os nomes dos jobs no
`EnumJob` colidiriam. Como o método existente já leva o recurso (`POST search` → `PostSearchPurchaseOrders`), os novos
também levam:

| Rota | Método | Rota no controller | Job | Chave em `TotvsRequests` | Efeito no ERP | Risco |
|---|---|---|---|---|---|---|
| `POST orders` | `PostPurchaseOrders` | `POST PurchaseOrders/{docCompany}` | 13180 `TotvsPostPurchaseOrders` | `branchCode|orderCode` (da resposta); sem resposta, `branchCode|supplierOrderCode` | cria um pedido de compra | **Alto**: compromisso com fornecedor, não idempotente (uma repetição cria outro pedido) |
| `POST quantity-items` | `PostPurchaseOrdersQuantityItems` | `POST PurchaseOrders/{docCompany}/QuantityItems` | 13181 `TotvsPostPurchaseOrdersQuantityItems` | `branchCode|orderCode` | troca a quantidade solicitada de itens | **Médio**: muda o que se espera receber; reversível com nova chamada |
| `POST orders/change-status` | `PostPurchaseOrdersChangeStatus` | `POST PurchaseOrders/{docCompany}/ChangeStatus` | 13182 `TotvsPostPurchaseOrdersChangeStatus` | `branchCode|orderCode` | Em andamento ↔ Bloqueado | **Baixo**: reversível |
| `POST orders/cancel` | `PostPurchaseOrdersCancel` | `POST PurchaseOrders/{docCompany}/Cancel` | 13183 `TotvsPostPurchaseOrdersCancel` | `branchCode|orderCode` | cancela o pedido | **Alto**: sem rota para desfazer (**a confirmar** se o ERP reabre pedido cancelado) |
| `POST purchase-order/batch-link` | `PostPurchaseOrderBatchLink` | `POST PurchaseOrders/{docCompany}/BatchLink` | 13184 `TotvsPostPurchaseOrderBatchLink` | `branchCode|orderCode|batchCode|sequence` ou `branchCode|orderCode|batchItemBarCode` | cria ou atualiza o vínculo de um item de lote | **Médio**: mexe no rastreio de lote do recebimento; sobrescreve vínculo existente |

As rotas novas não colidem com `POST PurchaseOrders` (o job, sem parâmetro). 13185 fica reservado para a decisão DP4;
13186–13189 livres.

**Comum às cinco**
* Passo 3 da §1.2: o `branchCode` do corpo precisa ser uma loja da empresa em `totvs.TotvsBranches` (o corpo traz
  código, não CNPJ: a consulta é `GetBranchCodes`, que já existe).
* Nas quatro que mexem num pedido existente (6.2 a 6.5), conferência local **só quando o pedido já está em
  `totvs.TotvsPurchaseOrders`** (a gravação pode estar até 3 min atrasada), como a D4 da §2.6: situação incompatível volta
  **422** sem chamar a API. Pedido ausente do banco: deixa a Totvs validar.
* Persistência: nenhuma além de `TotvsRequests`. O pedido alterado chega pela próxima execução do
  `PostSearchPurchaseOrders` (até 3 min).
* Situações do pedido na leitura (`StatusOrderType`, gravado pelo nome em `TotvsPurchaseOrders.status`): 1 `InProgress`,
  2 `LowReleased`, 3 `PartiallyReceived`, 4 `Attended`, 5 `Blocked`, 6 `Canceled`, 7 `Distributed`, 8 `PreOrder`.

### 6.1 `PostPurchaseOrders` — `POST purchase-order/v2/orders`

| Item | Especificação |
|---|---|
| Corpo | `InsertPurchaseOrderCommand` (abaixo) |
| Resposta da Totvs | **201** `PurchaseOrderResponseModel` (`branchCode`, `orderCode` do pedido criado); 400 `DomainNotificationMessage[]` |

**DTOs (`Dtos/Writes/PurchaseOrders`)**

| Classe | Campos (`*` obrigatório no swagger) |
|---|---|
| `PurchaseOrder` | `branchCode*` (até 4), `supplierCode` (até 9) **ou** `supplierCpfCnpj` (até 14), `buyerCode` (até 9) **ou** `buyerCpfCnpj` (até 14), `operationCode*` (até 4), `paymentConditionCode*` (até 4), `status*` (`StatusOrderCreateType`), `orderDate`, `supplierOrderCode` (até 20), `paymentType` (`PaymentType`), `shippingCompanyCode` (até 9) ou `shippingCompanyCpfCnpj` (até 14), `freightType` (`FreightType`), `deliveryForecastDate`, `deliveryDeadlineDate`, `basePaymentDate`, `totalAmountOrder*` (12 inteiros, 2 decimais), `items*`, `classifications`, `observations`, `expenses`, `discounts`, `additionalFields`, `additionalData` |
| `Item` | `productCode*` (até 9) / `productSku` (até 40), `value*` (9 + 6 decimais), `discountPercentage` (3 + 6), `ipiPercentage` (3 + 6), `quantity*` (9 + 3), `deliveryForecastDate`, `deliveryDeadlineDate`, `itemsValues*`, `itemsExpenses` |
| `ItemValue` | `valueType` (1: `C` custo, `P` preço), `valueCode` (até 4), `value` (3 + 6) |
| `ItemExpense` / `Expense` | `expenseCode` (até 9), `expensePercentage` (3 + 6), `costCenter` (até 9) — mesmo formato no item e no pedido |
| `Classification` | `classificationTypeCode` (até 4), `classificationCode` (até 10) |
| `Observation` | `observation` (até 80), `visualizationType` (`VisualizationObservationType`) |
| `Discount` | `discountTypeCode` (até 3), `discountPercentage` (3 + 6) |
| `AdditionalField` | `code`, `value` (string, número ou data `DD/MM/YYYY`: tipo `object?` no DTO) |
| `AdditionalData` | `currencyCode`, `currencyRateValue`, `costCenter`, `maximumExtraQuantityPercentage`, `maximumExtraQuantity`, `settleType` (`OrderSettleType`), `accessoryExpensePercentage`, `freightPercentage`, `insurancePercentage`, `interstatePurchaseReason`, `taxRuleCode` (sem tamanhos no swagger) |
| Enums (número da descrição, §1.3) | `StatusOrderCreateType` (1 Em andamento, 5 Bloqueado), `PaymentType` (1 Duplicata, 2 Boleto, 12 Depósito), `FreightType` (1 CIF, 2 FOB, 3 Terceiros, 4 Sem ocorrência, 5 Próprio remetente, 6 Próprio destinatário), `VisualizationObservationType` (1 Interna, 2 Fornecedor, 3 Alerta), `OrderSettleType` (0 Total e parcial, 1 Somente total) |

Os enums opcionais (`paymentType`, `freightType`, `visualizationType`, `settleType`) ficam anuláveis no DTO para serem
omitidos: o `OrderSettleType` tem o valor **0**, e um enum não anulável mandaria "Total e parcial" sem ninguém pedir.

**Validações (`PurchaseOrderValidator` e filhos)**
* obrigatórios e tamanhos do swagger; CPF/CNPJ só com dígitos (11 ou 14);
* **exatamente um** entre `supplierCode` e `supplierCpfCnpj`, e entre `buyerCode` e `buyerCpfCnpj` (descrição do swagger);
  **no máximo um** entre `shippingCompanyCode` e `shippingCompanyCpfCnpj`;
* `status` só 1 ou 5;
* pelo menos um item; cada item com `productCode` **ou** `productSku` (o schema marca `productCode` obrigatório, mas a
  descrição permite só o `productSku`: o DTO usa `int?`, como o `proratedPercentage` da §2.1), `quantity` > 0, `value` ≥ 0 e
  pelo menos um `itemsValues` (obrigatório no schema);
* produto sem repetição entre os itens (proposta; **a confirmar** se a API aceita o mesmo produto duas vezes);
* `valueType` só `C` ou `P`;
* classificação com os dois campos ou nenhum (descrição do swagger);
* `additionalFields.value` só string, número ou data no formato `dd/MM/yyyy` (o único campo de data fora do
  `yyyy-MM-ddTHH:mm:ss`);
* **não validado localmente:** a conferência do `totalAmountOrder` com a soma dos itens e descontos (a descrição diz que a
  Totvs confere, sem dar a fórmula: IPI, descontos em cascata e despesas), a soma dos rateios de despesa (o swagger não
  diz que é 100), e se `valueType`/`valueCode` estão no parâmetro `DS_LST_VLR_PED_COMPRA` do ERP. Tudo isso volta como
  400 → 422.

**Service:** fluxo da §1.2. Antes de chamar a API, se vier `supplierOrderCode`, confere em `totvs.TotvsPurchaseOrders` se
já há pedido com o mesmo `branchCode` + fornecedor + `supplierOrderCode` e devolve 422 (proteção contra a repetição de uma
inclusão que deu 502; decisão DP2). O `orderCode` da resposta vai no `Result`, na mensagem do log e na chave do
`TotvsRequests`.

**Riscos:** cria pedido real no ERP (com observação visível ao fornecedor se `visualizationType = 2`); **a confirmar** se a
inclusão gera previsão no contas a pagar pela condição de pagamento (efeito financeiro). É o corpo maior e o de mais regras
que só a Totvs conhece (operação, condição, comprador, valores permitidos).

### 6.2 `PostPurchaseOrdersQuantityItems` — `POST purchase-order/v2/quantity-items`

| Item | Especificação |
|---|---|
| Corpo | `RequestedQuantity` (`UpdateItemResquestedQuantityCommand`; a grafia "Resquested" é a do swagger, só no nome do schema): `branchCode`, `orderCode`, `items` (`RequestedQuantityItem`: `productCode`, `quantity`) |
| Resposta da Totvs | **200** `PurchaseOrderResponseModel` (`branchCode`, `orderCode`); 400 |

**Validações:** o swagger não marca nada como obrigatório; localmente `branchCode` e `orderCode` > 0, pelo menos um item
(`items` é anulável no schema, mas sem itens a chamada não faz nada), `productCode` > 0 e sem repetição, `quantity` ≥ 0
(tamanho 9 + 3 do `quantity` da inclusão).

**Service:** fluxo da §1.2. Com o pedido no banco: situação `Canceled` ou `Attended` → 422; cada `productCode` precisa
estar em `TotvsPurchaseOrdersProducts` do pedido, e a nova quantidade não pode ser menor que `settledQuantity` (já
recebida). **A confirmar com chamada real:** se a rota inclui um produto que não está no pedido, se `quantity = 0` cancela o
item, e se aceita quantidade abaixo da recebida.

### 6.3 `PostPurchaseOrdersChangeStatus` — `POST purchase-order/v2/orders/change-status`

| Item | Especificação |
|---|---|
| Corpo | `OrderStatus` (`UpdateOrderStatusCommand`): `branchCode`, `orderCode`, `status` (`StatusOrderChange`: 1 Em andamento, 5 Bloqueado) |
| Resposta da Totvs | **200** `PurchaseOrderResponseModel`; 400 |

**Validações:** `branchCode`, `orderCode` > 0; `status` só 1 ou 5 (obrigatório localmente, embora o swagger não marque).

**Service:** fluxo da §1.2. Com o pedido no banco: só `InProgress` → 5 e `Blocked` → 1; situação igual à pedida ou fora
dessas duas (recebido, atendido, cancelado...) → 422. **A confirmar** se a Totvs aceita bloquear um pedido
`PartiallyReceived` ou `LowReleased`; se aceitar, a conferência local afrouxa.

### 6.4 `PostPurchaseOrdersCancel` — `POST purchase-order/v2/orders/cancel`

| Item | Especificação |
|---|---|
| Corpo | `CancelOrder` (`CancelOrderCommand`): `branchCode`, `orderCode` |
| Resposta da Totvs | **200** `SuccessProcessingModel` (`processingDate`); 400 |

**Validações:** `branchCode`, `orderCode` > 0.

**Service:** fluxo da §1.2. Com o pedido no banco: `Canceled` → 422 (já cancelado); `Attended`, `PartiallyReceived` ou
`Distributed` → 422 (já há recebimento; **a confirmar** o que a Totvs faz com pedido parcialmente recebido: cancela o
saldo ou recusa). A rota não tem motivo de cancelamento no corpo.

**Riscos:** irreversível pela API; um `orderCode` errado cancela o pedido de outro fornecedor. Proposta (DP3): exigir no
corpo da **nossa** rota o `supplierCode` ou `supplierCpfCnpj` esperado, conferido com o banco antes de chamar (não vai para a
Totvs), quando o pedido está no banco.

### 6.5 `PostPurchaseOrderBatchLink` — `POST purchase-order/v2/purchase-order/batch-link`

| Item | Especificação |
|---|---|
| Corpo | `BatchLink` (`PurchaseOrderBatchLinkInCommand`): `branchCode*` (empresa do lote, até 4), `batchCode` (até 9) + `sequence` (até 4) **ou** `batchItemBarCode` (até 40), `orderCode` (sem descrição no swagger), `invoice` (`InvoiceIdentification`: `accessKey` (44), `serialCode` (3), `invoiceCode` (`long`, 9)) |
| Resposta da Totvs | **200** `SuccessProcessingModel`; **400**, **404** e **409** `DomainNotificationMessage[]` |

**Validações**
* `branchCode` obrigatório;
* **exatamente um** entre o par `batchCode` + `sequence` (os dois juntos) e `batchItemBarCode` (a descrição diz "obrigatório
  se informar branchCode, sequence" e "não informar junto com batchItemBarCode");
* `orderCode` > 0 (proposta: o vínculo é com pedido; o swagger não marca nem descreve, **a confirmar**);
* `invoice`, se vier: `accessKey` com 44 dígitos **ou** `serialCode` + `invoiceCode` (proposta: o swagger não diz se são
  alternativos, **a confirmar**).

**Service:** fluxo da §1.2, sem conferência de pedido: o `branchCode` é a empresa do **lote**, e o corpo não diz a empresa
do pedido (**a confirmar** se a Totvs procura o pedido na mesma empresa). 404 e 409 não estão na tabela da §1.2; proposta
(DP5): 404 → **404** (lote, pedido ou nota não encontrado) e 409 → **409** (conflito de vínculo), com as mensagens da Totvs.

**Riscos:** "criar **ou atualizar**": uma chamada com o item de lote já vinculado troca o vínculo sem aviso. Não há leitura de
lote no módulo para conferir antes, nem uso conhecido de lote de compra na Newbloomers (decisão DP6).

### 6.6 Arquivos

| Projeto | Arquivo | Mudança |
|---|---|---|
| `Domain.Totvs` | `Dtos/Writes/PurchaseOrders/*.cs` | novos: as classes de 6.1 a 6.5 e os 6 enums (`StatusOrderCreateType`, `StatusOrderChange`, `PaymentType`, `FreightType`, `VisualizationObservationType`, `OrderSettleType`) |
| `Domain.Totvs` | `Enums/EnumJob.cs` | 13180–13184 |
| `Application.Totvs` | `Validations/Writes/PurchaseOrders/*.cs` | validadores |
| `Application.Totvs` | `ITotvsPurchaseOrdersService` / `TotvsPurchaseOrdersService` | cinco métodos |
| `Application.Totvs` | `ITotvsCommandHandler` / `TotvsCommandHandler` | queries: pedido por chave (situação, fornecedor), itens do pedido (`settledQuantity`), pedido por `supplierOrderCode` |
| `Domain.Totvs` / `Infrastructure.Totvs` | `ITotvsPurchaseOrdersRepository` / `TotvsPurchaseOrdersRepository` | as consultas acima |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_Purchase_Orders_Writes_Jobs_Into_Jobs_Table.sql` | jobs 13180–13184 (intervalo 0) |
| Host | `Controllers/Totvs/TotvsController.cs` | cinco rotas `POST`; 404/409 no `WriteResponse` se aprovada a DP5 |
| Módulo | `README.md`, `Responsed Jsons/PurchaseOrders/` | escritas; exemplos de corpo e resposta (dados fictícios) |

### 6.7 Testes

1. **Sem API:** console com corpos válidos e inválidos por validador; JSON serializado contra o schema (enums como número,
   enums opcionais omitidos, `additionalFields.value` em `dd/MM/yyyy`, datas `yyyy-MM-ddTHH:mm:ss`).
2. **Na API de homologação** (§1.3; nunca em produção), com fornecedor, operação e condição de teste:
   * `PostPurchaseOrders` com um item e um `itemsValues`, `status = 5` (bloqueado, para não seguir para recebimento);
     conferir pelo `search`; um `totalAmountOrder` errado para ver o 400;
   * `PostPurchaseOrdersChangeStatus` 5 → 1 → 5, e 5 → 5;
   * `PostPurchaseOrdersQuantityItems` aumentando e reduzindo; `quantity = 0`; um produto fora do pedido;
   * `PostPurchaseOrderBatchLink` só se houver lote de teste (DP6): com barras, com lote + item, e um inexistente (404);
   * `PostPurchaseOrdersCancel` no pedido de teste, e de novo no mesmo (o 400 de já cancelado);
   * o enum enviado como nome, uma vez.
3. **Pelo host:** as cinco rotas com e sem `X-User-Name`, CNPJ inválido, corpo inválido (400), loja de outra empresa (422);
   conferir `totvs.TotvsRequests` e `auditing.*`.

### 6.8 Decisões para aprovar

| # | Pergunta | Proposta |
|---|---|---|
| DP1 | Expor as cinco escritas, ou só as que algum sistema vai usar? | **Respondida (27/10/2026):** as cinco, mesmo sem consumidor (T14), na ordem das fases (§9.4): change-status, quantity-items e inclusão, depois lote e cancelamento |
| DP2 | Tornar `supplierOrderCode` obrigatório na nossa rota e recusar repetição pelo banco? | Sim: é a única chave do lado do cliente para evitar pedido duplicado depois de um 502 |
| DP3 | No cancelamento, exigir o fornecedor esperado e conferir com o banco? | Sim, quando o pedido está no banco |
| DP4 | Criar uma leitura sob demanda `GetPurchaseOrderByKey` (`search` com `branchCodeList` + `orderCodeList`, como `GetSalesOrderByKey`) para conferir uma escrita na hora? | Sim, job 13185: serve depois de um 502 e para o sistema que escreveu ver o resultado sem esperar 3 min. Não é rota da documentação que falta, é complemento |
| DP5 | Repassar 404 e 409 do `batch-link` como 404/409 (a §1.2 só prevê 400 → 422)? | Sim, estendendo a tabela da §1.2 |
| DP6 | A Newbloomers usa lote no recebimento de compra? | **Respondida (27/10/2026):** implementar o `batch-link` mesmo sem uso (T14) |
| DP7 | Conferências locais de situação (6.2 a 6.4) só com o pedido no banco? | Sim, como a D4 da §2.6 |

### 6.9 Ordem de execução

1. Base de escrita da §1 (se ainda não entregue com o Accounts Payable);
2. `PostPurchaseOrdersChangeStatus` (corpo mínimo, reversível);
3. `PostPurchaseOrdersQuantityItems`;
4. `GetPurchaseOrderByKey` (se aprovada a DP4), antes da inclusão;
5. `PostPurchaseOrders` (o maior corpo; os testes dos passos 2 e 3 passam a usar o pedido criado aqui);
6. `PostPurchaseOrdersCancel` (por último entre as do pedido: irreversível; testa no pedido do passo 5);
7. `PostPurchaseOrderBatchLink` (se aprovada a DP6);
8. README, `Responsed Jsons`; teste 3 de 6.7 pelo host.

---

## 7. Sales Order

Swagger oficial: `https://apitotvsmoda.bhan.com.br/api/totvsmoda/sales-order/v2/swagger/index.html`, versão **2.8.33**
(a mesma da fase 2; nenhuma rota nova ou removida). Os métodos novos ficam no `TotvsSalesOrdersService`. Base de escrita:
§1 (fluxo da §1.2, respostas da §1.2, pontos da §1.3).

| # | Rota | Situação | Tipo proposto | Método | Job |
|---|---|---|---|---|---|
| 1 | `POST orders/search` | **Implementada** (`PostSearchSalesOrders`, job 13005; `GetSalesOrderByKey`, job 13012) | — | — | — |
| 2 | `GET discount-type` | Falta | Catálogo | `GetDiscountType` | 13190 |
| 3 | `GET classifications` | Falta | Catálogo | `GetClassifications` | 13191 |
| 4 | `GET batch-items` | Falta | Leitura recorrente (cadastro, 120 min) | `GetBatchItems` | 13192 |
| 5 | `GET changed-batch-items` | Falta | Leitura recorrente (3 min) | `GetChangedBatchItems` | 13193 |
| 6 | `GET billing-suggestions` | Falta | Leitura recorrente (3 min) | `GetBillingSuggestions` | 13194 |
| 7 | `GET invoices` | Falta | Sob demanda, **sem persistência** | `GetInvoices` | 13195 |
| 8 | `GET pending-items` | Falta | Sob demanda, **sem persistência** | `GetPendingItems` | 13196 |
| 9 | `POST b2c-orders` | Falta | Escrita (inclusão de pedido) | `PostB2COrders` | 13197 |
| 10 | `POST orders/cancel` | Falta | Escrita | `PostOrdersCancel` | 13198 |
| 11 | `POST orders/change-status` | Falta | Escrita | `PostOrdersChangeStatus` | 13199 |
| 12 | `POST header` | Falta | Escrita | `PostHeader` | 13200 |
| 13 | `POST additional-order` | Falta | Escrita | `PostAdditionalOrder` | 13201 |
| 14 | `POST shipping-order` | Falta | Escrita | `PostShippingOrder` | 13202 |
| 15 | `POST observations-order` | Falta | Escrita | `PostObservationsOrder` | 13203 |
| 16 | `POST pdv-transactions` | Falta | Escrita | `PostPdvTransactions` | 13204 |
| 17 | `POST items` | Falta | Escrita | `PostItems` | 13205 |
| 18 | `DELETE items` | Falta | Escrita | `DeleteItems` | 13206 |
| 19 | `POST additional-items` | Falta | Escrita | `PostAdditionalItems` | 13207 |
| 20 | `POST quantity-items` | Falta | Escrita | `PostQuantityItems` | 13208 |
| 21 | `POST price-items` | Falta | Escrita | `PostPriceItems` | 13209 |
| 22 | `POST cancel-items` | Falta | Escrita | `PostCancelItems` | 13210 |
| 23 | `POST relationship-counts` | Falta | Escrita | `PostRelationshipCounts` | 13211 |
| 24 | `POST relationship-batch-items` | Falta | Escrita | `PostRelationshipBatchItems` | 13212 |
| 25 | `DELETE batch-items` | Falta | Escrita | `DeleteBatchItems` | 13213 |
| 26 | `POST suggest-batch-items-nuance` | Falta | Escrita | `PostSuggestBatchItemsNuance` | 13214 |

Contagem: 1 implementada; faltam 3 leituras recorrentes, 2 catálogos, 2 sob demanda sem persistência e 18 escritas.
Jobs 13215–13249 ficam livres.

**Nomes.** O método segue a decisão 18 ao pé da letra (fica dentro do `TotvsSalesOrdersService`). No `EnumJob`, que é
um só para o módulo, `orders/cancel` e `quantity-items` (Purchase Order), `classifications` (Person, Product) e
`invoices` (Fiscal) colidiriam; por isso **todos** os jobs desta API levam `SalesOrder` depois do verbo:
`TotvsGetSalesOrderDiscountType`, `TotvsPostSalesOrderB2COrders`, `TotvsPostSalesOrderOrdersCancel`,
`TotvsDeleteSalesOrderItems`... (decisão SO9).

**O que o `orders/search` já cobre.** O job 13005 lê com o `expand` completo e grava `TotvsSaleOrdersInvoices` (mesmos
campos do `GET invoices`), `TotvsSaleOrdersProducts` (com `pendingQuantity`), `TotvsSaleOrdersProductsBatchItems`,
`TotvsSaleOrdersClassifications`, `TotvsSaleOrdersDiscounts` e `TotvsSaleOrdersCounts`. Isso pesa na classificação
abaixo: `invoices` e `pending-items` não ganham tabela.

**Chave do pedido nas escritas.** 16 das 18 escritas identificam o pedido por **`orderId` ou `branchCode` + `orderCode`**
(um ou outro, nunca os dois; `branchCode` exige `orderCode` e vice-versa). Um DTO base `OrderKey` (`branchCode` `int?`
até 4 dígitos, `orderCode` `int?` até 9, `orderId` até 100) e um `OrderKeyValidator` servem a todos. As exceções são
`relationship-counts` e `relationship-batch-items`, que só aceitam `branchCode` + `orderCode`.

---

### 7.1 Catálogos

| Rota | Rota no controller | Parâmetros | Tabela (chave) | Job / intervalo |
|---|---|---|---|---|
| `GET discount-type` | `GET TotvsJobs/Totvs/SalesOrderDiscountTypes` | `Page`, `PageSize` (máx. 1000) | `TotvsSaleDiscountTypes` (`doc_company`, `code`) | 13190, 120 min |
| `GET classifications` | `GET TotvsJobs/Totvs/SalesOrderClassifications` | `Page`, `PageSize` (máx. 1000) | `TotvsSaleClassifications` (`doc_company`, `typeCode`, `code`) | 13191, 120 min |

* **Na implementação (§10.9)**, pela T9, os dois seguem a janela do banco (a resposta traz `maxChangeFilterDate` com o filtro),
  não o hash abaixo.
* **Leitura inteira a cada execução** (domínios pequenos), sem `StartChangeDate`/`EndChangeDate`: o desenho "só regrava
  quando muda" das comissões, com hash do JSON de cada item em cache (`Totvs:SaleDiscountTypesHash:<cnpj>`,
  `Totvs:SaleClassificationsHash:<cnpj>`). Item novo ou com hash diferente vai para `untreated` e merge; o resto é
  descartado. Item que some da resposta **fica** na tabela (regra dos registros principais).
* `TotvsSaleDiscountTypes`: `code` int, `description` (sem tamanho no swagger: `nvarchar(100)`, a confirmar),
  `percentage` decimal(18,6), `maxChangeFilterDate` (vem nulo sem o filtro de alteração; a coluna fica para quando for
  usado).
* `TotvsSaleClassifications`: uma linha por classificação, com o tipo repetido: `typeCode` int, `typeDescription` (40),
  `type` (`Order`/`OrderItem`, nvarchar(20)), `code` (10), `description` (40), `maxChangeFilterDate`. O nome evita
  confusão com a filha `TotvsSaleOrdersClassifications` (classificações de cada pedido). **A confirmar:** se um tipo sem
  classificação vem com `code` nulo (quebraria a chave; nesse caso a linha é descartada e registrada em
  `auditing.Messages`).
* A rota não tem filtro de loja: uma chamada por empresa (`doc_company`).
* `discount-type` declara **404** além do 400: tratar 404 como lista vazia (a confirmar com chamada real).

### 7.2 Leituras recorrentes

| Rota | Rota no controller | Janela | Tabelas (chave) | Job / intervalo | Carga inicial |
|---|---|---|---|---|---|
| `GET batch-items` | `GET TotvsJobs/Totvs/SalesOrderBatches` | `StartChangeDate`/`EndChangeDate` | `TotvsSaleOrderBatches` (`doc_company`, `code`) | 13192, 120 min | desde 01/01/2000, numa chamada (cadastro pequeno) |
| `GET changed-batch-items` | `GET TotvsJobs/Totvs/SalesOrderChangedBatchItems` | `StartDate`/`EndDate` | `TotvsSaleOrderChangedBatchItems` (`doc_company`, `branchCode`, `orderCode`, `productCode`, `batchCode`, `sequence`) | 13193, 3 min | desde 01/01/2025 (igual aos pedidos), fatias de **1 dia** |
| `GET billing-suggestions` | `GET TotvsJobs/Totvs/BillingSuggestions` | `StartChangeDate`/`EndChangeDate`, `Expand=items` | `TotvsBillingSuggestions` (`doc_company`, `branchCode`, `suggestionCode`), `TotvsBillingSuggestionsOrders` (+ `orderBranchCode`, `orderCode`), `TotvsBillingSuggestionsOrdersItems` (+ `productCode`) | 13194, 3 min | desde 01/01/2025, fatias de 30 dias |

Fluxo das leituras recorrentes do README (janela pelo maior `maxChangeFilterDate`/`changeDate` gravado, menos 1 hora;
cache por empresa; validação; bulk insert + `sp_<Tabela>_Sync`; `auditing.Records`).

**`GET batch-items` (lotes de pedido).** Apesar do caminho, devolve o **lote de pedido** do PEDFM018 (`code` 10,
`description` 60, `status` int 1 Aberto / 2 Fechado, `startDate`, `endDate`, `closingDate`, `maxChangeFilterDate`), o
mesmo código que vai em `batchOrder` da capa do pedido. Não é o "item de lote" de produto do `DELETE batch-items`.
`Status` não é enviado (vêm abertos e fechados). Cache `Totvs:SaleOrderBatches:<cnpj>`. Declara 404: lista vazia.

**`GET changed-batch-items`: é incremental por alteração, com ressalvas.** Conferido no swagger:
* filtra por `StartDate`/`EndDate` ("Data de alteração") e cada item traz `changeDate`: serve de janela;
* **não pagina** (só `items`, sem `count`/`hasNext`) e **não tem `maxChangeFilterDate`**: a janela sai do
  `MAX(changeDate)` gravado, e as fatias precisam ser curtas (1 dia; reduzir se a resposta vier grande);
* `BranchCode`, `OrderCode` e `BatchCode` estão como opcionais. **A confirmar com chamada real:** se a rota aceita só
  as datas (sem loja) e se devolve itens de todas as lojas. Se exigir `BranchCode`, uma chamada por loja da empresa;
  se exigir `OrderCode`, a rota não serve como leitura recorrente e passa a sob demanda;
* o título fala em "itens de lote associados a pedidos atualizados": a lista parece ser a dos vínculos **atuais** de
  pedidos alterados na janela, sem sinal de vínculo removido (a confirmar).
* Colunas: as chaves int, `itemBatchKey` e `itemBar` (sem tamanho no swagger: `nvarchar(100)`, a confirmar),
  `changeDate`. **Proposta (SO2):** além de gravar, juntar os pedidos distintos da resposta e reler cada lote de até 500
  por `orders/search` com `orderCodeList` (mesmo método de gravação do job 13005), porque a alteração de um vínculo de
  lote pode não mudar a data de alteração do pedido, e a `TotvsSaleOrdersProductsBatchItems` ficaria desatualizada.

**`GET billing-suggestions`.** Paginada (máx. 1000), com `maxChangeFilterDate`. Não há filtro de lista de lojas
(`BranchCode` só junto com `SuggestionCode`): **a confirmar** se vem de todas as lojas do usuário da API; o service
descarta as sugestões de lojas fora de `TotvsBranches` da empresa. `statusSuggestion` vem como **número** (1, 4, 6), não
pelo nome: coluna int. Filhas: `orders` (quantidades e valores solicitados, sugeridos e pendentes) e `orderItems`
(`description`, `standardBarCode`, `sizeName`, `colorName`, quantidades, `price`; tamanhos dos textos a confirmar,
proposta igual a `TotvsSaleOrdersProducts`: 60/40/10/40). Merge com exclusão das filhas pelo horário do lote. Declara
404: lista vazia. **A confirmar:** se `Expand=items` traz `orderItems` (o `orders` parece vir sempre).

**Uso das três.** Lote de pedido, item de lote e sugestão de faturamento são do fluxo de atacado/expedição. Antes de
implementar, conferir no banco se `TotvsSaleOrdersProductsBatchItems`, `TotvsSaleOrdersCounts` e
`TotvsSaleOrders.batchOrder` têm dados (decisão SO2).

### 7.3 Leituras sob demanda (sem persistência)

| Rota | Rota no controller | Parâmetros enviados | Resposta da rota | Chave em `TotvsRequests` |
|---|---|---|---|---|
| `GET invoices` | `GET TotvsJobs/Totvs/SalesOrders/{docCompany}/{branchCode}/{orderCode}/Invoices` | `BranchCode`, `OrderCode`; opcional `invoiceStatus` (lista: `Canceled`, `Denied`, `Issued`, `Normal`, `Excluded`) → `InvoiceStatusType` | **200** com o `OrderInvoiceOutDto` da Totvs (notas, transação e NF-e); 404 sem notas; 422 empresa não cadastrada | `branchCode|orderCode` |
| `GET pending-items` | `GET TotvsJobs/Totvs/SalesOrders/{docCompany}/{branchCode}/{orderCode}/PendingItems` | `BranchCode`, `OrderCode` | **200** com o `OrderPendingItemOutDto` (itens com `pendingQuantity`, `grossPrice`, `itemDiscount`, `orderDiscount`, `netPrice`) | `branchCode|orderCode` |

* Fluxo das leituras sob demanda do README, mas **sem gravar** nas tabelas: o service devolve o DTO lido.
* **Por que sem persistência:** os dados já chegam pelo `orders/search` (`TotvsSaleOrdersInvoices`, `pendingQuantity` em
  `TotvsSaleOrdersProducts`). Gravar só a filha, sem o pai, quebra a regra do merge (a exclusão das filhas vale pelo
  horário do lote do pai). Os preços líquidos do `pending-items` são consulta do momento. Quem precisa do banco
  atualizado chama `GET SalesOrders/{docCompany}/{branchCode}/{orderCode}` (`GetSalesOrderByKey`).
* `X-User-Name` obrigatório, `{docCompany}` `^\d{14}$`, códigos `int:min(1)`. As rotas têm mais segmentos que a do
  `GetSalesOrderByKey`, sem colisão.
* `MaxChangeFilterDate` aparece como parâmetro de consulta nas duas rotas, sem descrição: **não é enviado**.
* `orderId` como alternativa de chave: decisão SO4.

### 7.4 Escritas

Todas seguem o fluxo da §1.2: validação no controller (400), empresa (passo 2), **passo 3: o `branchCode` do corpo é
uma loja da empresa em `totvs.TotvsBranches`** (por código, não por CNPJ; com `orderId`, o passo 3 é pulado ou feito
pelo `orderId` em `TotvsSaleOrders`, quando o pedido já está no banco), sem nova tentativa em 5xx, registro em
`TotvsRequests` com `request_body`. Resposta da Totvs: **200** `SuccessProcessingModel` (`processingDate`), exceto o
`b2c-orders` (**201** `OrderOutDto`); 400 `DomainNotificationMessage[]`. Chave em `TotvsRequests`: `branchCode|orderCode`
ou `orderId`, mais o que está indicado. Os `DELETE` mandam os campos como **parâmetros de consulta** (não há corpo):
o controller recebe `[FromQuery]`, e `request_body` grava o JSON desses parâmetros.

Depois da escrita o banco **não é atualizado na hora**: o pedido volta pelo job 13005 (até 3 min), como na SO3 do
Accounts Payable (decisão SO10). **A confirmar:** se toda alteração abaixo muda a data de alteração usada pelo
`filter.change` do `orders/search`.

#### Tabela-resumo

| Método | Rota no controller | Corpo (`*` obrigatório) | Efeito no ERP | Risco | Idempotente? | Chave extra em `TotvsRequests` |
|---|---|---|---|---|---|---|
| `PostB2COrders` | `POST SalesOrders/{docCompany}/B2C` | `OrderInDto` (§7.4.1) | **Cria pedido de venda** (reserva, financeiro, pagamento) | **Alto** | Só pelo `orderId` (único) — pré-checagem (SO5) | `branchCode|orderId` |
| `PostOrdersCancel` | `POST SalesOrders/{docCompany}/Cancel` | chave; `reasonCancellationCode*` (4); `ReasonCancellationDescription` (80) | **Cancela o pedido inteiro** (sem volta pela API) | **Alto** | Sim no efeito (segundo cancelamento deve dar 400) | — |
| `PostOrdersChangeStatus` | `POST SalesOrders/{docCompany}/Status` | chave; `statusOrder` (`UpdateStatusOrderType`: 1, 2, 5, 7, 8); `reasonBlockingCode` (4); `reasonBlockingDescription` (80) | Muda a situação; **2 = liberado para faturamento** | Médio/alto | Sim | `|statusOrder` |
| `PostHeader` | `POST SalesOrders/{docCompany}/Header` | chave; `averagePaymentPeriod` (até 6); `operationCode` (4); `priceTableCode` (9); `priorityCode` (2) | Altera capa: operação, tabela de preço, prazo, prioridade | Médio/alto | Sim | — |
| `PostAdditionalOrder` | `POST SalesOrders/{docCompany}/Additional` | chave; `shippingService` (10), `trackingId` (40), `ecommerceStage` (40), `omniSiteId`, `compensationDays` (3), `anticipationDay` (3), `entryType` (`EarlydeliveryType`), `experienceType` | Dados adicionais (rastreio, estágio do e-commerce) | Baixo | Sim | — |
| `PostShippingOrder` | `POST SalesOrders/{docCompany}/Shipping` | chave; transportadora e redespacho (código **ou** CPF/CNPJ), `freightType`, `redispatchingFreightType`, `freightPercentage`, `freightValue`, `shippingAddress` (`cep` 8, `publicPlace` 10, `address` 60, `number` 5, `complement` 65) | Transporte e frete (**muda o total**) | Médio | Sim | — |
| `PostObservationsOrder` | `POST SalesOrders/{docCompany}/Observations` | chave; `observations` (`observation` 80, `visualizationType`), `invoiceObservations` (`observation` 80) | **Inclui** linhas de observação | Baixo | **Não**: repetir duplica as linhas (a confirmar) | — |
| `PostPdvTransactions` | `POST SalesOrders/{docCompany}/PdvTransactions` | chave; `transactionId*` (100), `transactionBranchCode` (4), `transactionCode` (9), `transactionDate` | Liga o pedido a uma venda do TOTVS Moda PDV (o filtro `hasPdvTransaction` do search trata como **faturado pelo PDV**) | **Alto** | A confirmar (por `transactionId`) | `|transactionId` |
| `PostItems` | `POST SalesOrders/{docCompany}/Items` | chave; `items` (`AddOrderItemsInDto`, mesmos campos do item do B2C sem `outsourceds`) | **Inclui itens** (valor e reserva) | **Alto** | **Não** (repetir: erro ou soma, a confirmar) | — |
| `DeleteItems` | `DELETE SalesOrders/{docCompany}/Items` | query: chave; `ProductCode` **ou** `ProductSku` (40) | **Remove um item** | Médio/alto | Sim no efeito | `|productCode ou productSku` |
| `PostAdditionalItems` | `POST SalesOrders/{docCompany}/Items/Additional` | chave; `items` (`productCode` ou `productSku`, `rollMeasureCode` 9, `billingForecastDate`) | Previsão de faturamento e medida de rolo do item | Baixo | Sim | — |
| `PostQuantityItems` | `POST SalesOrders/{docCompany}/Items/Quantity` | chave; `items` (`productCode` ou `productSku`, `quantity` 9,3 — **nova quantidade, sempre maior que a atual**) | Aumenta quantidade (valor e reserva) | Médio/alto | Sim (valor absoluto; repetir dá 400 por não ser maior) | — |
| `PostPriceItems` | `POST SalesOrders/{docCompany}/Items/Price` | chave; `items` (`productCode` ou `productSku`, `price` 9,6, obrigatório pela descrição) | **Altera preço** (valor do pedido) | **Alto** | Sim (valor absoluto) | — |
| `PostCancelItems` | `POST SalesOrders/{docCompany}/Items/Cancel` | chave; `items` (`productCode` ou `productSku`, `cancelQuantity` 9,3, `reasonCancellationCode` 4, `reasonCancellationDescription` 80) | **Cancela quantidade** de itens | **Alto** | **Não**: `cancelQuantity` é delta; repetir cancela de novo | — |
| `PostRelationshipCounts` | `POST SalesOrders/{docCompany}/Counts` | `branchCode`, `orderCode`; `counts` (`branchCountCode` 4, `countCode` 9, `branchSuggestionCode` 4, `suggestionCode` 9) | Liga contagens (conferência) ao pedido | Médio | A confirmar | — |
| `PostRelationshipBatchItems` | `POST SalesOrders/{docCompany}/BatchItems` | `branchCode*`, `orderCode*`; `items*` (`code*` produto, `isUpdateQuantity`, `batchItems*`: `branchCode`+`code`+`sequence` **ou** `barCode` (40), `quantity*`) | Liga itens de lote ao item; com `isUpdateQuantity` **altera a quantidade do pedido** | Médio/alto | **Não** (item de lote já ligado dá erro) | — |
| `DeleteBatchItems` | `DELETE SalesOrders/{docCompany}/BatchItems` | query: chave; `ProductCode` ou `ProductSku`; `BatchSequence` (9) | Remove vínculo de item de lote | Médio | Sim no efeito | `|produto|batchSequence` |
| `PostSuggestBatchItemsNuance` | `POST SalesOrders/{docCompany}/BatchItems/SuggestNuance` | só a chave | O ERP **escolhe e grava** os lotes pelo nuance de cor | Médio/alto | **Não** (resultado decidido pelo ERP) | — |

Validações comuns: `OrderKeyValidator`; produto com **exatamente um** de `productCode` (até 9) e `productSku` (até 40);
listas `items` não vazias e sem produto repetido; tamanhos e casas decimais do swagger; enums só com os valores
listados; códigos de pessoa com **um** entre código e CPF/CNPJ (só dígitos, 11 ou 14).

**Trava de repetição (SO6).** Para as escritas não idempotentes (`PostB2COrders`, `PostItems`, `PostObservationsOrder`,
`PostCancelItems`, `PostRelationshipBatchItems`, `PostSuggestBatchItemsNuance`, `PostRelationshipCounts`,
`PostPdvTransactions`): antes de chamar a API, o service procura em `TotvsRequests` a mesma rota, `doc_company`,
`field_key_value` e `request_body` nos últimos 10 minutos. Achou com sucesso: **409** sem chamar a API. Achou com falha
5xx/exceção: **409** com a mensagem "a escrita anterior pode ter sido feita; confira o pedido antes de repetir". O
índice `IX_TotvsRequests_Company_Key` já atende a consulta.

#### 7.4.1 `PostB2COrders` — `POST sales-order/v2/b2c-orders`

| Item | Especificação |
|---|---|
| Rota do controller | `POST TotvsJobs/Totvs/SalesOrders/{docCompany}/B2C` |
| Corpo | `OrderInDto` (abaixo) |
| Resposta da Totvs | **201** `OrderOutDto` (`branchCode`, `orderCode`, `orderId` do pedido criado), que vai no `Result` e na mensagem do log; 400 |
| Job | 13197 `TotvsPostSalesOrderB2COrders` |
| Chave em `TotvsRequests` | `branchCode|orderId` |

**DTOs (`Dtos/Writes/SalesOrders/B2COrders`)**

| Classe (schema) | Campos (`*` obrigatório de fato) |
|---|---|
| `B2COrder` (`OrderInDto`) | `orderId*` (100), `branchCode*` (4), `orderDate*`, `customerCode` (9) **ou** `customerCpfCnpj` (14)*, `representativeCode` (9) **ou** `representativeCpfCnpj` (14)*, `operationCode*` (4), `paymentConditionCode*` (4), `priorityCode*` (2), `statusOrder*` (`StatusOrderInputType`), `totalAmountOrder*` (12,2), `items*`; opcionais: `customerOrderCode` (100), `integrationCode` (100, só em split de pedido, junto com `payments.paymentType`), intermediário, guia e vendedor (código **ou** CPF/CNPJ; vendedor até 6 dígitos), `paymentBaseDate`, `arrivalDate`, transportadora e redespacho (código **ou** CPF/CNPJ), `redispatchingFreightType` (1 Emitente, 2 Destinatário), `billingForecastDate`, `billingLimitDate`, `freightType`, `freightPercentage` (3,6), `freightValue` (12,2), `packageNumber` (9,3), `weight` (9,3), `reasonBlockingCode` (4), `omniSiteId`, `shippingService` (10), `experienceType`, `chargeType` (padrão 0 Carteira), `priceTableCode` (9), `additionalValue` (12,2), `batchOrder` (10), `receivingPerson` (100), `rollMeasureCode` (9), `buyerCustomerCode` (9), `buyerCustomerCpfCnpj` (14); listas abaixo; `shippingAddress` |
| `B2COrderItem` (`ItemInDto`) | `productCode` (9) **ou** `productSku` (40)*, `quantity*` (9,3), `price*` (9,3); `originalPrice` (9,6), `discountPercentage` (3,6), `discountValue`, `billingForecastDate`, `billingStartDate`, `customerOrderCode` (9), `customerProductCode` (20), `rollMeasureCode` (9), `sequentials` (cada 100), `sequentialsValues` (`code` 100, `freightValue`, `discountValue` 14,2), `groupObservations` (`observation` 80), `outsourceds`, `commissioneds` |
| `B2COrderDiscount` (`TypeDiscountInDto`) | `typeDiscountCode*` (3), `discountValue` (9,6) **ou** `discountPercentage` (9,3) |
| `B2COrderClassification` | `classificationTypeCode*` (4), `classificationCode*` (10) |
| `B2COrderPayment` (`PaymentInDto`) | `documentType*` (`DocumentInputType`), `installment*` (3), `paymentValue*` (12,2, total do pagamento, não da parcela); cartão (`CreditCard`/`DebitCard`): `nsu` (20), `authorizationCode` (20), `creditCardOperator` (60), `creditCardBrand` (60) obrigatórios; `alphanumericNsu` (100), `assignorCode` (4), `documentNumber` (10), operador financeiro (código **ou** CNPJ), `currentAccountCode` (9), `paymentType` (`PaymentType`), `paymentBranch` (4) **ou** `paymentBranchCnpj` (14), obrigatório com `SplitOmni`, `ourNumber` (18) |
| `B2COrderObservation`, `B2COrderRepresentativeObservation` | `observation` (80, obrigatório na manual), `visualizationType` |
| `B2COrderInvoiceObservation` | `observation*` (80) |
| `B2COrderShippingAddress` | `cep*` (8), `cityName` (60), `stateAbbreviation` (3), `publicPlace` (10), `address`, `number` (5), `neighborhood` (60), `complement` (65) |
| `B2COrderGift` (`OrderGiftInDto`) | `productCode` **ou** `productSku`, `quantity*` (9,3), `price*` (9,3) |
| `B2COrderCommissioned`, `B2COrderItemCommissioned` | representante (código **ou** CPF/CNPJ), `commissionedType*` (`CommissionedType`), `commissionPercentageBilling` e `commissionPercentageReceipt` (3,4; 0 quando não há) |
| `B2COrderOutsourced`, `B2COrderItemOutsourced` | endereço (`cep*`...), pessoa (código **ou** CPF/CNPJ), `operationCode*` (4) na capa / `distributionQuantity*` no item |
| `B2COrderBillingPercentageLimitDate` | `billingForecastDate`, `billingPercentageLimit` (5,2), `limitType` (`LimitType`) |
| Enums | `FreightType` (1 a 6; o swagger grafa `FreitghtType`), `StatusOrderInputType` (1, 5, 8), `ExperienceType` (1 a 5), `ChargeType` (0, 16, 17, 18, 28), `DocumentInputType` (1, 2, 3, 4, 5, 13, 16, 18, 20, 23, 24, 25, 26, 27), `PaymentType` (0 a 3), `VisualizationObservationType` (1, 2), `CommissionedType` (1, 2, 3, 90), `LimitType` (1, 2) |

**Validações (`B2COrderValidator` e filhos):** obrigatórios **pela descrição**: o schema marca os dois lados de
cliente, representante, produto e desconto como obrigatórios, mas a descrição manda **exatamente um**; transportadora
exige `freightType`, e o redespacho exige `redispatchingFreightType` (e vice-versa); pagamento com cartão exige NSU,
autorização, operadora e bandeira; `SplitOmni` exige a empresa do financeiro; `integrationCode` só com algum
`paymentType`; `sequentials`/`sequentialsValues` com tantos códigos quanto `quantity`; `statusOrder` `Blocked` exige
`reasonBlockingCode` (a confirmar); **conferência de valores**: soma dos `paymentValue` igual a `totalAmountOrder`
(tolerância de 0,01). A conferência da soma dos itens fica com a Totvs (descontos de capa e de item, frete e despesa
acessória, e arredondamento da conversão de valor em percentual).

**Service:** fluxo da §1.2. Passo 3: `branchCode` é loja da empresa. **Pré-checagem (SO5):** `orders/search` com
`branchCodeList` + `orderIdList` (`pageSize` 1); se o `orderId` já existe, **422** com a chave do pedido existente, sem
incluir. Isso também resolve a dúvida depois de um 502: repetir a chamada só inclui se o primeiro envio não gravou.
Clientes, representantes, operação, condição e tabela de preço não são conferidos localmente (a Totvs valida).

**Riscos:** cria um pedido real (estoque reservado, financeiro do pagamento). Teste só em homologação (§1.3).

#### 7.4.2 Regras que só aparecem na descrição (outras escritas)

* **`PostOrdersCancel`:** irreversível pela API. Conferência local (SO7): pedido no banco com `statusOrder` `Canceled`
  → 422 sem chamar; `Attended` (atendido) → 422 (a confirmar se a Totvs recusa `PartiallyAnswered`). O campo é
  `ReasonCancellationDescription`, **com R maiúsculo** no swagger: o DTO usa `[JsonProperty]` com a grafia exata.
* **`PostOrdersChangeStatus`:** `statusOrder` só 1, 2, 5, 7, 8 (não há 3, 4, 6: atendimento e cancelamento são de
  outras rotas); `Blocked` com `reasonBlockingCode` (a confirmar se obrigatório). `BillingReleased` libera para
  faturar: risco de o pedido seguir para nota.
* **`PostHeader`:** `operationCode` e `priceTableCode` só com o pedido em 1 (`InProgress`) ou 5 (`Blocked`): conferência
  local quando o pedido está no banco (SO7). `averagePaymentPeriod` é `int` **não anulável** no swagger: o DTO usa
  `int?` e omite nulos; **a confirmar** se omitir mantém o prazo atual ou grava 0 (SO8). "Enviar string vazia para
  limpar" o `priceTableCode`, que é inteiro: fora desta versão. Pelo menos um campo além da chave.
* **`PostAdditionalOrder`:** pelo menos um campo além da chave. `entryType` é `N`/`S` (letras, não número): enum
  serializado como **letra** (a confirmar; SO11).
* **`PostShippingOrder`:** mesmas regras de transportadora/redespacho do B2C; pelo menos um campo além da chave.
* **`PostQuantityItems`:** a quantidade é a **nova** e precisa ser maior que a atual; reduzir é pelo `PostCancelItems`.
  Conferência local contra `TotvsSaleOrdersProducts.quantity` quando o item está no banco.
* **`PostCancelItems`:** `cancelQuantity` > 0 e, quando o item está no banco, não maior que `pendingQuantity`; trava de
  repetição obrigatória.
* **`PostPriceItems`:** `price` > 0. Risco financeiro: sem conferência local de preço; o `Result` leva o corpo enviado
  para auditoria.
* **`PostRelationshipBatchItems`:** em cada item de lote, **exatamente um** de (`branchCode` + `code` + `sequence`) e
  `barCode` (o schema marca os quatro como obrigatórios). `isUpdateQuantity` padrão `false`; com `true`, a quantidade do
  pedido muda: o validador não bloqueia, mas o log registra.
* **`PostRelationshipCounts`:** o schema não marca nada como obrigatório; o validador exige `branchCode`, `orderCode` e
  pelo menos uma contagem com `branchCountCode` e `countCode`; sugestão (`branchSuggestionCode` + `suggestionCode`)
  aos pares.
* **`DeleteItems` / `DeleteBatchItems`:** o swagger expõe `LastChangeUser` (sem descrição) e
  `ValidationResult.IsValid/Errors/RuleSetsExecuted` (vazamento do modelo interno): **nenhum é enviado**.
* **`PostPdvTransactions`:** pelo filtro `hasPdvTransaction` do search, o pedido passa a constar como faturado pelo PDV.
  Só faz sentido para quem integra o PDV; fica fora da primeira entrega (SO1).

### 7.5 Arquivos

| Projeto | Arquivo | Mudança |
|---|---|---|
| `Domain.Totvs` | `Dtos/Reads/SalesOrders/DiscountType.cs`, `SaleClassification.cs`, `OrderBatch.cs`, `ChangedBatchItem.cs`, `BillingSuggestion*.cs`, `OrderInvoices.cs`, `PendingItem*.cs` | DTOs de leitura (`string`), um por arquivo; `GET invoices` reusa `Invoice`/`Electronic` existentes |
| `Domain.Totvs` | `Entities/Reads/SalesOrders/...` | entidades das 7 tabelas novas |
| `Domain.Totvs` | `Dtos/Writes/SalesOrders/OrderKey.cs` e `*.cs` por rota, `B2COrders/*.cs`, enums | DTOs de escrita (§1.1) |
| `Domain.Totvs` | `Enums/EnumJob.cs` | 13190–13214 |
| `Application.Totvs` | `Validations/Reads/SalesOrders/*.cs` | validadores dos DTOs de leitura |
| `Application.Totvs` | `Validations/Writes/SalesOrders/*.cs` | `OrderKeyValidator`, `B2COrderValidator` e filhos, um por comando |
| `Application.Totvs` | `Abstractions/Services/ITotvsSalesOrdersService.cs`, `Services/TotvsSalesOrdersService.cs` | 25 métodos |
| `Application.Totvs` | `Handlers/Commands/TotvsCommandHandler.cs` (+ interface) | loja por código; situação, quantidade e pendente do pedido/item; máximos de alteração das tabelas novas; consulta da trava de repetição em `TotvsRequests` |
| `Domain.Totvs` / `Infrastructure.Totvs` | `ITotvsSalesOrdersRepository` / `TotvsSalesOrdersRepository`, `ITotvsRepository` / `TotvsRepository` | bulk insert e merges das tabelas novas; consultas acima |
| `Infrastructure.Totvs` | `Scripts/SQLServer/Script0NN_Create_Totvs_Sales_Orders_Catalogs_Tables.sql` | `TotvsSaleDiscountTypes`, `TotvsSaleClassifications`, `TotvsSaleOrderBatches` (`untreated` + `totvs`) |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Sales_Orders_Batch_And_Suggestions_Tables.sql` | `TotvsSaleOrderChangedBatchItems`, `TotvsBillingSuggestions` e filhas (SO2) |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Sales_Orders_Catalogs_Sync_Procedures.sql` | `sp_<Tabela>_Sync` (ids de procedure a numerar, 13500+) |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_Sales_Orders_Reads_Jobs_Into_Jobs_Table.sql` | jobs 13190–13196 (e procedures) |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_Sales_Orders_Writes_Jobs_Into_Jobs_Table.sql` | jobs 13197–13214, intervalo 0 |
| Host | `Controllers/Totvs/TotvsController.cs` | 5 rotas de job, 2 `GET` sob demanda, 16 `POST` e 2 `DELETE` de escrita |
| Host | registro dos jobs recorrentes no Hangfire | 13190–13194 |
| Módulo | `README.md`, `Responsed Jsons/` | seções novas; exemplos de resposta e de corpo (dados fictícios) |

### 7.6 Testes

1. **Sem API:** console com corpos válidos e inválidos para cada validador (atenção aos "um ou outro" e às regras de
   cartão/split do B2C); JSON serializado conferido com o swagger (nomes exatos, inclusive `ReasonCancellationDescription`;
   enums como número; `entryType` como letra; datas `yyyy-MM-ddTHH:mm:ss`; nulos omitidos).
2. **Leituras na API de produção** (só leitura): `discount-type` e `classifications` inteiros; `batch-items` com e sem
   janela; `changed-batch-items` só com datas, depois com `BranchCode` (confirma obrigatoriedade, tamanho da resposta e
   se traz todas as lojas); `billing-suggestions` com e sem `Expand=items`; `invoices` e `pending-items` de um pedido
   conhecido; guardar os JSONs em `Responsed Jsons`.
3. **Escritas só em homologação** (§1.3), com pedidos criados para o teste, nesta ordem:
   * `PostB2COrders` com um item e um pagamento em dinheiro; conferir pelo `orders/search` com `orderIdList`; repetir o
     mesmo `orderId` (pré-checagem → 422; e uma vez direto na API para ver o que a Totvs faz);
   * `PostAdditionalOrder`, `PostShippingOrder`, `PostObservationsOrder` (duas vezes: confirma se duplica), `PostHeader`
     sem `averagePaymentPeriod` (confirma SO8);
   * `PostItems` (e o mesmo produto de novo), `PostQuantityItems` (maior e menor), `PostPriceItems`, `PostCancelItems`
     parcial, `DeleteItems`;
   * `PostOrdersChangeStatus` 1 → 5 → 1; `PostOrdersCancel`, e cancelar de novo;
   * um corpo com erro de regra para ver o 400 → 422; um enum enviado pelo nome, uma vez;
   * conferir se cada alteração aparece no próximo `PostSearchSalesOrders` (o `filter.change` pega a mudança?).
   * Lote, contagem e PDV só se aprovados na SO1/SO2.
4. **Pelo host:** cada rota com e sem `X-User-Name`, CNPJ inválido, corpo inválido (400), empresa ou loja não cadastrada
   (422), trava de repetição (409); conferir `totvs.TotvsRequests` e `auditing.*`.

### 7.7 Decisões para aprovar

| # | Pergunta | Proposta |
|---|---|---|
| SO1 | Quais das 18 escritas entram? Há um sistema que vai usá-las (e-commerce, OMS)? | **Respondida (27/10/2026):** as 18, mesmo sem consumidor (T14), em ondas (§9.4) |
| SO2 | Lote de pedido, item de lote, contagem e sugestão de faturamento (3 leituras e 5 escritas) são usados? | **Respondida (27/10/2026):** implementar as 8 rotas mesmo sem uso (T14); o `changed-batch-items` também relê os pedidos alterados |
| SO3 | `GET invoices` e `GET pending-items` sem persistência? | Sim: os dados já vêm no `orders/search`, e gravar a filha sem o pai quebra o merge |
| SO4 | Sob demanda por `orderId` (código do e-commerce) além de `branchCode` + `orderCode`? | Só `branchCode` + `orderCode` nesta versão, como o `GetSalesOrderByKey`; `orderId` por query string quando houver consumidor |
| SO5 | B2C: conferir o `orderId` antes de incluir? | Sim, sempre (uma busca a mais), porque não há nova tentativa em 5xx e o `orderId` é o único controle de duplicidade |
| SO6 | Trava de repetição (409) pelas chamadas em `TotvsRequests` para as escritas não idempotentes? | Sim, 10 minutos, mesma rota + chave + corpo |
| SO7 | Conferir localmente situação/quantidade antes de cancelar, alterar capa ou quantidade? | Só quando o pedido já está no banco (até 3 min atrasado), como a D4 da §2.6 |
| SO8 | `averagePaymentPeriod` não anulável no `header` | `int?` omitido quando nulo; o primeiro teste confirma se a omissão mantém o prazo |
| SO9 | Nomes dos jobs | Métodos com o nome literal da decisão 18; jobs com `SalesOrder` depois do verbo, por colisão com Purchase Order, Person, Product e Fiscal |
| SO10 | Reler o pedido logo após a escrita? | Não: esperar o job 13005 (3 min); o `Result` do B2C já traz a chave criada |
| SO11 | Enums no corpo | Número (D6 da §2.6), exceto `EarlydeliveryType` (`N`/`S`), que vai como letra; `ChargeType` tem nomes com acento no swagger: só número |

### 7.8 Ordem de execução

1. Catálogos `GetDiscountType` e `GetClassifications` (só leitura, tabelas pequenas; validam o desenho de hash);
2. `GetInvoices` e `GetPendingItems` (sob demanda sem persistência; sem tabela);
3. Peças comuns de escrita desta API: `OrderKey` + validador, loja por código, trava de repetição (SO6);
4. Escritas de baixo risco: `PostAdditionalOrder`, `PostAdditionalItems`, `PostObservationsOrder`, `PostShippingOrder`;
5. Situação e capa: `PostOrdersChangeStatus`, `PostHeader`;
6. Itens: `PostQuantityItems`, `PostPriceItems`, `PostItems`, `DeleteItems`, `PostCancelItems`;
7. `PostB2COrders` (corpo maior; depende da pré-checagem) e `PostOrdersCancel` (testada sobre pedidos do passo anterior);
8. Lote e sugestão de faturamento (SO2): `GetBatchItems`, `GetChangedBatchItems`, `GetBillingSuggestions`, depois `PostRelationshipBatchItems`,
   `DeleteBatchItems`, `PostSuggestBatchItemsNuance`, `PostRelationshipCounts`; `PostPdvTransactions` (SO1: implementada mesmo sem consumidor);
9. README, `Responsed Jsons`, proposta; teste 4 de 7.6 pelo host.

### 7.9 O que há de estranho no swagger

* Os schemas de inclusão (`OrderInDto`, `ItemInDto`, `AddOrderItemsInDto`, `TypeDiscountInDto`, `RelationshipBatchItemModel`)
  marcam **os dois lados** de cada "um ou outro" como obrigatórios.
* `GET batch-items` devolve **lotes de pedido** (PEDFM018) e `DELETE batch-items` remove **vínculo de item de lote**:
  mesmo caminho, recursos diferentes. O código do lote é `string` (10) num e `integer` no `changed-batch-items`.
* `changed-batch-items` não pagina e não tem `maxChangeFilterDate`; todos os parâmetros são opcionais.
* `GET`/`DELETE` expõem `ValidationResult.*` como parâmetros; `LastChangeUser` e `MaxChangeFilterDate` sem descrição.
* Grafias: `ReasonCancellationDescription` (R maiúsculo), `InsuranceValue` (I maiúsculo, já tratado na leitura),
  `FreitghtType`; o redespacho cita `redispatchingShippingCompanyCnpj`, que não existe (é `...CpfCnpj`).
* Enum `RedispatchingFreightType` (CIF/FOB) declarado, mas o campo é `integer` com 1 Emitente / 2 Destinatário.
* `ChargeType` com nomes acentuados (`CobrançaNaLojaEmpresa`, `Exportação`); `DocumentInputType` lista `Paypal` antes
  de `ReceiptCheck`, mas a descrição numera 18 (cheque presente) antes de 16 (Paypal): o mapeamento por nome é
  16 = `Paypal`, 18 = `ReceiptCheck` (a confirmar).
* Tamanhos implausíveis: `omniSiteId` "2000 números"; `distributionQuantity` (double) "4 números";
  `billingForecastDate` "14 caracteres"; `discountValue` do item "3 inteiros e 2 decimais" (até 999,99 de desconto
  total no item).
* `header.priceTableCode` é inteiro, mas a descrição manda "string vazia para limpar".
* `statusSuggestion` e o `status` do lote vêm como número, ao contrário dos outros enums da API (que vêm pelo nome).
* `discount-type`, `billing-suggestions` e `batch-items` declaram 404 (provavelmente lista vazia).

---

## 8. Fiscal

Swagger oficial: `https://apitotvsmoda.bhan.com.br/api/totvsmoda/fiscal/v2/swagger/index.html`, versão **2.8.18** (a mesma
da fase 2: nenhuma rota nova ou removida). Os métodos novos ficam no `TotvsInvoicesService` (um service por API, como o
`TotvsAccountsPayableService`), inclusive o centro de custo (decisão F7).

| Rota | Situação | Tipo proposto |
|---|---|---|
| `POST invoices/search` | Implementada (`PostSearchInvoices`, job 13003; `GetInvoiceByKey`, job 13013) | — |
| `GET invoices/disable` | **Falta** (§8.2) | Leitura recorrente |
| `GET cost-center` | **Falta** (§8.3) | Catálogo |
| `GET xml-contents/{accessKey}` | **Falta** (§8.4) | Sob demanda, sem persistência (arquivo) |
| `POST danfe-search` | **Falta** (§8.5) | Sob demanda, sem persistência (arquivo); POST, mas é consulta |
| `POST transaction/print` | **Falta** (§8.6) | Sob demanda, sem persistência (arquivo); POST, mas é consulta |
| `GET invoices/item-detail-search` | **Falta** (§8.7) | Sob demanda, sem persistência (proposta) |
| `GET invoices/pending-conditional-products` | **Falta** (§8.8) | Sob demanda, sem persistência (saldo do momento) |
| `POST invoice-products/search` | **Falta** (§8.9) | Sob demanda, sem persistência |
| `POST invoices/manifestations` | **Falta** (§8.10) | Escrita (risco alto; implementada mesmo sem caso de uso — F1/T14) |
| `GET digital-certificates` | **Falta** (§8.11) | **Não expor** (F2) |

A contagem prévia do topo do plano ("7 leituras, 3 escritas") muda: `danfe-search` e `transaction/print` são consultas, e
`digital-certificates` sai do escopo. Fica **8 leituras + 1 escrita + 1 não exposta**.

Os `GET` do swagger declaram os parâmetros de query `ValidationResult.IsValid`, `ValidationResult.Errors` e
`ValidationResult.RuleSetsExecuted`: são vazamento do modelo interno da Totvs e **nunca são enviados**.

### 8.1 Base das leituras sem persistência (nova, comum às §8.4 a §8.9)

Hoje toda leitura sob demanda grava e devolve `true`/`false`/`null`. Seis rotas da Fiscal devolvem o conteúdo a quem
chamou, sem tabela. Elas reusam o `Result` da §1.1 (`success`, `statusCode`, `message`, `data`) e o fluxo das leituras sob
demanda do README, com estas diferenças:

| Item | Regra |
|---|---|
| Nova tentativa | Como nas leituras recorrentes: 401, 429 e 502/503/504 (são consultas, idempotentes, inclusive os dois `POST`) |
| `TotvsRequests` | Uma linha por chamada à Totvs. Rotas de arquivo gravam em `response_body` só um **resumo** (`{"processingType":"Authorized","mainInvoiceXmlLength":18234}`), nunca o conteúdo; as demais gravam o JSON inteiro |
| `auditing.Records` | Não grava (não há registro integrado) |
| Resposta da rota | 200 com o conteúdo; 404 quando a Totvs responde 404 ou lista vazia; 422 para empresa não cadastrada e para 400 da Totvs (com as mensagens); 429; 502 para 5xx ou JSON inválido |

### 8.2 `GetInvoicesDisable` — `GET fiscal/v2/invoices/disable` (leitura recorrente)

Notas inutilizadas (faixas de numeração inutilizadas na SEFAZ, componente FISFC012).

| Item | Especificação |
|---|---|
| Rota do controller | `GET TotvsJobs/Totvs/InvoicesDisable` (dispara todas as empresas) |
| Parâmetros | `StartDate*`, `EndDate*` (data/hora de alteração), `BranchCodeList*` (lojas da empresa), `Page`, `PageSize` (máx. 100), `Order` = `maxChangeFilterDate` |
| Resposta da Totvs | 200 `InvoiceDisableDataResultModel` (envelope `count`/`totalPages`/`hasNext`/`totalItems`/`items`, igual ao `Page<T>`) |
| Job | **13250** `TotvsGetInvoicesDisable` |
| Intervalo | **120 min** proposto (volume baixo, inutilização não é urgente para o ETL); 3 min se o uso pedir (F4) |
| Janela | `MAX(maxChangeFilterDate)` − 1 h, em fatias de 30 dias; carga inicial desde **01/01/2025** (igual às notas) |
| Cache | `Totvs:InvoicesDisable:<cnpj>` (chave → registro), descarta o que não mudou |

**DTO `Dtos/Reads/Invoices/InvoiceDisable.cs`** (todos `string`, como os `Dtos/Reads`): `branchCode` (até 4), `branchCnpj`
(14), `initialInvoiceNumber` (até 9), `finalInvoiceNumber` (até 9), `serialCode` (3), `documentType` (até 2, anulável),
`receipt` (até 15, `long`), `receivementDate`, `maxChangeFilterDate`.

**Tabelas** `untreated.TotvsInvoicesDisable` e `totvs.TotvsInvoicesDisable`, sem filhas.
Chave: `doc_company + branchCode + documentType + serialCode + initialInvoiceNumber`. `serialCode` e `documentType` são
anuláveis no swagger: a entidade grava `''` e `0` quando vierem nulos, para caberem na PK. O `documentType` entra na chave
porque NF-e (55) e NFC-e (65) têm numeração própria e podem usar a mesma série (a confirmar com chamada real). Os registros
nunca são apagados (uma inutilização não se desfaz). Procedure `totvs.sp_TotvsInvoicesDisable_Sync` (a numerar).

**Validação** (`InvoiceDisableValidator`): `branchCode`, `initialInvoiceNumber`, `finalInvoiceNumber` numéricos e
`initialInvoiceNumber <= finalInvoiceNumber`; `branchCnpj` com 14 dígitos.

**A confirmar com chamada real:** o formato do array na query (`BranchCodeList=1&BranchCodeList=2`, padrão do ASP.NET); o
tamanho da URL com todas as lojas da holding (se passar de ~2 mil caracteres, dividir as lojas em blocos); se há limite no
intervalo de datas (as notas aceitam 6 meses; as fatias de 30 dias ficam abaixo de qualquer limite parecido); se o
`maxChangeFilterDate` vem sempre preenchido (o swagger o marca anulável).

### 8.3 `GetCostCenter` — `GET fiscal/v2/cost-center` (catálogo)

Centros de custo (componente GECFM001). É o domínio do `costCenterCode` das despesas do `PostDuplicates` (§2.1), que
poderia ser conferido localmente depois desta rota.

| Item | Especificação |
|---|---|
| Rota do controller | `GET TotvsJobs/Totvs/CostCenters` (dispara todas as empresas) |
| Parâmetros | `StartChangeDate*`, `EndChangeDate*`, `IsInactive` (não enviado: ativos e inativos, a confirmar), `Page`, `PageSize` (máx. **1000**) |
| Resposta da Totvs | 200 `CostCenterResultModel`: `code`, `description`, `class` (`ClassType`: Cost/Expense), `classType` (`ClassTypeType`: Direct/Indirect/Administrative/IndirectSelling), `indirectType` (`IndirectType`: FactoryIndirect/OtherIndirect), `isInactive`, `classifications[]` (`typeCode`, `typeDescription`, `code`, `description`) |
| Job | **13251** `TotvsGetCostCenter`, 120 min |
| Janela | **Leitura inteira a cada execução**: `StartChangeDate` = 01/01/2000, `EndChangeDate` = agora |

**Por que catálogo e não incremental:** a rota exige o filtro de alteração, mas a resposta **não traz `maxChangeFilterDate`**,
então a janela não pode sair do banco como nas outras leituras. O domínio é pequeno (dezenas a poucas centenas, a
confirmar), cabe em uma ou duas páginas de 1000.

**"Só regrava quando muda":** hash SHA-256 do JSON de cada centro (com as classificações), como as comissões
(`_logger.ComputeSha256Hash`). Diferença proposta (F5): o hash fica numa coluna `json_hash` de `totvs.TotvsCostCenters`
e o cache `Totvs:CostCentersHash:<cnpj>` é montado **a partir do banco**. Nas comissões o cache nasce vazio e expira em 2 h
absolutas: com o job de 3 min, todos os fechamentos são regravados uma vez a cada 2 h (e a cada reinício do host). Com o
intervalo de 120 min dos centros de custo, o cache expiraria entre quase todas as execuções, e tudo seria regravado sempre.

**Tabelas**
| Tabela | Chave | Colunas |
|---|---|---|
| `TotvsCostCenters` | `doc_company + code` | `description`, `class`, `classType`, `indirectType` (enums pelo nome, `nvarchar(30)`), `isInactive`, `json_hash` |
| `TotvsCostCentersClassifications` | `doc_company + costCenterCode + typeCode + code` | `typeDescription`, `description` (o `code` do pai vira `costCenterCode` para não colidir) |

A filha é apagada pelo merge quando o pai é regravado (regra geral). Um centro excluído na Totvs fica no banco (a rota
não informa exclusão; o `isInactive` cobre a desativação).

**A confirmar com chamada real:** se `StartChangeDate` = 2000 traz centros nunca alterados (data de atualização vazia);
se `IsInactive` omitido traz os dois; o nome dos enums na resposta (o swagger mostra nomes e números).

### 8.4 `GetXmlContents` — `GET fiscal/v2/xml-contents/{accessKey}` (arquivo, sem persistência)

| Item | Especificação |
|---|---|
| Rota do controller | `GET TotvsJobs/Totvs/ElectronicInvoices/{docCompany}/{accessKey}/Xml` |
| Parâmetros | `accessKey*` (44 dígitos) |
| Resposta da Totvs | 200 `InvoiceXmlOutDto`: `processingType` (`ElectronicInvoiceStatusType`), `mainInvoiceXml`, `cancelInvoiceXml` (só para canceladas); 404 quando a nota não existe ou não está Autorizada/Denegada/Cancelada |
| Resposta da rota | 200 com o mesmo JSON (quem chama escolhe o XML); 404 |
| Job | **13252** `TotvsGetXmlContents` (sob demanda) |
| Chave em `TotvsRequests` | `accessKey`; `response_body` = resumo (`processingType` e o tamanho de cada XML) |

**Validação:** `accessKey` `^\d{44}$` (rota com `regex`). Não há conferência local: a rota é do tenant inteiro e serve
também NF-e de entrada que ainda não está em `TotvsInvoices`.

**Por que não persiste:** o XML é o documento fiscal, guardado pela Totvs (obrigação de 5 anos). Tamanho estimado: 5 a
30 KB numa nota comum, ~1 a 2 KB por item a mais (uma nota de 500 itens passa de 1 MB); o de cancelamento, 2 a 4 KB.
Gravar em `response_body` (`nvarchar(max)`) cada consulta multiplicaria o log. Se houver uso (ex.: entregar o XML a um
sistema externo sem nova chamada), uma tabela `TotvsInvoicesXml` (`doc_company + accessKey`) é a alternativa (F6).

**A confirmar:** se `mainInvoiceXml` vem como XML em texto ou já em base64 (a descrição do `danfe-search` sugere texto, que
precisa ser codificado). **Conferido (§10.11):** vem em base64; a situação vem em `invoiceStatus` (não `processingType`); chave
inexistente e nota cancelada dão 400 `KeyNotFound`, não 404.

### 8.5 `PostSearchDanfe` — `POST fiscal/v2/danfe-search` (arquivo, sem persistência)

| Item | Especificação |
|---|---|
| Rota do controller | `GET TotvsJobs/Totvs/ElectronicInvoices/{docCompany}/{accessKey}/Danfe?nfeDocumentType=1` |
| Corpo enviado à Totvs | `DanfeRequestModel`: `mainInvoiceXml*` (o XML do `xml-contents`, **em base64**), `nfeDocumentType` (`NfeDocumentType`: 1 NFeNormal/retrato, 2 NFeSimplified/etiqueta; opcional) |
| Resposta da Totvs | 200 `DanfeResponseModel` (`danfePdfBase64`) |
| Resposta da rota | **200 `application/pdf`** (base64 decodificado, `File(bytes, "application/pdf", "<accessKey>.pdf")`) — ou JSON com o base64 (F8) |
| Job | **13253** `TotvsPostSearchDanfe` (sob demanda) |
| Chave em `TotvsRequests` | `accessKey`; duas linhas (a do `xml-contents` com job 13253 e a do `danfe-search`); `request_body` = `{"accessKey":..., "nfeDocumentType":...}` (**sem o XML**); `response_body` = tamanho do PDF |

**Service:** a rota recebe a chave, não o XML: chama o `xml-contents` (§8.4), codifica o `mainInvoiceXml` em base64 e chama
o `danfe-search`. Quem chama não precisa transportar o XML. Uma variante que aceite o XML no corpo não é proposta.

**Validações:** `accessKey` com 44 dígitos; `nfeDocumentType` só 1 ou 2 (enum como número, §1.3).

**Tamanho:** o PDF tem ~30 a 150 KB por página, e o base64 soma 33%. O corpo enviado é o XML em base64 (até ~1,4 MB numa
nota grande).

**A confirmar:** se a rota aceita NFC-e (modelo 65) ou só NF-e (55); se aceita nota cancelada.

### 8.6 `PostTransactionPrint` — `POST fiscal/v2/transaction/print` (arquivo, sem persistência)

| Item | Especificação |
|---|---|
| Rota do controller | `GET TotvsJobs/Totvs/Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Print?invoiceDate=yyyy-MM-dd&template=N&type=1` (um segmento a mais que a rota de `GetInvoiceByKey`: sem colisão) |
| Corpo enviado à Totvs | `TransactionPrintRequestModel`: `branchCode`, `invoiceDate`, `invoiceSequence`, `transactionPrintTemplate` (modelo do TRAFP004), `transactionPrintType` (`TransactionPrintType`: 1 Normal, 2 Richtext). O swagger não marca nenhum como obrigatório; aqui **todos são** |
| Resposta da Totvs | 200 `TransactionPrintResponseModel`: `content` (base64 do PDF, pelo resumo da rota), `unifaceResponseStatus`, `unifaceMessage` |
| Resposta da rota | 200 `application/pdf`; **422** quando `unifaceResponseStatus` indica erro ou `content` vem vazio (com `unifaceMessage`) |
| Job | **13254** `TotvsPostTransactionPrint` (sob demanda) |
| Chave em `TotvsRequests` | `branchCode|invoiceSequence|invoiceDate`; `response_body` = `unifaceResponseStatus`, `unifaceMessage` e tamanho do `content` |

**Validações:** `branchCode` e `invoiceSequence` > 0; `invoiceDate` obrigatória (faz parte da chave da nota em
`TotvsInvoices`); `template` > 0; `type` só 1 ou 2. Alternativa (F9): `invoiceDate` opcional, lida de `TotvsInvoices` pela
loja + sequência (422 se não achar ou achar mais de uma).

**Riscos:** a rota roda um processo Uniface ("Tomas") no servidor do ERP: é a mais pesada da API. Não entra em laço nem
em job. Os valores de `unifaceResponseStatus` não estão no swagger (a confirmar); o código do modelo de impressão vem do
TRAFP004 e precisa ser informado pela área (F9).

### 8.7 `GetSearchInvoicesItemDetail` — `GET fiscal/v2/invoices/item-detail-search` (sob demanda)

| Item | Especificação |
|---|---|
| Rotas do controller | `GET TotvsJobs/Totvs/Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Items?invoiceDate=yyyy-MM-dd` e `GET TotvsJobs/Totvs/ElectronicInvoices/{docCompany}/{accessKey}/Items` |
| Parâmetros | `BranchCode` + `InvoiceDate` + `InvoiceSequence` **ou** `AccessKey` (nenhum é obrigatório no swagger); `Expand` = `barcodes` |
| Resposta da Totvs | 200 `InvoiceSearchResponseModel`: `branchCode`, `invoiceSequence`, `invoiceDate`, `inTransactionAgroup`, `items[]` com `invoiceItemsProduct[]` (e nele `referencedInvoices[]` e `transactions[]` com `salesOrder`), `invoiceItemTax[]`, `barcodes[]`; 404 |
| Job | **13255** `TotvsGetSearchInvoicesItemDetail` (sob demanda) |
| Chave em `TotvsRequests` | `branchCode|invoiceSequence|invoiceDate` ou `accessKey`; `response_body` inteiro |

**Validações:** exatamente uma das duas formas (a chave completa ou a chave de acesso); `accessKey` com 44 dígitos.

**Persistência: nenhuma (proposta).** Os itens já são gravados pelo `invoices/search` (`TotvsInvoicesItems`,
`...ItemsProducts`, `...ItemsTaxes`, `...ItemsBarCodes`), e o item desta rota **não tem** `cest`,
`additionalItemInformation` nem `rateDifferential`: regravar as tabelas por aqui apagaria essas colunas. O que só esta rota
traz é `inTransactionAgroup`, as transações por produto (com o pedido de venda) e as `referencedInvoices` por produto.

**Possível contorno do `referencedInvoices` (F3):** se esta rota devolver as notas referenciadas das devoluções que dão 400
no `invoices/search`, ela pode preencher `TotvsInvoicesItemsProductsReferenced` só para essas notas. Precisa de chamada
real com uma das notas que falham, e de uma procedure própria (a regra geral do merge apaga as netas quando o pai é
regravado sem elas).

### 8.8 `GetInvoicesPendingConditionalProducts` — `GET fiscal/v2/invoices/pending-conditional-products` (sem persistência)

Saldo de produtos em condicional (peças levadas pelo cliente para provar) por loja e pessoa.

| Item | Especificação |
|---|---|
| Rota do controller | `GET TotvsJobs/Totvs/ConditionalProducts/{docCompany}/{branchCode}?personCode=N` ou `?personCpfCnpj=...`, `&onlyPendingItem=true` (padrão `true`) |
| Parâmetros | `BranchCode*`, `PersonCode` **ou** `PersonCpfCnpj` (até 14; um dos dois, não os dois), `OnlyPendingItem*` |
| Resposta da Totvs | 200 `PendingConditionalResultModel`: `items[]` (`productCode`, `name`, `sellerCode`, `referenceCode`, quantidades e valores de saída, devolução e saldo, `balanceUnitNetValue`) |
| Resposta da rota | 200 com a lista; 404 com lista vazia |
| Job | **13256** `TotvsGetInvoicesPendingConditionalProducts` (sob demanda) |
| Chave em `TotvsRequests` | `branchCode|personCode` ou `branchCode|personCpfCnpj`; `response_body` inteiro |

**Validações:** `branchCode` > 0 e loja da empresa em `TotvsBranches` (422 sem chamar a API); exatamente um entre
`personCode` (> 0) e `personCpfCnpj` (11 ou 14 dígitos). **Sem persistência:** é o saldo do momento, muda a cada venda ou
devolução, e não tem filtro de alteração para um job.

### 8.9 `PostSearchInvoiceProducts` — `POST fiscal/v2/invoice-products/search` (sem persistência)

Produtos das notas em lista plana (uma linha por produto, com os dados da nota).

| Item | Especificação |
|---|---|
| Rota do controller | `POST TotvsJobs/Totvs/InvoiceProducts/{docCompany}/Search` (corpo com o filtro; é consulta, por isso `POST`) |
| Corpo | `InvoiceProductSearchQuery`: `filter` (`InvoiceProductFilterModel`), `option.isReturn`, `page`, `pageSize` (máx. 100), `expand` (`batchItems`), `order` (`branchCode`, `invoiceSequence`, `invoiceDate`) |
| Filtro | `branchCodeList*`, `ProductCodeList`, `BatchBarCodeList` (grafia do swagger, com maiúscula), `invoiceCodeList`, `operationType`, `modality`, `origin`, `invoiceStatusList`, `personCodeList`, `ignorePersonCodeList`, `operationCodeList`, `personCpfCnpjList`, `ignorePersonCpfCnpjList`, `startIssueDate`, `endIssueDate`, `acessKeyList` (grafia do swagger) |
| Resposta da Totvs | 200 `InvoiceProductResultModel` (envelope de página + `items[]`: nota, transação, produto, item, `kitCode`, `kitSequence`, `returnedQuantity` com `isReturn`, `batchItems[]`, `calculatedIbsCbs`) |
| Resposta da rota | 200 com a página como veio (quem chama pagina) |
| Job | **13257** `TotvsPostSearchInvoiceProducts` (sob demanda) |
| Chave em `TotvsRequests` | `branchCodeList` + o primeiro filtro restritivo informado (ex.: `invoiceCodeList`); `request_body` e `response_body` inteiros |

**Por que não é recorrente:** a rota **não tem filtro de alteração** (só data de emissão) nem `maxChangeFilterDate`, e o
conteúdo repete o que o `invoices/search` já grava em `TotvsInvoicesItemsProducts`. O que é só dela (`returnedQuantity`,
lotes, kit) é consulta pontual — ex.: quanto de um produto da nota já foi devolvido, para troca.

**Validações** (`InvoiceProductSearchQueryValidator`): `branchCodeList` com ao menos uma loja, todas da empresa;
**pelo menos um filtro restritivo** além das lojas (`ProductCodeList`, `BatchBarCodeList`, `invoiceCodeList`,
`personCodeList`, `personCpfCnpjList`, `acessKeyList` ou o intervalo de emissão), para não varrer a base; intervalo de
emissão com início e fim e no máximo 31 dias (proposta, F10); `acessKeyList` com 44 dígitos; `pageSize` 1 a 100; enums só
os nomes do swagger.

**Estranho no swagger:** o schema marca `productCodeList` e `batchBarCodeList` como obrigatórios (em minúscula), mas as
propriedades se chamam `ProductCodeList`/`BatchBarCodeList` e são anuláveis. A confirmar com chamada real se a API exige
produto ou lote; se exigir, a rota vira "em quais notas o produto/lote saiu" e a validação acima muda. **Conferido (§10.12):** a
API exige um entre `ProductCodeList`, `invoiceCodeList` e `acessKeyList` (400 `EnterAtLeastOne`); a emissão sozinha não basta.

### 8.10 `PostInvoicesManifestations` — `POST fiscal/v2/invoices/manifestations` (escrita, risco alto)

| Item | Especificação |
|---|---|
| Rota do controller | `POST TotvsJobs/Totvs/InvoicesManifestations/{docCompany}` |
| Corpo | `UpsertManifestationCommand`: `items[]` de `ManifestationModel`: `accessKey`, `branchCnpj`, `issuerCnpj`, `issuerCorporateName`, `invoiceCode`, `issueDate`, `serialCode`, `nsu` (`long`), `operationType` (`ManifestationOperationType`: Input/Output), `invoiceStatus` (`ManifestationStatusType`: Authorized/Canceled/Denied/Issued/Generated/Rejected), `totalValue`, `invoiceXml`, `documentType` (`ManifestationDocumentType`: **55** NFe, **57** CTe, **59** CFe, **81** NFSe) |
| Resposta da Totvs | 200 `SuccessProcessingModel` (`processingDate`); 400 `DomainNotificationMessage[]` |
| Job | **13258** `TotvsPostInvoicesManifestations` (sob demanda) |
| Chave em `TotvsRequests` | as `accessKey` unidas por `|`; `request_body` **sem o `invoiceXml`** (trocado pelo tamanho) |

**DTOs (`Dtos/Writes/Fiscal`):** `Manifestation` (o comando), `ManifestationItem` e os três enums. O swagger não marca
nenhum campo como obrigatório e não traz tamanhos.

**Validações:** de 1 a 8 itens (8 chaves de 44 caracteres cabem nos 400 de `field_key_value`; F1); `accessKey` com 44
dígitos e sem repetição; `branchCnpj` e `issuerCnpj` com 14 dígitos; `invoiceCode` e `nsu` > 0; `totalValue` >= 0;
`documentType` só 55, 57, 59 ou 81 — o número do enum é o **modelo do documento**, não a posição (§1.3); os dois enums de
texto vão pelo nome (o swagger os declara `string`, diferente dos outros).

**Service:** fluxo da §1.2; passo 3 confere cada `branchCnpj` em `TotvsBranches`. Sem nova tentativa em 5xx.

**Riscos:**
* **O corpo não tem o tipo do evento** (ciência, confirmação, desconhecimento, operação não realizada). Os campos são os
  do resumo que a SEFAZ entrega na distribuição de DF-e (NSU, emissor, XML): a rota parece **incluir/alterar documentos no
  painel de manifestação (FISFP153)**, e o evento junto à SEFAZ seria disparado pelo ERP. O que acontece de fato precisa
  ser confirmado com a área fiscal e com a Totvs antes de qualquer teste.
* Um evento de manifestação tem efeito fiscal (a confirmação da operação não se desfaz e prazo legal conta a partir dela);
  um documento falso incluído no painel entra no fluxo de escrituração de entrada.
* A integração **não tem fonte** desses dados (NSU e XML de notas de terceiros vêm da SEFAZ, não da Totvs): quem chama
  a rota informa os dados do documento. Mesmo sem caso de uso hoje, a rota é **implementada** (F1/T14, 27/10/2026), com
  testes só em homologação.

### 8.11 `GetDigitalCertificates` — `GET fiscal/v2/digital-certificates` (não expor)

A resposta traz o certificado A1 da empresa: `content` e `certifacate` (base64, grafia da API) e **`password`** (a senha do
certificado), além de `securityCode` (CSC do contribuinte, usado para assinar o QR Code da NFC-e). Com isso, qualquer um
emite e assina documentos fiscais em nome da empresa.

**Proposta (F2): não implementar** — sem rota, sem job, sem tabela, e o job não é reservado. O fluxo padrão gravaria a
resposta em `TotvsRequests.response_body` e em `auditing.Records`, o que espalharia a chave privada pelo banco de log.
Se surgir a necessidade legítima de **avisar o vencimento**, a alternativa é um job que lê a rota, guarda **só**
`sequence`, `description` e `expirationDate` (descarta o resto em memória, antes de qualquer log) e não grava o JSON em
lugar nenhum. Parâmetros: `BranchCode*`, `EnviromentType*` (`EnviromentType`: `Official`, `Statement` = homologação).

### 8.12 Arquivos

| Projeto | Arquivo | Mudança |
|---|---|---|
| `Domain.Totvs` | `Dtos/Reads/Invoices/InvoiceDisable.cs` | novo (§8.2) |
| `Domain.Totvs` | `Dtos/Reads/CostCenters/CostCenter.cs`, `CostCenterClassification.cs` | novos (§8.3) |
| `Domain.Totvs` | `Dtos/Reads/Invoices/Files/InvoiceXml.cs`, `Danfe.cs`, `TransactionPrint.cs` | novos (§8.4–8.6) |
| `Domain.Totvs` | `Dtos/Reads/Invoices/ItemDetail/*.cs` (`InvoiceItemDetail`, `ItemDetail`, `ItemDetailProduct`, `ItemDetailProductTransaction`, `ItemDetailTax`, `ItemDetailBarcode`; reusa `ItemProductReferencedInvoice` e `SalesOrder`) | novos (§8.7) |
| `Domain.Totvs` | `Dtos/Reads/Invoices/PendingConditional/PendingConditional.cs`, `PendingConditionalItem.cs` | novos (§8.8) |
| `Domain.Totvs` | `Dtos/Reads/Invoices/InvoiceProducts/InvoiceProduct.cs`, `InvoiceProductBatch.cs`; `Dtos/Queries/Fiscal/InvoiceProductSearchQuery.cs`, `InvoiceProductFilter.cs`, `InvoiceProductOption.cs` e enums (com os tipos da API, como os `Dtos/Writes`) | novos (§8.9) |
| `Domain.Totvs` | `Dtos/Writes/Fiscal/Manifestation.cs`, `ManifestationItem.cs`, enums | novos, só se aprovada a F1 (§8.10) |
| `Domain.Totvs` | `Entities/Reads/Invoices/InvoiceDisable.cs`; `Entities/Reads/CostCenters/CostCenter.cs`, `CostCenterClassification.cs` | novos |
| `Domain.Totvs` | `Enums/EnumJob.cs` | 13250–13258 |
| `Application.Totvs` | `Validations/Invoices/InvoiceDisableValidator.cs`, `Validations/CostCenters/*`, `Validations/Queries/Fiscal/InvoiceProductSearchQueryValidator.cs`, `Validations/Writes/Fiscal/*` | novos |
| `Application.Totvs` | `ITotvsInvoicesService` / `TotvsInvoicesService` | nove métodos |
| `Application.Totvs` | `ITotvsCommandHandler` / `TotvsCommandHandler` | queries de inutilizações e centros de custo para o cache; `invoiceDate` por loja + sequência se F9 |
| `Domain.Totvs` / `Infrastructure.Totvs` | `ITotvsInvoicesRepository` / `TotvsInvoicesRepository` | bulk insert + merges das duas leituras novas, consultas acima |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Invoices_Disable_Tables.sql`, `Script0NN_Create_Totvs_Invoices_Disable_Sync_Procedures.sql`, `Script0NN_Insert_Totvs_Invoices_Disable_Jobs_Into_Jobs_Table.sql` | trio de §8.2 (job 13250 e procedure a numerar) |
| `Infrastructure.Totvs` | `Script0NN_Create_Totvs_Cost_Centers_Tables.sql`, `..._Sync_Procedures.sql`, `Script0NN_Insert_Totvs_Cost_Centers_Jobs_Into_Jobs_Table.sql` | trio de §8.3 (job 13251, duas procedures a numerar) |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_On_Demand_Fiscal_Jobs_Into_Jobs_Table.sql` | jobs 13252–13257, intervalo 0 |
| `Infrastructure.Totvs` | `Script0NN_Insert_Totvs_Fiscal_Writes_Jobs_Into_Jobs_Table.sql` | job 13258 (F1) |
| Host | `Controllers/Totvs/TotvsController.cs` | 2 rotas de job, 7 rotas sob demanda (`File(...)` nas de PDF), 1 de escrita (F1) |
| Módulo | `README.md`, `Responsed Jsons/` | seções novas; exemplos com dados fictícios. XML e PDF **não** entram na pasta (só a estrutura do JSON com o conteúdo trocado por `"<base64>"`) |

### 8.13 Testes

1. **Sem API:** console com cada validador (formas "um ou outro", listas vazias, chaves de 44 dígitos, filtro sem
   restrição); serialização dos corpos (`danfe-search`, `transaction/print`, `invoice-products/search`) contra o swagger.
2. **Na API (leituras podem ir em produção; nada é gravado na Totvs):**
   * `invoices/disable` numa janela de 30 dias com todas as lojas: formato do array na query, URL, `maxChangeFilterDate`,
     chave duplicada entre modelos 55/65;
   * `cost-center` com 2000 → agora, com e sem `IsInactive`; conferir o total;
   * `xml-contents` com uma nota autorizada, uma cancelada e uma chave inexistente (404); medir os tamanhos;
   * `danfe-search` com o XML da anterior (texto vs base64), nos dois `nfeDocumentType`, e com uma NFC-e;
   * `transaction/print` com um modelo do TRAFP004 informado pela área; registrar os valores de `unifaceResponseStatus`;
   * `item-detail-search` pelas duas formas e com uma das devoluções que dão 400 no `referencedInvoices` (F3);
   * `pending-conditional-products` com uma pessoa com condicional aberto, por código e por CPF;
   * `invoice-products/search` sem `ProductCodeList` (para ver se é obrigatório) e com `isReturn`.
3. **Escrita (F1, só em homologação, §1.3):** uma manifestação de um documento de teste, depois de a área fiscal
   confirmar o efeito.
4. **Pelo host:** as rotas com e sem `X-User-Name`, CNPJ e chave inválidos (400), empresa não cadastrada (422), loja de
   outra empresa (422); conferir que `TotvsRequests.response_body` das rotas de arquivo tem só o resumo.

### 8.14 Decisões para aprovar

| # | Pergunta | Proposta |
|---|---|---|
| F1 | Implementar `invoices/manifestations`? | **Respondida (27/10/2026):** implementar mesmo sem caso de uso (T14): o chamador informa os dados do documento (a integração não tem NSU nem XML de terceiros). Efeito fiscal incerto: testes só em homologação |
| F2 | Expor `digital-certificates`? | **Não**: devolve a senha e a chave privada do certificado. Só um monitor de vencimento, se pedido, sem gravar o JSON |
| F3 | Usar `item-detail-search` como contorno do `referencedInvoices` (400 no `invoices/search`)? | Testar com uma nota que falha; se funcionar, gravar só `TotvsInvoicesItemsProductsReferenced` por uma procedure própria (entrega separada) |
| F4 | Intervalo das inutilizações | 120 min (volume baixo); 3 min só se houver consumidor que precise |
| F5 | Hash dos centros de custo em coluna (`json_hash`) com o cache montado do banco, em vez do cache em memória das comissões? | Sim; e rever o das comissões, cujo cache expira entre duas execuções |
| F6 | Persistir o XML das NF-e? | Não; criar `TotvsInvoicesXml` só se aparecer consumidor |
| F7 | Centro de custo no `TotvsInvoicesService` (API Fiscal) ou num service próprio? | No `TotvsInvoicesService`: um service por API |
| F8 | Rotas de PDF devolvem o arquivo (`application/pdf`) ou JSON com base64? | O arquivo; o XML volta em JSON (são dois XML) |
| F9 | `transaction/print`: `invoiceDate` e modelo obrigatórios na rota, ou lidos do banco / de configuração? | `invoiceDate` obrigatória; modelo obrigatório até a área definir um padrão (então `Totvs:TransactionPrintTemplate` no `appsettings`) |
| F10 | `invoice-products/search`: exigir um filtro restritivo e limitar a emissão a 31 dias? | Sim, para não varrer a base por uma rota exposta |

### 8.15 Ordem de execução

1. Base das leituras sem persistência (§8.1) e `GetXmlContents` (a mais simples, sem corpo);
2. `PostSearchDanfe` (depende da 1) e `PostTransactionPrint`;
3. `GetInvoicesPendingConditionalProducts` e `GetSearchInvoicesItemDetail` (com o teste da F3);
4. `PostSearchInvoiceProducts` (depende da confirmação dos obrigatórios);
5. `GetCostCenter` (catálogo, trio de scripts);
6. `GetInvoicesDisable` (leitura recorrente, trio de scripts);
7. README, `Responsed Jsons`; teste 4 de §8.13 pelo host;
8. `PostInvoicesManifestations` (F1/T14), depois da base de escrita (§1) e testada só em homologação.

---

## 9. Consolidação

### 9.1 Decisões que valem para todas as APIs

Os levantamentos por API repetiram as mesmas perguntas. Elas ficam aqui uma vez só; as decisões de cada seção que são
casos particulares delas apontam para cá.

| # | Tema | Aparece em | Proposta |
|---|---|---|---|
| T1 | **Ambiente dos testes de escrita** | D1, §3.6, §5.11, §6.7, §7.6, §8.13 | Toda escrita é testada só em homologação, que **existe** (gateway da §1.3, login conferido em 27/10/2026). Leituras podem rodar nos dois ambientes. **Onda 3 testada em homologação em 28/10/2026** com o usuário pessoal `cayo.kume`, por decisão do dono do usuário, sem esperar o usuário de integração (§10.19) |
| T2 | **Autenticação do host** | D2, PE5 | **Respondida (27/10/2026):** o `[Authorize]` **não** é ligado agora; a autenticação dos endpoints de inbound vai ser outro mecanismo, a definir. Implementação e testes em homologação seguem; publicar escrita em produção espera esse mecanismo |
| T3 | **Formato dos enums no corpo** | D6, PE6, SO11, §5 (`ProductValueType` C/P, `PromotionStatusType` A/B/E), §8.10 | O swagger mistura três formatos: número da descrição (maioria), nome (`"NotInformed"` no reset de cliente) e letra (`C`/`P`, `N`/`S`). Cada enum de escrita declara o seu formato no DTO (um conversor por formato), e o primeiro teste de cada rota confirma. **Confirmado em homologação (28/10/2026):** número em `status` do pedido de compra e da mensagem, `freightType` e `visualizationType`; letra em `entryType` (§10.19) |
| T4 | **Escrita ligada por configuração** | PE5, PR8 | Cada grupo de escrita atrás de `Totvs:Writes:<Grupo>:Enabled`, falso por padrão; ligar é decisão do negócio, não do deploy |
| T5 | **Repetição de escrita não idempotente** | SO6, DP2, §3.4.5 | Além de não repetir em 5xx (§1.2), recusar com **409** a mesma rota + chave + corpo que já conste com sucesso em `TotvsRequests` nos últimos 10 min |
| T6 | **Dados pessoais em `TotvsRequests`** | PE7 | `request_body` e `response_body` com documentos, conta bancária e chave PIX mascarados; arquivos (XML, PDF) registrados só com tipo e tamanho (§8.1). **Implementada em 28/10/2026** (§10.18): só nos corpos; a rota e o `field_key_value` ficam inteiros |
| T7 | **Reler depois da escrita** | D3, PE11, SO10 | Não: o job da leitura traz em até 3 min, e a resposta devolve a chave para o chamador usar a leitura sob demanda |
| T8 | **Conferência local antes de escrever** | D4, DP7, SO7 | Só quando o registro já está no banco; senão a Totvs valida |
| T9 | **Catálogos: como saber o que mudou** | §3.2, §4.1, §5.1, §7.1, F5 | Com `maxChangeFilterDate` na resposta: janela pelo banco, como as leituras recorrentes. Sem ela: leitura inteira com o **hash numa coluna `json_hash`** da própria tabela, carregado do banco (F5), em vez do cache em memória das comissões, que é refeito a cada 2 h |
| T10 | **Onde ficam os métodos novos** | PE9, PR12, F7 | Um service por grupo de recurso (ex.: `TotvsRepresentativesService`, `TotvsPersonCatalogsService`, `TotvsBonusService`, `TotvsProductCatalogsService`, `TotvsProductPricesService`), para os services atuais não passarem de mil linhas e para isolar o que tem efeito financeiro |
| T11 | **Leitura sob demanda sem persistência** | §3.3, PR14, §7.3, §8.1 | Devolve o corpo da Totvs em `Result.data` (arquivo como `application/pdf` ou JSON, F8) e registra em `TotvsRequests` |
| T12 | **Parâmetros `ValidationResult.*`** | §3–§8 | Aparecem como query em quase todos os `GET`: é o modelo interno da Totvs vazando no swagger. Nunca são enviados |
| T13 | **Nomes de job, rota e tabela únicos** | §6, SO9 | O método segue a decisão 18 da proposta; quando o nome se repete entre APIs (`GetClassifications` em Person, Product e Sales Order; `PostOrdersChangeStatus`, `PostQuantityItems`...), o job leva a API: `TotvsPersonGetClassifications`, `TotvsSalesOrderPostQuantityItems` |
| T14 | **Rotas que dependem do uso pelo negócio** | DS2, PE2, PR5, PR6, DP6, SO2, F1, DP1, SO1 | **Respondida (27/10/2026):** **implementar todas**, mesmo que a Newbloomers não use, para o código já ficar pronto para futuros clientes. Vale também para as escritas que esperavam "um consumidor definido" (DP1, SO1). Ficam fora só `products/search` (PR1, coberta pelas referências) e `digital-certificates` (F2, expõe a senha do certificado), que não dependem de uso |

### 9.2 Decisões por API

**Situação em 27/10/2026: todas as decisões estão fechadas.** As que têm "Respondida (27/10/2026)" no texto foram
respondidas de forma explícita (D1, D2, D6, T1, T2, T14, e as que a T14 resolveu: DS2, PE2, PR5, PR6, DP1, DP6, SO1, SO2
e F1). **Todas as outras foram aprovadas como propostas**: vale a coluna "Proposta" de cada tabela de decisões.

| Seção | Ids | Quantidade | Respondidas de forma explícita | Aprovadas como propostas |
|---|---|---|---|---|
| §2 Accounts Payable | D1–D6 | 6 | D1, D2, D6 | D3, D4, D5 |
| §3 Person | PE1–PE11 | 11 | PE2 (T14) | as outras 10 |
| §4 Seller | DS1–DS3 | 3 | DS2 (T14) | DS1, DS3 |
| §5 Product | PR1–PR15 | 15 | PR5, PR6 (T14) | as outras 13 |
| §6 Purchase Order | DP1–DP7 | 7 | DP1, DP6 (T14) | as outras 5 |
| §7 Sales Order | SO1–SO11 | 11 | SO1, SO2 (T14) | as outras 9 |
| §8 Fiscal | F1–F10 | 10 | F1 (T14) | F2–F10 |
| §9.1 Todas as APIs | T1–T14 | 14 | T1, T2, T14 | T3–T13 |

### 9.3 Ordem entre as APIs

A ordem da tabela do topo é a ordem de **especificação**. Para a **execução**, a proposta é por risco, em ondas que
cortam as APIs, para entregar primeiro o que não muda nada no ERP:

| Onda | O que entra | Pré-requisito |
|---|---|---|
| 1. Catálogos e leituras recorrentes | Catálogos de Person, Seller, Product, Sales Order e o centro de custo do Fiscal; representantes e recepção de venda; inutilizações de nota; tabelas de preço e promoções, lote, lote de pedido e sugestão de faturamento (T14) | nenhum: só leitura, pode ser testado em produção |
| 2. Leituras sob demanda sem persistência | `GetBranches`, estatísticas, saldo de bônus (com o token por loja, PE3), produto/pack, kardex, XML, DANFE, impressão de transação, itens pendentes e notas do pedido | nenhum |
| 3. Escritas de baixo risco | Estágio da parcela (AP), mensagens de pessoa, situação do pedido de compra, cores e grades | T1 (homologação) e a base de escrita (§1); a publicação em produção espera a autenticação de inbound (T2) |
| 4. Escritas de cadastro e de documento | Duplicata, agrupamento, cliente PF/PJ, representante do cliente, classificações, referências, códigos de barras, pedido de compra, itens e capa do pedido de venda | onda 3 testada |
| 5. Escritas com efeito financeiro ou fiscal | Bônus, preço, custo e promoção, dados fiscais do produto (NCM, CST, CEST), pedido B2C, preço e cancelamento de itens, cancelamentos de pedido, transação PDV, manifestação do destinatário | T4 e T5 implementadas |
| Fora | `products/search` (PR1), `digital-certificates` (F2) | — |

### 9.4 Fases por service

Cada API é dividida em fases, e cada fase cai numa onda da §9.3. Uma fase só entra quando a sua onda está liberada (os
pré-requisitos da onda valem para ela) e quando as fases anteriores do **mesmo service** estão prontas. Fases de services
diferentes na mesma onda são independentes e podem andar em paralelo. A coluna "Service" é a divisão proposta na T10.

**Matriz (fase de cada service em cada onda)**

| Onda | Accounts Payable | Person | Seller | Product | Purchase Order | Sales Order | Fiscal |
|---|---|---|---|---|---|---|---|
| 1. Catálogos e leituras recorrentes | — | PE-1, PE-2 | SE-1 | PR-1, PR-2 | — | SO-1 | FI-1 |
| 2. Sob demanda sem persistência | — | PE-3, PE-4 | — | PR-3 | PO-1 | SO-2 | FI-2 |
| 3. Escritas de baixo risco | **AP-1** (monta a base de escrita) | PE-5 | — | PR-4 | PO-2 | SO-3 | — |
| 4. Escritas de cadastro e documento | AP-2 | PE-6 | — | PR-5 | PO-3 | SO-4 | — |
| 5. Efeito financeiro ou fiscal | — | PE-7 | — | PR-6 | PO-4 | SO-5 | FI-3 |

Duas fases montam peças que as outras reusam: **AP-1** monta a base de escrita (§1, com T3 a T6), e a primeira fase da
onda 2 que entrar monta a base das leituras sem persistência (T11, §8.1).

#### Accounts Payable (`TotvsAccountsPayableService`, §2)

| Fase | Onda | Métodos | Jobs | Depende de |
|---|---|---|---|---|
| AP-1 | 3 | Base de escrita (§1.1) + `PostDuplicatesInstallmentsStage` | 13020 | T1 (a publicação em produção espera a T2). **Implementada em 27/10/2026** (§10) |
| AP-2 | 4 | `PostDuplicates`, `PostGroupDuplicates` | 13018, 13019 | AP-1 (o agrupamento é testado com duplicatas do `PostDuplicates`). **Implementada em 28/10/2026** (§10.20) |

#### Person (§3)

| Fase | Onda | Service | Métodos | Jobs | Depende de |
|---|---|---|---|---|---|
| PE-1 | 1 | `TotvsPersonCatalogsService` | `GetClassifications`, `GetEmailTypes`, `GetPhoneTypes`, `GetContactTypes` | 13032–13035 | — **Implementada em 27/10/2026** (§10.2) |
| PE-2 | 1 | `TotvsRepresentativesService` | `PostSearchRepresentatives`; `GetRepresentativeByCode` (PE10); `GetSearchReceptionSale` (PE2) | 13030, 13039, 13031 | — **Implementada em 27/10/2026** (§10.3) |
| PE-3 | 2 | `TotvsPeopleService` | `GetBranches`, `GetPersonStatistics` | 13036, 13037 | base T11 **Implementada em 28/10/2026** (§10.13) |
| PE-4 | 2 | `TotvsBonusService` | token por loja (PE3) + `PostListBalanceBonus` | 13038 | PE-3 **Implementada em 28/10/2026** (§10.13) |
| PE-5 | 3 | `TotvsPeopleService` | `PostMessages`, `PutChangeMessages`, `DeleteMessages` | 13044–13046 | AP-1 **Implementada em 28/10/2026** (§10.14), com a base comum das escritas e a trava T5 |
| PE-6 | 4 | `TotvsPeopleService`, `TotvsRepresentativesService` | `PostRemoveClassificationRelationship`, `PostCustomerRepresentatives`, `PostIndividualCustomers`, `PostLegalCustomers` | 13040–13043 | PE-1 (valida os códigos nos catálogos), PE-5, T3 e T6 |
| PE-7 | 5 | `TotvsBonusService` | `PostBonusDueDate`, `PostBonusConsume` | 13047, 13048 | PE-4 (saldo antes do consumo, PE4), T4, T5 |

#### Seller (`TotvsSellersService`, §4)

| Fase | Onda | Métodos | Jobs | Depende de |
|---|---|---|---|---|
| SE-1 | 1 | `GetOperationalArea` (as 4 rotas num job, DS1) | 13070 | — (DS2: implementada mesmo sem regiões na base) **Implementada em 27/10/2026** (§10.4) |

#### Product (§5)

| Fase | Onda | Service | Métodos | Jobs | Depende de |
|---|---|---|---|---|---|
| PR-1 | 1 | `TotvsProductCatalogsService` | `GetClassificationType`, `GetClassifications`, `GetCategory`, `GetGrid`, `GetMeasurementUnit`, `PostSearchColors`, `GetAdditionalFieldsTypes`, `GetProductGrouperConfiguration`, `GetInstructionItems`, `PostCompositions`; `GetPriceTablesHeaders`, `GetPriceTableScales` (PR5) | 13080–13091 | — **Implementada em 27/10/2026** (§10.5) |
| PR-2 | 1 | `TotvsProductPricesService`, `TotvsProductsService` | fase 3, partes 4 a 6 (`PostSearchPrices`, `PostSearchCosts`, `PostSearchBalances`, com a escolha da PR2 sobre `PostOmniChangedBalances`); `PostSearchPriceTables`, `GetPromotion` (PR5); `GetSimilar`; `PostSearchBatch` (PR6); `PostSearchProductCodes` (PR3) | 13015–13017, 13092–13097 | PR-1 (cabeçalhos das tabelas de preço). **Parte 1 (preços, custos e saldos) implementada em 27/10/2026** (§10.6); **parte 2 (`PostSearchPriceTables`, `GetPromotion`) em 27/10/2026** (§10.7); **parte 3 (`GetSimilar`, `PostSearchBatch`, `PostSearchProductCodes`) em 27/10/2026** (§10.8): fase concluída, menos o `PostOmniChangedBalances` (13096 reservado, PR2) |
| PR-3 | 2 | `TotvsProductsService` | `GetProducts`, `GetKardexMovement`, `GetGridFrequency`, `GetMeasurementConversions`, `GetCompositionProduct`, `GetCompositionGroupProduct` | 13100–13105 | base T11 **Implementada em 28/10/2026** (§10.13) |
| PR-4 | 3 | `TotvsProductCatalogsService` | `PostColors`, `PostGrids` | 13124, 13126 | AP-1, PR-1 **Implementada em 28/10/2026** (§10.15) |
| PR-5 | 4 | `TotvsProductCatalogsService`, `TotvsProductsService` | `PostClassifications`, `PostCreateClassificationRelationship`, `PostUpdateClassificationRelationship` (sem a classificação 6000, PR10), `PostBarcodes`, `PostUpdateBarcodes`, `PostReferences` (sem `values`, PR11); lote: `PostCreateBatch`, `PutBatchInfo`, `PostBatchPreferentialSalesOrder` (PR6) | 13120–13123, 13125, 13127, 13135–13137 | PR-4 |
| PR-6 | 5 | `TotvsProductsService`, `TotvsProductPricesService` | `PutProducts`, `PutBranchInfo`, `PutData` (só listas de produto, PR9); `PostCreateValues`, `PostUpdateValues`, `PostUpdatePromotionValues`, `PostPromotionItems` (PR7) | 13128–13134 | PR-2 (preços e custos lidos antes de escrever), T4, T5 |
| — | fora | — | `PostSearchProducts` (PR1) | — | — |

#### Purchase Order (`TotvsPurchaseOrdersService`, §6)

| Fase | Onda | Métodos | Jobs | Depende de |
|---|---|---|---|---|
| PO-1 | 2 | `GetPurchaseOrderByKey` (DP4: sob demanda **com** persistência, como o `GetSalesOrderByKey`) | 13185 | — **Implementada em 28/10/2026** (§10.13) |
| PO-2 | 3 | `PostPurchaseOrdersChangeStatus` | 13182 | AP-1 **Implementada em 28/10/2026** (§10.16) |
| PO-3 | 4 | `PostPurchaseOrdersQuantityItems`, `PostPurchaseOrders` (com `supplierOrderCode` obrigatório, DP2); `PostPurchaseOrderBatchLink` (DP6) | 13181, 13180, 13184 | PO-2 |
| PO-4 | 5 | `PostPurchaseOrdersCancel` (com conferência do fornecedor, DP3) | 13183 | PO-3 |

#### Sales Order (`TotvsSalesOrdersService`, §7)

| Fase | Onda | Métodos | Jobs | Depende de |
|---|---|---|---|---|
| SO-1 | 1 | `GetDiscountType`, `GetClassifications`; `GetBatchItems`, `GetChangedBatchItems`, `GetBillingSuggestions` (SO2) | 13190–13194 | — **Implementada em 27/10/2026** (§10.9), sem o `GetChangedBatchItems`: a rota exige `BranchCode` e `OrderCode` e passou para a SO-2 |
| SO-2 | 2 | `GetInvoices`, `GetPendingItems`; `GetChangedBatchItems` (vindo da SO-1: sob demanda por loja + pedido) | 13195, 13196, 13193 | base T11 **Implementada em 28/10/2026** (§10.13) |
| SO-3 | 3 | trava de repetição (T5) + `PostAdditionalOrder`, `PostAdditionalItems`, `PostObservationsOrder`, `PostShippingOrder` | 13201, 13207, 13203, 13202 | AP-1 **Implementada em 28/10/2026** (§10.17); a trava T5 entrou antes, na PE-5 (§10.14) |
| SO-4 | 4 | `PostOrdersChangeStatus`, `PostHeader` (SO8), `PostQuantityItems`, `PostItems`, `DeleteItems`; lote e contagem: `PostRelationshipBatchItems`, `DeleteBatchItems`, `PostSuggestBatchItemsNuance`, `PostRelationshipCounts` (SO2) | 13199, 13200, 13208, 13205, 13206, 13211–13214 | SO-3 |
| SO-5 | 5 | `PostPriceItems`, `PostCancelItems`, `PostB2COrders` (com a conferência do `orderId`, SO5), `PostOrdersCancel`, `PostPdvTransactions` | 13209, 13210, 13197, 13198, 13204 | SO-4, T4 |

#### Fiscal (`TotvsInvoicesService`, §8)

| Fase | Onda | Métodos | Jobs | Depende de |
|---|---|---|---|---|
| FI-1 | 1 | `GetCostCenter` (hash em coluna, F5), `GetInvoicesDisable` (F4) | 13251, 13250 | — **Implementada em 27/10/2026** (§10.10). Com ela, a onda 1 fica concluída |
| FI-2 | 2 | base das leituras sem persistência (§8.1) + `GetXmlContents`, `PostSearchDanfe`, `PostTransactionPrint` (F9), `GetInvoicesPendingConditionalProducts`, `GetSearchInvoicesItemDetail` (com o teste da F3), `PostSearchInvoiceProducts` (F10) | 13252–13257 | — (se for a primeira fase da onda 2, é ela que monta a base T11). **Parte 1 (base T11, `GetXmlContents`, `PostSearchDanfe`, `PostTransactionPrint`) implementada em 28/10/2026** (§10.11); **parte 2 (`GetSearchInvoicesItemDetail`, `GetInvoicesPendingConditionalProducts`, `PostSearchInvoiceProducts`) em 28/10/2026** (§10.12): fase concluída, menos o teste da F3 |
| FI-3 | 5 | `PostInvoicesManifestations` (F1: implementada mesmo sem caso de uso) | 13258 | AP-1, T1 |
| — | fora | `GetDigitalCertificates` (F2) | — | — |

Dentro de cada fase vale a "Ordem de execução" da seção da API (§2.7, §3.8, §4.5, §5.13, §6.9, §7.8, §8.15); ela foi
escrita por API, e as fases acima a cortam pelas ondas.

### 9.5 Numeração

* **Jobs:** cada API tem a sua faixa (tabela do topo); os ids citados nas seções já estão dentro dela. Procedures de merge
  continuam de **13585** em diante, numeradas na ordem em que cada tabela entrar.
* **Scripts:** o próximo é o **Script032**. Como o DbUp roda em ordem alfabética e nenhum script que já rodou é editado,
  os números são dados **na implementação**, na ordem em que cada parte entrar (os `Script0NN` das seções são
  provisórios).

---

## 10. Execução

### 10.1 Fase AP-1: base de escrita e `PostDuplicatesInstallmentsStage` (27/10/2026)

**Base de escrita (§1.1), usada por todas as escritas daqui em diante:**

| Peça | Onde |
|---|---|
| `Result` (`success`, `key`, `statusCode`, `apiCalled`, `message`, `response`, `retryAfterSeconds`) | `Domain.Totvs/Dtos/Writes/Result.cs` |
| `WriteResponse`: 200 aceita; 429 com `Retry-After`; **502** quando a Totvs foi chamada e não respondeu ou respondeu 5xx; 422 nos demais | `TotvsController` |
| Chave de configuração por grupo (T4): `ITotvsService.IsWriteEnabled(grupo)` lê `Totvs:Writes:<grupo>:Enabled`; ausente ou `false` = desligada. O `appsettings.json` traz `AccountsPayable` desligado | `TotvsService`, `appsettings.json` |
| Loja pelo CNPJ (passo 3 da §1.2): `ITotvsService.GetBranchCodeByCnpj` → `CreateGetBranchCodeByCnpjQuery` | `TotvsService`, `TotvsRepository`, `TotvsCommandHandler` |
| Enum como número (D6/T3): o Newtonsoft já serializa `enum` como número; o enum do DTO usa os números da descrição do swagger | `ChangeInstallmentStageType` |
| Validação no controller, antes do service: corpo inválido volta 400 com `field`/`message` | `TotvsController` |

**`PostDuplicatesInstallmentsStage`** (§2.3): DTO `ChangeInstallmentStage`, `ChangeInstallmentStageValidator`, método no
`TotvsAccountsPayableService` (chamada escrita no próprio método, D5), rota `POST TotvsJobs/Totvs/Duplicates/{docCompany}/Installments/Stage`,
`EnumJob` 13018–13020 e `Script032` (os três jobs de escrita de contas a pagar; 13018 e 13019 ficam para a AP-2).

**O que mudou em relação ao plano:**
* **D4 não se aplica:** a busca de duplicatas (`DuplicateOutModel`) não devolve o estágio da parcela, então não há como
  conferir a transição pelo banco. A Totvs valida e responde 400, que vira 422 com a mensagem dela.
* **T5 (trava de repetição) e T6 (mascaramento) ficam para a primeira rota que precisa delas:** a troca de estágio grava um
  valor absoluto (repetir não muda nada) e o corpo não tem dado pessoal. A T5 entra na AP-2 (`PostDuplicates` não é
  idempotente) e a T6 na PE-6 (clientes).

**Conferido sem a API** (console descartável com uma API falsa):
* validador: corpo válido; sem fornecedor e com os dois fornecedores; CNPJ com pontuação, parcela de 4 dígitos e duplicata de
  11; estágio fora do enum; corpo vazio. Cada caso com a mensagem certa;
* serialização: `{"branchCnpj":"...","supplierCode":10,"duplicateCode":5001,"installmentCode":1,"newStage":5}` (o fornecedor
  não informado fica fora; o enum vai como número), e o corpo recebido pelo `System.Text.Json` do host lê `newStage` numérico;
* fluxo: 200 → 200; 400 da Totvs → 422 com `code: message`; 401 → token renovado → 200 (2 chamadas); 429 duas vezes → 429
  com `Retry-After`; 502 → 502 **sem nova tentativa** (1 chamada); exceção de rede → 502; escrita desligada, empresa não
  cadastrada e loja de outra empresa → 422 **sem chamar a API**. Cada chamada à API gera um registro em `TotvsRequests` com
  o corpo enviado.

**Não verificado:**
* a **chamada real em homologação** (§2.5, teste 2): precisa do token de um usuário do ERP de homologação. O login com o usuário
  pessoal funcionou, mas o teste fica para quando houver o usuário de integração (§1.3), ou para você rodar com o seu;
* o `Script032` em banco e a rota pelo host.

### 10.2 Fase PE-1: catálogos da API Person (27/10/2026)

**O que entrou:**
* `TotvsPersonCatalogsService` (novo, T10) com `GetClassifications`, `GetEmailTypes`, `GetPhoneTypes` e `GetContactTypes`,
  cada um com a própria chamada à API (decisão 15 da proposta), no desenho do `GetBranchesList`: janela pelo banco
  (`MAX(maxChangeFilterDate)` − 1 h; banco vazio, desde 2000), nova tentativa em 401/429/502/503/504, cache de estado
  `Totvs:<Catálogo>:<cnpj>` montado com a tabela inteira da empresa (catálogo pequeno), validação, `auditing.Records` e
  `finally`;
* DTOs, entidades e validadores em `People/Catalogs`, `ITotvsPersonCatalogsRepository`/`TotvsPersonCatalogsRepository` e
  as queries no `TotvsCommandHandler`;
* tabelas `TotvsPersonClassifications` (`doc_company`, `typeCode`, `code`), `TotvsEmailTypes`, `TotvsPhoneTypes` e
  `TotvsContactTypes` (`doc_company`, `code`): `Script033` (tabelas), `Script034` (procedures, sem exclusão) e `Script035`
  (jobs 13032–13035, 120 min, e procedures **13585–13588**);
* rotas `GET TotvsJobs/Totvs/People/Classifications`, `.../People/EmailTypes`, `.../People/PhoneTypes` e `.../People/ContactTypes`.

**Achados na chamada real (27/10/2026):**
* `contact-types` **não** devolve o `PhoneTypeModel` declarado no swagger: vem `code`, `typeName` e `maxChangeFilterDate`.
  O DTO e a tabela seguem a resposta real (`typeName`, tamanho 60 por falta de documentação; o maior valor real tem 10);
* `maxChangeFilterDate` só vem com `StartChangeDate`/`EndChangeDate`, como nas filiais;
* a base tem 6 classificações de pessoa (2 tipos), 2 tipos de e-mail, 4 de telefone e 3 de contato.

**Conferido com o console descartável** (os 4 métodos de verdade, API real e banco falso), duas execuções: a 1ª grava 15
registros (6 + 2 + 4 + 3) sem mensagem de validação; a 2ª, com o banco falso preenchido, não regrava nada (0 nos quatro).
As colunas das 4 `DataTable` batem com o `Script033`.

**Não verificado:** os scripts `033`–`035` em banco e as rotas pelo host.

### 10.3 Fase PE-2: representantes e recepção de venda (27/10/2026)

**O que entrou:**
* `TotvsRepresentativesService` (novo, T10) com três métodos, cada um com a própria chamada à API:
  * `PostSearchRepresentatives` (job 13030, 120 min): `POST person/v2/representatives/search` com `filter.change` e todas as
    flags `in*`, o `expand` da documentação, `option.branchRepresentativeParameterList` com as lojas da empresa e `pageSize`
    500. Janela pelo banco (`MAX(maxChangeFilterDate)` − 1 h; banco vazio, desde 2000), **lida inteira, sem fatias**: a base tem
    20 representantes. Cache `Totvs:Representatives:<cnpj>`;
  * `GetRepresentativeByCode` (job 13039, sob demanda, PE10): `representativeCodeList` com `pageSize` 1 e `filter.change` desde
    2000 com as flags, para o `maxChangeFilterDate` vir preenchido (como o `GetCustomerByCode`); rota
    `GET TotvsJobs/Totvs/Representatives/{docCompany}/{code}` com `X-User-Name`, registrada em `TotvsRequests`;
  * `GetSearchReceptionSale` (job 13031, 3 min, PE2/T14): janela pela **data do movimento** (banco vazio, desde 01/01/2025, em
    fatias de 30 dias; depois, desde 2 dias antes do maior movimento gravado). Sem data de alteração na resposta, o hash SHA-256
    do JSON de cada recepção vai na coluna `json_hash` e é comparado com o do banco para a fatia (T9). Os telefones do cliente
    não são gravados (já estão em `TotvsPeoplePhones`);
* tabelas `TotvsRepresentatives` e 10 filhas (`Script036`–`038`: tabelas com FKs, procedures com exclusão das filhas e jobs
  13030 e 13039, procedures **13589–13599**) e `TotvsReceptionSales` + `TotvsReceptionSalesDocumentTypes` (`Script039`–`041`,
  job 13031, procedures **13600–13601**);
* rotas `POST TotvsJobs/Totvs/Representatives`, `GET .../Representatives/{docCompany}/{code}` e `GET .../ReceptionSales`.

**Achados na chamada real (27/10/2026):**
* a busca aceita `filter.change` desde 2000 (sem limite de período, ao contrário de notas e produtos) e devolve os 20
  representantes da base, cada um só com um endereço (nenhum cliente, tabela de preço, região ou parâmetro cadastrado);
* sem `filter.change` o `maxChangeFilterDate` vem nulo, como nas outras buscas de pessoa;
* a recepção de venda está **vazia** na base (nenhuma recepção em nenhum filtro) e responde 200 com `items` vazio, não o 404
  declarado no swagger. Implementada pela T14; a lógica de gravação e de hash foi conferida com respostas sintéticas.

**Conferido com o console descartável** (API real, banco falso):
* representantes: 1ª execução grava 20 (20 endereços), sem mensagem de validação; a 2ª não regrava nada;
* por código: o representante 1 é gravado e registrado em `TotvsRequests`; o 999999999 devolve `false`;
* recepção: contra a API, 23 chamadas na carga inicial (fatias de 30 dias desde 01/01/2025), nada gravado; com duas recepções
  sintéticas, a 1ª execução grava as 2 (e 2 formas de pagamento), a 2ª não regrava nada (hash igual, 1 chamada na janela de
  2 dias) e a 3ª, com uma observação alterada, regrava só 1;
* as colunas das 13 `DataTable` batem com os scripts `036` e `039`.

**Não verificado:** os scripts `036`–`041` em banco e as rotas pelo host.

### 10.4 Fase SE-1: regiões de atuação (27/10/2026)

**O que entrou:**
* `TotvsSellersService.GetOperationalArea` (job **13070**, 120 min, DS1 opção A): lê as quatro rotas
  `GET seller/v2/operational-area`, `-state`, `-cep` e `-city` inteiras (`Page`/`PageSize` 1000, sem filtro de data), uma
  depois da outra; qualquer página com erro interrompe a empresa sem gravar nada. Não precisa de lojas;
* estados, CEPs e cidades são agrupados por `operationalAreaCode`, cada lista ordenada pela chave, e o hash SHA-256 do JSON
  agrupado (sem o `maxChangeFilterDate`, que preenchido mudaria o hash a cada execução) vai na coluna `json_hash`, comparado
  com o do banco (T9, sem cache de memória). Só a região nova ou com hash diferente é regravada, com as filhas;
* filha de região que não veio em `operational-area` vai para `auditing.Messages` e é descartada (a FK recusaria);
  `auditing.Records` leva a chave `operationalAreaCode` e o JSON agrupado;
* tabelas `TotvsOperationalAreas`, `TotvsOperationalAreasStates`, `TotvsOperationalAreasCeps` e
  `TotvsOperationalAreasCities` (`Script042`–`044`: tabelas com FKs, procedures com exclusão das filhas quando a região é
  regravada, job 13070 e procedures **13602–13605**);
* rota `GET TotvsJobs/Totvs/OperationalAreas`. Os validadores ficam em `Validations/Sellers`, junto dos de vendedor (o plano
  dizia `Validations/Reads/Sellers`, pasta que não existe).

**Achados na chamada real (27/10/2026):** as quatro rotas respondem 200 com `count = 0`, com e sem filtro de data: a base não
tem regiões de atuação. Implementada pela T14 (DS2); o formato do `cep`, a unicidade do `operationalAreaCode` entre tipos e
os tamanhos dos textos continuam **a confirmar** quando houver dados, e o exemplo em `Responsed Jsons` fica para então.

**Conferido com o console descartável** (API real e respostas sintéticas, banco falso):
* contra a API: 4 chamadas, nada gravado;
* sintético (3 regiões, a principal em 2 páginas; um estado de região inexistente; um CEP repetido; um CEP e um tipo
  inválidos): a 1ª execução grava as 3 regiões (2 estados, 3 CEPs, 2 cidades), registra a órfã e as duas validações; a 2ª não
  regrava nada; a 3ª, com as listas em outra ordem e o `maxChangeFilterDate` preenchido, também não; a 4ª, com uma cidade
  alterada, regrava só a região dela; a 5ª, com a rota de CEP respondendo 500, não grava nada e registra o erro;
* as colunas das 4 `DataTable` batem com o `Script042`.

**Não verificado:** os scripts `042`–`044` em banco e a rota pelo host.

### 10.5 Fase PR-1: catálogos da API Product (27/10/2026)

**O que entrou:**
* `TotvsProductCatalogsService` (novo, PR12) com os 12 catálogos, um job de 120 min cada (PR4):

| Método | Job | Rota da API | Rota no controller | Tabelas |
|---|---|---|---|---|
| `GetClassificationType` | 13080 `TotvsGetClassificationType` | `GET classificationType` | `GET Products/ClassificationTypes` | `TotvsClassificationTypes` |
| `GetClassifications` | 13081 `TotvsProductGetClassifications` | `GET classifications` | `GET Products/Classifications` | `TotvsProductClassifications` |
| `GetCategory` | 13082 `TotvsGetCategory` | `GET category` | `GET Products/Categories` | `TotvsCategories` |
| `GetGrid` | 13083 `TotvsGetGrid` | `GET grid` | `GET Products/Grids` | `TotvsGrids` + `TotvsGridsSizes` |
| `GetMeasurementUnit` | 13084 `TotvsGetMeasurementUnit` | `GET measurement-unit` | `GET Products/MeasurementUnits` | `TotvsMeasurementUnits` + `TotvsMeasurementUnitsVariations` |
| `PostSearchColors` | 13085 `TotvsPostSearchColors` | `POST colors/search` | `POST Products/Colors` | `TotvsColors` + `TotvsColorsAdditionalInformation` |
| `GetAdditionalFieldsTypes` | 13086 `TotvsGetAdditionalFieldsTypes` | `GET additional-fields-types` | `GET Products/AdditionalFieldsTypes` | `TotvsAdditionalFieldsTypes` |
| `GetProductGrouperConfiguration` | 13087 `TotvsGetProductGrouperConfiguration` | `GET product-grouper-configuration` | `GET Products/GrouperConfigurations` | `TotvsProductGrouperConfigurations` + `TotvsProductGrouperConfigurationsItems` |
| `GetInstructionItems` | 13088 `TotvsGetInstructionItems` | `GET instruction-items` | `GET Products/InstructionItems` | `TotvsInstructionItems` |
| `PostCompositions` | 13089 `TotvsPostCompositions` | `POST compositions` | `POST Products/Compositions` | `TotvsCompositions` + `TotvsCompositionsFibers` |
| `GetPriceTablesHeaders` | 13090 `TotvsGetPriceTablesHeaders` | `GET price-tables-headers` | `GET Products/PriceTablesHeaders` | `TotvsPriceTables` + `TotvsPriceTablesSalesOrderClassifications`, `TotvsPriceTablesPersonClassifications`, `TotvsPriceTablesPaymentConditions`, `TotvsPriceTablesAveragePeriods`, `TotvsPriceTablesAveragePeriodQuantities` |
| `GetPriceTableScales` | 13091 `TotvsGetPriceTableScales` | `GET price-table-scales` | `GET Products/PriceTableScales` | `TotvsPriceTableScales` + `TotvsPriceTableScalesItems` |

* onze catálogos seguem o desenho das filiais e da PE-1: janela pelo banco (`MAX(maxChangeFilterDate)` − 1 h; banco vazio,
  desde 2000), uma leitura paginada sem fatias, cache de estado `Totvs:<Catálogo>:<cnpj>` montado com a tabela inteira da
  empresa e só o registro novo ou com data mais recente é gravado. As filhas são apagadas no merge quando o pai é regravado;
* as cores não têm data de alteração na resposta: são lidas inteiras com `filter: {}` e o SHA-256 do JSON de cada cor vai
  na coluna `json_hash` (T9), comparado com o do banco;
* a paginação e a repetição (401, 429, 502/503/504) ficam num helper privado do service, `GetCatalogPages<T>`, que devolve
  cada item com o seu JSON; a chamada, a janela e a gravação continuam em cada método (decisão 15);
* filhas sem chave na API (os tamanhos da grade e os prazos médios por quantidade) levam `sequence`, a posição na lista;
* DTOs, entidades e validadores em `Products/Catalogs` (o `Products` já tem `Color`, `Composition` e `Classification` das
  referências), repositório `TotvsProductCatalogsRepository`, `Script045`–`047` (23 tabelas com FKs, procedures
  **13606–13628**, jobs 13080–13091).

**Achados na chamada real (27/10/2026):**
* com janela, todos trazem `maxChangeFilterDate`, menos as cores (como o swagger indicava); sem janela, nenhum traz;
* volume: 46 tipos de classificação, 609 classificações, 24 grades (264 tamanhos), 24 unidades de medida (1 variação), 632
  cores (2 páginas, nenhuma com informação adicional), 4 tipos de campo adicional e 24 composições (45 fibras). Categorias,
  agrupadores, itens de instrução, tabelas de preço e escalas estão **vazios** na base: implementados pela T14 e conferidos
  com respostas sintéticas;
* `colors/search` sem `filter` responde 400 (`EmptyField`); com `filter: {}` devolve tudo;
* `additional-fields-types` devolve o `type` como **número** (1), não pelo nome do enum do swagger: a coluna guarda o que
  vier (texto);
* uma grade pode repetir um tamanho (as grades 1 e 17), por isso a chave é a posição;
* nenhuma chave repetida nos catálogos com dados; os textos cabem nos tamanhos do `Script045`.

**Conferido com o console descartável** (API real e respostas sintéticas, banco falso):
* 1ª execução grava 46 tipos, 609 classificações, 24 grades com 264 tamanhos, 24 unidades com 1 variação, 632 cores, 4 tipos
  de campo adicional e 24 composições com 45 fibras, com 1 chamada por catálogo (2 nas cores); a 2ª não grava nada;
* cores: com um nome alterado só aquela cor é regravada; a execução seguinte não regrava, e a volta ao nome original regrava;
* grade com data nova e um tamanho a menos: regravada com os tamanhos restantes;
* sintéticos: tabela de preço com as 5 filhas (classificação de pedido repetida e condição de pagamento sem código
  descartadas; prazos por quantidade pela posição) e outra sem filhas; escala com 2 itens; agrupador com 1 item; item de
  instrução; categoria sem `maxChangeFilterDate` (registrada no log e regravada a cada execução, como na PE-1). A 2ª execução
  de cada um não grava nada;
* as colunas das 23 `DataTable` batem com o `Script045`.

**Não verificado:** os scripts `045`–`047` em banco e as rotas pelo host. Continua a confirmar se a alteração só numa filha
(tamanho da grade, fibra, filhas da tabela de preço) muda o `maxChangeFilterDate` do pai; se não mudar, o catálogo passa
para o desenho de hash das cores.

### 10.6 Fase PR-2, parte 1: preços, custos e saldos (27/10/2026)

A PR-2 foi dividida em partes, como a fase 3: esta entrega as partes 4 a 6 da fase 3 (`PostSearchPrices`, `PostSearchCosts`,
`PostSearchBalances`); `PostSearchPriceTables`, `GetPromotion`, `GetSimilar`, `PostSearchBatch` e `PostSearchProductCodes` ficam
para as próximas.

**Decisão do usuário (27/10/2026):** os três leem **todos** os códigos do cadastro (preço, custo e saldo), em vez de um
subconjunto; volume e tempo se resolvem no desenho (abaixo).

**O que entrou:**
* `TotvsProductPricesService` (novo, PR12) com `PostSearchPrices` (job 13015) e `PostSearchCosts` (13016), e
  `TotvsProductsService.PostSearchBalances` (13017), todos de 3 min (decisão 29 da proposta);
* **códigos descobertos pela própria API**, em cache de 2 h / 30 min (`Totvs:PriceCodes`, `Totvs:CostCodes`,
  `Totvs:StockCodes`): preço e custo mandam os códigos de 1 a 999 numa chamada e a API responde 400 com
  `Price N not found`/`Cost N not found` para cada um que não existe (o resto é a lista: hoje 1–6 e 1–8); o saldo pede os 999
  para um produto e a API devolve todos, mas só os que existem vêm com `stockDescription` (hoje 1–17). Um código novo no ERP
  entra sem mudar o código;
* **uma chamada por loja** de cada marca (`GetBrandBranchCodes`, as mesmas lojas das referências), filtrada pela classificação
  6000 da marca e ordenada por produto. As marcas (matrizes 1 e 101, códigos 2 e 4) foram para `TotvsProductBrands`, usada também
  pelo `PostSearchReferences`;
* **carga inicial por produto**: o `filter.change` aceita no máximo 1 ano, então a loja sem `maxChangeFilterDate` gravado é lida
  inteira, sem `change`, de `startProductCode` = maior produto gravado + 1 até 999999999, e cada página é gravada ao chegar. Se
  falhar, a próxima execução continua do maior produto gravado. Depois, a janela vai do maior `maxChangeFilterDate` − 1 h (ou do
  começo da carga inicial − 1 h) até agora, em fatias de 30 dias, com cache de estado por loja
  (`Totvs:<Prices|Costs|Balances>:<cnpj>:<loja>`);
* preço: `inPrice` + `inPromotionalPrice` (a mudança de promoção não entra no `inPrice`), `isPromotionalPrice` e `expand`
  `promotionalInformation,informationOtherPromotions`; a promoção vigente vira colunas do preço (decisão 26) e as outras
  promoções vão para `TotvsPricesOtherPromotions`. O preço programado fica de fora (não tem campo próprio na resposta);
* custo: `inCost` com `costCodeList` (o swagger pede `priceCodeList`, provável erro, §5.9);
* saldo: `inStock` e também `inSalesOrder`, `inTransaction` e `inPurchaseOrder`, porque os pendentes de pedido de venda,
  transação, pedido de compra e ordem de produção são lidos; `expand=locations` (`TotvsBalancesLocations`). O lote de
  planejamento (exige a lista de ciclos) e a `measurementConversion` ficam de fora;
* tabelas `TotvsPrices`, `TotvsPricesOtherPromotions`, `TotvsCosts`, `TotvsBalances` e `TotvsBalancesLocations`, uma linha por
  loja + produto + código, com `branchCode` antes do produto na chave (`Script048`–`050`, procedures **13629–13633**). Os dados do
  produto (nome, SKU, cor, tamanho) já estão em `TotvsSkus` e não são repetidos;
* rotas `POST TotvsJobs/Totvs/Products/Prices`, `.../Products/Costs` e `.../Products/Balances`.

**Achados na chamada real (27/10/2026):**
* a API devolve todo produto × loja × código pedido, com zero quando não há valor, e o saldo devolve até o código que não existe
  (sem descrição): por isso só os códigos existentes vão no pedido;
* tempo de resposta cresce com produtos × lojas × códigos por página: saldo de 1 loja com 17 códigos, 100 produtos em ~16 s e
  1000 em ~60 s; 15 lojas com 100 produtos deram 502 depois de 196 s; custo de 15 lojas, 165 produtos em ~29 s. Daí uma chamada
  por loja e página de 100;
* `startProductCode` exige `endProductCode`; com `change` de mais de 1 ano a API responde 400 `GeneralGreaterThanOneYear`;
* volume: 10 287 produtos na Misha e 11 613 na Open Era. **Estimativa da carga inicial** (sem banco, pelas medições): preço
  ~2 s por página de 100, ~3,5 min por loja; custo parecido; saldo ~16 s por página, ~27 min por loja, perto de 11 h para as
  25 lojas. A carga é retomável por loja e por produto.

**Conferido com o console descartável** (API real, banco falso, só a loja 1 da Misha; a Open Era sem lojas, com a mensagem no log;
o estado da loja semeado para a carga inicial começar nos últimos 250 produtos, a partir do 31125):
* descoberta: preço 1–6, custo 1–8 e saldo 1–17, uma chamada cada;
* 1ª execução: carga inicial em 3 páginas e a janela da última hora; preço grava 320 produtos (1 920 linhas, os 250 da faixa e os
  alterados na última hora), custo 252 (2 016) e saldo 257 (4 369), em 11 s, 4 s e 50 s. A 2ª execução faz 1 chamada (a janela) e
  não regrava nada, a não ser um saldo alterado entre as duas;
* janela forçada de 2 dias: preço regrava 82 produtos (2 páginas), custo 209 (2 páginas) e saldo 704 (8 páginas, 113 s); as flags
  `inSalesOrder`, `inTransaction` e `inPurchaseOrder` foram aceitas;
* falha: com 500 na 2ª página da carga inicial de custo, a 1ª página (100 produtos, até o 31331) fica gravada e a loja para; a
  execução seguinte continua do 31332 e completa os 252;
* nenhuma mensagem de validação; nenhuma outra promoção nem local nos produtos lidos (as tabelas ficam vazias no teste);
* as colunas das 5 `DataTable` batem com o `Script048`.

**Não verificado:** os scripts `048`–`050` em banco, as rotas pelo host e a carga inicial inteira (o teste semeou o estado da loja
1 perto do último produto). Os jobs da Totvs ainda não estão registrados no Hangfire; quando forem, a carga inicial dos saldos
(horas) precisa de proteção contra execução simultânea do mesmo job.

### 10.7 Fase PR-2, parte 2: itens das tabelas de preço e promoções (27/10/2026)

**O que entrou** (no `TotvsProductPricesService`, os dois a cada 120 min, PR5):
* `PostSearchPriceTables` (job 13092): as tabelas vigentes de `TotvsPriceTables` (cabeçalho da PR-1: sem `endDate` ou com ela de
  hoje em diante), uma chamada por tabela (`option.priceTableCode` aceita uma só) com todas as lojas da empresa. Mesmo desenho dos
  preços (§10.6): carga inicial por faixa de produto, retomável, e depois a janela com `change.inPriceTableItem`,
  `branchPriceTableCodeList` e `priceTableCodeList`, em fatias de 30 dias, com cache de estado por tabela
  (`Totvs:PriceTableItems:<cnpj>:<tabela>`). Tabelas `TotvsPriceTablesItems` (tabela + produto, com `referenceId`) e
  `TotvsPriceTablesItemsPrices` (+ loja + escala; o preço que sai do produto regravado é apagado no merge), com FK para o
  cabeçalho;
* `GetPromotion` (job 13093): uma leitura por loja (`BranchCode` é obrigatório), todas as páginas, e o hash do JSON de cada item
  (promoção × produto) comparado com o `json_hash` gravado (T9): só o item novo ou alterado é gravado, página a página.
  Tabela `TotvsPromotions` (loja + promoção + produto, com valores, período e situação). O item que sai da promoção fica no banco
  com a última situação;
* `Script051`–`053` (procedures **13634–13636**), rotas `POST TotvsJobs/Totvs/Products/PriceTables` e
  `GET .../Products/Promotions`.

**Achados na chamada real (27/10/2026):**
* `promotion` recusa `PageSize` acima de **100** (o swagger diz 500); `StartChangeDate`/`EndChangeDate` dão 400 `ORA-00920`
  (erro da Totvs) em todo formato de data testado, e só o `StartChangeDate` é ignorado. Daí a leitura inteira com hash;
* o filtro `Status` só aceita o nome do enum (`Open`, `Blocked`, `Closed`), mas a resposta traz a letra (`A`, `B`, `E`), gravada
  como veio;
* só as matrizes têm promoção: 5 962 itens na loja 1 e 20 142 na 101 (15 promoções, 19 929 abertas e 6 175 encerradas); as
  outras lojas devolvem 0;
* a base não tem tabela de preço: `price-tables/search` responde 400 `PriceTableCode N not found`. Implementado pela T14 e
  conferido com respostas sintéticas.

**Conferido com o console descartável** (banco falso):
* promoções, API real (lojas 1, 2 e 101): a 1ª execução grava os 26 104 itens em 263 chamadas (44 s); a 2ª não grava nada
  (30 s); com o valor do 1º item de cada página alterado, regrava só os 262; com o valor de volta, os mesmos 262; com 500 na
  3ª página de cada loja, grava as 2 primeiras páginas (400 itens), registra o erro e segue para a próxima loja, e a execução
  seguinte grava os 25 704 que faltavam;
* tabelas de preço: contra a API (tabela inexistente), o 400 vai para o log e nada é gravado; sintético, a carga inicial grava 2
  produtos (preço repetido por loja e escala descartado, produto sem preço gravado sem filhas) e a janela regrava o produto com
  data nova, ficando só o preço que veio; a 2ª execução faz 1 chamada e não grava nada;
* as colunas das 3 `DataTable` batem com o `Script051`.

**Não verificado:** os scripts `051`–`053` em banco e as rotas pelo host.

### 10.8 Fase PR-2, parte 3: similares, lotes e conferência das referências (27/10/2026)

**O que entrou** (no `TotvsProductsService`):
* `GetSimilar` (job 13094, 120 min): a rota exige `ProductsCode` ou a janela, e o `lastChageDate` da resposta vem sempre
  `0001-01-01`, então a janela começa no fim da última janela lida e gravada − 1 h (coluna `changeWindowEnd`; banco vazio, desde
  2000) e vai até agora. O tempo de cada chamada cresce com o volume da janela, não com a página, então a 1ª página diz o volume e a
  janela acima de 1000 produtos é dividida ao meio (até 1 minuto), em ordem de data; cada janela é gravada só inteira, e uma falha
  faz a próxima execução continuar da última completa. O hash do JSON de cada produto (`json_hash`, T9) evita regravar o que não
  mudou. Tabelas `TotvsProductsSimilar` (produto + `groupSequenceCode` + `changeWindowEnd`) e `TotvsProductsSimilarItems`
  (`listType` `Product` ou `ReferenceGroup` + produto similar). A lista da mesma grade (`similarGroupProductList`) não é gravada,
  como decidido na §5.2 (sai de `TotvsSkusColorsProducts` pela referência);
* `PostSearchBatch` (job 13095, 120 min, PR6): uma leitura por loja, com o período desde 2000 (a rota exige um produto ou um
  período: 400 `BatchMandatoryFields`) e o `expand` nas duas grafias (`additionalFieds,additionalFields`); a resposta não tem data
  de alteração, então o hash de cada lote diz o que mudou. Tabelas `TotvsBatches` (loja + lote + produto), `TotvsBatchesItems`
  (+ sequencial) e `TotvsBatchesItemsAdditionalFields` (+ código). Lote sem `batchCode` e item sem sequencial vão para o log e não
  são gravados. A janela por `auditing.Logs` da §5.2 foi trocada pela leitura inteira com hash (T9), que não depende do log;
* `PostSearchProductCodes` (job 13097, diário, PR3): por marca, os códigos do `product-codes/search` com o mesmo filtro do
  `PostSearchReferences` (matriz, ativos, produto acabado, classificação 6000; página de 5000) comparados com os produtos de
  `TotvsSkusColorsProducts` da matriz. Os que faltam têm a referência achada por `productCodeList` (500 por chamada) e lida inteira
  por `referenceCodeList` (50 por chamada, páginas unidas por `referenceId`), gravada pelo `SaveReferences` sem
  `maxChangeFilterDate` (como o `GetProductByCode`; o job a regrava quando mudar). No máximo 500 referências por execução; marca sem
  nada gravado é pulada (a carga é do `PostSearchReferences`). O `GetProductByCode` não é chamado: ele reinicia o log da execução;
* `Script054`–`056` (procedures **13637–13641**), rotas `GET TotvsJobs/Totvs/Products/Similar`, `POST .../Products/Batches` e
  `POST .../Products/ProductCodes`.

**Achados na chamada real (27/10/2026):**
* `similar`: 22 109 produtos desde 2000, quase todos só com a lista da mesma grade; `lastChageDate` sempre `0001-01-01` (não cabe
  em `datetime` e não é gravado). **O tempo de cada chamada depende do volume da janela, não do tamanho da página**: a janela
  desde 2000 leva 30 a 90 s por página (às vezes 502), e as 222 páginas passariam de 4 h; o dia 15/06/2026 (a migração, 18 996
  produtos) leva ~60 s mesmo com página de 1; uma hora desse dia (~6 mil), ~10 s; um mês comum (~400 a 700), ~1 s. Antes de 06/2026
  não há alteração; depois, 250 a 700 por mês. A primeira versão (janela sempre desde 2000) travou o teste e foi trocada pela
  janela incremental com divisão;
* `batch/search`: nenhum lote em nenhum tipo (1 a 5, numéricos; o nome do enum dá 400). Implementado pela T14 e conferido com
  respostas sintéticas;
* `product-codes/search`: com o `branchInfo` da marca, página de 100 000 deu 502 e de 5000 respondeu em ~22 s (Misha: 9 721
  produtos ativos acabados); sem `branchInfo` vêm também os inativos (10 287).

**Conferido com o console descartável** (API real e respostas sintéticas, banco falso):
* similares, carga inicial com 500 simulado na 40ª chamada: a 1ª execução desce de "desde 2000" até janelas de minutos dentro de
  15/06/2026 (40 chamadas, 15 min), grava 362 produtos das janelas completas e para; a 2ª continua de 15/06/2026 00:25 (o fim da
  última janela gravada) e grava os 21 747 que faltavam (295 chamadas em 293 janelas, 19 min), somando os 22 109 da API. A 3ª
  execução é incremental: 1 chamada, na janela da última hora, sem produto alterado (por isso o cenário de alteração não teve o
  que alterar). Nenhum produto da base tem `similarProductList` ou `similarRefereceGroupProductList` (a tabela de itens fica vazia);
* lotes: contra a API, nada (base sem lotes); sintético, grava os 2 lotes válidos (item sem sequencial descartado, campo
  adicional repetido fica o último), registra o lote sem `batchCode`, não regrava os inalterados e regrava só o lote cujo saldo
  mudou. As contagens dobradas do teste vêm da resposta sintética (a mesma, com a loja 1, nas chamadas das lojas 1 e 101);
* conferência: com 3 produtos da Misha tirados do banco falso (17, 10558 e 31475), compara os 9 721 da Totvs com os 9 718
  gravados, acha as 3 referências (421, 115, 8422) e as grava inteiras (8 produtos, os 3 que faltavam entre eles; o validador
  registra a falta do `maxChangeFilterDate`, como no `GetProductByCode`), em 4 chamadas e 38 s; a Open Era, sem nada gravado, é
  pulada com a mensagem;
* as colunas das 5 `DataTable` batem com o `Script054`.

**Não verificado:** os scripts `054`–`056` em banco e as rotas pelo host.

### 10.9 Fase SO-1: catálogos, lotes de pedidos e sugestões de faturamento da API Sales Order (27/10/2026)

**O que entrou** (no `TotvsSalesOrdersService`, como diz a §7):
* `GetDiscountType` (job 13190), `GetClassifications` (13191) e `GetBatchItems` (13192), a cada 120 min: com o filtro de
  alteração as três respostas trazem `maxChangeFilterDate`, então seguem o desenho das filiais e da PE-1/PR-1 (T9) e não o hash
  proposto na §7.1: janela pelo banco (`MAX(maxChangeFilterDate)` − 1 h; banco vazio, desde 2000), cache de estado
  `Totvs:<Catálogo>:<cnpj>` com a tabela inteira da empresa, e o registro sem a chave (`code` nulo) vai para o log sem ser gravado.
  Tabelas `TotvsSaleDiscountTypes`, `TotvsSaleClassifications` (o `type` vem como número, 1 capa e 2 item, e é gravado como veio)
  e `TotvsSaleOrderBatches`;
* `GetBillingSuggestions` (job 13194, 3 min): janela pelo banco desde 01/01/2025 em fatias de 30 dias, com `Expand=items`, cache de
  estado por (loja, sugestão) como os pedidos, e a sugestão de loja fora de `TotvsBranches` da empresa vai para o log e é
  descartada (a rota não filtra por lista de lojas). Tabelas `TotvsBillingSuggestions`, `TotvsBillingSuggestionsOrders` e
  `TotvsBillingSuggestionsOrdersItems`, com exclusão das filhas quando a sugestão é regravada;
* **`GetChangedBatchItems` não entrou**: `changed-batch-items` responde 400 `EmptyField` sem `BranchCode` e `OrderCode`, mesmo
  com as datas. Pela regra da §7.2, a rota não serve como leitura recorrente: passa para a SO-2 como leitura sob demanda por loja +
  pedido (job 13193 reservado), e a releitura dos pedidos alterados (SO2) cai junto;
* o helper `GetCatalogPages<T>` do service pagina os quatro GET e trata 404 (declarado por três deles) como lista vazia;
* `Script057`–`059` (6 tabelas, procedures **13642–13647**), rotas `GET TotvsJobs/Totvs/SalesOrderDiscountTypes`,
  `.../SalesOrderClassifications`, `.../SalesOrderBatches` e `.../BillingSuggestions`.

**Achados na chamada real (27/10/2026):** 6 tipos de desconto e 9 classificações (nenhum `code` nulo, nenhuma chave repetida;
textos bem abaixo dos tamanhos do swagger); nenhum lote de pedido e nenhuma sugestão de faturamento na base (implementados pela
T14, conferidos com respostas sintéticas).

**Conferido com o console descartável** (API real e respostas sintéticas, banco falso):
* tipos de desconto e classificações: a 1ª execução grava 6 e 9 em 1 chamada cada; a 2ª, com a janela desde o maior
  `maxChangeFilterDate` − 1 h, não grava nada;
* lotes de pedidos: contra a API, nada; sintético, grava o lote com código e registra o sem código; a 2ª não regrava;
* sugestões de faturamento: contra a API, as 23 fatias desde 01/01/2025 sem nada; sintético, grava a sugestão da loja 1 com 2
  pedidos (o pedido repetido fica com a última versão) e 1 item, registra e descarta a da loja 999; a 2ª, com a mesma data, não
  regrava; a 3ª, com data nova, regrava;
* as colunas das 6 `DataTable` batem com o `Script057`.

**Não verificado:** os scripts `057`–`059` em banco e as rotas pelo host.

### 10.10 Fase FI-1: inutilizações e centros de custo da API Fiscal (27/10/2026)

**O que entrou** (no `TotvsInvoicesService`, F7):
* `GetInvoicesDisable` (job 13250, 120 min, F4): `BranchCodeList` é obrigatório (400 `EmptyField` sem ele) e vai com todas as
  lojas da empresa repetidas na rota (`BranchCodeList=1&BranchCodeList=2...`), porque os parâmetros de query do `IApiClient` são
  um dicionário (o `BuildUrl` junta o resto com `&`); 25 lojas cabem com folga na URL. Janela pelo banco (`MAX(maxChangeFilterDate)`
  − 1 h; banco vazio, desde 01/01/2025) em fatias de 30 dias, cache de estado com a tabela inteira da empresa. Tabela
  `TotvsInvoicesDisable`, chave loja + modelo + série + número inicial, com série e modelo nulos gravados como `''` e `0`;
* `GetCostCenter` (job 13251, 120 min): a rota exige `StartChangeDate`/`EndChangeDate`, mas a resposta não tem
  `maxChangeFilterDate`: leitura inteira desde 2000 e o hash do JSON de cada centro na coluna `json_hash`, montado do banco (F5,
  T9). Tabelas `TotvsCostCenters` (os enums `class`, `classType`, `indirectType` pelo nome, como vêm) e
  `TotvsCostCentersClassifications` (o `code` do pai vira `costCenterCode`);
* o helper `GetFiscalPages<T>` do service pagina os dois GET; `Script060`–`062` (procedures **13648–13650**), rotas
  `GET TotvsJobs/Totvs/InvoicesDisable` e `GET .../CostCenters`.

**Achados na chamada real (27/10/2026):**
* `cost-center` recusa `PageSize` acima de **100** (o swagger diz 1000), como o `promotion`; 48 centros (47 `Expense` /
  `Administrative`, o 999 "padrão" sem classe), nenhum inativo, nenhum com classificação; sem `IsInactive` vêm todos;
* `invoices/disable`: 119 faixas nos últimos 12 meses (a primeira em 17/06/2026, depois da migração), em 16 lojas, 87 de NF-e
  (55) e 32 de NFC-e (65), nenhuma chave repetida; todas com `maxChangeFilterDate`.

**Conferido com o console descartável** (API real e respostas sintéticas, banco falso):
* inutilizações: a 1ª execução grava as 119 em 23 chamadas (as fatias desde 01/01/2025); a 2ª faz 1 chamada e não grava nada;
  sintético, a faixa com série e modelo nulos é gravada com `''` e `0`, e a faixa invertida com CNPJ curto vai para o log (e é
  gravada, pela regra das leituras);
* centros de custo: a 1ª grava os 48; a 2ª não grava nada; com a descrição de um centro alterada, só ele é regravado;
* as colunas das 3 `DataTable` batem com o `Script060`.

**Não verificado:** os scripts `060`–`062` em banco e as rotas pelo host.

**Onda 1 concluída** (PE-1, PE-2, SE-1, PR-1, PR-2, SO-1 e FI-1), com o `GetChangedBatchItems` passado para a SO-2 e o
`PostOmniChangedBalances` reservado (PR2). A onda 2 (leituras sob demanda sem persistência: PE-3, PE-4, PR-3, PO-1, SO-2 e FI-2)
começa pela base T11 (§8.1).

### 10.11 Fase FI-2, parte 1: base das leituras sem persistência e rotas de arquivo (28/10/2026)

A FI-2 foi dividida em partes: esta entrega a base T11 (§8.1) e as três rotas de arquivo (§8.4 a §8.6); `GetSearchInvoicesItemDetail`,
`GetInvoicesPendingConditionalProducts` e `PostSearchInvoiceProducts` ficam para a parte 2.

**Base T11 (usada pelas outras fases da onda 2):**
* o `Result` da §1.1 ganhou `data` (o JSON da Totvs, como `JsonElement`, que o System.Text.Json do host serializa como objeto) e
  `file`/`fileName`/`contentType` (fora do JSON, `[JsonIgnore]`), para as rotas de arquivo (F8);
* `ReadResponse` no controller: 200 com o JSON ou o arquivo (`File(bytes, "application/pdf", nome)`), 404 quando a Totvs não acha, e
  o resto como as escritas (`WriteResponse`): 429 com `Retry-After`, 502 para 5xx, falta de resposta ou JSON inválido, 422 nos demais
  (empresa não cadastrada, 400 da Totvs com as mensagens);
* no service, `CallFiscalOnDemand` (uma nova tentativa em 401, 429 e 502/503/504, porque são consultas; a exceção é registrada em
  `TotvsRequests` antes de subir), `FiscalError` (o 400 `KeyNotFound` e o 404 viram 404; o resto segue o status da Totvs, com as
  mensagens), `CompanyNotFound` e `InvalidResponse`. `auditing.Records` não é gravado (não há registro integrado);
* nas rotas de arquivo, `TotvsRequests` guarda só um resumo (situação e tamanho de cada XML; tamanho do PDF; status e mensagem do
  Uniface), nunca o conteúdo; o `request_body` do DANFE leva só a chave e o tipo, sem o XML.

**Rotas** (no `TotvsInvoicesService`, jobs sob demanda no `Script063`, com `IntervalTimeInMinutes` 0):
* `GET TotvsJobs/Totvs/ElectronicInvoices/{docCompany}/{accessKey}/Xml` → `GetXmlContents` (13252): devolve o JSON da Totvs
  (`invoiceStatus`, `mainInvoiceXml` em base64 e, nas canceladas, `cancelInvoiceXml`);
* `GET .../ElectronicInvoices/{docCompany}/{accessKey}/Danfe?nfeDocumentType=1|2` → `PostSearchDanfe` (13253): lê o XML (a linha do
  `xml-contents` em `TotvsRequests` leva o job do DANFE) e o envia como veio (já é base64) ao `danfe-search`; devolve o PDF;
* `GET .../Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Print?invoiceDate=yyyy-MM-dd&template=N&type=1|2` →
  `PostTransactionPrint` (13254): todos os campos obrigatórios (F9: `invoiceDate` e modelo na rota); devolve o PDF; 200 da Totvs com
  `unifaceResponseStatus` diferente de `Success` ou sem conteúdo vira 422 com a mensagem do Uniface.
* `accessKey` com 44 dígitos e `docCompany` com 14 pela rota; `nfeDocumentType` e `type` só 1 ou 2 (`Range`, 400 do ASP.NET).

**Achados na chamada real (28/10/2026):**
* `xml-contents`: o `mainInvoiceXml` vem em **base64** (o que o `danfe-search` pede, sem recodificar); a situação vem em
  `invoiceStatus`, não no `processingType` do swagger; chave inexistente e NF-e **cancelada** dão **400 `KeyNotFound`** (não 404);
* `danfe-search`: a NF-e (55) devolve o PDF em 20 a 28 s (~355 KB); a **NFC-e (65) dá 400** "There is no default printer currently
  selected" (erro do servidor da Totvs), que a rota devolve como 422 com a mensagem;
* `transaction/print`: os modelos 1 e 2 existem e funcionam para NF-e e NFC-e (~12 s, PDF de ~13 KB, `unifaceResponseStatus`
  `Success`); modelo inexistente dá 400 "Modelo transação ... não cadastrado". A NF-e cancelada dá 400 "Situação da nota fiscal
  inválida para impressão".

**Conferido com o console descartável** (API real, sem banco): os 8 cenários acima (XML autorizada, inexistente, cancelada, empresa
não cadastrada; DANFE da NF-e e da NFC-e; impressão com modelo 1 e 999), com o `Result` serializado pelo System.Text.Json (`data`
como objeto, arquivo fora do JSON) e as linhas de `TotvsRequests` só com os resumos.

**Não verificado:** o `Script063` em banco e as rotas pelo host (o mapeamento do `ReadResponse` para 200/404/422/502 não foi
chamado por HTTP).

### 10.12 Fase FI-2, parte 2: itens da nota, condicional e produtos das notas (28/10/2026)

**O que entrou** (no `TotvsInvoicesService`, sobre a base T11 da §10.11; jobs sob demanda no `Script064`):
* `GetSearchInvoicesItemDetail` (13255): `GET TotvsJobs/Totvs/Invoices/{docCompany}/{branchCode}/{invoiceSequence}/Items?invoiceDate=`
  e `GET .../ElectronicInvoices/{docCompany}/{accessKey}/Items`, com `Expand=barcodes`; devolve o JSON da Totvs sem gravar (§8.7);
* `GetInvoicesPendingConditionalProducts` (13256): `GET .../ConditionalProducts/{docCompany}/{branchCode}?personCode=` ou
  `?personCpfCnpj=`, `&onlyPendingItem=true` (padrão); a loja precisa estar em `TotvsBranches` da empresa (422 sem chamar a API);
  `personCode` ou `personCpfCnpj`, um dos dois (400 na rota); `items` vazio vira 404 (§8.8);
* `PostSearchInvoiceProducts` (13257): `POST .../InvoiceProducts/{docCompany}/Search` com o corpo (`InvoiceProductSearch`, nos nomes do
  swagger: `ProductCodeList`, `BatchBarCodeList` e `acessKeyList` como lá), validado no controller pelo `InvoiceProductSearchValidator`
  (lojas informadas, um filtro restritivo, emissão com início e fim e até 31 dias, chaves com 44 dígitos, enums pelo nome, `pageSize`
  de 1 a 100); as lojas precisam ser da empresa (422); a página volta como veio (§8.9, F10);
* o helper `ReadFiscalJson` (a leitura que devolve o JSON) registra em `TotvsRequests` o corpo enviado e o recebido inteiros, e
  `BranchNotFound` devolve o 422 das lojas fora da empresa.

**Achados na chamada real (28/10/2026):**
* `item-detail-search` exige exatamente uma das duas formas da chave (nenhuma: 400 `AllEmptyField`; as duas: 400 `CannotBeInformed`);
  nota inexistente dá 400 `KeyNotFound` (a rota devolve 404); a resposta de uma nota de 1 item tem ~3 KB;
* `pending-conditional-products` exige `PersonCode` ou `PersonCpfCnpj`, um dos dois (400 `FieldsHaveNoValue` / `FieldsHaveValue`); sem
  saldo, responde 200 com `items` vazio;
* `invoice-products/search` exige `ProductCodeList`, `invoiceCodeList` ou `acessKeyList` (400 `EnterAtLeastOne`): a emissão sozinha
  não basta; os enums (`operationType`, `origin`, `modality`, `invoiceStatusList`) só aceitam o **nome** (o número dá 400
  `InvalidValue`), embora o swagger declare três deles como `integer`; 40 dias de emissão junto com a nota foram aceitos pela Totvs (o
  limite de 31 dias da F10 é nosso).

**Conferido com o console descartável** (API real, sem banco): itens pela chave da nota e pela chave de acesso (200, o mesmo JSON),
nota inexistente (404); condicional sem saldo pelo código e pelo CPF (404 com `data` = `{"items":[]}`) e loja fora da empresa (422 sem
chamada); produtos da nota 1914 com `isReturn` e `batchItems` (200 com a página) e lista com loja fora da empresa (422 sem chamada); o
validador com 7 corpos (válido, sem filtro restritivo, emissão só com início, 40 dias, enum pelo número + chave curta + página 500, sem
lojas, sem `filter`).

**Pendente:** o teste da F3 (se o `item-detail-search` traz as `referencedInvoices` das devoluções que dão 400 no `invoices/search`):
precisa de uma nota de devolução que falhe, que não apareceu nesta conferência. **Não verificado:** o `Script064` em banco e as rotas pelo
host.

### 10.13 Fases PE-3, PE-4, PR-3, PO-1 e SO-2: o resto da onda 2 (28/10/2026)

**Base T11 compartilhada:** a leitura sem persistência saiu do `TotvsInvoicesService` (FI-2) para o `ITotvsService`, onde os outros
services a usam: `ReadOnDemand` (nova tentativa em 401, 429 e 502/503/504; `TotvsRequests` com o corpo enviado e o recebido; o JSON em
`Result.data`; o 404 e o 400 `KeyNotFound`, `NotFound` ou `PathParamNotFound` viram 404; com `emptyListProperty`, a lista vazia também),
`CompanyNotFound` e `BranchNotFound`. O `TotvsQuery` monta a query string na própria rota (valores codificados, listas repetidas,
nulos de fora), porque os parâmetros de query do `IApiClient` são um dicionário. O `TotvsInvoicesService` continua com a cópia da FI-2
(mesmo comportamento); passar a usar a base comum é uma limpeza à parte.

**O que entrou** (jobs sob demanda no `Script065`, `IntervalTimeInMinutes` 0):

| Fase | Método (service) | Rota | Job | Observação |
|---|---|---|---|---|
| PE-3 | `GetBranches` (`TotvsPeopleService`) | `GET Branches/{docCompany}/{branchId}` (código de 1 a 4 dígitos ou CNPJ) | 13036 | não grava (PE1) |
| PE-3 | `GetPersonStatistics` (`TotvsPeopleService`) | `GET PersonStatistics/{docCompany}?customerCode=` ou `customerCpfCnpj=`, `&branchCode=` repetido | 13037 | `BranchCode` é obrigatório na Totvs: sem lojas informadas, vão as da empresa; loja de fora, 422 |
| PE-4 | `PostListBalanceBonus` (`TotvsBonusService`, novo, PE9) | `POST ListBalanceBonus/{docCompany}` com `ListBalanceBonus` validado | 13038 | lojas por código ou CNPJ, conferidas na empresa; token comum (acima) |
| PR-3 | `GetProducts` | `GET Products/{docCompany}/Codes/{code}?branchCode=` | 13100 | `products/{code}` responde igual com a loja no caminho, na query ou sem ela |
| PR-3 | `GetKardexMovement` | `GET Products/{docCompany}/Kardex?productCode&branchCode&startDate&endDate&balanceType` | 13101 | todos obrigatórios na Totvs; até 180 dias (400 na rota antes de chamar) |
| PR-3 | `GetGridFrequency` | `GET Products/{docCompany}/{productCode}/GridFrequency?page&pageSize` | 13102 | uma página por chamada (quem chama pagina), em vez de ler todas |
| PR-3 | `GetMeasurementConversions` | `GET Products/{docCompany}/MeasurementConversions?productCode=` ou `groupSequence=` | 13103 | um dos dois (a Totvs recusa nenhum e os dois); `products` vazio, 404 |
| PR-3 | `GetCompositionProduct`, `GetCompositionGroupProduct` | `GET Products/{docCompany}/CompositionProduct` e `.../CompositionGroupProduct` | 13104, 13105 | uma das listas; `groupCodeList` é o `groupSequenceCode` (inteiro) |
| PO-1 | `GetPurchaseOrderByKey` (`TotvsPurchaseOrdersService`) | `GET PurchaseOrders/{docCompany}/{branchCode}/{orderCode}` | 13185 | **grava** (DP4), cópia do `GetSalesOrderByKey`, com o `change` desde 2000 para o `maxChangeFilterDate` vir preenchido |
| SO-2 | `GetInvoices` (`TotvsSalesOrdersService`) | `GET SalesOrders/{docCompany}/{branchCode}/{orderCode}/Invoices?invoiceStatus=` | 13195 | sem notas, 404 |
| SO-2 | `GetPendingItems` | `GET .../PendingItems` | 13196 | sem pendência é 200 com `items` vazio (é a resposta) |
| SO-2 | `GetChangedBatchItems` (vindo da SO-1) | `GET .../ChangedBatchItems?batchCode&startDate&endDate` | 13193 | a Totvs exige loja, pedido e período: sem período, desde 2000; sem itens, 404 |

**Achados na chamada real (28/10/2026):**
* `branches/{id}` aceita o código e o CNPJ; loja inexistente dá 404 `PathParamNotFound`;
* `person-statistics` exige `BranchCode` (400 `EmptyField`) e um entre `CustomerCode` e `CustomerCpfCnpj` (os dois, ou nenhum: 400
  `CannotBeInformed`); cliente inexistente, 404;
* `list-balance-bonus`: responde com o token comum; CPF inexistente dá 400 `NotFound` (404 aqui); a loja pode ir por código ou CNPJ;
* `products/{code}`: código inexistente, 404 `PathParamNotFound`; `kardex-movement` exige `BalanceType` e recusa mais de 180 dias
  (400 `PeriodExceeds180Days`); `measurement-conversions` exige um entre `ProductCode` e `GroupSequence`; as composições exigem uma das
  listas, e `GroupCodeList` é inteiro (o código do grupo em texto dá 400);
* `purchase-order/v2/search` aceita `orderCodeList` e o `change` desde 2000 (com ele o `maxChangeFilterDate` vem preenchido; sem ele,
  nulo);
* `sales-order/v2/invoices` com pedido inexistente dá 400 `KeyNotFound` (404 aqui); `changed-batch-items` aceita o período desde 2000 e
  não há item de lote ligado a pedido na base.

**Conferido com o console descartável** (API real, com o `TotvsService` de verdade sobre um repositório falso, para a base comum entrar
no teste): 32 cenários, todos com o status esperado — loja por código, por CNPJ e inexistente; estatísticas sem lojas (todas as da
empresa), pelo CPF com lojas, de cliente inexistente e com loja de fora (422 sem chamada); bônus por código e CNPJ, de CPF inexistente
(404) e com loja e CNPJ de fora (422); o validador do bônus com 3 corpos; produto por código, com loja e inexistente; kardex e kardex
com loja de fora; grade de frequência; conversões vazias (404); as duas composições; pedido de compra 1/55 gravado com o
`maxChangeFilterDate`; o 1/999999999, que **existe** na base (pedido de 29/09/2026), gravado; notas do pedido de venda 1/2293, com
filtro de situação e de pedido inexistente (404); itens pendentes vazios (200); itens de lote vazios (404); empresa não cadastrada (422).

**Não verificado:** o `Script065` em banco e as rotas pelo host.

**Onda 2 concluída** (PE-3, PE-4, PR-3, PO-1, SO-2 e FI-2), menos o teste da F3. A onda 3 (escritas de baixo risco: PE-5, PR-4, PO-2 e
SO-3; a AP-1 já está feita) depende da T1/T2 (homologação e autenticação dos endpoints de inbound).

### 10.14 Fase PE-5: mensagens da pessoa e a base comum das escritas (28/10/2026)

**Base comum das escritas** (no `ITotvsService`, para as fases daqui em diante; a `PostDuplicatesInstallmentsStage` da AP-1 continua com
a chamada no próprio método):
* `Write(company, job, user, key, method, route, body, checkHint, repeatLock)`: o fluxo da §1.2 inteiro (corpo sem os nulos, nova tentativa
  só em 401 e 429, registro em `TotvsRequests` com o corpo enviado, o recebido e o status, mensagens da Totvs no `Result`). `checkHint`
  completa a mensagem quando não dá para saber se a Totvs gravou (5xx, exceção); `body` nulo manda a chamada sem corpo (os `DELETE`, com os
  campos na query, que fica na rota gravada);
* `WriteDisabled(grupo, key)`: o 422 da escrita desligada (T4), sem chamada;
* **trava de repetição (T5)**, ligada por `repeatLock` nas escritas não idempotentes: a mesma rota + chave + corpo nos últimos 10 minutos
  que a Totvs aceitou, ou que ficou sem resposta certa (5xx ou exceção), volta **409** sem chamar a API. A recusa da Totvs (4xx) e o login
  que falhou não travam: nada foi gravado. A consulta (`CreateGetRecentRequestsQuery`) filtra por empresa, chave, rota e data (o índice
  `IX_TotvsRequests_Company_Key` atende) e o corpo é comparado no service, sem entrar no SQL;
* `Script066`: coluna `status_code` em `totvs.TotvsRequests` (status da Totvs; **0** quando a chamada lançou exceção; nulo sem chamada à
  rota e nos registros anteriores). As leituras sob demanda (`ReadOnDemand`) passam a gravar o status também;
* `WriteResponse` do controller devolve **409** na repetição recusada; as rotas novas recebem o validador por `[FromServices]`, sem
  aumentar o construtor do controller.

**PE-5** (§3.4.4, no `TotvsPeopleService`, grupo `Totvs:Writes:People:Enabled`, desligado no `appsettings.json`; jobs no `Script067`):
* `PostMessages` (13044): `POST TotvsJobs/Totvs/Messages/{docCompany}` com o `InsertMessagePerson` → `POST person/v2/messages`; chave = pessoa
  (código ou CPF/CNPJ); **com a trava T5** (repetir inclui as mensagens de novo). A resposta (201, as `sequence`) volta em `response` e fica
  em `TotvsRequests`, a única fonte das `sequence` (PE8);
* `PutChangeMessages` (13045): `PUT .../Messages/{docCompany}/Change` com o `UpdateMessagePerson` → `PUT person/v2/messages/change`; chave =
  pessoa `|` `sequence`; sem trava (valores absolutos); `status` vai como número (`MessageStatusType` 1, 2, 3);
* `DeleteMessages` (13046): `DELETE .../Messages/{docCompany}/{personCode}/{sequence}` → `DELETE person/v2/messages?PersonCode=&Sequence=`,
  sem corpo e sem o `LastChangeUser`; chave = `personCode|sequence`;
* validadores `InsertMessagePersonValidator` (pessoa: exatamente um entre código, até 9 dígitos, e CPF/CNPJ, 11 ou 14 dígitos; pelo menos uma
  mensagem; `description` obrigatória e até 200; **`priority` obrigatória** e maior que zero, porque o swagger a declara inteiro não anulável
  e a omissão mandaria 0) e `UpdateMessagePersonValidator` (`filter` com a pessoa e a `sequence`; pelo menos um entre `description`,
  `status` e `priority`; `description` até 200, o tamanho do `POST`).

**Conferido sem a API** (console descartável com uma Totvs falsa e `TotvsRequests` em memória, o `TotvsService` de verdade):
* validadores: 10 corpos (válidos, sem pessoa, as duas pessoas, CPF curto, sem mensagens, descrição de 201 sem prioridade, `PUT` sem
  `filter`, sem `sequence` e sem campo, `status` 9), cada um com a mensagem certa;
* corpo enviado sem nulos (`{"personCode":12345,"messages":[...]}`) e `status` como número no `PUT`; `DELETE` com os campos na query e
  sem corpo;
* fluxo: 201 → 200 com a resposta e `status_code` 201 gravado; **o mesmo corpo de novo → 409 sem chamar**; outro corpo da mesma pessoa →
  chama; 400 → mensagem `code: message` e a repetição **não** trava; 502 → 1 chamada só e a repetição → 409 ("pode ter sido feita");
  exceção → `status_code` 0 e a repetição → 409; 401 → token renovado; 429 duas vezes → 429 com `Retry-After`; login que falhou → sem
  chamar a rota e sem travar a próxima; escrita desligada e empresa não cadastrada → sem chamada e sem registro; `PUT` repetido chama
  de novo (sem trava).

**Não verificado:** a chamada real em homologação (T1: precisa do usuário de integração, §1.3), o formato do `status` (número, D6) e se
a `priority` é mesmo obrigatória; o `Script066`/`Script067` em banco e as rotas pelo host.

### 10.15 Fase PR-4: cores e grades (28/10/2026)

**O que entrou** (§5.5, no `TotvsProductCatalogsService`, pela base da §10.14; grupo `Totvs:Writes:Products:Enabled`, desligado; jobs no
`Script068`):
* `PostColors` (13124): `POST TotvsJobs/Totvs/Products/{docCompany}/Colors` com o `UpsertColor` → `POST product/v2/colors`; chave = `code`.
  É inclusão **e** alteração (um `code` que já existe é renomeado em todas as referências): valores absolutos, sem trava. As lojas de
  `colorInfoAdditionals` precisam ser da empresa em `TotvsBranches` (422 sem chamar a API, passo 3 da §1.2);
* `PostGrids` (13126): `POST .../Products/{docCompany}/Grids` com o `InsertGrid` → `POST product/v2/grids`; chave = `gridCode` ou, sem ele,
  `name`. Só inclusão e sem rota para desfazer: **com a trava T5** (repetir sem `gridCode` criaria outra grade). A resposta (201,
  `gridCode` e `name`) volta em `response`;
* validadores `UpsertColorValidator` (`code` até 10 e `name` até 40, obrigatórios; por loja: `branchCode` obrigatório com até 4 dígitos e
  sem repetição, `auxiliaryCode` 10, `auxiliaryName` 80, `pantoneCode` 40) e `InsertGridValidator` (`gridCode` opcional com até 6 dígitos,
  `name` obrigatório até 40, `type` até 40, pelo menos um tamanho, cada um obrigatório com até 10 e sem repetição, sem diferença de
  maiúsculas e espaços).

**Conferido sem a API** (mesmo console da §10.14): 7 corpos nos validadores; corpo da cor e da grade sem nulos (a grade sem `gridCode`); cor
aceita e repetida (chama de novo); cor com uma loja de outra empresa (422 sem chamar, com a loja na mensagem); cor sem dados por loja; grade
aceita com o `gridCode` na resposta, repetida (**409 sem chamar**) e com código existente (400 da Totvs como mensagem).

**Não verificado:** a chamada real em homologação (T1); se a Totvs numera a grade sem `gridCode` (o swagger declara o campo como inteiro
não anulável); o `Script068` em banco e as rotas pelo host.

### 10.16 Fase PO-2: situação do pedido de compra (28/10/2026)

**O que entrou** (§6.3, no `TotvsPurchaseOrdersService`, pela base da §10.14; grupo `Totvs:Writes:PurchaseOrders:Enabled`, desligado; job no
`Script069`):
* `PostPurchaseOrdersChangeStatus` (13182): `POST TotvsJobs/Totvs/PurchaseOrders/{docCompany}/ChangeStatus` com o `UpdateOrderStatus`
  (`branchCode`, `orderCode`, `status` 1 em andamento ou 5 bloqueado, como número) → `POST purchase-order/v2/orders/change-status`; chave =
  `branchCode|orderCode`; sem trava (valor absoluto);
* a loja precisa ser da empresa (422 sem chamar); **conferência local (DP7)** pela nova `GetPurchaseOrderStatus` (situação em
  `totvs.TotvsPurchaseOrders`): com o pedido no banco, só em andamento (1) → bloqueado (5) e bloqueado → em andamento chegam à Totvs; a
  situação igual à pedida ou qualquer outra (atendido, cancelado, parcialmente recebido...) volta 422 com o nome da situação;
* `UpdateOrderStatusValidator`: `branchCode` (até 4 dígitos), `orderCode` (até 9) e `status` (1 ou 5) obrigatórios, embora o swagger não
  marque nenhum.

**Achado na leitura real (28/10/2026):** o `purchase-order/v2/search` devolve a situação como **número** (`StatusOrderType`: nos 116 pedidos
da loja 1 alterados em 2026, `4` 60, `1` 17, `3` 11, `5` 11 e `6` 1), diferente das outras APIs, que devolvem o nome; é o número que fica
em `TotvsPurchaseOrders.status`, e a conferência local compara por ele. O `Responsed Jsons` do pedido de compra já mostrava `"status": 5`.

**Conferido sem a API** (mesmo console da §10.14): 3 corpos no validador; corpo com a situação como número; pedido fora do banco (chama);
em andamento → bloquear e bloqueado → desbloquear (chamam); bloqueado → bloquear, atendido → desbloquear e atendido → bloquear (422 sem
chamar, com as situações por extenso); loja de outra empresa (422 sem chamar); pedido inexistente (400 da Totvs como mensagem).

**Não verificado:** a chamada real em homologação (T1); se a Totvs aceita bloquear um pedido parcialmente recebido ou liberado para baixa
(hoje a conferência local recusa); o `Script069` em banco e a rota pelo host.

### 10.17 Fase SO-3: dados adicionais, transporte, observações e itens do pedido de venda (28/10/2026)

**O que entrou** (§7.4, no `TotvsSalesOrdersService`, pela base da §10.14; grupo `Totvs:Writes:SalesOrders:Enabled`, desligado; jobs no
`Script070`, com `SalesOrder` depois do verbo, SO9):
* **chave do pedido:** `OrderKey` (`branchCode` + `orderCode` **ou** `orderId`), base dos quatro corpos, com o `OrderKeyValidator` incluído
  pelos validadores de cada rota (um ou outro; loja e número juntos; até 4, 9 e 100). Chave em `TotvsRequests` = `branchCode|orderCode` ou o
  `orderId`. Com loja + número, a loja precisa ser da empresa (422 sem chamar); com `orderId` não há loja a conferir;
* `PostAdditionalOrder` (13201): `POST TotvsJobs/Totvs/SalesOrders/{docCompany}/Additional` → `POST sales-order/v2/additional-order`;
  `entryType` vai como **letra** (`N`/`S`, SO11) e `experienceType` como número (1 a 5); pelo menos um campo além da chave; sem trava;
* `PostShippingOrder` (13202): `POST .../SalesOrders/{docCompany}/Shipping` → `POST sales-order/v2/shipping-order`; transportadora e
  redespacho por código **ou** CPF/CNPJ, cada um exigindo o seu tipo de frete (`freightType` 1 a 6, `redispatchingFreightType` 1 ou 2, e
  este só com o redespacho); `freightPercentage` 3,6 e `freightValue` 12,2 (`PrecisionScale`); endereço com CEP de 8 dígitos e os tamanhos
  do swagger; sem trava (valores absolutos);
* `PostObservationsOrder` (13203): `POST .../SalesOrders/{docCompany}/Observations` → `POST sales-order/v2/observations-order`; pelo menos
  uma linha (do pedido ou da nota), cada uma obrigatória com até 80; `visualizationType` obrigatório (enum não anulável no swagger);
  **com a trava T5** (repetir duplica as linhas);
* `PostAdditionalItems` (13207): `POST .../SalesOrders/{docCompany}/Items/Additional` → `POST sales-order/v2/additional-items`; produto por
  código **ou** SKU, sem repetir, e `rollMeasureCode` ou `billingForecastDate` em cada item; sem trava.

**Conferido sem a API** (mesmo console da §10.14): 19 corpos nos validadores (chave nos quatro formatos errados, campos e tamanhos de cada
rota, as regras de transportadora e redespacho, casas decimais, linhas e itens repetidos); os corpos enviados sem nulos, com `entryType`
`"S"`, `experienceType` 2, `freightType` 2, `visualizationType` 2 e a data ISO (`2026-11-05T00:00:00`); dados adicionais aceitos e
repetidos (chama de novo), com loja de outra empresa (422 sem chamar) e por `orderId` (sem conferência de loja, chave = `orderId`);
transporte aceito; observações aceitas, **repetidas (409 sem chamar)** e outra linha no mesmo pedido (chama); itens aceitos e item fora
do pedido (400 da Totvs como mensagem).

**Não verificado:** a chamada real em homologação (T1), inclusive o formato do `entryType` (letra) e se toda alteração muda a data usada
pelo `filter.change` do `orders/search` (§7.4); o `Script070` em banco e as rotas pelo host.

**Onda 3 concluída** (AP-1, PE-5, PR-4, PO-2 e SO-3), toda conferida só com a Totvs falsa: falta a rodada em homologação (T1, com o
usuário de integração da §1.3) e a autenticação dos endpoints de inbound (T2) antes de ligar qualquer grupo de escrita em produção. A onda 4
(AP-2, PE-6, PR-5, PO-3, SO-4) depende disso; a T6 (mascaramento), que a PE-6 pedia para os clientes, entrou em seguida (§10.18).

### 10.18 T6: mascaramento dos dados pessoais em `TotvsRequests` (28/10/2026)

Adiantada da onda 4 porque não depende da homologação: a PE-6 (clientes PF e PJ) grava CPF, RG, CNH, passaporte, conta e PIX no
`request_body`, e as leituras sob demanda de cliente (`GetCustomerByCode`) já gravavam o cadastro inteiro no `response_body`.

**Onde:** `TotvsMasking.Body` (`Application.Totvs/Services`), chamado pelo `TotvsService.SaveRequest` nos dois corpos. Como toda chamada
registrada passa pelo `SaveRequest` (leituras sob demanda, escritas pela base comum, o fluxo próprio da AP-1 e a cópia da base T11 no
`TotvsInvoicesService`), nenhum service mudou.

**Regra:**

| Campo (nome no JSON, sem diferença de maiúsculas, em qualquer nível) | Máscara |
|---|---|
| `cpf`, `personCpf`, `sellerCpf`, `rg`, `rgIe`, `ctps`, `ctpsSerial`, `pisNumber`, `ricNumber`, `passportNumber`, `foreignDocumentNumber`, `voterRegistrationNumber`, `driverLicenseNumber`, `driverLicenseRegistrationNumber`, `driverLicenseSecurityNumber`, `accountNumber`, `accountDigit`, `pixKey` | sempre |
| `*CpfCnpj`, `*CpfCnpjList` e `cpfPartner` | só quando o valor é CPF (11 dígitos, com ou sem pontuação): CNPJ é dado da empresa e fica aberto |

* O valor vira `*` com os **2 últimos caracteres** (`*********01`), para conferir com quem chamou; valores de até 4 caracteres (o dígito da
  conta) e finais com escape do JSON somem inteiros. Número vira texto (`12345678901` → `"*********01"`); `null`, vazio e booleano ficam.
* Trabalha sobre o **texto**, sem desserializar: vale também na resposta cortada em 800 caracteres e não muda o resto do corpo
  (espaços, ordem, casas decimais). Listas de valores (`personCpfCnpjList`) são mascaradas item a item; em listas de objetos
  (`bankAccounts`), os campos de dentro.
* **A rota e o `field_key_value` ficam inteiros.** Eles são a busca do registro, e a trava T5 compara a chave: com a chave mascarada, dois
  CPFs com o mesmo final e a mesma mensagem (envio em lote) seriam tomados por repetição. Hoje levam CPF a chave das mensagens e das
  estatísticas da pessoa sem `personCode`, a do saldo de bônus, a dos itens pendentes de condicional sem `personCode` e a da troca de estágio com
  `supplierCpfCnpj`, e a rota das estatísticas (`CustomerCpfCnpj` na query).
* **Trava T5:** o banco guarda o corpo mascarado, então a comparação usa o novo corpo com a mesma máscara; a chave inteira separa o que a
  máscara igualaria.
* `auditing.Logs` (mensagens com a chave) e a resposta devolvida a quem chamou não mudam: a T6 trata só de `TotvsRequests`.

**Testes (console de rascunho):** CPF texto e número, com espaços e maiúsculas; `*CpfCnpj` com CPF formatado (mascarado) e com CNPJ (fica);
lista mista de CPF e CNPJ; conta, dígito e PIX dentro de `bankAccounts`; documentos dentro de `individualAdditionals` (o `socialName` fica);
`null`, vazio e booleano; escape no fim; JSON cortado; texto de exceção; `PostMessages` por CPF grava os corpos sem o CPF e a chave inteira,
envia o CPF inteiro à Totvs, devolve 409 na repetição e chama para outro CPF com o mesmo final. As fases da onda 3 rodaram de novo sem
falha.

**Não verificado:** os registros que já estão em `TotvsRequests` continuam sem máscara (nenhum script os reescreve); o host e o banco.

### 10.19 Rodada da onda 3 em homologação (28/10/2026)

Feita com o usuário pessoal `cayo.kume` (T1), por decisão do dono do usuário, num console de rascunho que chama os services de verdade
(`TotvsService.Write`, trava T5, máscara T6) contra o gateway de homologação da §1.3, com as tabelas em memória. As credenciais foram
digitadas por ele no terminal e não ficaram gravadas. Três rodadas; antes de cada uma, a descoberta só leu a homologação e a escrita esperou
confirmação.

| Fase | Rota | Resultado |
|---|---|---|
| PE-5 | `POST person/v2/messages` | 201; a resposta traz `sequence` e a descrição em maiúsculas. Repetida em 10 min: **409 sem chamar** (T5). Sem `priority`, a Totvs aceita e grava **0**: o validador continua exigindo a prioridade |
| PE-5 | `PUT person/v2/messages/change` | Sem `description`: **400 `EmptyField`**, mesmo só mudando a situação. **Mudança:** o `UpdateMessagePersonValidator` passou a exigir a descrição (antes bastava um campo qualquer). Com ela: 200, para lida (2) e de volta para não lida (1) |
| PE-5 | `DELETE person/v2/messages` | 200 com a mensagem **não lida**; lida: **400 `StatusNotPermited`**. Regra anotada no controller, na interface e no README |
| PR-4 | `POST product/v2/colors` | 201 na inclusão e na alteração pela mesma rota (relida com o nome novo). Cor `ZZT01` fica na homologação |
| PR-4 | `POST product/v2/grids` | 201 sem `gridCode`: **a Totvs numera** (devolveu 29). Grade "GRADE TESTE INTEGRACAO" fica na homologação |
| PO-2 | `POST purchase-order/v2/orders/change-status` | Pedido 1\|56: em andamento → bloqueado (5) → em andamento (1), cada troca relida no `purchase-order/v2/search`. Pedido terminou como estava |
| SO-3 | `POST sales-order/v2/additional-order` | 200 com `trackingId` e `entryType` **`N` (letra)**, no pedido 1\|1821 (bloqueado, do e-commerce) |
| SO-3 | `POST sales-order/v2/shipping-order` | Pedido bloqueado e do e-commerce: **400 `InvalidStatusUpdate` e `CannotChangedOrderEcommerce`**. Em andamento, fora do e-commerce (106\|266): 200, `freightType` 2 relido |
| SO-3 | `POST sales-order/v2/observations-order` | Do e-commerce: **400 `CannotChangedOrderEcommerce`**. 106\|266: 200 (linha relida); repetida: **409 sem chamar** |
| SO-3 | `POST sales-order/v2/additional-items` | Do e-commerce: **400 `CannotChangedOrderEcommerce`**. 106\|266: 200, `billingForecastDate` relida |

**Outras observações:**
* A busca de pedidos de venda aceita `filter.orderStatusList` **pelo nome** (`InProgress`, `BillingReleased`, `Blocked`, `InAnalysis`).
* As recusas da Totvs chegam como 422 com `code: message`, como previsto; nenhuma precisou de conferência local nova.
* Ficaram na homologação: a cor `ZZT01`, a grade 29, o `trackingId` "TESTEINTEGRACAO" no pedido 1\|1821 (o valor anterior não foi lido)
  e uma observação "TESTE INTEGRACAO - PODE APAGAR" no pedido 106\|266. As mensagens de teste foram excluídas.

**Não verificado:** a AP-1 em homologação (a busca não devolve o estágio da parcela, então a troca não teria como voltar ao original);
bloquear pedido de compra parcialmente recebido (a volta iria para em andamento, não para o estado original); `experienceType`; os
`Script066`–`070` em banco e as rotas pelo host.

**Situação da onda 4:** a condição "onda 3 testada" (§9.3) está cumprida para PE-5, PR-4, PO-2 e SO-3. Continua valendo a T2 para publicar
qualquer escrita em produção.

### 10.20 Fase AP-2: inclusão e agrupamento de duplicatas (28/10/2026)

Primeira fase da onda 4, já pela base comum das escritas (`ITotvsService.Write`, com a trava T5 e a máscara T6); a AP-1 continua com o
fluxo próprio (D5).

| Peça | Onde |
|---|---|
| DTOs `Duplicate`, `Installment`, `Expense`, `ObservationInstallment`, `BillingAccept`, `DuplicateClassification`, `DuplicateField`, `TaxAdditional`, `TaxDuplicate`, `BusinessUnit`, `GroupDuplicate`, `GroupInstallment` | `Domain.Totvs/Dtos/Writes/AccountsPayable` |
| Enums `DocumentType` (1 a 42 e 50 a 55), `PrevisionType`, `StageType` (1 a 5, 10, 20, 30, 40 a 43, 70, 71, 90), `AdvanceDiscountType` (0 a 7), `StatusTaxDuplicateType`, `RetentionType`, `GroupDocumentType` (1, 18) | idem, com os números da descrição do swagger (T3) |
| `DuplicateValidator` (com `InstallmentValidator`), `GroupDuplicateValidator` | `Application.Totvs/Validations/Writes/AccountsPayable` |
| `PostDuplicates` (13018) e `PostGroupDuplicates` (13019), as duas com a trava T5 | `TotvsAccountsPayableService` |
| `POST Duplicates/{docCompany}` e `POST GroupDuplicates/{docCompany}` | `TotvsController` |

Os jobs já estavam no `Script032` (reservados na AP-1): a fase não tem script.

**Regras:**
* Chave em `TotvsRequests`: `branchCnpj|supplierCpfCnpj|duplicateCode` (no agrupamento, a duplicata nova). O CPF do fornecedor fica inteiro
  na chave e mascarado no corpo gravado (T6).
* Loja: o `branchCnpj` do corpo e, no agrupamento, o de cada parcela agrupada precisam estar em `totvs.TotvsBranches` da empresa (422 sem
  chamar, com os CNPJs de fora). O `branchCnpj` dos impostos (`taxDuplicates`) não é conferido: a Totvs valida.
* Validador: os campos que o swagger declara sem `nullable` ficam anuláveis no DTO e obrigatórios no validador, também nos filhos
  opcionais (`classificationType`, `typeFieldCode`, `revenue`, `calculationPeriod`, `competenceDate`, `isThirteenthSalary`, `tax`,
  `ratePercentage`, `taxValue`, `status`, `proratedPercentage` da unidade de negócio): omitidos, iriam como 0, `false` ou `0001-01-01`.
* Despesa: rateio por `proratedPercentage` **ou** `proratedValue`; com todas por percentual, a soma da parcela fecha em 100. Campo
  adicional com o dado em exatamente um entre texto, número e data.
* Agrupamento: pelo menos duas parcelas, sem repetição; `document` só 1 ou 18.

**Testes (console de rascunho, Totvs falsa):** validadores (corpo válido, vazios, parcela repetida, tamanhos, casas decimais, despesa com os
dois rateios, soma 90, rateio por valor, vencimento antes da emissão, enums fora do swagger, filhos sem os campos não anuláveis; agrupamento
com uma parcela, documento 7, parcela repetida); serialização (enums como número, datas ISO, sem nulos); fluxo: aceita (201), repetida em
10 min (409 sem chamar), loja de outra empresa (422 sem chamar), 400 da Totvs (422 com `code: message`) que não trava a repetição, 502 sem
nova tentativa e com a dica do `duplicates/search`, fornecedor pessoa física (CPF inteiro na chave, mascarado no corpo gravado), escrita
desligada; agrupamento aceito com a chave gerada na resposta, repetido (409) e com parcela de loja de fora (422 só com o CNPJ de fora).

**Não verificado:** a chamada em homologação (cria duplicata de verdade no financeiro da homologação: fica para a rodada da onda 4); se a API
aceita agrupar uma parcela só (§2.2).
