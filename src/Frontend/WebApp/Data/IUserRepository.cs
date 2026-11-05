using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Usuários do painel (monitor.Users). Até o banco, o MockUserRepository; com a connection string, o SqlUserRepository.
    /// O primeiro usuário de uma holding nasce com ela (ICompanyRepository.CreateHolderAsync). Erros de regra (usuário ou
    /// e-mail repetido, senha fraca) chegam como CompanyRegistrationException, com a mensagem para a tela.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// O usuário com este usuário ou e-mail e esta senha; null se não existir ou a senha não bater.
        /// </summary>
        Task<UserModel?> ValidateAsync(string login, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Usuários das empresas da holding.
        /// </summary>
        Task<IReadOnlyList<UserModel>> GetUsersAsync(string holder, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cria o usuário numa empresa já cadastrada.
        /// </summary>
        Task CreateUserAsync(UserInput input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Muda nome, e-mail e, preenchida, a senha. O usuário (login) não muda.
        /// </summary>
        Task UpdateUserAsync(int idUser, UserInput input, CancellationToken cancellationToken = default);
    }
}
