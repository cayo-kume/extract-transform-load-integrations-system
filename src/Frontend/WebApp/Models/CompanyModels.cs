namespace WebApp.Models
{
    /// <summary>
    /// Empresa do painel (holding) em monitor.Company (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.5).
    /// </summary>
    public class CompanyModel
    {
        /// <summary>
        /// CNPJ, só dígitos: o mesmo doc_company das tabelas *Parameters.
        /// </summary>
        public string Company { get; set; } = "";

        /// <summary>
        /// Razão social.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Nome fantasia.
        /// </summary>
        public string? TradeName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Um CNPJ de parâmetros (marca ou a própria holding) ligado à empresa em monitor.HoldingCompanies, com os módulos
    /// em que ele tem parâmetros.
    /// </summary>
    public class HoldingCompanyModel
    {
        public string Company { get; set; } = "";

        public string? Name { get; set; }

        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// IdParent dos módulos com uma linha de *Parameters para este CNPJ.
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
    /// Uma linha de *Parameters como a tela vê: os valores que não são segredo e, dos segredos, só se estão definidos.
    /// </summary>
    public class ModuleParametersModel
    {
        public int IdParent { get; set; }

        /// <summary>
        /// CNPJ da linha (doc_company): uma linha por CNPJ em cada módulo.
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
        public string DocCompany { get; set; } = "";

        /// <summary>
        /// Nome do CNPJ para monitor.HoldingCompanies (opcional).
        /// </summary>
        public string? DocCompanyName { get; set; }

        public Dictionary<string, string?> Values { get; set; } = [];
    }
}
