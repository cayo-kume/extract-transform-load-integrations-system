# Padrão de Código e Scripts para Integrações

Este documento reúne as **preferências de código e de organização dos scripts de banco** definidas na refatoração do
módulo LinxMicrovix Outbound. A implementação de referência é
[`LinxMicrovix - Outbound Web Service`](../src/Backend/Modules/LinxMicrovix/LinxMicrovix%20-%20Outbound%20Web%20Service).

Todo novo projeto (e toda refatoração de um projeto existente) deve seguir este modelo.

---

## 1. Refatoração de formatação e codificação

Uma refatoração de estilo **não muda o comportamento nem a execução** da solução. Fica fora dela:

* **Regra de negócio:** condições, cálculos, filtros, mapeamentos, valores padrão e textos gravados ou enviados.
* **Dependências:** pacotes, versões, `TargetFramework`, referências entre projetos e registros de DI.
* **Execução:** ordem das chamadas, `async`/`await`, tratamento de exceções, logs, configuração, cache, SQL,
  procedures e views.
* **Contratos:** nomes que aparecem em serialização, colunas, rotas, chaves de configuração ou reflection.

Quando um pedido de refatoração puder alterar algum desses pontos, **avisar antes**, explicar o efeito e esperar a
decisão. O mesmo vale quando o efeito for duvidoso (ex.: trocar `foreach` por LINQ muda o momento da avaliação).

Antes de executar uma refatoração ampla, descrever o que foi entendido e o que foi encontrado no código, e só alterar
depois da confirmação.

---

## 2. Métodos privados de gravação (`Save*`)

Um método privado de gravação só existe quando é **reutilizado por mais de um método**. O caso típico é o do
AfterSale e do Movidesk: a busca por intervalo de datas e a busca de um único registro por id gravam pelo mesmo
`Save`.

Quando só **um** método chama a gravação, o código fica **dentro desse método**, sem um método privado separado.

```csharp
// valida, monta as entidades (com o mesmo horário de lote) e grava. Chave: cnpj_emp + id; fica a linha de maior timestamp
var batchTime = _linxMicrovixService.GetBatchTime();

var _listSomenteNovos = new List<Entities.LinxFidelidade>();
...

if (_listSomenteNovos.Count == 0)
    continue;

await _repository.BulkInsertIntoLinxFidelidade(_listSomenteNovos, _logger.GetExecutionGuid());
...

totalInseridos += _listSomenteNovos.Count;
```

Ao mover uma gravação para dentro do método pai:

* o `/// <summary>` do método vira um comentário `//` no começo do trecho movido;
* um valor que era argumento é calculado **uma vez, no mesmo ponto** da antiga chamada (ex.: `batchTime`, que lê o
  relógio e não pode ser chamado por registro);
* um `return` antecipado **não** pode ir para o pai como está, porque encerraria o método inteiro: vira `continue`
  quando a gravação é o último comando do laço, ou um `if` em volta da gravação nos outros casos;
* o valor de retorno passa a ser somado direto no acumulador do pai (ex.: `totalInseridos`).

### 2.1 Demais métodos privados

A mesma regra vale para qualquer método privado das services: se só **um** método pai o usa, ele vai para dentro
desse pai. Métodos usados por mais de um pai continuam como métodos da classe.

* **Uma chamada e um único `return`, no fim:** o código vai direto para o ponto da chamada.
* **Vários `return` no meio, várias chamadas no mesmo pai, recursão ou uso como referência de método**
  (ex.: `.GroupBy(DuplicateKey)`): vira **função local** no fim do método pai, `static` quando o original era
  `static`. Assim nada é duplicado e o fluxo continua o mesmo.
* Encadeados ficam aninhados: um método usado só por outro que também vai para dentro do pai entra primeiro no seu
  pai direto (ex.: `PricesBody` e `ReadPrices` dentro de `SyncPrices`, dentro de `PostSearchPrices`).
* Classes utilitárias que só estão na pasta `Services` (ex.: `TotvsMasking`) ficam de fora.

### 2.2 Ordem dos membros na service

No mesmo arquivo e na mesma classe: campos, constantes e construtor no topo, depois os métodos **públicos** e, no
fim da classe, os métodos **privados** (cada um com o seu comentário), separados dos públicos por esta linha:

```csharp
        // ------------------------------------------------------------------------------------------------------
```

### 2.3 Pasta `Validations` espelha os DTOs

Os validadores ficam em `Application.<Módulo>/Validations` na **mesma estrutura de pastas dos DTOs** que validam
(`Domain.<Módulo>/Dtos`): os de leitura em `Validations/Reads`, os de escrita em `Validations/Writes`, e dentro de
cada um as mesmas subpastas dos DTOs. O namespace acompanha a pasta. Um helper comum a todos os validadores (ex.:
`MicrovixValidatorsExtension`) fica na raiz de `Validations`.

