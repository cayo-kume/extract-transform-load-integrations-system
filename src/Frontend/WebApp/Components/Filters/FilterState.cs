using WebApp.Models;

namespace WebApp.Components.Filters
{
    /// <summary>
    /// Valores do componente Filters. Cada mudança gera um objeto novo, para a página comparar e refazer a consulta.
    /// </summary>
    public record FilterState
    {
        public Period Period { get; init; } = Period.Last24Hours;

        /// <summary>
        /// Nulo = todos os tipos.
        /// </summary>
        public int? IdType { get; init; }

        /// <summary>
        /// Nulo = todas as aplicações.
        /// </summary>
        public int? IdApp { get; init; }

        public string? Search { get; init; }

        /// <summary>
        /// O texto da busca aparece em algum dos campos (sem diferenciar maiúsculas); busca vazia aceita tudo.
        /// </summary>
        public bool Matches(params string?[] fields) =>
            string.IsNullOrWhiteSpace(Search)
            || fields.Any(x => x?.Contains(Search.Trim(), StringComparison.InvariantCultureIgnoreCase) == true);
    }
}
