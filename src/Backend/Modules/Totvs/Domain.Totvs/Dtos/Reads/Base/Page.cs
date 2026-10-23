namespace Domain.Totvs.Dtos.Reads.Base
{
    /// <summary>
    /// envelope de todas as buscas paginadas da Totvs: count é o total da busca e totalItems o total da página
    /// </summary>
    public class Page<T>
    {
        public int count { get; init; }
        public int totalPages { get; init; }
        public bool hasNext { get; init; }
        public int totalItems { get; init; }
        public List<T>? items { get; init; }
    }
}
