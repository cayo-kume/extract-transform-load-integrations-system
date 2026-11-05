namespace WebApp.Models
{
    /// <summary>
    /// Holding cliente em auditing.Holders (docs/PROPOSTA-SCHEMA-AUDITING-HOLDERS-CATALOG.md).
    /// </summary>
    public class HolderModel
    {
        /// <summary>
        /// CNPJ, só dígitos.
        /// </summary>
        public string Holder { get; set; } = "";

        public string? FantasyName { get; set; }

        /// <summary>
        /// Empresas da holding em auditing.Companies (a própria holding conta).
        /// </summary>
        public int Companies { get; set; }

        public string Label => string.IsNullOrWhiteSpace(FantasyName) ? Holder : FantasyName;
    }

    /// <summary>
    /// Empresa de uma holding em auditing.Companies: o doc_company das tabelas *Parameters e o Company dos jobs. A própria
    /// holding também é uma empresa (Company = Holder), com os jobs dos hosts.
    /// </summary>
    public class CompanyModel
    {
        public string Company { get; set; } = "";

        public string Holder { get; set; } = "";

        public string? FantasyName { get; set; }

        public bool IsHolder => Company == Holder;

        public string Label => string.IsNullOrWhiteSpace(FantasyName) ? Company : FantasyName;

        /// <summary>
        /// IdParent dos módulos com uma linha de *Parameters para esta empresa.
        /// </summary>
        public IReadOnlyList<int> Modules { get; set; } = [];
    }

    /// <summary>
    /// Um módulo do catálogo (auditing.Apps) e a situação dele para a empresa.
    /// </summary>
    public class CompanyAppModel
    {
        public int IdParent { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// Jobs do módulo em auditing.Jobs para a empresa (com as procedures). Zero = nunca contratado.
        /// </summary>
        public int Jobs { get; set; }

        public int ActiveJobs { get; set; }

        /// <summary>
        /// Contratar cria os jobs inativos e remover só os desativa: com jobs e nenhum ativo, o módulo pode estar recém-
        /// contratado ou removido; o banco não distingue os dois.
        /// </summary>
        public bool IsContracted => Jobs > 0;
    }

    /// <summary>
    /// Um job do catálogo (auditing.Catalog) de um módulo, com a agenda sugerida (docs/AGENDA-SUGERIDA-JOBS.csv), que a tela
    /// pode mudar antes de contratar o módulo.
    /// </summary>
    public class CatalogJobModel
    {
        public int IdJob { get; set; }

        public int IdType { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        /// <summary>
        /// Executado sob demanda (controllers): fica em auditing.Jobs com intervalo 0, fora da agenda.
        /// </summary>
        public bool IsOnDemand { get; set; }

        public int IntervalTimeInMinutes { get; set; }

        /// <summary>
        /// Preenchida, o job é diário e roda nessa hora (o intervalo é 1440).
        /// </summary>
        public TimeOnly? RunningDailyAt { get; set; }

        public string Queue { get; set; } = "default";
    }

    /// <summary>
    /// Tipo do campo de parâmetro, para a validação e o campo da tela.
    /// </summary>
    public enum ModuleFieldKind
    {
        Text,
        Url,
        Guid
    }

    /// <summary>
    /// Uma coluna da tabela *Parameters de um módulo. Segredo nunca volta para a tela depois de gravado.
    /// </summary>
    public sealed record ModuleField(string Column, string Label, bool Required, bool Secret, ModuleFieldKind Kind = ModuleFieldKind.Text, int MaxLength = 200);

    /// <summary>
    /// A tabela *Parameters de um módulo e as colunas que a tela edita.
    /// </summary>
    public sealed record ModuleDefinition(int IdParent, string Title, string Table, IReadOnlyList<ModuleField> Fields);

    /// <summary>
    /// A linha de *Parameters da empresa num módulo, como a tela vê: os valores que não são segredo e, dos segredos, só se
    /// estão definidos.
    /// </summary>
    public class ModuleParametersModel
    {
        public int IdParent { get; set; }

        /// <summary>
        /// CNPJ da linha (doc_company): a empresa. Uma linha por CNPJ em cada módulo.
        /// </summary>
        public string DocCompany { get; set; } = "";

        /// <summary>
        /// Valor das colunas que não são segredo, pelo nome da coluna.
        /// </summary>
        public IReadOnlyDictionary<string, string?> Values { get; set; } = new Dictionary<string, string?>();

        /// <summary>
        /// Colunas secretas com valor gravado.
        /// </summary>
        public IReadOnlySet<string> SecretsSet { get; set; } = new HashSet<string>();

        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// O que a tela manda gravar. Segredo vazio ou nulo mantém o valor gravado (o campo só aceita substituir).
    /// </summary>
    public class ModuleParametersInput
    {
        public Dictionary<string, string?> Values { get; set; } = [];
    }

    /// <summary>
    /// Um tipo de host de agenda no ar neste banco (auditing.SchedulingHosts, pelo heartbeat). Datas em UTC.
    /// </summary>
    public class SchedulingHostModel
    {
        public string HostType { get; set; } = "";

        public int Instances { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime LastHeartbeat { get; set; }
    }
}
