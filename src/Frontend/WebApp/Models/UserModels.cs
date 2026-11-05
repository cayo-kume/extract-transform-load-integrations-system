namespace WebApp.Models
{
    /// <summary>
    /// Perfis de acesso de monitor.Users (CK_Users_Profile), sempre dentro da holding do usuário: uma holding nunca vê outra.
    /// O perfil diz o que o usuário pode fazer; de quais empresas ele vê os dados vem da empresa dele (LoggedCompany). Ver
    /// todas as holdings é só do dono do aplicativo (UserModel.IsOwner), que não é um perfil.
    /// </summary>
    public static class UserProfiles
    {
        /// <summary>
        /// Administrador da holding: o mesmo que o Gestor e, além disso, dá o perfil Administrador.
        /// </summary>
        public const string Administrator = "Administrador";

        /// <summary>
        /// Vê e muda o que é da empresa dele: ativação, agenda, execução, reconhecimento, contratação, credenciais, Sob
        /// Demanda e cadastros; dá os perfis Gestor e Visualizador.
        /// </summary>
        public const string Manager = "Gestor";

        /// <summary>
        /// Só vê o painel.
        /// </summary>
        public const string Viewer = "Visualizador";

        public static readonly IReadOnlyList<string> All = [Administrator, Manager, Viewer];

        public static string Description(string profile) => profile switch
        {
            Administrator => "Administra a holding: tudo o que o Gestor faz e dá o perfil Administrador.",
            Manager => "Cadastra e muda o painel da empresa.",
            _ => "Só vê o painel."
        };
    }

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

        /// <summary>
        /// Perfil de acesso (UserProfiles).
        /// </summary>
        public string Profile { get; set; } = UserProfiles.Viewer;

        /// <summary>
        /// Dono do aplicativo: vê todas as holdings, o Executar Querys e os Hosts de Agenda. Nasce no primeiro usuário do banco
        /// e não muda pelo painel.
        /// </summary>
        public bool IsOwner { get; set; }

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

        /// <summary>
        /// Perfil de acesso (UserProfiles). Ignorado no primeiro usuário de uma holding, que nasce Gestor (ou Administrador, o
        /// primeiro do banco).
        /// </summary>
        public string? Profile { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}
