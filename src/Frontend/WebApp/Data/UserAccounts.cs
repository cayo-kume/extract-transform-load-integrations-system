using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Regras do usuário do painel, iguais no mock e no banco: os campos obrigatórios, a senha (8 caracteres ou mais, igual à
    /// confirmação) e o hash (PBKDF2, PasswordHasher do ASP.NET Core). A senha nunca é gravada em texto.
    /// </summary>
    public static class UserAccounts
    {
        public const int MinPasswordLength = 8;

        private static readonly PasswordHasher<UserModel> Hasher = new();

        /// <summary>
        /// Confere os campos de um usuário novo (passwordRequired) ou da edição, e devolve os valores aparados. A empresa não é
        /// conferida aqui: é do repositório.
        /// </summary>
        public static (string Name, string UserName, string Email) Validate(UserInput input, bool passwordRequired)
        {
            var name = input.Name?.Trim();
            var userName = input.UserName?.Trim();
            var email = input.Email?.Trim();

            if (string.IsNullOrEmpty(name))
                throw new CompanyRegistrationException("Informe o nome do usuário.");
            if (string.IsNullOrEmpty(userName))
                throw new CompanyRegistrationException("Informe o usuário.");
            if (userName.Contains('@') || userName.Any(char.IsWhiteSpace))
                throw new CompanyRegistrationException("O usuário não pode ter espaço nem @: o e-mail tem o campo dele.");
            if (string.IsNullOrEmpty(email) || !MailAddress.TryCreate(email, out _))
                throw new CompanyRegistrationException("Informe um e-mail válido.");

            if (passwordRequired || !string.IsNullOrEmpty(input.Password))
            {
                if (string.IsNullOrEmpty(input.Password) || input.Password.Length < MinPasswordLength)
                    throw new CompanyRegistrationException($"A senha precisa ter pelo menos {MinPasswordLength} caracteres.");
                if (input.Password != input.ConfirmPassword)
                    throw new CompanyRegistrationException("A senha e a confirmação não são iguais.");
            }

            return (name, userName, email);
        }

        public static string Hash(string password) => Hasher.HashPassword(null!, password);

        public static bool Verify(string hash, string password) =>
            Hasher.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
    }
}
