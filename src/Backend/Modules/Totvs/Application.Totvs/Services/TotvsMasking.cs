using System.Text.RegularExpressions;

namespace Application.Totvs.Services
{
    /// <summary>
    /// mascaramento dos dados pessoais no request_body e no response_body de totvs.TotvsRequests (T6): documentos da pessoa física, conta
    /// bancária e chave PIX ficam só com os 2 últimos caracteres. Campo que aceita CPF ou CNPJ só é mascarado quando traz um CPF (11 dígitos):
    /// CNPJ é dado da empresa. Trabalha sobre o texto, sem desserializar, para valer também na resposta cortada em 800 caracteres e nas
    /// mensagens de exceção. A rota e o field_key_value ficam inteiros: são por onde o registro é procurado
    /// </summary>
    public static partial class TotvsMasking
    {
        // documentos da pessoa física, conta e PIX: mascarados sempre
        private static readonly HashSet<string> Always = new(StringComparer.OrdinalIgnoreCase)
        {
            "cpf", "personCpf", "sellerCpf", "rg", "rgIe", "ctps", "ctpsSerial", "pisNumber", "ricNumber", "passportNumber", "foreignDocumentNumber",
            "voterRegistrationNumber", "driverLicenseNumber", "driverLicenseRegistrationNumber", "driverLicenseSecurityNumber", "nitNumber", "cifNumber",
            "accountNumber", "accountDigit", "pixKey"
        };

        // "nome": "texto", "nome": 123 ou "nome": [ "texto", 123, null ] (listas de documentos, como personCpfCnpjList). Lista de objetos não
        // casa aqui: os campos de dentro casam um a um
        [GeneratedRegex(@"""(?<name>[A-Za-z_][A-Za-z0-9_]*)""(?<colon>\s*:\s*)(?<value>""(?:[^""\\]|\\.)*""|-?\d+|\[\s*(?:(?:""(?:[^""\\]|\\.)*""|-?\d+|null)\s*,?\s*)*\])")]
        private static partial Regex Property();

        [GeneratedRegex(@"""(?:[^""\\]|\\.)*""|-?\d+")]
        private static partial Regex Item();

        [GeneratedRegex(@"\D")]
        private static partial Regex NotDigit();

        /// <summary>
        /// request_body e response_body: mascara o valor dos campos pessoais, onde quer que estejam no JSON
        /// </summary>
        public static string? Body(string? content)
        {
            if (String.IsNullOrEmpty(content))
                return content;

            return Property().Replace(content, match =>
            {
                var name = match.Groups["name"].Value;
                var cpfOnly = !Always.Contains(name);

                if (cpfOnly && !IsCpfField(name))
                    return match.Value;

                var value = match.Groups["value"].Value;
                var masked = value.StartsWith('[')
                    ? Item().Replace(value, item => MaskItem(item.Value, cpfOnly))
                    : MaskItem(value, cpfOnly);

                return $"\"{name}\"{match.Groups["colon"].Value}{masked}";
            });
        }

        // campos que aceitam CPF ou CNPJ: *CpfCnpj, *CpfCnpjList e o cpfPartner dos sócios
        private static bool IsCpfField(string name) =>
            name.Contains("cpfcnpj", StringComparison.OrdinalIgnoreCase) || name.Equals("cpfPartner", StringComparison.OrdinalIgnoreCase);

        private static bool IsCpf(string value) =>
            !value.Any(Char.IsLetter) && NotDigit().Replace(value, "").Length == 11;

        private static string MaskItem(string token, bool cpfOnly)
        {
            var quoted = token.StartsWith('"');
            var value = quoted ? token[1..^1] : token;

            if (value.Length == 0 || (cpfOnly && !IsCpf(value)))
                return token;

            return $"\"{Mask(value)}\"";
        }

        // os 2 últimos caracteres ficam, para conferir com quem chamou; valores curtos (dígito da conta) e finais com escape do JSON somem
        // inteiros, para o corpo continuar válido
        private static string Mask(string value) =>
            value.Length <= 4 || value[^2..].IndexOfAny(['\\', '"']) >= 0 ? new string('*', value.Length) : new string('*', value.Length - 2) + value[^2..];
    }
}
