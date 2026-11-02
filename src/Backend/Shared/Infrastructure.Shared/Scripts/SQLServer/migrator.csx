// Importa o pacote Nuget na hora da execução (sem precisar de .csproj)
#r "nuget: dbup-sqlserver"

using System;
using DbUp;
using Microsoft.Data.SqlClient;

// Pega a string de conexão dos argumentos ou usa uma default para teste local
var connectionString = Args.Count > 0 ? Args[0] : "Server=localhost;Database=Company;User Id=sa;Password=root@1234;TrustServerCertificate=True;";

// só servidor e banco: nada de usuário ou senha no console
var builder = new SqlConnectionStringBuilder(connectionString);
Console.WriteLine($"Conectando em: servidor {builder.DataSource}, banco {builder.InitialCatalog}");

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

// 4. Usuários de banco do WebApp (Script009, src/Frontend/WebApp/docs/PLANO-WEBAPP.md, passo 10): só com a senha na
// variável de ambiente, nunca no argumento nem no script. Sem a variável, quem administra roda a procedure à mão.
// Banco novo: o Script001 troca a collation do banco, e a conexão do pool aberta antes dele não volta mais ("Resetting the
// connection results in a different state than the initial login", visto no teste C1). Limpar o pool abre uma conexão nova
SqlConnection.ClearAllPools();

foreach (var (procedure, variable) in new[] { ("dbo.sp_Create_Monitor_User", "ETL_MONITOR_PASSWORD"), ("dbo.sp_Grant_Query_User", "ETL_QUERY_PASSWORD") })
{
    var password = Environment.GetEnvironmentVariable(variable);
    if (String.IsNullOrEmpty(password))
    {
        Console.WriteLine($"{variable} não definida: {procedure} não rodou.");
        continue;
    }

    using var connection = new SqlConnection(connectionString);
    connection.Open();

    using var command = new SqlCommand(procedure, connection) { CommandType = System.Data.CommandType.StoredProcedure, CommandTimeout = 300 };
    command.Parameters.AddWithValue("@password", password);

    try
    {
        command.ExecuteNonQuery();
        Console.WriteLine($"{procedure}: ok.");
    }
    catch (SqlException exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{procedure}: {exception.Message}");
        Environment.Exit(-1);
    }
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Sucesso!");
