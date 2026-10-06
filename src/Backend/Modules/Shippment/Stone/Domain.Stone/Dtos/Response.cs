namespace Domain.Stone.Dtos
{
    public class Response
    {
        public List<Order> data { get; set; }
        public Pagination pagination { get; set; }
    }
}
