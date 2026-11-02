using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Cadastro das empresas do painel (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.5): monitor.Company, os CNPJs ligados a cada
    /// uma em monitor.HoldingCompanies e a contratação dos módulos (auditing.sp_Add_Company_App e sp_Remove_Company_App).
    /// Até o banco, o MockCompanyRepository; com a connection string, o SqlCompanyRepository (Dapper, sem API e sem Shared).
    /// Erros de regra (empresa inexistente, CNPJ de outra holding) chegam como CompanyRegistrationException.
    /// </summary>
    public interface ICompanyRepository
    {
        Task<IReadOnlyList<CompanyModel>> GetCompaniesAsync(CancellationToken cancellationToken = default);

        Task<CompanyModel?> GetCompanyAsync(string company, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cadastra a empresa (auditing.sp_Add_Company, que também cria os jobs dos hosts dela). Já cadastrada é erro.
        /// </summary>
        Task CreateCompanyAsync(CompanyModel company, CancellationToken cancellationToken = default);

        /// <summary>
        /// Muda razão social e nome fantasia. O CNPJ não muda: é a chave dos jobs e dos logs da empresa.
        /// </summary>
        Task UpdateCompanyAsync(string company, string name, string? tradeName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ativa ou desativa a empresa. Desativada, nenhum host roda os jobs dela (fase 4.4).
        /// </summary>
        Task SetCompanyActiveAsync(string company, bool isActive, CancellationToken cancellationToken = default);

        /// <summary>
        /// CNPJs ligados à empresa em monitor.HoldingCompanies, com os módulos em que cada um tem parâmetros.
        /// </summary>
        Task<IReadOnlyList<HoldingCompanyModel>> GetHoldingCompaniesAsync(string company, CancellationToken cancellationToken = default);

        /// <summary>
        /// Todos os módulos do catálogo (auditing.Apps), com os jobs da empresa em cada um.
        /// </summary>
        Task<IReadOnlyList<CompanyAppModel>> GetAppsAsync(string company, CancellationToken cancellationToken = default);

        /// <summary>
        /// Contrata o módulo: copia os jobs do catálogo para a empresa, inativos. Rodar de novo não duplica; devolve
        /// quantos jobs foram criados agora.
        /// </summary>
        Task<int> AddAppAsync(string company, int idParent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove o módulo: desativa os jobs dele na empresa (não apaga, porque os logs referenciam os jobs).
        /// </summary>
        Task RemoveAppAsync(string company, int idParent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Parâmetros (credenciais) de cada módulo por empresa, nas tabelas *Parameters (docs/PLANO-TELA-CADASTRO-EMPRESA.md,
    /// 4.5). As colunas vêm de ModuleDefinitions. Segredos nunca voltam: a leitura diz só se estão definidos, e gravar com
    /// o segredo vazio mantém o valor gravado.
    /// </summary>
    public interface IModuleParametersRepository
    {
        /// <summary>
        /// As linhas do módulo dos CNPJs ligados à empresa (uma por CNPJ).
        /// </summary>
        Task<IReadOnlyList<ModuleParametersModel>> GetParametersAsync(int idParent, string company, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cria ou atualiza a linha do CNPJ no módulo e liga o CNPJ à empresa (auditing.sp_Add_Holding_Company), na mesma
        /// transação.
        /// </summary>
        Task SaveParametersAsync(int idParent, string company, ModuleParametersInput input, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Erro de regra do cadastro, com a mensagem para a tela. Code é o número do THROW das procedures (50002 empresa não
    /// cadastrada, 50003 CNPJ de outra holding, 50004 CNPJ de outra empresa do painel) ou 0 para validação do WebApp.
    /// </summary>
    public class CompanyRegistrationException(string message, int code = 0, Exception? inner = null) : Exception(message, inner)
    {
        public int Code { get; } = code;
    }
}