```
Domain.Totvs/Dtos/                      Application.Totvs/Validations/
├── Reads/                              ├── Reads/
│   ├── Products/Catalogs/...           │   ├── Products/Catalogs/...    (Application.Totvs.Validations.Reads.Products.Catalogs)
│   └── ...                             │   └── ...
└── Writes/                             └── Writes/
    └── ...                                 └── ...                      (Application.Totvs.Validations.Writes.*)
```

Os validadores são registrados pelo assembly (`AddValidatorsFromAssemblyContaining<...>()`) e as services recebem
`IValidator<Dto>` pelo tipo do DTO: mover um validador de pasta muda só o namespace e os `using` que o citam.

### 2.4 Um command handler por service

As queries SQL ficam num **command handler por service** (`<Módulo><Service>CommandHandler`, com a interface
`I<Módulo><Service>CommandHandler`), usado só pelo repositório daquele service. As queries comuns do módulo
(empresas, lojas, trava de repetição etc.) ficam no `<Módulo>CommandHandler`, usado pelo repositório comum.

A divisão segue o repositório que chama cada query: a query vai para o handler do service dono daquele repositório.

```
Application.Totvs/
├── Abstractions/Handlers/Commands/
│   ├── ITotvsCommandHandler.cs                 (comuns: TotvsRepository)
│   ├── ITotvsPeopleCommandHandler.cs           (TotvsPeopleRepository)
│   ├── ITotvsProductsCommandHandler.cs         (TotvsProductsRepository)
│   └── ...                                     (um por service com repositório)
└── Handlers/Commands/
    ├── TotvsCommandHandler.cs
    ├── TotvsPeopleCommandHandler.cs
    ├── TotvsProductsCommandHandler.cs
    └── ...
```

```csharp
            services.AddScoped<ITotvsPeopleService, TotvsPeopleService>();
            services.AddScoped<ITotvsPeopleRepository, TotvsPeopleRepository>();
            services.AddScoped<ITotvsPeopleCommandHandler, TotvsPeopleCommandHandler>();
```

* Cada handler é registrado na DI (`AddScoped`) ao lado do service e do repositório dele, e o repositório recebe só o
  seu handler (`_totvsPeopleCommandHandler`).
* Um service sem repositório (ex.: `TotvsBonusService`, só escritas) não tem handler.
* Um repositório que não usa nenhuma query não recebe handler no construtor (ex.: `AfterSaleMeRepository`).
* Um método privado que servia a queries de services diferentes (ex.: `CreateGetOrdersByKeysQuery`, de pedidos de
  compra e de venda) vai para dentro de cada método que o usa, com os parâmetros fixos (ex.: o nome da tabela), pela
  regra da seção 2.1.

---

## 3. Scripts de banco (DbUp, SQL Server)

### 3.1 Organização

Os scripts ficam em `Infrastructure.<Módulo>/Scripts/SQLServer`, em subpastas:

```
SQLServer/
├── migrator.csx
├── Parameters/              (scripts gerais do módulo)
│   ├── Script001_Create_<Módulo>_Schema_And_Parameters_Table.sql
│   ├── Script002_Insert_<Módulo>_Values_Into_Apps_Table.sql
│   ├── Script003_Insert_Values_Into_<Módulo>_Parameters_Table.sql(.example)
│   └── Script004_Create_<Módulo>_Requests_Table.sql
├── <Api>/<Service>/         (uma pasta por service; ex.: Erp/Stores, B2C/Customers)
│   ├── ScriptNNN_Create_<Módulo>_<Service>_Tables.sql
│   ├── ScriptNNN_Create_<Módulo>_<Service>_Sync_Procedures.sql
│   └── ScriptNNN_Insert_<Módulo>_<Service>_Jobs_Into_Jobs_Table.sql
└── ...
```

* **Uma pasta por service**, com o nome da service (sem o prefixo do módulo e sem o sufixo `Service`), dentro de uma
  pasta por API quando o módulo tem mais de uma (ex.: `Erp/` e `B2C/` no LinxMicrovix).
* **Um trio por service:** tabelas, procedures e jobs, nessa ordem, com **todas** as tabelas que os métodos da service
  gravam.
* O script de jobs da service traz **todos os jobs dela**: os recorrentes, as procedures e os **sob demanda**
  (em um bloco `/*Integrações sob demanda ...*/` no mesmo `INSERT`). Não existe um script de jobs sob demanda à parte.
* O `INSERT` de jobs lista `(IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)`
  (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.1). `RunningDailyAt` é `NULL`, menos nos jobs de integração
  diários (`IntervalTimeInMinutes = 1440`), que levam a hora (`'23:00'`); `Queue` é `'default'`. `IsActive` fica de fora
  e nasce 0: o job só entra na agenda do Hangfire depois de um `UPDATE`.
* Um job recorrente novo também entra no mapa do host (`src/Backend/Hosts/WebApi/Scheduling/RecurringJobs/<Módulo>RecurringJobs.cs`),
  com o método sem parâmetros do service que grava o job no log; sem o mapa, o `SchedulerRefresh` avisa e o deixa fora da
  agenda.
