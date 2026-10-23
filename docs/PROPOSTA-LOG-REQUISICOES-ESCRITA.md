# Proposta — log das requisições de escrita (POST/PUT)

Documento para revisão. **Nada foi alterado no código ainda.**

Objetivo: registrar, para cada chamada às rotas de escrita do `AfterSaleController`, **quem**
pediu a alteração (usuário) e **o JSON da requisição**, ligado à auditoria que já existe
(`auditing.Logs`).

Rotas cobertas hoje:

| Endpoint nosso | Método | Job (`EnumJob`) |
|---|---|---|
| `Reverses/{docCompany}/{id}/Approve` | POST | `AfterSaleApproveReverse = 5005` |
| `Reverses/{docCompany}/{id}/ReceiveProducts` | PUT | `AfterSaleReciveProducts = 5006` |
| `Reverses/{docCompany}/{id}/ReturnedInvoice` | PUT | `AfterSaleReturnedInvoice = 5007` |
| `Reverses/{docCompany}/{id}/Invoice` | POST | `AfterSaleReverseInvoice = 5008` |
| `Reverses/{docCompany}/{id}/InvoiceXml` | POST | `AfterSaleReverseInvoice = 5008` |
| `Reverses/{docCompany}/{id}/AdministrativeRecords` | POST | `AfterSaleAdministrativeRecordsReverse = 5021` |

---

## 1. O que já existe hoje

Cada uma dessas chamadas já passa pelo `SendReverseAction` (em `AfterSaleReversesService`), que:

1. abre um log em `auditing.Logs` com o job da ação (`IdJob` + `IdType = 2`, Integração);
2. grava em `auditing.Records` um registro com `{ "request": ..., "response": ... }`;
3. grava mensagens em `auditing.Messages` e, se a API não devolver sucesso, um erro em
   `aftersale.AfterSaleErrors`.

**O que falta:** o **usuário** que pediu a alteração, e um lugar próprio e consultável para a
requisição. Hoje o JSON vai misturado no `RecordText` de `auditing.Records`, que é a mesma tabela
usada pelos jobs de leitura para guardar cada registro integrado.

---

## 2. Onde gravar — opções

| Opção | Como | Prós | Contras |
|---|---|---|---|
| **A. Tabela nova em `auditing` (recomendada)** | `auditing.Requests`, genérica, com FK para `auditing.Logs` | Fica junto da auditoria, com o mesmo `Execution` do log; serve para qualquer módulo (os POSTs de reembolso e de outras integrações já podem usar); é gravada pelo logger, num ponto só | Exige mudança no projeto Shared (entidade, logger e repositório) |
| B. Tabela nova em `general`, só do AfterSale | `aftersale.AfterSaleReversesRequests`, pelo fluxo `untreated` → merge | Isolada no módulo | O fluxo `untreated` + procedure de merge não faz sentido para um log que só recebe inserts; cada módulo novo precisaria da sua tabela |
| C. Reaproveitar `auditing.Records` | Colocar o usuário dentro do JSON do `RecordText` | Nenhuma tabela nova | Usuário não fica em coluna própria (não dá para filtrar/indexar), e mistura com os registros dos jobs de leitura |

**Recomendação: opção A.**

---

## 3. Tabela proposta — `auditing.Requests`

Mesmo estilo de `auditing.Records` e `auditing.Messages` (PK com identidade + chave do log, FK
composta para `auditing.Logs`):

```sql
CREATE TABLE [auditing].[Requests](
	[IdRequest] [bigint] IDENTITY(1,1) NOT NULL,
	[IdJob] [int] NOT NULL,
	[IdType] [int] NOT NULL,
	[Execution] [uniqueidentifier] NOT NULL,
	[RequestDate] [datetime] NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[HttpMethod] [varchar](10) NOT NULL,
	[Route] [nvarchar](400) NOT NULL,
	[DocCompany] [nvarchar](14) NULL,
	[FieldKeyValue] [nvarchar](400) NULL,
	[RequestBody] [nvarchar](max) NULL,
	[ResponseBody] [nvarchar](max) NULL,
	[IsSuccess] [bit] NOT NULL,
 CONSTRAINT [PK_Requests] PRIMARY KEY CLUSTERED 
(
	[IdRequest] ASC,
	[IdJob] ASC,
	[IdType] ASC,
	[Execution] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF, DATA_COMPRESSION = ROW) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [auditing].[Requests] ADD CONSTRAINT [FK_Requests] FOREIGN KEY([IdJob], [IdType], [Execution])
REFERENCES [auditing].[Logs] ([IdJob], [IdType], [Execution])
GO

CREATE NONCLUSTERED INDEX [IX_Requests_UserName_RequestDate] ON [auditing].[Requests] ([UserName], [RequestDate])
GO
```

