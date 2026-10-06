namespace Domain.Totvs.Dtos.Product
{
    public class Root
    {
        public int count { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public int totalItems { get; set; }
        public List<Sku>? items { get; set; }
    }
}
