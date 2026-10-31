namespace WebApp.Models
{
    /// <summary>
    /// Uma página de uma lista e o total de itens, para as tabelas paginadas.
    /// </summary>
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
}