| Coluna | Conteúdo | Exemplo |
|---|---|---|
| `IdJob`, `IdType`, `Execution` | Ligação com `auditing.Logs` (mesmo log da execução) | `5005`, `2`, `<guid>` |
| `RequestDate` | Data/hora (horário de Brasília, como as demais tabelas) | `2026-10-16 10:32:00` |
| `UserName` | Quem pediu a alteração (ver seção 4) | `joao.silva` |
| `HttpMethod` | Método chamado na API externa | `POST` |
| `Route` | Rota chamada na API externa | `v3/api/reverses/4926/approve` |
| `DocCompany` | CNPJ da empresa | `38367316000199` |
| `FieldKeyValue` | Chave do registro alterado (mesmo nome usado em `Records`) | `4926` (id da reversa) |
| `RequestBody` | JSON enviado (já sem os campos nulos) | `{"comments":"...","products":[...]}` |
| `ResponseBody` | Resposta da API | `{"success":true,"status":{...}}` |
| `IsSuccess` | Se a API devolveu `success: true` | `1` |

> Coloquei `DocCompany` e `FieldKeyValue` como colunas próprias porque são os filtros mais
> prováveis ("o que foi alterado na reversa X da empresa Y"). Se preferir uma tabela 100% genérica,
> os dois podem sair, já que estão dentro do `RequestBody`/`Route`.

**Onde criar:** novo script no Shared, já que a tabela é da auditoria (que é criada no
`Script001` do Shared):
`src/Backend/Shared/Infrastructure.Shared/Scripts/SQLServer/Script008_Create_Requests_Table.sql`.

---

## 4. De onde vem o usuário

Hoje o `AfterSaleController` não tem autenticação (`[Authorize]` está comentado), então a API não
sabe quem chamou. Opções:

| Opção | Como | Observação |
|---|---|---|
| **A. Cabeçalho obrigatório (recomendada agora)** | O sistema externo envia `X-User-Name: joao.silva` em toda chamada POST/PUT; sem ele, 400 | Simples e funciona já. A confiança é no sistema que chama (ele é quem autentica o usuário). |
| B. Campo no corpo | `"user": "joao.silva"` em cada request | Mistura dado de auditoria com o corpo que vai para a API AfterSale (teria que ser removido antes de enviar). |
| C. Token (JWT) | Habilitar `[Authorize]` e ler o usuário das *claims* | O ideal a médio prazo, mas depende de configurar o provedor de identidade (há um `AzureAd` comentado no `BuilderExtensions`). |

**Recomendação:** criar uma abstração `ICurrentUserService` no `Application.Shared`, com
`string? UserName`, implementada no host lendo o cabeçalho `X-User-Name`. Assim:

- o service do AfterSale não depende de `HttpContext` nem precisa de um parâmetro `user` em cada
  método;
- quando o JWT for habilitado, só a implementação muda (passa a ler a *claim*), sem mexer nos
  services.

A validação do cabeçalho obrigatório pode ficar num filtro de ação aplicado só às rotas POST/PUT.

---

## 5. Como gravar — fluxo proposto

Seguindo o padrão atual do logger (o log é montado em memória e gravado no `CommitAllChanges`):

1. **Entidade** `Domain.Shared/Entities/Auditing/Request.cs`, com as colunas da tabela (sem
   `IdRequest`, que é identidade).
2. **`Log.Requests`** (`[SkipProperty] List<Request>`), como já existe `Log.Records` e
   `Log.Messages`.
3. **`ILoggerService.AddRequest(userName, httpMethod, route, docCompany, key, requestBody, responseBody, isSuccess)`**
   preenchendo `IdJob`, `IdType` e `Execution` a partir do log aberto.
4. **`LoggerRepository.LogInsert`**: depois do insert em `Logs` (por causa da FK), bulk insert em
   `auditing.Requests`, igual ao que já faz com `Records` e `Messages`.
5. **`SendReverseAction`** (service do AfterSale): troca o `AddRecord(id, {request, response})`
   por `AddRequest(...)`, com o usuário vindo de `ICurrentUserService`. Hoje o `SendReverseAction`
   recebe só uma função que chama a API; ele passaria a receber também o método e a rota, para
   gravar nas colunas `HttpMethod` e `Route`.

Pontos de atenção:

- **Falha antes da resposta** (timeout, erro de rede): hoje uma exceção no `send` sobe sem
  registrar a requisição. Proposta: capturar no `SendReverseAction`, gravar o `Request` com
  `IsSuccess = 0` e a mensagem da exceção em `ResponseBody`, e relançar.
- **Rota `InvoiceXml`**: o corpo é um arquivo XML. Proposta: gravar em `RequestBody` só os
  metadados `{"id": 4926, "xml": "nota.xml", "size": 18452, "sha256": "..."}` e não o conteúdo do
  arquivo. Se você quiser guardar o XML inteiro (para reprocessar ou provar o que foi enviado),
  dá para gravar o conteúdo, já que a coluna é `nvarchar(max)`; uma NF-e costuma ter dezenas de KB.
- **Dados pessoais**: os corpos atuais (comentários, quantidades, chaves de NF-e) não trazem dados
  pessoais de clientes, mas o campo `comments` é texto livre. Vale lembrar disso se a tabela for
  exposta em alguma tela.

