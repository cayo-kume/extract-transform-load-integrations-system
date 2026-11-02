using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApp.Components.Assistant
{
    /// <summary>
    /// O pouco de markdown que o assistente usa (negrito, listas com "- " e links), virado em HTML. O texto é codificado
    /// antes de qualquer marcação, e só links para telas do próprio painel (começando com "/") viram &lt;a&gt;: nada que o
    /// modelo escreva vira HTML ou leva para fora.
    /// </summary>
    public static partial class AssistantMarkdown
    {
        public static string ToHtml(string text)
        {
            var html = new StringBuilder();
            var inList = false;

            foreach (var rawLine in text.Replace("\r", "").Split('\n'))
            {
                var line = rawLine.TrimEnd();
                var item = ListItem().Match(line);

                if (item.Success)
                {
                    if (!inList)
                    {
                        html.Append("<ul>");
                        inList = true;
                    }

                    html.Append("<li>").Append(Inline(item.Groups[1].Value)).Append("</li>");
                    continue;
                }

                if (inList)
                {
                    html.Append("</ul>");
                    inList = false;
                }

                html.Append(line.Length == 0 ? "<br>" : $"<p>{Inline(line)}</p>");
            }

            if (inList)
                html.Append("</ul>");

            return html.ToString();
        }

        private static string Inline(string text)
        {
            var encoded = WebUtility.HtmlEncode(text);
            encoded = Bold().Replace(encoded, "<strong>$1</strong>");
            return Link().Replace(encoded, match =>
            {
                var href = WebUtility.HtmlDecode(match.Groups[2].Value);
                return href.StartsWith('/') && !href.StartsWith("//")
                    ? $"<a href=\"{WebUtility.HtmlEncode(href)}\">{match.Groups[1].Value}</a>"
                    : match.Groups[1].Value;
            });
        }

        [GeneratedRegex(@"^\s*(?:[-•*])\s+(.*)$")]
        private static partial Regex ListItem();

        [GeneratedRegex(@"\*\*(.+?)\*\*")]
        private static partial Regex Bold();

        [GeneratedRegex(@"\[([^\]]+)\]\(([^)\s]+)\)")]
        private static partial Regex Link();
    }
}
