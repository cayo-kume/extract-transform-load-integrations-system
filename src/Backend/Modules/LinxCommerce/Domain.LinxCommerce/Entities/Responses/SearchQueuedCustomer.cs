using Domain.LinxCommerce.Entities.Customer;
using Domain.LinxCommerce.Entities.Queue;

namespace Domain.LinxCommerce.Entities.Responses
{
    public class SearchQueuedCustomer
    {
        public class Root
        {
            public List<Result> Result { get; set; }
        }

        public class Result
        {
            public Person? Customer { get; set; }
            public QueueItem? QueueItem { get; set; }
        }
    }
}
