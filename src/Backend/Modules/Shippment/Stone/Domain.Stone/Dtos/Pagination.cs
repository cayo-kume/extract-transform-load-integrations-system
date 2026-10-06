namespace Domain.Stone.Dtos
{
    public class Pagination
    {
        public int? total { get; set; }
        public int? lastPage { get; set; }
        public int? currentPage { get; set; }
        public int? nextPage { get; set; }
    }
}
