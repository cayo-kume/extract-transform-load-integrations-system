// Importa o pacote Nuget na hora da execução (sem precisar de .csproj)
#r "nuget: dbup-postgresql" 

using System;
using DbUp;

// Pega a string de conexão dos argumentos ou usa uma default para teste local
var connectionString = Args.Count > 0 ? Args[0] : "User ID=postgres;Password=root@1234;Host=localhost;Port=5432;Database=Company;Pooling=true;";

Console.WriteLine($"Conectando em: {connectionString}");

// 1. Garante que o banco existe
EnsureDatabase.For.PostgresqlDatabase(connectionString);

// 2. Configura o DbUp
var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
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