namespace WebApp.Models
{
    /// <summary>
    /// Holding e empresa do painel, mostradas no topo (.docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md). A visão é só da
    /// empresa escolhida: nenhuma tela mostra dados de outra. Uma por aba (circuito): o seletor do topo troca a empresa e
    /// guarda a escolha no navegador; o MainLayout lê a escolha antes de mostrar a primeira tela. Até a fase 6 do plano
    /// (.docs/PLANO-WEBAPP.md), a primeira escolha vem da configuração (LoggedCompany:DocCompany e LoggedCompany:Name).
    /// O CNPJ da empresa é o Company de auditing.Jobs, e o da holding, o Holder.
    /// </summary>
    public class LoggedCompany(IConfiguration configuration)
    {
        private const string DefaultDocument = "44065634000106";
        private const string DefaultName = "NEWBLOOMERS";

        /// <summary>
        /// CNPJ da holding (auditing.Holders).
        /// </summary>
        public string Holder { get; private set; } = Configured(configuration, "LoggedCompany:DocCompany", DefaultDocument);

        public string HolderName { get; private set; } = Configured(configuration, "LoggedCompany:Name", DefaultName);

        /// <summary>
        /// CNPJ da empresa (auditing.Companies): o doc_company dos parâmetros e o Company dos jobs.
        /// </summary>
        public string DocCompany { get; private set; } = Configured(configuration, "LoggedCompany:DocCompany", DefaultDocument);

        /// <summary>
        /// Nome fantasia da empresa.
        /// </summary>
        public string Name { get; private set; } = Configured(configuration, "LoggedCompany:Name", DefaultName);

        public string Label => $"{Name} - {DocCompany}";

        /// <summary>
        /// Empresas que o usuário logado vê na holding escolhida (as do seletor).
        /// </summary>
        public IReadOnlyList<CompanyModel> VisibleCompanies { get; private set; } = [];

        /// <summary>
        /// A empresa escolhida, a que as telas mostram (como lista, para o CompanyView).
        /// </summary>
        public IReadOnlyList<CompanyModel> ScopeCompanies =>
            VisibleCompanies.Where(x => x.Company == DocCompany).DefaultIfEmpty(new CompanyModel { Company = DocCompany, Holder = Holder, FantasyName = Name }).ToList();

        /// <summary>
        /// A própria holding está escolhida no seletor (não uma filial).
        /// </summary>
        public bool IsHolderSelected => DocCompany == Holder;

        /// <summary>
        /// As empresas que a Visão Geral soma: com a holding escolhida, ela e todas as empresas que o usuário vê; com uma
        /// filial, só a filial.
        /// </summary>
        public IReadOnlyList<CompanyModel> SummedCompanies => IsHolderSelected && SeesWholeHolding && VisibleCompanies.Count > 1 ? VisibleCompanies : ScopeCompanies;

        /// <summary>
        /// Nome curto de uma empresa visível, para os chips e as colunas Empresa (sem o nome da holding na frente, quando a
        /// empresa começa com ele).
        /// </summary>
        public string CompanyLabel(string company)
        {
            var label = VisibleCompanies.FirstOrDefault(x => x.Company == company)?.Label ?? company;
            var prefix = HolderName + " ";
            return company != Holder && label.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && label.Length > prefix.Length ? label[prefix.Length..] : label;
        }

        public void SetVisibleCompanies(IReadOnlyList<CompanyModel> visible) => VisibleCompanies = visible;

        /// <summary>
        /// CNPJ da empresa do usuário logado (monitor.Users); vazio sem login (tela de cadastro).
        /// </summary>
        public string UserCompany { get; private set; } = "";

        /// <summary>
        /// O usuário é da própria holding: vê todas as empresas dela. O de uma empresa só vê a empresa dele, nem as outras
        /// empresas nem a holding.
        /// </summary>
        public bool IsHoldingUser => UserCompany.Length > 0 && UserCompany == Holder;

        /// <summary>
        /// Vê todas as empresas da holding escolhida: o usuário da holding e o dono do aplicativo (que vê todas as holdings).
        /// O perfil não muda isso: um Administrador de uma empresa só vê a empresa dele.
        /// </summary>
        public bool SeesWholeHolding => IsOwner || IsHoldingUser;

        /// <summary>
        /// Empresa da holding escolhida que o usuário logado pode ver.
        /// </summary>
        public bool CanSee(string company) => SeesWholeHolding || company == UserCompany;

        /// <summary>
        /// Perfil de acesso do usuário logado (UserProfiles); vazio sem login.
        /// </summary>
        public string Profile { get; private set; } = "";

        /// <summary>
        /// Administrador da holding: dá o perfil Administrador. Não vê outras holdings.
        /// </summary>
        public bool IsAdministrator => Profile == UserProfiles.Administrator;

        /// <summary>
        /// Dono do aplicativo: todas as holdings, Executar Querys, Registros no SQL e Hosts de Agenda (que leem o banco inteiro).
        /// </summary>
        public bool IsOwner { get; private set; }

        /// <summary>
        /// Administrador ou Gestor: muda o painel (ativação, agenda, execução, reconhecimento, contratação, credenciais), abre
        /// o Sob Demanda e os cadastros. O Visualizador só vê.
        /// </summary>
        public bool CanManage => Profile is UserProfiles.Administrator or UserProfiles.Manager;

        /// <summary>
        /// Usuário logado (monitor.Users.IdUser): ninguém muda o próprio perfil.
        /// </summary>
        public int IdUser { get; private set; }

        public void SetUser(int idUser, string userCompany, string profile, bool isOwner)
        {
            IdUser = idUser;
            UserCompany = userCompany;
            Profile = profile;
            IsOwner = isOwner;
        }

        /// <summary>
        /// Troca a holding e a empresa da aba. Nome vazio vira o CNPJ.
        /// </summary>
        public void Set(string holder, string? holderName, string company, string? companyName)
        {
            Holder = holder;
            HolderName = string.IsNullOrWhiteSpace(holderName) ? holder : holderName;
            DocCompany = company;
            Name = string.IsNullOrWhiteSpace(companyName) ? company : companyName;
        }

        private static string Configured(IConfiguration configuration, string key, string fallback) =>
            configuration[key] is { Length: > 0 } value ? value : fallback;
    }
}
