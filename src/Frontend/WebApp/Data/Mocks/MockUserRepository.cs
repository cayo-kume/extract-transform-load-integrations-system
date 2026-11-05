using WebApp.Models;

namespace WebApp.Data.Mocks
{
    /// <summary>
    /// IUserRepository em memória (MockCompanyStore), com as mesmas regras do SqlUserRepository.
    /// </summary>
    public class MockUserRepository(MockCompanyStore store) : IUserRepository
    {
        public Task<UserModel?> ValidateAsync(string login, string password, CancellationToken cancellationToken = default)
        {
            var key = login.Trim();

            lock (store.Gate)
            {
                var found = store.Users.FirstOrDefault(x =>
                    string.Equals(x.User.UserName, key, StringComparison.OrdinalIgnoreCase) || string.Equals(x.User.Email, key, StringComparison.OrdinalIgnoreCase));

                return Task.FromResult(found.User is not null && UserAccounts.Verify(found.PasswordHash, password) ? WithCompany(found.User) : null);
            }
        }

        public Task<IReadOnlyList<UserModel>> GetUsersAsync(string holder, CancellationToken cancellationToken = default)
        {
            var key = ModuleDefinitions.Digits(holder);

            lock (store.Gate)
            {
                IReadOnlyList<UserModel> users = store.Users
                    .Select(x => WithCompany(x.User))
                    .Where(x => x.Holder == key)
                    .OrderBy(x => x.Name)
                    .ToList();

                return Task.FromResult(users);
            }
        }

        public Task CreateUserAsync(UserInput input, CancellationToken cancellationToken = default)
        {
            var company = ModuleDefinitions.Digits(input.Company);

            lock (store.Gate)
            {
                if (!store.Companies.ContainsKey(company))
                    throw new CompanyRegistrationException($"A empresa {input.Company} não está cadastrada.", 50002);

                store.Users.Add(NewUser(store, input, company));
            }

            return Task.CompletedTask;
        }

        public Task UpdateUserAsync(int idUser, UserInput input, CancellationToken cancellationToken = default)
        {
            lock (store.Gate)
            {
                var index = store.Users.FindIndex(x => x.User.IdUser == idUser);
                if (index < 0)
                    throw new CompanyRegistrationException("O usuário não está cadastrado.");

                // o usuário (login) não muda: vale o gravado
                var (user, hash) = store.Users[index];
                input.UserName = user.UserName;
                var (name, _, email) = UserAccounts.Validate(input, passwordRequired: false);

                if (store.Users.Any(x => x.User.IdUser != idUser && string.Equals(x.User.Email, email, StringComparison.OrdinalIgnoreCase)))
                    throw new CompanyRegistrationException($"O e-mail {email} já é de outro usuário.");

                user.Name = name;
                user.Email = email;
                store.Users[index] = (user, string.IsNullOrEmpty(input.Password) ? hash : UserAccounts.Hash(input.Password));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Confere o usuário novo e devolve a linha para gravar. Dentro do lock do MockCompanyStore.
        /// </summary>
        internal static (UserModel User, string PasswordHash) NewUser(MockCompanyStore store, UserInput input, string company)
        {
            var (name, userName, email) = UserAccounts.Validate(input, passwordRequired: true);

            if (store.Users.Any(x => string.Equals(x.User.UserName, userName, StringComparison.OrdinalIgnoreCase)))
                throw new CompanyRegistrationException($"O usuário {userName} já existe.");
            if (store.Users.Any(x => string.Equals(x.User.Email, email, StringComparison.OrdinalIgnoreCase)))
                throw new CompanyRegistrationException($"O e-mail {email} já é de outro usuário.");

            var user = new UserModel
            {
                IdUser = store.Users.Count == 0 ? 1 : store.Users.Max(x => x.User.IdUser) + 1,
                Company = company,
                Name = name,
                UserName = userName,
                Email = email,
                CreatedAt = DateTime.Now
            };

            return (user, UserAccounts.Hash(input.Password!));
        }

        // a holding e o nome da empresa, lidos do cadastro (dentro do lock)
        private UserModel WithCompany(UserModel user)
        {
            var company = store.Companies.GetValueOrDefault(user.Company);

            return new UserModel
            {
                IdUser = user.IdUser,
                Company = user.Company,
                Holder = company?.Holder ?? user.Company,
                CompanyName = company?.FantasyName,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
