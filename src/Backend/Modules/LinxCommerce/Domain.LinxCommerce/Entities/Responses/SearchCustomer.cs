using Domain.LinxCommerce.Entities.Queue;

namespace Domain.LinxCommerce.Entities.Responses
{
    public class SearchCustomer
    {
        public class Root
        {
            public List<Result> Result { get; set; }
        }

        public class Result
        {
            public int CustomerID { get; set; }
            public string? CustomerType { get; set; }
            public QueueItem? QueueItem { get; set; }
        }
    }
}
