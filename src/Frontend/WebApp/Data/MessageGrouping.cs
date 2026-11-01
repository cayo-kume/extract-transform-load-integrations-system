using System.Globalization;
using System.Text.RegularExpressions;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Agrupamento das mensagens por causa e busca de texto, iguais no mock e no repositório da fase 5. O agrupamento é
    /// feito em C# sobre as mensagens do job no período (não com GROUP BY no SQL): o texto bruto muda a cada execução
    /// (página, CNPJ, timestamp), então a chave troca Guids e números por marcadores.
    /// </summary>
    public static partial class MessageGrouping
    {
        [GeneratedRegex(@"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}")]
        private static partial Regex GuidPattern();

        [GeneratedRegex(@"\d+")]
        private static partial Regex NumberPattern();

        /// <summary>
        /// "Erro da API Totvs em X (empresa 123, página 4)" e "(empresa 456, página 9)" caem na mesma chave.
        /// </summary>
        public static string Key(MessageLevel level, string? message)
        {
            var text = GuidPattern().Replace(message ?? "", "‹id›");
            text = NumberPattern().Replace(text, "#");
            return $"{(int)level}:{text}";
        }

        public static string Key(MessageModel message) => Key(message.Level, message.Message);

        /// <summary>
        /// O texto aparece no campo, sem diferenciar maiúsculas nem acentos ("excecao" acha "Exceção").
        /// </summary>
        public static bool Contains(string? field, string? search) =>
            string.IsNullOrWhiteSpace(search)
            || (field is not null && CultureInfo.InvariantCulture.CompareInfo.IndexOf(field, search.Trim(), CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0);

        /// <summary>
        /// Busca nas mensagens: o texto da mensagem e o da exceção.
        /// </summary>
        public static bool Matches(MessageModel message, string? search) =>
            string.IsNullOrWhiteSpace(search)
            || Contains(message.Message, search)
            || Contains(message.ExceptionMessage, search);

        /// <summary>
        /// Abaixo disso, a busca não vai às mensagens (fica só no IdJob e no título), por causa do custo do LIKE no banco.
        /// </summary>
        public const int MinSearchLength = 3;
    }
}
