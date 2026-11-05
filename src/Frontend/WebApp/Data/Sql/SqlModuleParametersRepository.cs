using Dapper;
using WebApp.Models;

namespace WebApp.Data.Sql
{
    /// <summary>
    /// IModuleParametersRepository nas tabelas *Parameters dos módulos, com Dapper: a linha de cada empresa (doc_company =
    /// empresa). Tabela e colunas vêm só de ModuleDefinitions; os valores vão sempre como parâmetro. Os segredos não saem
    /// daqui: a leitura devolve só se estão definidos.
    /// </summary>
    public class SqlModuleParametersRepository(MainDatabase database) : IModuleParametersRepository
    {
        public async Task<ModuleParametersModel?> GetParametersAsync(int idParent, string company, CancellationToken cancellationToken = default)
        {
            var definition = ModuleDefinitions.Get(idParent);
            await using var connection = await database.OpenAsync(cancellationToken);

            if (!(await MainDatabase.ExistingTablesAsync(connection)).Contains(definition.Table))
                return null;

            var row = (IDictionary<string, object?>?)await connection.QueryFirstOrDefaultAsync(new CommandDefinition(
                $"SELECT CAST(p.doc_company AS varchar(20)) AS doc_company, p.integrated_or_updated_at, {Columns(definition)} FROM {definition.Table} p WHERE p.doc_company = @company",
                new { company = ModuleDefinitions.Digits(company) }, cancellationToken: cancellationToken));

            if (row is null)
                return null;

            return new ModuleParametersModel
            {
                IdParent = idParent,
                DocCompany = (string)row["doc_company"]!,
                UpdatedAt = row["integrated_or_updated_at"] as DateTime?,
                Values = definition.Fields.Where(x => !x.Secret).ToDictionary(x => x.Column, x => Text(row[x.Column])),
                SecretsSet = definition.Fields.Where(x => x.Secret && Text(row[x.Column]) is not null).Select(x => x.Column).ToHashSet()
            };
        }

        public async Task SaveParametersAsync(int idParent, string company, ModuleParametersInput input, CancellationToken cancellationToken = default)
        {
            var definition = ModuleDefinitions.Get(idParent);
            var doc = ModuleDefinitions.Digits(company);

            await using var connection = await database.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            if (!(await MainDatabase.ExistingTablesAsync(connection, transaction)).Contains(definition.Table))
                throw new CompanyRegistrationException($"A tabela de parâmetros de {definition.Title} não existe neste banco: rode o migrator do módulo.");

            // só empresa cadastrada tem parâmetros: os services leem a linha pelo CNPJ da empresa da execução
            var registered = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                "SELECT COUNT(*) FROM [auditing].[Companies] WHERE Company = @doc", new { doc }, transaction, cancellationToken: cancellationToken));
            if (registered == 0)
                throw new CompanyRegistrationException($"A empresa {doc} não está cadastrada.", 50002);

            // a linha atual da empresa, travada até o fim da transação, para saber quais segredos já estão gravados
            var current = (IDictionary<string, object?>?)await connection.QueryFirstOrDefaultAsync(new CommandDefinition(
                $"SELECT {Columns(definition)} FROM {definition.Table} WITH (UPDLOCK, HOLDLOCK) WHERE doc_company = @doc",
                new { doc }, transaction, cancellationToken: cancellationToken));

            var secretsSet = current is null
                ? new HashSet<string>()
                : definition.Fields.Where(x => x.Secret && Text(current[x.Column]) is not null).Select(x => x.Column).ToHashSet();

            var error = ModuleDefinitions.Validate(definition, input, secretsSet);
            if (error is not null)
                throw new CompanyRegistrationException(error);

            // segredo vazio mantém o gravado: a coluna fica fora do UPDATE (num INSERT, segredo obrigatório já foi validado)
            var fields = definition.Fields
                .Where(x => !(x.Secret && string.IsNullOrWhiteSpace(input.Values.GetValueOrDefault(x.Column))))
                .ToList();

            var parameters = new DynamicParameters(new { doc });
            for (var i = 0; i < fields.Count; i++)
                parameters.Add($"p{i}", Value(fields[i], input.Values.GetValueOrDefault(fields[i].Column)));

            var sql = current is null
                ? $"INSERT INTO {definition.Table} (integrated_or_updated_at, doc_company{string.Concat(fields.Select(x => $", [{x.Column}]"))}) VALUES ({MainDatabase.Now}, @doc{string.Concat(fields.Select((_, i) => $", @p{i}"))})"
                : $"UPDATE {definition.Table} SET integrated_or_updated_at = {MainDatabase.Now}{string.Concat(fields.Select((x, i) => $", [{x.Column}] = @p{i}"))} WHERE doc_company = @doc";

            await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction, cancellationToken: cancellationToken));
            await transaction.CommitAsync(cancellationToken);
        }

        private static string Columns(ModuleDefinition definition) =>
            string.Join(", ", definition.Fields.Select(x => $"[{x.Column}]"));

        // uniqueidentifier vai como Guid; vazio vira nulo. A escolha da URL é bit not null: sem valor, 0 (a Base URL)
        private static object? Value(ModuleField field, string? value)
        {
            if (field.Column == ModuleDefinitions.UseHomologUrl)
                return ModuleDefinitions.UsesHomolog(new Dictionary<string, string?> { [field.Column] = value });

            value = value?.Trim();
            if (string.IsNullOrEmpty(value))
                return null;

            return field.Kind == ModuleFieldKind.Guid ? Guid.Parse(value) : value;
        }

        private static string? Text(object? value) => value switch
        {
            null or DBNull => null,
            string text when text.Length == 0 => null,
            _ => value.ToString()
        };
    }
}
