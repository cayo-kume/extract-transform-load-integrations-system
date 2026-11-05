using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Cadastro das holdings (auditing.Holders), das empresas delas (auditing.Companies) e dos módulos contratados por empresa
    /// (os jobs em auditing.Jobs, a partir do catálogo), com INSERT direto (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md).
    /// Até o banco, o MockCompanyRepository; com a connection string, o SqlCompanyRepository (Dapper, sem API e sem Shared).
    /// Erros de regra (CNPJ já cadastrado, de outra holding, empresa inexistente) chegam como CompanyRegistrationException.
    /// </summary>
    public interface ICompanyRepository
    {
        Task<IReadOnlyList<HolderModel>> GetHoldersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Cadastra a holding, a empresa que é a própria holding (Company = Holder), os jobs dos hosts dela (IdParent 140),
        /// inativos e com a agenda sugerida, e o primeiro usuário do painel, na empresa da holding, numa transação. CNPJ já
        /// cadastrado é erro, e nada é gravado.
        /// </summary>
        Task CreateHolderAsync(string holder, string? fantasyName, UserInput firstUser, CancellationToken cancellationToken = default);

        /// <summary>
        /// Muda o nome fantasia. O CNPJ não muda: é a chave dos jobs e dos logs.
        /// </summary>
        Task UpdateHolderAsync(string holder, string? fantasyName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Empresas da holding (a própria holding primeiro), com os módulos em que cada uma tem parâmetros.
        /// </summary>
        Task<IReadOnlyList<CompanyModel>> GetCompaniesAsync(string holder, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cadastra uma empresa na holding. Um CNPJ já cadastrado (nesta ou em outra holding) é recusado.
        /// </summary>
        Task CreateCompanyAsync(string holder, string company, string? fantasyName, CancellationToken cancellationToken = default);

        Task UpdateCompanyAsync(string company, string? fantasyName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Todos os módulos do catálogo (auditing.Apps), com os jobs da empresa em cada um.
        /// </summary>
        Task<IReadOnlyList<CompanyAppModel>> GetAppsAsync(string company, CancellationToken cancellationToken = default);

        /// <summary>
        /// Jobs do módulo no catálogo (auditing.fn_CatalogTree, com as procedures), com a agenda sugerida.
        /// </summary>
        Task<IReadOnlyList<CatalogJobModel>> GetCatalogJobsAsync(int idParent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Contrata o módulo: cria os jobs dele para a empresa, inativos, com a agenda de <paramref name="jobs"/> (os que
        /// faltarem ficam com a sugerida). Rodar de novo não duplica; devolve quantos jobs foram criados agora.
        /// </summary>
        Task<int> AddAppAsync(string company, int idParent, IReadOnlyList<CatalogJobModel> jobs, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove o módulo: desativa os jobs dele na empresa (não apaga, porque os logs referenciam os jobs).
        /// </summary>
        Task RemoveAppAsync(string company, int idParent, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Parâmetros (credenciais) de cada módulo por empresa, nas tabelas *Parameters (doc_company = empresa). As colunas vêm de
    /// ModuleDefinitions. Segredos nunca voltam: a leitura diz só se estão definidos, e gravar com o segredo vazio mantém o
    /// valor gravado.
    /// </summary>
    public interface IModuleParametersRepository
    {
        /// <summary>
        /// A linha do módulo da empresa; null sem parâmetros ou sem a tabela do módulo neste banco.
        /// </summary>
        Task<ModuleParametersModel?> GetParametersAsync(int idParent, string company, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cria ou atualiza a linha da empresa no módulo. A empresa precisa estar em auditing.Companies.
        /// </summary>
        Task SaveParametersAsync(int idParent, string company, ModuleParametersInput input, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Erro de regra do cadastro, com a mensagem para a tela. Code é 50001 (CNPJ já cadastrado), 50002 (empresa não
    /// cadastrada) ou 0 para validação do WebApp.
    /// </summary>
    public class CompanyRegistrationException(string message, int code = 0, Exception? inner = null) : Exception(message, inner)
    {
        public const int AlreadyRegistered = 50001;

        public int Code { get; } = code;
    }
}
