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

Uma pasta com scripts que **não** devem rodar no deploy (ex.: `Homologacao/` do Totvs, com as credenciais de
homologação) fica de fora pelo `Filter`:

```csharp
    .WithScriptsFromFileSystem(".", new FileSystemScriptOptions
    {
        IncludeSubDirectories = true,
        Filter = path => !Path.GetRelativePath(".", path).StartsWith("Homologacao", StringComparison.OrdinalIgnoreCase)
    })
```

Quando o módulo tem uma só API, as pastas das services ficam direto em `SQLServer/` (ex.: `SQLServer/People/`). Os
scripts de jobs de escrita e de leitura sob demanda que estavam soltos vão para o script de jobs da service que usa
cada job (`EnumJob` referenciado na service).

### 3.5 Ao mover ou renomear scripts

* Um script que está no `.gitignore` (ex.: o `Script003` com as chaves) precisa ter o **caminho atualizado** no
  `.gitignore`, senão deixa de ser ignorado e pode ir para um commit.
* Atualizar as referências aos nomes dos scripts: comentários do `EnumJob`, README do módulo e documentos vigentes.
  Documentos históricos (propostas, histórico de versões) ficam como estão.

> **Aviso:** Renomear, dividir ou renumerar scripts: em bancos que já rodaram as migrações, o DbUp vê os scripts como
> novos e executa de novo. Esses bancos precisam ser recriados do zero.
