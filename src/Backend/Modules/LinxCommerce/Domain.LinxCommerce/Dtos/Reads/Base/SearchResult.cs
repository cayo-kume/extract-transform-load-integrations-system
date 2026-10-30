namespace Domain.LinxCommerce.Dtos.Reads.Base
{
    /// <summary>
    /// envelope das buscas do Linx Commerce (Search*): a página e a lista. Só o SearchOrders do código antigo lia o Page;
    /// nas outras buscas o mesmo envelope está a conferir (src/Backend/Modules/LinxCommerce/docs/PROPOSTA-PADRONIZACAO-LINXCOMMERCE.md 4.2, item 4)
    /// </summary>
    public class SearchResult<T>
    {
        public Page? Page { get; init; }
        public List<T>? Result { get; init; }
    }

    public class Page
    {
        public int? Index { get; init; }
        public int? PageCount { get; init; }
        public int? RecordCount { get; init; }
        public int? PageSize { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
