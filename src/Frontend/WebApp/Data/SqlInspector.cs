using System.Text.RegularExpressions;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Leitura das palavras-chave do SQL da tela Executar Querys, sem comentários e sem o texto entre aspas, para decidir
    /// o que pede confirmação e o que ganha o segundo aviso.
    /// </summary>
    public static partial class SqlInspector
    {
        public static QueryKind Kind(string sql)
        {
            var text = Normalize(sql);

            if (Execute().IsMatch(text))
                return QueryKind.Execute;

            if (Ddl().IsMatch(text))
                return QueryKind.Ddl;

            if (Write().IsMatch(text))
                return QueryKind.Write;

            return QueryKind.Select;
        }

        /// <summary>
        /// Comando que altera dados, objetos ou executa procedure: pede confirmação antes e roda numa transação.
        /// </summary>
        public static bool ChangesSomething(string sql) => Kind(sql) != QueryKind.Select;

        /// <summary>
        /// Motivos do segundo aviso: UPDATE ou DELETE sem WHERE, DROP e TRUNCATE.
        /// </summary>
        public static IReadOnlyList<string> Dangers(string sql)
        {
            var dangers = new List<string>();

            foreach (var statement in Statements().Split(Normalize(sql)).Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var hasWhere = Where().IsMatch(statement);

                if (Update().IsMatch(statement) && !hasWhere)
                    dangers.Add("UPDATE sem WHERE: altera todas as linhas da tabela.");

                if (Delete().IsMatch(statement) && !hasWhere)
                    dangers.Add("DELETE sem WHERE: apaga todas as linhas da tabela.");

                if (Drop().IsMatch(statement))
                    dangers.Add("DROP: apaga o objeto do banco.");

                if (Truncate().IsMatch(statement))
                    dangers.Add("TRUNCATE: apaga todas as linhas da tabela.");
            }

            return dangers.Distinct().ToList();
        }

        /// <summary>
        /// O SQL sem comentários e com o texto entre aspas esvaziado, para as palavras-chave dentro deles não contarem.
        /// </summary>
        public static string Normalize(string sql)
        {
            var text = BlockComment().Replace(sql, " ");
            text = LineComment().Replace(text, " ");
            return Literal().Replace(text, "''");
        }

        [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
        private static partial Regex BlockComment();

        [GeneratedRegex(@"--[^\r\n]*")]
        private static partial Regex LineComment();

        [GeneratedRegex(@"'(?:[^']|'')*'")]
        private static partial Regex Literal();

        [GeneratedRegex(@";|^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
        private static partial Regex Statements();

        [GeneratedRegex(@"\b(EXEC|EXECUTE)\b", RegexOptions.IgnoreCase)]
        private static partial Regex Execute();

        [GeneratedRegex(@"\b(CREATE|ALTER|DROP|TRUNCATE)\b", RegexOptions.IgnoreCase)]
        private static partial Regex Ddl();

        [GeneratedRegex(@"\b(INSERT|UPDATE|DELETE|MERGE)\b", RegexOptions.IgnoreCase)]
        private static partial Regex Write();

        [GeneratedRegex(@"\bWHERE\b", RegexOptions.IgnoreCase)]
        private static partial Regex Where();

        [GeneratedRegex(@"\bUPDATE\b", RegexOptions.IgnoreCase)]
        private static partial Regex Update();

        [GeneratedRegex(@"\bDELETE\b", RegexOptions.IgnoreCase)]
        private static partial Regex Delete();

        [GeneratedRegex(@"\bDROP\b", RegexOptions.IgnoreCase)]
        private static partial Regex Drop();

        [GeneratedRegex(@"\bTRUNCATE\b", RegexOptions.IgnoreCase)]
        private static partial Regex Truncate();
    }
}