---

## 6. Registro na auditoria (`auditing.Jobs`)

Li o [Script007_Insert_AfterSale_Values_Into_Jobs_Table.sql](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/Script007_Insert_AfterSale_Values_Into_Jobs_Table.sql).

**Os cinco jobs das rotas de escrita já estão cadastrados**, nas linhas 56–60, dentro do bloco
`/*Integrações*/` (faixa 5000–5499), com `IdParent = 50` (app AfterSale) e `IdType = 2`
(Integração):

```sql
(50, 5005, 'AfterSaleApproveReverse', 'Integração de aprovação de produtos por ID da reversa', 3, 2),
(50, 5006, 'AfterSaleReciveProducts', 'Integração de recebimento de produtos por ID da reversa', 3, 2),
(50, 5007, 'AfterSaleReturnedInvoice', 'Integração de atualização da nota de devolucao por ID dos produtos', 3, 2),
(50, 5008, 'AfterSaleReverseInvoice', 'Integração de definição de dados de logística (processar/gerar codigo)', 3, 2),
(50, 5021, 'AfterSaleAdministrativeRecordsReverse', 'Integração de criação de registros administrativos por ID da reversa', 3, 2),
```

Os 5005–5008 vieram do seu cadastro original; o 5021 eu adicionei quando implementei os registros
administrativos. Os quatro de reembolso (5017–5020, linhas 68–71) também já existem, para quando
essas rotas forem implementadas. Ou seja: **não é preciso inserir linhas novas** para as rotas de
hoje.

Sugestões de ajuste (opcionais):

1. **Separar as ações num bloco próprio**, para ficar claro o que é consulta e o que é escrita,
   como fizemos com `Reads`/`Writes`. Substituir as linhas 56–60 por um bloco logo após a linha 55:

   ```sql
   /*Ações (POST/PUT) - executadas sob demanda pelo AfterSaleController*/
   (50, 5005, 'AfterSaleApproveReverse', 'Aprovação de produtos por ID da reversa (POST approve)', 0, 2),
   (50, 5006, 'AfterSaleReciveProducts', 'Recebimento de produtos por ID da reversa (PUT receive-products)', 0, 2),
   (50, 5007, 'AfterSaleReturnedInvoice', 'Atualização da nota de devolução por ID dos produtos (PUT products/returned-invoice)', 0, 2),
   (50, 5008, 'AfterSaleReverseInvoice', 'Envio dos dados da nota para a transportadora e geração do código (POST invoice)', 0, 2),
   (50, 5021, 'AfterSaleAdministrativeRecordsReverse', 'Criação de registro administrativo por ID da reversa (POST administrative-records)', 0, 2),
   ```

2. **`IntervalTimeInMinutes = 0`** para as ações: hoje elas estão com `3`, como se rodassem a
   cada 3 minutos, mas são chamadas sob demanda. Nenhum código lê essa coluna hoje (procurei no
   repositório), então mudar para `0` só deixa o cadastro correto. Se algum monitoramento futuro
   usar o intervalo para alertar "job não rodou", o `0` indica que não é recorrente.

3. **Novo `IdType` para ações?** Os tipos atuais são Procedure, Integração, RPA e as duas
   aplicações. As ações são chamadas da integração, então `2` (Integração) faz sentido. Não vejo
   necessidade de um tipo novo.

---

## 7. Resumo do que seria alterado

| Camada | Arquivo | Mudança |
|---|---|---|
| SQL (Shared) | `Script008_Create_Requests_Table.sql` (novo) | Tabela `auditing.Requests`, FK e índice |
| SQL (AfterSale) | `Script007_Insert_AfterSale_Values_Into_Jobs_Table.sql` | Opcional: bloco de ações e `IntervalTimeInMinutes = 0` |
| Domain.Shared | `Entities/Auditing/Request.cs` (novo), `Log.cs` | Entidade e lista `Requests` no log |
| Application.Shared | `ILoggerService`, `LoggerService`, `ICurrentUserService` (novo) | `AddRequest` e abstração do usuário |
| Infrastructure.Shared | `LoggerRepository` | Bulk insert em `auditing.Requests` |
| Host Hangfire | `CurrentUserService` (novo), filtro do cabeçalho, registro no DI | Leitura do `X-User-Name` |
| Application.AfterSale | `AfterSaleReversesService.SendReverseAction` | `AddRequest` no lugar do `AddRecord`, captura de exceção |

## 8. Decisões para você

1. Tabela em `auditing` (opção A) ou em `general` só do AfterSale (opção B)?
2. Usuário pelo cabeçalho `X-User-Name` agora, com JWT depois?
3. Manter `DocCompany` e `FieldKeyValue` como colunas próprias?
4. Na rota `InvoiceXml`, gravar só os metadados do arquivo ou o XML inteiro?
5. Aplicar os ajustes opcionais no `Script007` (bloco próprio e `IntervalTimeInMinutes = 0`)?
