using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Data.Sql
{
    /// <summary>
    /// IUserRepository no banco principal, com Dapper: monitor.Users, com o hash da senha (UserAccounts). Usuário e e-mail
    /// são únicos (UQ_Users_UserName e UQ_Users_Email); a empresa precisa estar em auditing.Companies (FK_Users_Companies).
    /// </summary>
    public class SqlUserRepository(MainDatabase database) : IUserRepository
    {
        private const string SelectUsers = """
            SELECT u.IdUser, u.Company, c.Holder, c.FantasyName AS CompanyName, u.Name, u.UserName, u.Email, u.CreatedAt, u.PasswordHash
            FROM [monitor].[Users] u
            JOIN [auditing].[Companies] c ON c.Company = u.Company
            """;

        public async Task<UserModel?> ValidateAsync(string login, string password, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var row = await connection.QueryFirstOrDefaultAsync<UserRow>(new CommandDefinition(
                SelectUsers + " WHERE u.UserName = @login OR u.Email = @login", new { login = login.Trim() }, cancellationToken: cancellationToken));

            return row is not null && UserAccounts.Verify(row.PasswordHash, password) ? row : null;
        }

        public async Task<IReadOnlyList<UserModel>> GetUsersAsync(string holder, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var rows = await connection.QueryAsync<UserRow>(new CommandDefinition(
                SelectUsers + " WHERE c.Holder = @holder ORDER BY u.Name",
                new { holder = new DbString { Value = ModuleDefinitions.Digits(holder), IsAnsi = true, Length = 20 } }, cancellationToken: cancellationToken));

            return rows.ToList();
        }

        public async Task CreateUserAsync(UserInput input, CancellationToken cancellationToken = default)
        {
            var company = ModuleDefinitions.Digits(input.Company);

            await using var connection = await database.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            var exists = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                "SELECT COUNT(*) FROM [auditing].[Companies] WHERE Company = @company",
                new { company = new DbString { Value = company, IsAnsi = true, Length = 20 } }, transaction, cancellationToken: cancellationToken));
            if (exists == 0)
                throw new CompanyRegistrationException($"A empresa {input.Company} não está cadastrada.", 50002);

            await InsertAsync(connection, transaction, input, company, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task UpdateUserAsync(int idUser, UserInput input, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);

            // o usuário (login) não muda: vale o gravado
            input.UserName = await connection.ExecuteScalarAsync<string?>(new CommandDefinition(
                "SELECT UserName FROM [monitor].[Users] WHERE IdUser = @idUser", new { idUser }, cancellationToken: cancellationToken))
                ?? throw new CompanyRegistrationException("O usuário não está cadastrado.");

            var (name, _, email) = UserAccounts.Validate(input, passwordRequired: false);
            var hash = string.IsNullOrEmpty(input.Password) ? null : UserAccounts.Hash(input.Password);

            try
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    UPDATE [monitor].[Users]
                    SET Name = @name, Email = @email, PasswordHash = COALESCE(@hash, PasswordHash)
                    WHERE IdUser = @idUser
                    """, new { idUser, name, email, hash }, cancellationToken: cancellationToken));
            }
            catch (SqlException exception) when (exception.Number is 2627 or 2601)
            {
                throw new CompanyRegistrationException($"O e-mail {email} já é de outro usuário.", inner: exception);
            }
        }

        /// <summary>
        /// Confere e grava um usuário novo na transação de quem chama (o cadastro da holding também usa).
        /// </summary>
        internal static async Task InsertAsync(SqlConnection connection, IDbTransaction transaction, UserInput input, string company, CancellationToken cancellationToken)
        {
            var (name, userName, email) = UserAccounts.Validate(input, passwordRequired: true);

            var taken = await connection.QueryFirstOrDefaultAsync<string?>(new CommandDefinition("""
                SELECT TOP (1) CASE WHEN UserName = @userName THEN 'user' ELSE 'email' END
                FROM [monitor].[Users]
                WHERE UserName = @userName OR Email = @email
                """, new { userName, email }, transaction, cancellationToken: cancellationToken));

            if (taken == "user")
                throw new CompanyRegistrationException($"O usuário {userName} já existe.");
            if (taken == "email")
                throw new CompanyRegistrationException($"O e-mail {email} já é de outro usuário.");

            await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO [monitor].[Users] (Company, Name, UserName, Email, PasswordHash)
                VALUES (@company, @name, @userName, @email, @hash)
                """, new { company = new DbString { Value = company, IsAnsi = true, Length = 20 }, name, userName, email, hash = UserAccounts.Hash(input.Password!) },
                transaction, cancellationToken: cancellationToken));
        }

        // a linha com o hash, que não sai daqui: quem recebe vê só o UserModel
        private sealed class UserRow : UserModel
        {
            public string PasswordHash { get; set; } = "";
        }
    }
}
