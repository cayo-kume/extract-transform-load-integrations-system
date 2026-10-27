using System.Globalization;

namespace Domain.LinxMicrovix.Outbound.Extensions
{
    /// <summary>
    /// conversores que mantêm o null da resposta do Microvix, para colunas anuláveis. Os formatos são os da resposta real
    /// (testado em 26/10/2026): datas yyyy-MM-ddTHH:mm:ss, decimais com ponto e BIT como True/False, que a especificação
    /// avisa poder vir também como 0/1 (v274 §4.3). O ToNullableBoolean do Shared não aceita 0/1, por isso o módulo tem os seus
    /// </summary>
    public static class MicrovixConverters
    {
        public static DateTime? ToNullableDateTime(string? value) =>
            DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) && d >= new DateTime(1753, 1, 1) ? d : null;

        public static int? ToNullableInt32(string? value) =>
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : null;

        public static long? ToNullableInt64(string? value) =>
            long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var l) ? l : null;

        public static decimal? ToNullableDecimal(string? value) =>
            decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : null;

        public static bool? ToNullableBoolean(string? value) =>
            value?.Trim() switch
            {
                "1" => true,
                "0" => false,
                _ => bool.TryParse(value, out var b) ? b : null
            };

        public static Guid? ToNullableGuid(string? value) =>
            Guid.TryParse(value, out var g) ? g : null;

        /// <summary>
        /// valores de chave: 0 quando a resposta não traz o valor (o validador já registrou a mensagem e o registro é descartado)
        /// </summary>
        public static int ToInt32(string? value) => ToNullableInt32(value) ?? 0;

        public static long ToInt64(string? value) => ToNullableInt64(value) ?? 0;

        /// <summary>
        /// texto cortado no tamanho da coluna do script; vazio vira null (a coluna é anulável)
        /// </summary>
        public static string? ToNullableString(string? value, int length) =>
            String.IsNullOrEmpty(value) ? null : value.Length > length ? value[..length] : value;
    }
}
