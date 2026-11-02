// Importa o pacote Nuget na hora da execução (sem precisar de .csproj)
#r "nuget: dbup-sqlserver" 

using System;
using DbUp;

// tabelas do Quartz do host Worker (docs/PLANO-HOST-WORKER.md §3.3). Roda depois do migrator do Shared, no mesmo banco
// das integrações: a string de conexão dos argumentos ou uma default para teste local
var connectionString = Args.Count > 0 ? Args[0] : "Server=localhost;Database=Company;User Id=sa;Password=root@1234;TrustServerCertificate=True;";

Console.WriteLine($"Conectando em: {connectionString}");

// 1. Garante que o banco existe
EnsureDatabase.For.SqlDatabase(connectionString);

// 2. Configura o DbUp
var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsFromFileSystem(".") 
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
