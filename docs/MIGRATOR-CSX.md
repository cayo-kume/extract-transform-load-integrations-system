# Script `migrator.csx` — Migrações de Banco de Dados (DbUp)

Cada módulo tem um `migrator.csx` em `Infrastructure.<Módulo>/Scripts/SQLServer`: um script C# de linha de comando,
executado pelo **`dotnet-script`**, que usa a biblioteca **DbUp** para aplicar os scripts `.sql` do módulo no SQL Server. O
padrão de organização dos scripts está em [PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md](PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md)
(seção 3).

## Onde ficam

| Módulo | Pasta (a partir de `src/Backend/`) | Estrutura dos scripts |
|---|---|---|
| Shared | `Shared/Infrastructure.Shared/Scripts/SQLServer` | Direto na pasta (5 scripts) |
| Totvs | `Modules/Totvs/Infrastructure.Totvs/Scripts/SQLServer` | `Parameters/` e uma pasta por service |
| LinxMicrovix | `Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/Infrastructure.LinxMicrovix.Outbound/Scripts/SQLServer` | `Parameters/`, `Erp/<Service>` e `B2C/<Service>` |
| LinxCommerce | `Modules/LinxCommerce/Infrastructure.LinxCommerce/Scripts/SQLServer` | `Parameters/` e uma pasta por service |
| Movidesk | `Modules/General/Movidesk/Infrastructure.Movidesk/Scripts/SQLServer` | `Parameters/` e uma pasta por service |
| AfterSale | `Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer` | `Parameters/` e uma pasta por service |
| DataWarehouse | `Modules/DataWarehouse/Infrastructure.DataWarehouse/Scripts/SQLServer` | Uma pasta por camada (`Parameters/`, `Landing/`, `History/`...) |

Os módulos fora do host (Dootax, Mobsim, Oto, Pagarme, FlashCourier, Jadlog, Stone e TotalExpress) também têm um
`migrator.csx` em `Scripts/SQLServer`, com os scripts direto na pasta, como no sistema antigo.

