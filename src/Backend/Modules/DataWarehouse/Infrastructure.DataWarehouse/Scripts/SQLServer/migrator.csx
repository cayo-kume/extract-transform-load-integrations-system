// Importa o pacote Nuget na hora da execução (sem precisar de .csproj)
#r "nuget: dbup-sqlserver"

using System;
using System.Collections.Generic;
using System.Linq;
using DbUp;
using DbUp.ScriptProviders;
using Microsoft.Data.SqlClient;

// A connection string é obrigatória e vem só do argumento: sem padrão no código e sem imprimir a string,
// para a senha não aparecer em log (docs/datawarehouse, seção 16, passo 1)
if (Args.Count == 0 || String.IsNullOrWhiteSpace(Args[0]))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("A connection string é obrigatória: dotnet script migrator.csx \"<connection string>\"");
    Console.WriteLine("Informe o servidor (Server), o banco (Database) e a autenticação (User Id e Password, ou Integrated Security=True, ou Authentication).");
    Environment.Exit(-1);
}

var connectionString = Args[0];

SqlConnectionStringBuilder builder;

try
{
    builder = new SqlConnectionStringBuilder(connectionString);
}
catch (Exception)
{
    // a mensagem da exceção pode repetir trechos da string; só o aviso
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("A connection string informada não é válida. Confira o formato: Server=...;Database=...;User Id=...;Password=...;");
    Environment.Exit(-1);
    return;
}

var missing = new List<string>();

if (String.IsNullOrWhiteSpace(builder.DataSource))
    missing.Add("servidor (Server)");

if (String.IsNullOrWhiteSpace(builder.InitialCatalog))
    missing.Add("banco (Database)");

var hasAuthentication = builder.IntegratedSecurity || builder.Authentication != SqlAuthenticationMethod.NotSpecified;

if (!hasAuthentication && String.IsNullOrWhiteSpace(builder.UserID))
    missing.Add("usuário (User Id), ou Integrated Security=True, ou Authentication");

if (!hasAuthentication && !String.IsNullOrWhiteSpace(builder.UserID) && String.IsNullOrWhiteSpace(builder.Password))
    missing.Add("senha (Password)");

if (missing.Count > 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"A connection string está incompleta. Falta: {String.Join("; ", missing)}.");
    Environment.Exit(-1);
}

// só servidor e banco: nada de usuário ou senha no console
Console.WriteLine($"Conectando em: servidor {builder.DataSource}, banco {builder.InitialCatalog}");

// 1. Garante que o banco existe (o nome vem da connection string)
EnsureDatabase.For.SqlDatabase(connectionString);

// 2. Configura o DbUp. Tempo limite por comando de 10 min (o padrão é 30 s): logo depois da criação de centenas de
// tabelas pelos outros módulos, a primeira geração de procedure pode passar de 30 s.
// Os scripts ficam em subpastas por camada (Parameters, Landing, History...); o DbUp nomeia cada um pelo caminho
// (ex.: Landing.Script008_...sql), então a ordem de execução é a do número do arquivo, não a da pasta
var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".", new FileSystemScriptOptions { IncludeSubDirectories = true })
    .WithScriptSorter(scripts => scripts.OrderBy(s => s.Name.Split('.')[^2], StringComparer.Ordinal))
    .WithExecutionTimeout(TimeSpan.FromMinutes(10))
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
