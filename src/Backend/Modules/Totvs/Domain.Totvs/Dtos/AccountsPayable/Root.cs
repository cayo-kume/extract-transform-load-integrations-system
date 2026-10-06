namespace Domain.Totvs.Dtos.AccountsPayable
{
    public class Root
    {
        public int count { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public int totalItems { get; set; }
        public Duplicate[] items { get; set; }
    }
}
