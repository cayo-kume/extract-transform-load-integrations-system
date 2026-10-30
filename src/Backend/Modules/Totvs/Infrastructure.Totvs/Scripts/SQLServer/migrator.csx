// Importa o pacote Nuget na hora da execução (sem precisar de .csproj)
#r "nuget: dbup-sqlserver" 

using System;
using System.IO;
using System.Linq;
using DbUp;
using DbUp.ScriptProviders;

// Pega a string de conexão dos argumentos ou usa uma default para teste local
var connectionString = Args.Count > 0 ? Args[0] : "Server=localhost;Database=Company;User Id=sa;Password=root@1234;TrustServerCertificate=True;";

Console.WriteLine($"Conectando em: {connectionString}");

// 1. Garante que o banco existe
EnsureDatabase.For.SqlDatabase(connectionString);

// 2. Configura o DbUp
// os scripts ficam em subpastas (Parameters, com os gerais, e uma por service); o DbUp nomeia cada um pelo caminho
// (ex.: People.Script005_...sql), então a ordem de execução é a do número do arquivo, não a da pasta.
// A pasta Homologacao fica de fora: o Script003 de homologação roda à mão, no lugar do de produção
var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".", new FileSystemScriptOptions
    {
        IncludeSubDirectories = true,
        Filter = path => !Path.GetRelativePath(".", path).StartsWith("Homologacao", StringComparison.OrdinalIgnoreCase)
    })
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