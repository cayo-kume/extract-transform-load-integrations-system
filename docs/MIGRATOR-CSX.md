# Script `migrator.csx` — Migrações de Banco de Dados (DbUp)

Localização: `src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/migrator.csx`

Este é um script C# de linha de comando (`.csx`, executado via `dotnet-script`) que usa a biblioteca **DbUp** para aplicar migrações de banco de dados a partir dos arquivos `.sql` da pasta.

## O que cada parte faz

### Referência ao pacote NuGet em tempo de execução

```csharp
#r "nuget: dbup-sqlserver"
```

Baixa e carrega o DbUp na hora da execução, sem precisar de um `.csproj`. É o que permite rodar o arquivo direto com `dotnet script migrator.csx`.

### String de conexão

```csharp
var connectionString = Args.Count > 0 ? Args[0] : "Server=localhost;...";
```

Pega a connection string do **primeiro argumento** passado na linha de comando. Se nenhum for passado, usa uma default apontando para `localhost` / banco `Company` (somente para teste local).

### Garante que o banco existe

```csharp
EnsureDatabase.For.MySqlDatabase(connectionString);
```

Cria o banco de dados caso ele ainda não exista.

> **Importante:** o DbUp cria o banco **com o nome informado na própria connection string** (parâmetro `Database=` / `Initial Catalog=`). Ou seja, se você passar `Database=NEWBLOOMERS`, é o banco `NEWBLOOMERS` que será criado (se ainda não existir) e é nele que **todos** os scripts `.sql` serão aplicados.
>
> Por isso **não é necessário** ter um script SQL separado do tipo `CREATE DATABASE ...` — o `EnsureDatabase` já cuida disso. Um script `CREATE DATABASE` seguido de `USE` no mesmo batch, inclusive, causa o erro `911 - Database does not exist`, porque o SQL Server compila o batch inteiro antes de executá-lo e o `USE` não enxerga um banco criado no mesmo batch. A forma correta é apontar a connection string para o banco desejado e deixar o `EnsureDatabase` criá-lo.

### Configura o DbUp

```csharp
var upgrader = DeployChanges.To
    .MySqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".")   // lê todos os .sql da pasta atual
    .LogToConsole()
    .Build();
```

Monta o "upgrader": ele varre a **pasta atual** (`.`) procurando arquivos `.sql` — que aqui são os `Script001_...` até `Script005_...`. O DbUp cria uma tabela de controle (`SchemaVersions`) no banco e executa cada script **apenas uma vez**, em ordem alfabética, registrando os já aplicados. Rodar de novo não reaplica o que já passou.

### Executa e reporta

```csharp
var result = upgrader.PerformUpgrade();
```

Roda os scripts pendentes. Se falhar, imprime o erro em vermelho e sai com código `-1`. Se der certo, imprime `"Sucesso!"` em verde.

## Pré-requisito: instalar o `dotnet-script`

Este arquivo `.csx` **não** roda com o `dotnet` sozinho — ele depende da ferramenta **`dotnet-script`**, que precisa estar instalada. Ela é distribuída como uma _.NET global tool_.

Instale (uma única vez por máquina):

```powershell
dotnet tool install -g dotnet-script
```

Para atualizar uma instalação existente:

```powershell
dotnet tool update -g dotnet-script
```

Para verificar se já está instalado / qual a versão:

```powershell
dotnet script --version
```

> **Observação:** ferramentas globais ficam em `%USERPROFILE%\.dotnet\tools`. Se o comando `dotnet script` não for reconhecido após a instalação, garanta que esse caminho está na variável de ambiente `PATH` (ou abra um novo terminal).

## Como rodar

Como o script usa `WithScriptsFromFileSystem(".")`, ele precisa ser executado **de dentro da pasta que contém o `migrator.csx` e os `.sql`**.

No Windows, o caminho real do repositório é muito profundo e estoura o limite de **260 caracteres (MAX_PATH)** do Windows — o `dotnet-script` falha ao carregar a DLL nativa `Microsoft.Data.SqlClient.SNI.dll` com o erro `0x800700CE` (ver [Solução de Problemas](#solução-de-problemas)). Por isso, a **forma padrão de executar** é mapeando a pasta para uma **unidade virtual curta** com `subst`:

```powershell
# 1. Mapeia a pasta profunda para a unidade virtual X: (ajuste o caminho conforme o módulo)
subst X: "C:\Users\Cayo\source\repos\extract-transform-load-integrations-system\src\Backend\Modules\General\AfterSale\Infrastructure.AfterSale\Scripts\SQLServer"

# 2. Entra na unidade virtual
X:

# 3. Executa o migrator passando a connection string
dotnet script migrator.csx -- "Server=localhost;Database=NEWBLOOMERS;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;"

# 4. (Opcional) Remove a unidade virtual ao terminar
subst X: /D
```

Vantagens do `subst`:

- Encurta o caminho de cache do `dotnet-script`, contornando o MAX_PATH sem precisar copiar arquivos.
- Aponta para os **arquivos reais** do repositório — qualquer edição nos `.sql`/`migrator.csx` vale na hora, sem cópia desatualizada.

> **Observação:** o `subst` **não é persistente** — a unidade `X:` some ao reiniciar o Windows. Basta rodar o passo 1 de novo quando precisar.

> Se o caminho do repositório for curto o suficiente (por exemplo, no módulo `Shared`), dá para rodar direto sem `subst`:
> ```powershell
> cd caminho\ate\a\pasta\SQLServer
> dotnet script migrator.csx -- "Server=...;Database=...;User Id=...;Password=...;TrustServerCertificate=True;"
> ```

## Scripts SQL aplicados

Os scripts ficam em subpastas (`Parameters/` e uma por service; no DW, uma por camada) e o `migrator.csx` de cada módulo os executa
na ordem do número do arquivo (`IncludeSubDirectories` e o sorter pelo nome do arquivo). A lista de cada módulo está no README dele;
o padrão está em `docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md` (seção 3).

## ⚠️ Inconsistência importante

O script está na pasta **`SQLServer`** e importa o pacote **`dbup-sqlserver`**, mas o código chama métodos de **MySQL**:

- `EnsureDatabase.For.MySqlDatabase(...)`
- `.MySqlDatabase(...)`

Isso parece um **erro de copy-paste**. Para SQL Server o correto seria:

```csharp
EnsureDatabase.For.SqlDatabase(connectionString);

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".")
    .LogToConsole()
    .Build();
```

Do jeito que está, ou o `dbup-sqlserver` não expõe `MySqlDatabase` (falha de compilação), ou depende de outro pacote MySQL não referenciado. Vale corrigir para os métodos `SqlDatabase`.
