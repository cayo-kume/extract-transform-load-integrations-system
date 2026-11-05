using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// As tabelas *Parameters de cada módulo e as colunas que a tela de cadastro edita, num único lugar
    /// (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.5). Cada módulo tem colunas próprias; a fonte é o Script001 de Parameters do
    /// módulo. Os nomes de tabela e coluna daqui entram no SQL do repositório, então só vêm desta lista, nunca da tela.
    /// </summary>
    public static class ModuleDefinitions
    {
        public static IReadOnlyList<ModuleDefinition> All { get; } =
        [
            new(30, "Movidesk", "[movidesk].[MovideskParameters]",
            [
                new("token", "Token da API", Required: true, Secret: true, ModuleFieldKind.Guid),
                new("base_url", "Base URL", Required: false, Secret: false, ModuleFieldKind.Url)
            ]),
            new(50, "AfterSale", "[aftersale].[AfterSaleParameters]",
            [
                new("token", "Token da API", Required: true, Secret: true, ModuleFieldKind.Guid),
                new("base_url", "Base URL", Required: false, Secret: false, ModuleFieldKind.Url)
            ]),
            new(70, "LinxCommerce", "[linxcommerce].[LinxCommerceParameters]",
            [
                new("base_url", "Endereço do ambiente", Required: true, Secret: false, ModuleFieldKind.Url),
                new("user_name", "Usuário da API", Required: true, Secret: false, MaxLength: 100),
                new("api_key", "Chave da API", Required: true, Secret: true)
            ]),
            new(80, "LinxMicrovix", "[linxmicrovix].[LinxMicrovixParameters]",
            [
                new("grupo", "Grupo (LinxGrupoLojas)", Required: false, Secret: false, MaxLength: 100),
                new("erp_chave", "Chave do WS de saída (ERP)", Required: false, Secret: true, ModuleFieldKind.Guid),
                new("erp_user_name", "Usuário do WS de saída", Required: false, Secret: false, MaxLength: 50),
                new("erp_password", "Senha do WS de saída", Required: false, Secret: true, MaxLength: 50),
                new("b2c_chave", "Chave do WS B2C", Required: false, Secret: true, ModuleFieldKind.Guid),
                new("b2c_user_name", "Usuário do WS B2C", Required: false, Secret: false, MaxLength: 50),
                new("b2c_password", "Senha do WS B2C", Required: false, Secret: true, MaxLength: 50),
                new("base_url", "Base URL", Required: false, Secret: false, ModuleFieldKind.Url)
            ]),
            new(130, "Totvs", "[totvs].[TotvsParameters]",
            [
                new("client_id", "Client ID", Required: true, Secret: false, MaxLength: 100),
                new("client_secret", "Client secret", Required: true, Secret: true),
                new("username", "Usuário", Required: true, Secret: false, MaxLength: 100),
                new("password", "Senha", Required: true, Secret: true),
                new("base_url", "Base URL", Required: false, Secret: false, ModuleFieldKind.Url)
            ])
        ];

        public static ModuleDefinition? Find(int idParent) => All.FirstOrDefault(x => x.IdParent == idParent);

        /// <summary>
        /// A empresa tem credencial gravada no módulo: os campos obrigatórios preenchidos e, no LinxMicrovix (que não tem
        /// obrigatório), a chave do WS de saída ou a do B2C. Sem ela, os jobs do módulo não são ativados.
        /// </summary>
        public static bool HasCredential(ModuleDefinition definition, ModuleParametersModel? parameters)
        {
            if (parameters is null)
                return false;

            bool IsSet(ModuleField field) =>
                field.Secret ? parameters.SecretsSet.Contains(field.Column) : !string.IsNullOrWhiteSpace(parameters.Values.GetValueOrDefault(field.Column));

            if (!definition.Fields.Where(x => x.Required).All(IsSet))
                return false;

            return definition.IdParent != 80 || definition.Fields.Where(x => x.Column is "erp_chave" or "b2c_chave").Any(IsSet);
        }

        public static ModuleDefinition Get(int idParent) =>
            Find(idParent) ?? throw new CompanyRegistrationException($"O módulo {idParent} não tem parâmetros editáveis pelo painel.");

        /// <summary>
        /// CNPJ só com os dígitos (aceita com pontos, barra e traço); vazio se não sobrar nenhum.
        /// </summary>
        public static string Digits(string? value) => new((value ?? "").Where(char.IsAsciiDigit).ToArray());

        /// <summary>
        /// Confere os valores contra a definição do módulo antes de gravar. <paramref name="existing"/> diz quais segredos
        /// já estão gravados (um segredo vazio mantém o gravado). Devolve a primeira mensagem de erro, ou null.
        /// </summary>
        public static string? Validate(ModuleDefinition definition, ModuleParametersInput input, IReadOnlySet<string> existing)
        {
            foreach (var key in input.Values.Keys)
            {
                if (definition.Fields.All(x => x.Column != key))
                    return $"O campo {key} não existe em {definition.Title}.";
            }

            foreach (var field in definition.Fields)
            {
                var value = input.Values.GetValueOrDefault(field.Column)?.Trim();

                if (string.IsNullOrEmpty(value))
                {
                    if (field.Required && !(field.Secret && existing.Contains(field.Column)))
                        return $"{field.Label} é obrigatório.";

                    continue;
                }

                if (value.Length > field.MaxLength)
                    return $"{field.Label} passa de {field.MaxLength} caracteres.";

                if (field.Kind == ModuleFieldKind.Guid && !Guid.TryParse(value, out _))
                    return $"{field.Label} precisa ser um GUID (00000000-0000-0000-0000-000000000000).";

                if (field.Kind == ModuleFieldKind.Url && !(Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps))
                    return $"{field.Label} precisa ser um endereço https://.";
            }

            return null;
        }
    }
}
