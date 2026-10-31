namespace WebApp.Models
{
    /// <summary>
    /// O que o comando da tela Executar Querys faz, pelas palavras-chave do SQL.
    /// </summary>
    public enum QueryKind
    {
        Select,
        Write,
        Ddl,
        Execute
    }

    /// <summary>
    /// Resultado de um comando da tela Executar Querys.
    /// </summary>
    public class QueryResult
    {
        public QueryKind Kind { get; set; }

        /// <summary>
        /// Preenchido quando o comando alterou algo e a transação espera "Confirmar" ou "Desfazer".
        /// </summary>
        public Guid? TransactionId { get; set; }

        /// <summary>
        /// O próprio comando (uma procedure, por exemplo) já fez COMMIT ou ROLLBACK, e não há o que desfazer.
        /// </summary>
        public bool TransactionClosedByCommand { get; set; }

        public IReadOnlyList<string> Columns { get; set; } = [];
        public IReadOnlyList<object?[]> Rows { get; set; } = [];

        /// <summary>
        /// Linhas que o SELECT devolveu; Rows traz no máximo IQueryRunner.MaxRows delas.
        /// </summary>
        public int TotalRows { get; set; }

        public bool Truncated => TotalRows > Rows.Count;

        public int? RowsAffected { get; set; }

        /// <summary>
        /// PRINT e avisos do SQL Server.
        /// </summary>
        public IReadOnlyList<string> Messages { get; set; } = [];

        /// <summary>
        /// Erro do SQL Server; com ele, o resto do resultado fica vazio.
        /// </summary>
        public string? Error { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
