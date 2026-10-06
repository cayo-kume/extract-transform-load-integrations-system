namespace Domain.Totvs.Dtos.Person.Branch
{
    public class Root
    {
        public int count { get; init; }
        public int totalPages { get; init; }
        public bool hasNext { get; init; }
        public int totalItems { get; init; }
        public List<Branch> items { get; init; }
    }
}
