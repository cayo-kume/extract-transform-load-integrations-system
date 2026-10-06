namespace Domain.Totvs.Dtos.Seller
{
    public class Root
    {
        public int count { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public int totalItems { get; set; }
        public List<Item> items { get; set; }
    }
}
