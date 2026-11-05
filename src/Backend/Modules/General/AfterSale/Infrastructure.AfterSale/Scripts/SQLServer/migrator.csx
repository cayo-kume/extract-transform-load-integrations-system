// Importa o pacote Nuget na hora da execução (sem precisar de .csproj)
#r "nuget: dbup-sqlserver" 

using System;
using System.Linq;
using System.IO;
using DbUp;
using DbUp.Engine;
using DbUp.ScriptProviders;
using DbUp.Support;
using Microsoft.Data.SqlClient;

// Pega a string de conexão dos argumentos ou usa uma default para teste local
var connectionString = Args.Count > 0 ? Args[0] : "Server=localhost;Database=Company;User Id=sa;Password=root@1234;TrustServerCertificate=True;";

Console.WriteLine($"Conectando em: {connectionString}");

// 1. Garante que o banco existe
EnsureDatabase.For.SqlDatabase(connectionString);

// tabelas escolhidas na tela do tools/migrators/Invoke-Migrators.ps1: a variável ETL_MIGRATOR_TABLES traz os nomes das
// tabelas dos jobs escolhidos, sem o schema (valem para a untreated e a trusted), separados por ";". Sem ela, todas
var tables = Environment.GetEnvironmentVariable("ETL_MIGRATOR_TABLES")?
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
var scriptsRoot = Path.GetFullPath(".");

Console.WriteLine(tables is null ? "Tabelas: todas." : $"Tabelas: {tables.Count} dos jobs escolhidos (as que já existem ficam como estão).");

// a seleção vai para dbo.MigratorTables (Shared, Script010), que os scripts de tabelas consultam pela dbo.fn_Migrator_Creates
using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    using var command = new SqlCommand(@"
        if object_id(N'[dbo].[MigratorTables]') is null
            throw 50020, N'dbo.MigratorTables não existe: rode antes o migrator do Shared.', 1;
        delete from [dbo].[MigratorTables];
        insert into [dbo].[MigratorTables] ([Name]) select distinct value from string_split(@names, ';') where value <> N'';", connection);
    command.Parameters.AddWithValue("@names", tables is null ? "*" : string.Join(';', tables));
    command.ExecuteNonQuery();
}

// os scripts de tabelas e de procedures de sincronização dos services rodam em toda migração: cada tabela só é criada se não
// existe e está na seleção, e cada procedure só com a tabela criada. Os outros (Parameters, a raiz e o catálogo dos jobs de
// cada service) rodam uma vez só, como sempre, e sobem todos: o catálogo tem todos os jobs, com tabela ou não
bool RunsAlways(string path)
{
    var folder = Path.GetRelativePath(scriptsRoot, Path.GetDirectoryName(Path.GetFullPath(path))!).Replace('\\', '/');
    var name = Path.GetFileName(path);
    return folder != "." && !folder.Equals("Parameters", StringComparison.OrdinalIgnoreCase)
        && (name.EndsWith("_Tables.sql", StringComparison.OrdinalIgnoreCase) || name.EndsWith("_Sync_Procedures.sql", StringComparison.OrdinalIgnoreCase));
}

// 2. Configura o DbUp
// os scripts ficam em subpastas (Parameters, com os gerais, e uma por service); o DbUp nomeia cada um pelo caminho
// (ex.: Me.Script007_...sql), então a ordem de execução é a do número do arquivo, não a da pasta
var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".", new FileSystemScriptOptions { IncludeSubDirectories = true, Filter = path => !RunsAlways(path) })
    .WithScriptsFromFileSystem(".", new FileSystemScriptOptions { IncludeSubDirectories = true, Filter = RunsAlways }, new SqlScriptOptions { ScriptType = ScriptType.RunAlways })
    .WithScriptSorter(scripts => scripts.OrderBy(s => s.Name.Split('.')[^2], StringComparer.Ordinal))
    .LogToConsole()
    .Build();

// 3. Executa
var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Environment.Exit(-1); 
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Sucesso!");