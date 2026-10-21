using System.Globalization;

namespace Domain.Shared.Extensions
{
    /// <summary>
    /// conversores que mantêm o null da API, para colunas anuláveis.
    /// Alternativa ao ConverterValidatorsExtensions, que devolve 1990-01-01 / 0 quando o valor vem vazio.
    /// </summary>
    public static class NullableConverters
    {
        public static DateTime? ToNullableDateTime(string? value) =>
            String.IsNullOrWhiteSpace(value) ? null : ConverterValidatorsExtensions.ConvertToDateTimeValidation(value);

        public static int? ToNullableInt32(string? value) =>
            String.IsNullOrWhiteSpace(value) ? null : ConverterValidatorsExtensions.ConvertToInt32Validation(value);

        // InvariantCulture: o Newtonsoft entrega os decimais com ponto (63089.171960499996) e a máquina está em pt-BR
        public static decimal? ToNullableDecimal(string? value) =>
            decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : null;

        // TimeSpan (e não TimeOnly): o SqlBulkCopy converte TimeSpan para a coluna time
        public static TimeSpan? ToNullableTimeSpan(string? value) =>
            TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var t) ? t : null;

        // bool? para campos que a API manda null (ex.: includeInParents); bool comum gravaria false, que é outra informação
        public static bool? ToNullableBoolean(string? value) =>
            String.IsNullOrWhiteSpace(value) ? null : ConverterValidatorsExtensions.ConvertToBooleanValidation(value);
    }
}
