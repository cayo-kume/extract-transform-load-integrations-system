using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Domain.LinxCommerce.Extensions
{
    /// <summary>
    /// conversores do módulo que o Shared não tem. As datas do Linx Commerce vêm com fuso (ISO 8601) ou no formato do WCF
    /// (/Date(1700000000000-0300)/); as duas formas são aceitas, porque nenhuma resposta real foi conferida
    /// (docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 3.2)
    /// </summary>
    public static partial class LinxCommerceConverters
    {
        [GeneratedRegex(@"^/Date\((?<ms>-?\d+)(?<offset>[+-]\d{4})?\)/$")]
        private static partial Regex WcfDate();

        public static DateTimeOffset? ToNullableDateTimeOffset(string? value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            var wcf = WcfDate().Match(value.Trim());

            if (wcf.Success)
            {
                var utc = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(wcf.Groups["ms"].Value, CultureInfo.InvariantCulture));

                if (!wcf.Groups["offset"].Success)
                    return utc;

                var offset = wcf.Groups["offset"].Value;
                var span = new TimeSpan(int.Parse(offset[1..3], CultureInfo.InvariantCulture), int.Parse(offset[3..5], CultureInfo.InvariantCulture), 0);

                return utc.ToOffset(offset[0] == '-' ? -span : span);
            }

            if (!DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                return null;

            // sem fuso no texto vale o de Brasília, e não o do servidor (que pode estar em UTC)
            if (!HasOffset(value))
                parsed = new DateTimeOffset(parsed.DateTime, Brasilia.GetUtcOffset(parsed.DateTime));

            // o "0001-01-01" do .NET é o vazio da API
            return parsed.Year < 1753 ? null : parsed;
        }

        public static long? ToNullableInt64(string? value) =>
            long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var l) ? l : null;

        public static Guid? ToNullableGuid(string? value) =>
            Guid.TryParse(value, out var g) && g != Guid.Empty ? g : null;

        /// <summary>
        /// listas de ids (SkusIDs, WebSites...) gravadas como texto: "1, 2, 3"; outro formato vai como veio
        /// </summary>
        public static string ToText(JToken? value) =>
            value switch
            {
                null or { Type: JTokenType.Null } => String.Empty,
                JArray array => String.Join(", ", array.Select(x => x.Type == JTokenType.String ? (string?)x : x.ToString(Formatting.None))),
                JValue { Type: JTokenType.String } text => (string?)text ?? String.Empty,
                _ => value.ToString(Formatting.None)
            };

        /// <summary>
        /// horário de Brasília de uma data da API: as janelas de leitura são montadas nesse horário, como no código antigo
        /// </summary>
        public static DateTime ToBrasilia(DateTimeOffset value) =>
            TimeZoneInfo.ConvertTime(value, Brasilia).DateTime;

        private static TimeZoneInfo Brasilia =>
            TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        private static bool HasOffset(string value)
        {
            var time = value.IndexOf('T');

            return value.EndsWith('Z') || (time > 0 && (value.IndexOf('+', time) > 0 || value.IndexOf('-', time) > 0));
        }
    }
}
