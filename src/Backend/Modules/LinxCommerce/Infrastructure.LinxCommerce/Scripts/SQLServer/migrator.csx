// Importa o pacote Nuget na hora da execução (sem precisar de .csproj)
#r "nuget: dbup-sqlserver" 

using System;
using System.Linq;
using System.IO;
using DbUp;
using DbUp.ScriptProviders;

// Pega a string de conexão dos argumentos ou usa uma default para teste local
var connectionString = Args.Count > 0 ? Args[0] : "Server=localhost;Database=Company;User Id=sa;Password=root@1234;TrustServerCertificate=True;";

Console.WriteLine($"Conectando em: {connectionString}");

// 1. Garante que o banco existe
EnsureDatabase.For.SqlDatabase(connectionString);

// services escolhidos na tela do tools/migrators/Invoke-Migrators.ps1: a variável ETL_MIGRATOR_SERVICES traz as pastas
// (ex.: People;Erp/Customers), separadas por ";". Sem ela, sobem todas; Parameters e os scripts da raiz sobem sempre
var services = Environment.GetEnvironmentVariable("ETL_MIGRATOR_SERVICES")?
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(x => x.Replace('\\', '/').Trim('/'))
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
var scriptsRoot = Path.GetFullPath(".");

bool IsSelected(string path)
{
    if (services is null) return true;

    var folder = Path.GetRelativePath(scriptsRoot, Path.GetDirectoryName(Path.GetFullPath(path))!).Replace('\\', '/');
    return folder == "." || folder.Equals("Parameters", StringComparison.OrdinalIgnoreCase) || services.Contains(folder);
}

if (services is not null)
    Console.WriteLine($"Services: {(services.Count > 0 ? string.Join(", ", services.Order()) : "nenhum")} (Parameters sempre).");

// 2. Configura o DbUp
// os scripts ficam em subpastas (Parameters, com os gerais, e uma por service); o DbUp nomeia cada um pelo caminho
// (ex.: Orders.Script007_...sql), então a ordem de execução é a do número do arquivo, não a da pasta
var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".", new FileSystemScriptOptions { IncludeSubDirectories = true, Filter = IsSelected })
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