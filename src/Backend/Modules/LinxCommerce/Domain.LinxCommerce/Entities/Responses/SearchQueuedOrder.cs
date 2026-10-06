using Domain.LinxCommerce.Entities.Queue;

namespace Domain.LinxCommerce.Entities.Responses
{
    public class SearchQueuedOrder
    {
        public class Root
        {
            public List<Result> Result { get; set; }
        }

        public class Result
        {
            public Order.Order.Root? Order { get; set; }
            public QueueItem? QueueItem { get; set; }
        }
    }
}
