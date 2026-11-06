using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// As tabelas *Parameters de cada módulo e as colunas que a tela de cadastro edita, num único lugar
    /// (.docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.5). Cada módulo tem colunas próprias; a fonte é o Script001 de Parameters do
    /// módulo. Os nomes de tabela e coluna daqui entram no SQL do repositório, então só vêm desta lista, nunca da tela.
    /// No Movidesk, AfterSale, LinxMicrovix e Totvs cada credencial tem a gêmea de homologação (hml_), e a aba Parâmetros
    /// edita a Base URL, a Homolog URL e qual das duas os jobs usam (use_homolog_url): marcada a Homolog URL, os jobs usam
    /// ela e a credencial de homologação; senão a Base URL (vazia, o &lt;Módulo&gt;:BaseUrl do appsettings) e a de produção.
    /// </summary>
    public static class ModuleDefinitions
    {
        public const string BaseUrl = "base_url";
        public const string HomologUrl = "homolog_url";
        public const string UseHomologUrl = "use_homolog_url";

        private const string HomologPrefix = "hml_";

        public static IReadOnlyList<ModuleDefinition> All { get; } =
        [
            new(30, "Movidesk", "[movidesk].[MovideskParameters]", WithEnvironments(
            [
                new("token", "Token da API", Required: true, Secret: true, ModuleFieldKind.Guid)
            ])),
            new(50, "AfterSale", "[aftersale].[AfterSaleParameters]", WithEnvironments(
            [
                new("token", "Token da API", Required: true, Secret: true, ModuleFieldKind.Guid)
            ])),
            new(70, "LinxCommerce", "[linxcommerce].[LinxCommerceParameters]",
            [
                new("base_url", "Endereço do ambiente", Required: true, Secret: false, ModuleFieldKind.Url),
                new("user_name", "Usuário da API", Required: true, Secret: false, MaxLength: 100),
                new("api_key", "Chave da API", Required: true, Secret: true)
            ]),
            new(80, "LinxMicrovix", "[linxmicrovix].[LinxMicrovixParameters]", WithEnvironments(
            [
                new("erp_chave", "Chave do WS de saída (ERP)", Required: false, Secret: true, ModuleFieldKind.Guid),
                new("erp_user_name", "Usuário do WS de saída", Required: false, Secret: false, MaxLength: 50),
                new("erp_password", "Senha do WS de saída", Required: false, Secret: true, MaxLength: 50),
                new("b2c_chave", "Chave do WS B2C", Required: false, Secret: true, ModuleFieldKind.Guid),
                new("b2c_user_name", "Usuário do WS B2C", Required: false, Secret: false, MaxLength: 50),
                new("b2c_password", "Senha do WS B2C", Required: false, Secret: true, MaxLength: 50)
            ], shared: [new("grupo", "Grupo (LinxGrupoLojas)", Required: false, Secret: false, MaxLength: 100)])),
            new(130, "Totvs", "[totvs].[TotvsParameters]", WithEnvironments(
            [
                new("client_id", "Client ID", Required: true, Secret: false, MaxLength: 100),
                new("client_secret", "Client secret", Required: true, Secret: true),
                new("username", "Usuário", Required: true, Secret: false, MaxLength: 100),
                new("password", "Senha", Required: true, Secret: true)
            ]))
        ];

        // as credenciais de produção, as gêmeas de homologação (mesmo rótulo; o obrigatório só vale com a Homolog URL marcada,
        // em Validate e HasCredential) e os campos da aba Parâmetros. shared: campos iguais nos dois ambientes
        private static IReadOnlyList<ModuleField> WithEnvironments(IReadOnlyList<ModuleField> credentials, IReadOnlyList<ModuleField>? shared = null) =>
        [
            .. shared ?? [],
            .. credentials,
            .. credentials.Select(x => x with { Column = HomologPrefix + x.Column, Required = false }),
            new(BaseUrl, "Base URL", Required: false, Secret: false, ModuleFieldKind.Url),
            new(HomologUrl, "Homolog URL", Required: false, Secret: false, ModuleFieldKind.Url),
            new(UseHomologUrl, "Usar a Homolog URL", Required: false, Secret: false, MaxLength: 5)
        ];

        public static ModuleDefinition? Find(int idParent) => All.FirstOrDefault(x => x.IdParent == idParent);

        /// <summary>
        /// Base URL, Homolog URL e a escolha entre as duas: ficam na aba Parâmetros, não na Credenciais. O endereço obrigatório
        /// do LinxCommerce é parte da credencial e continua lá.
        /// </summary>
        public static bool IsApiUrl(ModuleField field) => !field.Required && field.Column is BaseUrl or HomologUrl or UseHomologUrl;

        /// <summary>
        /// Credencial de homologação (hml_), a gêmea de uma de produção.
        /// </summary>
        public static bool IsHomolog(ModuleField field) => field.Column.StartsWith(HomologPrefix, StringComparison.Ordinal);

        /// <summary>
        /// O módulo tem credencial de homologação e a escolha da URL (aba Parâmetros).
        /// </summary>
        public static bool HasEnvironments(ModuleDefinition definition) => definition.Fields.Any(IsHomolog);

        /// <summary>
        /// A credencial de produção de uma de homologação.
        /// </summary>
        public static ModuleField Production(ModuleDefinition definition, ModuleField homolog) =>
            definition.Fields.First(x => x.Column == homolog.Column[HomologPrefix.Length..]);

        /// <summary>
        /// A Homolog URL está marcada: o bit chega como 1/0 (tela, mock) ou True/False (banco).
        /// </summary>
        public static bool UsesHomolog(IReadOnlyDictionary<string, string?>? values) =>
            values?.GetValueOrDefault(UseHomologUrl)?.Trim() is { } value && (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// A empresa tem credencial gravada no ambiente que os jobs usam (a Homolog URL marcada ou não): os campos
        /// obrigatórios preenchidos e, no LinxMicrovix (que não tem obrigatório), a chave do WS de saída ou a do B2C. Sem ela, os
        /// jobs do módulo não são ativados.
        /// </summary>
        public static bool HasCredential(ModuleDefinition definition, ModuleParametersModel? parameters) =>
            HasCredential(definition, parameters, UsesHomolog(parameters?.Values));

        /// <summary>
        /// A credencial de um dos ambientes está completa (produção ou homologação).
        /// </summary>
        public static bool HasCredential(ModuleDefinition definition, ModuleParametersModel? parameters, bool homolog)
        {
            if (parameters is null)
                return false;

            string Column(string column) => homolog ? HomologPrefix + column : column;

            bool IsSet(ModuleField field) => field.Secret
                ? parameters.SecretsSet.Contains(Column(field.Column))
                : !string.IsNullOrWhiteSpace(parameters.Values.GetValueOrDefault(Column(field.Column)));

            var production = definition.Fields.Where(x => !IsHomolog(x) && !IsApiUrl(x)).ToList();

            if (!production.Where(x => x.Required).All(IsSet))
                return false;

            return definition.IdParent != 80 || production.Where(x => x.Column is "erp_chave" or "b2c_chave").Any(IsSet);
        }

        public static ModuleDefinition Get(int idParent) =>
            Find(idParent) ?? throw new CompanyRegistrationException($"O módulo {idParent} não tem parâmetros editáveis pelo painel.");

        /// <summary>
        /// CNPJ só com os dígitos (aceita com pontos, barra e traço); vazio se não sobrar nenhum.
        /// </summary>
        public static string Digits(string? value) => new((value ?? "").Where(char.IsAsciiDigit).ToArray());

        /// <summary>
        /// Rótulo do campo nas mensagens: o de homologação diz que é de homologação.
        /// </summary>
        public static string Label(ModuleField field) => IsHomolog(field) ? $"{field.Label} de homologação" : field.Label;

        /// <summary>
        /// Confere os valores contra a definição do módulo antes de gravar. <paramref name="existing"/> diz quais segredos
        /// já estão gravados (um segredo vazio mantém o gravado). Com a Homolog URL marcada, ela e as credenciais de
        /// homologação que são obrigatórias em produção passam a ser obrigatórias. Devolve a primeira mensagem de erro, ou null.
        /// </summary>
        public static string? Validate(ModuleDefinition definition, ModuleParametersInput input, IReadOnlySet<string> existing)
        {
            foreach (var key in input.Values.Keys)
            {
                if (definition.Fields.All(x => x.Column != key))
                    return $"O campo {key} não existe em {definition.Title}.";
            }

            var homolog = UsesHomolog(input.Values);

            if (input.Values.GetValueOrDefault(UseHomologUrl)?.Trim() is { Length: > 0 } use && use is not ("0" or "1") && !bool.TryParse(use, out _))
                return "A escolha da URL precisa ser 0 ou 1.";

            if (homolog && string.IsNullOrWhiteSpace(input.Values.GetValueOrDefault(HomologUrl)))
                return "Preencha a Homolog URL para marcá-la.";

            foreach (var field in definition.Fields.Where(x => x.Column != UseHomologUrl))
            {
                var value = input.Values.GetValueOrDefault(field.Column)?.Trim();
                var required = field.Required || (homolog && IsHomolog(field) && Production(definition, field).Required);

                if (string.IsNullOrEmpty(value))
                {
                    if (required && !(field.Secret && existing.Contains(field.Column)))
                        return $"{Label(field)} é obrigatório.";

                    continue;
                }

                if (value.Length > field.MaxLength)
                    return $"{Label(field)} passa de {field.MaxLength} caracteres.";

                if (field.Kind == ModuleFieldKind.Guid && !Guid.TryParse(value, out _))
                    return $"{Label(field)} precisa ser um GUID (00000000-0000-0000-0000-000000000000).";

                if (field.Kind == ModuleFieldKind.Url && !(Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps))
                    return $"{Label(field)} precisa ser um endereço https://.";
            }

            if (homolog && definition.IdParent == 80
                && !new[] { "hml_erp_chave", "hml_b2c_chave" }.Any(x => !string.IsNullOrWhiteSpace(input.Values.GetValueOrDefault(x)) || existing.Contains(x)))
                return "Com a Homolog URL marcada, grave a chave do WS de saída ou a do B2C de homologação.";

            return null;
        }
    }
}
