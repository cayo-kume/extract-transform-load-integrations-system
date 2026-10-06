namespace Domain.Totvs.Dtos.AccountsPayable
{
    public class ComissionPaidRoot
    {
        public int count { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public int totalItems { get; set; }
        public ComissionPaid[] items { get; set; }
    }
}
