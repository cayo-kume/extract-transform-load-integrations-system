using System.Globalization;
using System.Text.Json;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// regras comuns das escritas de cliente: documento com dígito verificador, data em texto, enum pelo nome e o value dos campos adicionais
    /// </summary>
    public static class CustomerRules
    {
        // "" zera o campo (descrição do swagger): passa
        public static bool IsDate(string? value) =>
            String.IsNullOrEmpty(value) || DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

        public static bool IsEnum(string? value, IReadOnlyList<string> names) =>
            String.IsNullOrEmpty(value) || names.Contains(value);

        public static string Names(IReadOnlyList<string> names) => String.Join(", ", names);

        public static bool OneDefault(IEnumerable<bool?> isDefault) =>
            isDefault.Count(x => x == true) <= 1;

        public static bool IsCpf(string? cpf)
        {
            if (cpf is null || cpf.Length != 11 || !cpf.All(Char.IsAsciiDigit) || cpf.Distinct().Count() == 1)
                return false;

            var d = cpf.Select(c => c - '0').ToArray();
            int Digit(int length) { var sum = 0; for (var i = 0; i < length; i++) sum += d[i] * (length + 1 - i); var r = sum % 11; return r < 2 ? 0 : 11 - r; }

            return Digit(9) == d[9] && Digit(10) == d[10];
        }

        public static bool IsCnpj(string? cnpj)
        {
            if (cnpj is null || cnpj.Length != 14 || !cnpj.All(Char.IsAsciiDigit) || cnpj.Distinct().Count() == 1)
                return false;

            var d = cnpj.Select(c => c - '0').ToArray();
            int Digit(int length) { var sum = 0; for (var i = 0; i < length; i++) sum += d[i] * ((length - 1 - i) % 8 + 2); var r = sum % 11; return r < 2 ? 0 : 11 - r; }

            return Digit(12) == d[12] && Digit(13) == d[13];
        }

        // texto até 80 (inclusive a data DD/MM/YYYY) ou número com até 12 inteiros e 6 decimais
        public static bool IsAdditionalValue(object? value) => value switch
        {
            JsonElement { ValueKind: JsonValueKind.String } e => e.GetString()!.Length <= 80,
            JsonElement { ValueKind: JsonValueKind.Number } e => e.TryGetDecimal(out var n) && IsAdditionalNumber(n),
            string s => s.Length <= 80,
            decimal n => IsAdditionalNumber(n),
            int n => IsAdditionalNumber(n),
            long n => IsAdditionalNumber(n),
            double n => IsAdditionalNumber((decimal)n),
            _ => false
        };

        private static bool IsAdditionalNumber(decimal n) => Math.Abs(n) < 1_000_000_000_000m && n.Scale <= 6;
    }
}