* O método do service chamado por um job recorrente leva `[JobVersion("MAJOR.MINOR.PATCH")]` (`Application.Shared.Attributes`)
  no método da classe, começando em `1.0.0`; quem muda o código do método muda a versão (PATCH correção, MINOR mudança
  compatível, MAJOR mudança no que o job grava). A versão vai no nome do job no agendador
  (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.10).
* Os scripts gerais do módulo (schema, parâmetros, app, tabela de requests) ficam em `Parameters/`.

### 3.2 Numeração

* A numeração é **única e sequencial no módulo inteiro**, atravessando as pastas: ela define a ordem de execução.
* Ordem: `Parameters/` primeiro, depois as services de cada API (ex.: todo o ERP antes do B2C).
* Os nomes seguem `ScriptNNN_<Verbo>_<Módulo>_<Assunto>.sql`.

### 3.3 Tabela criada errada se corrige no script de criação

Não se cria um script de `ALTER` para corrigir uma tabela, procedure ou view que foi criada errada: a correção vai no
**script de criação original**, e o script de alteração é excluído.

### 3.4 `migrator.csx`

Com subpastas, o `migrator.csx` precisa ler as subpastas e ordenar pelo **nome do arquivo**. O DbUp nomeia cada script
pelo caminho (ex.: `Erp.Stores.Script005_...sql`), e a ordem alfabética padrão poria `B2C.*` antes de
`Parameters.Script001`:

```csharp
using System.Linq;
using DbUp.ScriptProviders;

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".", new FileSystemScriptOptions { IncludeSubDirectories = true })
    .WithScriptSorter(scripts => scripts.OrderBy(s => s.Name.Split('.')[^2], StringComparer.Ordinal))
    .LogToConsole()
    .Build();
```

Credenciais de outro ambiente (ex.: homologação) não ficam numa pasta à parte: ficam **comentadas** no próprio
script de parâmetros (`Parameters/Script003`), para rodar à mão no lugar das de produção.

Quando o módulo tem uma só API, as pastas das services ficam direto em `SQLServer/` (ex.: `SQLServer/People/`). Os
scripts de jobs de escrita e de leitura sob demanda que estavam soltos vão para o script de jobs da service que usa
cada job (`EnumJob` referenciado na service).

Um módulo que não é integração por service (ex.: o DataWarehouse, com uma service só) organiza as pastas **por
camada**, na ordem de execução (`Parameters/`, `Landing/`, `History/`, `PersonalData/`, `Dimensions/`,
`Intermediate/`, `Facts/`, `Datamart/`, `Outbound/`, `Checks/`), com um só script de jobs em `Parameters/`. A ordem
segue as dependências: view e função inline precisam que os objetos lidos já existam (por isso `Dimensions/` vem antes
de `Intermediate/`, onde `fn_StockPosition` lê `DimStore` e `DimProduct`); procedure pode citar tabela criada depois.
Um objeto refeito numa fase seguinte (`create or alter` de novo) fica só no script de criação, já na versão final.

O `Shared` (auditoria, `sp_Create_Merge_Procedure` e manutenção) não tem services nem parâmetros: os scripts ficam direto em
`SQLServer/`, sem subpastas. Script que não executa nada (inteiramente comentado) sai.

Um método `[GeneratedRegex] private static partial Regex` fica na classe, junto dos campos: o gerador de código não aceita
função local, então é exceção ao §2.1 e ao §2.2.

### 3.5 Ao mover ou renomear scripts

* Um script que está no `.gitignore` (ex.: o `Script003` com as chaves) precisa ter o **caminho atualizado** no
  `.gitignore`, senão deixa de ser ignorado e pode ir para um commit.
* Atualizar as referências aos nomes dos scripts: comentários do `EnumJob`, README do módulo e documentos vigentes.
  Documentos históricos (propostas, histórico de versões) ficam como estão.

> **Aviso:** Renomear, dividir ou renumerar scripts: em bancos que já rodaram as migrações, o DbUp vê os scripts como
> novos e executa de novo. Esses bancos precisam ser recriados do zero.

---

## 4. Documentação

* Cada módulo tem uma pasta `docs/` na raiz (ao lado do `README.md`), com as propostas, os planos, as implementações e as
  modelagens dele (ex.: `src/Backend/Modules/Totvs/docs`). Os do Shared ficam em `src/Backend/Shared/docs`, e o guia do
  data warehouse em `src/Backend/Modules/DataWarehouse/docs`.
* As respostas de exemplo da API, usadas para montar DTOs e entidades (`Responsed Jsons` ou `Responsed XMLs`, com os
  dados trocados por dados fictícios), também ficam dentro do `docs/` do módulo.
* A pasta `docs/` da raiz do repositório guarda só o que vale para todos os módulos: os padrões, o `MIGRATOR-CSX.md` e a
  revisão contra o padrão.
* Ao mover um documento, os links relativos dele e para ele são refeitos a partir do novo lugar.
