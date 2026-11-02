using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using WebApp.Models;

namespace WebApp.Data.Sql
{
    /// <summary>
    /// ICompanyRepository no banco principal, com Dapper (sem API e sem Shared, docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.5).
    /// </summary>
    public class SqlCompanyRepository(MainDatabase database) : ICompanyRepository
    {
        private const string SelectCompany = "SELECT Company, [Name], TradeName, IsActive, CreatedAt, UpdatedAt FROM [monitor].[Company]";

        public async Task<IReadOnlyList<CompanyModel>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var rows = await connection.QueryAsync<CompanyModel>(new CommandDefinition($"{SelectCompany} ORDER BY [Name]", cancellationToken: cancellationToken));
            return rows.ToList();
        }

        public async Task<CompanyModel?> GetCompanyAsync(string company, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<CompanyModel>(new CommandDefinition(
                $"{SelectCompany} WHERE Company = @company", new { company = ModuleDefinitions.Digits(company) }, cancellationToken: cancellationToken));
        }

        public async Task CreateCompanyAsync(CompanyModel company, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company.Company);
            if (key.Length is not (11 or 14))
                throw new CompanyRegistrationException("O CNPJ precisa ter 14 dígitos (ou 11, de CPF).");
            if (string.IsNullOrWhiteSpace(company.Name))
                throw new CompanyRegistrationException("A razão social é obrigatória.");

            await using var connection = await database.OpenAsync(cancellationToken);

            // sp_Add_Company só confere uma empresa já cadastrada; aqui, cadastrar de novo é erro de quem preencheu
            var exists = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
                "SELECT COUNT(*) FROM [monitor].[Company] WHERE Company = @company", new { company = key }, cancellationToken: cancellationToken));
            if (exists > 0)
                throw new CompanyRegistrationException($"A empresa {key} já está cadastrada.");

            await MainDatabase.Translate(() => connection.ExecuteAsync(new CommandDefinition(
                "[auditing].[sp_Add_Company]",
                new { company = key, name = company.Name.Trim(), tradeName = Blank(company.TradeName) },
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken)));
        }

        public async Task UpdateCompanyAsync(string company, string name, string? tradeName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new CompanyRegistrationException("A razão social é obrigatória.");

            await using var connection = await database.OpenAsync(cancellationToken);
            var updated = await connection.ExecuteAsync(new CommandDefinition(
                $"UPDATE [monitor].[Company] SET [Name] = @name, TradeName = @tradeName, UpdatedAt = {MainDatabase.Now} WHERE Company = @company",
                new { company = ModuleDefinitions.Digits(company), name = name.Trim(), tradeName = Blank(tradeName) }, cancellationToken: cancellationToken));

            if (updated == 0)
                throw NotFound(company);
        }

        public async Task SetCompanyActiveAsync(string company, bool isActive, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            var updated = await connection.ExecuteAsync(new CommandDefinition(
                $"UPDATE [monitor].[Company] SET IsActive = @isActive, UpdatedAt = {MainDatabase.Now} WHERE Company = @company",
                new { company = ModuleDefinitions.Digits(company), isActive }, cancellationToken: cancellationToken));

            if (updated == 0)
                throw NotFound(company);
        }

        public async Task<IReadOnlyList<HoldingCompanyModel>> GetHoldingCompaniesAsync(string company, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(company);
            await using var connection = await database.OpenAsync(cancellationToken);

            var companies = (await connection.QueryAsync<HoldingCompanyModel>(new CommandDefinition(
                "SELECT Company, [Name], CreatedAt FROM [monitor].[HoldingCompanies] WHERE Holding = @company ORDER BY Company",
                new { company = key }, cancellationToken: cancellationToken))).ToList();

            // em que módulos cada CNPJ tem parâmetros: um SELECT por tabela *Parameters que existe no banco. Tabela e IdParent
            // vêm de ModuleDefinitions, nunca da tela
            var existing = await MainDatabase.ExistingTablesAsync(connection);
            var queries = ModuleDefinitions.All
                .Where(x => existing.Contains(x.Table))
                .Select(x => $"SELECT DISTINCT {x.IdParent} AS IdParent, CAST(p.doc_company AS varchar(20)) AS DocCompany FROM {x.Table} p JOIN [monitor].[HoldingCompanies] h ON h.Company = p.doc_company WHERE h.Holding = @company")
                .ToList();

            if (queries.Count > 0)
            {
                var modules = await connection.QueryAsync<(int IdParent, string DocCompany)>(new CommandDefinition(
                    string.Join(" UNION ALL ", queries), new { company = key }, cancellationToken: cancellationToken));
                var byCompany = modules.ToLookup(x => x.DocCompany, x => x.IdParent);

                foreach (var holdingCompany in companies)
                    holdingCompany.Modules = byCompany[holdingCompany.Company].Order().ToList();
            }

            return companies;
        }

        public async Task<IReadOnlyList<CompanyAppModel>> GetAppsAsync(string company, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);

            // os jobs do módulo são os da árvore do catálogo (integrações e as procedures que elas chamam), como em
            // auditing.sp_Add_Company_App e sp_Remove_Company_App
            var rows = await connection.QueryAsync<CompanyAppModel>(new CommandDefinition("""
                SELECT
                    a.IdParent,
                    a.Title,
                    a.[Description],
                    COUNT(j.IdJob) AS Jobs,
                    COALESCE(SUM(CASE WHEN j.IsActive = 1 THEN 1 ELSE 0 END), 0) AS ActiveJobs
                FROM
                    [auditing].[Apps] a
                    OUTER APPLY [auditing].[fn_JobCatalogTree](a.IdParent) t
                    LEFT JOIN [auditing].[Jobs] j ON j.IdJob = t.IdJob AND j.IdType = t.IdType AND j.Company = @company
                GROUP BY
                    a.IdParent, a.Title, a.[Description]
                ORDER BY
                    a.Title
                """, new { company = ModuleDefinitions.Digits(company) }, cancellationToken: cancellationToken));

            return rows.ToList();
        }

        public async Task<int> AddAppAsync(string company, int idParent, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            return await MainDatabase.Translate(() => connection.ExecuteScalarAsync<int>(new CommandDefinition(
                "[auditing].[sp_Add_Company_App]", new { company = ModuleDefinitions.Digits(company), idParent },
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken)));
        }

        public async Task RemoveAppAsync(string company, int idParent, CancellationToken cancellationToken = default)
        {
            await using var connection = await database.OpenAsync(cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition(
                "[auditing].[sp_Remove_Company_App]", new { company = ModuleDefinitions.Digits(company), idParent },
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        }

        private static CompanyRegistrationException NotFound(string company) =>
            new($"A empresa {company} não está cadastrada.", 50002);

        private static string? Blank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
