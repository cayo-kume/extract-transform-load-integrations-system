namespace WebApp.Models
{
    /// <summary>
    /// Usuário do painel em monitor.Users: entra pela tela inicial com o usuário ou o e-mail e vê a holding da empresa dele.
    /// A senha fica só como hash e nunca sai do repositório.
    /// </summary>
    public class UserModel
    {
        public int IdUser { get; set; }

        /// <summary>
        /// CNPJ da empresa (auditing.Companies).
        /// </summary>
        public string Company { get; set; } = "";

        /// <summary>
        /// CNPJ da holding da empresa.
        /// </summary>
        public string Holder { get; set; } = "";

        public string? CompanyName { get; set; }

        public string Name { get; set; } = "";

        public string UserName { get; set; } = "";

        public string Email { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public string CompanyLabel => string.IsNullOrWhiteSpace(CompanyName) ? Company : CompanyName;
    }

    /// <summary>
    /// O que a tela manda gravar. Na edição, o usuário não muda, e senha vazia mantém a gravada.
    /// </summary>
    public class UserInput
    {
        public string? Company { get; set; }

        public string? Name { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}