As pastas `Scripts/MySQL` e `Scripts/PostgreSQL` de cada módulo têm só um `migrator.csx`, sem nenhum script `.sql`: o
banco do sistema é o SQL Server. Veja [Migrators de MySQL e PostgreSQL](#migrators-de-mysql-e-postgresql).

## Ordem entre os módulos

Todos os módulos usam o **mesmo banco** e a mesma tabela de controle do DbUp. Rode os migrators nesta ordem:

1. **Shared**: schemas `auditing` e `untreated`, as tabelas e procedures de auditoria e a `dbo.sp_Create_Merge_Procedure`,
   que os scripts de procedures dos módulos usam para gerar os merges.
2. **Módulos** (Totvs, LinxMicrovix, LinxCommerce, Movidesk, AfterSale), em qualquer ordem entre eles.
3. **DataWarehouse**, por último: ele lê as tabelas dos módulos.

## O que o script faz

### Referência ao pacote NuGet em tempo de execução

```csharp
#r "nuget: dbup-sqlserver"
```

Baixa e carrega o DbUp na hora da execução, sem `.csproj`. É o que permite rodar o arquivo com `dotnet script migrator.csx`.

### String de conexão

```csharp
var connectionString = Args.Count > 0 ? Args[0] : "Server=localhost;Database=Company;...";
```

Usa a connection string do **primeiro argumento**. Nos módulos de integração e no Shared, sem argumento ele usa um padrão
de teste local (`localhost`, banco `Company`). O **DataWarehouse** é diferente: a connection string é obrigatória e não tem
padrão no código; sem ela, ou com ela incompleta, o script para e diz o que falta (servidor, banco, usuário e senha ou
autenticação integrada), e imprime só o servidor e o banco, nunca o usuário ou a senha.

### Garante que o banco existe

```csharp
EnsureDatabase.For.SqlDatabase(connectionString);
```

Cria o banco se ele ainda não existir.

> **Importante:** o DbUp cria o banco **com o nome informado na própria connection string** (`Database=` ou
> `Initial Catalog=`). Se você passar `Database=NEWBLOOMERS`, é o banco `NEWBLOOMERS` que será criado (se ainda não existir) e
> é nele que **todos** os scripts serão aplicados.
>
> Por isso **não é necessário** um script `CREATE DATABASE ...`. Um `CREATE DATABASE` seguido de `USE` no mesmo batch, inclusive,
> causa o erro `911 - Database does not exist`, porque o SQL Server compila o batch inteiro antes de executá-lo e o `USE` não
> enxerga um banco criado no mesmo batch.

### Configura o DbUp

```csharp
var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".", new FileSystemScriptOptions { IncludeSubDirectories = true })
    .WithScriptSorter(scripts => scripts.OrderBy(s => s.Name.Split('.')[^2], StringComparer.Ordinal))
    .LogToConsole()
    .Build();
```

* **`WithScriptsFromFileSystem(".", ... IncludeSubDirectories = true)`**: lê todos os `.sql` da pasta atual **e das
  subpastas**. O DbUp nomeia cada script pelo caminho, com pontos no lugar das barras (ex.:
  `People.Script005_Create_Totvs_People_Tables.sql`).
* **`WithScriptSorter(...)`**: ordena pelo **nome do arquivo**, sem a pasta. Sem ele, a ordem seria alfabética pelo nome
  completo, e `B2C.*` rodaria antes de `Parameters.Script001`. Com ele, a ordem de execução é a do número do arquivo, que é
  única e sequencial no módulo.
* O **DataWarehouse** acrescenta `.WithExecutionTimeout(TimeSpan.FromMinutes(10))`: logo depois da criação das tabelas
  dos outros módulos, a primeira geração de procedure pode passar dos 30 segundos padrão.
* O **Shared** não tem subpastas e usa só `.WithScriptsFromFileSystem(".")`.

O DbUp cria a tabela de controle `dbo.SchemaVersions` e executa cada script **uma única vez**, gravando o nome (com a
pasta). Rodar de novo só aplica os scripts que ainda não estão nela.

### Executa e reporta

```csharp
var result = upgrader.PerformUpgrade();
```

Roda os scripts pendentes. Se um falhar, imprime o erro em vermelho e sai com código `-1` (os seguintes não rodam); se
der certo, imprime `Sucesso!` em verde.

## Pré-requisito: instalar o `dotnet-script`

O `.csx` **não** roda com o `dotnet` sozinho: ele depende da ferramenta **`dotnet-script`**, distribuída como _.NET global
tool_. Instale uma vez por máquina:

```powershell
dotnet tool install -g dotnet-script
```

Para atualizar e para ver a versão instalada:

```powershell
dotnet tool update -g dotnet-script
dotnet script --version
```

> As ferramentas globais ficam em `%USERPROFILE%\.dotnet\tools`. Se o comando `dotnet script` não for reconhecido depois da
> instalação, confira se esse caminho está no `PATH` (ou abra um novo terminal).

## Como rodar

O script lê a **pasta atual** (`.`), então precisa ser executado **de dentro da pasta `Scripts/SQLServer` do módulo**, a
que contém o `migrator.csx`.

No Windows, o caminho dos módulos passa do limite de **260 caracteres (MAX_PATH)** e o `dotnet-script` falha ao carregar a
DLL nativa do SQL Client (ver [Solução de problemas](#solução-de-problemas)). A forma padrão é mapear a pasta para uma
**unidade virtual curta** com `subst`:

```powershell
# 1. Mapeia a pasta do módulo para a unidade virtual X: (ajuste o caminho conforme o módulo)
subst X: "C:\Users\<usuário>\source\repos\extract-transform-load-integrations-system\src\Backend\Modules\Totvs\Infrastructure.Totvs\Scripts\SQLServer"

# 2. Entra na unidade virtual
X:

# 3. Executa o migrator passando a connection string
dotnet script migrator.csx -- "Server=localhost;Database=NEWBLOOMERS;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;"

# 4. Remove a unidade virtual ao terminar (opcional)
subst X: /D
```

Vantagens do `subst`:

- encurta o caminho de cache do `dotnet-script`, contornando o MAX_PATH sem copiar arquivos;
- aponta para os **arquivos reais** do repositório: qualquer edição nos `.sql` ou no `migrator.csx` vale na hora.

> O `subst` **não é persistente**: a unidade `X:` some ao reiniciar o Windows. Basta rodar o passo 1 de novo.

Se o caminho for curto o bastante (por exemplo, no `Shared`), dá para rodar direto, sem `subst`:

```powershell
cd <repositório>\src\Backend\Shared\Infrastructure.Shared\Scripts\SQLServer
dotnet script migrator.csx -- "Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True;"
```

### Antes de rodar

* **Credenciais:** confira o `Parameters/Script003` do módulo (empresas e credenciais da API). No LinxMicrovix e no
  LinxCommerce o `.sql` real fica fora do git: copie o `.sql.example` para o mesmo nome sem `.example` e preencha. Na Totvs, as
  credenciais de homologação estão comentadas no próprio `Script003`, para rodar à mão no banco de homologação ou local, no
  lugar das de produção.
* **Banco que já rodou as migrações:** os scripts são identificados pelo nome (com a pasta) na `SchemaVersions`. Renomear,
  dividir, renumerar ou mover scripts faz o DbUp vê-los como novos e executá-los de novo, e um `CREATE TABLE` repetido
  falha. Depois de uma reorganização como a dos scripts em pastas, o banco precisa ser **recriado do zero**.

## Solução de problemas

| Sintoma | Causa | O que fazer |
|---|---|---|
| Erro `0x800700CE` ao carregar `Microsoft.Data.SqlClient.SNI.dll` | O caminho do repositório mais o cache do `dotnet-script` passa de 260 caracteres | Rodar a partir de uma unidade `subst` (seção [Como rodar](#como-rodar)) |
| `dotnet script` não é reconhecido | A ferramenta não está instalada ou `%USERPROFILE%\.dotnet\tools` não está no `PATH` | `dotnet tool install -g dotnet-script` e abrir um novo terminal |
| `911 - Database does not exist` | Um script com `CREATE DATABASE` seguido de `USE` | Remover o script: o `EnsureDatabase` já cria o banco da connection string |
| `Could not find stored procedure 'dbo.sp_Create_Merge_Procedure'` | O migrator do módulo rodou antes do Shared | Rodar o migrator do Shared e depois o do módulo de novo |
| `There is already an object named ...` | O banco já tinha rodado os scripts com outro nome (antes de uma renomeação ou reorganização) | Recriar o banco do zero |
| Script ignorado ou fora de ordem | O script foi colocado fora de `Scripts/SQLServer` ou o número repete outro do módulo | A numeração é única e sequencial no módulo; o arquivo precisa estar dentro da pasta do migrator ou de uma subpasta dela |
| DataWarehouse para com "A connection string é obrigatória" ou "está incompleta" | O migrator do DW não tem padrão e confere a connection string | Passar a connection string completa (servidor, banco e autenticação) |

## Migrators de MySQL e PostgreSQL

As pastas `Scripts/MySQL` e `Scripts/PostgreSQL` existem em todos os módulos, mas **não têm scripts** e não são usadas: o
sistema roda só em SQL Server. Os migrators delas estão prontos para o banco de cada pasta:

| Pasta | Pacote | Métodos | Connection string padrão (teste local) |
|---|---|---|---|
| `MySQL/` | `dbup-mysql` | `EnsureDatabase.For.MySqlDatabase` e `.MySqlDatabase(...)` | `Server=localhost;Port=3306;Database=Company;User Id=root;Password=...;` |
| `PostgreSQL/` | `dbup-postgresql` | `EnsureDatabase.For.PostgresqlDatabase` e `.PostgresqlDatabase(...)` | `User ID=postgres;Password=...;Host=localhost;Port=5432;Database=Company;Pooling=true;` |

Eles leem só a pasta atual, sem subpastas. Se o sistema for para MySQL ou PostgreSQL, será preciso escrever os scripts para o
banco de destino e acrescentar `IncludeSubDirectories` e o sorter, como no migrator de SQL Server.
